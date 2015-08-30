using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace EPPZ.Utils
{


	public class EPPZUtils_StatusLabel : MonoBehaviour
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

	public class Status : EPPZUtils_StatusLabel { }
}