using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
	public class UISpawnModule : MonoBehaviour
	{
		public GameObject panel;
		public float itemRatio;
		public float gap;
		public UIUnitButton[] items;
		public RectTransform scrollViewTranform;
		
		public float panelTransitionTime;
		public AnimationCurve panelTransitionCurve;
		private Rect initialRectangle;

		private bool opened;

		private void Awake()
		{
			GameFlow.uiSpawnUnit = this;
			AdjustPanel(panel.GetComponent<RectTransform>());
		}
		private void Start()
		{
			StateUpdate();
		}
		public void Close()
		{
			if (opened)
			{
				opened = false;
				StateUpdate();
				StartCoroutine(CloseCoroutine());
			}
		}
		public void Open()
		{
			opened = true;
			StateUpdate();
			StartCoroutine(OpenCoroutine());
		}
		public void AdjustPanel(RectTransform transform)
		{
			initialRectangle = scrollViewTranform.rect;
			float width = transform.rect.width;
			float height = width / itemRatio;
			Debug.Log(width);
			
			for (int i = 0;i < transform.childCount;i++)
			{
				RectTransform child = transform.GetChild(i).GetComponent<RectTransform>();
				child.offsetMin = new Vector2(0, -((i) * (height+gap)+height));
				child.offsetMax = new Vector2(0, -((i) * (height + gap)));
			}
			panel.GetComponent<RectTransform>().offsetMin = new Vector2(0, -(transform.childCount) * (height + gap) - gap);
			SetPosition(0);
		}
		public void OnButton()
		{
			if (!GameFlow.unitSelector.IsCommandingUnit())
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
		}
		public void OnSpawnButton(int i)
		{
			EventHandle.CreateFriendlyUnit((byte)i);
			Close();
		}
		public void OnMoneyUpgrade()
		{
			EventHandle.MoneyUpgrade();
			Close();
		}
		public void OnFuelUpgrade()
		{
			EventHandle.FuelUpgrade();
			Close();
		}
		public void StateUpdate()
		{
			for (int i = 0;i < items.Length;i++)
			{
				items[i].Evaluate();
			}
		}
		IEnumerator OpenCoroutine()
		{
			StateUpdate();
			for (float i = 0;i < panelTransitionTime;i += Time.deltaTime)
			{
				SetPosition(panelTransitionCurve.Evaluate(i / panelTransitionTime));
				yield return new WaitForEndOfFrame();
			}
			SetPosition(1);
		}
		IEnumerator CloseCoroutine()
		{
			scrollViewTranform.gameObject.SetActive(true);
			for (float i = 0; i < panelTransitionTime; i += Time.deltaTime)
			{
				SetPosition(1-panelTransitionCurve.Evaluate(i / panelTransitionTime));
				yield return new WaitForEndOfFrame();
			}
			SetPosition(0);
		}
		public void SetPosition(float i)
		{
			scrollViewTranform.offsetMin = new Vector2((-1+i) * (initialRectangle.width), 0);
			scrollViewTranform.offsetMax = new Vector2(i * (initialRectangle.width), 1);
		}
	}
}