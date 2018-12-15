using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICurtain : MonoBehaviour 
{
	public float transitionTime;
	public AnimationCurve curve;
	public void Awake()
	{
		GameFlow.uiCurtain = this;
		StartCoroutine(OpenCoroutine());
	}
	public	IEnumerator Open()
	{
		gameObject.SetActive(true);
		yield return StartCoroutine(OpenCoroutine());
	}
	public IEnumerator Close()
	{
		gameObject.SetActive(true);
		yield return StartCoroutine(CloseCoroutine());
	}
	IEnumerator CloseCoroutine()
	{
		RectTransform transform = GetComponent<RectTransform>();
		transform.anchorMax = Vector2.one + Vector2.up;
		transform.anchorMin = Vector2.up;
		yield return new WaitForEndOfFrame();
		for (float i = 0;i < transitionTime;i+= Time.deltaTime)
		{
			transform.anchorMax = Vector2.right + Vector2.up*(2-curve.Evaluate(i/transitionTime));
			transform.anchorMin = Vector2.up* (1 - curve.Evaluate(i / transitionTime));
			yield return new WaitForEndOfFrame();
		}
		transform.anchorMax = Vector2.right + Vector2.up;
		transform.anchorMin = Vector2.up * (0);
	}
	IEnumerator OpenCoroutine()
	{
		RectTransform transform = GetComponent<RectTransform>();
		transform.anchorMax = Vector2.one;
		transform.anchorMin = Vector2.zero;
		yield return new WaitForEndOfFrame();
		for (float i = 0; i < transitionTime; i += Time.deltaTime)
		{
			transform.anchorMax = Vector2.right + Vector2.up * (1+ curve.Evaluate(i / transitionTime));
			transform.anchorMin = Vector2.up * (curve.Evaluate(i / transitionTime));
			yield return new WaitForEndOfFrame();
		}
		transform.anchorMax = Vector2.right + Vector2.up*2;
		transform.anchorMin = Vector2.up * (1);
		gameObject.SetActive(false);
	}
}
