#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;


namespace EPPZ.Geometry.Editor
{


	[CustomEditor(typeof(EPPZ.Geometry.PolygonInspector))]
	public class PolygonInspector : UnityEditor.Editor 
	{


		public override void OnInspectorGUI()
		{
			// References.
			EPPZ.Geometry.PolygonInspector polygonInspector = (EPPZ.Geometry.PolygonInspector)target;
			Polygon polygon = polygonInspector.polygon;
			if (polygon == null) return;
			Edge edge = polygon.edges[polygonInspector.currentEdgeIndex];
			if (edge == null) return;

			if (GUILayout.Button("-"))
			{
				polygonInspector.currentEdgeIndex = edge.previousEdge.index;
				edge = polygon.edges[polygonInspector.currentEdgeIndex];

				ShowUpEdges(edge);
			}

			if (GUILayout.Button("Show up ("+polygonInspector.currentEdgeIndex.ToString()+")"))
			{
				ShowUpEdges(edge);
			}

			if (GUILayout.Button("+"))
			{
				polygonInspector.currentEdgeIndex = edge.nextEdge.index;
				edge = polygon.edges[polygonInspector.currentEdgeIndex];

				ShowUpEdges(edge);
			}
		}

		private void ShowUpEdges(Edge edge)
		{
			Debug.DrawLine(edge.previousEdge.a, edge.previousEdge.b, Color.blue, 1.0f);
			Debug.DrawLine(edge.a, edge.b, Color.red, 1.0f);
			Debug.DrawLine(edge.nextEdge.a, edge.nextEdge.b, Color.green, 1.0f);
		}
	}
}
#endif