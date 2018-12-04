using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Control;

public class SetupFactory : MonoBehaviour, IControl
{
	public new Transform transform;

	private bool invokeReleaseEvent = false;
	private Vector2 prevPosition;
	private Vector2 currentPosition;

	private void Awake()
	{
		GameFlow.setupFactory = this;
	}
	public void KeyCanceled()
	{
		invokeReleaseEvent = false;
		transform.position = prevPosition;
	}

	public void KeyMoved(Vector2 position)
	{
		transform.position = position;
	}

	public void KeyPressed(Vector2 position)
	{
		invokeReleaseEvent = true;
		transform.position = position;
	}

	public void KeyReleased(Vector2 position)
	{
		if (invokeReleaseEvent)
		{
			prevPosition = transform.position;
		}
	}

	public void OnOKButton()
	{
		
	}
}
