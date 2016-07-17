using UnityEngine;
using System.Collections;


namespace EPPZ.NGUI
{
	
	
	/// <summary>
	/// Layout tool that binds a foreign widget position to this widget.
	/// </summary>
	
	
	[ExecuteInEditMode]
	[AddComponentMenu("eppz!/NGUI/Position Constraint")]
	public class PositionConstraint : Constraint
	{


		public override void Layout()
		{
			// Get local position difference.
			Vector3 targetPosition = widget.cachedTransform.parent.InverseTransformPoint(targetWidget.cachedTransform.position);
			Vector3 widgetPosition = widget.cachedTransform.localPosition;
			Vector3 positionDifference = targetPosition - widgetPosition;

			// Adjust every NGUI internal goodie under the hood (Anchors, Serialization, etc.).
			NGUIMath.MoveWidget(widget, positionDifference.x, positionDifference.y);
		}
	}
}
