using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Army;
using GameElements;
using Singleton;

namespace Weapon
{
    public class Projectile : MonoBehaviour
    {
        public GameObject explosion;
        public float speed;
        public GameObject baseObject;
        public int damage;
        private new Transform transform;

        public void SetVisibility()
        {
            Vector2 positionF = transform.position;
            bool visible = false;
            for (int i = 0; i < GameFlow.units.Count; i++)
            {
                Unit u = GameFlow.units[i];
                if (u.type == UnitType.Friendly)
                {
                    if ((u.position - positionF).sqrMagnitude <= u.viewRadius * u.viewRadius)
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
        public void Launch(Vector2 from, Vector2 to, Unit unit)
        {
            transform = GetComponent<Transform>();
            StartCoroutine(Animate(from, to, unit));
        }
        protected virtual IEnumerator Animate(Vector2 from, Vector2 to, Unit unit)
        {
            float distance = (to - from).magnitude;
            GameFlow.projectiles.Add(this);
            transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(to.y - from.y, to.x - from.x) * Mathf.Rad2Deg);
            if (distance > 0)
            {
                for (float i = 0; i < distance; i += speed * Time.deltaTime)
                {
                    SetVisibility();
                    transform.position = (Vector3)Vector2.Lerp(from, to, (i / distance));
                    yield return new WaitForEndOfFrame();
                }
            }
            GameFlow.billboardManager.Spawn(damage, to);
            GameFlow.projectiles.Remove(this);
            unit.hpIndicator.UpdateMesh();
            SimplePool.Despawn(gameObject);
            SimplePool.Spawn(explosion, to, Quaternion.identity).GetComponent<Explosion>().Trigger();
        }
    }
}