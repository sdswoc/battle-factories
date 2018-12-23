using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridRectangle : MonoBehaviour
{
	public Vector2Int position;
	public Vector2Int size;
	public bool autoRegister;
	public bool registerShootObstacle;
	public bool adjustTransform;
	public static List<GridRectangle> list = new List<GridRectangle>();

	private void Awake()
	{
		if (registerShootObstacle)
		{
			list.Add(this);
		}
		if (adjustTransform)
		{
			transform.position =  (Vector2)position; ;
		}
		else
		{
			position = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
			transform.position = (Vector2)position;
		}
	}
	private void OnDestroy()
	{
		list.Remove(this);
	}
	private void Start()
	{
		if (autoRegister)
		{
			GameFlow.map.RegisterObstacle(this);
		}
	}
	public void Setup()
	{
		transform.position = (Vector2)position;
	}
	private Vector2Int Minimim()
	{
		return new Vector2Int(-1, -1) + (position * 2);
	}
	private Vector2Int Maximum()
	{
		return new Vector2Int(-1, -1) + (position+size) * 2;
	}

	public bool LineTest(Vector2Int start, Vector2Int end)
	{
		end *= 2;
		start *= 2;
		Vector2Int delta = end - start;
		Vector2Int perpendicular = new Vector2Int(delta.y,-delta.x);
		Vector2Int min = Minimim();
		Vector2Int max = Maximum();
		Vector2Int[] points = { min, new Vector2Int(max.x, min.y), max, new Vector2Int(min.x, max.y)  };
		int squareMin, squareMax;
		squareMax = squareMin = Dot(perpendicular, min);
		for (int i = 0;i < points.Length;i++)
		{
			int tt = Dot(points[i], perpendicular);
			squareMin = Mathf.Min(squareMin, tt);
			squareMax = Mathf.Max(squareMax, tt);
		}
		int endPara = Dot(end, perpendicular);
		int startPara = Dot(start, perpendicular);
		if (endPara <= squareMin || endPara >= squareMax)
		{
			return false;
		}
		if (Mathf.Max(start.x,end.x) >= max.x && Mathf.Min(start.x, end.x) >= max.x)
		{
			return false;
		}
		if (Mathf.Max(start.x, end.x) <= min.x && Mathf.Min(start.x, end.x) <= min.x)
		{
			return false;
		}
		if (Mathf.Max(start.y, end.y) >= max.y && Mathf.Min(start.y, end.y) >= max.y)
		{
			return false;
		}
		if (Mathf.Max(start.y, end.y) <= min.y && Mathf.Min(start.y, end.y) <= min.y)
		{
			return false;
		}
		return true;
	}

	private int Dot(Vector2Int a, Vector2Int b)
	{
		return a.x* b.x+a.y*b.y;	
	}
	private void OnDrawGizmos()
	{
		if (adjustTransform)
		{
			transform.position = (Vector2)position; ;
		}
		else
		{
			position = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
			transform.position = (Vector2)position;
		}
		Gizmos.color = Color.white;
		Gizmos.DrawWireCube((Vector3)((Vector2)position + (Vector2)size * 0.5f)-Vector3.one*0.5f, new Vector3(size.x, size.y));
	}
}
