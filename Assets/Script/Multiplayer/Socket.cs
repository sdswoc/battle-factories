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
		public static Action OnBeaconListUpdated;
		public static Action<Packet> OnData;
		public static Action<string,int> OnListenerStarted;
		public static SocketType socketType;
		private static Server server;
		private static Client client;
		private static Beacon beacon;
		private static Probe probe;
		private static int connectionID = -1;
		public static string playerName;
		private const  string BEACON_TEXT = "battle-factories";
		public static List<BeaconLocation> beaconLocations = new List<BeaconLocation>();
		public static string StartServer()
		{
			SetLogger();
			connectionID = -1;
			server = new Server();
			client?.Disconnect();
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
			server?.Stop();
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
			beacon?.Stop();
			beacon?.Dispose();
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
			beacon.BeaconData = playerName ?? "Unnamed Player";
			beacon.Start();
		}
		public static void StartProbe()
		{
			probe?.Dispose();
			probe = new Probe(BEACON_TEXT);
			probe.BeaconsUpdated += OnBeaconFound;
			probe.Start();
		}
		public static void StopProbe()
		{
			probe?.Stop();
			probe?.Dispose();
			probe = null;
		}
		public static void StopBeacon()
		{
			beacon?.Stop();
			beacon?.Dispose();
			beacon = null;
		}
		public static void OnBeaconFound(IEnumerable<BeaconLocation> beacons)
		{
			List<BeaconLocation> list = new List<BeaconLocation>();
			foreach (BeaconLocation beacon in beacons)
			{
				Debug.Log("Beacon found " + beacon.Address);
				list.Add(beacon);
				/*
				StartClient(beacon.Address.Address.ToString(), beacon.Address.Port);
StopProbe();*/
			}
			if (list.Count != beaconLocations.Count)
			{
				beaconLocations = list;
				OnBeaconListUpdated?.Invoke();
				Debug.Log("unequal");
				return;
			}
			for (int i = 0;i < list.Count;i++)
			{
				bool found = false;
				for (int j = 0;j < beaconLocations.Count;j++)
				{
					if (beaconLocations[j].Address.ToString().Equals(list[i].Address.ToString()) && beaconLocations[j].Data == list[i].Data)
					{
						found = true;
						break;
					}
				}
				if (!found)
				{
					beaconLocations = list;
					OnBeaconListUpdated?.Invoke();
					Debug.Log("Different");
					return;
				}
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
		public static void Reset()
		{
			server?.Stop();
			client?.Disconnect();
			beacon?.Stop();
			beacon?.Dispose();
			probe?.Stop();
			probe?.Dispose();
		}
	}
	public enum SocketType
	{
		Server,Client
	}
}