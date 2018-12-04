using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour 
{
	public float viewRadius;
	public float movementRadius;
	public float selectionRadius;
	public int movementBlock;
	public UnitType type;
	public Vector2Int position;
	private new Transform transform;
	public static List<Unit> units = new List<Unit>();

	private void Awake()
	{
		transform = GetComponent<Transform>();
		position = new Vector2Int(Mathf.RoundToInt(transform.position.x),Mathf.RoundToInt(transform.position.y));
		transform.position = (Vector2)position;
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
	
	public void Spawn(Vector2Int position,UnitType type)
	{
		this.position = position;
		this.type = type;
		transform.position = (Vector2)position;
		units.Add(this);
	}
	
	public void Despawn()
	{
		units.Remove(this);
	}
	
	public void Move(Vector2Int position)
	{
		this.position = position;
		transform.position = (Vector2)position;
	}
}

public enum UnitType
{
	Friendly,Enemy
}