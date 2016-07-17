using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace EPPZ.Utils
{


	public class StatusLabel : MonoBehaviour
	{


		public static UILabel label = null;
		public static void Log(string message)
		{
			if (label == null) return;
			label.text = message;
		}

		void Awake()
		{
			label = this.GetComponent<UILabel>();
		}
	}

	public class Status : StatusLabel { }
}