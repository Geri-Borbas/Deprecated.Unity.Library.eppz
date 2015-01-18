using UnityEngine;
using System.Collections;
using EPPZGeometry;
using EPPZDebug;


public class TestScene_08_Controller : MonoBehaviour
{


	// 08 Segment-segment intersection point


	public Material polygonMaterial;
	public Material passingMaterial;

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
		bool areIntersecting = segment_a.IsIntersectingWithSegment(segment_b);
		// if (areIntersecting)
		{
			Vector2 intersectionPoint = 
				Geometry.IntersectionOfSegments(
					segment_a.a,
					segment_a.b,
					segment_b.a,
					segment_b.b
					);

			intersectionPointObject.transform.position = new Vector3(
				intersectionPoint.x,
				intersectionPoint.y,
				0.0f
				); // Align point
		}

		return areIntersecting;
	}

	void RenderTestResult(bool testResult)
	{
		// Set corresponding materials.
		Material currentMaterial = (testResult) ? passingMaterial : polygonMaterial;
		segmentRendererA.lineMaterial = currentMaterial;
		segmentRendererB.lineMaterial = currentMaterial;
		intersectionPointObject.renderer.material = currentMaterial;
	}
}
