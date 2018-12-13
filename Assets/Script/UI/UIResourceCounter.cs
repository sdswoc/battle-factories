using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace UI
{
	public class UIResourceCounter : MonoBehaviour
	{
		public Text text;
		public Text moneyCounter;
		public Text moneySecondaryCounter;
		public Text fuelCounter;
		public Text fuelSecondaryCounter;
		public AnimationCurve curve;
		public float spawnTime;

		private void Awake()
		{
			GameFlow.uIResourceCounter = this;
			gameObject.SetActive(false);
		}
		public void Spawn()
		{
			gameObject.SetActive(true);
			StateUpdate();
			StartCoroutine(Animate());
		}
		public void StateUpdate()
		{
			//text.text = "RS" + GameFlow.money.ToString() + " FU" + GameFlow.fuel.ToString();
			moneyCounter.text = GameFlow.money.ToString();
			moneySecondaryCounter.text = "+"+GameFlow.moneyRate.ToString();
			fuelCounter.text = GameFlow.fuel.ToString();
			fuelSecondaryCounter.text ="/"+GameFlow.fuelLimit.ToString();
		}
		IEnumerator Animate()
		{
			RectTransform r = GetComponent<RectTransform>();
			for (float i = 0;i < spawnTime;i+= Time.deltaTime)
			{
				r.localScale = curve.Evaluate(i / spawnTime)*Vector3.one;
				yield return new WaitForEndOfFrame();
			}
			r.localScale = Vector3.one;
		}
	}
}