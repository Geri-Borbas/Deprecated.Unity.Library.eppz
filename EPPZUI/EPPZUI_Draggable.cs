using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using EPPZGeometry;


namespace EPPZ.UI
{


	public class EPPZUI_Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
	{


		public RectTransform panel;
		public RectTransform bottomPositionPanel;
		public RectTransform topPositionPanel;
		public RectTransform targetPositionPanel;
		public float deceleratingFilter = 0.05f;
		public enum Direction { Horizontal, Vertical }
		public Direction direction = Direction.Vertical;

		public bool isDragged = false;
		public bool isDecelerating = false;

		private Vector2 touchOffset;
		private Vector2 parentSize 
		{
			get
			{
				RectTransform parentPanel = panel.transform.parent as RectTransform;
				return parentPanel.rect.size;
			}
		}

		private Vector2 topPosition
		{ get { return panel.parent.InverseTransformPoint(topPositionPanel.position).xy(); } }

		private Vector2 bottomPosition
		{ get { return panel.parent.InverseTransformPoint(bottomPositionPanel.position).xy(); } }

		private Vector2 targetPosition
		{ get { return panel.parent.InverseTransformPoint(targetPositionPanel.position).xy(); } }


		public void OnBeginDrag(PointerEventData eventData)
		{
			isDragged = true;
			isDecelerating = false;
			touchOffset = panel.anchoredPosition - eventData.position;
		}

		public void OnDrag(PointerEventData eventData)
		{
			// Determine dragged position.
			Vector2 draggedPosition = touchOffset + eventData.position;
			switch (direction)
			{
				case Direction.Horizontal: panel.anchoredPosition = new Vector2(draggedPosition.x, panel.anchoredPosition.y); break;
				case Direction.Vertical: panel.anchoredPosition = new Vector2(panel.anchoredPosition.x, draggedPosition.y); break;
			}
		}
		
		public void OnEndDrag(PointerEventData eventData)
		{
			isDragged = false;
			isDecelerating = true;

			// Snap to top or bottom.
			float topDistance = Vector2.Distance(panel.anchoredPosition, topPosition);
			float bottomDistance = Vector2.Distance(panel.anchoredPosition, bottomPosition);
			targetPositionPanel = (topDistance < bottomDistance) ? topPositionPanel : bottomPositionPanel;
		}

		void Update()
		{
			if (isDragged) return; // Only if not dragging
			if (isDecelerating == false) return; // Only if decelerating

			// Determine target position (may change during transition).
			Vector2 targetPanelPosition = Vector2.zero;
			switch (direction)
			{
				case Direction.Horizontal: targetPanelPosition = new Vector2(targetPosition.x, panel.anchoredPosition.y); break;
				case Direction.Vertical: targetPanelPosition = new Vector2(panel.anchoredPosition.x, targetPosition.y); break;
			}

			// Decelerating.
			panel.anchoredPosition = Vector2.Lerp(panel.anchoredPosition, targetPanelPosition, deceleratingFilter);
		}
	}
}
