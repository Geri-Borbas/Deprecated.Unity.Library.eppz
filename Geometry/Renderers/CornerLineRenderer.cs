using UnityEngine;
using System.Collections;
using EPPZ.Geometry;


namespace EPPZ.Lines
{


	public class CornerLineRenderer : DirectLineRenderer
	{


		public Color segmentAColor;
		public Color segmentBColor;
		public Color segmentNormalColor;

		public Segment segmentA;
		public Segment segmentB;
		public Segment normal;

		
		protected override void OnDraw()
		{
			DrawSegment(segmentA, segmentAColor);
			DrawSegment(segmentB, segmentBColor);
			DrawSegment(normal, segmentNormalColor);
		}
	}
}