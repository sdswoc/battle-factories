using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int width;
    public int height;
    public float gridScale;
    private bool[,] data;
    private void Awake()
    {
        data = new bool[width,height];

    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(new Vector3(width / 2*gridScale, height / 2*gridScale), new Vector3(width, height)*gridScale);
        if (data != null)
        {
            for (int i = 0;i < width;i++)
            {
                for (int j = 0;j < height;j++)
                {
                    if (data[i,j])
                    {
                        Gizmos.color = Color.red;
                    }
                    else
                    {
                        Gizmos.color = Color.white;
                    }
                    Gizmos.DrawWireCube(new Vector3(i, j)*gridScale, Vector3.one * 0.5f*gridScale);
                }
            }
        }
    }
}
