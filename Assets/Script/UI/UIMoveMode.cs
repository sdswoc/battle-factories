using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class UIMoveMode : MonoBehaviour
	{
		public Text timerText;
		private bool finish;

		private void Awake()
		{
			GameFlow.uiMoveMode = this;
		}
		public void SetDeadline(float time)
		{
			finish = false;
			StartCoroutine(Timer(time));
		}
		IEnumerator Timer(float time)
		{
			for (float i = 0; i < time; i += Time.deltaTime)
			{
				timerText.text = Mathf.RoundToInt(time - i).ToString();
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
			StopAllCoroutines();
			EventHandle.FinishTurnLocal();
		}
	}
}