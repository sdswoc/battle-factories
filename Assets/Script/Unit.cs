using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HUD;
public class Unit : MonoBehaviour 
{
	public float viewRadius;
	public int unitID;
	public UnitType type;
	public int hp;
	public int maxHp;
	public int finalHp;
	public Vector2Int position;
	public Vector2 mainPosition;
	public HealthIndicator hpIndicator;
	public RangeIndicator rangeIndicator;

	public virtual void Spawn(Vector2Int position, UnitType type, int id)
	{
		this.position = position;
		this.type = type;
		unitID = id;
		hp = maxHp;
		finalHp = maxHp;
		GetComponent<Transform>().position = (Vector2)position;
		hpIndicator.UpdateMesh();
		GameFlow.units.Add(this);
		GameFlow.map.RegisterObstacle(position);
		rangeIndicator.UpdateMaterial();
		mainPosition = position;
	}
	public virtual void Despawn()
	{
		GameFlow.map.UnRegisterObstacle(position);
		GameFlow.units.Remove(this);
	}
}
