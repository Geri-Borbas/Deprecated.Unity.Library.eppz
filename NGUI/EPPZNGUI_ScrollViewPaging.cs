using UnityEngine;
using System.Collections.Generic;


namespace EPPZ.NGUI
{


	/// <summary>
	/// Inspired by `UICenterOnChild`, but implemented more like iOS `UIScrollView` paging.
	/// The concept `normalizedContentPosition` is borrowed from uGUI `ScrollRect`.
	/// Add to the `UIScrollView` to be paged, and the rest is just basically happens.
	/// SpringPanel onFinished did not hooked up.
	/// </summary>


	[AddComponentMenu("eppz!/NGUI/Scroll View Paging")]
	public class EPPZNGUI_ScrollViewPaging : MonoBehaviour
	{


		/// <summary>
		/// Layout modes to use when scrolling to page positions.
		/// `Instant` mode lays out on the current frame immediately, 
		/// while `Animated` mode uses NGUI `SpringPanel`.
		/// </summary>
		public enum LayoutMode { Instant, Animated }

		/// <summary>
		/// UIScrollView to control (on this GameObject).
		/// </summary>
		public UIScrollView _scrollView = null;
		public UIScrollView scrollView
		{
			get
			{
				if (_scrollView == null)
				{
					_scrollView = GetComponent<UIScrollView>();
					if (_scrollView == null)
					{
						Debug.LogWarning("`" + GetType() + "` requires `" + typeof(UIScrollView) + "` on this object in order to work.", this);
						enabled = false;
						return null;
					}
					AddScrollViewListeners();
				}
				return _scrollView;
			}
		}

		/// <summary>
		/// Content Widget to scroll (for paging calculations). Not always the same as
		/// the cummulative bounds of every children (think of pull-to-refresh controls).
		/// </summary>
		public UIWidget content;

		// Paging.
		public float springStrength = 8.0f;
		[Range (0,1)] public float snapDistance = 1.0f;

		// Sizes.
		public Vector2 pageSize;
		public Vector2 contentSize;
		public Vector2 scrollSize;
		public Vector2 normalizedPageSize;
		public Vector2 normalizedPageSize_half;

		// Positions.
		public Vector2 contentPosition;
		public Vector2 normalizedContentPosition;

		public delegate void OnPageIndexChange(EPPZNGUI_ScrollViewPaging scrollViewPaging);
		public OnPageIndexChange onPageIndexChange;

		private bool pageIndicesChanged;
		public int horizontalPageIndex;
		public int verticalPageIndex;
		private int _previousHorizontalPageIndex = -1; // Track change
		private int _previousVerticalPageIndex = -1; // Track change
		
		public float horizontalPageCount;
		public float verticalPageCount;
		private float _previousHorizontalPageCount = -1; // Track change
		private float _previousVerticalPageCount = -1; // Track change

		// Touch values.
		private Vector2 touchContentPosition;
		private Vector2 normalizedTouchContentPosition;
		private float touchTime;

		// Flicking.
		private float flickDistance = 10.0f;
		private float flickTime = 0.3f;
		private int horizontalPageIndex_Flick = 0;
		private int verticalPageIndex_Flick = 0;

		// Snapping.
		private int horizontalPageIndex_Snap = 0;
		private int verticalPageIndex_Snap = 0;

		// Page Control.
		[HideInInspector] public EPPZNGUI_ScrollViewPageControl pageControl; // Populated externally by Page Control


		#region Delegates
		
		void Start()
		{
			// Add listeners removed by `OnDestroy()` calls on Play / Stop Mode changes.
			AddScrollViewListeners();
		}
		
		void OnDestroy()
		{
			// Remove listeners when component gets removed from the inspector (also at Play / Stop Mode changes).
			RemoveScrollViewListeners();
		}
		
		void AddScrollViewListeners()
		{
			if (scrollView == null) return;
			scrollView.onDragFinished += OnDragFinished;
			scrollView.onDragStarted += OnDragStarted;

			if (scrollView.panel != null)
			{ scrollView.panel.onClipMove += OnScroll; }

			if (scrollView.horizontalScrollBar != null)
			{ scrollView.horizontalScrollBar.onDragFinished += OnScrollBarDragFinished; }
			
			if (scrollView.verticalScrollBar != null)
			{ scrollView.verticalScrollBar.onDragFinished += OnScrollBarDragFinished; }
		}
		
