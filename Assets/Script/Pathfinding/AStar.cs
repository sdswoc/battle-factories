using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	public class AStar
	{
		private bool[,] data;
		private int width;
		private int height;
		private List<Node> openList = new List<Node>();
		private List<Node> closeList = new List<Node>();

		public AStar(ref bool[,] grid, int width, int height)
		{
			data = grid;
			this.width = width;
			this.height = height;
		}

		public bool GeneratePath(Vector2Int start, Vector2Int end, ref List<PathNode> path, int maxNode)
		{
			if (start.x == end.x && start.y == end.y)
			{
				path.Clear();
				return false;
			}
			openList.Clear();
			closeList.Clear();
			openList.Add(new Node(0, 0, 0, 0, start.x, start.y, null));
			Node solution = null;
			while (openList.Count > 0)
			{
				Node current = openList[openList.Count - 1];
				openList.RemoveAt(openList.Count - 1);
				closeList.Add(current);
				if (current.x == end.x && current.y == end.y)
				{
					solution = current;
					break;
				}
				if (current.g <= maxNode)
				{
					ProcessNode(current.x + 1, current.y, start.x, start.y, end.x, end.y, current, 1);
					ProcessNode(current.x, current.y + 1, start.x, start.y, end.x, end.y, current, 2);
					ProcessNode(current.x - 1, current.y, start.x, start.y, end.x, end.y, current, 3);
					ProcessNode(current.x, current.y - 1, start.x, start.y, end.x, end.y, current, 4);
					openList.Sort((y, x) =>
					{
						if (x.f != y.f)
							return x.f - y.f;
						else
							return x.turn - y.turn;
					});
				}
			}
			bool isValid = solution != null;
			if (solution == null)
			{
				Debug.Log("Impossible");
			}
			else
			{
				path.Clear();
			}

			int index = 0;
			while (solution != null)
			{
				path.Add(new PathNode(new Vector2Int(solution.x, solution.y), index));
				index++;
				maxNode--;
				if (maxNode <= 0)
				{
					break;
				}
				solution = solution.parent;
			}
			SimplifyPath(ref path);
			return isValid;
		}
		private void ProcessNode(int x, int y, int startX, int startY, int endX, int endY, Node parent, byte direction)
		{
			int index = -1;
			if (GetObstacle(x, y, startX, startY, endX, endY) || Find(closeList, x, y, ref index) != null)
			{
				return;
			}
			else
			{
				Node open = Find(openList, x, y, ref index);
				int penalty = 0;
				if (parent.direction * direction == 0 || parent.direction % 2 == direction % 2)
				{
					penalty = 0;
				}
				else
				{
					penalty = 1;
				}
				if (open == null)
				{
					openList.Add(new Node(direction, parent.g + 1, Mathf.Abs(endX - x) + Mathf.Abs(endY - y), parent.turn + penalty, x, y, parent));
				}
				else
				{
					if (open.g > parent.g + 1)
					{
						open.parent = parent;
						open.g = parent.g + 1;
						open.f = open.g + Mathf.Abs(endX - x) + Mathf.Abs(endY - y);
					}
				}
			}
		}
		private bool GetObstacle(int x, int y, int startX, int startY, int endX, int endY)
		{
			if (x == startX && y == startY)
			{
				return false;
			}
			if (x == endX && y == endY)
			{
				return false;
			}
			if (x >= 0 && x < width)
			{
				if (y >= 0 && y < height)
				{
					return data[x, y];
				}
			}
			return true;
		}
		private Node Find(List<Node> list, int x, int y, ref int index)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].x == x && list[i].y == y)
				{
					index = i;
					return list[i];
				}
			}
			return null;
		}
		private void SimplifyPath(ref List<PathNode> path)
		{
			for (int i = 1; i < path.Count - 1; i++)
			{
				Vector2Int a = path[i + 1].position - path[i].position;
				Vector2Int b = path[i].position - path[i - 1].position;
				if (a.x * b.y - b.x * a.y == 0)
				{
					path.RemoveAt(i);
					i--;
				}
			}
		}
	}
	public class Node
	{
		public byte direction;
		public int f;
		public int g;
		public int turn;
		public int x;
		public int y;
		public Node parent;

		public Node(byte direction, int g, int h, int turn, int x, int y, Node parent)
		{
			this.direction = direction;
			this.x = x;
			this.y = y;
			this.g = g;
			this.f = g + h;
			this.turn = turn;
			this.parent = parent;
		}
	}

	public class PathNode
	{
		public Vector2Int position;
		public int block;
		public PathNode(Vector2Int position, int block)
		{
			this.position = position;
			this.block = block;
		}
	}
}