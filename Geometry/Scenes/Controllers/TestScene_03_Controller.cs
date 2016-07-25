using UnityEngine;
using System.Collections;
using EPPZ.Geometry;
using EPPZ.Lines;


public class TestScene_03_Controller : MonoBehaviour
{


	// 03 Polygon permiter-point containment (default)


	public Material polygonMaterial;
	public Material intersectingMaterial;

	public PolygonSource polygonSource;
	public GameObject pointSource;
	public PolygonLineRenderer polygonRenderer;

	private Polygon polygon { get { return polygonSource.polygon; } }
	private Vector2 point  { get { return pointSource.transform.position.xy(); } }

	void Update()
	{
		RenderTestResult(PointContainmentTest());
	}

	bool PointContainmentTest()
	{
		float accuracy = 0.1f;
		return polygon.PermiterContainsPoint(point, accuracy);
	}

	void RenderTestResult(bool testResult)
	{
		// Set corresponding materials.
		Material renderMaterial = (testResult) ? intersectingMaterial : polygonMaterial;
		polygonRenderer.lineColor = renderMaterial.color;
		pointSource.GetComponent<Renderer>().material = renderMaterial;
	}
}
