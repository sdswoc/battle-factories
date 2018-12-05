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
		public byte[,] potentialMap;
		private Mesh mesh;

		private void Awake()
		{
			GameFlow.map = this;
			GetComponent<MeshFilter>().mesh = GenerateMesh();
			data = new bool[width, height];
			GameFlow.aStar = new AStar(ref data, width, height);
			potentialMap = new byte[width, height];
		}

		private Mesh GenerateMesh()
		{
			Mesh m = new Mesh();
			Vector3[] vertices = new Vector3[4];
			vertices[0] = -new Vector3(1,1,0) * 0.5f;
			vertices[1] = Vector3.down * 0.5f + Vector3.right * (width - 0.5f);
			vertices[2] = Vector3.up * (height - 0.5f) + Vector3.right * (width - 0.5f);
			vertices[3] = Vector3.left * 0.5f + Vector3.up * (height - 0.5f);
			m.vertices = vertices;
			Vector2[] uvs = new Vector2[4];
			uvs[0] = Vector2.zero;
			uvs[1] = Vector2.right * (width) / cellSize;
			uvs[2] = Vector2.right * (width) / cellSize + Vector2.up * (height) / cellSize;
			uvs[3] = Vector2.up * (height) / cellSize;
			int[] triangles = new int[] { 0, 2, 1, 2, 0, 3 };
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

			for (int i = rectangle.position.x; i < rectangle.position.x+rectangle.size.x; i++)
			{
				for (int j = rectangle.position.y; j < rectangle.position.y+rectangle.size.y; j++)
				{
					if (ValidatePosition(new Vector2Int(i, j)))
					{
						data[i, j] = true;
					}
				}
			}
		}

		public void RegisterObstacle(Vector2Int pos)
		{
			if (ValidatePosition(pos))
			{
				data[pos.x, pos.y] = true;
			}
		}
		public void UnRegisterObstacle(Vector2Int pos)
		{
			if (ValidatePosition(pos))
			{
				data[pos.x, pos.y] = false;
			}
		}

		public bool GetObstacle(Vector2Int position)
		{
			if (ValidatePosition(position))
			{
				return data[position.x, position.y];
			}
			return true;			
		}

		public void MoveUnit(Vector2Int start,Vector2Int end)
		{
			if (ValidatePosition(start) && ValidatePosition(end))
			{
				data[start.x, start.y] = false;
				data[end.x, end.y] = true;
			}
		}

		private bool ValidatePosition(Vector2Int position)
		{
			if (position.x >= 0 && position.x < width)
			{
				if (position.y >= 0 && position.y < height)
				{
					return true;
				}
			}
			return false;
		}

		private void OnDrawGizmos()
		{
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					if (data != null)
					{
						if (!data[i,j])
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
					Gizmos.DrawCube(new Vector3(i, j), new Vector3(0.2f, 0.2f, 0));
				}
			}
		}
	}
}