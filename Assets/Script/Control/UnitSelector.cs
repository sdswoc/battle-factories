using UnityEngine;
using System.Collections.Generic;

namespace Control
{
	public class UnitSelector : MonoBehaviour, IControl
	{
		public int circleDetailMedium;
		public int circleDetailBig;
		public float stripWidth;
		public float pathWidth;
		public MeshFilter movementMeshFilter;
		public MeshFilter rangeMeshFilter;
		public MeshFilter pathMeshFilter;
		public Transform rangeIndicatorTransform;
		private Mesh movementMesh;
		private Mesh rangeMesh;
		private Mesh pathMesh;
		private new Transform transform;
		private bool triggerReleaseEvent = false;
		private Unit selection;
		private Unit prevSelection;
		private bool[,] validPosition;
		private int width;
		private int height;
		private List<Vector3> vertices = new List<Vector3>();
		private List<int> triangles = new List<int>();
		private List<Color> colorList = new List<Color>();
		private List<Vector2Int> pathList = new List<Vector2Int>();


		private void Awake()
		{
			GameFlow.unitSelector = this;
			transform = GetComponent<Transform>();
			movementMesh = new Mesh();
			rangeMesh = new Mesh();
			pathMesh = new Mesh();
			rangeMesh.MarkDynamic();
			movementMesh.MarkDynamic();
			pathMesh.MarkDynamic();
			movementMeshFilter.mesh = movementMesh;
			rangeMeshFilter.mesh = rangeMesh;
			pathMeshFilter.mesh = pathMesh;
		}

		private void Start()
		{
			width = GameFlow.map.width;
			height = GameFlow.map.height;
			validPosition = new bool[GameFlow.map.width, GameFlow.map.height];
		}

		private Unit GetUnit(Vector2 position, UnitType type)
		{
			float maxDistance = float.PositiveInfinity;
			Unit tmpUnit = null;
			for (int i = 0; i < Unit.units.Count; i++)
			{
				if (Unit.units[i].type == type)
				{
					Unit un = Unit.units[i];
					float distance = (un.position - position).sqrMagnitude;
					if (distance < un.selectionRadius * un.selectionRadius)
					{
						if (distance < maxDistance)
						{
							maxDistance = distance;
							tmpUnit = un;
						}
					}
				}
			}
			return tmpUnit;
		}

		private void Select(Unit unit)
		{
			if (unit == null)
			{
				movementMeshFilter.gameObject.SetActive(false);
				rangeMeshFilter.gameObject.SetActive(false);
			}
			else
			{
				movementMeshFilter.gameObject.SetActive(true);
				rangeMeshFilter.gameObject.SetActive(true);
				rangeIndicatorTransform.position = (Vector2)unit.position;
				GameFlow.map.GeneratePotential(unit.position, unit.movementBlock);
				GeneratePotentialMesh(movementMesh);
				GenerateRangeMesh(rangeMesh,unit.viewRadius,unit.viewRadius+stripWidth,50);
			}
		}

		public void KeyPressed(Vector2 position)
		{
			triggerReleaseEvent = true;
			selection = GetUnit(position, UnitType.Friendly);
			if (selection == null)
			{
				Select(prevSelection);
			}
			else
			{
				Select(selection);
			}
		}

		public void KeyReleased(Vector2 position)
		{
			if (triggerReleaseEvent)
			{
				prevSelection = selection;
				if (selection != null)
				{
					if (IsValidPosition(Convert(position), selection.position))
					{
						
						selection.Move(Convert(position));
						Debug.Log("Move");
					}
				}
				Select(selection);
			}
		}

		public void KeyMoved(Vector2 position)
		{
			if (selection != null)
			{
			/*
				if ((selection.position - position).sqrMagnitude > selection.selectionRadius * selection.selectionRadius)
				{
					rangeIndicatorTransform.position = selection.position + Vector2.ClampMagnitude(position - selection.position, selection.movementRadius);
				}
				else
				{
					rangeIndicatorTransform.position = (Vector2)selection.position;
				}*/
				if (IsValidPosition(Convert(position), selection.position) && GameFlow.aStar.GeneratePath(selection.position, Convert(position), ref pathList))
				{
					SetMeshColor(rangeMesh, Color.cyan);
					GeneratePathMesh(selection.movementBlock);
				}
				else
				{
					SetMeshColor(rangeMesh, Color.red);
				}
				rangeIndicatorTransform.position = (Vector2)Convert(position);
			}
		}

		private bool IsValidPosition(Vector2Int position,Vector2Int selection)
		{
			return validPosition[position.x, position.y] && (position.x != selection.x || position.y != selection.y);
		}

