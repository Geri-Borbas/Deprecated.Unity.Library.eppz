using UnityEngine;
using System.Collections;


namespace EPPZ.NGUI
{


	/// <summary>
	/// Determine when to execute constrain layouts (use full name `EPPZ.NGUI.ConstraintUpdate` for safety). Updating constraint `Manually` means you are responsible to call `Layout()` on the constraint when desired.
	/// </summary>
	public enum ConstraintUpdate { OnUpdate, OnTargetUpdate, OnBothUpdate, Manually }

	/// <summary>
	/// Size constraint constants to use within namespace (use full name `EPPZ.NGUI.SizeConstraint` for safety).
	/// </summary>
	public enum SizeConstraint { Width, Height, Both }

	/// <summary>
	/// Aspect constraint constants to use within namespace (use full name `EPPZ.NGUI.AspectConstraint` for safety).
	/// </summary>
	public enum AspectConstraint { BasedOnWidth, BasedOnHeight }
}
