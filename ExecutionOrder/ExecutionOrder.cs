using System;
using System.Collections.Generic;
using UnityEngine;


namespace EPPZ.ExecutionOrder
{


	public class ExecutionOrder : Attribute
	{


		public int executionOrder;


		public ExecutionOrder(int executionOrder)
		{
			this.executionOrder = executionOrder;
		}
	}
}