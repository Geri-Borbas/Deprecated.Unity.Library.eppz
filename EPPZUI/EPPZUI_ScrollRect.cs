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
			perpendicularScrollRect = HighestPerpendicularScrollRectBehind(eventData);

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

		private ScrollRect HighestPerpendicularScrollRectBehind(PointerEventData eventData)
		{
			// Raycast with the pointer event.
			List<RaycastResult> results = new  List<RaycastResult>();
			EventSystem.current.RaycastAll(eventData, results);

			// Look for ScrollRect.
			ScrollRect scrollRect = null;
			int eachDepth = 0;
			int highestDepth = 0;
			foreach (RaycastResult eachResult in results)
			{
				if (eachResult.isValid == false) continue; // Only having `GameObject` hit
				
				// Get object.
				GameObject eachGameObject = eachResult.gameObject;
				if (eachGameObject == this.gameObject) continue; // Skip this object
				
				// Get depth if any.
				Graphic eachGraphic = eachGameObject.GetComponent<Graphic>();
				if (eachGraphic != null) eachDepth = eachGraphic.depth;
				
				// Get ScrollRect if any.
				ScrollRect eachScrollRect = eachGameObject.GetComponent<ScrollRect>();
				if (eachScrollRect == null) continue;

				// Look if perpendicular.
				bool perpendicular = (this.horizontal && eachScrollRect.vertical) || (this.vertical && eachScrollRect.horizontal);
				if (perpendicular == false) continue;

				// Set search hit.
				if (scrollRect == null || // If this is the first hit
				    eachDepth >= highestDepth) // Or if it has higher hierarchy depth
				{
					scrollRect = eachScrollRect;
					highestDepth = eachDepth; // Track depth
				}
			}

			return scrollRect;
		}
	}
}
