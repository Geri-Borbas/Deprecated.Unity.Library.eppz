using UnityEngine;
using System.Collections;


namespace EPPZ.NGUI
{
	
	
	/// <summary>
	/// Layout tool that binds a foreign widget aspect ratio to this widget.
	/// </summary>
	
	
	[ExecuteInEditMode]
	[AddComponentMenu("eppz!/NGUI/Aspect Constraint")]
	public class EPPZNGUI_AspectConstraint : EPPZNGUI_Constraint
	{

		
		/// <summary>
		/// Scale the aspect got from the target (when not using Free aspect).
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


		protected override void Layout()
		{
			// Only with Aspect layout rule if any (inspector emits notification about this).
			if (widget.keepAspectRatio == UIWidget.AspectRatioSource.Free) return;

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

			// Calculate new size.
			float width = widget.width;
			float height = widget.height;
			float aspect = targetWidget.aspectRatio * multiplier;

			if (widget.keepAspectRatio == UIWidget.AspectRatioSource.BasedOnWidth)
			{ height = width / aspect; }

			if (widget.keepAspectRatio == UIWidget.AspectRatioSource.BasedOnHeight)
			{ width = height * aspect; }

			// Do actual (NGUI safe) layout.

			UIWidget.AspectRatioSource keepAspectRatio = widget.keepAspectRatio;
			widget.keepAspectRatio = UIWidget.AspectRatioSource.Free;

				widget.AdjustSize(width, height);

			widget.keepAspectRatio = keepAspectRatio;

			// Track changes.
			_previousMultiplier = multiplier;
			_previousWidgetWidth = widget.width;
			_previousWidgetHeight = widget.height;
			_previousTargetWidth = targetWidget.width;
			_previousTargetHeight = targetWidget.height;

		}
	}
}