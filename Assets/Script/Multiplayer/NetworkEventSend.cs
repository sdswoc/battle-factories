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
			Packet p = new Packet(1);
			p.WriteInt(x);
			p.WriteInt(y);
			Socket.Send(p);
		}

		// Code = 2
		public static void SpawnUnit(int x,int y,byte index,int id)
		{
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
			Packet p = new Packet(3);
			p.WriteInt(id);
			p.WriteInt(from.x);
			p.WriteInt(from.y);
			p.WriteInt(to.x);
			p.WriteInt(to.y);
			Socket.Send(p);
		}
	}
}