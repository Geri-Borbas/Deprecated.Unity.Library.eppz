#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;


namespace EPPZ.EditorTools
{


	public class Editor_Snap : ScriptableObject
	{	
		[MenuItem ("eppz!/Snap center to Grid &%g")] // Alt + CMD + G
		static void MenuSnapToGrid()
		{
			foreach (Transform eachTransform in Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable))
			{
				eachTransform.position = new Vector3(
					Mathf.Round(eachTransform.position.x / EditorPrefs.GetFloat("MoveSnapX")) * EditorPrefs.GetFloat("MoveSnapX"),
					Mathf.Round(eachTransform.position.y / EditorPrefs.GetFloat("MoveSnapY")) * EditorPrefs.GetFloat("MoveSnapY"),
					Mathf.Round(eachTransform.position.z / EditorPrefs.GetFloat("MoveSnapZ")) * EditorPrefs.GetFloat("MoveSnapZ")
					);
			}
		}

		[MenuItem ("eppz!/Snap Bounds to Origin &%b")] // Alt + CMD + B
		static void MenuSnapBoundsToGrid()
		{
			foreach (Transform eachTransform in Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable))
			{
				if (eachTransform.gameObject.GetComponent<Renderer>() == null) continue; // Only if any renderer

				eachTransform.position = new Vector3(
					eachTransform.position.x - eachTransform.gameObject.GetComponent<Renderer>().bounds.center.x,
					eachTransform.position.y - eachTransform.gameObject.GetComponent<Renderer>().bounds.center.y,
					eachTransform.position.z - eachTransform.gameObject.GetComponent<Renderer>().bounds.center.z
					);
			}
		}
	}
}
#endif