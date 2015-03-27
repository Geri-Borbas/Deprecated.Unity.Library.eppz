using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using EPPZGeometry;


namespace EPPZ.UI
{
	
	
	[ExecuteInEditMode]
	public class EPPZUI_AspectConstraint : MonoBehaviour
	{


		// To be used (tested) with center anchors (both direction).

		
		private RectTransform _rectTransform;
		private RectTransform rectTransform
		{
			get
			{
				if (_rectTransform == null) _rectTransform = transform as RectTransform;
				return _rectTransform;
			}
		}

		public enum Aspect { Any, Tall, Wide }
		public enum Mode { Fit, Fill }
		public Mode mode = Mode.Fit;

		[System.Serializable]
		public class Constraint
		{
			public Aspect parentAspect;
			public RectTransform aspectReferenceRectTransform;
		}
		public Constraint[] constraints;
		
		void Update()
		{
			Layout();
		}
		
		public void Layout()
		{
			// Checks.
			if (rectTransform == null) return; // Only if any
			RectTransform parentRectTransform = transform.parent as RectTransform;
			if (parentRectTransform == null) return; // Only if any

			// Parent aspect.
			float parentAspectRatio = parentRectTransform.rect.width / parentRectTransform.rect.height;
			Aspect parentAspect = (parentAspectRatio > 1.0f) ? Aspect.Wide : Aspect.Tall;

			// Constraint rules.
			foreach (Constraint eachConstraint in constraints)
			{
				// Apply if match.
				if (parentAspect == eachConstraint.parentAspect ||
				    eachConstraint.parentAspect == Aspect.Any)
				{
					RectTransform referenceRectTransform = eachConstraint.aspectReferenceRectTransform;
					if (referenceRectTransform == null) continue; // Only if any

					// Get aspects.
					float aspect = referenceRectTransform.rect.width / referenceRectTransform.rect.height;
					bool isWiderThanParent = aspect > parentAspectRatio;

					float width = 0.0f;
					float height = 0.0f;
					switch (mode)
					{
						case Mode.Fit:
						
						if (isWiderThanParent)
						{
							width = parentRectTransform.rect.width;
							height = Mathf.Floor(width / aspect);
						}
						
						else
						{
							height = parentRectTransform.rect.height;
							width = Mathf.Floor(height * aspect);
						}
						
						break;
						case Mode.Fill:
						
						if (isWiderThanParent)
						{
							height = parentRectTransform.rect.height;
							width = height * aspect;
						}
						
						else
						{
							width = parentRectTransform.rect.width;
							height = width / aspect;
						}
						
						break;
					}
					
					// Apply.
					_rectTransform.sizeDelta = new Vector2(width, height);
				}
			}
		}
	}
}
