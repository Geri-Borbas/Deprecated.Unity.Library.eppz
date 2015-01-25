using UnityEngine;
using System.Collections;


namespace EPPZGeometry
{


	public class Edge : Segment
	{


		private int _index;
		public int index { get { return _index; } } // Readonly
		public Polygon polygon { get { return vertexA.polygon; } } // Readonly

		public Vertex vertexA;
		public Vertex vertexB;

		// If `alwaysCalculate` is on, every `normal` and `perpendicular` property access invokes recalculation of values based on actual topology.
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

		public Vector2 _perpendicular;
		public Vector2 perpendicular
		{
			get
			{
				if (_perpendicular == Vector2.zero || alwaysCalculate) { CalculatePerpendicular(); } // Lazy calculation or force calculate on every access
				return _perpendicular;
			}
			
			set
			{ _perpendicular = value; }
		}

		public void CalculateNormal()
		{
			_normal = this.perpendicular.normalized;
		}
		
		public void CalculatePerpendicular()
		{
			Vector2 translated = (this.b - this.a); // Translate to origin
			_perpendicular = new Vector2( -translated.y, translated.x); // Rotate CCW
		}


		/*
		 * 
		 * Factory
		 * 
		 */

		public static Edge EdgeAtIndexWithVertices(int index, Vertex vertexA, Vertex vertexB)
		{
			Edge instance = new Edge();
			instance._index = index;
			instance.vertexA = vertexA;
			instance.vertexB = vertexB;
			return instance;
		}


		/*
		 * 
		 * Override segment point accessors (perefencing polygon points directly).
		 * 
		 */ 

		public override Vector2 a
		{
			get { return polygon.points[vertexA.index]; }
			set { polygon.points[vertexA.index] = value; }
		}
		
		public override Vector2 b
		{
			get { return polygon.points[vertexB.index]; }
			set { polygon.points[vertexB.index] = value; }
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
				int _edgeIndex = (index < polygon.edgeCount - 1) ? index + 1 : 0;
				return polygon.edges[_edgeIndex];
			}
		}

	}
}