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

	public EPPZGeometry_Polygon_Inspector polygonInspector;
	public EPPZGeometry_Polygon_Inspector rawOffsetPolygonInspector;
	public EPPZGeometry_Polygon_Inspector offsetPolygonInspector;

	IEnumerator offsetPolygonEnumerator;

	void Start()
	{
		// Debug.
		polygonInspector.polygon = polygonSource.polygon;

		// Kick off.
		List<IntersectionVertex> intersectionVertices = new List<IntersectionVertex>();
		offsetPolygonEnumerator = polygon.OffsetPolygon(offset, intersectionVertices);
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			OnMouseDown();
		}
	}

	void OnMouseDown()
	{
		// Step.
		offsetPolygonEnumerator.MoveNext();
				
		// Pass to renderers.
		rawOffsetPolygonRenderer.polygon = polygon.rawOffsetPolygon;
		offsetPolygonRenderer.polygon = polygon.offsetPolygon; 
		
		// Debug.
		rawOffsetPolygonInspector.polygon = polygon.rawOffsetPolygon;
		offsetPolygonInspector.polygon = polygon.offsetPolygon;
	}
}
