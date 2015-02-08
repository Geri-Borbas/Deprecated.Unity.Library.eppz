using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EPPZGeometry;
using EPPZDebug;


public class TestScene_09_Controller : MonoBehaviour
{


	public Material offsetPolygonMaterial;
	public float offset = 0.5f;

	public EPPZGeometry_PolygonSource polygonSource;
	public EPPZDebug_PolygonDebugRenderer rawOffsetPolygonRenderer;
	public EPPZDebug_PolygonDebugRenderer offsetPolygonRenderer;
	public GameObject intersectionVertexRenderer; // Prototype reference

	private string tagName = "Intersection Vertex Renderer";

	private Polygon offsetPolygon;
	private Polygon polygon { get { return polygonSource.polygon; } }


	void Update()
	{
		// Remove previous.
		GameObject[] intersectionVertexObjects = GameObject.FindGameObjectsWithTag(tagName);
		foreach (GameObject each in intersectionVertexObjects) Destroy(each);

		List<IntersectionVertex> intersectionVertices = new List<IntersectionVertex>();

		Polygon rawOffsetPolygon = null;
		offsetPolygon = polygon.OffsetPolygon(offset, out rawOffsetPolygon, intersectionVertices); // Create an offset polygon around

		// Pass to renderers.
		rawOffsetPolygonRenderer.polygon = rawOffsetPolygon;
		offsetPolygonRenderer.polygon = offsetPolygon; 

		foreach (IntersectionVertex each in intersectionVertices)
		{
			// Create debug object.
			GameObject eachObject = GameObject.Instantiate(intersectionVertexRenderer) as GameObject;
			eachObject.SetActive(true);
			eachObject.tag = tagName;

			EPPZDebug_IntersectionVertex eachRenderer = eachObject.GetComponent<EPPZDebug_IntersectionVertex>();
			eachRenderer.SetupWithIntersectionVertex(each);
		}
	}
}
