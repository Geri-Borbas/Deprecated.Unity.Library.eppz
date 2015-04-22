//
//  EPPZEditor_SpriteSliceRenamer.cs
//  Sprite sheet slice renamer tool
//
//  Created by Borbás Geri on 22/04/15
//  Copyright (c) 2015 eppz! development, LLC.
//
//  follow http://www.twitter.com/_eppz
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//


#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


namespace EPPZ.Editor
{


	public class EPPZEditor_SliceRenamer : EditorWindow
	{


		// Properties.
		public Texture2D texture;
		public string format = "slice_{0:00}";
		[System.Serializable] public class Model : ScriptableObject
		{
			[System.Serializable] public class Argument
			{
				public int start = 0;
				public int incrementEveryNth = 1;
				public int resetEveryNth = 0;

				public int valueForIndex(int index)
				{
					int value = 0;

					// Increment.
					if (incrementEveryNth == 0)
					{ value = start + index; } // Simply increment
					else
					{ value = start + (index / incrementEveryNth); } // Increment every Nth only

					// Reset (if requested).
					if (resetEveryNth > 0)
					{ value = value % resetEveryNth; }

					return value;
				}
			}

			public List<Argument> arguments = new List<Argument>();
		}
		public static Model model;

		// GUI.
		private static SerializedObject serializedModel;
		private Vector2 scroll;
		private string status = "";

		// Asset.
		private string path;
		private TextureImporter textureImporter;


		// Window.
		[MenuItem("Window/eppz!/Slice Renamer")]
		public static void ShowWindow()
		{
			// Show window.
			EditorWindow.GetWindow(typeof(EPPZEditor_SliceRenamer), false,  "Slice Renamer");

			// Model setup.
			model = new Model();
			model.arguments.Add(new Model.Argument()); // Default with a single incrementing argument
			serializedModel = new SerializedObject(model);
		}


		// GUI.
		void OnGUI()
		{
			// Texture asset.
			string textureName = (texture == null) ? "No Texture selected" : texture.name;
			EditorGUILayout.LabelField(textureName, EditorStyles.boldLabel);
			texture = EditorGUILayout.ObjectField("Texture", texture, typeof(Texture2D), true) as Texture2D;

			// Format string.
			format = EditorGUILayout.TextField("Format", format);

			// Arguments array.
			EditorGUILayout.PropertyField(serializedModel.FindProperty("arguments"), new GUIContent("Arguments"), true);
			serializedModel.ApplyModifiedProperties();

			if (GUILayout.Button("Preview"))
			{ Apply(true); }

			if (GUILayout.Button("Rename slices"))
			{ Apply(false); }

			// Status.
			scroll = EditorGUILayout.BeginScrollView(scroll);
			EditorGUILayout.LabelField(status, EditorStyles.helpBox);
			EditorGUILayout.EndScrollView();
		}

		// Apply settings.
		void Apply(bool preview)
		{
			// Error.
			if (texture == null)
			{
				status = "Drag a texture into the slot.";
				return;
			}

			// Locate asset, get meta.
			path = AssetDatabase.GetAssetPath(texture);
			textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
			SpriteMetaData[] sliceMetaData = textureImporter.spritesheet;

			// Error.
			if (sliceMetaData == null || sliceMetaData.Length == 0)
			{
				status = "Seems no slices defined in texture.";
				return;
			}

			// Naming loop.
			int index = 0;
			status = "";
			if (model.arguments.Count > 3) { status += "Exceeded argument limit of 3. Only first 3 arguments will be used.\n\n"; }
			foreach (SpriteMetaData eachSliceMetaData in sliceMetaData)
			{
				string eachName = "";
				
				// Create string.
				switch (model.arguments.Count)
				{
				case 0: eachName = format; break;
				case 1: eachName = string.Format(format, model.arguments[0].valueForIndex(index)); break;
				case 2: eachName = string.Format(format, model.arguments[0].valueForIndex(index), model.arguments[1].valueForIndex(index)); break;
				case 3: eachName = string.Format(format, model.arguments[0].valueForIndex(index), model.arguments[1].valueForIndex(index), model.arguments[2].valueForIndex(index)); break;
				}
				
				// Assemble name.
				string verb = (preview) ? "Rename" : "Renamed";
				if (index > 0) status += "\n";
				status += verb+" `"+eachSliceMetaData.name+"` to `"+eachName+"`.";
				
				// Assign.
				if (preview == false)
				{ sliceMetaData[index].name = eachName; }
				
				index++;
			}

			// Branding.
			status += "\n\nBrought to you by @_eppz";

			// Apply.
			if (preview == false)
			{
				// Save settings.
				textureImporter.spritesheet = sliceMetaData;
				EditorUtility.SetDirty(textureImporter);
				textureImporter.SaveAndReimport();
				
				// Reimport asset.
				AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
			}
		}
	}
}
#endif