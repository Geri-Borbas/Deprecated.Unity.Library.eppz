using UnityEngine;
using System.Collections;
using EPPZ.Geometry;


namespace EPPZ.Lines
{


	public class SegmentLineRenderer : DirectLineRenderer
	{


		public Color lineColor;
		public Color boundsColor;
		private Segment segment;
		
		
		void Start()
		{
			// Model reference.
			SegmentSource segmentSource_ = GetComponent<SegmentSource>();
			segment = segmentSource_.segment;
		}
		
		protected override void OnDraw()
		{
			DrawRect(segment.bounds, boundsColor);
			DrawSegment(segment, lineColor);
		}
	}
}