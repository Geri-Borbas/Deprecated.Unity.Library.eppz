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
		
		private Edge _previousEdge;
		public virtual Edge previousEdge { get { return _previousEdge; } } // Readonly
		public void SetPreviousEdge(Edge edge) { _previousEdge = edge; } // Explicit setter (injected at creation time)
		
		private Edge _nextEdge;
		public virtual Edge nextEdge  { get { return _nextEdge; } } // Readonly
		public void SetNextEdge(Edge edge) { _nextEdge = edge; } // Explicit setter (injected at creation time)

	}
}