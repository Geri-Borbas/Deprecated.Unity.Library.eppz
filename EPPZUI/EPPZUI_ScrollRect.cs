using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using EPPZGeometry;


namespace EPPZ.UI
{
	
	
	public class EPPZUI_ScrollRect : ScrollRect
	{


		// Paging.
		public bool paging = false;
		[Range (0,1)] public float pagingStrength = 0.135f;
		[Range (0,1)] public float snapDistance = 1.0f;

		private float flickDistance = 10.0f;
		private float flickTime = 0.3f;

		[HideInInspector] public int horizontalPageIndex;
		[HideInInspector] public int verticalPageIndex;
		private bool isDecelerating;

		private Vector2 pageSize;
		private Vector2 scrollSize;
		private Vector2 normalizedPageSize;
		private Vector2 normalizedPageSize_half;

		private float horizontalPageCount;
		private float verticalPageCount;
		private Vector2 targetNormalizedPosition;

		private int horizontalPageIndex_Flick = 0;
		private int verticalPageIndex_Flick = 0;
		private int horizontalPageIndex_Snap = 0;
		private int verticalPageIndex_Snap = 0;

		// Drag dispatching.
		private Vector2 touchPosition;
		private Vector2 touchNormalizedContentPosition;
		private float touchTime;
		private bool dispatched;
		private ScrollRect perpendicularScrollRect;


		#region Drag dispatching

		public override void OnInitializePotentialDrag(PointerEventData eventData)
		{
			base.OnInitializePotentialDrag(eventData);
			if (content == null) return; // Only having content
			CaptureTouchData(eventData.position, Time.time);
		}

		public void CaptureTouchData(Vector2 touchPosition_, float touchTime_)
		{
			// Track touch position for drag direction measurement later on (could be a dispatched touch as well).
			touchPosition = touchPosition_;
			touchTime = touchTime_;

			// Local.
			touchNormalizedContentPosition = normalizedPosition;
			isDecelerating = false;
		}

		public override void OnBeginDrag(PointerEventData eventData)
		{
			if (content == null) return; // Only having content

			// Look for any perpendicular ScrollRect behind.
			perpendicularScrollRect = GetClosestPerpendicularScrollRectParent();

			// Calculate drag horizontalness.
			Vector2 offset = eventData.position - touchPosition;
			float angle = Mathf.Abs(Mathf.Atan2(offset.x, offset.y) * Mathf.Rad2Deg);
			bool isHorizontalDrag = (angle > 45.0f) && (angle < 135.0f);
			bool isDragInScrollDirection = (this.horizontal && isHorizontalDrag);

			if (perpendicularScrollRect == null || // If no perpendicular ScrollRect...
			    isDragInScrollDirection) // Or drag happens in the scroll direction of this ScrollRect...
			{
				// ...simply drag.
				base.OnBeginDrag(eventData);
			}

			else // Otherwise...
			{
				// ...dispatch this drag session to the perpendicular ScrollRect.
				dispatched = true;

				// Dispatch inital touch data.
				if (perpendicularScrollRect.GetType() == this.GetType())
				{
					EPPZUI_ScrollRect perpendicularEppzScrollRect = perpendicularScrollRect as EPPZUI_ScrollRect;
					perpendicularEppzScrollRect.CaptureTouchData(touchPosition, touchTime);
				}

				// Dispatch event itself.
				perpendicularScrollRect.OnBeginDrag(eventData);
			}
		}
		
		public override void OnDrag(PointerEventData eventData)
		{
			if (content == null) return; // Only having content

			if (dispatched)
			{
				perpendicularScrollRect.OnDrag(eventData);
			}
			
			else
			{
				base.OnDrag(eventData);
				CalculatePageIndices();
			}
		}
		
		public override void OnEndDrag(PointerEventData eventData)
		{
			if (content == null) return; // Only having content

			if (dispatched)
			{
				perpendicularScrollRect.OnEndDrag(eventData);

				// End dispatch after every drag session.
				dispatched = false;
			}
			
			else
			{
				base.OnEndDrag(eventData);
				if (paging) ApplyPaging(eventData);
			}
		}

		public override void OnScroll(PointerEventData eventData)
		{
			if (content == null) return; // Only having content

			base.OnScroll(eventData);
			CalculatePageIndices();
			ClampPageIndices();
		}

		#endregion


		#region Paging
		
		
		void CalculatePageDimensions()
		{
			RectTransform rectTransform = this.transform as RectTransform;
			pageSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
			scrollSize = new Vector2(content.rect.width - pageSize.x, content.rect.height - pageSize.y);
			normalizedPageSize = new Vector2(
				(scrollSize.x <= 0.0f) ? 0.0f : pageSize.x / scrollSize.x,
				(scrollSize.y <= 0.0f) ? 0.0f : pageSize.y / scrollSize.y
				);
			normalizedPageSize_half = new Vector2(normalizedPageSize.x / 2.0f, normalizedPageSize.y / 2.0f);
		}

