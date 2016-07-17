#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


namespace EPPZ.NGUI.Editor
{
	
	
	[CanEditMultipleObjects]
	[CustomEditor(typeof(EPPZ.NGUI.ScrollViewPageControl), true)]
	public class ScrollViewPageControl : EditorBase
	{
		
		
		protected override string headerName
		{ get { return "Scroll View Page Control"; } }
		
		
		protected override void Draw()
		{
			NGUIEditorTools.SetLabelWidth(160.0f); // Space for the lyric

			SerializedProperty scrollViewPaging = serializedObject.FindProperty("scrollViewPaging");
			NGUIEditorTools.DrawProperty("Scroll View Paging", scrollViewPaging, false, GUILayout.MinWidth(130.0f));

			SerializedProperty spritePrototypeObject = serializedObject.FindProperty("spritePrototypeObject");
			NGUIEditorTools.DrawProperty("Sprite Prototype GameObject", spritePrototypeObject, false, GUILayout.MinWidth(130.0f));
			
			SerializedProperty spriteSpacing = serializedObject.FindProperty("spriteSpacing");
			NGUIEditorTools.DrawProperty("Sprite Spacing (%)", spriteSpacing, false, GUILayout.MinWidth(130.0f));
			
			SerializedProperty normalColor = serializedObject.FindProperty("normalColor");
			NGUIEditorTools.DrawProperty("Normal Color", normalColor, false, GUILayout.MinWidth(130.0f));

			SerializedProperty selectedColor = serializedObject.FindProperty("selectedColor");
			NGUIEditorTools.DrawProperty("Selected Color", selectedColor, false, GUILayout.MinWidth(130.0f));
		}
	}
}
#endif

