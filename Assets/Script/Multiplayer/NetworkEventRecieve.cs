using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Multiplayer
{
	public class NetworkEventRecieve
	{
		// Code = 1
		public static void FactoryLocation(int x,int y)
		{
			Debug.Log("FactoryLocation");
			EventHandle.SetEnemyFactory(new Vector2Int(x, y));
		}

		// Code = 2
		public static void SpawnUnit(int x, int y, byte index, int id)
		{
			Debug.Log("SpawnUnit");
			EventHandle.CreateEnemyUnit(new Vector2Int(x, y), index, id);
		}

		// Code = 3
		public static void MoveUnit(int id, Vector2Int from, Vector2Int to)
		{
			Debug.Log("MoveUnit");
			EventHandle.MoveEnemyUnit(id, from, to);
		}

		// Code = 4
		public static void SetTurn(bool myTurn)
		{
			Debug.Log("SetTurn");
			EventHandle.SetTurn(myTurn);
		}

		// Code = 5
		public static void TurnFinish()
		{
			Debug.Log("TurnFinish");
			EventHandle.FinishTurnRemote();
		}

		// Code = 6
		public static void HPSync(Packet p)
		{
			int size = p.ReadInt();
			Vector2Int[] data = new Vector2Int[size];
			for (int i = 0;i < size;i++)
			{
				data[i].x = p.ReadInt();
				data[i].y = p.ReadInt();
			}
			Debug.Log("HPSync");
			EventHandle.SyncHP(data);
		}

		public static void RecieveEvent(Packet p)
		{
			byte opCode = p.ReadByte();
			switch (opCode)
			{
				case 1:
					FactoryLocation(p.ReadInt(), p.ReadInt());
					break;
				case 2:
					SpawnUnit(p.ReadInt(), p.ReadInt(), p.ReadByte(), p.ReadInt());
					break;
				case 3:
					MoveUnit(p.ReadInt(), new Vector2Int(p.ReadInt(), p.ReadInt()), new Vector2Int(p.ReadInt(), p.ReadInt()));
					break;
				case 4:
					SetTurn(p.ReadByte() != 0);
					break;
				case 5:
					TurnFinish();
					break;
				case 6:
					HPSync(p);
					break;
			}

		}
	}
}