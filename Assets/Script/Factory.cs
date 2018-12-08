using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Multiplayer;

public class Factory : MonoBehaviour 
{
	public GridRectangle rectangle;
	public Vector2Int position;
	public UnitType type;
	public GameObject[] units;
	public Vector2Int serverFactoryPosition;
	public Vector2Int clientFactoryPosition;
	public bool established;
	private static int idGeneratorIndex;


	private void Awake()
	{
		if (type == UnitType.Friendly)
		{
			GameFlow.friendlyFactory = this;
		}
		else
		{
			GameFlow.enemyFactory = this;
			gameObject.SetActive(false);
		}
		if (Socket.socketType == SocketType.Server)
		{
			MoveToPosition(serverFactoryPosition);
		}
		else
		{
			MoveToPosition(clientFactoryPosition);
		}
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
		if (Socket.socketType == SocketType.Server)
		{
			if (rectangle.position.y <= 0 || rectangle.position.y+rectangle.size.y > GameFlow.map.height/2)
			{
				good = false;
			}
		}
		else
		{
			if (rectangle.position.y <= GameFlow.map.height/2 || rectangle.position.y + rectangle.size.y > GameFlow.map.height)
			{
				good = false;
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
	public Vector2Int GetNearestEmptyLocation()
	{
		float distance = float.MaxValue;
		Vector2Int samplePosition = new Vector2Int(0, 0);
		Vector2Int position = rectangle.position + new Vector2Int((int)(rectangle.size.x * 0.5f), (int)(rectangle.size.y * 0.5f));
		for (int i = 0;i < GameFlow.map.width;i++)
		{
			for (int j = 0; j < GameFlow.map.height; j++)
			{
				if (!GameFlow.map.GetObstacle(new Vector2Int(i, j)))
				{
					if ((position - new Vector2Int(i, j)).sqrMagnitude < distance)
					{
						samplePosition.x = i;
						samplePosition.y = j;
						distance = (position - new Vector2Int(i, j)).sqrMagnitude;
					}
				}
			}
		}
		return samplePosition;
	}
	public Unit CreateUnit(byte unitIndex)
	{
		return CreateUnit(unitIndex, GetID());
	}
	public Unit CreateUnit(byte unitIndex,int id)
	{
		return CreateUnit(GetNearestEmptyLocation(), unitIndex, id);
	}
	public Unit CreateUnit(Vector2Int postion,byte unitIndex,int id)
	{
		Unit u = Instantiate(units[unitIndex % units.Length]).GetComponent<Unit>();
		u.Spawn(postion, type, id);
		return u;
	}

	public static int GetID()
	{
		idGeneratorIndex++;
		return ((int)Socket.socketType | (idGeneratorIndex++ << 1));
	}
}
