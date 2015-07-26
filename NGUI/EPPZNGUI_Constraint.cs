using UnityEngine;
using System.Collections;


namespace EPPZ.NGUI
{


	/// <summary>
	/// Abstract class for layout tool that binds foreign widget's properties to a widget on this GameObject.
	/// </summary>


	[ExecuteInEditMode]
	public abstract class EPPZNGUI_Constraint : MonoBehaviour
	{


		/// <summary>
		/// Foreign Widget to get size from.
		/// </summary>
		[SerializeField] 
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
				NGUITools.SetDirty(this);
			}
		}

		/// <summary>
		/// Widget to control on this GameObject.
		/// </summary>
		private UIWidget _widget = null;
		protected UIWidget widget
		{
			get
			{
				if (_widget == null)
				{
					_widget = GetComponent<UIWidget>();
					_widget.didUpdate += TargetDidUpdate; // Add listener
					NGUITools.SetDirty(this);
				}
				return _widget;
			}
		}


		#region Delegates

		void Start()
		{
			Debug.Log("EPPZNGUI_Constraint.Start()");

			// Add listeners removed by `OnDestroy()` calls on Play / Stop Mode changes.
			AddWidgetListeners();
		}

		void OnDestroy()
		{
			Debug.Log("EPPZNGUI_Constraint.OnDestroy()");

			// Remove listeners when component gets removed from the inspector (also at Play / Stop Mode changes)
			RemoveWidgetListeners();
		}

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

		void AddWidgetListeners()
		{
			Debug.Log("AddWidgetListeners()");

			if (widget != null) widget.didUpdate += TargetDidUpdate; // Add listener
			if (targetWidget != null) targetWidget.didUpdate += TargetDidUpdate;  // Add listener
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
		
		protected virtual void Layout()
		{ }
		
		#endregion
	}
}
