using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;

namespace HUD
{
    public class BillboardManager : MonoBehaviour
    {
        public GameObject billboardObject;
        public Color healColor;
        public Color damageColor;

        private void Awake()
        {
            GameFlow.billboardManager = this;
            SimplePool.Preload(billboardObject, 5);
        }
        public void Spawn(int damage, Vector2 position)
        {
            SimplePool.Spawn(billboardObject, Vector3.zero, Quaternion.identity).GetComponent<Billboard>().Spawn(Mathf.Abs(damage).ToString(), position, (damage > 0) ? healColor : damageColor);
        }
    }
}