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
			NGUIEditorTools.SetLabelWidth(80f);
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
				
					SerializedProperty target = serializedObject.FindProperty("target");
					NGUIEditorTools.DrawProperty("Target", target, false, GUILayout.MinWidth(130f));

					SerializedProperty constraint = serializedObject.FindProperty("constraint");
					NGUIEditorTools.DrawProperty("Constraint", constraint, false, GUILayout.MinWidth(130f));

					SerializedProperty multiplier = serializedObject.FindProperty("multiplier");
					NGUIEditorTools.DrawProperty("Multiplier", multiplier, false, GUILayout.MinWidth(130f));
				
				NGUIEditorTools.EndContents();
			}
		}
	}
}
#endif
