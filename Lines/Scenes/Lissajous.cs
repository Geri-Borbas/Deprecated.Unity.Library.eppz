using UnityEngine;
using System.Collections;


public class Lissajous : EPPZ.Lines.DirectLineRenderer
{


	float t = 0.0f;

	const float resolution = 0.01f;
	const float speed = 0.2f;


	protected override void OnDraw ()
	{
		// Draw Lissajous curve segments.
		Vector2 from = Function(t, 0.0f);
		for (float s = resolution; s < Mathf.PI * (2.0 + resolution); s += resolution)
		{
			Vector2 to = Function(t, s);

			// Direct draw.
			DrawLine(from, to, Color.white);

			from = to;
		}

		// Step.
		t += speed;
	}

	Vector2 Function(float t, float s)
	{
		// More at https://en.wikipedia.org/wiki/Lissajous_curve
		return new Vector2(
			Mathf.Sin (5 * s + t),
			Mathf.Sin (4 * s + t)
		);
	}
}
