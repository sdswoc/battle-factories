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
		private Button button;

		void Awake()
		{
			button = GetComponent<Button>();
		}

		public void Evaluate()
		{
			if (GameFlow.money >= money && GameFlow.fuel >=  fuel)
			{
				button.interactable = true;
			}
			else
			{
				button.interactable = false;
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