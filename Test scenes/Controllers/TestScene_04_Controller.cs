using UnityEngine;
using System.Collections;
using EPPZGeometry;
using EPPZDebug;


public class EPPZPolygon_2_Controller : MonoBehaviour
{


	public Material polygonMaterial;
	public Material intersectingMaterial;

	public EPPZGeometry_PolygonSource starSource;
	public EPPZGeometry_PolygonSource squareSource;
	public EPPZDebug_PolygonDebugRenderer starRenderer;
	public EPPZDebug_PolygonDebugRenderer squareRenderer;

	private Polygon star { get { return starSource.polygon; } }
	private Polygon square { get { return squareSource.polygon; } }
		

	void Update()
	{
		RenderTestResult(IsSegmentsInsideTest());
	}

	bool IsSegmentsInsideTest()
	{
		// Point containment.
		bool pointContainment = true;
		square.EnumeratePoints((Vector2 eachPoint) =>
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

		// Polygon inside test.
		bool polygonInside = pointContainment && segmentIntersecting == false && permiterContainsSegment == false;

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
