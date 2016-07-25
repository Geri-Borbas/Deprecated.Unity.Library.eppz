using UnityEngine;
using System.Collections;
using EPPZ.Geometry;
using EPPZ.Lines;


public class TestScene_052_Controller : MonoBehaviour
{


	// 05 Polygon-polygon containment


	public Material polygonMaterial;
	public Material intersectingMaterial;

	public PolygonSource polygonSource;
	public PolygonSource squareSource;
	public PolygonLineRenderer polygonRenderer;
	public PolygonLineRenderer squareRenderer;

	private Polygon polygon { get { return polygonSource.polygon; } }
	private Polygon square { get { return squareSource.polygon; } }
		

	void Update()
	{
		RenderTestResult(IsPolygonInsideTest());
	}

	bool IsPolygonInsideTest()
	{
		// Point containment.
		bool pointContainment = true;
		square.EnumeratePointsRecursive((Vector2 eachPoint) =>
		{
			pointContainment &= polygon.ContainsPoint(eachPoint);
		});

		// Segment-Polygon intersecion, Segment endpoint-permiter contaimnent.
		bool segmentIntersecting = false;
		foreach (Edge eachEdge in square.edges)
		{
			segmentIntersecting |= polygon.IsIntersectingWithSegment(eachEdge);
		}

		// A polygon contains a foreign polygon, when
		// + foreign polygon vertices are contained by polygon
		// + foreign polygon segments are not intersecting with polygon
		bool polygonInside = (
			pointContainment &&
			segmentIntersecting == false
			);

		return polygonInside;
	}

	void RenderTestResult(bool testResult)
	{
		// Set corresponding materials.
		Material renderMaterial = (testResult) ? intersectingMaterial : polygonMaterial;
		polygonRenderer.lineColor = renderMaterial.color;
		squareRenderer.lineColor = renderMaterial.color;
	}
}
