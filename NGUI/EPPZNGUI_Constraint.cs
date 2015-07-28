using UnityEngine;
using System;	
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
		/// Determine when to execute constrain layouts.
		/// </summary>
		public enum ConstraintUpdate { OnUpdate, OnTargetUpdate, OnBothUpdate }
		public ConstraintUpdate updateConstraint = ConstraintUpdate.OnUpdate;

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
				RemoveTargetWidgetListener();
				_targetWidget = value;
				AddTargetWidgetListener();
				LayoutIfActive();
#if UNITY_EDITOR
				NGUITools.SetDirty(this);
#endif
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
					AddWidgetListener();
#if UNITY_EDITOR
					NGUITools.SetDirty(this);
#endif
				}
				return _widget;
			}
		}


		#region Delegates

		void Start()
		{
			// Add listeners removed by `OnDestroy()` calls on Play / Stop Mode changes.
			AddListeners();
		}

		void OnDestroy()
		{
			// Remove listeners when component gets removed from the inspector (also at Play / Stop Mode changes).
			RemoveListeners();
		}

		void AddListeners()
		{
			AddWidgetListener();
			AddTargetWidgetListener();
		}

		void RemoveListeners()
		{
			RemoveWidgetListener();
			RemoveTargetWidgetListener();
		}

		void AddWidgetListener()
		{ if (widget != null) widget.didUpdate += WidgetDidUpdate; }

		void RemoveWidgetListener()
		{ if (widget != null) widget.didUpdate -= WidgetDidUpdate; }

		void AddTargetWidgetListener()
		{ if (targetWidget != null) targetWidget.didUpdate += TargetDidUpdate; }
		
		void RemoveTargetWidgetListener()
		{ if (targetWidget != null) targetWidget.didUpdate -= TargetDidUpdate; }
		
		#endregion


		#region Layout

		void WidgetDidUpdate()
		{
			if (updateConstraint == ConstraintUpdate.OnUpdate ||
			    updateConstraint == ConstraintUpdate.OnBothUpdate)
				LayoutIfActive();
		}
		
		void TargetDidUpdate()
		{ 
			if (updateConstraint == ConstraintUpdate.OnTargetUpdate ||
			    updateConstraint == ConstraintUpdate.OnBothUpdate)
				LayoutIfActive();
		}

		void LayoutIfActive()
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