		void RemoveScrollViewListeners()
		{
			if (scrollView == null) return;
			scrollView.onDragFinished -= OnDragFinished;
			scrollView.onDragStarted -= OnDragStarted;
			
			if (scrollView.panel != null)
			{ scrollView.panel.onClipMove -= OnScroll; }
			
			if (scrollView.horizontalScrollBar != null)
			{ scrollView.horizontalScrollBar.onDragFinished -= OnScrollBarDragFinished; }
			
			if (scrollView.verticalScrollBar != null)
			{ scrollView.verticalScrollBar.onDragFinished -= OnScrollBarDragFinished; }
		}
		
		#endregion


		#region Events

		void OnDragStarted()
		{
			if (enabled == false) return; // Only if enabled
			
			// Track touch position for drag direction measurement later on.
			CalculatePositions();
			
			// Capture.
			touchTime = Time.time;
			touchContentPosition = contentPosition;
			normalizedTouchContentPosition = normalizedContentPosition;
		}
		
		void OnScrollBarDragFinished()
		{ Layout(); }
		
		void OnDragFinished()
		{ LayoutOnTouchFinished(); }

		void OnScroll(UIPanel panel)
		{
			// Calculate indices only, assuming no changes in dimensions during scroll.
			CalculatePositions();
			CalculatePageIndices();

			LookUpPageIndexChange();
			LayoutPageControlPageIndexIfNeeded();
		}

		#endregion


		#region Paging
	
		/// <summary>
		/// Something like `UICenterOnChild.Recenter`.
		/// Snap `UIScrollView` position to the nearest page position.
		/// </summary>

		[ContextMenu("Execute")]
		public void Layout()
		{
			if (enabled == false) return; // Only if enabled

			// Precalculations.
			CalculateSizes();
			CalculatePageDimensions();
			CalculatePageCounts();
			CalculatePositions();
			
			// Calculate desired page position.
			CalculatePageIndices();
			ClampPageIndices();
			
			// Apply the calculated value.
			ScrollToPageIndices(LayoutMode.Instant);
		}

		void LayoutOnTouchFinished() // Always animated
		{ 
			if (enabled == false) return; // Only if enabled
			
			// Precalculations.
			CalculateSizes();
			CalculatePageDimensions();
			CalculatePageCounts();
			CalculatePositions();

			// Modifiers.
			bool isFlick = LookupFlick();
			if (isFlick == false)
			{ LookupSnap(); }
			
			// Calculate desired page position.
			CalculatePageIndices();
			horizontalPageIndex += (int)Mathf.Clamp(horizontalPageIndex_Flick + horizontalPageIndex_Snap, -1, 1);
			verticalPageIndex += (int)Mathf.Clamp(verticalPageIndex_Flick + verticalPageIndex_Snap, -1, 1);
			ClampPageIndices();

			// Apply the calculated value.
			ScrollToPageIndices(LayoutMode.Animated);
		}

		void LayoutWithPageIndexModifier(int modifier) // Modifier should be either -1 or 1
		{ 
			if (enabled == false) return; // Only if enabled
			
			// Precalculations.
			CalculateSizes();
			CalculatePageDimensions();
			CalculatePageCounts();
			CalculatePositions();
			
			// Calculate desired page position.
			CalculatePageIndices();
			horizontalPageIndex += modifier;
			verticalPageIndex += modifier;
			ClampPageIndices();
			
			// Apply the calculated value.
			ScrollToPageIndices(LayoutMode.Animated);
		}

		void ScrollToPageIndices(LayoutMode layoutMode)
		{
			// The result (paged scroll position).
			Vector2 pagedContentPosition = ContentPositionForPageIndices(horizontalPageIndex, verticalPageIndex);
			float localOffset_x = - (pagedContentPosition.x - contentPosition.x); // Straight
			float localOffset_y = + (pagedContentPosition.y - contentPosition.y); // Flipped

			// Clamp.
			if (scrollView.movement == UIScrollView.Movement.Horizontal) localOffset_y = 0.0f;
			if (scrollView.movement == UIScrollView.Movement.Vertical) localOffset_x = 0.0f;

			// Scroll amount.
			Vector3 localOffset = new Vector3(
				localOffset_x,
				localOffset_y,
				0.0f
				);

			// Apply.
			if (layoutMode == LayoutMode.Animated &&
			    springStrength != 0.0f) // Another way to opt-out spring motion (mainly for debugging)
			{
				SpringPanel.Begin(
					scrollView.panel.cachedGameObject,
					scrollView.panel.cachedTransform.localPosition + localOffset,
					springStrength
					);
			}
			
			else
			{
				scrollView.MoveRelative(localOffset);
				scrollView.momentumAmount = 0.0f; // No spring after release
			}
		}

