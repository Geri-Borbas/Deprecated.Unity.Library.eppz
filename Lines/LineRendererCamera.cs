using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace EPPZ.Lines
{


	/**
	 * Should be added to any camera that renders the scene.
	 * All the EPPZ.Lines.LineRenderer instances (collected
	 * on Awake) gonna be notified by this component after
	 * this camera finished rendering.
	 */
	public class LineRendererCamera : MonoBehaviour
	{


		// Singleton access.
		public static LineRendererCamera shared;

		// Camera.
		private Camera _camera;

		// Renderers.
		private DirectLineRenderer[] directLineRenderers;
		private CachedLineRenderer[] cachedLineRenderers;
		private List<EPPZ.Lines.Line> lineBatch = new List<EPPZ.Lines.Line>(); 

		// Material for drawing (lazy).
		Material _material;
		Material material
		{
			get
			{ 
				if (_material == null) _material = new Material(Shader.Find("eppz!/Vertex color"));
				return _material;
			}
		}


		void Awake()
		{
			shared = this; 
			_camera = GetComponent<Camera>();

			// Collect every debug renderer in the scene.
			directLineRenderers = FindObjectsOfType(typeof(EPPZ.Lines.DirectLineRenderer)) as EPPZ.Lines.DirectLineRenderer[];
			cachedLineRenderers = FindObjectsOfType(typeof(EPPZ.Lines.CachedLineRenderer)) as EPPZ.Lines.CachedLineRenderer[];
		}

		void OnPreRender()
		{ }

		void Update()
		{
			BatchLines();
			DrawDebugLines();
		}

		void BatchLines()
		{
			// Flush.
			lineBatch.Clear();

			// Batch lines from direct renderers.
			foreach (EPPZ.Lines.DirectLineRenderer eachDirectLineRenderer in directLineRenderers)
			{ eachDirectLineRenderer.OnLineRendererCameraPostRender(); }

			// Add up line collections from cached renderers.
			foreach (EPPZ.Lines.CachedLineRenderer eachCachedLineRenderer in cachedLineRenderers)
			{ lineBatch.AddRange(eachCachedLineRenderer.lines); }
		}

		void OnPostRender()
		{
			GL.PushMatrix();
			GL.LoadProjectionMatrix(_camera.projectionMatrix);
			DrawCall();
			GL.PopMatrix();
		}

		public void BatchLine(EPPZ.Lines.Line line)
		{ lineBatch.Add(line); }

		void DrawDebugLines()
		{
			foreach (EPPZ.Lines.Line eachLine in lineBatch)
			{
				// Draw in Scene view.
				Debug.DrawLine(eachLine.from, eachLine.to, eachLine.color);
			}
		}

		void DrawCall()
		{
			// Assign vertex color material.
			material.SetPass(0); // Single draw call (set pass call)

			// Send vertices in GL_LINES Immediate Mode.
			GL.Begin(GL.LINES);
			foreach (EPPZ.Lines.Line eachLine in lineBatch)
			{
				GL.Color(eachLine.color);
				GL.Vertex(eachLine.from);
				GL.Vertex(eachLine.to);
			}
			GL.End();
		}
	}
}