using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardManager : MonoBehaviour 
{
	public GameObject billboardObject;

	private void Awake()
	{
		GameFlow.billboardManager = this;
		SimplePool.Preload(billboardObject, 5);
	}
	public void Spawn(string s,Vector2 position)
	{
	SimplePool.Spawn(billboardObject,Vector3.zero,Quaternion.identity).GetComponent<Billboard>().Spawn(s, position);
	}
}
