using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDisconnectPanel : MonoBehaviour
{
	public RectTransform parentTransform;
	public float showTime;
	public AnimationCurve animationCurve;
	private bool clicked;

	void Awake()
	{
		GameFlow.disconnectPanel = this;
		gameObject.SetActive(false);
	}
	public void OnButtonClick()
	{
		clicked = true;
	}
	public IEnumerator Show()
	{
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
			parentTransform.localScale = Vector3.one * animationCurve.Evaluate(i / showTime);
			yield return new WaitForEndOfFrame();
		}
		parentTransform.localScale = Vector3.one;
		yield return new WaitUntil(() => { return clicked; });
	}
}
