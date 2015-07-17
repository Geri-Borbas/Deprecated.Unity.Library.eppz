using UnityEngine;
using System.Collections;


namespace EPPZ.NGUI
{


	/// <summary>
	/// Layout tool that binds a foreign widget size to this widget dimensions.
	/// </summary>


	[ExecuteInEditMode]
	[AddComponentMenu("eppz!/NGUI/Size Constraint")]
	public class EPPZNGUI_SizeConstraint : MonoBehaviour
	{


		/// <summary>
		/// Widget to control on this GameObject.
		/// </summary>
		private UIWidget _widget;
		private UIWidget widget
		{
			get
			{
				if (_widget == null)
				{ _widget = GetComponent<UIWidget>(); }
				return _widget;
			}
		}

		/// <summary>
		/// Foreign Widget to get size from.
		/// </summary>
		public UIWidget target;


		// Doing.
		public float multiplier = 1.0f;


		#region Events

		void Update()
		{

			// Should be OnBeforeUpdate

			// Add widget listener lazy.
			if (widget.onBeforeAnchor == null)
			{ widget.onBeforeAnchor = OnBeforeAnchor; }

			// Add target listener lazy.
			if (target.onBeforeAnchor == null)
			{ target.onBeforeAnchor = OnBeforeTargetAnchor; }
		}

		private void OnBeforeAnchor()
		{
			Debug.Log(name+".OnBeforeAnchor()");
			
			LayoutDimensions();
			AdjustAnchorsToDimensions(widget.width, widget.height);
		}
		
		private void OnBeforeTargetAnchor()
		{
			Debug.Log(name+".OnBeforeTargetAnchor()");

			LayoutDimensions();
			AdjustAnchorsToDimensions(widget.width, widget.height);
		}

		#endregion


		#region Layout

		private void LayoutDimensions()
		{
			Debug.Log(name+".LayoutDimensions()");
			
			int width = widget.width;
			int height = Mathf.RoundToInt((float)target.height * multiplier);
			
			// Apply new dimensions (yet without Anchor adjustments).
			// Comes with every NGUI feature (pixel snapping, handle rotations, power of two paddings, etc.).
			widget.SetDimensions(width, height);
		}

		// From `NGUIMath.AdjustWidget` implementation.
		private void AdjustAnchorsToDimensions(int width, int height)
		{
			// Vector3 bottomLeft = widget.localCorners[0];
			// widget.SetRect(bottomLeft.x, bottomLeft.y, width, height);

			Debug.Log(name+".AdjustAnchorsToDimensions()");

			// Adjust Anchors if any.
			if (widget.isAnchored)
			{
				Transform parentTransform = widget.cachedTransform.parent;
				float x = widget.cachedTransform.localPosition.x - widget.pivotOffset.x * width;
				float y = widget.cachedTransform.localPosition.y - widget.pivotOffset.y * height;
				
				if (widget.leftAnchor.target) widget.leftAnchor.SetHorizontal(parentTransform, x);
				if (widget.rightAnchor.target) widget.rightAnchor.SetHorizontal(parentTransform, x + width);
				if (widget.bottomAnchor.target) widget.bottomAnchor.SetVertical(parentTransform, y);
				if (widget.topAnchor.target) widget.topAnchor.SetVertical(parentTransform, y + height);
			}
			
			#if UNITY_EDITOR
			NGUITools.SetDirty(widget);
			#endif
		}

		#endregion
	}
}