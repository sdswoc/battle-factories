using UnityEngine;
using UI;
namespace Control
{
	public class ControlRelay : MonoBehaviour
	{
		public UIMode uiMode;

		private IControl currentControl;

		private void Awake()
		{
			GameFlow.controlRelay = this;
		}
		private void Start()
		{
			GameFlow.SetMode(uiMode);
		}
		public void SwitchUIMode(UIMode mode)
		{
			if (currentControl != null)
			{
				currentControl.SetActive(false);
			}
			if (mode == UIMode.Move)
			{
				currentControl = GameFlow.unitSelector;
			}
			else if (mode == UIMode.Setup)
			{
				currentControl = GameFlow.setupFactory;
			}
			else
			{
				currentControl = null;
			}
			if (currentControl != null)
			{
				currentControl.SetActive(true);
			}
			uiMode = mode;
		}

		public void KeyMoved(Vector2 position)
		{
			if (currentControl != null && currentControl.GetActive())
			{
				currentControl.KeyMoved(position);
			}
		}

		public void KeyPressed(Vector2 position)
		{
			if (currentControl != null && currentControl.GetActive())
			{
				currentControl.KeyPressed(position);
			}
		}

		public void KeyReleased(Vector2 position)
		{
			if (currentControl != null && currentControl.GetActive())
			{
				currentControl.KeyReleased(position);
			}
		}

		public void KeyCanceled()
		{
			if (currentControl != null && currentControl.GetActive())
			{
				currentControl.KeyCanceled();
			}
		}


	}

	public enum UIMode
	{
		Setup = 0,Move = 1,EnemyWait = 2,Fire = 3,None = 4
	}
}