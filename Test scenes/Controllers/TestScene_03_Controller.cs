using UnityEngine;
using System.Collections;
using EPPZGeometry;
using EPPZDebug;


public class TestScene_03_Controller : MonoBehaviour
{


	// 03 Polygon permiter-point containment (default)


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
		float accuracy = 0.1f;
		return star.PermiterContainsPoint(point, accuracy);
	}

	void RenderTestResult(bool testResult)
	{
		// Set corresponding materials.
		Material renderMaterial = (testResult) ? intersectingMaterial : polygonMaterial;
		starRenderer.lineMaterial = renderMaterial;
		pointSource.renderer.material = renderMaterial;
	}
}
