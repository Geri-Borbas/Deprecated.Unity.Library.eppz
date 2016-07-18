#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


namespace EPPZ.NGUI.Editor
{
	
	
	[CanEditMultipleObjects]
	[CustomEditor(typeof(EPPZ.NGUI.AnchorSizeConstraint), true)]
	public class AnchorSizeConstraint : EditorBase
	{
		
		
		protected override string headerName
		{ get { return "Anchor Size Constraint"; } }
		
		
		protected override void Draw()
		{
			EPPZ.NGUI.AnchorSizeConstraint targetConstraint = target as EPPZ.NGUI.AnchorSizeConstraint;
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