using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour 
{
	public GridRectangle rectangle;
	public Vector2Int position;
	private bool established;

	private void Awake()
	{
	}
	public void MoveToPosition(Vector2Int position)
	{
		if (!established)
		{
			this.position = position;
			rectangle.position = position - new Vector2Int((int)(rectangle.size.x * 0.5f), (int)(rectangle.size.y * 0.5f));
			rectangle.Setup();
		}
	}
	public bool EvaluatePosition()
	{
		bool good = true;
		for (int i = rectangle.position.x; i < rectangle.position.x + rectangle.size.x; i++)
		{
			for (int j = rectangle.position.y; j < rectangle.position.y + rectangle.size.y; j++)
			{
				if (GameFlow.map.GetObstacle(new Vector2Int(i, j)))
				{
					good = false;
					break;
				}
			}
		}
		return good;
	}
	public void Setup(Vector2Int position)
	{
		MoveToPosition(position);
		GameFlow.map.RegisterObstacle(rectangle);
		established = true;
	}
}
