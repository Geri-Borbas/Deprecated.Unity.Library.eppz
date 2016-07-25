using UnityEngine;
using System.Collections;
using EPPZ.Geometry;
using EPPZ.Lines;


public class TestScene_08_Controller : MonoBehaviour
{


	// 08 Segment-segment intersection point


	public Material polygonMaterial;
	public Material passingMaterial;
	public float accuracy = 0.0f;

	public SegmentSource segmentSourceA;
	public SegmentSource segmentSourceB;
	public SegmentLineRenderer segmentRendererA;
	public SegmentLineRenderer segmentRendererB;

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
		segmentRendererA.lineColor = currentMaterial.color;
		segmentRendererB.lineColor = currentMaterial.color;

		// Show / hide intersection point.
		intersectionPointObject.GetComponent<Renderer>().enabled = testResult;
	}
}
