using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Telepathy;
using BeaconLib;

namespace Multiplayer
{
	public class Socket
	{
		public static Action OnConnected;
		public static Action OnDisconnected;
		public static Action<Packet> OnData;
		public static Action<string,int> OnListenerStarted;
		public static SocketType socketType;
		private static Server server;
		private static Client client;
		private static Beacon beacon;
		private static Probe probe;
		private static int connectionID = -1;
		private const  string BEACON_TEXT = "battle-factories";
		public static string StartServer()
		{
			SetLogger();
			server = new Server();
			client = null;
			server.ListenerStarted += ListenerStarted;
			server.Start(0, 1);
			socketType = SocketType.Server;
			return server.GetLocalIPAddress();
		}
		public static void StopServer()
		{
			server?.Stop();
			server = null;
		}
		public static void StartClient(string address,int port)
		{
			SetLogger();
			server = null;
			client = new Client();
			client.Connect(address, port);
			socketType = SocketType.Client;
		}
		public static void StopClient()
		{
			client?.Disconnect();
			client = null;
		}
		public static void ConnectEvent()
		{
			OnConnected?.Invoke();
			Debug.Log("Connected");
		}
		public static void DisconnectEvent()
		{
			OnDisconnected?.Invoke();
			Debug.Log("Disconnect");
		}
		public static void DataEvent(Packet packet)
		{
			OnData?.Invoke(packet);
		}
		public static void Update()
		{
			Common common = (Common)server ?? (Common)(client ?? null);			
			if (common != null)
			{
				Message message;
				while (common.GetNextMessage(out message))
				{
					switch (message.eventType)
					{
						case Telepathy.EventType.Connected:
							connectionID = message.connectionId;
							ConnectEvent();
							break;
						case Telepathy.EventType.Disconnected:
							DisconnectEvent();
							break;
						case Telepathy.EventType.Data:
							Packet p = new Packet(message.data);
							DataEvent(p);
							break;
					}
				}
			}
		}
		public static void ListenerStarted(string address,int port)
		{
			OnListenerStarted?.Invoke(address,port);
			beacon?.Dispose();
			beacon = new Beacon(BEACON_TEXT, (ushort)port);
			beacon.BeaconData = "Hola Friends";
			beacon.Start();
		}
		public static void StartProbe()
		{
			probe?.Dispose();
			probe = new Probe(BEACON_TEXT);
			probe.BeaconsUpdated += OnBeaconFound;
			probe.Start();
		}
		public static void OnBeaconFound(IEnumerable<BeaconLocation> beacons)
		{
			foreach (BeaconLocation beacon in beacons)
			{
				Debug.Log("Beacon Found"+ beacon.Address.ToString()+ beacon.Address.Address.ToString());
				StartClient(beacon.Address.Address.ToString(), beacon.Address.Port);
				probe?.Stop();
				probe?.Dispose();
				probe = null;
			}
		}
		public static void SetLogger()
		{
			Telepathy.Logger.LogMethod = Debug.Log;
			Telepathy.Logger.LogWarningMethod = Debug.LogWarning;
			Telepathy.Logger.LogErrorMethod = Debug.LogError;
		}
		public static void Send(Packet packet)
		{
			server?.Send(connectionID, packet?.data?.ToArray());
			client?.Send(packet?.data?.ToArray());
		}

	}
	public enum SocketType
	{
		Server,Client
	}
}