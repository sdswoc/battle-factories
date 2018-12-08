using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	public class PotentialMap
	{
		public byte[,] data;
		public int width;
		public int height;
		private List<Vector2Int> pendingList;

		public PotentialMap(int w,int h)
		{
			width = w;
			height = h;
			data = new byte[width, height];
			pendingList = new List<Vector2Int>();
			Reset();
		}

		private void Reset()
		{
			for (int i = 0;i < width;i ++)
			{
				for (int j = 0;j < height;j++)
				{
					data[i, j] = byte.MaxValue;
				}
			}
			pendingList.Clear();
		}

		public void GeneratePotential(Vector2Int position,byte steps)
		{
			Reset();
			if (!ValidatePosition(position))
			{
				return;
			}
			data[position.x, position.y] = 0;
			pendingList.Add(position);
			while(pendingList.Count > 0)
			{
				Vector2Int pos = pendingList[pendingList.Count - 1];
				pendingList.RemoveAt(pendingList.Count - 1);
				ProcessPosition(pos, steps);
			}
		}
		public Vector2Int GetNearestPoint(Vector2Int position)
		{
			Vector2Int sample = position;
			float distance = float.MaxValue;
			for (int i = 0;i < width;i++)
			{
				for (int j = 0;j < height;j++)
				{
					if (data[i,j] != byte.MaxValue && (new Vector2(i,j)-position).sqrMagnitude < distance)
					{
						sample = new Vector2Int(i, j);
						distance = (new Vector2Int(i, j) - position).sqrMagnitude;
					}
				}
			}
			return sample;
		}
		private void ProcessPosition(Vector2Int position,byte steps)
		{
			if (data[position.x,position.y] < steps)
			{
				byte s = (byte)(data[position.x, position.y]+1);
				AddPoints(position + Vector2Int.right, s);
				AddPoints(position + Vector2Int.left, s);
				AddPoints(position + Vector2Int.up, s);
				AddPoints(position + Vector2Int.down, s);
			}
		}
		private void AddPoints(Vector2Int position,byte steps)
		{
			if (ValidatePosition(new Vector2Int(position.x, position.y)) && !GameFlow.map.GetObstacle(position))
			{
				if (steps < data[position.x, position.y])
				{
					data[position.x, position.y] = steps;
					pendingList.Add(position);
				}
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

	}
}