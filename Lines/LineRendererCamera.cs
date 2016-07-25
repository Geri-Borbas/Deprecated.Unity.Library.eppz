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
		private LineRenderer[] lineRenderers;
		private List<LineRenderer.Line> lines = new List<LineRenderer.Line>(); 

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
			lineRenderers = FindObjectsOfType(typeof(EPPZ.Lines.LineRenderer)) as EPPZ.Lines.LineRenderer[];
		}

		void OnPreRender()
		{ }

		void Update()
		{
			// Flush line batch.
			lines.Clear();

			// Batch lines from renderers.
			foreach (EPPZ.Lines.LineRenderer eachDebugRenderer in lineRenderers)
			{ eachDebugRenderer.OnLineRendererCameraPostRender(); }

			// Draw debug lines before render.
			DrawDebugLines();
		}

		void OnPostRender()
		{
			GL.PushMatrix();
			GL.LoadProjectionMatrix(_camera.projectionMatrix);

			// Draw line batch.
			DrawCall();

			GL.PopMatrix();
		}

		public void BatchLine(EPPZ.Lines.LineRenderer.Line line)
		{ lines.Add(line); }

		void DrawDebugLines()
		{
			foreach (EPPZ.Lines.LineRenderer.Line eachLine in lines)
			{
				Vector3 eachFrom = new Vector3 (eachLine.from.x, eachLine.from.y, 0.0f);
				Vector3 eachTo = new Vector3 (eachLine.to.x, eachLine.to.y, 0.0f);

				// Draw in Scene view.
				Debug.DrawLine(eachFrom, eachTo, eachLine.color);
			}
		}

		void DrawCall()
		{
			// Assign vertex color material.
			material.SetPass(0); // Single draw call (set pass call)

			// Send vertices in immediate mode.
			GL.Begin(GL.LINES);
			Vector3 cursor = Vector3.zero;
			foreach (EPPZ.Lines.LineRenderer.Line eachLine in lines)
			{
				Vector3 eachFrom = new Vector3 (eachLine.from.x, eachLine.from.y, 0.0f);
				Vector3 eachTo = new Vector3 (eachLine.to.x, eachLine.to.y, 0.0f);

				// Faking "MoveTo" (if needed).
				if (eachFrom != cursor)
				{
					GL.Color(Color.clear);
					GL.Vertex (cursor);
					GL.Vertex (eachFrom);
				}

				// Draw actual line.
				GL.Color(eachLine.color);
				GL.Vertex(eachFrom);
				GL.Vertex(eachTo);

				// Adjust "Caret".
				cursor = eachTo;
			}
			GL.End();
		}
	}
}