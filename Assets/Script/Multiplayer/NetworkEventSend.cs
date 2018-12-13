using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Multiplayer
{
	public class NetworkEventSend
	{
		// Code = 1
		public static void FactoryLocation(int x,int y)
		{
			Debug.Log("SendFactoryLocation");
			Packet p = new Packet(1);
			p.WriteInt(x);
			p.WriteInt(y);
			Socket.Send(p);
		}

		// Code = 2
		public static void SpawnUnit(int x,int y,byte index,int id)
		{
			Debug.Log("SendSpawnUnit");
			Packet p = new Packet(2);
			p.WriteInt(x);
			p.WriteInt(y);
			p.WriteByte(index);
			p.WriteInt(id);
			Socket.Send(p);
		}

		// Code = 3
		public static void MoveUnit(int id,Vector2Int from,Vector2Int to)
		{
			Debug.Log("SendMoveUnit");
			Packet p = new Packet(3);
			p.WriteInt(id);
			p.WriteInt(from.x);
			p.WriteInt(from.y);
			p.WriteInt(to.x);
			p.WriteInt(to.y);
			Socket.Send(p);
		}

		// Code = 4
		public static void SetTurn(bool myTurn)
		{
			Debug.Log("SendSetTurn");
			Packet p = new Packet(4);
			if (myTurn)
			{
				p.WriteByte(1);
			}
			else
			{
				p.WriteByte(0);
			}
			Socket.Send(p);
		}

		// Code = 5
		public static void TurnFinish()
		{
			Debug.Log("SendTurnFinish");
			Packet p = new Packet(5);
			Socket.Send(p);
		}

		// Code = 6
		public static void HPSync(Vector2Int[] data)
		{
			Debug.Log("SendHPSync");
			Packet p = new Packet(6);
			p.WriteInt(GameFlow.units.Count);
			for (int i = 0;i < GameFlow.units.Count;i++)
			{
				p.WriteInt(GameFlow.units[i].unitID);
				p.WriteInt(GameFlow.units[i].hp);
			}
			Socket.Send(p);
		}
	}
}