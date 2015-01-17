using UnityEngine;
using System.Collections;


namespace EPPZGeometry
{
	
	
	public class IntersectionVertex : Vertex
	{


		static public float defaultAccuracy = 1e-6f;


		private Edge _edgeA;
		private Edge _edgeB;


		/*
		 * 
		 * Factory
		 * 
		 */
		
		public static IntersectionVertex IntersectionVertexOfEdges(Edge edgeA, Edge edgeB)
		{
			IntersectionVertex instance = new IntersectionVertex();
			instance._edgeA = edgeA;
			instance._edgeB = edgeB;
			instance.CalculateIntersection();
			return instance;
		}


		/*
		 * 
		 * Accessors
		 * 
		 */
		
		public override Edge previousEdge // Readonly
		{
			get
			{
				return _edgeA;
			}
		}
		
		public override Edge nextEdge // Readonly
		{
			get
			{
				return _edgeB;
			}
		}

		private Vector2 _point;
		public virtual Vector2 point // Has own point until not participating in a polygon
		{
			get { return _point; }
		}


		/*
		 * 
		 * Features
		 * 
		 */ 

		void CalculateIntersection()
		{
			// Containment (endpoint or segment itself).
			bool containsA = nextEdge.ContainsPoint(edge.a);
			if (containsA)
			{ _point = edge.a; }

			bool containsB = nextEdge.ContainsPoint(edge.b);
			if (containsB)
			{ _point = edge.b; }

			// Arbitrary intersection point.
			bool containsEndpoint = (containsA || containsB);
			if (containsEndpoint == false)
			{

			}

			alwaysCalculate = false; // Do not recalculate bisector / normal on access
			CalculateBisector();
			CalculateNormal();

		}
/*
		public Vector2 IntersectionOfSegments(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2)
		{

				double BBXMinusBAX = B.B.X - B.A.X;
				double AAXMinusABX = A.A.X - A.B.X;
				double Denominator = (A.B.Y - A.A.Y) * BBXMinusBAX - AAXMinusABX * (B.A.Y - B.B.Y);
				if (Denominator == 0)
					return null;
				double X = ((A.A.X * A.B.Y - A.A.Y * A.B.X) * BBXMinusBAX + (B.A.X * B.B.Y - B.A.Y * B.B.X) * AAXMinusABX) /
					Denominator;
				double Y;
				if (A.A.X == A.B.X)
				{
					Y = (X * B.B.Y - X * B.A.Y - B.A.X * B.B.Y + B.A.Y * B.B.X) / (B.B.X - B.A.X);
				}
				else
				{
					Y = (X * A.B.Y - X * A.A.Y - A.A.X * A.B.Y + A.A.Y * A.B.X) / (A.B.X - A.A.X);
				}

				if ((X < A.A.X && X < A.B.X) ||
				    (X < B.A.X && X < B.B.X) ||
				    (Y < A.A.Y && Y < A.B.Y) ||
				    (Y < B.A.Y && Y < B.B.Y))
					return null;

				// return new Vector2(x, y);

			float x;
			float y;
			float C1 = A1.x + B1.y;
			float C2 = A2.x + B2.y;

			float det = A1 * B2 - A2 * B1;
			if (det == 0)
			{
				//Lines are parallel
			}
			else
			{
				x = (B2 * C1 - B1 * C2) / det;
				y = (A1 * C2 - A2 * C1) / det;
			}
		
			return new Vector2(x, y);
		}
*/

		bool isEqual(IntersectionVertex intersectionVertex)
		{
			float distance = Vector3.Distance(this.point, intersectionVertex.point);
			bool positionMatch = (distance < defaultAccuracy);
			bool intersectingEdgeMatch = (this.nextEdge == intersectionVertex.nextEdge);
			return (positionMatch && intersectingEdgeMatch);
		}
	}
}
