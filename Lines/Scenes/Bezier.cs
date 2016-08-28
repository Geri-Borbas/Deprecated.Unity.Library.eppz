using UnityEngine;
using System.Collections;


public class Bezier : EPPZ.Lines.DirectLineRenderer
{


	public Transform a;
	public Transform b;
	public Transform c;
	[Range (1,50)] public int segments = 20;


	protected override void OnDraw()
	{
		// Increments.
		Vector3 increment_a_b = (b.position - a.position) / (float)segments;
		Vector3 increment_b_c = (c.position - b.position) / (float)segments;

		// Draw segments.
		for (int index = 0; index <= segments; index ++)
		{
			Vector2 from = a.position + (increment_a_b * index);
			Vector2 to = b.position + (increment_b_c * index);

			// Direct draw.
			DrawLine(from, to, Color.white);
		}
	}
}
