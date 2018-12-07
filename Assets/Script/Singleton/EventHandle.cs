using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Multiplayer;

public class EventHandle : MonoBehaviour 
{
	private static List<Pathfinding.PathNode> pathNodes = new List<Pathfinding.PathNode>();
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
			NetworkEventSend.FactoryLocation(position.x, position.y);
		}
	}
	public static void SetEnemyFactory(Vector2Int position)
	{
		GameFlow.enemyFactory.Setup(position);
	}
	public static void CreateFriendlyUnit(byte index)
	{
		Unit unit = GameFlow.friendlyFactory.CreateUnit(index);
		NetworkEventSend.SpawnUnit(unit.position.x, unit.position.y, index, unit.unitID);
	}
	public static void CreateEnemyUnit(Vector2Int position,byte index,int id)
	{
		GameFlow.enemyFactory.CreateUnit(position, index, id);
	}
	private static void MoveUnitGeneral(int id,Vector2Int from,Vector2Int to)
	{
		Unit unit = null;
		for (int i = 0;i < Unit.units.Count;i++)
		{
			if (Unit.units[i].unitID == id)
			{
				unit = Unit.units[i];
				break;
			}
		}
		if (unit != null)
		{
			pathNodes.Clear();
			GameFlow.aStar.GeneratePath(from, to, ref pathNodes,int.MaxValue);
			unit.Move(pathNodes);
		}
	}
	public static void MoveFriendlyUnit(int id, Vector2Int from, Vector2Int to)
	{
		MoveUnitGeneral(id, from, to);
		NetworkEventSend.MoveUnit(id, from, to);
	}
	public static void MoveEnemyUnit(int id, Vector2Int from, Vector2Int to)
	{
		MoveUnitGeneral(id, from, to);
	}
}
