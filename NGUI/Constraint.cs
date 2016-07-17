using UnityEngine;
using System;	
using System.Collections;


namespace EPPZ.NGUI
{


	/// <summary>
	/// Abstract class for layout tool that binds foreign Widget's properties to a Widget on this GameObject.
	/// </summary>


	[ExecuteInEditMode]
	public abstract class Constraint : MonoBehaviour
	{


		/// <summary>
		/// Determine when to execute constrain layouts.
		/// </summary>
		public EPPZ.NGUI.ConstraintUpdate updateConstraint = EPPZ.NGUI.ConstraintUpdate.OnUpdate;

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
		public UIWidget widget
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

		protected void AddListeners()
		{
			AddWidgetListener();
			AddTargetWidgetListener();
		}

		protected void RemoveListeners()
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
			if (updateConstraint == EPPZ.NGUI.ConstraintUpdate.OnUpdate ||
			    updateConstraint == EPPZ.NGUI.ConstraintUpdate.OnBothUpdate)
				LayoutIfActive();
		}
		
		void TargetDidUpdate()
		{ 
			if (updateConstraint == EPPZ.NGUI.ConstraintUpdate.OnTargetUpdate ||
			    updateConstraint == EPPZ.NGUI.ConstraintUpdate.OnBothUpdate)
				LayoutIfActive();
		}

		protected virtual bool ShouldLayout()
		{
			return (targetWidget != null); // Layout only if anything targeted
		}

		protected virtual bool HasChanged()
		{
			return true; // Layout only if something has changed
		}

		void LayoutIfActive()
		{
			if (this == null) return; // Only if not destroyed
			if (this.enabled == false) return; // Only if active
			if (ShouldLayout() && (HasChanged()))
			{ Layout(); }
		}

		public virtual void Layout()
		{ }
		
		#endregion
	}
}
