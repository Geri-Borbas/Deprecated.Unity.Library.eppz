using UnityEngine;
using System.Collections;


namespace EPPZ.NGUI
{
	
	
	/// <summary>
	/// Layout tool that binds a foreign widget aspect ratio to this widget.
	/// </summary>
	
	
	[ExecuteInEditMode]
	[AddComponentMenu("eppz!/NGUI/Aspect Constraint")]
	public class EPPZNGUI_AspectConstraint : EPPZNGUI_Constraint
	{

		
		/// <summary>
		/// Scale the aspect got from the target (when not using Free aspect).
		/// </summary>
		public float multiplier = 1.0f;
		
		
		protected override void Layout()
		{
			widget.aspectRatio = targetWidget.aspectRatio * multiplier;

			// TODO: NGUI compatible layout using `NGUIMath.AdjustWidget`.
		}
	}
}