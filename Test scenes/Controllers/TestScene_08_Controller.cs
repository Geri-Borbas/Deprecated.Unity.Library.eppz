using UnityEngine;
using System.Collections;
using EPPZGeometry;
using EPPZDebug;


public class TestScene_08_Controller : MonoBehaviour
{


	public Material offsetPolygonMaterial;
	public float offset = 0.5f;

	public EPPZGeometry_PolygonSource polygonSource;
	public EPPZDebug_PolygonDebugRenderer offsetPolygonRenderer;

	private Polygon offsetPolygon;
	private Polygon polygon { get { return polygonSource.polygon; } }


	void Update()
	{
		offsetPolygon = polygon.OffsetPolygon(offset); // Create an offset polygon around
		offsetPolygonRenderer.polygon = offsetPolygon; // Pass to renderer
	}
}
