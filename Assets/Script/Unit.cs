using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour 
{
	public int unitID;
	public UnitType type;
	public int hp;
	public Vector2Int position;

	public virtual void Spawn(Vector2Int position, UnitType type, int id)
	{
		this.position = position;
		this.type = type;
		unitID = id;
		GetComponent<Transform>().position = (Vector2)position;
		GameFlow.units.Add(this);
		GameFlow.map.RegisterObstacle(position);
		Debug.Log(GameFlow.units.Count);
		Debug.Log(position);
	}
	public virtual void Despawn()
	{
		GameFlow.map.UnRegisterObstacle(position);
		GameFlow.units.Remove(this);
	}
}
