using UnityEngine;
using System.Collections;
using EPPZGeometry;
using EPPZDebug;


public class TestScene_01_Controller : MonoBehaviour
{


	// 01 Polygon-segment intersection


	public Material polygonMaterial;
	public Material passingMaterial;

	public EPPZGeometry_PolygonSource starSource;
	public EPPZGeometry_SegmentSource segmentSourceA;
	public EPPZGeometry_SegmentSource segmentSourceB;
	public EPPZDebug_PolygonDebugRenderer starRenderer;
	public EPPZDebug_SegmentDebugRenderer segmentRendererA;
	public EPPZDebug_SegmentDebugRenderer segmentRendererB;

	private Polygon star { get { return starSource.polygon; } }
	private Segment segment_a { get { return segmentSourceA.segment; } }
	private Segment segment_b { get { return segmentSourceB.segment; } }
		

	void Update()
	{
		RenderTestResult(SegmentIntersectingTest());
	}

	bool SegmentIntersectingTest()
	{
		return (
			star.IsIntersectingWithSegment(segment_a) ||
			star.IsIntersectingWithSegment(segment_b)
			);
	}

	void RenderTestResult(bool testResult)
	{
		// Set corresponding materials.
		Material currentMaterial = (testResult) ? passingMaterial : polygonMaterial;
		starRenderer.lineMaterial = currentMaterial;
		segmentRendererA.lineMaterial = currentMaterial;
		segmentRendererB.lineMaterial = currentMaterial;
	}
}
