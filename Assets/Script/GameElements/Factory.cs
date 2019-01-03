using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Multiplayer;
using Army;
using Singleton;

namespace GameElements
{
    public class Factory : MonoBehaviour
    {
        public GameObject container;
        public GridRectangle rectangle;
        public Vector2Int position;
        public UnitType type;
        public Vector2Int flagPosition;
        public GameObject[] unitObjects;
        public bool established;
        public CloudTrigger cloudTrigger;
        public float fallHeight;
        public float fallTime;
        public float rangeIndicatorCloseTime;
        public AnimationCurve rangeIndicatorCloseAnimation;
        private int idGeneratorIndex;
        private bool visible = false;

        private void Start()
        {
            OverwriteValues();
            GetComponent<Unit>().rangeIndicator.UpdateMesh();
        }
        private void OverwriteValues()
        {
            Unit u = GetComponent<Unit>();
            u.viewRadius = ValueLoader.factoryRange == 0 ? u.viewRadius : ValueLoader.factoryRange;
            u.maxHp = u.finalHp = u.hp = ValueLoader.factoryHealth == 0 ? u.maxHp : ValueLoader.factoryHealth;
        }
        private IEnumerator WrenchSoundCoroutine()
        {
            GetComponent<AudioSource>().Play();
            yield return new WaitForSeconds(4);
            GetComponent<AudioSource>().Stop();
        }
        private IEnumerator VisibilityCheckCoroutine()
        {
            while (!visible && established && type == UnitType.Enemy)
            {
                for (int i = 0; i < GameFlow.units.Count; i++)
                {
                    Unit u = GameFlow.units[i];
                    if (u.type == UnitType.Friendly)
                    {
                        if ((u.mainPosition - position).sqrMagnitude <= u.viewRadius * u.viewRadius)
                        {
                            visible = true;
                            break;
                        }
                    }
                }
                if (visible)
                {
                    container.SetActive(visible);
                    cloudTrigger.Show();
                }
                yield return new WaitForSeconds(0.2f);
            }
        }
        private int GetID()
        {
            idGeneratorIndex++;
            return ((int)Socket.socketType | (idGeneratorIndex++ << 1));
        }
        public void MoveToPosition(Vector2Int position)
        {
            if (!established)
            {
                this.position = position;
                rectangle.position = position - new Vector2Int((int)(rectangle.size.x * 0.5f), (int)(rectangle.size.y * 0.5f));
                rectangle.Setup();
                GetComponent<Unit>().mainPosition = position;
            }
        }
        public bool EvaluatePosition()
        {
            if (GameFlow.flag != null)
            {
                if ((position - GameFlow.flag.position).sqrMagnitude <= ValueLoader.flagBoundaryRange * ValueLoader.flagBoundaryRange)
                {
                    return false;
                }
            }
            for (int i = rectangle.position.x; i < rectangle.position.x + rectangle.size.x; i++)
            {
                for (int j = rectangle.position.y; j < rectangle.position.y + rectangle.size.y; j++)
                {
                    if (GameFlow.map.GetObstacle(new Vector2Int(i, j)))
                    {
                        return false;
                    }
                }
            }
            if (Socket.socketType == SocketType.Server)
            {
                if (rectangle.position.y <= 0 || rectangle.position.y + rectangle.size.y > GameFlow.map.height / 2)
                {
                    return false;
                }
            }
            else
            {
                if (rectangle.position.y <= GameFlow.map.height / 2 || rectangle.position.y + rectangle.size.y > GameFlow.map.height)
                {
                    return false;
                }
            }
            return true;
        }
        public void Setup(Vector2Int position)
        {
            GetComponent<Unit>().Spawn(position, type, GetID());
            MoveToPosition(position);
            GameFlow.map.RegisterObstacle(rectangle);
            established = true;
            cloudTrigger.Show();
            if (type == UnitType.Enemy)
            {
                container.SetActive(false);
            }
            GetComponent<Unit>().mainPosition = position;
            StartCoroutine(WrenchSoundCoroutine());
            StartCoroutine(VisibilityCheckCoroutine());
        }
        public Vector2Int GetNearestEmptyLocation()
        {
            float distance = float.MaxValue;
            Vector2Int samplePosition = new Vector2Int(0, 0);
            Vector2Int position = rectangle.position + new Vector2Int((int)(rectangle.size.x * 0.5f), (int)(rectangle.size.y * 0.5f));
            for (int i = 0; i < GameFlow.map.width; i++)
            {
                for (int j = 0; j < GameFlow.map.height; j++)
                {
                    if (!GameFlow.map.GetObstacle(new Vector2Int(i, j)) && new Vector2(i, j) != position)
                    {
                        if ((position - new Vector2Int(i, j)).sqrMagnitude < distance)
                        {
                            samplePosition.x = i;
                            samplePosition.y = j;
                            distance = (position - new Vector2Int(i, j)).sqrMagnitude;
                        }
                    }
                }
            }
            return samplePosition;
        }
        public Troop CreateUnit(byte unitIndex)
        {
            return CreateUnit(GetNearestEmptyLocation(), unitIndex, GetID());
        }
        public Troop CreateUnit(Vector2Int postion, byte unitIndex, int id)
        {
            Troop u = Instantiate(unitObjects[unitIndex % unitObjects.Length]).GetComponent<Troop>();
            u.Spawn(postion, type, id);
            return u;
        }
        public IEnumerator DespawnCoroutine()
        {
            Vector3 pos = transform.position;
            Unit unit = GetComponent<Unit>();
            unit.hpIndicator.gameObject.SetActive(false);
            for (float i = 0; i < rangeIndicatorCloseTime; i += Time.deltaTime)
            {
                unit.rangeIndicator.transform.localScale = Vector3.one * rangeIndicatorCloseAnimation.Evaluate(i / rangeIndicatorCloseTime);
                yield return new WaitForEndOfFrame();
            }
            for (float i = 0; i < fallTime; i += Time.deltaTime)
            {
                transform.position = pos + Vector3.forward * (i / fallTime) * fallHeight;
                yield return new WaitForEndOfFrame();
            }
            yield return null;
        }
    }
}