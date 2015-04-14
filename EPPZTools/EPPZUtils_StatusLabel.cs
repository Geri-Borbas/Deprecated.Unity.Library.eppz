using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace EPPZ.Utils
{


	public class EPPZUtils_StatusLabel : MonoBehaviour
	{


		public static Text label = null;
		public static void Log(string message)
		{
			if (label == null) return;
			label.text = message;
		}

		void Awake()
		{
			label = this.GetComponent<Text>();
		}
	}

	public class Status : EPPZUtils_StatusLabel { }
}