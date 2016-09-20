using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EPPZ.Geometry;
using EPPZ.Lines;


public class TestScene_10_Controller : MonoBehaviour
{

	public Transform centroid;
	public PolygonSource[] polygonSources;
	List<Polygon> polygons = new List<Polygon>();


	void Update()
	{
		// Collect polygons.
		polygons.Clear();
		foreach (PolygonSource eachPolygonSource in polygonSources)
		{ polygons.Add(eachPolygonSource.polygon); }

		// Calculate compund centroid.
		centroid.position = Geometry.CentroidOfPolygons(polygons.ToArray());
	}
}
