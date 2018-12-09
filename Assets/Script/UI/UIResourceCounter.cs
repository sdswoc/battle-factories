using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace UI
{
	public class UIResourceCounter : MonoBehaviour
	{
		public Text text;

		private void Awake()
		{
			GameFlow.uIResourceCounter = this;
		}
		public void StateUpdate()
		{
			text.text = "RS" + GameFlow.money.ToString() + " FU" + GameFlow.fuel.ToString();
		}
	}
}