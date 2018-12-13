using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour 
{
	public float duration;
	public float floatDistance;
	public float scaleRelativeToCamera;
	private Transform cameraTransform;
	private new Transform transform;
	private TextMesh tMesh;

	public void Spawn(string text,Vector2 position)
	{
		tMesh = GetComponent<TextMesh>();
		transform = GetComponent<Transform>();
		tMesh.text = (text);
		transform.rotation = GameFlow.cameraTransform.rotation;
		StartCoroutine(Animate(position));
	}
	private IEnumerator Animate(Vector2 position)
	{
		for (float i = 0;i < duration;i+= Time.deltaTime)
		{
			transform.position = Vector3.Lerp(position, (Vector3)position - Vector3.forward * floatDistance, i / duration);
			transform.localScale = Vector3.one * GameFlow.camera.orthographicSize * scaleRelativeToCamera;
			yield return new WaitForEndOfFrame();
		}
		Destroy(gameObject);
	}
}
