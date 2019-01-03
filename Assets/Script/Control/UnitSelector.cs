using UnityEngine;
using System.Collections.Generic;
using Pathfinding;
using Army;

namespace Control
{
	public class UnitSelector : MonoBehaviour, IControl
	{
        public string popUpText;
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
		private Troop selection;
		private Troop prevSelection;
		private Vector2Int prevPointer;
		private List<Vector3> vertices = new List<Vector3>();
		private List<int> triangles = new List<int>();
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
		private Troop GetUnit(Vector2 position, UnitType type)
		{
			float maxDistance = float.PositiveInfinity;
			Troop tmpUnit = null;
			for (int i = 0; i < GameFlow.units.Count; i++)
			{
				if (GameFlow.units[i].type == type)
				{
					Troop un = GameFlow.units[i] as Troop;
					if (un != null && un.selectable == true && un.fuelConsumption <= GameFlow.fuel)
					{
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
		private void Select(Troop unit)
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
				GameFlow.GenerateCircleMesh(movementMesh, unit.movementBlock, unit.movementBlock + stripWidth, circleDetailMedium);
                GameFlow.SetMeshColor(movementMesh, movementColor);
                GameFlow.GenerateCircleMesh(rangeMesh,unit.viewRadius,unit.viewRadius+stripWidth,circleDetailBig);
                GameFlow.SetMeshColor(rangeMesh, neutralRangeColor);
				movementIndicatorTranform.position = (Vector2)unit.position;
				rangeIndicatorTransform.position = (Vector2)unit.position;
				prevPointer = unit.position;
			}
		}
		public void KeyPressed(Vector2 position)
		{
			triggerReleaseEvent = true;
			GameFlow.uiSpawnUnit.Close();
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
						}
					}
				}
				Select(selection);
				triggerReleaseEvent = false;
			}
			Select(null);
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
                        GameFlow.SetMeshColor(rangeMesh, Color.cyan);
						GeneratePathMesh(pathMesh);
                        GameFlow.SetMeshColor(pathMesh, pathColor);
					}
					else
					{
                        GameFlow.SetMeshColor(rangeMesh, Color.red);
						GeneratePathMesh(pathMesh);
                        GameFlow.SetMeshColor(pathMesh, pathColor);
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
		private void GeneratePathMesh(Mesh m)
		{
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
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
                GameFlow.uiTutorialText.Pop(popUpText);
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
		public bool IsCommandingUnit()
		{
			return triggerReleaseEvent && (selection != null);
		}
	}
}