		void ApplyPaging(PointerEventData eventData)
		{
			// Flick distance based on global drag treshold.
			flickDistance = EventSystem.current.pixelDragThreshold * 2.0f;

			// Page dimensions.
			CalculatePageDimensions();

			// Offset.
			Vector2 touchOffset = eventData.position - touchPosition;
			Vector2 normalizedPositionOffset = normalizedPosition - touchNormalizedContentPosition;
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
			horizontalPageIndex_Flick = (isHorizontalFlick) ? -sign_x : 0;
			verticalPageIndex_Flick = (isVerticalFlick) ? -sign_y : 0;

			// Lookup snap distance (if any set).
			if (isFlick == false)
			{
				if (snapDistance < 1.0f)
				{
					bool isHorizontalSnap = (Mathf.Abs(normalizedPositionOffset.x) < normalizedPageSize_half.x) && (Mathf.Abs(normalizedPositionOffset.x) >= (normalizedPageSize.x * snapDistance));
					bool isVerticalSnap = (Mathf.Abs(normalizedPositionOffset.y) < normalizedPageSize_half.y) && (Mathf.Abs(normalizedPositionOffset.y) >= (normalizedPageSize.y * snapDistance));
					horizontalPageIndex_Snap = (isHorizontalSnap) ? -sign_x : 0;
					verticalPageIndex_Snap = (isVerticalSnap) ? -sign_y : 0;
				}
			}

			// Calculate desired page position.
			CalculatePageIndices();
			horizontalPageIndex += horizontalPageIndex_Flick + horizontalPageIndex_Snap;
			verticalPageIndex += verticalPageIndex_Flick + verticalPageIndex_Snap;
			ClampPageIndices();
			Vector2 normalizedPagedScrollPosition = NormalizedPositionForPageIndices(horizontalPageIndex, verticalPageIndex);
			
			// If `Clamped` ...
			if (this.movementType == MovementType.Clamped)
			{
				// ...set immediately.
				normalizedPosition = normalizedPagedScrollPosition;
			}
			
			// Otherwise...
			else
			{
				// ...set target position for now...
				targetNormalizedPosition = normalizedPagedScrollPosition;
				isDecelerating = true; // ...`Update` handles the rest.
			}
		}
		
		void CalculatePageIndices()
		{
			CalculatePageDimensions();

			// Determine page indices.
			horizontalPageIndex = (scrollSize.x <= 0.0f) ? 0 : Mathf.FloorToInt((horizontalNormalizedPosition + (normalizedPageSize_half.x)) / normalizedPageSize.x);
			verticalPageIndex = (scrollSize.y <= 0.0f) ? 0 : Mathf.FloorToInt((verticalNormalizedPosition + (normalizedPageSize_half.y)) / normalizedPageSize.y);
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

		Vector2 NormalizedPositionForPageIndices(int horizontalPageIndex_, int verticalPageIndex_)
		{
			return new Vector2(
				normalizedPageSize.x * (float)horizontalPageIndex_,
				normalizedPageSize.y * (float)verticalPageIndex_
				);
		}

		void Update()
		{
			if (isDecelerating == false) return; // Only if decelerating

			// Lerp towards target (using inherited elasticity value).
			normalizedPosition = Vector2.Lerp(normalizedPosition, targetNormalizedPosition, pagingStrength);

			// Arrival.
			if (Vector2.Distance(normalizedPosition, targetNormalizedPosition) < 0.0001f)
			{
				normalizedPosition = targetNormalizedPosition;
				isDecelerating = false;
			}
		}

		#endregion


		#region ScrollRect hierarchy lookup

		ScrollRect GetClosestPerpendicularScrollRectParent()
		{ return GetClosestPerpendicularScrollRectParentOf(this.transform); }

		ScrollRect GetClosestPerpendicularScrollRectParentOf(Transform transform)
		{
			// Only having parent.
			Transform parentTransform = transform.parent;
			if (parentTransform == null) return null;

			// Only having GameObject parent.
			GameObject parentObject = parentTransform.gameObject;
			if (parentObject == null) return null;

			// Look for parent ScrollRect recursive.
			ScrollRect parentScrollRect = parentObject.GetComponent<ScrollRect>();
			if (parentScrollRect == null) return GetClosestPerpendicularScrollRectParentOf(parentTransform);

			// Look for perpendicular parent ScrollRect recursive.
			bool perpendicular = (this.horizontal && parentScrollRect.vertical) || (this.vertical && parentScrollRect.horizontal);
			if (perpendicular == false) return GetClosestPerpendicularScrollRectParentOf(parentTransform);

			// Got it.
			return parentScrollRect;
		}

		#endregion
	}
}
