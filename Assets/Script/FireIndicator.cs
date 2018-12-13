using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireIndicator : MonoBehaviour
{
	public RectTransform fireIconTransform;
	public float rotationSpeed;
	public float popTime;
	public AnimationCurve openCurve;
	public AnimationCurve closeCurve;
	private bool opened;
	private void Awake()
	{
		GameFlow.fireIndicator = this;
		opened = false;
		gameObject.SetActive(false);
	}
	public void StartFire()
	{
		gameObject.SetActive(true);
		StopAllCoroutines();
		StartCoroutine(Coroutine());
	}
	public void CloseFire()
	{
		if (opened)
		{
			opened = false;
			if (gameObject.activeInHierarchy)
			{
				StopAllCoroutines();
				StartCoroutine(CloseTimer());
			}
		}
		else
		{
			gameObject.SetActive(false);
		}
	}
	IEnumerator Coroutine()
	{
		gameObject.SetActive(true);
		for (float i = 0; i < popTime; i += Time.deltaTime)
		{
			fireIconTransform.localScale = Vector3.one * openCurve.Evaluate(i / popTime);
			yield return new WaitForEndOfFrame();
		}
		opened = true;
		while (true)
		{
			fireIconTransform.eulerAngles = new Vector3(0, 0, fireIconTransform.eulerAngles.z + rotationSpeed * Time.deltaTime);
			yield return new WaitForEndOfFrame();
		}
	}
	IEnumerator CloseTimer()
	{
		float f = fireIconTransform.localScale.x;
		for (float i = 0; i < popTime; i += Time.deltaTime)
		{
			fireIconTransform.localScale = Vector3.one * closeCurve.Evaluate(1 - i / popTime)*f;
			yield return new WaitForEndOfFrame();
		}
		gameObject.SetActive(false);
	}
}
