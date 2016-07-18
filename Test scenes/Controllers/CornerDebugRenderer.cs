using UnityEngine;
using System.Collections;
using EPPZ.Geometry;


namespace EPPZ.DebugTools
{


	public class CornerDebugRenderer : DebugRenderer
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
}