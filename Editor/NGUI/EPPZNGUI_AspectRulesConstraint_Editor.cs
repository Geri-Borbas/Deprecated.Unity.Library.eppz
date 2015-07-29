#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


namespace EPPZ.NGUI
{
	
	
	[CanEditMultipleObjects]
	[CustomEditor(typeof(EPPZNGUI_AspectRulesConstraint), true)]
	public class EPPZNGUI_AspectRulesConstraint_Editor : EPPZNGUI_Constraint_Editor
	{
		

		protected override string headerName
		{ get { return "Aspect Rules Constraint"; } }
		
		
		protected override void Draw()
		{			
			EditorGUILayout.PropertyField(serializedObject.FindProperty("aspectRules"), new GUIContent("Aspect Rules"), true);
		}
	}
}
#endif