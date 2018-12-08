using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UI
{
	public class UIFireMode : MonoBehaviour
	{
		public bool myTurn;
		private void Awake()
		{
			GameFlow.uiFireMode = this;
		}
	}
}