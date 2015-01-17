using UnityEngine;
using System.Collections;
using EPPZGeometry;
using EPPZDebug;


public class TestScene_02_Controller : MonoBehaviour
{


	// 02 Polygon permiter-point containment (precise)


	public float accuracy = 1.0f;
	private float _previousAccuracy;

	public Material polygonMaterial;
	public Material intersectingMaterial;

	public EPPZGeometry_PolygonSource starSource;
	public GameObject pointSource;
	public EPPZDebug_PolygonDebugRenderer starRenderer;

	private Polygon star { get { return starSource.polygon; } }
	private Vector2 point  { get { return pointSource.transform.position.xy(); } }


	void Update()
	{
		RenderTestResult(PointContainmentTest());
	}

	bool PointContainmentTest()
	{
		return star.PermiterContainsPoint(point, accuracy, Segment.ContainmentMethod.Precise);
	}

	void RenderTestResult(bool testResult)
	{
		// Set corresponding materials.
		Material renderMaterial = (testResult) ? intersectingMaterial : polygonMaterial;
		starRenderer.lineMaterial = renderMaterial;
		pointSource.renderer.material = renderMaterial;
	}
}
