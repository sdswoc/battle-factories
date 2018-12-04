using UnityEngine;

namespace Control
{
	public class UnitSelector : MonoBehaviour, IControl
	{
		public int circleDetailMedium;
		public int circleDetailBig;
		public float stripWidth;
		public MeshFilter movementMeshFilter;
		public MeshFilter rangeMeshFilter;
		public Transform rangeIndicatorTransform;
		private Mesh movementMesh;
		private Mesh rangeMesh;
		private new Transform transform;
		private bool triggerReleaseEvent = false;
		private Unit selection;
		private Unit prevSelection;

		private void Awake()
		{
			GameFlow.unitSelector = this;
			transform = GetComponent<Transform>();
			movementMesh = new Mesh();
			rangeMesh = new Mesh();
			ConstructCircle(movementMesh, 0, 0, circleDetailMedium);
			ConstructCircle(rangeMesh, 0, 0, circleDetailBig);
			movementMeshFilter.mesh = movementMesh;
			rangeMeshFilter.mesh = rangeMesh;
		}

		private void ConstructCircle(Mesh m, float radius1, float radius2, int detail)
		{
			Vector3[] vertices = new Vector3[detail * 2];
			int[] triangles = new int[detail * 6];
			for (int i = 0; i < detail; i++)
			{
				vertices[i * 2 + 0] = new Vector3(Mathf.Sin((float)i * (Mathf.PI / (float)detail) * 2), Mathf.Cos((float)i * (Mathf.PI / (float)detail) * 2)) * radius1;
				vertices[i * 2 + 1] = new Vector3(Mathf.Sin((float)i * (Mathf.PI / (float)detail) * 2), Mathf.Cos((float)i * (Mathf.PI / (float)detail) * 2)) * (radius2);
				triangles[i * 6 + 0] = (i * 2 + 0) % (detail * 2);
				triangles[i * 6 + 1] = (i * 2 + 1) % (detail * 2);
				triangles[i * 6 + 2] = (i * 2 + 2) % (detail * 2);
				triangles[i * 6 + 3] = (i * 2 + 1) % (detail * 2);
				triangles[i * 6 + 4] = (i * 2 + 3) % (detail * 2);
				triangles[i * 6 + 5] = (i * 2 + 2) % (detail * 2);
			}
			m.vertices = vertices;
			m.triangles = triangles;
			m.RecalculateNormals();
			m.RecalculateBounds();
		}

		private void ModifyCircle(Mesh m, float radius1, float radius2)
		{
			Vector3[] vertices = m.vertices;
			for (int i = 0; i < vertices.Length / 2; i++)
			{
				vertices[i * 2 + 0] = new Vector3(Mathf.Sin((float)i * (Mathf.PI / (float)(vertices.Length / 2)) * 2), Mathf.Cos((float)i * (Mathf.PI / (float)(vertices.Length / 2)) * 2)) * radius1;
				vertices[i * 2 + 1] = new Vector3(Mathf.Sin((float)i * (Mathf.PI / (float)(vertices.Length / 2)) * 2), Mathf.Cos((float)i * (Mathf.PI / (float)(vertices.Length / 2)) * 2)) * (radius2);
			}
			m.vertices = vertices;
			m.RecalculateNormals();
			m.RecalculateBounds();
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
				transform.position = unit.position;
				rangeIndicatorTransform.position = unit.position;
				ModifyCircle(movementMesh, unit.movementRadius, unit.movementRadius + stripWidth);
				ModifyCircle(rangeMesh, unit.viewRadius, unit.viewRadius + stripWidth);
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
					if ((selection.position - position).sqrMagnitude > selection.selectionRadius * selection.selectionRadius)
					{
						selection.Move(selection.position + Vector2.ClampMagnitude(position - selection.position, selection.movementRadius));
					}
				}
				Select(selection);
			}
		}

		public void KeyMoved(Vector2 position)
		{
			if (selection != null)
			{
				if ((selection.position - position).sqrMagnitude > selection.selectionRadius * selection.selectionRadius)
				{
					rangeIndicatorTransform.position = selection.position + Vector2.ClampMagnitude(position - selection.position, selection.movementRadius);
				}
				else
				{
					rangeIndicatorTransform.position = selection.position;
				}
			}
		}

		public void KeyCanceled()
		{
			triggerReleaseEvent = false;
			selection = prevSelection;
			Select(selection);
		}
	}
}