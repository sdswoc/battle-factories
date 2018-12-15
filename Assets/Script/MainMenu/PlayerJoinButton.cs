using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeaconLib;
using UnityEngine.UI;
using Multiplayer;
public class PlayerJoinButton : MonoBehaviour 
{
	public BeaconLocation location;
	public Text text;

	public void Initialise(BeaconLocation loc)
	{
		location = loc;
		text.text = location.Data + "@" + location.Address.Address.ToString() +":"+ location.Address.Port.ToString();
	}
	public void OnJoinCallback()
	{
		Socket.StartClient(location.Address.Address.ToString(), location.Address.Port);
	}
}
