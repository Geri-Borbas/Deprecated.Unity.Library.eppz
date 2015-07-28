using UnityEngine;
using System.Collections;


namespace EPPZ.NGUI
{


	/// <summary>
	/// Layout tool that binds a foreign widget size to this widget dimensions.
	/// </summary>


	[ExecuteInEditMode]
	[AddComponentMenu("eppz!/NGUI/Size Constraint")]
	public class EPPZNGUI_SizeConstraint : EPPZNGUI_Constraint
	{


		/// <summary>
		/// Which dimension to get from the target.
		/// </summary>
		public enum Constraint { Width, Height, Both }
		public Constraint constraint = Constraint.Both;

		/// <summary>
		/// Scale the dimension got from the target.
		/// </summary>
		public float multiplier = 1.0f;

		/// <summary>
		/// Tracking changes to spare calculations.
		/// </summary>
		private int _previousWidgetWidth;
		private int _previousWidgetHeight;
		private int _previousTargetWidth;
		private int _previousTargetHeight;


		protected override void Layout()
		{
			// Adjust only if something has changed.
			bool targetChanged = (
				(targetWidget.width != _previousTargetWidth) ||
				(targetWidget.height != _previousTargetHeight)
				);
			bool widgetChanged = (
				(widget.width != _previousWidgetWidth) ||
				(widget.height != _previousWidgetHeight)
				);
			if (targetChanged == false && widgetChanged == false) return;

			Adjust();

			// Track changes.
			_previousWidgetWidth = widget.width;
			_previousWidgetHeight = widget.height;
			_previousTargetWidth = targetWidget.width;
			_previousTargetHeight = targetWidget.height;
		}

		void Adjust()
		{
			// Get size difference.
			float widthDifference = targetWidget.width * multiplier - widget.width;
			float heightDifference = targetWidget.height * multiplier - widget.height;

			float leftDifference = 0.0f;
			float bottomDifference = 0.0f;
			float rightDifference = 0.0f;
			float topDifference = 0.0f;

			// Calculate horizontal differences.
			if (constraint ==  Constraint.Width || constraint == Constraint.Both)
			{
				leftDifference = -(widget.pivotOffset.x * widthDifference);
				rightDifference = (1.0f - widget.pivotOffset.x) * widthDifference;
			}
			
			// Calculate vertical differences.
			if (constraint == Constraint.Height || constraint == Constraint.Both)
			{
				bottomDifference = -(widget.pivotOffset.y * heightDifference);
				topDifference = (1.0f - widget.pivotOffset.y) * heightDifference;
			}

			// Adjust every NGUI internal goodie under the hood (Anchors, Serialization, Colliders, Aspect, etc.).
			NGUIMath.AdjustWidget(widget, leftDifference, bottomDifference, rightDifference, topDifference);
		}
	}
}