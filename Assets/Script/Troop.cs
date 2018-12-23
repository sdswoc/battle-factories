using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Pathfinding;
using HUD;

public class Troop : Unit
{
	public GameObject baseObject;
	public float selectionRadius;
	public float speed;
	public int movementBlock;
	public bool selectable;
	public int damage;
	public int fuelConsumption;
	public bool visible;


	private List<PathNode> path = new List<PathNode>();
	private new Transform transform;
	public List<Unit> attackList = new List<Unit>();

	private void OnDestroy()
	{
		GameFlow.units.Remove(this);
	}

	private void Update()
	{
		mainPosition = transform.position;
		if (type == UnitType.Enemy)
		{
			Vector2 positionF = transform.position;
			visible = false;
			for (int i = 0; i < GameFlow.units.Count; i++)
			{
				Unit u = GameFlow.units[i];
				if (u.type == UnitType.Friendly)
				{
					if (((Vector2)u.mainPosition - positionF).sqrMagnitude <= u.viewRadius * u.viewRadius)
					{
						visible = true;
						break;
					}
				}
			}
			if (baseObject.activeInHierarchy != visible)
			{
				baseObject.SetActive(visible);
			}
		}
	}

	public override void Spawn(Vector2Int position,UnitType type,int id)
	{
		base.Spawn(position, type, id);
		selectable = true;
		path = new List<PathNode>();
		rangeIndicator.UpdateMaterial();
		transform = GetComponent<Transform>();
	}
	
	public override void Despawn()
	{
		base.Despawn();
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

	public virtual void GenerateAttackList()
	{
		attackList.Clear();
		for (int i = 0;i < GameFlow.units.Count;i++)
		{
			Unit unit = GameFlow.units[i];
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

	public virtual IEnumerator AnimateAttack(Unit unit)
	{
		yield return new WaitForEndOfFrame();
	}

	public virtual void FollowPath(Vector2Int start,Vector2Int stop,float t)
	{
		
	}

	public virtual void StartPath(Vector2Int position)
	{
		selectable = false;
	}

	public virtual void EndPath(Vector2Int position)
	{
		selectable = true;
	}

	public  virtual IEnumerator Attack()
	{
		for (int i = 0; i < attackList.Count; i++)
		{
			Unit unit = attackList[i];
			EventHandle.DamageUnit(unit, damage);
			yield return StartCoroutine(AnimateAttack(unit));
		}
	}

	private IEnumerator FollowPath()
	{
		if (path.Count > 1)
		{
			Debug.Log("Moving");
			int index = 0;
			StartPath(path[0].position);
			for (float i = 0;i < path[path.Count-1].block;i += Time.deltaTime*speed)
			{
				while (path[index + 1].block < i)
				{
					index++;
				}
				FollowPath(path[index].position, path[index + 1].position, (i - path[index].block) / (float)(path[index + 1].block - path[index].block));
				//transform.position = Vector2.Lerp(path[index].position, path[index + 1].position, (i - path[index].block) / (float)(path[index + 1].block - path[index].block));
				yield return new WaitForEndOfFrame();	
			}
			EndPath(path[path.Count - 1].position);
			selectable = true;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere((Vector2)position, selectionRadius);
	}
}

public enum UnitType
{
	Friendly,Enemy
}