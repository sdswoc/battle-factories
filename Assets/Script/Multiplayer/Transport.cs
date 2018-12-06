using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Telepathy;
namespace Multiplayer
{
	public class Transport : MonoBehaviour
	{
		public static Server server;
		public static Client client;
		public static SocketMode socketMode;
		public static int connectionId;
		public void CloseConnections()
		{
			if (server != null)
			{
				server.Stop();
				server = null;
			}
			if (client != null)
			{
				client.Disconnect();
				client = null;
			}
		}
		private void OnApplicationQuit()
		{
			CloseConnections();
		}
	}
	public enum SocketMode
	{
		Server,Client
	}
}