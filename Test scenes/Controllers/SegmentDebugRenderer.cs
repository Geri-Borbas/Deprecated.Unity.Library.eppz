using UnityEngine;
using System.Collections;
using EPPZGeometry;


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
			EPPZGeometry_SegmentSource segmentSource_ = GetComponent<EPPZGeometry_SegmentSource>();
			segment = segmentSource_.segment;
		}
		
		protected override void OnDraw()
		{
			DrawRect(segment.bounds, boundsMaterial);
			DrawSegment(segment, lineMaterial);
		}
	}
}