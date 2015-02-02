using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace EPPZGeometry
{


	public class Polygon
	{


		// Identifiers.
		public int index;
		public string name;

		// Windings.
		public enum WindingDirection { Unknown, CCW, CW };
		private WindingDirection _windingDirection = WindingDirection.Unknown;
		public WindingDirection windingDirection { get { return _windingDirection; } }
		public bool isCW { get { return (_windingDirection == WindingDirection.CW); } }
		public bool isCCW { get { return (_windingDirection == WindingDirection.CCW); } }

		// For internal use.
		// Edge, Vertex class can read from this directly
		// Debug renderers can access raw points during development
		// From the outside, use vertices, edges only, or enumerators
		private Vector2[] _points;
		public Vector2[] points { get { return _points; } } // Readonly

		public Vertex[] vertices; // Vertices of this polygon (excluding sub polygon vertices)
		public Edge[] edges; // Edges of this polygon (excluding sub polygon edges)
		private List<Polygon> polygons = new List<Polygon>(); // Sub-polygons (if any)

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
		{ return Polygon.PolygonWithPoints(pointList.ToArray()); }

		public static Polygon PolygonWithSource(EPPZGeometry_PolygonSource polygonSource)
		{ return Polygon.PolygonWithPointTransforms(polygonSource.pointTransforms, polygonSource.windingDirection); }

		public static Polygon PolygonWithPointTransforms(Transform[] pointTransforms)
		{ return PolygonWithPointTransforms(pointTransforms, WindingDirection.Unknown); }

		public static Polygon PolygonWithPointTransforms(Transform[] pointTransforms, WindingDirection windingDirection) // Uses Transform.localPosition.xy()
		{
			// Create points array.
			Vector2[] points = new Vector2[pointTransforms.Length];
			for (int index = 0; index < pointTransforms.Length; index++)
			{
				Transform eachPointTransform = pointTransforms[index];
				points[index] = eachPointTransform.localPosition.xy();
			}
			
			return Polygon.PolygonWithPoints(points, windingDirection);
		}

		public static Polygon PolygonWithPoints(Vector2[] points)
		{ return PolygonWithPoints(points, WindingDirection.Unknown); }

		public static Polygon PolygonWithPoints(Vector2[] points, WindingDirection windingDirection)
		{
			Polygon polygon = new Polygon();
			
			// Allocate points.
			polygon._points = new Vector2[points.Length];
			polygon.vertices = new Vertex[points.Length];
			polygon.edges = new Edge[points.Length];
			
			// Create points (copy actually).
			for (int index = 0; index < points.Length; index++)
			{
				polygon._points[index] = points[index];
			}
			
			// Polygon calculations.
			polygon.CalculateBounds();
			polygon.CalculateArea();
			polygon.CalculateWindingDirectionIfNeeded();

			// Create members.
			polygon.CreateVerticesFromPoints();
			polygon.CreateEdgesConnectingPoints();

			return polygon;
		}

		/*
		 * 
		 * Model updates
		 * 
		 */ 

		public void UpdatePointPositionsWithSource(EPPZGeometry_PolygonSource polygonSource) // Assuming unchanged point count
		{
			UpdatePointPositionsWithTransforms(polygonSource.pointTransforms);
		}

		public void UpdatePointPositionsWithTransforms(Transform[] pointTransforms) // Assuming unchanged point count
		{
			for (int index = 0; index < pointTransforms.Length; index++)
			{
				Transform eachPointTransform = pointTransforms[index];
				_points[index] = eachPointTransform.localPosition.xy();
			}

			// Polygon calculations.
			CalculateBounds();
			CalculateArea();
			CalculateWindingDirectionIfNeeded();
		}

		public void AddPolygon(Polygon polygon)
		{
			polygons.Add(polygon);

			// Polygon calculations.
			CalculateBounds();
		}
		
		/*
		 * 
		 * Accessors
		 * 
		 */ 
		
		private int pointCount { get { return _points.Length; } } // Readonly
		private int vertexCount { get { return vertices.Length; } } // Readonly
		private int edgeCount { get { return edges.Length; } } // Readonly

		public int pointCountRecursive
		{
			get
			{
				int pointCountRecursive = pointCount;
				foreach (Polygon eachPolygon in polygons)
				{
					pointCountRecursive += eachPolygon.pointCount;
				}
				return pointCountRecursive;
			}
		}

		/*
		 * 
		 * Enumerators
		 * 
		 */
		
		public void EnumeratePoints(Action<Vector2> action)
		{
			// Enumerate local points.
			foreach (Vector2 eachPoint in _points)
			{
				action(eachPoint);
			}
		}
		
		public void EnumerateVertices(Action<Vertex> action)
		{
			// Enumerate local points.
			foreach (Vertex eachVertex in vertices)
			{
				action(eachVertex);
			}
		}
		
		public void EnumerateEdges(Action<Edge> action)
		{
			// Enumerate local points.
			foreach (Edge eachEdge in edges)
			{
				action(eachEdge);
			}
		}

		public void EnumeratePointsRecursive(Action<Vector2> action)
		{
			// Enumerate local points.
			foreach (Vector2 eachPoint in _points)
			{
				action(eachPoint);
			}
			
			// Enumerate each sub-polygon points.
			foreach (Polygon eachPolygon in polygons)
			{
				eachPolygon.EnumeratePointsRecursive((Vector2 eachPoint_) =>
				{
					action(eachPoint_);
				});
			}
		}
		
		public void EnumerateVerticesRecursive(Action<Vertex> action)
		{
			// Enumerate local points.
			foreach (Vertex eachVertex in vertices)
			{
				action(eachVertex);
			}
			
			// Enumerate each sub-polygon points.
			foreach (Polygon eachPolygon in polygons)
			{
				eachPolygon.EnumerateVerticesRecursive((Vertex eachVertex_) =>
				{
					action(eachVertex_);
				});
			}
		}
		
		public void EnumerateEdgesRecursive(Action<Edge> action)
		{
			// Enumerate local points.
			foreach (Edge eachEdge in edges)
			{
				action(eachEdge);
			}
			
			// Enumerate each sub-polygon points.
			foreach (Polygon eachPolygon in polygons)
			{
				eachPolygon.EnumerateEdgesRecursive((Edge eachEdge_) =>
				{
					action(eachEdge_);
				});
			}
		}

		/*
		 * 
		 * Polygon calculations
		 * 
		 */

		private void CalculateBounds()
		{
			float left = float.MaxValue; // Out in the right
			float right = float.MinValue; // Out in the left
			float top = float.MinValue; // Out in the bottom
			float bottom = float.MaxValue; // Out in the top
			
			// Enumerate points.
			EnumeratePointsRecursive((Vector2 eachPoint) =>
			{				
				// Track bounds.
				if (eachPoint.x < left) left = eachPoint.x; // Seek leftmost
				if (eachPoint.x > right) right = eachPoint.x; // Seek rightmost
				if (eachPoint.y < bottom) bottom = eachPoint.y; // Seek bottommost
				if (eachPoint.y > top) top = eachPoint.y; // Seek topmost
			});
			
			// Set bounds.
			_bounds.xMin = left;
			_bounds.yMin = bottom;
			_bounds.xMax = right;
			_bounds.yMax = top;
		}
		
		private void CalculateArea()
		{
			// From https://en.wikipedia.org/wiki/Shoelace_formula
			Vector2[] points_ = new Vector2[_points.Length + 1];
			
			// Create point list for calculations.
			if (windingDirection == WindingDirection.CW)
			{
				Vector2[] reversed = new Vector2[_points.Length];
				System.Array.Copy(_points, reversed, _points.Length);
				System.Array.Copy(reversed, points_, _points.Length);
			}
			
			if (windingDirection == WindingDirection.CCW)
			{ System.Array.Copy(_points, points_, _points.Length); }
			
			points_[_points.Length] = _points[0];
			
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
			
			// Set signed area.
			_signedArea = area_;

			// Set area.
			_area = Mathf.Abs(area_);

			// Add / Subtract sub-polygon areas.
			foreach (Polygon eachPolygon in polygons)
			{
				_area += (eachPolygon.windingDirection == windingDirection) ? eachPolygon.area : -eachPolygon.area;
			}
		}

		private void CalculateWindingDirectionIfNeeded()
		{
			if (_windingDirection == WindingDirection.Unknown) // Only if unknown
			{
				_windingDirection = (Mathf.Sign (_signedArea) > 0.0f) ? WindingDirection.CCW : WindingDirection.CW;
			}
		}

		
		private void CreateVerticesFromPoints()
		{
			Debug.Log("CreateVerticesFromPoints()");

			// Enumerate points (only for index).
			Vertex eachVertex = null;
			Vertex eachPreviousVertex = null;
			for (int index = 0; index < _points.Length; index++)
			{
				eachVertex = Vertex.VertexAtIndexInPolygon(index, this);

				// Inject references.
				if (eachPreviousVertex != null)
				{
					eachPreviousVertex.SetNextVertex(eachVertex);
					eachVertex.SetPreviousVertex(eachPreviousVertex);
				}

				// Collect.
				vertices[index] = eachVertex;

				// Track.
				eachPreviousVertex = eachVertex;
			}

			// Inject last references.
			Vertex firstVertex = vertices[0];
			eachVertex.SetNextVertex(firstVertex);
			firstVertex.SetPreviousVertex(eachVertex);
		}
		
		private void CreateEdgesConnectingPoints()
		{
			// Enumerate vertices.
			Edge eachEdge = null;
			Edge eachPreviousEdge = null;
			EnumerateVertices((Vertex eachVertex) =>
			{
				int index = eachVertex.index;
				eachEdge = Edge.EdgeAtIndexWithVertices(index, eachVertex, eachVertex.nextVertex);

				// Inject references.
				if (eachPreviousEdge != null)
				{
					eachPreviousEdge.SetNextEdge(eachEdge);
					eachEdge.SetPreviousEdge(eachPreviousEdge);
				}

				// Collect.
				edges[index] = eachEdge;
				
				// Track.
				eachPreviousEdge = eachEdge;
			});

			// Inject last references.
			Edge firstEdge = edges[0];
			eachEdge.SetNextEdge(firstEdge);
			firstEdge.SetPreviousEdge(eachEdge);

			// Inject vertex edge references.
			EnumerateEdges((Edge eachEdge_) =>
			{
				eachEdge_.vertexA.SetPreviousEdge(eachEdge_.previousEdge);
				eachEdge_.vertexA.SetNextEdge(eachEdge_);
			});
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

		public Polygon OffsetPolygon(float offset, List<IntersectionVertex> intersectionVertices)
		{
			// Allocate.
			int offsetPolygonPointCount = pointCount * (3);
			Vector2[] offsetPolygonPoints = new Vector2[offsetPolygonPointCount];

			// Extrude each vertex in 3 ways.
			EnumerateVertices((Vertex eachVertex) =>
			{
				offsetPolygonPoints[eachVertex.index * 3] = eachVertex.point + eachVertex.previousEdge.normal * offset;
				offsetPolygonPoints[eachVertex.index * 3 + 1] = eachVertex.point + eachVertex.normal * offset;
				offsetPolygonPoints[eachVertex.index * 3 + 2] = eachVertex.point + eachVertex.nextEdge.normal * offset;
			});

			Polygon rawOffsetPolygon = Polygon.PolygonWithPoints(offsetPolygonPoints);
			return CleanedUpOffsetPolygon(rawOffsetPolygon, intersectionVertices);
		}

		public Polygon CleanedUpOffsetPolygon(Polygon rawOffsetPolygon, List<IntersectionVertex> intersectionVertices)
		{
			if (rawOffsetPolygon.edgeCount <= 3) return rawOffsetPolygon; // Only with 4 edges at least

			List<Vector2> offsetPolygonPoints = new List<Vector2>();

			// Select starting edge.
			Edge firstEdge = rawOffsetPolygon.edges[0];

			// -------------------
			// Offset polygon mode
			// -------------------

			bool loopMode = false;

			foreach (Edge eachEdge in rawOffsetPolygon.edges)
			{
				bool isIntersecting;
				Vector2 intersectionPoint;
				List<IntersectionVertex> pool = new List<IntersectionVertex>();

				// Select next edge to test with.
				Edge intersectingEdge = eachEdge.nextEdge.nextEdge; 
				
				// Forward intersection test.
				while (true)
				{
					isIntersecting = eachEdge.IntersectionWithSegment(intersectingEdge, out intersectionPoint);
					if (isIntersecting)
					{
						// ---------
						// Loop mode
						// ---------

						// Create intersection vertex.
						IntersectionVertex intersectionVertex = IntersectionVertex.IntersectionVertexOfEdges(eachEdge, intersectingEdge, intersectionPoint);


						// End.
						int index = pool.IndexOf(intersectionVertex);
						bool alreadyPooled = (index != -1);

						// Close a loop.
						if (alreadyPooled)
						{
							// Splice.
							int count = pool.Count - index;
							List<IntersectionVertex> loopPoints = pool.GetRange(index, count);
							pool.RemoveRange(index, count);

							// Add sub-polygon.
							if (count > 1)
							{
								// HEY, BUT HOW?
							}

							// Or add offset polygon point.
							else
							{
								offsetPolygonPoints.Add(intersectionVertex.point);
								offsetPolygonPoints.Add(intersectionVertex.point);
							}
						}

						// Pool points.
						else
						{
							pool.Add (intersectionVertex);
						}
						
						// Debug.
						// if (intersectionVertices != null) intersectionVertices.Add(eachIntersectionVertex);
					}

					// Step forward (to the next edge).
					intersectingEdge = intersectingEdge.nextEdge;

					// Every edge checked.
					if (intersectingEdge == eachEdge.previousEdge) break;
				}

				{
					// Collect endpoint.
					offsetPolygonPoints.Add(eachEdge.b);
					
				}
			}

			return rawOffsetPolygon;
		}


		// --

		
		public void ConvertCorneredToCentered()
		{
			Vector2 originalCenter = bounds.center;
			Vector2 offset = -originalCenter;

			TranslatePoints(offset);
		}

		public void TranslatePoints(Vector2 translation)
		{
			// Apply to each point.
			for (int index = 0; index < _points.Length; index++)
			{
				_points[index] += translation;
			}

			// Apply to each sub-polygon.
			foreach (Polygon eachPolygon in polygons)
			{
				eachPolygon.TranslatePoints(translation);
			}
			
			// Apply to bounds.
			_bounds.position += translation;
		}
	}


}
