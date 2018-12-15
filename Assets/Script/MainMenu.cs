using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using Multiplayer;
using Telepathy;
using UnityEngine.SceneManagement;
using BeaconLib;

public class MainMenu : MonoBehaviour 
{
	public InputField input;
	public Text text;
	public Scene scene;
	public Common common;
	public Beacon beacon;
	public Probe probe;
	private string textField = "";
	
	public void StartServer()
	{
		Socket.OnConnected += SwitchScene;
		Socket.OnListenerStarted += OnListenerStarted;
		text.text = Socket.StartServer();
	}
	public void StartClient()
	{
		Socket.OnConnected += SwitchScene;
		if (input.text == "")
		{
			Socket.StartProbe();
		}
		else
		{
			Socket.StartClient(input.text.Split(':')[0], int.Parse(input.text.Split(':')[1]));
		}
	}
	public void SwitchScene()
	{
		Socket.OnConnected -= SwitchScene;
		SceneManager.LoadScene("SampleScene");
	}
	private void Update()
	{
		Socket.Update();
		if (!text.text.Equals(textField))
		{
			text.text = textField;
		}
	}
	public void OnListenerStarted(string s,int p)
	{
		textField = s + ":" + p.ToString();
	}
}
