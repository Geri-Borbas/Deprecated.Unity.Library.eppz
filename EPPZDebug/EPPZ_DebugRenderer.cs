using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EPPZGeometry;


namespace EPPZDebug
{


	public class EPPZ_DebugRenderer : MonoBehaviour
	{

		
		public bool debugMode = false;

		public class DebugLine
		{
			public Vector2 from;
			public Vector2 to;
			public Material material;
		}
		protected List <DebugLine> debugLines = new List<DebugLine>();

				
		/**
		 * Internally invoked by `EPPZ_DebugCamera.OnPostRender` if any.
		 */
		public void OnDebugCameraPostRender(Camera camera_)
		{
			if (debugMode == false) return; // Only in debug mode
			
			GL.PushMatrix();
			GL.LoadProjectionMatrix(camera_.projectionMatrix);
			
			// Draw debug lines.
			foreach (DebugLine eachDebugLine in debugLines)
			{
				DrawLine(
					this.transform.TransformPoint(eachDebugLine.from),
					this.transform.TransformPoint(eachDebugLine.to),
					eachDebugLine.material
					);
			}

			OnDraw();
			
			GL.PopMatrix();
		}
		
		protected virtual void OnDraw()
		{
			// Subclass template.
		}

		public void AddDebugLine(Vector2 from, Vector2 to, Material material)
		{
			// Create.
			DebugLine debugLine = new DebugLine();
			debugLine.from = from;
			debugLine.to = to;
			debugLine.material = material;
			
			// Collect.
			debugLines.Add(debugLine);
		}

		protected void DrawSegment(Segment segment, Material material)	
		{ DrawLine(segment.a, segment.b, material); }

		protected void DrawPolygon(Polygon polygon, Material material)
		{ polygon.EnumerateEdgesRecursive((Edge eachEdge) => DrawLine(eachEdge.a, eachEdge.b, material)); }

		protected void DrawPolygonWithTransform(Polygon polygon, Material material, Transform transform_)
		{ DrawPolygonWithTransform (polygon, material, transform_, false); }

		protected void DrawPolygonWithTransform(Polygon polygon, Material material, Transform transform_, bool drawNormals)
		{
			polygon.EnumerateEdgesRecursive((Edge eachEdge) =>
			{
				DrawLineWithTransform(eachEdge.a, eachEdge.b, material, transform_);
				if (drawNormals)
				{
					Vector2 halfie = eachEdge.a + ((eachEdge.b - eachEdge.a) / 2.0f);
					DrawLineWithTransform(halfie, halfie + eachEdge.normal * 0.1f, material, transform_);
				}
			});
		}

		protected void DrawPoints(Vector2[] points, Material material)
		{ DrawPointsWithTransform(points, material, this.transform); }

		protected void DrawPointsWithTransform(Vector2[] points, Material material, Transform transform_)
		{
			for (int index = 0; index < points.Length; index++)
			{
				Vector2 eachPoint = points[index];
				Vector2 eachNextPoint = (index < points.Length - 1) ? points[index + 1] : points[0];
				
				// Apply shape transform.
				eachPoint = transform_.TransformPoint(eachPoint);
				eachNextPoint = transform_.TransformPoint(eachNextPoint);
				
				// Draw.
				DrawLine(eachPoint, eachNextPoint, material);
			}
		}

		protected void DrawRect(Rect rect, Material material)
		{ DrawRectWithTransform(rect, material, this.transform); }

		protected void DrawRectWithTransform(Rect rect, Material material, Transform transform_)
		{
			Vector2 leftTop = transform_.TransformPoint(new Vector2(rect.xMin, rect.yMin));
			Vector2 rightTop = transform_.TransformPoint(new Vector2(rect.xMax, rect.yMin));
			Vector2 rightBottom = transform_.TransformPoint(new Vector2(rect.xMax, rect.yMax));
			Vector2 leftBottom = transform_.TransformPoint(new Vector2(rect.xMin, rect.yMax));

			DrawLine(
				leftTop,
				rightTop,
				material);
			
			DrawLine(
				rightTop,
				rightBottom,
				material);
			
			DrawLine(
				rightBottom,
				leftBottom,
				material);
			
			DrawLine(
				leftTop,
				leftBottom,
				material);
		}

		private void DrawLineWithTransform(Vector2 from, Vector2 to, Material material, Transform transform_)
		{
			Vector2 from_ = transform_.TransformPoint(from);
			Vector2 to_ = transform_.TransformPoint(to);
			DrawLine (from_, to_, material);
		}

		protected void DrawLine(Vector2 from, Vector2 to, Material material)
		{
			if (Application.isPlaying)
			{
				Debug.DrawLine(new Vector3(from.x, from.y, 0.0f), new Vector3(to.x, to.y, 0.0f), material.color);

				material.SetPass(0);
				GL.Color(Color.white);
				
				GL.Begin(GL.LINES);
				GL.Vertex (new Vector3(from.x, from.y, 0.0f));
				GL.Vertex (new Vector3(to.x, to.y, 0.0f));
				GL.End();
			}
		}
	}
}