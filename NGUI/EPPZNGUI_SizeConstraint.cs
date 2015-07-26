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
		private int _previousFrame;

		protected override void Layout()
		{
			// Debug.
			int frame = Time.frameCount;
			bool doubleLayout = (frame == _previousFrame);
			if (doubleLayout) Debug.Log("`"+name+"` Laid out twice this frame.");
			_previousFrame = frame;

			// Only if changed.
			bool changed = (
				(targetWidget.width != _previousTargetWidth) ||
				(targetWidget.height != _previousTargetHeight)
				);
			if (changed == false) return;

			int width = widget.width;
			int height = widget.height;
			
			switch (constraint)
			{
				case Constraint.Width: width = Mathf.RoundToInt((float)targetWidget.width * multiplier); break;
				case Constraint.Height: height = Mathf.RoundToInt((float)targetWidget.height * multiplier); break;
				case Constraint.Both: width = Mathf.RoundToInt((float)targetWidget.width * multiplier); height = Mathf.RoundToInt((float)targetWidget.height * multiplier); break;
			}

			// Accessors handles Anchors, Aspect, and other NGUI stuff.
			widget.width = width;
			widget.height = height;

			// widget.SetDimensions(width, height);
			// widget.UpdateAnchors();

			// Track changes.
			_previousTargetWidth = targetWidget.width;
			_previousTargetHeight = targetWidget.height;
		}
	}
}