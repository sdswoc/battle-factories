using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
	public class UIRegister : MonoBehaviour
	{
		private void Awake()
		{
			GameFlow.eventSystem = GetComponent<EventSystem>();
			GameFlow.graphicRaycaster = GetComponent<GraphicRaycaster>();
		}
	}
}