using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
	public class Map : MonoBehaviour
	{
		public int width;
		public int height;
		public int cellSize;
		public bool[,] data;
		private Mesh mesh;

		private void Awake()
		{
			GameFlow.map = this;
			GetComponent<MeshFilter>().mesh = GenerateMesh();
			data = new bool[width, height];
		}
		
		private Mesh GenerateMesh()
		{
			Mesh m = new Mesh();
			Vector3[] vertices = new Vector3[4];
			vertices[0] = -Vector3.one * 0.5f;
			vertices[1] = Vector3.down * 0.5f + Vector3.right * (width - 0.5f);
			vertices[2] = Vector3.up * (height - 0.5f) + Vector3.right * (width - 0.5f);
			vertices[3] = Vector3.left * 0.5f + Vector3.up * (height - 0.5f);
			m.vertices = vertices;
			Vector2[] uvs = new Vector2[4];
			uvs[0] = Vector2.zero;
			uvs[1] = Vector2.right * (width) / cellSize;
			uvs[2] = Vector2.right * (width) / cellSize + Vector2.up * (height) / cellSize;
			uvs[3] = Vector2.up * (height) / cellSize;
			int[] triangles = new int[] { 0, 1, 2, 0, 2, 3 };
			m.vertices = vertices;
			m.uv = uvs;
			m.triangles = triangles;
			m.RecalculateNormals();
			m.RecalculateBounds();
			return m;
		}
		
		public float GetCameraWidth()
		{
			return width - 1;
		}
		
		public float GetCameraHeight()
		{
			return height - 1;
		}

		public void RegisterObstacle(GridRectangle rectangle)
		{
			for (int i = rectangle.position.x; i < rectangle.size.x;i++)
			{
				for (int j = rectangle.position.y;j < rectangle.size.y;j++)
				{
					if (i >= 0 && j < width)
					{
						if (j >= 0 && j < height)
						{
							data[i, j] = true;
						}
					}
				}
			}
		}
		
		private void OnDrawGizmos()
		{
			
			{
				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						if (data != null)
						{
							if (!data[i, j])
							{
								Gizmos.color = Color.cyan;
							}
							else
							{
								Gizmos.color = Color.red;
							}
						}
						else
						{
							Gizmos.color = Color.cyan;
						}
						Gizmos.DrawCube(new Vector3(i, j), new Vector3(0.8f, 0.8f, 0));
					}
				}
			}
		}
	}
}