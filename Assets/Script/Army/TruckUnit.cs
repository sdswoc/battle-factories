using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon;
using Singleton;

namespace Army
{
    public class TruckUnit : Troop
    {
        public Transform barrelTip;
        public float fireTime;
        public GameObject projectile;

        public override void OverwriteValues()
        {
            this.viewRadius = ValueLoader.truckRange == 0 ? this.viewRadius : ValueLoader.truckRange;
            this.movementBlock = ValueLoader.truckMobility == 0 ? this.movementBlock : ValueLoader.truckMobility;
            this.damage = ValueLoader.truckDamage == 0 ? this.damage : ValueLoader.truckDamage;
            this.hp = ValueLoader.truckHealth == 0 ? this.hp : ValueLoader.truckHealth;
            this.maxHp = this.finalHp = this.hp = ValueLoader.truckHealth == 0 ? this.maxHp : ValueLoader.truckHealth;
            this.fuelConsumption = ValueLoader.truckFuelUse == 0 ? this.fuelConsumption : ValueLoader.truckFuelUse;
        }
        public override IEnumerator AnimateAttack(Unit unit)
        {
            base.AnimateAttack(unit);
            Projectile p = SimplePool.Spawn(projectile, Vector3.zero, Quaternion.identity).GetComponent<Projectile>();
            p.Launch(barrelTip.position, unit.position, unit);
            p.damage = damage;
            GetComponent<AudioSource>().Play();

            yield return new WaitForSeconds(fireTime);
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
    }
}