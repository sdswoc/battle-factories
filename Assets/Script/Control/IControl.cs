using UnityEngine;

namespace Control
{
	public interface IControl
	{
		bool GetActive();

		void SetActive(bool b);

		void KeyPressed(Vector2 position);

		void KeyReleased(Vector2 position);
		
		void KeyMoved(Vector2 position);
		
		void KeyCanceled();
	}
}