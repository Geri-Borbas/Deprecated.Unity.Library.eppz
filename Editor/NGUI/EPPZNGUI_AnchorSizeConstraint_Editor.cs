#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


namespace EPPZ.NGUI
{
	
	
	[CanEditMultipleObjects]
	[CustomEditor(typeof(EPPZNGUI_AnchorSizeConstraint), true)]
	public class EPPZNGUI_AnchorSizeConstraint_Editor : EPPZNGUI_Constraint_Editor
	{
		
		
		protected override string headerName
		{ get { return "Anchor Size Constraint"; } }
		
		
		protected override void Draw()
		{
			EPPZNGUI_AnchorSizeConstraint targetConstraint = target as EPPZNGUI_AnchorSizeConstraint;
			targetConstraint.targetWidget = EditorGUILayout.ObjectField("Target Widget", targetConstraint.targetWidget, typeof(UIWidget), true) as UIWidget;
			
			SerializedProperty updateConstraint = serializedObject.FindProperty("updateConstraint");
			NGUIEditorTools.DrawProperty("Execute", updateConstraint, false, GUILayout.MinWidth(130.0f)); // NGUI like wording
			
			SerializedProperty constraint = serializedObject.FindProperty("constraint");
			NGUIEditorTools.DrawProperty("Constraint", constraint, false, GUILayout.MinWidth(130.0f));
			
			SerializedProperty multiplier = serializedObject.FindProperty("multiplier");
			NGUIEditorTools.DrawProperty("Multiplier", multiplier, false, GUILayout.MinWidth(130.0f));
		}
	}
}
#endif