		public void KeyCanceled()
		{
			triggerReleaseEvent = false;
			selection = prevSelection;
			Select(selection);
		}

		private Vector2Int Convert(Vector2 p)
		{
			return new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.y));
		}

		private void GeneratePotentialMesh(Mesh m)
		{
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					validPosition[i, j] = (GameFlow.map.potentialMap[i, j] != byte.MaxValue);
				}
			}
			vertices.Clear();
			triangles.Clear();
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					validPosition[i, j] = (GameFlow.map.potentialMap[i, j] != byte.MaxValue);
					if (validPosition[i,j])
					{
						int c = vertices.Count;
						triangles.Add(c + 0);
						triangles.Add(c + 2);
						triangles.Add(c + 1);
						triangles.Add(c + 0);
						triangles.Add(c + 3);
						triangles.Add(c + 2);
						vertices.Add(new Vector3(i - 0.5f, j - 0.5f));
						vertices.Add(new Vector3(i + 0.5f, j - 0.5f));
						vertices.Add(new Vector3(i + 0.5f, j + 0.5f));
						vertices.Add(new Vector3(i - 0.5f, j + 0.5f));
					}
				}
			}
			m.Clear();
			m.SetVertices(vertices);
			m.SetTriangles(triangles,0);
			m.RecalculateBounds();
			m.RecalculateNormals();
		}
		
		private void GenerateRangeMesh(Mesh m, float radius1, float radius2, int detail)
		{
			vertices.Clear();
			triangles.Clear();
			colorList.Clear();
			for (int i = 0; i < detail; i++)
			{
				vertices.Add(new Vector3(Mathf.Sin((float)i * (Mathf.PI / (float)detail) * 2), Mathf.Cos((float)i * (Mathf.PI / (float)detail) * 2)) * radius1);
				vertices.Add(new Vector3(Mathf.Sin((float)i * (Mathf.PI / (float)detail) * 2), Mathf.Cos((float)i * (Mathf.PI / (float)detail) * 2)) * (radius2));
				triangles.Add((i * 2 + 0) % (detail * 2));
				triangles.Add((i * 2 + 1) % (detail * 2));
				triangles.Add((i * 2 + 2) % (detail * 2));
				triangles.Add((i * 2 + 1) % (detail * 2));
				triangles.Add((i * 2 + 3) % (detail * 2));
				triangles.Add((i * 2 + 2) % (detail * 2));
				colorList.Add(Color.white);
				colorList.Add(Color.white);
			}
			m.SetVertices(vertices);
			m.SetColors(colorList);
			m.SetTriangles(triangles,0);
			m.RecalculateNormals();
			m.RecalculateBounds();
		}

		private void GeneratePathMesh(int blockLength)
		{
			vertices.Clear();
			triangles.Clear();
			if (pathList.Count > 1)
			{
				int line = 0;
				for (int i = pathList.Count - 2; i >= 0 && line < blockLength;i--)
				{
					CreateLine(pathList[i], pathList[i + 1]);
				}
			}
			pathMesh.SetVertices(vertices);
			pathMesh.SetTriangles(triangles, 0);
			pathMesh.RecalculateNormals();
			pathMesh.RecalculateBounds();
		}
		private void CreateLine(Vector2Int start,Vector2Int end)
		{
			if (end.x-start.x+end.y-start.y < 0)
			{
				Vector2Int t = start;
				start = end;
				end = t;
			}
			int c = vertices.Count;
			triangles.Add(c + 0);
			triangles.Add(c + 2);
			triangles.Add(c + 1);
			triangles.Add(c + 0);
			triangles.Add(c + 3);
			triangles.Add(c + 2);
			if (start.x == end.x)
			{
				vertices.Add(start + new Vector2(-pathWidth, -pathWidth));
				vertices.Add(start + new Vector2(pathWidth, -pathWidth));
				vertices.Add(end + new Vector2(pathWidth, pathWidth));
				vertices.Add(end + new Vector2(-pathWidth, pathWidth));
			}
			else
			{
				vertices.Add(start + new Vector2(-pathWidth, -pathWidth));
				vertices.Add(end + new Vector2(pathWidth, -pathWidth));
				vertices.Add(end + new Vector2(pathWidth, pathWidth));
				vertices.Add(start + new Vector2(-pathWidth, pathWidth));
			}
		}
		private void SetMeshColor(Mesh m,Color c)
		{
			for (int i = 0;i < colorList.Count;i++)
			{
				colorList[i] = c;
			}
			m.SetColors(colorList);
		}
	}
}