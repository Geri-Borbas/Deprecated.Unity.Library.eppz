using UnityEngine;
using System.Collections;
using EPPZ.Geometry;


namespace EPPZ.DebugTools
{


	public class SegmentDebugRenderer : DebugRenderer
	{


		public Material lineMaterial;
		public Material boundsMaterial;
		private Segment segment;
		
		
		void Start()
		{
			// Model reference.
			SegmentSource segmentSource_ = GetComponent<SegmentSource>();
			segment = segmentSource_.segment;
		}
		
		protected override void OnDraw()
		{
			DrawRect(segment.bounds, boundsMaterial);
			DrawSegment(segment, lineMaterial);
		}
	}
}