using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;


namespace EPPZGeometry
{


	public class EPPZGeometry_PolygonSource : MonoBehaviour
	{


		public Transform[] pointTransforms;
		public Polygon.WindingDirection windingDirection = Polygon.WindingDirection.Unknown;
		[FormerlySerializedAs("updating")] public bool updateModel = false;

		public Polygon polygon;


		void Awake()
		{
			// Construct a polygon model from transforms (if not created by a root polygon already).
			if (polygon == null) polygon = Polygon.PolygonWithSource(this);
		}

		void Update()
		{
			if (updateModel)
			{
				// Update polygon model with transforms, also update calculations.
				polygon.UpdatePointPositionsWithSource(this);
			}
		}
	}
}
