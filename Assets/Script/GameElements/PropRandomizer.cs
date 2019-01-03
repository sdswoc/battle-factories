using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameElements
{
    public class PropRandomizer : MonoBehaviour
    {
        public float sizeMin;
        public float sizeMax;
        public float angleMin;
        public float angleMax;

        private void Awake()
        {
            Transform t = GetComponent<Transform>();
            t.localScale = Vector3.one * Random.Range(sizeMin, sizeMax);
            t.eulerAngles = new Vector3(0, 0, Random.Range(angleMin, angleMax));
        }
    }
}
