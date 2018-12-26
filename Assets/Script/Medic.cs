using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medic : Troop
{
	public Transform body;
	public float fireTime;
	public float fallDepth;
	public float fallSpeed;

	private float bodyRotation;
	private float bodyRotationVelocity;
    public override void Spawn(Vector2Int position, UnitType type, int id)
    {
        base.Spawn(position, type, id);
        OverwriteValues();
    }
    public void OverwriteValues()
    {
        this.viewRadius = ValueLoader.medicRange == 0 ? this.viewRadius : ValueLoader.medicRange;
        this.movementBlock = ValueLoader.medicMobility == 0 ? this.movementBlock : ValueLoader.medicMobility;
        this.damage = ValueLoader.medicDamage == 0 ? this.damage : ValueLoader.medicDamage;
        this.hp = ValueLoader.medicHealth == 0 ? this.hp : ValueLoader.medicHealth;
        this.maxHp = this.finalHp = this.finalHp = ValueLoader.medicHealth == 0 ? this.maxHp : ValueLoader.medicHealth;
        this.fuelConsumption = ValueLoader.medicFuelUse == 0 ? this.fuelConsumption : ValueLoader.medicFuelUse;
        rangeIndicator.UpdateMesh();
    }
    public override void FollowPath(Vector2Int start, Vector2Int stop, float t)
	{
		base.FollowPath(start, stop, t);
		float angle = Mathf.Atan2(stop.y - start.y, stop.x - start.x) * Mathf.Rad2Deg; bodyRotation = Mathf.SmoothDampAngle(bodyRotation, angle, ref bodyRotationVelocity, 0.06f);
		transform.position = Vector3.Lerp((Vector2)start, (Vector2)stop, t);
		body.eulerAngles = new Vector3(0, 0, bodyRotation - 90);
	}
	public override IEnumerator AnimateAttack(Unit unit)
	{
		Troop t = (unit as Troop);
		if( (t != null) && (t.type == UnitType.Friendly || t.visible))
		{
			base.AnimateAttack(unit);
			GameFlow.billboardManager.Spawn(damage, unit.position);
			unit.hpIndicator.UpdateMesh(); 
			yield return new WaitForSeconds(fireTime);
		}
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
			if (unit as Troop != null && unit as Medic == null)
				if (unit.type == type)
				{
					if ((unit.position - position).sqrMagnitude <= viewRadius * viewRadius)
					{
						attackList.Add(unit);
					}
				}
			}
		}
	}
	public override IEnumerator Attack()
	{
		if (attackList.Count > 0)
		{
			GetComponent<AudioSource>().Play();

			for (int i = 0; i < attackList.Count; i++)
			{
				Unit unit = attackList[i];
				EventHandle.DamageUnit(unit, damage);
				yield return StartCoroutine(AnimateAttack(unit));
			}
			GetComponent<AudioSource>().Stop();
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