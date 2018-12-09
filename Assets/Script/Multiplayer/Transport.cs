using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Telepathy;
namespace Multiplayer
{
	public class Transport : MonoBehaviour
	{
		private void Awake()
		{
			Socket.OnData += DataEvent;
			Socket.OnDisconnected += DisconnectEvent;
		}
		private void Update()
		{
			Socket.Update();
		}
		private void OnApplicationQuit()
		{
			Socket.StopClient();
			Socket.StopServer();
		}
		private void DisconnectEvent()
		{
			Application.Quit();
		}
		private void DataEvent(Packet packet)
		{
			NetworkEventRecieve.RecieveEvent(packet);
		}
	}
	public enum SocketMode
	{
		Server,Client
	}
}