using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour 
{
	public float destroyTime;

	public void Trigger()
	{
		StopAllCoroutines();
		StartCoroutine(Coroutine());
		GetComponent<AudioSource>().Play();
	}

	IEnumerator Coroutine()
	{
		yield return new WaitForSeconds(destroyTime);
		SimplePool.Despawn(gameObject);
	}
}
