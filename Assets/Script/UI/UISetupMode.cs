using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Control;
using UnityEngine.UI;
using System;
namespace UI
{
	public class UISetupMode : MonoBehaviour
	{
		public Text timerText;
		private bool clicked = false;
		private void Awake()
		{
			GameFlow.uiSetupMode = this;
		}
		public void OnOKButton()
		{
			if (!clicked)
			{
				GameFlow.setupFactory.OnOKButton();
				clicked = true;
			}
		}
		public void SetDeadLine()
		{
			StopAllCoroutines();
			StartCoroutine(Timer(GameFlow.FACTORY_SETUP_TIMELIMIT));
		}
		IEnumerator Timer(float time)
		{
			for (float i = 0;i < time;i += Time.deltaTime)
			{
				timerText.text = Mathf.RoundToInt(time - i).ToString();
				yield return new WaitForEndOfFrame();
			}
			OnOKButton();
		}
	}
}