using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


namespace EPPZ.Utils
{


	public class EPPZUtils_FPSLabel : MonoBehaviour
	{


		public float updateInterval = 1.0f; // Seconds
		private Text label = null;
		private List<float> fpsWindow = new List<float>();
		private List<float> fixedFpsWindow = new List<float>();

		void Awake()
		{
			label = this.GetComponent<Text>();
			StartCoroutine("Tick");
		}

		void Update()
		{
			float currentFps = 1.0f / Time.deltaTime;
			float currentFixedFps = 1.0f / Time.fixedDeltaTime;

			fpsWindow.Add(currentFps);
			fixedFpsWindow.Add(currentFixedFps);
		}
		
		IEnumerator Tick()
		{
			while (true)
			{
				float fps = AverageOfFloatList(fpsWindow);
				float fixedFps = AverageOfFloatList(fixedFpsWindow);
				label.text = string.Format("Frame (Average FPS) {0:##.##} \nFixed (Average FPS) {1:##.##}", fps, fixedFps);

				fpsWindow.Clear();
				fixedFpsWindow.Clear();

				yield return new WaitForSeconds(updateInterval);
			}
		}

		float AverageOfFloatList(List<float> list)
		{
			float sum = 0;
			for(int index = 0; index < list.Count; index++)
			{
				sum += list[index];
			}
			return sum / (float)list.Count;
		}
	}
}
