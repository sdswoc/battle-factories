using UnityEngine;
using UI;
namespace Control
{
	public class ControlRelay : MonoBehaviour, IControl
	{
		public UIMode uiMode;

		private IControl currentControl;

		private void Awake()
		{
			GameFlow.controlRelay = this;
		}
		private void Start()
		{
			SetMode(uiMode);
		}
		public void SetMode(UIMode mode)
		{
			if (mode == UIMode.Move)
			{
				currentControl = GameFlow.unitSelector;
			}
			else if (mode == UIMode.Setup)
			{
				currentControl = GameFlow.setupFactory;
			}
			GameFlow.uiPanelSwitcher.SwitchUIMode(mode);
		}

		public void KeyMoved(Vector2 position)
		{
			if (currentControl != null)
			{
				currentControl.KeyMoved(position);
			}
		}

		public void KeyPressed(Vector2 position)
		{
			if (currentControl != null)
			{
				currentControl.KeyPressed(position);
			}
		}

		public void KeyReleased(Vector2 position)
		{
			if (currentControl != null)
			{
				currentControl.KeyReleased(position);
			}
		}

		public void KeyCanceled()
		{
			if (currentControl != null)
			{
				currentControl.KeyCanceled();
			}
		}


	}

	public enum UIMode
	{
		Setup = 0,Move = 1,EnemyMove = 2,Fire = 3
	}
}