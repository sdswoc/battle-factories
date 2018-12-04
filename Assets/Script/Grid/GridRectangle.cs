using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRectangle : MonoBehaviour
{
	public Vector2Int position;
	public Vector2Int size;

	private void Start()
	{
		GameFlow.map.RegisterObstacle(this);
	}

	private void OnDrawGizmos()
	{
		if (Application.isEditor)
		{
			transform.position = (Vector2)position;
		}
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube((Vector3)((Vector2)position + (Vector2)size * 0.5f)-Vector3.one*0.5f, new Vector3(size.x, size.y));
	}
}
