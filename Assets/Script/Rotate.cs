using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour 
{
	private void Update()
	{
		GetComponent<Transform>().Rotate(Vector3.forward * Time.deltaTime * 180);
	}
}
