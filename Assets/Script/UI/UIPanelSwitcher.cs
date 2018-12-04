using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Control;
namespace UI
{
	public class UIPanelSwitcher : MonoBehaviour
	{
		public UIMode uiMode;
		public RectTransform[] panels;
		private RectTransform currentPanel;

		private void Awake()
		{
			GameFlow.uiPanelSwitcher = this;
		}
		public void SwitchUIMode(UIMode mode)
		{
			uiMode = mode;
			for (int i = 0; i < panels.Length; i++)
			{
				panels[i].gameObject.SetActive(false);
			}
			StartCoroutine(SwitchPanel(currentPanel, panels[(int)mode]));
			currentPanel = panels[(int)mode];
		}

		private IEnumerator SwitchPanel(RectTransform prevPanel, RectTransform newPanel)
		{
			Debug.Log("Switch");
			if (newPanel != null)
			{
				newPanel.gameObject.SetActive(false);
			}
			if (prevPanel != null)
			{
				prevPanel.gameObject.SetActive(true);
				for (float i = 0; i < 1; i += Time.deltaTime)
				{
					prevPanel.anchorMax = Vector2.right + Vector2.up * (1 - i);
					prevPanel.anchorMin = Vector2.up * -i;
					yield return new WaitForEndOfFrame();
				}
				prevPanel.gameObject.SetActive(false);
			}
			if (newPanel != null)
			{
				newPanel.gameObject.SetActive(true);
				for (float i = 0; i < 1; i += Time.deltaTime)
				{
					newPanel.anchorMax = Vector2.right + Vector2.up * i;
					newPanel.anchorMin = Vector2.up * (-1 + i);
					yield return new WaitForEndOfFrame();
				}
				newPanel.anchorMax = Vector2.right + Vector2.up;
				newPanel.anchorMin = Vector2.zero;
			}
		}
	}
}