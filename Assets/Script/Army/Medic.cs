using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;

namespace Army
{
    public class Medic : Troop
    {
        public float fireTime;
        public override void OverwriteValues()
        {
            this.viewRadius = ValueLoader.medicRange == 0 ? this.viewRadius : ValueLoader.medicRange;
            this.movementBlock = ValueLoader.medicMobility == 0 ? this.movementBlock : ValueLoader.medicMobility;
            this.damage = ValueLoader.medicDamage == 0 ? this.damage : ValueLoader.medicDamage;
            this.hp = ValueLoader.medicHealth == 0 ? this.hp : ValueLoader.medicHealth;
            this.maxHp = this.finalHp = this.hp = ValueLoader.medicHealth == 0 ? this.maxHp : ValueLoader.medicHealth;
            this.fuelConsumption = ValueLoader.medicFuelUse == 0 ? this.fuelConsumption : ValueLoader.medicFuelUse;
            rangeIndicator.UpdateMesh();
        }
        public override IEnumerator AnimateAttack(Unit unit)
        {
            Troop t = (unit as Troop);
            if ((t != null) && (t.type == UnitType.Friendly || t.visible))
            {
                base.AnimateAttack(unit);
                GameFlow.billboardManager.Spawn(damage, unit.position);
                unit.hpIndicator.UpdateMesh();
                yield return new WaitForSeconds(fireTime);
            }
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
                if (unit.unitID != unitID && unit as Troop != null && unit as Medic == null && unit.type == type && (unit.position - position).sqrMagnitude <= viewRadius * viewRadius)
                {
                    attackList.Add(unit);
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
    }
}