using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFactoryButtonFitter : MonoBehaviour 
{
	private new RectTransform transform;
	private void Awake()
	{
		transform = GetComponent<RectTransform>();
	}
	private void Update()
	{
		float size = transform.rect.height;
		for (int i = 0;i < transform.childCount;i++)
		{
			RectTransform child = (RectTransform)transform.GetChild(i);
			child.anchorMax = Vector2.up;
			child.anchorMin = Vector2.zero;
			child.offsetMax = new Vector2(size * (i+1), 0);
			child.offsetMin = new Vector2(size * (i), 0);
		}
	}
}
