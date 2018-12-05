using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandle : MonoBehaviour 
{
	private void Awake()
	{
		GameFlow.eventHandle = this;
	}
	/*
	public void MoveUnit(int unitId,List<PathNode> path)
	{
		Unit unit;
		for (int i = 0;i < Unit.units.Count;i++)
		{
		}
	}*/
}
