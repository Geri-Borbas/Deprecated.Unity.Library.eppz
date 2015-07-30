using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace EPPZ.NGUI
{
	
	
	/// <summary>
	/// tangram! UI constraints.
	/// </summary>
	
	
	[ExecuteInEditMode]
	public class TMenuConstraints : EPPZNGUI_Constraint
	{

		
		// UI.
		private float aspect;
		private bool portrait;
		private bool landscape;

		public UIWidget square;
		public EPPZNGUI_AnchorSizeConstraint paddingSizeConstraint;
		public EPPZNGUI_AnchorSizeConstraint areaConstraint;
		public UIWidget centerPanel;
		public UIWidget bottomPanel;
		public UIGrid centerGrid;

		// Tracking changes to spare layout calculations.
		private int _previousWidgetWidth;
		private int _previousWidgetHeight;


		protected override bool ShouldLayout()
		{ return true; } // Layout even without target widget

		void OnEnable()
		{ Execute(); }

		[ContextMenu ("Execute")]
		void Execute()
		{
			RemoveListeners();
			AddListeners();
			Adjust();
		}

		protected override bool HasChanged()
		{
			// Adjust only if something (probably screen size) has changed.
			bool widgetChanged = (
				(widget.width != _previousWidgetWidth) ||
				(widget.height != _previousWidgetHeight)
				);
			return widgetChanged;
		}

		public override void Layout()
		{
			Adjust();
			
			// Track changes.
			_previousWidgetWidth = widget.width;
			_previousWidgetHeight = widget.height;
		}
		
		void Adjust()
		{
			// Calculate new size.
			aspect = (float)widget.width / (float)widget.height; // Calculate aspect on the fly (!)

			// Shortcuts.
			landscape = (aspect > 1.0f);
			portrait = !landscape;

			// Layout order.
			LayoutSquare();
			LayoutPaddingSize();
			paddingSizeConstraint.Layout();
			areaConstraint.Layout();
			LayoutPanel();
			LayoutGrid();
		}

		void LayoutSquare()
		{
			if (portrait)
			{
				// Aspect fit square.
				square.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnWidth;
				square.aspectRatio = 1.0f;
				square.width = widget.width;
			}
			
			if (landscape)
			{
				// Aspect fit square.
				square.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnHeight;
				square.aspectRatio = 1.0f;
				square.height = widget.height;
			}
		}

		void LayoutPaddingSize()
		{
			float portrait_9_16 = 9.0f / 16.0f;
			float landscape_16_9 = 16.0f / 9.0f;

			// Portrait 16:9 (iPhone 5).
			if (aspect < portrait_9_16)
			{ paddingSizeConstraint.multiplier = 0.25f; }

			// Portrait.
			else if (aspect < 1.0f)
			{ paddingSizeConstraint.multiplier = 0.5f; }

			// Landscape.
			else if (aspect < landscape_16_9)
			{ paddingSizeConstraint.multiplier = 0.5f; }

			// Landscape 16:9 (iPhone 5).
			// else
			// { paddingSizeConstraint.multiplier = 0.25f; }
		}

		void LayoutPanel()
		{
			float portrait_3_4 = 3.0f / 4.0f;
			float landscape_4_3 = 4.0f / 3.0f;

			if (portrait)
			{
				// Aspect fit panel.
				centerPanel.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnWidth;
				centerPanel.aspectRatio = portrait_3_4;
				centerPanel.width = areaConstraint.widget.width;
			}
			
			if (landscape)
			{
				// Aspect fit panel.
				centerPanel.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnHeight;
				centerPanel.aspectRatio = landscape_4_3;
				centerPanel.height = areaConstraint.widget.height;
			}
		}

		void LayoutGrid()
		{
			float cellSize = 0.0f;

			if (portrait)
			{
				cellSize = centerPanel.height / 4.0f;
				centerGrid.maxPerLine = 3;
			}
			
			if (landscape)
			{
				cellSize = centerPanel.width / 4.0f;
				centerGrid.maxPerLine = 4;
			}

			// Adjust cells.
			centerGrid.cellWidth = cellSize;
			centerGrid.cellHeight = cellSize;

			// Adjust children size.
			List<Transform> childList = centerGrid.GetChildList();
			foreach (Transform eachChildTransfrom in childList)
			{
				if (eachChildTransfrom == null) continue;
				UIWidget eachChildWidget = eachChildTransfrom.gameObject.GetComponent<UIWidget>();
				if (eachChildWidget == null) continue;
				eachChildWidget.height = Mathf.FloorToInt(cellSize + 0.5f);
			}

			// Align.
			centerGrid.Reposition(); // TODO: Subclass UIGrid to cache children after populated
		}
	}
}
