using UnityEngine;
using System.Collections;


namespace EPPZ.NGUI
{


	/// <summary>
	/// Allows dragging of two embedded perpendicular scroll views depending on drag direction.
	/// Based on `UIDragScrollView`, but does not resolve scroll views automatically (so usage takes some care).
	/// </summary>

	[AddComponentMenu("eppz!/NGUI/Drag Perpendicular Scroll Views")]
	public class DragPerpendicularScrollViews : MonoBehaviour
	{


		/// <summary>
		/// Reference to the vertical scroll view that will be dragged if user dragged vertically.
		/// </summary>
		public UIScrollView verticalScrollView;

		/// <summary>
		/// Reference to the vertical scroll view that will be dragged if user dragged vertically.
		/// </summary>
		public UIScrollView horizontalScrollView;
		
		/// <summary>
		/// Reference to the scroll view that is selected based on drag direction.
		/// </summary>
		UIScrollView selectedScrollView;

		/// <summary>
		/// Early returns extracted.
		/// </summary>
		bool isInvalid
		{
			get
			{
				if (verticalScrollView == null) return true;
				if (horizontalScrollView == null) return true;
				if (enabled == false) return true;
				if (NGUITools.GetActive(gameObject) == false) return true;
				if (NGUITools.GetActive(this) == false) return true;
				return false;
			}
		}


		void OnPress(bool pressed)
		{
			if (isInvalid) return;
			verticalScrollView.Press(pressed);
			horizontalScrollView.Press(pressed);
		}

		void OnDragStart()
		{
			if (isInvalid) return;

			// Calculate drag horizontalness.
			Vector2 offset = UICamera.currentTouch.totalDelta;
			float angle = Mathf.Abs(Mathf.Atan2(offset.x, offset.y) * Mathf.Rad2Deg);
			bool isHorizontalDrag = (angle > 45.0f) && (angle < 135.0f);

			// Dispatch drag based on direction.
			if (isHorizontalDrag)
			{
				selectedScrollView = horizontalScrollView;
				verticalScrollView.Press(false); // Release other scroll view
			}
			else
			{
				selectedScrollView = verticalScrollView;
				horizontalScrollView.Press(false); // Release other scroll view
			}
		}

		void OnDrag(Vector2 delta)
		{
			// Drag the selected scroll view.
			if (isInvalid) return;
			selectedScrollView.Drag();
		}
	}
}

