using UnityEngine;
using System.Collections;
using EPPZGeometry;
using EPPZDebug;


public class TestScene_08_Controller : MonoBehaviour
{


	// 08 Segment-segment intersection point


	public Material polygonMaterial;
	public Material passingMaterial;
	public float accuracy = 0.0f;

	public EPPZGeometry_SegmentSource segmentSourceA;
	public EPPZGeometry_SegmentSource segmentSourceB;
	public EPPZDebug_SegmentDebugRenderer segmentRendererA;
	public EPPZDebug_SegmentDebugRenderer segmentRendererB;

	public GameObject intersectionPointObject;

	private Segment segment_a { get { return segmentSourceA.segment; } }
	private Segment segment_b { get { return segmentSourceB.segment; } }
		

	void Update()
	{
		RenderTestResult(SegmentIntersectionTest());
	}

	bool SegmentIntersectionTest()
	{
		Vector2 intersectionPoint;
		bool isIntersecting = segment_a.IntersectionWithSegmentWithAccuracy(segment_b, accuracy, out intersectionPoint);
		if (isIntersecting)
		{
			intersectionPointObject.transform.position = new Vector3(
				intersectionPoint.x,
				intersectionPoint.y,
				intersectionPointObject.transform.position.z
				); // Align point
		}

		return isIntersecting;
	}

	void RenderTestResult(bool testResult)
	{
		// Set corresponding materials.
		Material currentMaterial = (testResult) ? passingMaterial : polygonMaterial;
		segmentRendererA.lineMaterial = currentMaterial;
		segmentRendererB.lineMaterial = currentMaterial;

		// Show / hide intersection point.
		intersectionPointObject.GetComponent<Renderer>().enabled = testResult;
	}
}
