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
		private UIWidget _targetWidget = null;
		public UIWidget targetWidget
		{
			get
			{ return _targetWidget; }

			set
			{
				if (_targetWidget == value) return; // Only if changed
				TargetWidgetWillChange(_targetWidget, value);
				_targetWidget = value;
			}
		}

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
					_widget.didUpdate += TargetDidUpdate; // Add listener
				}
				return _widget;
			}
		}


		#region Delegates

		void TargetWidgetWillChange(UIWidget from, UIWidget to)
		{
			// Debug.
			string fromName = (from) ? from.name : "<null>";
			string toName = (to) ? to.name : "<null>";
			Debug.Log("TargetWidgetWillChange("+fromName+", "+toName+")");

			if (from != null)
			{
				from.didUpdate -= TargetDidUpdate;
				Debug.Log("Removed subscription from `"+fromName+"`.");
			} // Remove listener from previous
			
			if (to != null)
			{
				to.didUpdate += TargetDidUpdate;
				Debug.Log("Added subscription to `"+toName+"`.");
			} // Add listener
		}

		void OnDestroy()
		{
			Debug.Log("EPPZNGUI_SizeConstraint.OnDestroy()");
			RemoveWidgetListeners();
		}

		public void RemoveWidgetListeners()
		{
			Debug.Log("RemoveWidgetListeners()");

			if (widget != null)
			{
				widget.didUpdate -= WidgetDidUpdate;
				Debug.Log("Removed Widget subscription.");
			} // Remove listener
			
			if (targetWidget != null)
			{
				targetWidget.didUpdate -= TargetDidUpdate;
				Debug.Log("Removed Target subscription.");
			} // Remove listener
		}

		private void WidgetDidUpdate()
		{ LayoutIfActive(); }
		
		private void TargetDidUpdate()
		{ LayoutIfActive(); }

		#endregion


		#region Layout

		private void LayoutIfActive()
		{
			if (this == null) return; // Only if not destroyed
			if (this.enabled == false) return; // Only if active
			if (targetWidget == null) return; // Only if anything targeted
			Layout();
		}

		private void Layout()
		{
			int width = widget.width;
			int height = widget.height;
			
			switch (constraint)
			{
				case Constraint.Width: width = Mathf.RoundToInt((float)targetWidget.width * multiplier); break;
				case Constraint.Height: height = Mathf.RoundToInt((float)targetWidget.height * multiplier); break;
				case Constraint.Both: width = Mathf.RoundToInt((float)targetWidget.width * multiplier); height = Mathf.RoundToInt((float)targetWidget.height * multiplier); break;
			}

			// Handles Anchors, Aspect, and other NGUI stuff in setters.
			widget.width = width;
			widget.height = height;
		}

		#endregion
	}
}