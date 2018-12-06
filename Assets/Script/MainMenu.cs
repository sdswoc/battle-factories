using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using Multiplayer;
using Telepathy;
using UnityEngine.SceneManagement;
using BeaconLib;
using System.Net;

public class MainMenu : MonoBehaviour 
{
	public InputField input;
	public Text text;
	public Scene scene;
	public Common common;
	public Beacon beacon;
	public Probe probe;
	
	public void StartServer()
	{
		Transport.server = new Server();
		Transport.client = null;
		Transport.socketMode = SocketMode.Server;
		common = Transport.server;
		Transport.server.Start(0);
		SetLogger();
	}
	public void StartClient()
	{
		Transport.server = null;
		Transport.client = new Client();
		common = Transport.client;
		Transport.socketMode = SocketMode.Client;
		SetLogger();
		probe = new Probe("battle-factories");
		probe.BeaconsUpdated += OnProbeFind;
		probe.Start();
	}
	public void SwitchScene()
	{
		SceneManager.LoadScene("SampleScene");
	}
	public void SetLogger()
	{
		Telepathy.Logger.LogMethod = Debug.Log;
		Telepathy.Logger.LogWarningMethod = Debug.LogWarning;
		Telepathy.Logger.LogErrorMethod = Debug.LogError;
	}
	private void OnProbeFind(IEnumerable<BeaconLocation> locations)
	{
		foreach (var beacon in locations)
		{
			if (Transport.client != null)
			{
				Transport.client.Connect(beacon.Address.Address.MapToIPv4().ToString(), int.Parse(beacon.Data));
				Debug.Log("Connected");
				break;
			}
		}
	}
	private void Update()
	{
		if (Transport.server != null)
		{
			if (Transport.server.GetListenerEndPoint() != null)
			{
				if (beacon == null)
				{
					beacon = new Beacon("battle-factories", (ushort)((IPEndPoint)Transport.server.GetListenerEndPoint()).Port);
					beacon.BeaconData = ((ushort)((IPEndPoint)Transport.server.GetListenerEndPoint()).Port).ToString();
					beacon.Start();
					text.text = "Beacon started";
					Debug.Log("Started beacon @ " + ((ushort)((IPEndPoint)Transport.server.GetListenerEndPoint()).Port).ToString());
				}
			}
		}
		if (common != null)
		{
			Message msg;
			if (common.GetNextMessage(out msg))
			{
				if (msg.eventType == Telepathy.EventType.Connected)
				{
					Transport.connectionId = msg.connectionId;
					if (beacon != null)
					{
						beacon.Stop();
						beacon.Dispose();
						beacon = null;
					}
					if (probe != null)
					{
						probe.Stop();
						probe.Dispose();
						probe = null;
					}
					SwitchScene();
				}
			}
		}

	}
}
