#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


namespace EPPZ.NGUI
{
	
	
	[CanEditMultipleObjects]
	[CustomEditor(typeof(EPPZNGUI_AspectConstraint), true)]
	public class EPPZNGUI_AspectConstraint_Editor : EPPZNGUI_Constraint_Editor
	{
		
		
		protected override string headerName
		{ get { return "Aspect Constraint"; } }
		
		
		protected override void Draw()
		{
			EPPZNGUI_AspectConstraint targetConstraint = target as EPPZNGUI_AspectConstraint;
			targetConstraint.targetWidget = EditorGUILayout.ObjectField("Target Widget", targetConstraint.targetWidget, typeof(UIWidget), true) as UIWidget;

			SerializedProperty updateConstraint = serializedObject.FindProperty("updateConstraint");
			NGUIEditorTools.DrawProperty("Execute", updateConstraint, false, GUILayout.MinWidth(130.0f)); // NGUI like wording

			SerializedProperty multiplier = serializedObject.FindProperty("multiplier");
			NGUIEditorTools.DrawProperty("Multiplier", multiplier, false, GUILayout.MinWidth(130.0f));

			// Aspect layout message if needed.
			if (targetConstraint.widget.keepAspectRatio == UIWidget.AspectRatioSource.Free)
			{ EditorGUILayout.HelpBox("Constraint will have no effect. Widget Aspect is set to Free, please assign an Aspect layout rule (either Based On Width or Based On Height).", MessageType.Info); }
		}
	}
}
#endif


