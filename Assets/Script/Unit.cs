using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Pathfinding;

public class Unit : MonoBehaviour 
{
	public float viewRadius;
	public float selectionRadius;
	public float speed;
	public int movementBlock;
	public UnitType type;
	public Vector2Int position;
	public bool selectable;
	public int unitID;
	public int hp;
	public int damage;
	public int fuelConsumption;
	private new Transform transform;
	private List<PathNode> path;
	public static List<Unit> attackList = new List<Unit>();
	public static List<Unit> units = new List<Unit>();

	private void Awake()
	{
		transform = GetComponent<Transform>();
		position = new Vector2Int(Mathf.RoundToInt(transform.position.x),Mathf.RoundToInt(transform.position.y));
		transform.position = (Vector2)position;
		path = new List<PathNode>();
	}

	public void Spawn(Vector2Int position,UnitType type,int id)
	{
		this.position = position;
		this.type = type;
		unitID = id;
		transform.position = (Vector2)position;
		selectable = true;
		units.Add(this);
		GameFlow.map.RegisterObstacle(position);
		if (type == UnitType.Enemy)
		{
			GetComponent<MeshRenderer>().material = null;
		}
	}
	
	public void Despawn()
	{
		GameFlow.map.UnRegisterObstacle(position);
		units.Remove(this);
		Destroy(gameObject);
	}

	public void Move(List<PathNode> path)
	{
		if (path.Count > 1)
		{
			GameFlow.map.MoveUnit(path[0].position, path[path.Count - 1].position);
			position = path[path.Count - 1].position;
		}
		if (type == UnitType.Friendly)
		{
			EventHandle.SetResource(GameFlow.money, GameFlow.fuel - fuelConsumption);
		}
		this.path.Clear();
		for (int i = 0;i < path.Count;i++)
		{
			this.path.Add(path[i]);
		}
		StartCoroutine(FollowPath());
	}

	public void GenerateAttackList()
	{
		attackList.Clear();
		for (int i = 0;i < units.Count;i++)
		{
			Unit unit = units[i];
			if (unit.unitID != unitID)
			{
				if (unit.type != type)
				{
					if ((unit.position - position).sqrMagnitude <= viewRadius * viewRadius)
					{
						attackList.Add(unit);
					}
				}
			}
		}
	}

	public IEnumerator Attack()
	{
		for (int i = 0; i < attackList.Count; i++)
		{
			Unit unit = attackList[i];
			EventHandle.DamageUnit(unit, damage);
			for (float t = 0; t < 1; t += Time.deltaTime*4)
			{
				transform.position = Vector2.Lerp((Vector2)position, (Vector2)unit.position, Mathf.Sin(Mathf.PI*t));
				Debug.DrawLine((Vector2)(position), (Vector2)unit.position, Color.green);
				yield return new WaitForEndOfFrame();
			}
			transform.position = (Vector2)position;
		}
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