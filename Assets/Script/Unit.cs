using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour 
{
	public float viewRadius;
	public float movementRadius;
	public float selectionRadius;
	public UnitType type;
	public Vector2 position;
	private new Transform transform;
	public static List<Unit> units = new List<Unit>();

	private void Awake()
	{
		transform = GetComponent<Transform>();
		position = transform.position;
		Spawn(position, type);
	}
	
	private void OnDrawGizmos()
	{
		if (type == UnitType.Friendly)
			Gizmos.color = Color.green;
		else
			Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(GetComponent<Transform>().position, selectionRadius);
	}
	
	public void Spawn(Vector2 position,UnitType type)
	{
		this.position = position;
		this.type = type;
		transform.position = position;
		units.Add(this);
	}
	
	public void Despawn()
	{
		units.Remove(this);
	}
	
	public void Move(Vector2 position)
	{
		this.position = position;
		transform.position = position;
	}
}

public enum UnitType
{
	Friendly,Enemy
}