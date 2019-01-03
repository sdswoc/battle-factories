using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Army;
using GameElements;
using Singleton;

namespace Weapon
{
    public class Missile : Projectile
    {
        public float heightGain;
        public AnimationCurve heightCurve;
        public float verticalTime;
        public TrailRenderer trail;

        protected override IEnumerator Animate(Vector2 from, Vector2 to, Unit unit)
        {
            Vector3 pos = from;
            transform.position = pos;
            trail.emitting = true;
            transform.eulerAngles = new Vector3(180, 0, 0);
            GameFlow.projectiles.Add(this);
            for (float i = 0; i < verticalTime; i += Time.deltaTime)
            {
                SetVisibility();
                transform.position = pos - Vector3.forward * heightGain * heightCurve.Evaluate(i / verticalTime);
                yield return new WaitForEndOfFrame();
            }
            pos -= Vector3.forward * heightGain;
            float distance = (pos - (Vector3)from).magnitude;
            transform.LookAt((Vector2)unit.position, -Vector3.forward);
            if (distance > 0)
            {
                for (float i = 0; i < distance; i += speed * Time.deltaTime)
                {
                    SetVisibility();
                    transform.position = Vector3.Lerp(pos, to, (i / distance));
                    yield return new WaitForEndOfFrame();
                }
            }
            GameFlow.billboardManager.Spawn(damage, to);
            GameFlow.projectiles.Remove(this);
            unit.hpIndicator.UpdateMesh();
            trail.emitting = false;
            SimplePool.Despawn(gameObject);
            SimplePool.Spawn(explosion, to, Quaternion.identity).GetComponent<Explosion>().Trigger();
        }
    }
}