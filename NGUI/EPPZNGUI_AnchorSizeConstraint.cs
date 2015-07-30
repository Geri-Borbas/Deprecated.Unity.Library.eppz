using UnityEngine;
using System.Collections;


namespace EPPZ.NGUI
{
	
	
	/// <summary>
	/// Layout tool that binds a foreign widget size (local corners relative to the pivot) to this widget absolute anchor dimensions.
	/// </summary>
	
	
	[ExecuteInEditMode]
	[AddComponentMenu("eppz!/NGUI/Anchor Size Constraint")]
	public class EPPZNGUI_AnchorSizeConstraint : EPPZNGUI_Constraint
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
		

		protected override bool HasChanged()
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
			return multiplierChanged || targetChanged || widgetChanged;
		}

		public override void Layout()
		{
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
			// Local space corners of the widget. The order is bottom-left, top-left, top-right, bottom-right.
			Vector3[] localCorners = targetWidget.localCorners;
			float bottom = localCorners[0].y * multiplier;
			float left = localCorners[0].x * multiplier;
			float top = localCorners[2].y * multiplier;
			float right = localCorners[2].x * multiplier;

			if (widget.isAnchored)
			{
				if (constraint == EPPZ.NGUI.SizeConstraint.Width || constraint == EPPZ.NGUI.SizeConstraint.Both)
				{
					if (widget.leftAnchor.target != null)
					{ widget.leftAnchor.absolute = Mathf.FloorToInt(left); }

					if (widget.rightAnchor.target != null)
					{ widget.rightAnchor.absolute = Mathf.FloorToInt(right); }
				}

				if (constraint == EPPZ.NGUI.SizeConstraint.Height || constraint == EPPZ.NGUI.SizeConstraint.Both)
				{
					if (widget.topAnchor.target != null)
					{ widget.topAnchor.absolute = Mathf.FloorToInt(top); }
					
					if (widget.bottomAnchor.target != null)
					{ widget.bottomAnchor.absolute = Mathf.FloorToInt(bottom); }
				}	

				// Execute anchoring.
				widget.UpdateAnchors();
			}
		}
	}
}
