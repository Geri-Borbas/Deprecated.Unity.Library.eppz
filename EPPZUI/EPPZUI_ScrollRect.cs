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

		private float flickDistance = 10.0f;
		private float flickTime = 0.3f;

		[HideInInspector] public int horizontalPageIndex;
		[HideInInspector] public int verticalPageIndex;
		private bool isDecelerating;
		private float normalizedPageWidth;
		private float normalizedPageHeight;
		private float horizontalPageCount;
		private float verticalPageCount;
		private Vector2 targetNormalizedPosition;

		// Drag dispatching.
		private Vector2 touchPosition;
		private float touchTime;
		private bool dispatched;
		private ScrollRect perpendicularScrollRect;


		#region Drag dispatching

		public override void OnInitializePotentialDrag(PointerEventData eventData)
		{
			base.OnInitializePotentialDrag(eventData);
			if (content == null) return; // Only having content

			// Track touch position for drag direction measurement later on.
			touchPosition = eventData.position;
			touchTime = Time.time;
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
			bool isHorizontalDrag = (angle > 45) && (angle < 135);
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

		void ApplyPaging(PointerEventData eventData)
		{
			// Flick distance based on global drag treshold.
			flickDistance = EventSystem.current.pixelDragThreshold * 2.0f;

			// Lookup flick.
			Vector2 offset = eventData.position - touchPosition;
			float timeDistance = Time.time - touchTime;
			bool isHorizontalFlick = (Mathf.Abs(offset.x) > flickDistance) && (timeDistance < flickTime);
			bool isVerticalFlick = (Mathf.Abs(offset.x) > flickDistance) && (timeDistance < flickTime);
			int horizontalPageFlick = (isHorizontalFlick) ? -(int)Mathf.Sign(offset.x) : 0;
			int verticalPageFlick = (isVerticalFlick) ? -(int)Mathf.Sign(offset.y) : 0;

			// Calculate desired page position.
			CalculatePageIndices();
			horizontalPageIndex += horizontalPageFlick;
			verticalPageIndex += verticalPageFlick;
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
			// Calculate page dimensions.
			RectTransform rectTransform = this.transform as RectTransform;
			float width = rectTransform.rect.width;
			float height = rectTransform.rect.height;
			float scrollWidth = content.rect.width - width;
			float scrollHeight = content.rect.height - height;
			normalizedPageWidth = (scrollWidth <= 0.0f) ? 0.0f : width / scrollWidth;
			normalizedPageHeight = (scrollHeight <= 0.0f) ? 0.0f : height / scrollHeight;

			// Determine page indices.
			horizontalPageIndex = (scrollWidth <= 0.0f) ? 0 : Mathf.FloorToInt((horizontalNormalizedPosition + (normalizedPageWidth / 2.0f)) / normalizedPageWidth);
			verticalPageIndex = (scrollHeight <= 0.0f) ? 0 : Mathf.FloorToInt((verticalNormalizedPosition + (normalizedPageHeight / 2.0f)) / normalizedPageHeight);
			
			// Calculate page counts.
			horizontalPageCount = Mathf.Floor(scrollWidth / width);
			verticalPageCount = Mathf.Floor(scrollWidth / width);
		}

		void ClampPageIndices()
		{
			
			horizontalPageIndex = Mathf.FloorToInt(Mathf.Clamp((float)horizontalPageIndex, 0.0f, horizontalPageCount));
			verticalPageIndex = Mathf.FloorToInt(Mathf.Clamp((float)verticalPageIndex, 0.0f, verticalPageCount));
		}

		Vector2 NormalizedPositionForPageIndices(int horizontalPageIndex_, int verticalPageIndex_)
		{
			return new Vector2(
				normalizedPageWidth * (float)horizontalPageIndex_,
				normalizedPageHeight * (float)verticalPageIndex_
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
