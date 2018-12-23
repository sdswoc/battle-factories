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
        public AudioSource tickSound;
		public Text timerText;
		public RectTransform buttonTransform;
		public RectTransform timerTextTransform;
		public RectTransform tickImageTransform;
		public AnimationCurve transitionCurve;
        public string popMessageHurry;
		private bool clicked = false;
		private void Awake()
		{
			buttonTransform.offsetMin = new Vector2(buttonTransform.offsetMax.x - buttonTransform.rect.height, buttonTransform.offsetMin.y);
			GameFlow.uiSetupMode = this;
			ButtonChangeTranslation(0);
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
            GameFlow.uiTutorialText.Pop(popMessageHurry);
			StopAllCoroutines();
			StartCoroutine(ButtonTransition());
			StartCoroutine(Timer(GameFlow.FACTORY_SETUP_TIMELIMIT));
		}
        IEnumerator Timer(float time)
        {
            int prevTimerValue = -1;
            int prevI = Mathf.CeilToInt(5);
            for (float i = time; i >= 0; i -= Time.deltaTime)
            {
                if (i < prevI)
                {
                    prevI = Mathf.FloorToInt(i);
                    tickSound.Play();
                }
                int tt = Mathf.RoundToInt(i);
                if (tt != prevTimerValue)
                {
                    prevTimerValue = tt;
                    int ones = tt % 10;
                    tt /= 10;
                    int tens = tt;
                    timerText.text = tens.ToString() + ones.ToString();
                }
                yield return new WaitForEndOfFrame();
            }
            tickSound.Play();
            OnOKButton();
        }
		IEnumerator ButtonTransition()
		{
			for (float i = 0;i < 1;i += Time.deltaTime)
			{
				ButtonChangeTranslation(transitionCurve.Evaluate( i));
				yield return new WaitForEndOfFrame();
			}
		}
		public void ButtonChangeTranslation(float t)
		{
			buttonTransform.offsetMin = new Vector2(buttonTransform.offsetMax.x - buttonTransform.rect.height*(1+t), buttonTransform.offsetMin.y);
			tickImageTransform.anchorMax = Vector2.Lerp(Vector2.one, new Vector2(0.5f, 1), t);
			timerTextTransform.localScale = Vector3.one * t;
		}
	}
}