using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class UIMoveMode : MonoBehaviour
	{
		public Text timerText;
		private bool clicked;

		private void Awake()
		{
			GameFlow.uiMoveMode = this;
		}
		public void SetDeadline(float time)
		{
			clicked = false;
			StartCoroutine(Timer(time));
		}
		IEnumerator Timer(float time)
		{
			int prevTime = -1;
			for (float i = 0; i < time; i += Time.deltaTime)
			{
				int t = Mathf.RoundToInt(time - i);
				if (t != prevTime)
				{
					prevTime = t;
					int ones = t % 10;
					t /= 10;
					int tens = t;
					timerText.text = tens.ToString()+ones.ToString();
				}
				yield return new WaitForEndOfFrame();
			}
			OnFinishButtonClick();
		}
		public void OnSpawnButton(int i)
		{
			EventHandle.CreateFriendlyUnit((byte)i);
		}
		public void OnFinishButtonClick()
		{
			if (!clicked)
			{
				StopAllCoroutines();
				EventHandle.FinishTurnLocal();
				GameFlow.uiSpawnUnit.Close();
				clicked = true;
			}
		}
	}
}