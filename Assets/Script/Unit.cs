using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Pathfinding;

public class Unit : MonoBehaviour 
{
	public float viewRadius;
	public float movementRadius;
	public float selectionRadius;
	public float speed;
	public int movementBlock;
	public UnitType type;
	public Vector2Int position;
	public bool selectable;
	public int unitID;
	private new Transform transform;
	private List<PathNode> path;
	public static List<Unit> units = new List<Unit>();
	private static int idGenerator;

	private void Awake()
	{
		transform = GetComponent<Transform>();
		position = new Vector2Int(Mathf.RoundToInt(transform.position.x),Mathf.RoundToInt(transform.position.y));
		transform.position = (Vector2)position;
		path = new List<PathNode>();
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
		GameFlow.map.RegisterObstacle(position);
	}
	
	public void Despawn()
	{
		GameFlow.map.UnRegisterObstacle(position);
		units.Remove(this);
	}
	
	public void Move(Vector2Int position)
	{
		this.position = position;
		transform.position = (Vector2)position;
	}

	public void Move(List<PathNode> path)
	{
		if (path.Count > 1)
		{
			GameFlow.map.MoveUnit(path[0].position, path[path.Count - 1].position);
			position = path[path.Count - 1].position;
		}
		this.path.Clear();
		for (int i = 0;i < path.Count;i++)
		{
			this.path.Add(path[i]);
		}
		StartCoroutine(FollowPath());
	}

	private IEnumerator FollowPath()
	{
		if (path.Count > 1)
		{
			int index = 0;
			selectable = false;
			for (float i = 0;i < path[path.Count-1].block;i += Time.deltaTime*speed)
			{
				while (path[index + 1].block < i)
				{
					index++;
				}
				transform.position = Vector2.Lerp(path[index].position, path[index + 1].position, (i - path[index].block) / (float)(path[index + 1].block - path[index].block));
				yield return new WaitForEndOfFrame();	
			}
			transform.position = (Vector2)path[path.Count - 1].position;
			selectable = true;
		}
	}
}

public enum UnitType
{
	Friendly,Enemy
}