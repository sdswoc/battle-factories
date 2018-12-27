using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankUnit : Troop 
{
	public Transform turret;
	public Transform body;
	public Transform barrel;
	public Transform barrelTip;
	public float aimTime;
	public float fireTime;
	public float turretRecoil;
	public float fallDepth;
	public float fallSpeed;
	public AnimationCurve recoilCurve;
	public GameObject projectile;
    public ParticleSystem system;
    public int emitCount;

    private float turretRotation;
	private float bodyRotation;
	private float turretRotationVelocity;
	private float bodyRotationVelocity;

    public override void Spawn(Vector2Int position, UnitType type, int id)
    {
        base.Spawn(position, type, id);
        OverwriteValues();
        rangeIndicator.UpdateMesh();
    }
    public void OverwriteValues()
    {
        this.viewRadius = ValueLoader.tankRange == 0 ? this.viewRadius : ValueLoader.tankRange;
        this.movementBlock = ValueLoader.tankMobility == 0 ? this.movementBlock : ValueLoader.tankMobility;
        this.damage = ValueLoader.tankDamage == 0 ? this.damage : ValueLoader.tankDamage;
        this.hp = ValueLoader.tankHealth == 0 ? this.hp : ValueLoader.tankHealth;
        this.maxHp = this.finalHp = this.finalHp = ValueLoader.tankHealth == 0 ? this.maxHp : ValueLoader.tankHealth;
        this.fuelConsumption = ValueLoader.tankFuelUse == 0 ? this.fuelConsumption : ValueLoader.tankFuelUse;
    }
    public override void FollowPath(Vector2Int start, Vector2Int stop, float t)
	{
		base.FollowPath(start, stop, t);
		float angle = Mathf.Atan2(stop.y - start.y, stop.x - start.x)*Mathf.Rad2Deg;
		turretRotation = Mathf.SmoothDampAngle(turretRotation, angle, ref turretRotationVelocity, 0.1f);
		bodyRotation = Mathf.SmoothDampAngle(bodyRotation, angle, ref bodyRotationVelocity, 0.06f);
		transform.position = Vector3.Lerp((Vector2)start, (Vector2)stop, t);
		turret.eulerAngles = new Vector3(0, 0, turretRotation-90);
		body.eulerAngles = new Vector3(0, 0, bodyRotation-90);
	}
	public override IEnumerator AnimateAttack(Unit unit)
	{
		base.AnimateAttack(unit);
		Debug.Log("Started");
		float angle = Mathf.Atan2(unit.position.y - position.y, unit.position.x - position.x) * Mathf.Rad2Deg;
		for (float i = 0;i < aimTime;i += Time.deltaTime)
		{
			turretRotation = Mathf.LerpAngle(turretRotation, angle, i / aimTime);
			turret.eulerAngles = new Vector3(0, 0, turretRotation-90);
			yield return new WaitForEndOfFrame();
		}
		GetComponent<AudioSource>().Play();
		turret.eulerAngles = new Vector3(0, 0, angle-90);
		Projectile p = SimplePool.Spawn(projectile, Vector3.zero, Quaternion.identity).GetComponent<Projectile>();
		p.Launch(barrelTip.position, unit.position,unit);
		p.damage = damage;
        barrel.localPosition = new Vector3(barrel.localPosition.x, -recoilCurve.Evaluate(0 / fireTime) * turretRecoil, barrel.localPosition.z);
        system.Emit(emitCount);
        for (float i = 0; i < fireTime; i += Time.deltaTime)
		{
			barrel.localPosition = new Vector3(barrel.localPosition.x, -recoilCurve.Evaluate(i / fireTime)*turretRecoil, barrel.localPosition.z);
			yield return new WaitForEndOfFrame();
		}
		barrel.localPosition = new Vector3(barrel.localPosition.x, 0, barrel.localPosition.z);

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
						bool valid = true;
						for (int ii = 0;ii < GridRectangle.list.Count;ii++)
						{
						if (GridRectangle.list[ii].LineTest(position,unit.position))
							{
							if (GridRectangle.list[ii].GetComponent<Unit>() != unit)
								valid = false;
							}
						}
						if (valid)
						{
							attackList.Add(unit);
						}
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
				if ((position - attackList[i].position).sqrMagnitude < (position - u.position).sqrMagnitude)
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
        for (float i = 0;i < fallDepth;i+= Time.deltaTime*fallSpeed)
		{
			transform.position = pos + Vector3.forward * i;
			yield return new WaitForEndOfFrame();
		}
		Destroy(gameObject);
	}
}