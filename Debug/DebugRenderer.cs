using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EPPZ.Geometry;


namespace EPPZ.DebugTools
{


	public class DebugRenderer : MonoBehaviour
	{
		

		public bool debugMode = false;

		// Material for drawing.
		Material _material;
		Material material
		{
			get
			{ 
				if (_material == null) _material = new Material(Shader.Find("eppz!/Vertex color"));
				return _material;
			}
		}

		public class DebugLine
		{
			public Vector2 from;
			public Vector2 to;
			public Color color;
		}
		protected List<DebugLine> lines = new List<DebugLine>();

				
		/**
		 * Internally invoked by `EPPZ_DebugCamera.OnPostRender` if any.
		 */
		public void OnDebugCameraPostRender(Camera camera_)
		{
			if (debugMode == false) return; // Only in debug mode
			
			GL.PushMatrix();
			GL.LoadProjectionMatrix(camera_.projectionMatrix);

			// Clear line batch.
			lines.Clear();

			// Collect more lines to the batch.
			OnDraw();

			// Do it.
			DrawCall();
			
			GL.PopMatrix();
		}
		
		protected virtual void OnDraw()
		{
			// Subclass template.
		}

		public void AddDebugLine(Vector2 from, Vector2 to, Color color)
		{
			// Create.
			DebugLine debugLine = new DebugLine();
			debugLine.from = from;
			debugLine.to = to;
			debugLine.color = color;
			
			// Collect.
			lines.Add(debugLine);
		}

		protected void DrawSegment(Segment segment, Color color)	
		{ DrawLine(segment.a, segment.b, color); }

		protected void DrawPolygon(Polygon polygon, Color color)
		{ polygon.EnumerateEdgesRecursive((Edge eachEdge) => DrawLine(eachEdge.a, eachEdge.b, color)); }

		protected void DrawPolygonWithTransform(Polygon polygon, Color color, Transform transform_)
		{ DrawPolygonWithTransform(polygon, color, transform_, false); }

		protected void DrawPolygonWithTransform(Polygon polygon, Color color, Transform transform_, bool drawNormals)
		{
			polygon.EnumerateEdgesRecursive((Edge eachEdge) =>
			{
				DrawLineWithTransform(eachEdge.a, eachEdge.b, color, transform_);
				if (drawNormals)
				{
					Vector2 halfie = eachEdge.a + ((eachEdge.b - eachEdge.a) / 2.0f);
					DrawLineWithTransform(halfie, halfie + eachEdge.normal * 0.1f, color, transform_);
				}
			});
		}

		protected void DrawPoints(Vector2[] points, Color color)
		{ DrawPointsWithTransform(points, color, this.transform); }

		protected void DrawPointsWithTransform(Vector2[] points, Color color, Transform transform_)
		{
			for (int index = 0; index < points.Length; index++)
			{
				Vector2 eachPoint = points[index];
				Vector2 eachNextPoint = (index < points.Length - 1) ? points[index + 1] : points[0];
				
				// Apply shape transform.
				eachPoint = transform_.TransformPoint(eachPoint);
				eachNextPoint = transform_.TransformPoint(eachNextPoint);
				
				// Draw.
				DrawLine(eachPoint, eachNextPoint, color);
			}
		}

		protected void DrawRect(Rect rect, Color color)
		{ DrawRectWithTransform(rect, color, this.transform); }

		protected void DrawRectWithTransform(Rect rect, Color color, Transform transform_)
		{
			Vector2 leftTop = transform_.TransformPoint(new Vector2(rect.xMin, rect.yMin));
			Vector2 rightTop = transform_.TransformPoint(new Vector2(rect.xMax, rect.yMin));
			Vector2 rightBottom = transform_.TransformPoint(new Vector2(rect.xMax, rect.yMax));
			Vector2 leftBottom = transform_.TransformPoint(new Vector2(rect.xMin, rect.yMax));

			DrawLine(
				leftTop,
				rightTop,
				color);
			
			DrawLine(
				rightTop,
				rightBottom,
				color);
			
			DrawLine(
				rightBottom,
				leftBottom,
				color);
			
			DrawLine(
				leftTop,
				leftBottom,
				color);
		}

		private void DrawLineWithTransform(Vector2 from, Vector2 to, Color color, Transform transform_)
		{
			Vector2 from_ = transform_.TransformPoint(from);
			Vector2 to_ = transform_.TransformPoint(to);
			DrawLine (from_, to_, color);
		}

		protected void DrawLine(Vector2 from, Vector2 to, Color color)
		{
			if (Application.isPlaying)
			{
				Debug.DrawLine(new Vector3(from.x, from.y, 0.0f), new Vector3(to.x, to.y, 0.0f), color);

				// Create and collect.
				DebugLine line = new DebugLine();
				line.from = from;
				line.to = to;
				line.color = color;
				lines.Add(line);
			}
		}

		void DrawCall()
		{
			// Assign vertex color material.
			material.SetPass(0); // Single draw call (set pass call)

			// Send vertices in immediate mode.
			GL.Begin(GL.LINES);
			Vector3 cursor = Vector3.zero;
			foreach (DebugLine eachLine in lines)
			{
				Vector3 eachFrom = new Vector3 (eachLine.from.x, eachLine.from.y, 0.0f);
				Vector3 eachTo = new Vector3 (eachLine.to.x, eachLine.to.y, 0.0f);

				// Fake "MoveTo" (if needed).
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