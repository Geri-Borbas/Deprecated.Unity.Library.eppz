#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using EPPZ.UI;


[CustomEditor(typeof(EPPZUI_ScrollRect))]
public class EPPZUI_ScrollRect_Editor : Editor
{


	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
	}
}
#endif
