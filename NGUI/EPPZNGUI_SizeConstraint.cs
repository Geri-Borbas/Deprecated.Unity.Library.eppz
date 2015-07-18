using UnityEngine;
using System.Collections;


namespace EPPZ.NGUI
{


	/// <summary>
	/// Layout tool that binds a foreign widget size to this widget dimensions.
	/// </summary>


	[ExecuteInEditMode]
	[AddComponentMenu("eppz!/NGUI/Size Constraint")]
	public class EPPZNGUI_SizeConstraint : MonoBehaviour
	{


		/// <summary>
		/// Foreign Widget to get size from.
		/// </summary>
		private UIWidget _target = null;
		public UIWidget target = null;

		/// <summary>
		/// Which dimension to get from the target.
		/// </summary>
		public enum Constraint { Width, Height, Both }
		public Constraint constraint = Constraint.Both;

		/// <summary>
		/// Scale the dimenstion got from the target.
		/// </summary>
		public float multiplier = 1.0f;
		
		/// <summary>
		/// Widget to control on this GameObject.
		/// </summary>
		private UIWidget _widget;
		private UIWidget widget
		{
			get
			{
				if (_widget == null)
				{
					_widget = GetComponent<UIWidget>();

					_widget.didUpdate -= WidgetDidUpdate;
					_widget.didUpdate += TargetDidUpdate; // Add listener (once)
				}
				return _widget;
			}
		}


		#region Delegates

		void Update()
		{
			// Fake setter for `target`.
			if (target != _target)
			{ TargetChanged(); }
		}

		void TargetChanged()
		{
			if (target == null)
			{ _target.didUpdate -= TargetDidUpdate; } // Remove listener from previous
			
			_target = target;
			
			if (_target != null)
			{
				_target.didUpdate -= TargetDidUpdate;
				_target.didUpdate += TargetDidUpdate; // Add listener (once)
			}
		}

		void OnEnable()
		{
			if (widget != null)
			{
				widget.didUpdate -= WidgetDidUpdate;
				widget.didUpdate += WidgetDidUpdate; // Add listener (once)
			}
			
			if (target != null)
			{
				target.didUpdate -= TargetDidUpdate;
				target.didUpdate += TargetDidUpdate; // Add listener (once)
			}
		}

		void OnDisable()
		{
			if (widget != null)
			{ widget.didUpdate -= WidgetDidUpdate; } // Remove listener

			if (target != null)
			{ target.didUpdate -= TargetDidUpdate; } // Remove listener
		}

		private void WidgetDidUpdate()
		{ LayoutIfActive(); }
		
		private void TargetDidUpdate()
		{ LayoutIfActive(); }

		#endregion


		#region Layout

		private void LayoutIfActive()
		{
			if (this.enabled == false) return; // Only if active
			Layout();
		}

		private void Layout()
		{
			int width = widget.width;
			int height = widget.height;
			
			switch (constraint)
			{
				case Constraint.Width: width = Mathf.RoundToInt((float)target.width * multiplier); break;
				case Constraint.Height: height = Mathf.RoundToInt((float)target.height * multiplier); break;
				case Constraint.Both: width = Mathf.RoundToInt((float)target.width * multiplier); height = Mathf.RoundToInt((float)target.height * multiplier); break;
			}

			// Handles Anchors, Aspect, and other NGUI stuff in setters.
			widget.width = width;
			widget.height = height;
		}

		#endregion
	}
}