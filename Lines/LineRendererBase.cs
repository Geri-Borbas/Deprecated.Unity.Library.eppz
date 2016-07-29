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
		public bool isActive = false;


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

			protected void DrawLineWithTransform(Vector2 from, Vector2 to, Color color, Transform transform_)
			{
				Vector2 from_ = transform_.TransformPoint(from);
				Vector2 to_ = transform_.TransformPoint(to);
				DrawLine (from_, to_, color);
			}

		#endregion


	}
}
