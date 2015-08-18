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


		private List<UISprite> sprites = new List<UISprite>();
		private UISprite selectedSprite;
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
			UISprite spritePrototype = spritePrototypeObject.GetComponent<UISprite>();
			if (spritePrototype == null) return; // Only having sprite prototype

			// Flush previous.
			foreach (UISprite eachSprite in sprites) { Destroy(eachSprite.gameObject); }
			sprites.Clear();

			// Activate prototype.
			spritePrototypeObject.SetActive(true);

			// Instantiate clones.
			for (int index = 0; index < pageCount; index++)
			{
				GameObject eachSpriteObject = Instantiate(spritePrototypeObject) as GameObject;
				eachSpriteObject.transform.SetParent(spritePrototypeObject.transform.parent, false); // UI-proof parenting
				eachSpriteObject.name = spritePrototypeObject.name + " " + (index + 1); // Proper name

				UISprite eachSprite = eachSpriteObject.GetComponent<UISprite>();
				if (eachSprite == null) continue;

				eachSprite.depth = spritePrototype.depth + index + 1; // Depth
				sprites.Add(eachSprite); // Collect
			}
			
			// Layout grid.
			spriteGrid.cellWidth = spritePrototype.width * (spriteSpacing + 1.0f);
			spritePrototypeObject.SetActive(false); // Hide prototype
			spriteGrid.Reposition();
		}

		public void LayoutPageIndex(int pageIndex)
		{
			// Some index validating.
			if (pageIndex < 0) return;
			if (pageIndex > (sprites.Count - 1)) return;

			// Switch sprite colors.
			if (selectedSprite != null) selectedSprite.color = normalColor;
			selectedSprite = sprites[pageIndex];
			selectedSprite.color = selectedColor;
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