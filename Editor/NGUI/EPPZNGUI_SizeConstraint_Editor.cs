#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


namespace EPPZ.NGUI
{


	[CanEditMultipleObjects]
	[CustomEditor(typeof(EPPZNGUI_SizeConstraint), true)]
	public class EPPZNGUI_SizeConstraint_Editor : Editor
	{


		public override void OnInspectorGUI()
		{
			NGUIEditorTools.SetLabelWidth(80.0f);
			EditorGUILayout.Space();
			serializedObject.Update();
			DrawSizeConstraint();
			serializedObject.ApplyModifiedProperties();
		}

		private void DrawSizeConstraint()
		{
			if (NGUIEditorTools.DrawHeader("Size Constraint"))
			{
				NGUIEditorTools.BeginContents();

					// Properties.
					EPPZNGUI_SizeConstraint sizeConstraint = target as EPPZNGUI_SizeConstraint;
					sizeConstraint.targetWidget = EditorGUILayout.ObjectField("Target Widget", sizeConstraint.targetWidget, typeof(UIWidget), true) as UIWidget;

					// Fields.
					SerializedProperty constraint = serializedObject.FindProperty("constraint");
					NGUIEditorTools.DrawProperty("Constraint", constraint, false, GUILayout.MinWidth(130.0f));

					SerializedProperty multiplier = serializedObject.FindProperty("multiplier");
					NGUIEditorTools.DrawProperty("Multiplier", multiplier, false, GUILayout.MinWidth(130.0f));
				
				NGUIEditorTools.EndContents();
			}
		}
	}
}
#endif
