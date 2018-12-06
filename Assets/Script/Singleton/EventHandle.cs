using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandle : MonoBehaviour 
{
	private void Awake()
	{
		GameFlow.eventHandle = this;
	}
	public static void SetFriendlyFactory(Vector2Int position)
	{
		if (GameFlow.controlRelay.uiMode == Control.UIMode.Setup)
		{
			GameFlow.friendlyFactory.Setup(position);
			GameFlow.SetMode(Control.UIMode.Move);
		}
	}
}
