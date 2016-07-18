#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


namespace EPPZ.NGUI.Editor
{
	
	
	[CanEditMultipleObjects]
	[CustomEditor(typeof(EPPZ.NGUI.PositionConstraint), true)]
	public class PositionConstraint : EditorBase
	{
		
		
		protected override string headerName
		{ get { return "Position Constraint"; } }
		
		
		protected override void Draw()
		{
			EPPZ.NGUI.PositionConstraint targetConstraint = target as EPPZ.NGUI.PositionConstraint;
			targetConstraint.targetWidget = EditorGUILayout.ObjectField("Target Widget", targetConstraint.targetWidget, typeof(UIWidget), true) as UIWidget;

			SerializedProperty updateConstraint = serializedObject.FindProperty("updateConstraint");
			NGUIEditorTools.DrawProperty("Execute", updateConstraint, false, GUILayout.MinWidth(130.0f)); // NGUI like wording
		}
	}
}
#endif

