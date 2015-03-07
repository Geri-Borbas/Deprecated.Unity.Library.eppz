using UnityEngine;
using System.Collections;
using EPPZGeometry;
using EPPZDebug;


public class TestScene_00_Controller : MonoBehaviour
{


	// 00 Polygon-point containment


	public Material polygonMaterial;
	public Material passingMaterial;

	public EPPZGeometry_PolygonSource polygonSource;
	public GameObject[] pointObjects;
	public EPPZDebug_PolygonDebugRenderer polygonRenderer;

	private Polygon polygon { get { return polygonSource.polygon; } }
		

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
			containsAllPoints &= polygon.ContainsPoint(eachPoint);
		}
		return containsAllPoints;
	}
	
	void RenderTestResult(bool testResult)
	{
		// Set corresponding materials.
		Material currentMaterial = (testResult) ? passingMaterial : polygonMaterial;
		polygonRenderer.lineMaterial = currentMaterial;
		foreach (GameObject eachPointObject in pointObjects)
		{ eachPointObject.GetComponent<Renderer>().material = currentMaterial; }
	}
}
