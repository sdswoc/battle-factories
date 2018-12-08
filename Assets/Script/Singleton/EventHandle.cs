using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Multiplayer;

public class EventHandle : MonoBehaviour 
{
	private static bool myTurn;
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
			GameFlow.SetMode(Control.UIMode.EnemyWait);
			GameFlow.uiWaitMode.Wait(GameFlow.FACTORY_SETUP_TIMELIMIT);
			NetworkEventSend.FactoryLocation(position.x, position.y);
			CheckFactorySetupAndStartTheGame();
		}
	}
	public static void SetEnemyFactory(Vector2Int position)
	{
		GameFlow.enemyFactory.gameObject.SetActive(true);
		GameFlow.enemyFactory.Setup(position);
		CheckFactorySetupAndStartTheGame();
	}
	public static void CheckFactorySetupAndStartTheGame()
	{
		if (!GameFlow.friendlyFactory.established && GameFlow.enemyFactory.established)
		{
			GameFlow.uiSetupMode.SetDeadLine();
		}
		if (Socket.socketType == SocketType.Server)
		{
			if (GameFlow.friendlyFactory.established && GameFlow.enemyFactory.established)
			{
				SocketType firstTurn = SocketType.Server;
				Debug.Log("TODO set randomness");
				if (Random.Range(0.1f, 1) > 0)
				{
					firstTurn = SocketType.Client;
				}
				AnnounceStartGame(firstTurn);
			}
		}
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
	public static void AnnounceStartGame(SocketType type)
	{
		SetTurn(type == Socket.socketType);
		if (Socket.socketType == SocketType.Server)
		{
			NetworkEventSend.SetTurn(type == SocketType.Client);
		}
	}
	public static void SetTurn(bool myTurn)
	{
		if (myTurn)
		{
			GameFlow.SetMode(Control.UIMode.Move);
			GameFlow.uiMoveMode.SetDeadline(GameFlow.TURN_TIME_LIMIT);
		}
		else
		{
			GameFlow.SetMode(Control.UIMode.EnemyWait);
			GameFlow.uiWaitMode.Wait(GameFlow.TURN_TIME_LIMIT);
		}
		EventHandle.myTurn = myTurn;
	}
	public static void FinishTurnLocal()
	{
		GameFlow.SetMode(Control.UIMode.Fire);
		GameFlow.uiFireMode.myTurn = true;
		GameFlow.fireControl.Execute(true);
		NetworkEventSend.TurnFinish();
	}
	public static void FinishTurnRemote()
	{
		GameFlow.SetMode(Control.UIMode.Fire);
		GameFlow.uiFireMode.myTurn = false;
		GameFlow.fireControl.Execute(false);
	}
	public static void FinishFire(bool myTurn)
	{
		if (myTurn)
		{
			SendHP();
			GameFlow.fireControl.EvaluateHP(true);
		}
	}
	public static void SendHP()
	{
		Debug.Log("HPs sent");
		int size = Unit.units.Count;
		Vector2Int[] data = new Vector2Int[size];
		for (int i = 0; i < size; i++)
		{
			data[i].x = Unit.units[i].unitID;
			data[i].y = Unit.units[i].hp;
		}
		NetworkEventSend.HPSync(data);
	}
	public static void SyncHP(Vector2Int[] hps)
	{
		Debug.Log("HP data recievd");
		GameFlow.fireControl.EvaluateHP(hps,false);
	}
	public static void DamageUnit(Unit unit,int deltaHP)
	{
		unit.hp += deltaHP;
		GameFlow.billboardManager.Spawn(deltaHP.ToString(), unit.position);
	}
}
