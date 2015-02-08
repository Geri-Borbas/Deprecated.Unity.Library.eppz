using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace EPPZGeometry
{
	
	
	public class IntersectionVertex : Vertex, IEquatable<IntersectionVertex>
	{


		static public float defaultAccuracy = 1e-6f;


		private Edge _edgeA;
		private Edge _edgeB;


		/*
		 * 
		 * Factory
		 * 
		 */
		
		public static IntersectionVertex IntersectionVertexOfEdges(Edge edgeA, Edge edgeB, Vector2 intersectionPoint)
		{
			IntersectionVertex instance = new IntersectionVertex();
			instance._edgeA = edgeA;
			instance._edgeB = edgeB;
			instance._point = intersectionPoint;
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
		public override Vector2 point // Has own point as not participating in a polygon
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
			alwaysCalculate = false; // Do not recalculate bisector / normal on access
			CalculateBisector();
			CalculateNormal();

		}

		public bool Equals(IntersectionVertex intersectionVertex)
		{
			bool edgesMatch = ((this._edgeA == intersectionVertex._edgeA) && (this._edgeB == intersectionVertex._edgeB)) || 
							  ((this._edgeA == intersectionVertex._edgeB) && (this._edgeB == intersectionVertex._edgeA));
			return edgesMatch;
		}
	}
}