		bool LookupFlick()
		{
			// Flick distance based on configured drag treshold.
			if(SystemInfo.deviceType == DeviceType.Desktop)
			{ flickDistance = UICamera.current.mouseDragThreshold * 2.0f; }
			if (SystemInfo.deviceType == DeviceType.Handheld)
			{ flickDistance = UICamera.current.touchDragThreshold * 2.0f; }

			// Offset.
			Vector2 touchOffset = contentPosition - touchContentPosition;
			int sign_x = (int)Mathf.Sign(touchOffset.x);
			int sign_y = (int)Mathf.Sign(touchOffset.y);
			
			// Modifiers.
			horizontalPageIndex_Flick = 0;
			verticalPageIndex_Flick = 0;
			horizontalPageIndex_Snap = 0;
			verticalPageIndex_Snap = 0;
			
			// Lookup flick.
			float timeDistance = Time.time - touchTime;
			bool isHorizontalFlick = (Mathf.Abs(touchOffset.x) > flickDistance) && (timeDistance < flickTime);
			bool isVerticalFlick = (Mathf.Abs(touchOffset.y) > flickDistance) && (timeDistance < flickTime);
			bool isFlick = (isHorizontalFlick || isVerticalFlick);
			horizontalPageIndex_Flick = (isHorizontalFlick) ? sign_x : 0;
			verticalPageIndex_Flick = (isVerticalFlick) ? sign_y : 0;

			return isFlick;
		}

		void LookupSnap()
		{
			// Lookup snap distance (if any set).
			if (snapDistance < 1.0f)
			{
				// Offset.
				Vector2 normalizedPositionOffset = normalizedContentPosition - normalizedTouchContentPosition;
				int sign_x = (int)Mathf.Sign(normalizedPositionOffset.x);
				int sign_y = (int)Mathf.Sign(normalizedPositionOffset.y);

				bool isHorizontalSnap = (
					(Mathf.Abs(normalizedPositionOffset.x) < normalizedPageSize_half.x) && // Below the ususal half paging distance
					(Mathf.Abs(normalizedPositionOffset.x) >= (normalizedPageSize_half.x * snapDistance)) // Above the desired snap treshold
					);
				bool isVerticalSnap = (
					(Mathf.Abs(normalizedPositionOffset.y) < normalizedPageSize_half.y) && // Below the ususal half paging distance
					(Mathf.Abs(normalizedPositionOffset.y) >= (normalizedPageSize_half.y * snapDistance)) // Above the desired snap treshold
					);
				horizontalPageIndex_Snap = (isHorizontalSnap) ? sign_x : 0;
				verticalPageIndex_Snap = (isVerticalSnap) ? sign_y : 0;
			}
		}

		void CalculatePositions()
		{
			contentPosition = new Vector2(
				+ scrollView.panel.clipOffset.x, // Straight
				- scrollView.panel.clipOffset.y  // Flipped
				);
			normalizedContentPosition = new Vector2(
				(contentSize.x > pageSize.x) ? contentPosition.x / scrollSize.x : 0.0f,
				(contentSize.y > pageSize.y) ? contentPosition.y / scrollSize.y : 0.0f
				);
		}

		void CalculateSizes()
		{
			pageSize = scrollView.panel.GetViewSize();
			contentSize = content.localSize;
			scrollSize = new Vector2(
				(contentSize.x - pageSize.x),
				(contentSize.y - pageSize.y)
				);
		}

		void CalculatePageDimensions()
		{	
			normalizedPageSize = new Vector2(
				(scrollSize.x <= 0.0f) ? 0.0f : pageSize.x / scrollSize.x,
				(scrollSize.y <= 0.0f) ? 0.0f : pageSize.y / scrollSize.y
				);
			normalizedPageSize_half = new Vector2(normalizedPageSize.x / 2.0f, normalizedPageSize.y / 2.0f);
		}

