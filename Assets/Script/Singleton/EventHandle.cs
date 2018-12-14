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
		else if (GameFlow.friendlyFactory.established && !GameFlow.enemyFactory.established)
		{
			GameFlow.SetMode(Control.UIMode.EnemyWait);
			GameFlow.uiWaitMode.Wait(GameFlow.FACTORY_SETUP_TIMELIMIT);
		}
		else if (Socket.socketType == SocketType.Server)
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
		else
		{
			GameFlow.SetMode(Control.UIMode.None);
		}
	}
	public static void CreateFriendlyUnit(byte index)
	{
		Troop unit = GameFlow.friendlyFactory.CreateUnit(index);
		NetworkEventSend.SpawnUnit(unit.position.x, unit.position.y, index, unit.unitID);
	}
	public static void CreateEnemyUnit(Vector2Int position,byte index,int id)
	{
		GameFlow.enemyFactory.CreateUnit(position, index, id);
	}
	private static void MoveUnitGeneral(int id,Vector2Int from,Vector2Int to)
	{
		Troop unit = null;
		for (int i = 0;i < GameFlow.units.Count;i++)
		{
			if (GameFlow.units[i].unitID == id)
			{
				unit = GameFlow.units[i] as Troop;
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
			GameFlow.uiSpawnUnit.Close();
		}
		else
		{
			GameFlow.SetMode(Control.UIMode.EnemyWait);
			GameFlow.uiWaitMode.Wait(GameFlow.TURN_TIME_LIMIT);
		}
		SetResource(GameFlow.money + GameFlow.moneyRate, GameFlow.fuelLimit);
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
		int size = GameFlow.units.Count;
		Vector2Int[] data = new Vector2Int[size];
		for (int i = 0; i < size; i++)
		{
			data[i].x = GameFlow.units[i].unitID;
			data[i].y = GameFlow.units[i].hp;
		}
		NetworkEventSend.HPSync(data);
	}
	public static void SyncHP(Vector2Int[] hps)
	{
		GameFlow.fireControl.EvaluateHP(hps,false);
	}
	public static void DamageUnit(Unit unit,int deltaHP)
	{
		unit.finalHp += deltaHP;
		//GameFlow.billboardManager.Spawn(deltaHP.ToString(), unit.position);
	}
	public static void SetResource(int money,int fuel)
	{
		GameFlow.money = money;
		GameFlow.fuel = fuel;
		GameFlow.uIResourceCounter.StateUpdate();
		GameFlow.uiSpawnUnit.StateUpdate();
	}
	public static void Initialization()
	{
		GameFlow.money = 0;
		GameFlow.fuel = 0;
		GameFlow.moneyRate = 3;
		GameFlow.fuelLimit = 5;
	}
	public static void MoneyUpgrade()
	{
		GameFlow.moneyRate += 5;
		GameFlow.uIResourceCounter.StateUpdate();
	}
	public static void FuelUpgrade()
	{
		GameFlow.fuelLimit += 5;
		GameFlow.uIResourceCounter.StateUpdate();
	}
}
