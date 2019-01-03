using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Singleton;

namespace UI
{
	public class UIUnitButton : MonoBehaviour
	{
        public ButtonType buttonType;
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
            OverwriteValues();
            UpdateText();
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
            OverwriteValues();
            UpdateText();
		}
		public void EvaluateUI()
		{
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
        public void OverwriteValues()
        {
            if (buttonType == ButtonType.Tank)
            {
                money = ValueLoader.tankCostCoin == 0 ? money : ValueLoader.tankCostCoin;
                fuel = ValueLoader.tankCostFuel == 0 ? fuel : ValueLoader.tankCostFuel;
            }
            else if (buttonType == ButtonType.Truck)
            {
                money = ValueLoader.truckCostCoin == 0 ? money : ValueLoader.truckCostCoin;
                fuel = ValueLoader.truckCostFuel == 0 ? fuel : ValueLoader.truckCostFuel;
            }
            else if (buttonType == ButtonType.Medic)
            {
                money = ValueLoader.medicCostCoin == 0 ? money : ValueLoader.medicCostCoin;
                fuel = ValueLoader.medicCostFuel == 0 ? fuel : ValueLoader.medicCostFuel;
            }
            else if (buttonType == ButtonType.FuelUpgrade)
            {
                money = ValueLoader.fuelUpgradeCostCoin == 0 ? money : ValueLoader.fuelUpgradeCostCoin;
                fuel = GameFlow.fuelLimit;
            }
            else if (buttonType == ButtonType.MoneyUpgrade)
            {
                money = ValueLoader.moneyUpgradeCostCoin == 0 ? money : ValueLoader.moneyUpgradeCostCoin;
                fuel = GameFlow.fuelLimit;
            }
        }
        public void UpdateText()
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
	}
    [SerializeField]
    public enum ButtonType
    {
        Tank,Truck,Medic,FuelUpgrade,MoneyUpgrade
    }
}