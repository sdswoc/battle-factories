using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace UI
{
	public class UIWaitMode : MonoBehaviour
	{
		public Text text;

		private void Awake()
		{
			GameFlow.uiWaitMode = this;
		}
		public void Wait(float time)
		{
			StartCoroutine(Timer(time));
		}
		private IEnumerator Timer(float time)
		{
			text.text = time.ToString();
			for (float i = 0; i < time; i += Time.deltaTime)
			{
				text.text = Mathf.RoundToInt(time - i).ToString();
				yield return new WaitForEndOfFrame();
			}
			text.text = "Wait";
		}
	}
}