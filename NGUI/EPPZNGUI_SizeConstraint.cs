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
		public EPPZ.NGUI.SizeConstraint constraint = EPPZ.NGUI.SizeConstraint.Both;

		/// <summary>
		/// Scale the dimension got from the target.
		/// </summary>
		public float multiplier = 1.0f;

		/// <summary>
		/// Tracking changes to spare layout calculations.
		/// </summary>
		private float _previousMultiplier = 1.0f;
		private int _previousWidgetWidth;
		private int _previousWidgetHeight;
		private int _previousTargetWidth;
		private int _previousTargetHeight;


		public override void Layout()
		{
			// Adjust only if something has changed.
			bool multiplierChanged = (multiplier != _previousMultiplier);
			bool targetChanged = (
				(targetWidget.width != _previousTargetWidth) ||
				(targetWidget.height != _previousTargetHeight)
				);
			bool widgetChanged = (
				(widget.width != _previousWidgetWidth) ||
				(widget.height != _previousWidgetHeight)
				);
			if (targetChanged == false && widgetChanged == false && multiplierChanged == false) return;

			// Do actual layout.
			widget.AdjustSize(
				targetWidget.width * multiplier,
				targetWidget.height * multiplier,
				constraint
				);

			// Track changes.
			_previousMultiplier = multiplier;
			_previousWidgetWidth = widget.width;
			_previousWidgetHeight = widget.height;
			_previousTargetWidth = targetWidget.width;
			_previousTargetHeight = targetWidget.height;
		}
	}
}