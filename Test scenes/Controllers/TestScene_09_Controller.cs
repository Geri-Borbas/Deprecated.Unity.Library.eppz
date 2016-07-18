using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EPPZ.Geometry;
using EPPZ.DebugTools;


public class TestScene_09_Controller : MonoBehaviour
{


	public Material offsetPolygonMaterial;
	public float offset = 0.5f;

	public PolygonSource polygonSource;
	public PolygonDebugRenderer offsetPolygonRenderer;

	private Polygon offsetPolygon;
	private Polygon polygon { get { return polygonSource.polygon; } }

	public PolygonInspector polygonInspector;
	public PolygonInspector offsetPolygonInspector;


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
