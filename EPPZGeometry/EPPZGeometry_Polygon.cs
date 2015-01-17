using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace EPPZGeometry
{


	public class Polygon
	{


		// Identifiers.
		public int index;
		public string name;
		
		public enum WindingDirection { Unknown, CCW, CW };
		private WindingDirection _windingDirection = WindingDirection.Unknown;
		public WindingDirection windingDirection { get { return _windingDirection; } }
		public bool isCW { get { return (_windingDirection == WindingDirection.CW); } }
		public bool isCCW { get { return (_windingDirection == WindingDirection.CCW); } }

		public Vector2[] points; // Access raw points (TODO: Make it internal later on).

		public Vertex[] vertices;
		public Edge[] edges;

		private Rect _bounds;
		public Rect bounds { get { return _bounds; } }

		
		private float _area;
		public float area { get { return _area; } }

		private float _signedArea;
		public float signedArea { get { return _signedArea; } }


		/*
		 * 
		 * Factories
		 * 
		 */ 
		
		public static Polygon PolygonWithPointList(List<Vector2> pointList)
		{
			return Polygon.PolygonWithPoints(pointList.ToArray());
		}

		public static Polygon PolygonWithSource(EPPZGeometry_PolygonSource polygonSource)
		{
			return Polygon.PolygonWithPointTransforms(polygonSource.pointTransforms);
		}

		public static Polygon PolygonWithPointTransforms(Transform[] pointTransforms) // Uses Transform.localPosition.xy()
		{
			// Create points array.
			Vector2[] points = new Vector2[pointTransforms.Length];
			for (int index = 0; index < pointTransforms.Length; index++)
			{
				Transform eachPointTransform = pointTransforms[index];
				points[index] = eachPointTransform.localPosition.xy();
			}
			
			return Polygon.PolygonWithPoints(points);
		}
		
		public static Polygon PolygonWithPoints(Vector2[] points)
		{
			Polygon polygon = new Polygon();
			
			// Allocate points.
			polygon.points = new Vector2[points.Length];
			polygon.vertices = new Vertex[points.Length];
			polygon.edges = new Edge[points.Length];
			
			// Create points (copy actually).
			for (int index = 0; index < points.Length; index++)
			{
				polygon.points[index] = points[index];
			}
			
			// Polygon calculations.
			polygon.UpdateCalculations();

			// Create members.
			polygon.CreateVertices();
			polygon.CreateEdges();
			
			return polygon;
		}

		/*
		 * 
		 * Model updates
		 * 
		 */ 

		public void UpdateWithSource(EPPZGeometry_PolygonSource polygonSource) // Assuming unchanged point count
		{
			UpdateWithTransforms(polygonSource.pointTransforms);
		}

		public void UpdateWithTransforms(Transform[] pointTransforms) // Assuming unchanged point count
		{
			for (int index = 0; index < pointTransforms.Length; index++)
			{
				Transform eachPointTransform = pointTransforms[index];
				points[index] = eachPointTransform.localPosition.xy();
			}

			// Polygon calculations.
			UpdateCalculations();
		}
		
		/*
		 * 
		 * Accessors
		 * 
		 */ 
		
		public int pointCount { get { return points.Length; } } // Readonly
		public int vertexCount { get { return vertices.Length; } } // Readonly
		public int edgeCount { get { return edges.Length; } } // Readonly
		
		public Vector2 PointForIndex(int index)
		{
			return points[index];
		}
		
		public Vector2 NextPointForIndex(int index)
		{
			int nextIndex = (index < pointCount - 1) ? index + 1 : 0;
			return points[nextIndex];
		}
		
		public Vector2 PreviousPointForIndex(int index)
		{
			int previousIndex = (index > 0) ? index - 1 : pointCount - 1;
			return points[previousIndex];
		}
		
		public Vertex VertexForIndex(int index)
		{
			return vertices[index];
		}
		
		public Vertex NextVertexForIndex(int index)
		{
			int nextIndex = (index < vertexCount - 1) ? index + 1 : 0;
			return vertices[nextIndex];
		}
		
		public Vertex PreviousVertexForIndex(int index)
		{
			int previousIndex = (index > 0) ? index - 1 : vertexCount - 1;
			return vertices[previousIndex];
		}
		
		
		/*
		 * 
		 * Polygon calculations
		 * 
		 */

		// To be called manually upon polygon point data has changed.
		public void UpdateCalculations()
		{
			CalculateBounds();
			CalculateArea();
		}

		private void CalculateBounds()
		{
			float left = float.MaxValue; // Out in the right
			float right = float.MinValue; // Out in the left
			float top = float.MinValue; // Out in the bottom
			float bottom = float.MaxValue; // Out in the top
			
			// Enumerate points.
			for (int index = 0; index < points.Length; index++)
			{
				Vector2 eachPoint = points[index];
				
				// Track bounds.
				if (eachPoint.x < left) left = eachPoint.x; // Seek leftmost
				if (eachPoint.x > right) right = eachPoint.x; // Seek rightmost
				if (eachPoint.y < bottom) bottom = eachPoint.y; // Seek bottommost
				if (eachPoint.y > top) top = eachPoint.y; // Seek topmost
			}
			
			// Set bounds.
			_bounds.xMin = left;
			_bounds.yMin = bottom;
			_bounds.xMax = right;
			_bounds.yMax = top;
		}
		
		private void CalculateArea()
		{
			// From https://en.wikipedia.org/wiki/Shoelace_formula
			Vector2[] points_ = new Vector2[points.Length + 1];
			
			// Create point list for calculations.
			if (windingDirection == WindingDirection.CW)
			{
				Vector2[] reversed = new Vector2[points.Length];
				System.Array.Copy(points, reversed, points.Length);
				System.Array.Copy(reversed, points_, points.Length);
			}
			
			if (windingDirection == WindingDirection.CCW)
			{ System.Array.Copy(points, points_, points.Length); }
			
			points_[points.Length] = points[0];
			
			// Calculations.
			float firstProducts = 0.0f;
			float secondProducts = 0.0f;
			for (int index = 0; index < points_.Length - 1; index++)
			{
				Vector2 eachPoint = points_[index];
				Vector2 eachNextPoint = points_[index + 1];
				
				firstProducts += eachPoint.x * eachNextPoint.y;
				secondProducts += eachPoint.y * eachNextPoint.x;
			}
			float area_ = (firstProducts - secondProducts) / 2.0f;
			
			// Set.
			_signedArea = area_;
			_windingDirection = (Mathf.Sign(_signedArea) > 0.0f) ? WindingDirection.CCW : WindingDirection.CW;
			_area = Mathf.Abs(area_);
		}

		
		private void CreateVertices()
		{
			// Enumerate points.
			for (int index = 0; index < points.Length; index++)
			{
				Vertex eachVertex = Vertex.VertexAtIndexInPolygon(index, this);
				
				// Collect.
				vertices[index] = eachVertex;
			}
		}
		
		private void CreateEdges()
		{
			for (int index = 0; index < edgeCount; index++)
			{
				Vertex eachVertex = VertexForIndex(index);
				Vertex eachNextVertex = NextVertexForIndex(index);
				Edge eachEdge = Edge.EdgeAtIndexWithVertices(index, eachVertex, eachNextVertex);

				// Collect.
				edges[index] = eachEdge;

				// Vertex references.
				eachVertex.AssignToEdge(eachEdge);
				eachNextVertex.AssignToEdge(eachEdge);
			}
		}


		/*
		 * 
		 * Geometry features
		 * 
		 */ 

		public bool ContainsPoint(Vector2 point)
		{
			return Geometry.IsPolygonContainsPoint(this, point);
		}

		public bool PermiterContainsPoint(Vector2 point)
		{ return PermiterContainsPoint(point, Segment.defaultAccuracy); }

		public bool PermiterContainsPoint(Vector2 point, float accuracy)
		{ return PermiterContainsPoint(point, accuracy, Segment.ContainmentMethod.Default); }

		public bool PermiterContainsPoint(Vector2 point, float accuracy, Segment.ContainmentMethod containmentMethod)
		{
			bool contains = false;
			foreach (Edge eachEdge in edges)
			{
				contains |= eachEdge.ContainsPoint(point, accuracy, containmentMethod);
			}
			return contains;
		}

		public bool IsIntersectingWithSegment(Segment segment)
		{
			bool isIntersecting = false;
			foreach (Edge eachEdge in edges)
			{
				isIntersecting |= segment.IsIntersectingWithSegment(eachEdge);
			}
			return isIntersecting;
		}
		
		
		/*
		 * 
		 * Geometry features (Offset)
		 * 
		 */ 

		public Polygon OffsetPolygon(float offset)
		{
			// Allocate.
			int offsetPolygonPointCount = pointCount * (3);
			Vector2[] offsetPolygonPoints = new Vector2[offsetPolygonPointCount];
			
			for (int index = 0; index < vertexCount; index++)
			{
				Vertex eachVertex = VertexForIndex(index);
				offsetPolygonPoints[index * 3] = eachVertex.point + eachVertex.previousEdge.normal * offset;
				offsetPolygonPoints[index * 3 + 1] = eachVertex.point + eachVertex.normal * offset;
				offsetPolygonPoints[index * 3 + 2] = eachVertex.point + eachVertex.nextEdge.normal * offset;
			}

			Polygon rawOffsetPolygon = Polygon.PolygonWithPoints(offsetPolygonPoints);
			return CleanedUpOffsetPolygon(rawOffsetPolygon);
		}

		public Polygon CleanedUpOffsetPolygon(Polygon rawOffsetPolygon)
		{
			// Loop polygon mode.
			foreach (Edge eachEdge in rawOffsetPolygon.edges)
			{
				// Forward intersection text.
				Edge eachNextEdge = eachEdge.nextEdge;
				bool isIntersecting = eachEdge.IsIntersectingWithSegment(eachNextEdge);
				if (isIntersecting)
				{
					// Get intersection vertex.
					IntersectionVertex eachIntersectionVertex = IntersectionVertex.IntersectionVertexOfEdges(eachEdge, eachNextEdge);
				}
			}


			return rawOffsetPolygon;
		}


		// --

		
		public void AlignCentered()
		{
			Vector2 originalCenter = bounds.center;
			Vector2 offset = -originalCenter;
			
			// Apply to each point.
			for (int index = 0; index < points.Length; index++)
			{
				points[index] += offset;
			}
			
			// Apply to bounds.
			_bounds.position += offset;
		}
	}


}