		void CalculatePageIndices()
		{
			horizontalPageIndex = (scrollSize.x <= 0.0f) ? 0 : Mathf.FloorToInt((normalizedContentPosition.x + (normalizedPageSize_half.x)) / normalizedPageSize.x);
			verticalPageIndex = (scrollSize.y <= 0.0f) ? 0 : Mathf.FloorToInt((normalizedContentPosition.y + (normalizedPageSize_half.y)) / normalizedPageSize.y);
		}

		void CalculatePageCounts()
		{
			horizontalPageCount = Mathf.Floor(scrollSize.x / pageSize.x);
			verticalPageCount = Mathf.Floor(scrollSize.y / pageSize.y);
		}

		void ClampPageIndices()
		{
			horizontalPageIndex = Mathf.FloorToInt(Mathf.Clamp((float)horizontalPageIndex, 0.0f, horizontalPageCount));
			verticalPageIndex = Mathf.FloorToInt(Mathf.Clamp((float)verticalPageIndex, 0.0f, verticalPageCount));
		}

		Vector2 ContentPositionForPageIndices(int horizontalPageIndex_, int verticalPageIndex_)
		{
			return new Vector2(
				pageSize.x * (float)horizontalPageIndex_,
				pageSize.y * (float)verticalPageIndex_
				);
		}

		#endregion


		#region Delegate events

		void LookUpPageIndexChange()
		{
			pageIndicesChanged = ((horizontalPageIndex != _previousHorizontalPageIndex) || (verticalPageIndex != _previousVerticalPageIndex));
			if (pageIndicesChanged)
			{
				// Delegate call.
				if (onPageIndexChange != null) onPageIndexChange(this);

				// Track.
				_previousHorizontalPageIndex = horizontalPageIndex;
				_previousVerticalPageIndex = verticalPageIndex;
			}
		}

		#endregion


		#region PageControl events

		public void OnPageControlPreviousPress()
		{ LayoutWithPageIndexModifier(-1); }

		public void OnPageControlNextPress()
		{ LayoutWithPageIndexModifier(+1); }

		#endregion


		#region PageControl layout

		public void LayoutPageControl() // Called by Page Control upon Start once it is hooked up to this Scroll View (a bit reverse, but less property accessor code)
		{
			// Necessary precalculations.
			CalculateSizes();
			CalculatePageCounts();
			CalculatePageIndices();

			// Force layout.
			LayoutPageControlPageCount();
			LayoutPageControlPageIndex();
		}
		
		void LayoutPageControlPageCountIfNeeded()
		{
			// Layout Page Control if count changed.
			bool pageCountChanged = ((horizontalPageCount != _previousHorizontalPageCount) || (verticalPageCount != _previousVerticalPageCount));
			if (pageCountChanged)
			{
				LayoutPageControlPageCount();
				
				// Track.
				_previousHorizontalPageCount = horizontalPageCount;
				_previousVerticalPageCount = verticalPageCount;
			}
		}

		void LayoutPageControlPageCount()
		{
			if (pageControl == null) return; // Only having Page Control

			// Assume either horizontal or vertical movement.
			int pageCount = (scrollView.movement == UIScrollView.Movement.Horizontal) ? Mathf.FloorToInt(horizontalPageCount) : Mathf.FloorToInt(verticalPageCount);
			pageCount++; // `EPPZNGUI_ScrollViewPaging` uses 0 based `pageCount` values for easier `pageIndex` interaction
			pageControl.LayoutPageCount(pageCount);
		}

		void LayoutPageControlPageIndexIfNeeded()
		{
			if (pageIndicesChanged == false) return; // Only if changed
			if (pageControl == null) return; // Only having any `PageControl`
			
			LayoutPageControlPageIndex();
		}

		void LayoutPageControlPageIndex()
		{
			if (pageControl == null) return; // Only having Page Control

			// Assume either horizontal or vertical movement.
			int pageIndex = (scrollView.movement == UIScrollView.Movement.Horizontal) ? horizontalPageIndex : verticalPageIndex;
			pageControl.LayoutPageIndex(pageIndex);
		}

		#endregion
	}
}
