using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace EPPZ
{


	public class EPPZTools_RetinaCanvas : MonoBehaviour
	{


		void Awake()
		{
			CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
			if (Screen.dpi >= 260.0f)
			{
				canvasScaler.scaleFactor = 2.0f;
			}
		}
	}
}
