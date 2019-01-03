using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Timer : MonoBehaviour
    {
        public string timerPopUpText;
        public RectTransform clockTransform;
        public Image centralCircle;
        public float dangerTime;
        public float shakeAmplitude;
        public float popTime;
        public AnimationCurve openCurve;
        public AnimationCurve closeCurve;
        private void Awake()
        {
            GameFlow.timer = this;
            gameObject.SetActive(false);
        }
        public void StartTimer(float time)
        {
            GameFlow.uiTutorialText.Pop(timerPopUpText);
            gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(Coroutine(time));
        }
        public void CloseTime()
        {
            if (gameObject.activeInHierarchy)
            {
                StopAllCoroutines();
                StartCoroutine(CloseTimer());
            }
        }
        IEnumerator Coroutine(float time)
        {
            gameObject.SetActive(true);
            centralCircle.fillAmount = 1;
            for (float i = 0; i < popTime; i += Time.deltaTime)
            {
                clockTransform.localScale = Vector3.one * openCurve.Evaluate(i / popTime);
                yield return new WaitForEndOfFrame();
            }
            clockTransform.localScale = Vector3.one;
            for (float i = 0; i < time - dangerTime; i += Time.deltaTime)
            {
                centralCircle.fillAmount = 1 - (i / time);
                yield return new WaitForEndOfFrame();
            }
            int prev = -1;
            for (float i = 0; i < dangerTime; i += Time.deltaTime)
            {
                if (i > prev)
                {
                    GetComponent<AudioSource>().Play();
                    prev = Mathf.CeilToInt(i);
                }
                centralCircle.fillAmount = 1 - ((i + Mathf.Max(0, time - dangerTime)) / time);
                clockTransform.eulerAngles = new Vector3(0, 0, Mathf.Sin(i * Mathf.PI * 20) * Mathf.Lerp(0, shakeAmplitude, i / dangerTime));
                yield return new WaitForEndOfFrame();
            }
            GetComponent<AudioSource>().Play();
            clockTransform.eulerAngles = Vector3.zero;
            for (float i = 0; i < popTime; i += Time.deltaTime)
            {
                clockTransform.localScale = Vector3.one * closeCurve.Evaluate(1 - i / popTime);
                yield return new WaitForEndOfFrame();
            }
            gameObject.SetActive(false);
            yield return new WaitForEndOfFrame();
        }
        IEnumerator CloseTimer()
        {
            clockTransform.eulerAngles = Vector3.zero;
            for (float i = 0; i < popTime; i += Time.deltaTime)
            {
                clockTransform.localScale = Vector3.one * closeCurve.Evaluate(1 - i / popTime);
                yield return new WaitForEndOfFrame();
            }
            GetComponent<AudioSource>().Stop();
            gameObject.SetActive(false);

        }
    }
}