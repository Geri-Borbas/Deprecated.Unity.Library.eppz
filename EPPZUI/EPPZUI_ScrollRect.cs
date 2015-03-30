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


		private Vector2 touchPosition;
		private bool dispatched;
		private ScrollRect perpendicularScrollRect;


		public void Dispatch()
		{
			dispatched = true;
		}

		public void Undispatch()
		{
			dispatched = false;
		}

		public override void OnInitializePotentialDrag(PointerEventData eventData)
		{
			base.OnInitializePotentialDrag(eventData);

			// Track touch position for drag direction measurement later on.
			touchPosition = eventData.position;
		}

		public override void OnBeginDrag(PointerEventData eventData)
		{
			// Look for any perpendicular ScrollRect behind.
			perpendicularScrollRect = GetClosestPerpendicularScrollRectParent();

			// Calculate drag horizontalness.
			Vector2 offset = eventData.position - touchPosition;
			float angle = Mathf.Abs(Mathf.Atan2(offset.x, offset.y) * Mathf.Rad2Deg);
			bool isHorizontalDrag = (angle > 45) && (angle < 135);
			bool isDragInScrollDirection = (this.horizontal && isHorizontalDrag);
			Debug.Log("angle ("+angle+") isHorizontalDrag("+isHorizontalDrag+")");

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
			if (dispatched)
			{
				perpendicularScrollRect.OnDrag(eventData);
			}
			
			else
			{
				base.OnDrag(eventData);
			}
		}
		
		public override void OnEndDrag(PointerEventData eventData)
		{
			if (dispatched)
			{
				perpendicularScrollRect.OnEndDrag(eventData);

				// End dispatch after every drag session.
				dispatched = false;
			}
			
			else
			{
				base.OnEndDrag(eventData);
			}
		}


		private ScrollRect GetClosestPerpendicularScrollRectParent()
		{ return GetClosestPerpendicularScrollRectParentOf(this.transform); }

		private ScrollRect GetClosestPerpendicularScrollRectParentOf(Transform transform)
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
	}
}
