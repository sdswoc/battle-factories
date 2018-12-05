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
		private List<Vector2Int> potentialPoints = new List<Vector2Int>();

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

		public void GeneratePotential(Vector2Int position, int maxPotential)
		{
			Debug.Log("Generated");
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					potentialMap[i, j] = byte.MaxValue;
				}
			}
			if (ValidatePosition(position))
			{
				SetPotential(position, 0);
			}
			potentialPoints.Clear();
			potentialPoints.Add(position);
			while (potentialPoints.Count > 0)
			{
				Vector2Int node = potentialPoints[0];
				potentialPoints.RemoveAt(0);
				if (ValidatePosition(node))
				{
					int nodeValue = GetPotential(node);
					if (nodeValue < maxPotential)
					{
						SetPointPotential(node + Vector2Int.right, (byte)(nodeValue + 1));
						SetPointPotential(node + Vector2Int.left, (byte)(nodeValue + 1));
						SetPointPotential(node + Vector2Int.up, (byte)(nodeValue + 1));
						SetPointPotential(node + Vector2Int.down, (byte)(nodeValue + 1));
					}
				}

			}

		}

		private void SetPointPotential(Vector2Int position, byte potential)
		{
			if (ValidatePosition(position))
			{
				if (!data[position.x, position.y])
				{
					if (GetPotential(position) == byte.MaxValue)
					{
						potentialPoints.Add(position);
					}
					SetPotential(position, potential);
				}
				
			}
		}
		private void SetPotential(Vector2Int position, byte potential)
		{
			if (potentialMap[position.x, position.y] > potential)
			{
				potentialMap[position.x, position.y] = potential;
			}
		}
		private byte GetPotential(Vector2Int position)
		{
			return (potentialMap[position.x, position.y]);
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
					if (potentialMap != null)
					{
						if (potentialMap[i,j] != byte.MaxValue)
						{
							Gizmos.color = new Color(1,1,1,potentialMap[i,j]/255.0f);
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