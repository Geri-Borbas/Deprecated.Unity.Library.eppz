using UnityEngine;
using System.Collections;
using EPPZ.Geometry;


namespace EPPZ.Lines
{


	public class PolygonLineRenderer : LineRenderer
	{


		public Color lineColor;
		public Color boundsColor;
		public GameObject windingDirectionObject;
		public TextMesh areaTextMesh;
		public bool normals = false;

		private float _previousArea;
		private Polygon.WindingDirection _previousWindingDirection;

		public Polygon polygon;
		
		
		void Start()
		{
			// Model reference.
			PolygonSource polygonSource_ = GetComponent<PolygonSource>();
			if (polygonSource_ != null)
			{ polygon = polygonSource_.polygon; }
		}

		void Update()
		{
			if (polygon == null) return; // Only having polygon

			// Layout winding direction object if any.
			bool hasWindingDirectionObject = (windingDirectionObject != null);
			bool windingChanged = (polygon.windingDirection != _previousWindingDirection);
			if (hasWindingDirectionObject && windingChanged)
			{
				windingDirectionObject.transform.localScale = (polygon.isCW) ? Vector3.one : new Vector3 (1.0f, -1.0f, 1.0f);
				windingDirectionObject.transform.rotation = (polygon.isCW) ? Quaternion.identity : Quaternion.Euler( new Vector3 (0.0f, 0.0f, 90.0f) );
			}

			// Layout area text mesh if any.
			bool hasAreaTextMesh = (areaTextMesh != null);
			bool areaChanged = (polygon.area != _previousArea);
			if (hasAreaTextMesh && areaChanged)
			{
				areaTextMesh.text = polygon.area.ToString();
			}

			// Track.
			_previousWindingDirection = polygon.windingDirection;
			_previousArea = polygon.area;
		}

		protected override void OnDraw()
		{
			if (polygon == null) return; // Only having polygon

			DrawRect(polygon.bounds, boundsColor);
			DrawPolygonWithTransform(polygon, lineColor, transform, normals);
		}
	}
}