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
	public class EPPZUI_CellSizeConstraint : MonoBehaviour
	{


		// To be used (tested) with stretch anchors (both direction).


		private GridLayoutGroup _gridLayout;
		private GridLayoutGroup gridLayout
		{
			get
			{
				if (_gridLayout == null) _gridLayout = GetComponent<GridLayoutGroup>();
				return _gridLayout;
			}
		}

		public RectTransform sizeReferenceRectTransform;
		public enum Constraint { Width, Height, Both }
		public Constraint constraint = Constraint.Width;
		public float multiplier = 1.0f;

		void Update()
		{
			Layout();
		}

		public void Layout()
		{
			if (gridLayout == null) return; // Only if any
			if (sizeReferenceRectTransform == null) return; // Only if any

			switch (constraint)
			{
				case Constraint.Width: gridLayout.cellSize = new Vector2(Mathf.Floor(sizeReferenceRectTransform.rect.width * multiplier), gridLayout.cellSize.y); break;
				case Constraint.Height: gridLayout.cellSize = new Vector2(gridLayout.cellSize.x, Mathf.Floor(sizeReferenceRectTransform.rect.height * multiplier)); break;
				case Constraint.Both: gridLayout.cellSize = new Vector2(Mathf.Floor(sizeReferenceRectTransform.rect.width * multiplier), Mathf.Floor(sizeReferenceRectTransform.rect.height * multiplier)); break;
			}
		}
	}
}
