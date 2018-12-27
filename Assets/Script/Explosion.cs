using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour 
{
    public int emitCount;
	public float destroyTime;
    public ParticleSystem system;

    private void Awake()
    {
        Trigger();
    }
    public void Trigger()
	{
		StopAllCoroutines();
		StartCoroutine(Coroutine());
		GetComponent<AudioSource>().Play();
        system.Emit(emitCount);
	}

	IEnumerator Coroutine()
	{
        yield return new WaitForSeconds(destroyTime);
		SimplePool.Despawn(gameObject);
	}
}
