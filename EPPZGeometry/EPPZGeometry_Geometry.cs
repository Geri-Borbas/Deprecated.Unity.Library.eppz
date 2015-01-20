using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace EPPZGeometry
{


	public static class Geometry
	{


		// --


		// Determine if `bounds_` size fits into `bounds`.
		public static bool IsBoundsContainsBoundsSize(Rect bounds, Rect bounds_) // Compare sizes only 
		{
			return Geometry.IsBoundsContainsBoundsSize(bounds, bounds_, 0.0f);
		}

		// Determine if `bounds_` size fits into `bounds` with a given accuracy.
		public static bool IsBoundsContainsBoundsSize(Rect bounds, Rect bounds_, float accuracy) // Compare sizes only 
		{
			return (
				Mathf.Abs(bounds.width + accuracy * 2.0f) >= Mathf.Abs(bounds_.width)) &&
				(Mathf.Abs(bounds.height + accuracy * 2.0f) >= Mathf.Abs(bounds_.height)
				 );
		}

		// Determine if `bounds_` is contained by `bounds` (even if permiters are touching).
		public static bool IsBoundsContainsBounds(Rect bounds, Rect bounds_)
		{ return Geometry.IsBoundsContainsBounds(bounds, bounds_, 0.0f); }

		public static bool IsBoundsContainsBounds(Rect bounds, Rect bounds_, float accuracy)
		{
			bool xMin = (bounds.xMin - accuracy <= bounds_.xMin);
			bool xMax = (bounds.xMax + accuracy >= bounds_.xMax);
			bool yMin = (bounds.yMin - accuracy <= bounds_.yMin);
			bool yMax = (bounds.yMax + accuracy >= bounds_.yMax);
			return xMin && xMax && yMin && yMax;
		}

		// Determine if bounds are touching.
		public static bool IsBoundsTouchesBounds(Rect bounds, Rect bounds_)
		{ return false; }

		// Determine winding direction of three points.
		public static bool ArePointsCCW(Vector2 a, Vector2 b, Vector2 c)
		{
			return ((c.y - a.y) * (b.x - a.x) > (b.y - a.y) * (c.x - a.x));
		}

		/**
		 * Determine if two segments defined by endpoints are intersecting (defined by points).
		 * 
		 * True when the two segments are intersecting. Not true when endpoints
		 * are equal, nor when a point is contained by other segment.
		 * 
		 * From http://bryceboe.com/2006/10/23/line-segment-intersection-algorithm/ 
		 */
		public static bool AreSegmentsIntersecting(Vector2 a1, Vector2 b1, Vector2 a2, Vector2 b2)
		{
			return (
				(ArePointsCCW(a1, a2, b2) != ArePointsCCW(b1, a2, b2)) &&
				(ArePointsCCW(a1, b1, a2) != ArePointsCCW(a1, b1, b2))
				);
		}

		// 
		
		
		/**
		 * Returns intersection point of two lines (defined by segment endpoints).
		 * 
		 * Returns zero, when segments have common points,
		 * or when a segment point lies on other.
		 */
		public static Vector2 IntersectionOfSegments(Vector2 a1, Vector2 b1, Vector2 a2, Vector2 b2)
		{
			float d = (a1.x - b1.x) * (a2.y - b2.y) - (a1.y - b1.y) * (a2.x - b2.x);
			// if (d == 0.0f) return Vector2.zero;

			float x = ((a2.x - b2.x) * (a1.x * b1.y - a1.y * b1.x) - (a1.x - b1.x) * (a2.x * b2.y - a2.y * b2.x)) / d;
			float y = ((a2.y - b2.y) * (a1.x * b1.y - a1.y * b1.x) - (a1.y - b1.y) * (a2.x * b2.y - a2.y * b2.x)) / d;

			// if (x < Mathf.Min(a1.x, b1.x) || x > Mathf.Max(a1.x, b1.x)) return Vector2.zero;
			// if (x < Mathf.Min(a2.x, b2.x) || x > Mathf.Max(a2.x, b2.x)) return Vector2.zero;

			return new Vector2(x, y);
		}

		// Determine point distance from line (not segment) defined by endpoints.
		public static float PointDistanceFromLine(Vector2 point, Vector2 a, Vector2 b)
		{
			float a_ = point.x - a.x;
			float b_ = point.y - a.y;
			float c_ = b.x - a.x;
			float d_ = b.y - a.y;
			return Mathf.Abs(a_ * d_ - c_ * b_) / Mathf.Sqrt(c_ * c_ + d_ * d_);
		}

		public static bool PointIsLeftOfSegment(Vector2 point, Vector2 a, Vector2 b)
		{
			float crossProduct = (b.x - a.x) * (point.y - a.y) - (b.y - a.y) * (point.x - a.x);
			return (crossProduct > 0.0f);
		}

		// Test if a polygon contains the given point.
		// Uses the same Bryce boe algorythm, so considerations are the same.
		// From https://en.wikipedia.org/wiki/Point_in_polygon#Ray_casting_algorithm
		public static bool IsPolygonContainsPoint(Polygon polygon, Vector2 point)
		{
			// Winding ray left point.
			Vector2 left = new Vector2(polygon.bounds.xMin - polygon.bounds.width, point.y);

			// Enumerate polygon segments.
			int windingNumber = 0;
			for (int pointIndex = 0; pointIndex < polygon.pointCount; pointIndex++)
			{
				// Segment points.
				Vector2 eachPoint = polygon.PointForIndex(pointIndex);
				Vector2 eachNextPoint = polygon.NextPointForIndex(pointIndex);

				// Test winding ray against each polygon segment.
				if (AreSegmentsIntersecting(left, point, eachPoint, eachNextPoint)) 
				{ windingNumber++; }
			}

			bool isOdd = (windingNumber % 2 != 0); // Odd winding number means point falls outside
			return isOdd;
		}


		// --


		// Determine if points are equal with a given accuracy.
		public static bool ArePointsEqual(Vector2 a, Vector2 b, float accuracy)
		{
			return Vector2.Distance(a, b) <= accuracy;
		}
		
		// Determine if segments defined by endpoints are equal with a given accuracy.
		public static bool AreSegmentsEqual(Vector2 a1, Vector2 b1, Vector2 a2, Vector2 b2, float accuracy)
		{
			return (
				(ArePointsEqual(a1, a2, accuracy) && ArePointsEqual(b1, b2, accuracy)) ||
				(ArePointsEqual(a1, b2, accuracy) && ArePointsEqual(b1, a2, accuracy))
				);
		}
		
		// Determine if segments defined by endpoints have common points with a given accuracy.
		public static bool HaveSegmentsCommonPoints(Vector2 a1, Vector2 b1, Vector2 a2, Vector2 b2, float accuracy)
		{
			return (
				ArePointsEqual(a1, a2, accuracy) ||
				ArePointsEqual(a1, b2, accuracy) ||
				ArePointsEqual(b1, a2, accuracy) ||
				ArePointsEqual(b1, b2, accuracy)
				);
		}
	}
}
