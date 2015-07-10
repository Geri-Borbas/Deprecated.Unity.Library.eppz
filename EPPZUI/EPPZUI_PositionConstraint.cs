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
	public class EPPZUI_PositionConstraint : UIBehaviour
	{


		// Simply binds `Transform.position`.


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
		public enum Constraint { X, Y, Both }
		public Constraint constraint = Constraint.Both;

		void LateUpdate()
		{
			Layout();
		}

		public void Layout()
		{
			if (rectTransform == null) return; // Only if any
			if (referenceRectTransform == null) return; // Only if any

			switch (constraint)
			{
				case Constraint.X: _rectTransform.position = new Vector3(referenceRectTransform.position.x, _rectTransform.position.y, _rectTransform.position.z); break;
				case Constraint.Y: _rectTransform.position = new Vector3(_rectTransform.position.x, referenceRectTransform.position.y, _rectTransform.position.z); break;
				case Constraint.Both: _rectTransform.position = referenceRectTransform.position; break;
			}
		}
	}
}
