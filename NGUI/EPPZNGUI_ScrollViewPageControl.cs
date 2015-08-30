using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace EPPZ.NGUI
{


	public class EPPZNGUI_ScrollViewPageControl : MonoBehaviour
	{


		public EPPZNGUI_ScrollViewPaging scrollViewPaging;
		public GameObject spritePrototypeObject;
		public float spriteSpacing = 1.0f; // Percent of sprite width

		// Tint Colors
		public Color normalColor;
		public Color selectedColor;


		private List<UIWidget> spriteWidgets = new List<UIWidget>();
		private UIWidget selectedSpriteWidget;
		private UIGrid spriteGrid;


		void Start()
		{
			// References.
			spriteGrid = GetComponentInChildren<UIGrid>();

			// Reverse hook.
			if (scrollViewPaging != null)
			{
				scrollViewPaging.pageControl = this;
				scrollViewPaging.LayoutPageControl();
			}
		}


		#region Layout (ScrollView events)

		public void LayoutPageCount(int pageCount)
		{
			if (Application.isPlaying == false) return; // Only in play mode
			if (spritePrototypeObject == null) return; // Only having sprite prototype object
			UIWidget spriteWidgetPrototype = spritePrototypeObject.GetComponent<UIWidget>();
			if (spriteWidgetPrototype == null) return; // Only having sprite prototype

			// Flush previous.
			foreach (UIWidget eachSpriteWidget in spriteWidgets) { Destroy(eachSpriteWidget.gameObject); }
			spriteWidgets.Clear();

			// Activate prototype.
			spritePrototypeObject.SetActive(true);

			// Instantiate clones.
			for (int index = 0; index < pageCount; index++)
			{
				GameObject eachSpriteObject = Instantiate(spritePrototypeObject) as GameObject;
				eachSpriteObject.transform.SetParent(spritePrototypeObject.transform.parent, false); // UI-proof parenting
				eachSpriteObject.name = spritePrototypeObject.name + " " + (index + 1); // Proper name

				UIWidget eachSpriteWidget = eachSpriteObject.GetComponent<UIWidget>();
				if (eachSpriteWidget == null) continue;

				eachSpriteWidget.depth = spriteWidgetPrototype.depth + index + 1; // Depth
				spriteWidgets.Add(eachSpriteWidget); // Collect
			}
			
			// Layout grid.
			spriteGrid.cellWidth = spriteWidgetPrototype.width * (spriteSpacing + 1.0f);
			spritePrototypeObject.SetActive(false); // Hide prototype
			spriteGrid.enabled = true; // Invoke `Reposition()` within
		}

		public void LayoutPageIndex(int pageIndex)
		{
			// Some index validating.
			if (pageIndex < 0) return;
			if (pageIndex > (spriteWidgets.Count - 1)) return;

			// Switch sprite colors.
			if (selectedSpriteWidget != null) selectedSpriteWidget.color = normalColor;
			selectedSpriteWidget = spriteWidgets[pageIndex];
			selectedSpriteWidget.color = selectedColor;
		}

		#endregion


		#region Events

		public void OnPreviousPress()
		{ scrollViewPaging.OnPageControlPreviousPress(); }
		
		public void OnNextPress()
		{ scrollViewPaging.OnPageControlNextPress(); }

		#endregion
	}
}