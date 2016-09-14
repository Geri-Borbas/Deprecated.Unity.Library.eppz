using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;


namespace EPPZ.Geometry
{


	public class PolygonSource : MonoBehaviour
	{


		public Transform[] pointTransforms;
		public Polygon.WindingDirection windingDirection = Polygon.WindingDirection.Unknown;
		[Range (-2,2)] public float offset = 0.0f;
		public bool updateModel = false;

		public Polygon polygon;


		void Awake()
		{
			// Construct a polygon model from transforms (if not created by a root polygon already).
			if (polygon == null) polygon = Polygon.PolygonWithSource(this);
			if (offset != 0.0f) polygon = polygon.OffsetPolygon(offset);
		}

		void Update()
		{
			if (updateModel)
			{
				// Update polygon model with transforms, also update calculations.
				polygon.UpdatePointPositionsWithSource(this);
				if (offset != 0.0f) polygon = polygon.OffsetPolygon(offset);
			}
		}
	}
}
