using UnityEngine;
using System.Collections;


namespace EPPZ.DebugTools
{


	/**
	 * Should be added to any camera that renders the scene.
	 * All the DebugRenderer instances collected on awake
	 * gonna be notified by this component after this camera
	 * finished rendering.
	 */
	public class DebugCamera : MonoBehaviour
	{


		private DebugRenderer[] debugRenderers;


		void Awake()
		{
			// Collect every debug renderer in the scene.
			debugRenderers = FindObjectsOfType(typeof(DebugRenderer)) as DebugRenderer[];
		}

		void OnPostRender()
		{
			foreach (DebugRenderer eachDebugRenderer in debugRenderers)
			{
				eachDebugRenderer.OnDebugCameraPostRender(this.GetComponent<Camera>());
			}
		}
	}
}