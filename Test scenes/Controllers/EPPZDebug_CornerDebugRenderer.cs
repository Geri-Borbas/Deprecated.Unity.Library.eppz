using UnityEngine;
using System.Collections;
using EPPZDebug;
using EPPZGeometry;


public class EPPZDebug_CornerDebugRenderer : EPPZ_DebugRenderer
{


	public Material segmentAMaterial;
	public Material segmentBMaterial;
	public Material segmentNormalMaterial;

	public Segment segmentA;
	public Segment segmentB;
	public Segment normal;

	
	protected override void OnDraw()
	{
		DrawSegment(segmentA, segmentAMaterial);
		DrawSegment(segmentB, segmentBMaterial);
		DrawSegment(normal, segmentNormalMaterial);
	}
}
