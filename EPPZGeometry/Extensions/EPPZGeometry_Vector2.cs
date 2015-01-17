using System;
using UnityEngine;


namespace EPPZGeometry
{


	public static class EPPZGeometry_Vector2
	{


		public static Vector2 Rotated(this Vector2 this_, float degrees)
		{
			// Checks.
			float radians = degrees * Mathf.Deg2Rad;
			if (radians == 0.0f) return this_;
			if (radians == (Mathf.PI * 2.0f)) return this_;

			float sin = Mathf.Sin(radians);
			float cos = Mathf.Cos(radians);

			Vector2 rotated = new Vector2(
				(cos * this_.x) - (sin * this_.y),
				(sin * this_.x) + (cos * this_.y)
				);

			return rotated;
		}

		public static Vector2 RotatedAround(this Vector2  this_, Vector2 around, float degrees)
		{
			// Checks.
			float radians = degrees * Mathf.Deg2Rad;
			if (radians == 0.0f) return this_;
			if (radians == (Mathf.PI * 2.0f)) return this_;
			
			float sin = Mathf.Sin(radians);
			float cos = Mathf.Cos(radians);
			Vector2 rotated = new Vector2(
				((this_.x - around.x) * cos) - ((around.y - this_.y) * sin) + around.x,
				((around.y - this_.y) * cos) - ((this_.x- around.x) * sin) + around.y
				);
			
			return rotated;
		}
	}
}

