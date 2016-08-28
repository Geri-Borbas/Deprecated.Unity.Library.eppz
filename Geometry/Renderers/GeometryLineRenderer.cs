using UnityEngine;
using System.Collections;
using EPPZ.Geometry;


namespace EPPZ.Lines
{


	public class GeometryLineRenderer : DirectLineRenderer
	{


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
	}
}

