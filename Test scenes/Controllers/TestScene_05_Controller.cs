using UnityEngine;
using System.Collections;
using EPPZGeometry;
using EPPZ.DebugTools;


public class TestScene_05_Controller : MonoBehaviour
{


	// 05 Polygon-polygon containment


	public Material polygonMaterial;
	public Material intersectingMaterial;

	public EPPZGeometry_PolygonSource starSource;
	public EPPZGeometry_PolygonSource squareSource;
	public PolygonDebugRenderer starRenderer;
	public PolygonDebugRenderer squareRenderer;

	private Polygon star { get { return starSource.polygon; } }
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
			pointContainment &= star.ContainsPoint(eachPoint);
		});

		// Segment-Polygon intersecion, Segment endpoint-permiter contaimnent.
		bool segmentIntersecting = false;
		bool permiterContainsSegment = false;
		foreach (Edge eachEdge in square.edges)
		{
			permiterContainsSegment |= star.PermiterContainsPoint(eachEdge.a) || star.PermiterContainsPoint(eachEdge.b);
			segmentIntersecting |= star.IsIntersectingWithSegment(eachEdge);
		}

		// A polygon contains a foreign polygon, when
		// + foreign polygon vertices are contained by polygon
		// + foreign polygon segments are not intersecting with polygon
		// + foreign polygon vertices are not contained by polygon permiter
		bool polygonInside = (
			pointContainment &&
			segmentIntersecting == false &&
			permiterContainsSegment == false
			);

		return polygonInside;
	}

	void RenderTestResult(bool testResult)
	{
		// Set corresponding materials.
		Material renderMaterial = (testResult) ? intersectingMaterial : polygonMaterial;
		starRenderer.lineMaterial = renderMaterial;
		squareRenderer.lineMaterial = renderMaterial;
	}
}
