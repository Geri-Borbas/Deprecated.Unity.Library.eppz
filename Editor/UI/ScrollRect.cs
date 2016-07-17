#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;


namespace EPPZ.UI.Editor
{


	[CustomEditor(typeof(EPPZ.UI.ScrollRect))]
	public class ScrollRect : UnityEditor.Editor
	{


		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
		}
	}
}
#endif
