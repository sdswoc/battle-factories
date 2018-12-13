using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
	public class UIUnitButton : MonoBehaviour
	{
		public int money;
		public int fuel;
		[SerializeField]
		public UnityEvent OnClickEvent;
		public Text fuelText;
		public Text coinText;
		public Image fuelImage;
		public Image coinImage;
		public Image sourceImage;
		public Button button;
		private Color previousColor;

		void Awake()
		{
			int m = money;
			int ones = m % 10;
			int tens = m / 10;
			coinText.text = tens.ToString() + ones.ToString();
			m = fuel;
			ones = m % 10;
			tens = m / 10;
			fuelText.text = tens.ToString() + ones.ToString();
		}

		private void Update()
		{
			fuelImage.color = coinImage.color = fuelText.color = coinText.color = sourceImage.color;
		}

		public void Evaluate()
		{
			if (GameFlow.money >= money && GameFlow.fuel >= fuel)
			{
				button.interactable = true;
			}
			else
			{
				button.interactable = false;
			}
			if (previousColor != sourceImage.color)
			{
				previousColor = sourceImage.color;
				fuelImage.color = coinImage.color = fuelText.color = coinText.color = sourceImage.color;
			}
		}
		public void EvaluateUI()
		{
			Debug.Log(button.interactable);
			if (button.interactable)
			{
				GetComponent<Animator>().SetTrigger(button.animationTriggers.normalTrigger);
				GetComponent<Animator>().SetBool("Normal", true);
			}
			else
			{
				GetComponent<Animator>().SetTrigger(button.animationTriggers.disabledTrigger);
				GetComponent<Animator>().SetBool("Disabled", true);
			}
		}

		public void OnClick()
		{
			if (GameFlow.money >= money && GameFlow.fuel >= fuel)
			{
				EventHandle.SetResource(GameFlow.money - money, GameFlow.fuel - fuel);
				OnClickEvent?.Invoke();
			}
		}
	}

}