using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour 
{
    public float rotationSpeed;
    private new Transform transform;
    private void Awake()
    {
        transform = GetComponent<Transform>();
    }
    void Update () 
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
	}
}
