using UnityEngine;
using System.Collections;


namespace EPPZ.Geometry
{


	public class SegmentSource : MonoBehaviour
	{


		public Transform[] pointTransforms;
		public bool updateModel = false;

		public Segment segment;


		void Awake()
		{
			// Construct a segment model from transforms.
			segment = Segment.SegmentWithSource(this);
		}
		
		void Update()
		{
			if (updateModel)
			{
				// Update segment model with transforms, also update calculations.
				segment.UpdateWithSource(this);
			}
		}
	}
}
