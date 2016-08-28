using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace EPPZ.Lines
{


	/**
	 * Caching a  drawing in a `CachedLineRenderer` gets drawn each frame.
	 * Suitable for static drawngs where underlying line model not changes
	 * too often.
	 */
	public class CachedLineRenderer : LineRendererBase
	{


		public List<EPPZ.Lines.Line> lines = new List<EPPZ.Lines.Line>();


		#region Events

			/**
			 * You can just add up lines directly in the Inspector, or use same
			 * drawing methods just like in `DirectLineRenderer`, but drawing
			 * (each line drawn) gets cached here in `lines` collection, and gets
			 * rendered each frame.
			 */

		#endregion


		#region Cache lines when use drawing methods

			protected override void DrawLine(Vector2 from, Vector2 to, Color color)
			{
				if (Application.isPlaying)
				{
					// Create and batch.
					Line line = new Line();
					line.from = from;
					line.to = to;
					line.color = color;

					// Collect here.
					lines.Add(line);
				}
			}

		#endregion
	}
}