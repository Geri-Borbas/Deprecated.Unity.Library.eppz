using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EPPZ.Geometry;


namespace EPPZ.Lines
{


	/**
	 * Submitting drawing in a `DirectLineRenderer` gets drawn for a single frame.
	 * Suitable for dynamic drawngs where underlying line model keep changing every
	 * frame.
	 */
	public class DirectLineRenderer : LineRendererBase
	{



		#region Events

			public override void OnLineRendererCameraPostRender()
			{
				if (isActive == false) return; // Only if active
				OnDraw(); // Collect lines to the batch from subclasses
			}

			protected virtual void OnDraw()
			{
				/**
				 * Subclass template.
				 * 
				 * Use `DrawLine(from, to, color)` in subclasses here, or further
				 * `EPPZ.Geometry` drawing methods in `EPPZ.Lines.LineRendererBase`.
				 * 
				 * See `EPPZ.Lines.PolygonLineRenderer` for example.
				 */
			}

		#endregion


		#region Batch lines

			protected override void DrawLine(Vector2 from, Vector2 to, Color color)
			{
				if (Application.isPlaying)
				{
					// Create and batch.
					Line line = new Line();
					line.from = from;
					line.to = to;
					line.color = color;

					// Collect directly.
					LineRendererCamera.shared.BatchLine(line);
				}
			}

		#endregion
	}
}