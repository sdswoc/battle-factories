using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;

namespace GameElements
{
    public class Flag : MonoBehaviour
    {
        public MeshFilter circleFilter;
        public float stripWidth;
        public int detail;
        public Transform pivotTransform;
        public float pivotHeight;
        public Vector2Int position;
        public GameObject flagGreen;
        public GameObject flagRed;
        public GameObject flagWhite;
        public ParticleSystem system;
        private Mesh circleMesh;
        private float pivotVelocity;
        private float pivotPosition;
        private void Awake()
        {
            circleMesh = new Mesh();
            circleFilter.mesh = circleMesh;
            GameFlow.flag = this;
            transform.position = (Vector2)position;
        }
        private void Start()
        {
            GameFlow.GenerateCircleMesh(circleMesh, ValueLoader.flagRange, ValueLoader.flagRange + stripWidth, detail);
        }
        private void Update()
        {
            pivotPosition = Mathf.SmoothDamp(pivotPosition, (Mathf.Abs((float)GameFlow.flagCapture / ValueLoader.flagCaptureTurnLimit) * pivotHeight), ref pivotVelocity, 1f);
            pivotTransform.localPosition = -Vector3.forward * pivotPosition;
            if (GameFlow.flagCapture == 0)
            {
                flagGreen.SetActive(false);
                flagRed.SetActive(false);
                flagWhite.SetActive(true);
            }
            else if (GameFlow.flagCapture > 0)
            {
                flagGreen.SetActive(true);
                flagRed.SetActive(false);
                flagWhite.SetActive(false);
            }
            else
            {
                flagGreen.SetActive(false);
                flagRed.SetActive(true);
                flagWhite.SetActive(false);
            }
        }
        public void Play()
        {
            system.Play();
        }
    }
}