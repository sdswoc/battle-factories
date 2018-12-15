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
			Socket.OnConnected += ConnectEvent;
		}
		private void OnDestroy()
		{
			Socket.OnData -= DataEvent;
			Socket.OnDisconnected -= DisconnectEvent;
			Socket.OnConnected -= ConnectEvent;
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
			StartCoroutine(DisconnectCrorutine());
			//Application.Quit();
		}
		private void ConnectEvent()
		{
			NetworkEventSend.NameSync(GameFlow.friendlyName ?? "Unnamed Player");
		}
		private IEnumerator DisconnectCrorutine()
		{
			yield return new WaitForEndOfFrame();
			if (!GameFlow.finishPanel.showing)
			{
				GameFlow.timer.CloseTime();
				GameFlow.fireIndicator.CloseFire();
				GameFlow.cameraInput.active = false;
				GameFlow.disconnectPanel.gameObject.SetActive(true);
				GameFlow.SetMode(Control.UIMode.None);
				yield return StartCoroutine(GameFlow.disconnectPanel.Show());
				GameFlow.uiCurtain.gameObject.SetActive(true);
				yield return StartCoroutine(GameFlow.uiCurtain.Close());
				EventHandle.GoToMainMenu();
			}
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