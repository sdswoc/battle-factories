﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuPanelSwitcher : MonoBehaviour 
{
	public RectTransform currentTransform;
	public AnimationCurve curve;
	public RectTransform[] panels;

	public float switchTime;
    public bool startTrigger;

    public void Start()
    {
        startTrigger = false;
    }
    private void Update()
    {
        if (!startTrigger && Input.GetMouseButtonDown(0))
        {
            startTrigger = true;
            StartCoroutine(SwitchCoroutine(0));
        }
    }
    public void Switch(int i)
	{
		StartCoroutine(SwitchCoroutine(i));
	}

	IEnumerator SwitchCoroutine(int t)
	{
	for (int i = 0;i < panels.Length;i++)
	{
	if (panels[i] != currentTransform)
			panels[i].gameObject.SetActive(false);
	}
		if (currentTransform != null)
		{
			currentTransform.gameObject.SetActive(true);
			for (float i = 0; i < switchTime; i += Time.deltaTime)
			{
				currentTransform.localScale = Vector3.one * (1 - curve.Evaluate(i / switchTime));
				yield return new WaitForEndOfFrame();
			}
			currentTransform.gameObject.SetActive(false);
		}
        currentTransform = panels[t % panels.Length];
        currentTransform.gameObject.SetActive(true);
        if (currentTransform != null)
		{

			for (float i = 0; i < switchTime; i += Time.deltaTime)
			{
				currentTransform.localScale = Vector3.one * (curve.Evaluate(i / switchTime));
				yield return new WaitForEndOfFrame();
			}
		}
	}
}
