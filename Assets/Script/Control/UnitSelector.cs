using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

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
		public Transform movementIndicatorTranform;
		public Color positiveRangeColor;
		public Color negativeRangeColor;
		public Color neutralRangeColor;
		public Color movementColor;
		public Color pathColor;

		private Mesh movementMesh;
		private Mesh rangeMesh;
		private Mesh pathMesh;
		private bool triggerReleaseEvent = false;
		private Unit selection;
		private Unit prevSelection;
		private Vector2Int prevPointer;
		private List<Vector3> vertices = new List<Vector3>();
		private List<int> triangles = new List<int>();
		private List<Color> colorList = new List<Color>();
		private List<PathNode> pathList = new List<PathNode>();
		private bool active;

		private void Awake()
		{
			GameFlow.unitSelector = this;
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

		private Unit GetUnit(Vector2 position, UnitType type)
		{
			float maxDistance = float.PositiveInfinity;
			Unit tmpUnit = null;
			for (int i = 0; i < Unit.units.Count; i++)
			{
				if (Unit.units[i].type == type)
				{
					if (Unit.units[i].selectable == true)
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
			}
			return tmpUnit;
		}

		private void Select(Unit unit)
		{
			pathList.Clear();
			GeneratePathMesh(pathMesh);
			if (unit == null)
			{
				movementMeshFilter.gameObject.SetActive(false);
				rangeMeshFilter.gameObject.SetActive(false);
				pathMeshFilter.gameObject.SetActive(false);
			}
			else
			{
				movementMeshFilter.gameObject.SetActive(true);
				rangeMeshFilter.gameObject.SetActive(true);
				pathMeshFilter.gameObject.SetActive(true);

				GameFlow.potentialMap.GeneratePotential(unit.position, (byte)unit.movementBlock);
				
				GenerateCircleMesh(movementMesh, unit.movementBlock, unit.movementBlock + stripWidth, circleDetailMedium);
				SetMeshColor(movementMesh, movementColor);
				GenerateCircleMesh(rangeMesh,unit.viewRadius,unit.viewRadius+stripWidth,circleDetailBig);
				SetMeshColor(rangeMesh, neutralRangeColor);
				movementIndicatorTranform.position = (Vector2)unit.position;
				rangeIndicatorTransform.position = (Vector2)unit.position;
				prevPointer = unit.position;
			}
		}

		public void KeyPressed(Vector2 position)
		{
			triggerReleaseEvent = true;
			selection = GetUnit(position, UnitType.Friendly);
			if (selection == null)
			{
				Select(prevSelection);
				if (prevSelection != null)
				{
					rangeIndicatorTransform.position = (Vector2)prevSelection.position;
				}
			}
			else
			{
				Select(selection);
				rangeIndicatorTransform.position = (Vector2)selection.position;
			}
		}

		public void KeyReleased(Vector2 position)
		{
			if (triggerReleaseEvent)
			{
				prevSelection = selection;
				if (selection != null)
				{
					if (pathList.Count > 0)
					{
						if (IsValidPosition(GameFlow.potentialMap.GetNearestPoint(prevPointer), selection.position))
						{
							EventHandle.MoveFriendlyUnit(selection.unitID, prevPointer, selection.position);
							Debug.Log("Move");
						}
					}
				}
				Select(selection);
			}
		}

		public void KeyMoved(Vector2 position)
		{
			if (selection != null && triggerReleaseEvent)
			{
				Vector2Int effectivePosition = GameFlow.potentialMap.GetNearestPoint(Convert(position));
				if (effectivePosition != prevPointer)
				{
					if (IsValidPosition(effectivePosition, selection.position) && GameFlow.aStar.GeneratePath(effectivePosition, selection.position, ref pathList, selection.movementBlock))
					{
						SetMeshColor(rangeMesh, Color.cyan);
						GeneratePathMesh(pathMesh);
						SetMeshColor(pathMesh, pathColor);
					}
					else
					{
						SetMeshColor(rangeMesh, Color.red);
						GeneratePathMesh(pathMesh);
						SetMeshColor(pathMesh, pathColor);
					}
					if (pathList.Count > 0)
					{
						rangeIndicatorTransform.position = (Vector2)pathList[pathList.Count - 1].position;
					}
					else
					{
						rangeIndicatorTransform.position = (Vector2)selection.position;
					}
					if (effectivePosition == selection.position)
					{
						pathMeshFilter.gameObject.SetActive(false);
						rangeIndicatorTransform.position = (Vector2)selection.position;
						pathList.Clear();
					}
					else
					{
						pathMeshFilter.gameObject.SetActive(true);
					}
					prevPointer = effectivePosition;
				}
			}
		}

		public void KeyCanceled()
		{
			triggerReleaseEvent = false;
			selection = prevSelection;
			Select(selection);
		}

		private bool IsValidPosition(Vector2Int position,Vector2Int selection)
		{
			return !GameFlow.map.GetObstacle(position) && (position != selection);
		}

		private Vector2Int Convert(Vector2 p)
		{
			return new Vector2Int(Mathf.RoundToInt(p.x), Mathf.RoundToInt(p.y));
		}
		
		private void GenerateCircleMesh(Mesh m, float radius1, float radius2, int detail)
		{
			vertices.Clear();
			triangles.Clear();
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
			}
			m.SetVertices(vertices);
			m.SetTriangles(triangles,0);
			m.RecalculateNormals();
			m.RecalculateBounds();
		}

		private void GeneratePathMesh(Mesh m)
		{
			vertices.Clear();
			triangles.Clear();
			if (pathList.Count > 1)
			{
				for (int i = 0; i < pathList.Count-1;i++)
				{
					CreateLine(pathList[i].position, pathList[i + 1].position);
				}
			}
			pathMesh.Clear();
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
			colorList.Clear();
			for (int i = 0;i < m.vertexCount;i++)
			{
				colorList.Add(c);
			}
			m.SetColors(colorList);
		}

		public bool GetActive()
		{
			return active;
		}

		public void SetActive(bool b)
		{
			active = b;
			triggerReleaseEvent = false;
			Select(null);
			if (active)
			{
				gameObject.SetActive(true);
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
	}
}