using UnityEngine;
using System.Collections;


namespace EPPZ.NGUI
{


	/// <summary>
	/// Layout tools to use within EPPZ.NGUI constraints. Adjusts every internal NGUI goodie under the hood (Anchors, Serialization, Colliders, Aspect, etc.).
	/// </summary>


	public static class _UIWidget
	{


		public static void AdjustSize(this UIWidget widget, float width, float height)
		{ widget.AdjustSize(width, height, EPPZ.NGUI.SizeConstraintType.Both); }

		public static void AdjustSize(this UIWidget widget, float width, float height, EPPZ.NGUI.SizeConstraintType constraint)
		{
			// Get size difference.
			float widthDifference = width - widget.width;
			float heightDifference = height - widget.height;
			
			float leftDifference = 0.0f;
			float bottomDifference = 0.0f;
			float rightDifference = 0.0f;
			float topDifference = 0.0f;
			
			// Calculate horizontal differences.
			if (constraint == EPPZ.NGUI.SizeConstraintType.Width || constraint == EPPZ.NGUI.SizeConstraintType.Both)
			{
				leftDifference = -(widget.pivotOffset.x * widthDifference);
				rightDifference = (1.0f - widget.pivotOffset.x) * widthDifference;
			}
			
			// Calculate vertical differences.
			if (constraint == EPPZ.NGUI.SizeConstraintType.Height || constraint == EPPZ.NGUI.SizeConstraintType.Both)
			{
				bottomDifference = -(widget.pivotOffset.y * heightDifference);
				topDifference = (1.0f - widget.pivotOffset.y) * heightDifference;
			}
			
			// Adjust every internal NGUI goodie under the hood (Anchors, Serialization, Colliders, Aspect, etc.).
			NGUIMath.AdjustWidget(widget, leftDifference, bottomDifference, rightDifference, topDifference);
		}
	}
}
