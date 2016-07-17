#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


namespace EPPZ.NGUI.Editor
{
	
	
	[CanEditMultipleObjects]
	public class EditorBase : UnityEditor.Editor
	{
		
		
		protected virtual string headerName
		{ get { return "Constraint"; } }


		public override void OnInspectorGUI()
		{
			NGUIEditorTools.SetLabelWidth(80.0f);
			EditorGUILayout.Space();
			
			serializedObject.Update();
			if (NGUIEditorTools.DrawHeader(headerName))
			{
				NGUIEditorTools.BeginContents();
				Draw();
				NGUIEditorTools.EndContents();
			}
			serializedObject.ApplyModifiedProperties();
		}

		protected virtual void Draw()
		{ }
	}
}
#endif