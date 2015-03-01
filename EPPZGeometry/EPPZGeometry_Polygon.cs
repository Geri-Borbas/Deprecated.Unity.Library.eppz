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
		[HideInInspector] public enum WindingDirection { Unknown, CCW, CW };
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
		{
			Polygon rootPolygon = Polygon.PolygonWithPointTransforms (polygonSource.pointTransforms, polygonSource.windingDirection);

			// Collect sub-olygons if any.
			foreach (Transform eachChildTransform in polygonSource.gameObject.transform)
			{
				GameObject eachChildGameObject = eachChildTransform.gameObject;
				EPPZGeometry_PolygonSource eachChildPolygonSource = eachChildGameObject.GetComponent<EPPZGeometry_PolygonSource>();
				if (eachChildPolygonSource != null)
				{
					Polygon eachSubPolygon = Polygon.PolygonWithSource(eachChildPolygonSource);
					eachChildPolygonSource.polygon = eachSubPolygon; // Inject into source
					rootPolygon.AddPolygon(eachSubPolygon);
				}
			}

			return rootPolygon;
		}

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
			Polygon polygon = new Polygon(points.Length);
			
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

		public Polygon(int pointCount = 1)
		{
			this._points = new Vector2[pointCount];
			this.vertices = new Vertex[pointCount];
			this.edges = new Edge[pointCount];
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
			_windingDirection = WindingDirection.Unknown;
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
			float subPolygonAreas = 0.0f;
			foreach (Polygon eachPolygon in polygons)
			{
				// Outer or inner polygon (supposing there is no self-intersection).
				float subPolygonArea = eachPolygon.CalculatedArea();
				subPolygonAreas += (eachPolygon.windingDirection == windingDirection) ? subPolygonArea : -subPolygonArea;
			}

			_area = _area + subPolygonAreas;
		}
		
		private float CalculatedArea()
		{
			CalculateArea();
			return _area;
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
			EnumerateEdgesRecursive ((Edge eachEdge) =>
			{
				contains |= eachEdge.ContainsPoint(point, accuracy, containmentMethod);
			});

			return contains;
		}

		public bool IsIntersectingWithSegment(Segment segment)
		{
			bool isIntersecting = false;
			EnumerateEdgesRecursive ((Edge eachEdge) =>
			{
				isIntersecting |= segment.IsIntersectingWithSegment(eachEdge);
			});

			return isIntersecting;
		}
		
		
		/*
		 * 
		 * Geometry features (Offset)
		 * 
		 */ 

		public Polygon rawOffsetPolygon;
		public Polygon offsetPolygon;

		public IEnumerator OffsetPolygon(float offset, List<IntersectionVertex> intersectionVertices)
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

			rawOffsetPolygon = Polygon.PolygonWithPoints(offsetPolygonPoints);

			Debug.Log("Offset polygon created.");

			IEnumerator cleanedUpOffsetPolygonEnumerator = CleanedUpOffsetPolygon(intersectionVertices);
			while(true)
			{
				cleanedUpOffsetPolygonEnumerator.MoveNext();
				yield return null;
			}
		}

		IEnumerator CleanedUpOffsetPolygon(List<IntersectionVertex> intersectionVertices)
		{
			// Debug.
			int whileCount = 0;
			int whileGuard = 10000;

			if (rawOffsetPolygon.edgeCount <= 3) yield break; // Only with 4 edges at least

			// The lovely cleaned up polygon.
			List<Vector2> offsetPolygonPoints = new List<Vector2>();
			List<Polygon> loopPolygons = new List<Polygon>();
			List<IntersectionVertex> pool = new List<IntersectionVertex>();
			// pool.Clear();

			
			// Reset loop stuff.
			bool loopOpened = false;
			bool loopClosed = false;
			IntersectionVertex loopOpeningIntersectionVertex = null;
			Edge loopOpeningEdge = null;

			Debug.Log ("Clean up offset polygon.");
			yield return null;
			
			// Offset polygon mode.
			Edge firstEdge = rawOffsetPolygon.edges[0]; 
			Edge eachEdge = firstEdge;
			while(true)
			{
				bool isIntersecting;
				Vector2 intersectionPoint;

				// Select next edge to test with (probably wont intersect with neighbouring edge).
				Edge intersectingEdge = eachEdge.nextEdge.nextEdge; 


				Debug.Log ("Offset mode edge ("+eachEdge.index+")");
				Debug.DrawLine(eachEdge.a, eachEdge.b, Color.red, 1.0f);
				yield return null;

				// Forward intersection test.
				while (true)
				{

					isIntersecting = eachEdge.IntersectionWithSegment(intersectingEdge, out intersectionPoint);
					if (isIntersecting)
					{
						Debug.Log ("Test ("+eachEdge.index+")-("+intersectingEdge.index+")");
						Debug.Log("Is intersecting.");

						// Open loop.
						loopOpened = true;
						
						// Create intersection vertex.
						IntersectionVertex intersectionVertex = IntersectionVertex.IntersectionVertexOfEdges(eachEdge, intersectingEdge, intersectionPoint); 
						if (loopOpeningIntersectionVertex == null) loopOpeningIntersectionVertex = intersectionVertex; // Store loop opening intersection vertex
						if (loopOpeningEdge == null)
						{
							loopOpeningEdge = intersectingEdge; // Store loop opening edge
						}

						// Check if intersection point has pooled already.
						int index = pool.IndexOf(intersectionVertex);
						bool alreadyPooled = (index != -1);

						// Close a loop.
						if (alreadyPooled)
						{
							Debug.Log(intersectionVertex.name+" already pooled.");

							loopClosed = true;
							break;
						}

						// Pool points.
						else
						{
							Debug.Log("Pool ("+pool.Count+") ("+eachEdge.index+")-("+intersectingEdge.index+")");
							pool.Add(intersectionVertex);
						}
						
						// Debug.
						// if (intersectionVertices != null) intersectionVertices.Add(eachIntersectionVertex);
					}

					// Step forward (to the next edge).
					intersectingEdge = intersectingEdge.nextEdge;

					// Every edge checked (for having loop).
					if (intersectingEdge == eachEdge || intersectingEdge == loopOpeningEdge) break;
					if (whileCount > whileGuard)
					{
						Debug.LogError("Loop polygon mode: Stack overflow.");
						break;
					}
					whileCount++;
				}

				if (loopOpened)
				{
					if (loopClosed)
					{
						// Collect loop opening-, loop endpoint if a loop has opened.
						offsetPolygonPoints.Add(loopOpeningIntersectionVertex.point);
						// offsetPolygonPoints.Add(loopOpeningEdge.b);
						eachEdge = loopOpeningEdge; // Continue offset polygon mode

						
						Debug.Log("Closie loopie.");

						// Reset loop stuff.
						loopOpened = false;
						loopClosed = false;
						loopOpeningIntersectionVertex = null;
						loopOpeningEdge = null;
					}
				}

				else
				{
					// Or simply collect endpoint if no any loop.
					offsetPolygonPoints.Add(eachEdge.b);
					eachEdge = eachEdge.nextEdge; // Step loop
				}

				// Every edge checked (for cleanup).
				if (eachEdge == firstEdge) break;
				if (whileCount > whileGuard)
				{
					Debug.LogError("Offset polygon mode: Stack overflow.");
					break;
				}
				whileCount++;
			}

			// Construct cleaned up olygon.
			offsetPolygon = Polygon.PolygonWithPointList(offsetPolygonPoints);
			foreach (Polygon eachLoopPolygon in loopPolygons)
			{
				// Validate.
				// Add if needed.
			}

			yield break;
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
