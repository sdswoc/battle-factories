using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Army;

public class HealthIndicator : MonoBehaviour 
{
	public Color friendlyColor;
	public Color enemyColor;
	public Color neutralColor;
	public float sweepAngle;
	public float angleUnit;
	public float radius;
	public float bandWidth;
	public Unit unit;
	private Mesh mesh;
	private int previousHP;
	private float angle = -90+45; // Set According to camera angle
	private new Transform transform;

	private void Awake()
	{
		mesh = new Mesh();
		transform = GetComponent<Transform>();
		GetComponent<MeshFilter>().mesh = mesh;
	}
	private void Update()
	{
		transform.eulerAngles = new Vector3(0, 0, angle);
		//UpdateMesh();
	}

	public void UpdateMesh()
	{
		mesh = mesh ?? new Mesh();
		int hp = Mathf.Clamp(unit.hp,0,unit.maxHp);
		float percent = (float)hp / unit.maxHp;
		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();
		List<Color> colors = new List<Color>();
		Color baseColor = unit.type == UnitType.Friendly ? friendlyColor : enemyColor;
		int vertLength;
		for (float i = 0;i < sweepAngle* percent;i+= angleUnit)
		{
			vertices.Add(new Vector3(Mathf.Sin(i*Mathf.Deg2Rad), Mathf.Cos(i * Mathf.Deg2Rad)) * radius);
			vertices.Add(new Vector3(Mathf.Sin(i * Mathf.Deg2Rad), Mathf.Cos(i * Mathf.Deg2Rad)) * (radius + bandWidth));
			colors.Add(baseColor);
			colors.Add(baseColor);
			vertLength = vertices.Count-1;
			if (vertLength - 3 >= 0)
			{
				triangles.Add(vertLength - 0);
				triangles.Add(vertLength - 1);
				triangles.Add(vertLength - 2);
				triangles.Add(vertLength - 1);
				triangles.Add(vertLength - 3);
				triangles.Add(vertLength - 2);
			}
		}
		vertices.Add(new Vector3(Mathf.Sin(sweepAngle * percent * Mathf.Deg2Rad), Mathf.Cos(sweepAngle * percent * Mathf.Deg2Rad)) * radius);
		vertices.Add(new Vector3(Mathf.Sin(sweepAngle * percent * Mathf.Deg2Rad), Mathf.Cos(sweepAngle * percent * Mathf.Deg2Rad)) * (radius + bandWidth));
		colors.Add(baseColor);
		colors.Add(baseColor);
		vertLength = vertices.Count - 1;
		if (vertLength - 3 >= 0)
		{
			triangles.Add(vertLength - 0);
			triangles.Add(vertLength - 1);
			triangles.Add(vertLength - 2);
			triangles.Add(vertLength - 1);
			triangles.Add(vertLength - 3);
			triangles.Add(vertLength - 2);
		}

		for (float i = sweepAngle; i > sweepAngle * percent; i -= angleUnit)
		{
			vertices.Add(new Vector3(Mathf.Sin(i * Mathf.Deg2Rad), Mathf.Cos(i * Mathf.Deg2Rad)) * radius);
			vertices.Add(new Vector3(Mathf.Sin(i * Mathf.Deg2Rad), Mathf.Cos(i * Mathf.Deg2Rad)) * (radius + bandWidth));
			colors.Add(neutralColor);
			colors.Add(neutralColor);
			vertLength = vertices.Count - 1;
			if (!Mathf.Approximately(i,sweepAngle))
			{
				triangles.Add(vertLength - 0);
				triangles.Add(vertLength - 2);
				triangles.Add(vertLength - 1);
				triangles.Add(vertLength - 1);
				triangles.Add(vertLength - 2);
				triangles.Add(vertLength - 3);
			}
		}
		vertices.Add(new Vector3(Mathf.Sin(sweepAngle * percent * Mathf.Deg2Rad), Mathf.Cos(sweepAngle * percent * Mathf.Deg2Rad)) * radius);
		vertices.Add(new Vector3(Mathf.Sin(sweepAngle * percent * Mathf.Deg2Rad), Mathf.Cos(sweepAngle * percent * Mathf.Deg2Rad)) * (radius + bandWidth));
		colors.Add(neutralColor);
		colors.Add(neutralColor);
		vertLength = vertices.Count - 1;
		if (!Mathf.Approximately(sweepAngle * percent, sweepAngle))
		{
			triangles.Add(vertLength - 0);
			triangles.Add(vertLength - 2);
			triangles.Add(vertLength - 1);
			triangles.Add(vertLength - 1);
			triangles.Add(vertLength - 2);
			triangles.Add(vertLength - 3);
		}
		mesh.Clear();
		mesh.SetVertices(vertices);
		mesh.SetTriangles(triangles, 0);
		mesh.SetColors(colors);
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}
}
