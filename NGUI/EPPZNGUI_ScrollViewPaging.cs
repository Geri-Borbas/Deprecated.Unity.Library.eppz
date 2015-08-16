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
		
		public int horizontalPageIndex;
		public int verticalPageIndex;
		
		public float horizontalPageCount;
		public float verticalPageCount;

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

		public enum LayoutMode { Instant, Animated }


		#region Delegates
		
		void Start()
		{
			// Add listeners removed by `OnDestroy()` calls on Play / Stop Mode changes.
			AddListeners();
		}
		
		void OnDestroy()
		{
			// Remove listeners when component gets removed from the inspector (also at Play / Stop Mode changes).
			RemoveListeners();
		}
		
		protected void AddListeners()
		{ AddScrollViewListeners(); }
		
		protected void RemoveListeners()
		{ RemoveScrollViewListeners(); }
		
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


		#region Paging
		
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


		/// <summary>
		/// Something like `UICenterOnChild.Recenter`. Snap `UIScrollView` position to the nearest page.
		/// Can be happen animated upon request (using `SpringPanel`).
		/// </summary>

		[ContextMenu("Execute")]
		public void Layout()
		{ Layout(LayoutMode.Instant); }

		public void Layout(LayoutMode layoutMode)
		{
			if (enabled == false) return; // Only if enabled

			// Precalculations.
			CalculateSizes();
			CalculatePageDimensions();
			CalculatePositions();
			
			// Calculate desired page position.
			CalculatePageIndices();
			ClampPageIndices();
			
			// Apply the calculated value.
			ScrollToPageIndices(layoutMode);
		}

		void LayoutOnTouchFinished()
		{ 
			if (enabled == false) return; // Only if enabled
			
			// Precalculations.
			CalculateSizes();
			CalculatePageDimensions();
			CalculatePositions();

			// Modifiers.
			bool isFlick = LookupFlick();
			if (isFlick == false)
			{ LookupSnap(); }
			
			// Calculate desired page position.
			CalculatePageIndices();
			horizontalPageIndex += horizontalPageIndex_Flick + horizontalPageIndex_Snap;
			verticalPageIndex += verticalPageIndex_Flick + verticalPageIndex_Snap;
			ClampPageIndices();

			// Apply the calculated value.
			ScrollToPageIndices(LayoutMode.Animated);
		}

		void ScrollToPageIndices(LayoutMode layoutMode)
		{
			// The result (paged scroll position).
			Vector2 pagedContentPosition = ContentPositionForPageIndices(horizontalPageIndex, verticalPageIndex);
			Vector3 localOffset = new Vector3(
				pagedContentPosition.x - contentPosition.x,
				pagedContentPosition.y - contentPosition.y,
				0.0f
				);

			Debug.Log("`" + GetType() + "` Layout");
			Debug.Log("pagedContentPosition: "+pagedContentPosition);
			Debug.Log("localOffset: "+localOffset);

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

		void OnScroll(UIPanel panel)
		{ 
			bool hasPageControl = false;
			if (hasPageControl == false) return; // Only having any `PageControl`
		}

		void CalculatePositions()
		{
			contentPosition = new Vector2(
				- scrollView.panel.clipOffset.x,
				- scrollView.panel.clipOffset.y
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
			// Determine page indices.
			horizontalPageIndex = (scrollSize.x <= 0.0f) ? 0 : Mathf.FloorToInt((normalizedContentPosition.x + (normalizedPageSize_half.x)) / normalizedPageSize.x);
			verticalPageIndex = (scrollSize.y <= 0.0f) ? 0 : Mathf.FloorToInt((normalizedContentPosition.y + (normalizedPageSize_half.y)) / normalizedPageSize.y);
		}
		
		void ClampPageIndices()
		{
			// Calculate page counts.
			horizontalPageCount = Mathf.Floor(scrollSize.x / pageSize.x);
			verticalPageCount = Mathf.Floor(scrollSize.y / pageSize.y);
			
			// Clamp.
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
	}
}
