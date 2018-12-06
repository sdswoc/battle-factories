using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Multiplayer;

public class Test : MonoBehaviour 
{
	private void Awake()
	{
		Packet p = new Packet();
		p.WriteByte(255);
		p.WriteByte(125);
		p.WriteInt(89878);
		p.WriteString("test string");
		p.WriteString("");
		Debug.Log(p.ReadByte());
		Debug.Log(p.ReadByte());
		Debug.Log(p.ReadInt());
		Debug.Log(p.ReadString());
		Debug.Log(p.ReadString());
		Debug.Log(p.ReadString());

	}
}
