using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using ClipperLib;


namespace EPPZ.Geometry
{


	// Clipper definitions.
	using Path = List<IntPoint>;
	using Paths = List<List<IntPoint>>;


	public static class Geometry_Clipper
	{


	#region Polygon

		public static Polygon PolygonFromClipperPaths(Paths paths, float scale)
		{
			Polygon polygon = null;
			for (int index = 0; index < paths.Count; index++)
			{
				Path eachPath = paths[index];
				Polygon eachPolygon = PolygonFromClipperPath(eachPath, scale);

				if (index == 0)
				{ polygon = Polygon.PolygonWithPoints(eachPolygon.points); } // Parent polygon
				else
				{ polygon.AddPolygon(eachPolygon); } // Child polygons
			}
			return polygon;
		}

		public static Polygon PolygonFromClipperPath(Path path, float scale)
		{
			List<Vector2> points = new List<Vector2>();
			for (int index = path.Count - 1; index >= 0; index--) // Reverse enumeration (to flip normals)
			{
				IntPoint eachPoint = path[index];
				points.Add(new Vector2(eachPoint.X / scale, eachPoint.Y / scale));
			}
			return Polygon.PolygonWithPointList(points);
		}

		public static Paths ClipperPaths(this Polygon this_, float scale)
		{
			Paths paths = new Paths();
			this_.EnumeratePolygons((Polygon eachPolygon) =>
			{
				paths.Add(eachPolygon.ClipperPath(scale));
			});
			return paths;    
		}

		public static Path ClipperPath(this Polygon this_, float scale)
		{
			Path path = new Path();
			this_.EnumeratePoints((Vector2 eachPoint) =>
			{
				path.Add(new IntPoint(
					eachPoint.x * scale,
					eachPoint.y * scale
				));
			});
			return path;
		}

	#endregion


	#region Generic

		public static Vector2[] PointsFromClipperPath(Path path, float scale)
		{
			List<Vector2> points = new List<Vector2>();
			for (int index = path.Count - 1; index >= 0; index--) // Reverse enumeration (to flip normals)
			{
				IntPoint eachPoint = path[index];
				points.Add(new Vector2(eachPoint.X / scale, eachPoint.Y / scale));
			}
			return points.ToArray();
		}


	#endregion

	}
}

