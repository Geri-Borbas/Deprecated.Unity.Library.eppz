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

		private Edge _edge;
		public Edge edge { get { return _edge; } } // Readonly

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

		internal void AssignToEdge(Edge edge)
		{
			_edge = edge; 
		}


		/*
		 * 
		 * Accessors
		 * 
		 */

		public virtual Edge previousEdge // Readonly
		{
			get
			{
				int _edgeIndex = (index > 0) ? index - 1 : polygon.edgeCount - 1;
				return polygon.edges[_edgeIndex];
			}
		}

		public virtual Edge nextEdge // Readonly
		{
			get
			{
				int _edgeIndex = (index < polygon.edgeCount) ? index : 0;
				return polygon.edges[_edgeIndex];
			}
		}

		public virtual Vector2 point
		{
			get { return polygon.points[index]; }
		}

		public float x // Operate on `Polygon.Vector2` directly
		{
			get { return point.x; }
			// set { polygon.points[index].x = value; }
		}

		public float y // Operate on `Polygon.Vector2` directly
		{
			get { return point.y; }
			// set { polygon.points[index].y = value; }
		}
	}
}
