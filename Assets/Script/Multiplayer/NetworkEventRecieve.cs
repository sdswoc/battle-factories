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
			EventHandle.SetEnemyFactory(new Vector2Int(x, y));
		}

		// Code = 2
		public static void SpawnUnit(int x, int y, byte index, int id)
		{
			EventHandle.CreateEnemyUnit(new Vector2Int(x, y), index, id);
		}

		// Code = 3
		public static void MoveUnit(int id, Vector2Int from, Vector2Int to)
		{
			EventHandle.MoveEnemyUnit(id, from, to);
		}

		public static void RecieveEvent(Packet p)
		{
			byte opCode = p.ReadByte();
			Debug.Log("OPCode is " + opCode.ToString());
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
			}

		}
	}
}