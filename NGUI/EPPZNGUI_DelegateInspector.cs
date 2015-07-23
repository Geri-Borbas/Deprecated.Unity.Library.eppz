using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace EPPZ.NGUI
{
	
	
	/// <summary>
	/// Debug tool for inspect delegate subscribers for the given target component.
	/// </summary>
	
	
	[ExecuteInEditMode]
	[AddComponentMenu("eppz!/NGUI/Debug/Delegate Inspector")]
	public class EPPZNGUI_DelegateInspector : MonoBehaviour
	{


		/// <summary>
		/// A target component to inspect.
		/// </summary>
		public Component target;


		/// <summary>
		/// The name of the delegate field in the target component.
		/// </summary>
		public string delegateFieldName = "didUpdate";


		/// <summary>
		/// List of serializable (inspectable) description of delegates attached to the target.
		/// </summary>
		public List<MonoBehaviour> delegateTargets = new List<MonoBehaviour>();


		private Delegate delegates;
		private Delegate _previousDelegates;


		void Update()
		{
			// Get the field by name.
			delegates = (Delegate)target.GetType().GetField(delegateFieldName).GetValue(target);

			// Listen for change.
			if (delegates == _previousDelegates) return;
			GetDelegateTargets();
			_previousDelegates = delegates; // Track
		}

		void GetDelegateTargets()
		{
			// Only if any.
			if (delegates == null)
			{
				delegateTargets.Clear();
				return; 
			}
			
			// Get invocation list.
			Delegate[] invocations = delegates.GetInvocationList();
			
			// Rebuild descriptions.
			delegateTargets.Clear();
			foreach (Delegate eachDelegate in invocations)
			{
				MonoBehaviour eachTarget = eachDelegate.Target as MonoBehaviour;
				delegateTargets.Add(eachTarget);
			}
		}
	}
}
