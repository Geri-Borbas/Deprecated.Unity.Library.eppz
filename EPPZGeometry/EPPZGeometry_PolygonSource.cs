using UnityEngine;
using System.Collections;


namespace EPPZGeometry
{


	public class EPPZGeometry_PolygonSource : MonoBehaviour
	{


		public Transform[] pointTransforms;
		public bool updateModel = false;

		public Polygon polygon;


		void Awake()
		{
			// Construct a polygon model from transforms.
			polygon = Polygon.PolygonWithSource(this);
		}

		void Update()
		{
			if (updateModel)
			{
				// Update polygon model with transforms, also update calculations.
				polygon.UpdateWithSource(this);
			}
		}
	}
}
