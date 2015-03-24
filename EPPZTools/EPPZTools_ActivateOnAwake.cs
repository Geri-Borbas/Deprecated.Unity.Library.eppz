using UnityEngine;
using System.Collections;


namespace EPPZ
{


	public class EPPZTools_ActivateOnAwake : MonoBehaviour
	{


		public GameObject[] targets;


		void Awake()
		{
			foreach (GameObject eachTarget in targets)
			{
				if (eachTarget == null) continue;
				eachTarget.SetActive(true);
			}
		}
	}
}
