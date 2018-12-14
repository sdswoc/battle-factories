using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFinishPanel : MonoBehaviour 
{
	public RectTransform parentTransform;
	public float showTime;
	public AnimationCurve animationCurve;
	public GameObject[] objects;
	private bool clicked;

	void Awake()
	{
		GameFlow.finishPanel = this;
		gameObject.SetActive(false);
	}
	public void OnButtonClick()
	{
		clicked = true;
	}
	public IEnumerator Show(int code)
	{
		for (int i = 0;i < objects.Length;i++)
		{
			if (i == code)
			{
				objects[i].SetActive(true);
			}
			else
			{
				objects[i].SetActive(false);
			}
		}
		yield return StartCoroutine(ShowCoroutine());
	}
	IEnumerator ShowCoroutine()
	{
		clicked = false;
		parentTransform.localScale = Vector3.zero;
		yield return new WaitForEndOfFrame();
		gameObject.SetActive(true);
		for (float i = 0; i < showTime; i += Time.deltaTime)
		{
			parentTransform.localScale = Vector3.one * animationCurve.Evaluate(i/showTime);
			yield return new WaitForEndOfFrame();
		}
		parentTransform.localScale = Vector3.one;
		yield return new WaitUntil(() => { return clicked; });
	}
}
