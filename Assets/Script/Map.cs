using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public int width;
    public int height;

    private void Awake()
    {
        Transform self = GetComponent<Transform>();
        self.localScale = new Vector3(width, height, 1);
		self.position = new Vector3(width*0.5f, height*0.5f, 0);
    }
}
