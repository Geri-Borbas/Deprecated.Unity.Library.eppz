using UnityEngine;
using System.Collections;


namespace EPPZGeometry
{


	public class Vertex
	{


		private int _index;
		public int index { get { return _index; } } // Readonly
		private Polygon _polygon;
		public Polygon polygon { get { return _polygon; } } // Readonly

		// If `alwaysCalculate` is on, every `normal` and `bisector` property access invokes recalculation of values based on actual topology.
		public bool alwaysCalculate = true;
		
		private Vector2 _normal;
		public Vector2 normal
		{
			get
			{
				if (_normal == Vector2.zero || alwaysCalculate) { CalculateNormal(); } // Lazy calculation or force calculate on every access
				return _normal;
			}
			
			set
			{ _normal = value; }
		}

		// Bisector is simply the sum of the neighbouring edge normals (not normalized).
		public Vector2 _bisector;
		public Vector2 bisector
		{
			get
			{
				if (_bisector == Vector2.zero || alwaysCalculate) { CalculateBisector(); } // Lazy calculation or force calculate on every access
				return _bisector;
			}
			
			set
			{ _bisector = value; }
		}
		
		public void CalculateNormal()
		{ _normal = this.bisector.normalized; }
		
		public void CalculateBisector()
		{ _bisector = previousEdge.normal + nextEdge.normal; }
		
		
		/*
		 * 
		 * Factory
		 * 
		 */
		
		public static Vertex VertexAtIndexInPolygon(int index, Polygon polygon)
		{
			Vertex instance = new Vertex();
			instance._index = index;
			instance._polygon = polygon;
			return instance;
		}

		/*
		 * 
		 * Accessors
		 * 
		 */

		private Vertex _previousVertex;
		public virtual Vertex previousVertex { get { return _previousVertex; } } // Readonly
		public void SetPreviousVertex(Vertex vertex) { _previousVertex = vertex; } // Explicit setter (injected at creation time)

		private Vertex _nextVertex;
		public virtual Vertex nextVertex  { get { return _nextVertex; } } // Readonly
		public void SetNextVertex(Vertex vertex) { _nextVertex = vertex; } // Explicit setter (injected at creation time)

		private Edge _previousEdge;
		public virtual Edge previousEdge { get { return _previousEdge; } } // Readonly
		public void SetPreviousEdge(Edge edge) { _previousEdge = edge; } // Explicit setter (injected at creation time)
		
		private Edge _nextEdge;
		public virtual Edge nextEdge  { get { return _nextEdge; } } // Readonly
		public void SetNextEdge(Edge edge) { _nextEdge = edge; } // Explicit setter (injected at creation time)

		public virtual Vector2 point { get { return polygon.points[index]; } } // Readonly
		public float x { get { return point.x; } } // Readonly
		public float y { get { return point.y; } } // Readonly
	}
}
