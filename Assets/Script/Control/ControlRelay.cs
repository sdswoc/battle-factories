using UnityEngine;

namespace Control
{
	public class ControlRelay : MonoBehaviour, IControl
	{
		public UnitSelector unitSelector;
		public UIMode uiMode;
		private IControl currentControl;

		public void SetMode(UIMode mode)
		{
			if (mode == UIMode.Move)
			{
				currentControl = unitSelector;
			}
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
		Setup,Move,EnemyMove,Fire
	}
}