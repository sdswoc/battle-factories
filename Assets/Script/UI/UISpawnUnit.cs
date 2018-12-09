using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class UISpawnUnit : MonoBehaviour
	{
		public GameObject panel;
		public UIUnitButton[] items;

		private bool opened;

		private void Awake()
		{
			GameFlow.uiSpawnUnit = this;
		}

		public void Close()
		{
			opened = false;
			StateUpdate();
		}
		public void Open()
		{
			opened = true;
			StateUpdate();
		}

		public void OnButton()
		{
			if (opened)
			{
				Close();
			}
			else
			{
				Open();
			}
		}
		public void OnSpawnButton(int i)
		{
			EventHandle.CreateFriendlyUnit((byte)i);
		}
		public void OnMoneyUpgrade()
		{
			EventHandle.MoneyUpgrade();
		}
		public void OnFuelUpgrade()
		{
			EventHandle.FuelUpgrade();
		}
		public void StateUpdate()
		{
			panel.SetActive(opened);
			for (int i = 0;i < items.Length;i++)
			{
				items[i].Evaluate();
			}
		}
	}
}