#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;


namespace EPPZ.NGUI
{
	
	
	[CanEditMultipleObjects]
	[CustomEditor(typeof(EPPZNGUI_PositionConstraint), true)]
	public class EPPZNGUI_PositionConstraint_Editor : EPPZNGUI_Constraint_Editor
	{
		
		
		protected override string headerName
		{ get { return "Position Constraint"; } }
		
		
		protected override void Draw()
		{
			EPPZNGUI_PositionConstraint targetConstraint = target as EPPZNGUI_PositionConstraint;
			targetConstraint.targetWidget = EditorGUILayout.ObjectField("Target Widget", targetConstraint.targetWidget, typeof(UIWidget), true) as UIWidget;
		}
	}
}
#endif

