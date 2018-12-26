using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITutorialText : MonoBehaviour 
{
    public Text text;
    public AnimationCurve popUpCurve;
    public float popUpTime;
    public float displayTime;
    private bool displaying;
    private void Awake()
    {
        GameFlow.uiTutorialText = this;
    }
    public void Pop(string message)
    {
        gameObject.SetActive(true);
        StartCoroutine(Coroutine(message));
    }
    private IEnumerator Coroutine(string message)
    {
        Transform t = GetComponent<Transform>();
        if (displaying)
        {
            for (float i = 0; i < popUpTime; i += Time.deltaTime)
            {
                t.localScale = Vector3.one * popUpCurve.Evaluate(1 - (i / popUpTime));
                yield return new WaitForEndOfFrame();
            }
        }
        text.text = message;
        for (float i = 0; i < popUpTime; i += Time.deltaTime)
        {
            t.localScale = Vector3.one * popUpCurve.Evaluate(i / popUpTime);
            yield return new WaitForEndOfFrame();
        }
        displaying = true;
        yield return new WaitForSeconds(displayTime);
        for (float i = 0; i < popUpTime; i += Time.deltaTime)
        {
            t.localScale = Vector3.one * popUpCurve.Evaluate(1 - (i / popUpTime));
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
        displaying = false;
    }
}
