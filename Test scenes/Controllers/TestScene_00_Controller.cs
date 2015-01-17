using UnityEngine;
using System.Collections;
using EPPZGeometry;
using EPPZDebug;


public class TestScene_00_Controller : MonoBehaviour
{


	// 00 Polygon-point containment


	public Material polygonMaterial;
	public Material passingMaterial;

	public EPPZGeometry_PolygonSource starSource;
	public GameObject[] pointObjects;
	public EPPZDebug_PolygonDebugRenderer starRenderer;

	private Polygon star { get { return starSource.polygon; } }
		

	void Update()
	{
		RenderTestResult(PointContainmentTest());
	}

	bool PointContainmentTest()
	{
		bool containsAllPoints = true;
		foreach (GameObject eachPointObject in pointObjects)
		{
			Vector2 eachPoint = eachPointObject.transform.position.xy();
			containsAllPoints &= star.ContainsPoint(eachPoint);
		}
		return containsAllPoints;
	}
	
	void RenderTestResult(bool testResult)
	{
		// Set corresponding materials.
		Material currentMaterial = (testResult) ? passingMaterial : polygonMaterial;
		starRenderer.lineMaterial = currentMaterial;
		foreach (GameObject eachPointObject in pointObjects)
		{ eachPointObject.renderer.material = currentMaterial; }
	}
}
