using UnityEngine;
using System.Collections;


namespace EPPZ.NGUI
{
	
	
	/// <summary>
	/// Layout tool that binds a foreign widget position to this widget.
	/// </summary>
	
	
	[ExecuteInEditMode]
	[AddComponentMenu("eppz!/NGUI/Position Constraint")]
	public class EPPZNGUI_PositionConstraint : EPPZNGUI_Constraint
	{


		protected override void Layout()
		{
			widget.cachedTransform.position = targetWidget.cachedTransform.position;
		}
	}
}
