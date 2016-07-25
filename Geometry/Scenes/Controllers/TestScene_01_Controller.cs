using UnityEngine;
using System.Collections;
using EPPZ.Geometry;
using EPPZ.Lines;


public class TestScene_01_Controller : MonoBehaviour
{


	// 01 Polygon-segment intersection


	public Material polygonMaterial;
	public Material passingMaterial;

	public PolygonSource polygonSource;
	public SegmentSource segmentSourceA;
	public SegmentSource segmentSourceB;
	public PolygonLineRenderer polygonRenderer;
	public SegmentLineRenderer segmentRendererA;
	public SegmentLineRenderer segmentRendererB;

	private Polygon polygon { get { return polygonSource.polygon; } }
	private Segment segment_a { get { return segmentSourceA.segment; } }
	private Segment segment_b { get { return segmentSourceB.segment; } }
		

	void Update()
	{
		RenderTestResult(SegmentIntersectingTest());
	}

	bool SegmentIntersectingTest()
	{
		return (
			polygon.IsIntersectingWithSegment(segment_a) ||
			polygon.IsIntersectingWithSegment(segment_b)
			);
	}

	void RenderTestResult(bool testResult)
	{
		// Set corresponding materials.
		Material currentMaterial = (testResult) ? passingMaterial : polygonMaterial;
		polygonRenderer.lineColor = currentMaterial.color;
		segmentRendererA.lineColor = currentMaterial.color;
		segmentRendererB.lineColor = currentMaterial.color;
	}
}
