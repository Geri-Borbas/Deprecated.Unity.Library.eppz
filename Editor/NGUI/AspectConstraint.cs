#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


namespace EPPZ.NGUI.Editor
{
	
	
	[CanEditMultipleObjects]
	[CustomEditor(typeof(EPPZ.NGUI.AspectConstraint), true)]
	public class AspectConstraint : EditorBase
	{
		
		
		protected override string headerName
		{ get { return "Aspect Constraint"; } }
		
		
		protected override void Draw()
		{
			EPPZ.NGUI.AspectConstraint targetConstraint = target as EPPZ.NGUI.AspectConstraint;
			targetConstraint.targetWidget = EditorGUILayout.ObjectField("Target Widget", targetConstraint.targetWidget, typeof(UIWidget), true) as UIWidget;

			SerializedProperty updateConstraint = serializedObject.FindProperty("updateConstraint");
			NGUIEditorTools.DrawProperty("Execute", updateConstraint, false, GUILayout.MinWidth(130.0f)); // NGUI like wording
			
			SerializedProperty keepAspectRatio = serializedObject.FindProperty("keepAspectRatio");
			NGUIEditorTools.DrawProperty("Aspect", keepAspectRatio, false, GUILayout.MinWidth(130.0f)); // NGUI like wording

			SerializedProperty multiplier = serializedObject.FindProperty("multiplier");
			NGUIEditorTools.DrawProperty("Multiplier", multiplier, false, GUILayout.MinWidth(130.0f));

			// Aspect layout alert.
			if (targetConstraint.widget.keepAspectRatio != UIWidget.AspectRatioSource.Free)
			{ EditorGUILayout.HelpBox("Constraint will have no effect. Please assign Free Aspect layout rule on the Widget, then set the desired Aspect behaviour in the constraint.", MessageType.Info); }
		}
	}
}
#endif


