using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Multiplayer;

public class Test : MonoBehaviour 
{
	private void Awake()
	{
		
	}
	private void Update()
	{
		bool test = false;
		for (int i = 0;i < GridRectangle.list.Count;i++)
		{
			if (GridRectangle.list[i].LineTest(Vector2Int.zero,new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y))))
			{
				test = true;
			}
		}
		Debug.DrawLine(Vector2.zero, new Vector2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y)), test ? Color.red : Color.cyan);
	}
}
