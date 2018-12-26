using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckUnit : Troop
{
	public Transform body;
	public Transform barrelTip;
	public float fireTime;
	public float fallDepth;
	public float fallSpeed;
	public GameObject projectile;

	private float bodyRotation;
	private float bodyRotationVelocity;
    public override void Spawn(Vector2Int position, UnitType type, int id)
    {
        base.Spawn(position, type, id);
        OverwriteValues();
        rangeIndicator.UpdateMesh();
    }
    public void OverwriteValues()
    {
        this.viewRadius = ValueLoader.truckRange == 0 ? this.viewRadius : ValueLoader.truckRange;
        this.movementBlock = ValueLoader.truckMobility == 0 ? this.movementBlock : ValueLoader.truckMobility;
        this.damage = ValueLoader.truckDamage == 0 ? this.damage : ValueLoader.truckDamage;
        this.hp = ValueLoader.truckHealth == 0 ? this.hp : ValueLoader.truckHealth;
        this.maxHp = this.finalHp = this.finalHp = ValueLoader.truckHealth == 0 ? this.maxHp : ValueLoader.truckHealth;
        this.fuelConsumption = ValueLoader.truckFuelUse == 0 ? this.fuelConsumption : ValueLoader.truckFuelUse;
    }
    public override void FollowPath(Vector2Int start, Vector2Int stop, float t)
	{
		base.FollowPath(start, stop, t);
		float angle = Mathf.Atan2(stop.y - start.y, stop.x - start.x) * Mathf.Rad2Deg;		bodyRotation = Mathf.SmoothDampAngle(bodyRotation, angle, ref bodyRotationVelocity, 0.06f);
		transform.position = Vector3.Lerp((Vector2)start, (Vector2)stop, t);
		body.eulerAngles = new Vector3(0, 0, bodyRotation - 90);
	}

	public override IEnumerator AnimateAttack(Unit unit)
	{
		base.AnimateAttack(unit);
		Debug.Log("Started");
		float angle = Mathf.Atan2(unit.position.y - position.y, unit.position.x - position.x) * Mathf.Rad2Deg;
		Projectile p = SimplePool.Spawn(projectile, Vector3.zero, Quaternion.identity).GetComponent<Projectile>();
		p.Launch(barrelTip.position, unit.position, unit);
		p.damage = damage;
		GetComponent<AudioSource>().Play();

		yield return new WaitForSeconds(fireTime);
	}
	public override void EndPath(Vector2Int position)
	{
		base.EndPath(position);
		GetComponent<Transform>().position = (Vector2)position;
	}
	public override void Despawn()
	{
		base.Despawn();
		StartCoroutine(Destroy());
	}
	public override void GenerateAttackList()
	{
		attackList.Clear();
		for (int i = 0; i < GameFlow.units.Count; i++)
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
		SortList();
	}
	public void SortList()
	{
		if (attackList.Count > 0)
		{
			Unit u = attackList[0];
			for (int i = 0; i < attackList.Count; i++)
			{
				if ((position - attackList[i].position).sqrMagnitude > (position - u.position).sqrMagnitude)
				{
					u = attackList[i];
				}
				else if ((position - attackList[i].position).sqrMagnitude == (position - u.position).sqrMagnitude)
				{
					if (attackList[i].hp < u.hp)
					{
						u = attackList[i];
					}
				}
			}
			attackList.Clear();
			attackList.Add(u);
		}
	}
	public IEnumerator Destroy()
	{
		Debug.Log("Destroy");
		Transform transform = GetComponent<Transform>();
		Vector3 pos = transform.position;
		for (float i = 0; i < fallDepth; i += Time.deltaTime * fallSpeed)
		{
			transform.position = pos + Vector3.forward * i;
			yield return new WaitForEndOfFrame();
		}
		Destroy(gameObject);
	}
}