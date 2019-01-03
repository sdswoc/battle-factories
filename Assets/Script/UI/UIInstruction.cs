using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInstruction : MonoBehaviour 
{
    public int index;
    public int maxTutorial;
    public RectTransform container;
    public float switchTime;
    public AnimationCurve curve;

    private RectTransform[] tutorials;

    private void Awake()
    {
        maxTutorial = container.childCount;
        tutorials = new RectTransform[maxTutorial];
        for (int i = 0;i < maxTutorial;i++)
        {
            tutorials[i] = (RectTransform)container.GetChild(i);
        }
    }
    public void NextButtonCallback()
    {
        int next = (index + 1) % maxTutorial;
        StopAllCoroutines();
        StartCoroutine(Translate(index, next));
    }
    public void PreviousButtonCallback()
    {
        int next = (index - 1 + maxTutorial) % maxTutorial;
        StartCoroutine(Translate(index, next));
    }
    private IEnumerator Translate(int initial,int final)
    {
        for (int i = 0;i < maxTutorial;i++)
        {
            tutorials[i].gameObject.SetActive(i == initial);
        }
        for (float i = 0;i < switchTime;i  += Time.deltaTime)
        {
            tutorials[initial].localScale = Vector3.one * curve.Evaluate(1-i / switchTime);
            yield return new WaitForEndOfFrame();
        }
        tutorials[initial].gameObject.SetActive(false);
        tutorials[final].gameObject.SetActive(true);
        for (float i = 0; i < switchTime; i += Time.deltaTime)
        {
            tutorials[final].localScale = Vector3.one * curve.Evaluate(i / switchTime);
            yield return new WaitForEndOfFrame();
        }
        tutorials[final].localScale = Vector3.one;
        index = final;
    }
}
