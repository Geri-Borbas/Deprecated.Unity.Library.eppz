using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace EPPZ.NGUI
{
	
	
	/// <summary>
	/// Layout tool that binds a foreign widget aspect ratio to this widget.
	/// </summary>
	
	
	[ExecuteInEditMode]
	[AddComponentMenu("eppz!/NGUI/Aspect Rules Constraint")]
	public class EPPZNGUI_AspectRulesConstraint : EPPZNGUI_Constraint
	{
		
		
		/// <summary>
		/// List of Aspect rules to apply when Widget aspect has changed.
		/// </summary>
		[System.Serializable]
		public class AspectRule
		{


			public enum Relation { GreaterThan, GreaterThanOrEquals, LessThan, LessThanOrEquals, Equals }
			public Relation relation;
			public float aspect;
			public List<MonoBehaviour> components = new List<MonoBehaviour>();
			public List<GameObject> gameObjects = new List<GameObject>();


			bool EvaluateAspect(float aspect_)
			{
				bool result = false;
				switch (relation)
				{
					case Relation.GreaterThan: result = (aspect_ > aspect); break;
					case Relation.GreaterThanOrEquals: result = (aspect_ >= aspect); break;
					case Relation.LessThan: result = (aspect_ < aspect); break;
					case Relation.LessThanOrEquals: result = (aspect_ <= aspect); break;
					case Relation.Equals: result = (aspect_ == aspect); break;
				}
				return result;
			}

			public void Execute(float aspect_)
			{
				bool result = EvaluateAspect(aspect_);

				// Switch Components.
				foreach(MonoBehaviour eachComponent in components)
				{
					if (eachComponent == null) continue;
					eachComponent.enabled = result;
				}

				// Switch GameObjects.
				foreach(GameObject eachGameObject in gameObjects)
				{
					if (eachGameObject == null) continue;
					eachGameObject.SetActive(result);
				}
			}
		}
		public List<AspectRule> aspectRules = new List<AspectRule>();
		
		/// <summary>
		/// Tracking changes to spare layout calculations.
		/// </summary>
		private int _previousWidgetWidth;
		private int _previousWidgetHeight;
		

		protected override bool ShouldLayout()
		{
			return true; // Layout even without target widget
		}

		protected override void Layout()
		{
			// Adjust only if something has changed.
			bool widgetChanged = (
				(widget.width != _previousWidgetWidth) ||
				(widget.height != _previousWidgetHeight)
				);
			if (widgetChanged == false) return;
			
			Adjust();
			
			// Track changes.
			_previousWidgetWidth = widget.width;
			_previousWidgetHeight = widget.height;
		}
		
		void Adjust()
		{
			// Calculate new size.
			float aspect = (float)widget.width / (float)widget.height; // Calculate aspect on the fly (!)

			foreach (AspectRule eachAspectRule in aspectRules)
			{ eachAspectRule.Execute(aspect); }

		}
	}
}
