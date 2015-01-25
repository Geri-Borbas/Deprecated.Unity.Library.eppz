using UnityEngine;
using System.Collections;
using EPPZGeometry;


public class EPPZDebug_IntersectionVertex : MonoBehaviour
{


	public LineRenderer previousEdgeRenderer;
	public LineRenderer nextEdgeRenderer;

	IntersectionVertex intersectionVertex;

	public void SetupWithIntersectionVertex(IntersectionVertex intersectionVertex_)
	{
		// Model.
		intersectionVertex = intersectionVertex_;

		// Name.
		this.name = "Intersection Vertex ("+intersectionVertex.previousEdge.index+"-"+intersectionVertex.nextEdge.index+")";

		// Layout.
		float z = this.transform.position.z;
		this.transform.position = new Vector3 (
			intersectionVertex.point.x,
			intersectionVertex.point.y,
			this.transform.position.z
			);

		previousEdgeRenderer.SetPosition(0, new Vector3(
			intersectionVertex.previousEdge.a.x,
			intersectionVertex.previousEdge.a.y,
			z));

		previousEdgeRenderer.SetPosition(1, new Vector3(
			intersectionVertex.previousEdge.b.x,
			intersectionVertex.previousEdge.b.y,
			z));

		nextEdgeRenderer.SetPosition(0, new Vector3(
			intersectionVertex.nextEdge.a.x,
			intersectionVertex.nextEdge.a.y,
			z));
		
		nextEdgeRenderer.SetPosition(1, new Vector3(
			intersectionVertex.nextEdge.b.x,
			intersectionVertex.nextEdge.b.y,
			z));
	}
}
