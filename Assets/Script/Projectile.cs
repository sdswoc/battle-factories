using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour 
{
	public float speed;
	public int damage;
	public float heightGain;
	public AnimationCurve curveHorizontal;
	public AnimationCurve curveVerical;
	private new Transform transform;
	public static List<Projectile> list = new List<Projectile>();
	public void Launch(Vector2 from,Vector2 to)
	{
		transform = GetComponent<Transform>();
		StartCoroutine(Animate(from, to));
	}
	private IEnumerator Animate(Vector2 from,Vector2 to)
	{
		float distance = (to - from).magnitude;
		list.Add(this);
		transform.eulerAngles = new Vector3(0,0,Mathf.Atan2(to.y - from.y, to.x - from.x) * Mathf.Rad2Deg);
		if (distance > 0)
		{
			for (float i = 0;i < distance;i += speed*Time.deltaTime)
			{
				transform.position = (Vector3)Vector2.Lerp(from, to, curveHorizontal.Evaluate(i / distance))-Vector3.forward*curveVerical.Evaluate(i/distance)*heightGain;
				yield return new WaitForEndOfFrame();
			}
		}
		GameFlow.billboardManager.Spawn(damage.ToString(), to);
		list.Remove(this);
		SimplePool.Despawn(gameObject);
	}
}
