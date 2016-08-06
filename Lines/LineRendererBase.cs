using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using EPPZ.Geometry;


namespace EPPZ.Lines
{


	/**
	 * Base class for two types of line renderers.
	 * Not meant for direct client useage.
	 */
	public class LineRendererBase : MonoBehaviour
	{


		// Preserve backward compatibility.
		[FormerlySerializedAs("debugMode")]
		[HideInInspector]
		public bool isActive = true;


		#region Events

			// Internally invoked by `LineRendererCamera.OnPostRender` (if any).
			public virtual void OnLineRendererCameraPostRender()
			{
				// Subclass template.
			}

		#endregion


		#region Batch lines

			protected virtual void DrawLine(Vector2 from, Vector2 to, Color color)
			{ 
				// Subclass template.
			}

		#endregion


		#region `EPPZ.Geometry` drawing methods

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

			protected void DrawCircle(Vector2 center, float radius, int segments, Color color)
			{ DrawCircleWithTransform(center, radius, segments, color, this.transform); }

			protected void DrawCircleWithTransform(Vector2 center, float radius, int segments, Color color, Transform transform)
			{
				Vector2[] vertices = new Vector2[segments];

				// Compose a half circle (and mirrored) in normalized space (at 0,0).
				float angularStep = Mathf.PI * 2.0f / (float)segments;
				for (int index = 0; index < 1 + segments / 2; index++)
				{
					// Trigonometry.
					float angle = (float)index * angularStep;
					float x = Mathf.Sin(angle);
					float y = Mathf.Cos(angle);

					Vector2 vertex = new Vector2(x * radius, y * radius);
					Vector2 mirrored =  new Vector2(-x * radius, y * radius);

					// Save right, then left.
					vertices[index] = vertex;
					if (index > 0) vertices[segments-index] = mirrored;
				}

				// Draw around center.
				for (int index = 0; index < segments - 1; index++)
				{
					DrawLineWithTransform(
						center + vertices[index],
						center + vertices[index + 1],
						color,
						transform
					);
				}

				// Last segment.
				DrawLineWithTransform(
					center + vertices[segments - 1],
					center + vertices[0],
					color,
					transform
				);
			}

			protected void DrawLineWithTransform(Vector2 from, Vector2 to, Color color, Transform transform_)
			{
				Vector2 from_ = transform_.TransformPoint(from);
				Vector2 to_ = transform_.TransformPoint(to);
				DrawLine (from_, to_, color);
			}

		#endregion


	}
}
