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


	using ExecutionOrder;


	[ExecutionOrder (1000)]
	public class LineRendererCamera : MonoBehaviour
	{


		// Singleton access.
		public static LineRendererCamera shared;

		// Camera.
		private Camera _camera;

		// Renderers.
		private List<DirectLineRenderer> directLineRenderers = new List<DirectLineRenderer>();
		private List<CachedLineRenderer> cachedLineRenderers = new List<CachedLineRenderer>();
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


		public static void AddDirectRenderer(DirectLineRenderer renderer)
		{ shared.directLineRenderers.Add(renderer); }

		public static void AddCachedRenderer(CachedLineRenderer renderer)
		{ shared.cachedLineRenderers.Add(renderer); }

		void Awake()
		{
			shared = this; 
			_camera = GetComponent<Camera>();
		}

		void OnPreRender()
		{ }

		void Update()
		{
			BatchLines();
			DrawLines();
		}

		void BatchLines()
		{
			// Flush.
			lineBatch.Clear();

			// Batch lines from direct renderers.
			foreach (EPPZ.Lines.DirectLineRenderer eachDirectLineRenderer in directLineRenderers)
			{
				if (eachDirectLineRenderer == null) continue;
				eachDirectLineRenderer.OnLineRendererCameraPostRender();
			}

			// Add up line collections from cached renderers.
			foreach (EPPZ.Lines.CachedLineRenderer eachCachedLineRenderer in cachedLineRenderers)
			{
				if (eachCachedLineRenderer == null) continue;
				lineBatch.AddRange(eachCachedLineRenderer.lines);
			}
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

		void DrawLines()
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