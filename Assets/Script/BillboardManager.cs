using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardManager : MonoBehaviour 
{
	public GameObject billboardObject;

	private void Awake()
	{
		GameFlow.billboardManager = this;
	}
	public void Spawn(string s,Vector2 position)
	{
		Instantiate(billboardObject).GetComponent<Billboard>().Spawn(s,position);
	}
}
