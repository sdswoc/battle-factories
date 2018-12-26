using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Multiplayer;
using System;

public class MenuScript : MonoBehaviour 
{
	public MenuPanelSwitcher switcher;
	public InputField nameInputField;
	public Text ipText;
	public InputField ipInput;
	public string name;
	public string ip;
	public GameObject playerListPrefab;
	public RectTransform contentTransform;
	private bool tempSwitch;
	private bool beaconListDirty;
	private void Awake()
	{
        Application.targetFrameRate = 60;
		name = GameFlow.friendlyName = PlayerPrefs.GetString("name", "Unnamed Player");
		nameInputField.text = name;
		tempSwitch = false;
		Socket.OnBeaconListUpdated += OnBeaconListUpdate;
		Socket.StartProbe();
	}
	private void OnDestroy()
	{
		Socket.OnBeaconListUpdated -= OnBeaconListUpdate;
	}
	public void HostButtonCallback()
	{
		UpdateName();
		Socket.OnListenerStarted -= OnListenerStarted;
		Socket.OnListenerStarted += OnListenerStarted;
		Socket.Reset();
		Socket.playerName = name;
		Socket.StartServer();
	}
	public void UpdateName()
	{
		name = (nameInputField.text.Length == 0) ? name : nameInputField.text;
		nameInputField.text = name;
		PlayerPrefs.SetString("name", name);
		GameFlow.friendlyName = name;
	}
	public void ClientButtonCallback()
	{
		UpdateName();
	}
	public void ErrorOKButtonCallback()
	{

	}
	public void CancelButtonCallBack()
	{
		Socket.Reset();
	}
	public void OnListenerStarted(string address,int port)
	{
		Socket.OnListenerStarted -= OnListenerStarted;
		ip = (address.Equals("noip")) ? "No network interface" : (address + ":" + port.ToString());
		tempSwitch = true;
	}
	public void OnBeaconListUpdate()
	{
		beaconListDirty = true;
	}
	public void UpdateBeaconList()
	{
		contentTransform.offsetMin = new Vector2(0, -290 * Socket.beaconLocations.Count);
		List<Transform> child = new List<Transform>();
		for (int i = 0; i < contentTransform.childCount; i++)
		{
			child.Add(contentTransform.GetChild(i));
		}
		contentTransform.DetachChildren();
		for (int i = 0; i < child.Count; i++)
		{
			Destroy(child[i].gameObject);
		}
		for (int i = 0; i < Socket.beaconLocations.Count; i++)
		{
			PlayerJoinButton p = Instantiate(playerListPrefab).GetComponent<PlayerJoinButton>();
			p.Initialise(Socket.beaconLocations[i]);
			p.GetComponent<RectTransform>().parent = contentTransform;
			p.GetComponent<Transform>().localScale = Vector3.one;
		}
	}
	public void OnJoinButton()
	{
		string text = ipInput.text;
		text = text.Trim();
		if (text.Length == 0)
		{
			ipInput.text = "";
			return;
		}
		string[] array = text.Split(':');
		if (array.Length != 2)
		{
			ipInput.text = "";
			return;
		}
		string two = array[1];
		string one = array[0];
		int port = -1;
		try
		{
			port = int.Parse(two);
		}
		catch (Exception e)
		{
			ipInput.text = "";
			return;
		}
		string[] add = one.Split('.');
		if (add.Length != 4)
		{
			ipInput.text = "";
			return;
		}
		for (int i = 0;i < add.Length;i++)
		{
			int test = -1;
			try
			{
				test = int.Parse(add[i]);
			}
			catch (Exception e)
			{
				ipInput.text = "";
				return;
			}
			if (test < 0 || test >= 256)
			{
				Debug.Log(test);
				ipInput.text = "";
				return;
			}
		}
		Debug.Log("Safe IP");
		Socket.StartClient(one, port);

	}
	private void Update()
	{
		if (tempSwitch)
		{
			tempSwitch = false;
			ipText.text = ip;
			switcher.Switch(2);
		}
		if (beaconListDirty)
		{
			beaconListDirty = false;
			UpdateBeaconList();
		}
	}
	private void OnConnected()
	{
		
	}
}
