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
		public AnimationCurve closePanel;
		public AnimationCurve openPanel;
		public float switchSpeed;
		private RectTransform currentPanel;

		private void Awake()
		{
			GameFlow.uiPanelSwitcher = this;
		}
		public void SwitchUIMode(UIMode mode)
		{
			uiMode = mode;
			GameFlow.timer.CloseTime();
			GameFlow.uiSpawnUnit.Close();
			for (int i = 0; i < panels.Length; i++)
			{
				panels[i].gameObject.SetActive(false);
			}
			StartCoroutine(SwitchPanel(currentPanel, panels[(int)mode]));
			currentPanel = panels[(int)mode];
		}

		private IEnumerator SwitchPanel(RectTransform prevPanel, RectTransform newPanel)
		{
			if (newPanel != null)
			{
				newPanel.gameObject.SetActive(false);
			}
			if (prevPanel != null)
			{
				prevPanel.gameObject.SetActive(true);
				for (float i = 0; i < 1; i += Time.deltaTime*switchSpeed)
				{
					prevPanel.anchorMax = Vector2.right + Vector2.up * closePanel.Evaluate(i);
					prevPanel.anchorMin = Vector2.up * (closePanel.Evaluate(i)-1);
					yield return new WaitForEndOfFrame();
				}
				prevPanel.gameObject.SetActive(false);
			}
			if (newPanel != null)
			{
				newPanel.gameObject.SetActive(true);
				for (float i = 0; i < 1; i += Time.deltaTime*switchSpeed)
				{
					newPanel.anchorMax = Vector2.right + Vector2.up * openPanel.Evaluate(i);
					newPanel.anchorMin = Vector2.up * (-1 + openPanel.Evaluate(i));
					yield return new WaitForEndOfFrame();
				}
				newPanel.anchorMax = Vector2.right + Vector2.up;
				newPanel.anchorMin = Vector2.zero;
			}
		}
	}
}