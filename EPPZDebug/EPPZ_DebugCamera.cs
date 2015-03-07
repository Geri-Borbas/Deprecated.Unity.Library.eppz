using UnityEngine;
using System.Collections;


namespace EPPZDebug
{


	/**
	 * Should be added to any camera that renders the scene.
	 * All the DebugRenderer instances collected on awake
	 * gonna be notified by this component after this camera
	 * finished rendering.
	 */
	public class EPPZ_DebugCamera : MonoBehaviour
	{


		private EPPZ_DebugRenderer[] debugRenderers;


		void Awake()
		{
			// Collect every debug renderer in the scene.
			debugRenderers = FindObjectsOfType(typeof(EPPZ_DebugRenderer)) as EPPZ_DebugRenderer[];
		}

		void OnPostRender()
		{
			foreach (EPPZ_DebugRenderer eachDebugRenderer in debugRenderers)
			{
				eachDebugRenderer.OnDebugCameraPostRender(this.GetComponent<Camera>());
			}
		}
	}
}