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
	public class EPPZUI_SizeConstraint : MonoBehaviour
	{


		// To be used (tested) with stretch anchors (both direction).


		private RectTransform _rectTransform;
		private RectTransform rectTransform
		{
			get
			{
				if (_rectTransform == null) _rectTransform = transform as RectTransform;
				return _rectTransform;
			}
		}

		public RectTransform referenceRectTransform;
		public enum Constraint { Width, Height, Both }
		public Constraint constraint = Constraint.Width;
		public float multiplier = 1.0f;

		void Update()
		{
			Layout();
		}

		public void Layout()
		{
			if (rectTransform == null) return; // Only if any
			if (referenceRectTransform == null) return; // Only if any

			switch (constraint)
			{
				case Constraint.Width: _rectTransform.sizeDelta = new Vector2(referenceRectTransform.rect.width * multiplier, _rectTransform.sizeDelta.y); break;
				case Constraint.Height: _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, referenceRectTransform.rect.height * multiplier); break;
				case Constraint.Both: _rectTransform.sizeDelta = new Vector2(referenceRectTransform.rect.width * multiplier, referenceRectTransform.rect.height * multiplier); break;
			}
		}
	}
}
