using UnityEngine;
using System.Collections;
using EPPZ.Geometry;
using EPPZ.Lines;


public class TestScene_02_Controller : MonoBehaviour
{


	// 02 Polygon permiter-point containment (precise)


	public float accuracy = 1.0f;
	private float _previousAccuracy;

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
		return polygon.PermiterContainsPoint(point, accuracy, Segment.ContainmentMethod.Precise);
	}

	void RenderTestResult(bool testResult)
	{
		// Set corresponding materials.
		Material renderMaterial = (testResult) ? intersectingMaterial : polygonMaterial;
		polygonRenderer.lineColor = renderMaterial.color;
		pointSource.GetComponent<Renderer>().material = renderMaterial;
	}
}
