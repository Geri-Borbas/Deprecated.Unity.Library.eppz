using UnityEngine;
using System.Collections;
using EPPZGeometry;
using EPPZDebug;


public class TestScene_06_Controller : MonoBehaviour
{

	public Material validNormalMaterial;
	public Material invalidNormalMaterial;

	public EPPZDebug_CornerDebugRenderer cornerRenderer;
	public Renderer normalPointRenderer;

	public Transform[] segmentATransforms;
	public Transform[] segmentBTransforms;
	public Transform[] normalTransforms;

	private Segment segmentA;
	private Segment segmentB;
	private Segment normal;

	void Start()
	{
		// Create models.
		segmentA = Segment.SegmentWithPointTransforms(segmentATransforms);
		segmentB = Segment.SegmentWithPointTransforms(segmentBTransforms);
		normal = Segment.SegmentWithPointTransforms(normalTransforms);

		// Feed renderer.
		cornerRenderer.segmentA = segmentA;
		cornerRenderer.segmentB = segmentB;
		cornerRenderer.normal = normal;
	}

	void Update()
	{
		// Update model.
		segmentA.UpdateWithTransforms(segmentATransforms);
		segmentB.UpdateWithTransforms(segmentBTransforms);
		normal.UpdateWithTransforms(normalTransforms);

		// Test.
		RenderTestResult(NormalFacingTest());
	}

	bool NormalFacingTest()
	{
		Vector2 point = normal.b;
		bool acute = segmentA.IsPointLeft(segmentB.b);
		bool leftA = segmentA.IsPointLeft(point);
		bool leftB = segmentB.IsPointLeft(point);
		bool outward = (acute) ? leftA && leftB : leftA || leftB;
		bool inward = !outward;
		return inward;
	}

	void RenderTestResult(bool testResult)
	{
		// Set corresponding materials.
		Material normalMaterial = (testResult) ? validNormalMaterial : invalidNormalMaterial;
		cornerRenderer.segmentNormalMaterial = normalMaterial;
		normalPointRenderer.material = normalMaterial;
	}
}
