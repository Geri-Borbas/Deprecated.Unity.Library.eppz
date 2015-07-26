#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


namespace EPPZ.NGUI
{


	[CanEditMultipleObjects]
	[CustomEditor(typeof(EPPZNGUI_SizeConstraint), true)]
	public class EPPZNGUI_SizeConstraint_Editor : EPPZNGUI_Constraint_Editor
	{


		protected override string headerName
		{ get { return "Size Constraint"; } }


		protected override void Draw()
		{
			EPPZNGUI_SizeConstraint targetConstraint = target as EPPZNGUI_SizeConstraint;
			targetConstraint.targetWidget = EditorGUILayout.ObjectField("Target Widget", targetConstraint.targetWidget, typeof(UIWidget), true) as UIWidget;

			SerializedProperty constraint = serializedObject.FindProperty("constraint");
			NGUIEditorTools.DrawProperty("Constraint", constraint, false, GUILayout.MinWidth(130.0f));
			
			SerializedProperty multiplier = serializedObject.FindProperty("multiplier");
			NGUIEditorTools.DrawProperty("Multiplier", multiplier, false, GUILayout.MinWidth(130.0f));
		}
	}
}
#endif
