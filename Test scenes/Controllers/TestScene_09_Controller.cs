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
	public EPPZDebug_PolygonDebugRenderer offsetPolygonRenderer;

	private Polygon offsetPolygon;
	private Polygon polygon { get { return polygonSource.polygon; } }

	public EPPZGeometry_Polygon_Inspector polygonInspector;
	public EPPZGeometry_Polygon_Inspector offsetPolygonInspector;


	void Start()
	{
		// Debug.
		polygonInspector.polygon = polygonSource.polygon;
	}

	void Update()
	{
		offsetPolygon = polygon.OffsetPolygon(offset);

		// Render.
		offsetPolygonRenderer.polygon = offsetPolygon; 

		// Debug.
		offsetPolygonInspector.polygon = offsetPolygon;
	}
}
