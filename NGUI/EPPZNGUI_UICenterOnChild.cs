using UnityEngine;
using System.Collections.Generic;
	

// Created as a copy of `UICenterOnChild` (NGUI 3.7.8) with slight modifications. Same behaviour
// with option to opt-out spring motion (centering now can happen on the same frame it requested).
// In addition it works with a fixed list of children determined beforehand.


namespace EPPZ.NGUI
{


	/// <summary>
	/// Ever wanted to be able to auto-center on an object within a draggable panel?
	/// Attach this script to the container that has the objects to center on as its children.
	/// </summary>


	[AddComponentMenu("eppz!/NGUI/Center Scroll View on Child")]
	public class EPPZNGUI_UICenterOnChild : UICenterOnChild
	{


		public List<Transform> children = new List<Transform>();


		void Start () { Recenter(); }
		void OnEnable () { if (mScrollView) { mScrollView.centerOnChild = this; Recenter(); } }
		void OnDisable () { if (mScrollView) mScrollView.centerOnChild = null; }
		void OnDragFinished () { if (enabled) Recenter(); }
		
		/// <summary>
		/// Ensure that the threshold is always positive.
		/// </summary>
		
		void OnValidate () { nextPageThreshold = Mathf.Abs(nextPageThreshold); }
		
		/// <summary>
		/// Recenter the draggable list on the center-most child.
		/// </summary>
		
		[ContextMenu("Execute")]
		new public void Recenter ()
		{ Recenter(true); }

		public void Recenter (bool useSpringPanel)
		{
			if (mScrollView == null)
			{
				mScrollView = NGUITools.FindInParents<UIScrollView>(gameObject);
				
				if (mScrollView == null)
				{
					Debug.LogWarning(GetType() + " requires " + typeof(UIScrollView) + " on a parent object in order to work", this);
					enabled = false;
					return;
				}
				else
				{
					if (mScrollView)
					{
						mScrollView.centerOnChild = this;
						mScrollView.onDragFinished += OnDragFinished;
					}
					
					if (mScrollView.horizontalScrollBar != null)
						mScrollView.horizontalScrollBar.onDragFinished += OnDragFinished;
					
					if (mScrollView.verticalScrollBar != null)
						mScrollView.verticalScrollBar.onDragFinished += OnDragFinished;
				}
			}
			if (mScrollView.panel == null) return;
			
			Transform trans = transform;
			if (children.Count == 0) return;
			
			// Calculate the panel's center in world coordinates
			Vector3[] corners = mScrollView.panel.worldCorners;
			Vector3 panelCenter = (corners[2] + corners[0]) * 0.5f;
			
			// Offset this value by the momentum
			Vector3 momentum = mScrollView.currentMomentum * mScrollView.momentumAmount;
			Vector3 moveDelta = NGUIMath.SpringDampen(ref momentum, 9f, 2f);
			Vector3 pickingPoint = panelCenter - moveDelta * 0.01f; // Magic number based on what "feels right"
			
			float min = float.MaxValue;
			Transform closest = null;
			
			// Determine the closest child
			// for (int i = 0, imax = trans.childCount, ii = 0; i < imax; ++i)
			foreach (Transform eachChildTransform in children)
			{
				Transform t = eachChildTransform; // trans.GetChild(i);
				if (!t.gameObject.activeInHierarchy) continue;
				float sqrDist = Vector3.SqrMagnitude(t.position - pickingPoint);
				
				if (sqrDist < min)
				{
					min = sqrDist;
					closest = t;
				}
			}
			
			// If we have a touch in progress and the next page threshold set
			if (nextPageThreshold > 0f && UICamera.currentTouch != null)
			{
				// If we're still on the same object
				if (mCenteredObject != null && mCenteredObject.transform == closest)
				{
					Vector3 totalDelta = UICamera.currentTouch.totalDelta;
					totalDelta = transform.rotation * totalDelta;
					
					float delta = 0f;
					
					switch (mScrollView.movement)
					{
					case UIScrollView.Movement.Horizontal:
					{
						delta = totalDelta.x;
						break;
					}
					case UIScrollView.Movement.Vertical:
					{
						delta = totalDelta.y;
						break;
					}
					default:
					{
						delta = totalDelta.magnitude;
						break;
					}
					}
					
					if (Mathf.Abs(delta) > nextPageThreshold)
					{
						int closestIndex = children.IndexOf(closest);

						if (delta > nextPageThreshold)
						{
							// Next page.
							if (closestIndex < children.Count - 1)
							{ closest = children[closestIndex + 1]; }
						}
						else if (delta < -nextPageThreshold)
						{
							// Previous page.
							if (closestIndex > 0)
							{ closest = children[closestIndex - 1]; }
						}
					}
				}
			}
			CenterOn(closest, panelCenter, useSpringPanel);
		}
		
		/// <summary>
		/// Center the panel on the specified target.
		/// </summary>
		
		void CenterOn (Transform target, Vector3 panelCenter, bool useSpringPanel)
		{
			if (target != null && mScrollView != null && mScrollView.panel != null)
			{
				Transform panelTrans = mScrollView.panel.cachedTransform;
				mCenteredObject = target.gameObject;
				
				// Figure out the difference between the chosen child and the panel's center in local coordinates
				Vector3 cp = panelTrans.InverseTransformPoint(target.position);
				Vector3 cc = panelTrans.InverseTransformPoint(panelCenter);
				Vector3 localOffset = cp - cc;
				
				// Offset shouldn't occur if blocked
				if (!mScrollView.canMoveHorizontally) localOffset.x = 0f;
				if (!mScrollView.canMoveVertically) localOffset.y = 0f;
				localOffset.z = 0f;
				
				// Spring the panel to this calculated position
#if UNITY_EDITOR
				if (!Application.isPlaying)
				{
					panelTrans.localPosition = panelTrans.localPosition - localOffset;
					
					Vector4 co = mScrollView.panel.clipOffset;
					co.x += localOffset.x;
					co.y += localOffset.y;
					mScrollView.panel.clipOffset = co;
				}
				else
#endif
				{
					if (useSpringPanel)
					{
						SpringPanel.Begin(
								mScrollView.panel.cachedGameObject,
								panelTrans.localPosition - localOffset,
								springStrength
								).onFinished = onFinished;
					}

					else
					{
						panelTrans.localPosition = panelTrans.localPosition - localOffset;
						
						Vector4 co = mScrollView.panel.clipOffset;
						co.x += localOffset.x;
						co.y += localOffset.y;
						mScrollView.panel.clipOffset = co;
					}
				}
			}
			else mCenteredObject = null;
			
			// Notify the listener
			if (onCenter != null) onCenter(mCenteredObject);
		}
		
		/// <summary>
		/// Center the panel on the specified target.
		/// </summary>

		new public void CenterOn (Transform target)
		{ CenterOn(target, true); }

		public void CenterOn (Transform target, bool useSpringPanel)
		{
			if (mScrollView != null && mScrollView.panel != null)
			{
				Vector3[] corners = mScrollView.panel.worldCorners;
				Vector3 panelCenter = (corners[2] + corners[0]) * 0.5f;
				CenterOn(target, panelCenter, useSpringPanel);
			}
		}
	}
}
