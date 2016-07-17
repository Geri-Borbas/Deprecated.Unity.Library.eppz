using UnityEngine;
using System.Collections;


namespace EPPZ.NGUI
{


	/// <summary>
	/// Layout tool that binds a foreign widget aspect ratio to this widget.
	/// </summary>
	
	
	[ExecuteInEditMode]
	[AddComponentMenu("eppz!/NGUI/Aspect Constraint")]
	public class AspectConstraint : Constraint
	{


		/// <summary>
		/// Which dimension to preserve (get other dimension from the target).
		/// </summary>
		public EPPZ.NGUI.AspectConstraintType keepAspectRatio = EPPZ.NGUI.AspectConstraintType.BasedOnWidth;

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

			Adjust();
			
			// Track changes.
			_previousMultiplier = multiplier;
			_previousWidgetWidth = widget.width;
			_previousWidgetHeight = widget.height;
			_previousTargetWidth = targetWidget.width;
			_previousTargetHeight = targetWidget.height;
		}

		void Adjust()
		{
			// Calculate new size.
			float width = widget.width;
			float height = widget.height;
			float targetAspect = (float)targetWidget.width / (float)targetWidget.height; // Calculate aspect on the fly (!)
			float aspect = targetAspect * multiplier;
			EPPZ.NGUI.SizeConstraintType constraint = EPPZ.NGUI.SizeConstraintType.Height;
			
			if (keepAspectRatio == EPPZ.NGUI.AspectConstraintType.BasedOnWidth)
			{
				height = width / aspect;
				constraint = EPPZ.NGUI.SizeConstraintType.Height;
			}
			
			if (keepAspectRatio == EPPZ.NGUI.AspectConstraintType.BasedOnHeight)
			{
				width = height * aspect;
				constraint = EPPZ.NGUI.SizeConstraintType.Width;
			}
			
			// Do actual (NGUI safe) layout.
			widget.AdjustSize(
				width,
				height,
				constraint
				);
		}
	}
}