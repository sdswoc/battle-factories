using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Pathfinding;
using HUD;

namespace Army
{
    public class Troop : Unit
    {
        public Transform body;
        public GameObject baseObject;
        public float selectionRadius;
        public float speed;
        public int movementBlock;
        public bool selectable;
        public int damage;
        public int fuelConsumption;
        public bool visible;
        public float fallDepth;
        public float fallSpeed;
        public List<Unit> attackList = new List<Unit>();

        private List<PathNode> path = new List<PathNode>();
        private new Transform transform;
        private float bodyRotation;
        private float bodyRotationVelocity;
        private Coroutine followPathCoroutine;

        private void OnDestroy()
        {
            GameFlow.units.Remove(this);
        }
        private IEnumerator VisibilityCheckCoroutine()
        {
            while (true)
            {
                Check();
                yield return new WaitForSeconds(0.2f);
            }
        }
        private void Check()
        {
            if (type == UnitType.Enemy)
            {
                Vector2 positionF = transform.position;
                visible = false;
                for (int i = 0; i < GameFlow.units.Count; i++)
                {
                    Unit u = GameFlow.units[i];
                    Debug.DrawLine(u.mainPosition, positionF, Color.red, 0.2f);
                    if (u.type == UnitType.Friendly)
                    {
                        if (((Vector2)u.mainPosition - positionF).sqrMagnitude <= u.viewRadius * u.viewRadius)
                        {
                            Debug.DrawLine(u.mainPosition, positionF, Color.red, 0.2f);
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
        public override void Spawn(Vector2Int position, UnitType type, int id)
        {
            base.Spawn(position, type, id);
            selectable = true;
            path = new List<PathNode>();
            transform = GetComponent<Transform>();
            OverwriteValues();
            rangeIndicator.UpdateMaterial();
            rangeIndicator.UpdateMesh();
            StartCoroutine(VisibilityCheckCoroutine());
            Check();
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
            for (int i = 0; i < path.Count; i++)
            {
                this.path.Add(path[i]);
            }
            followPathCoroutine =  StartCoroutine(FollowPath());
        }
        public virtual void GenerateAttackList()
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
        }
        public virtual IEnumerator AnimateAttack(Unit unit)
        {
            yield return new WaitForEndOfFrame();
        }
        public virtual void FollowPath(Vector2Int start, Vector2Int stop, float t)
        {
            float angle = Mathf.Atan2(stop.y - start.y, stop.x - start.x) * Mathf.Rad2Deg; 
            bodyRotation = Mathf.SmoothDampAngle(bodyRotation, angle, ref bodyRotationVelocity, 0.06f);
            transform.position = Vector3.Lerp((Vector2)start, (Vector2)stop, t);
            body.eulerAngles = new Vector3(0, 0, bodyRotation - 90);
            mainPosition = transform.position;
        }
        public virtual void StartPath(Vector2Int position)
        {
            selectable = false;
        }
        public virtual void EndPath(Vector2Int position)
        {
            selectable = true;
            transform.position = (Vector2)position;
            mainPosition = transform.position;
            if (followPathCoroutine != null)
            {
                StopCoroutine(followPathCoroutine);
            }
            Check();
        }
        public virtual void OverwriteValues()
        {

        }
        public virtual IEnumerator Attack()
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
                int index = 0;
                StartPath(path[0].position);
                for (float i = 0; i < path[path.Count - 1].block; i += Time.deltaTime * speed)
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
        public IEnumerator Destroy()
        {
            hpIndicator.gameObject.SetActive(false);
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
    public enum UnitType
    {
        Friendly, Enemy
    }
}