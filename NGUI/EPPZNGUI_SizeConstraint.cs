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
		private int _previousTargetWidth;
		private int _previousTargetHeight;


		protected override void Layout()
		{
			// Adjust only if changed.
			bool changed = (
				(targetWidget.width != _previousTargetWidth) ||
				(targetWidget.height != _previousTargetHeight)
				);
			if (changed == false) return;

			Adjust();
			
			#if UNITY_EDITOR
			NGUITools.SetDirty(widget);
			#endif

			// Track changes.
			_previousTargetWidth = targetWidget.width;
			_previousTargetHeight = targetWidget.height;
		}

		void Adjust()
		{
			// Adjust Anchors directly if Widget is anchored.
			if (widget.isAnchored)
			{
				// Get size difference.
				int widthDifference = Mathf.RoundToInt(targetWidget.width * multiplier) - widget.width;
				int heightDifference = Mathf.RoundToInt(targetWidget.height * multiplier) - widget.height;
				
				// Adjust horizontal anchors if needed.
				if (constraint ==  Constraint.Width || constraint == Constraint.Both)
				{
					widget.leftAnchor.absolute -= (int)(widget.pivotOffset.x * widthDifference);
					widget.rightAnchor.absolute += (int)((1.0f - widget.pivotOffset.x) * widthDifference);
				}
				
				// Adjust vertical anchors if needed.
				if (constraint == Constraint.Height || constraint == Constraint.Both)
				{
					widget.bottomAnchor.absolute -= (int)(widget.pivotOffset.y * heightDifference);
					widget.topAnchor.absolute += (int)((1.0f - widget.pivotOffset.y) * widthDifference);
				}
			}

			// Adjust dimensions only otherwise.
			else
			{
				widget.SetDimensions(targetWidget.width, targetWidget.height);
			}
		}

	}
}