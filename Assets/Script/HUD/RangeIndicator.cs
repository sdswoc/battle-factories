using UnityEngine;

namespace HUD
{
	public class RangeIndicator : MonoBehaviour
	{
		public int detail;
		public Unit unit;
		private Mesh mesh;
		private void Awake()
		{
			mesh = new Mesh();
			Vector3[] vertices = new Vector3[detail + 1];
			int[] triangles = new int[detail * 3];
			vertices[0] = Vector3.zero;
			for (int i = 0; i < detail; i++)
			{
				vertices[i + 1] = new Vector3(Mathf.Sin(i * Mathf.PI * 2 / detail), Mathf.Cos(i * Mathf.PI * 2 / detail)) * unit.viewRadius;
				triangles[i * 3] = 0;
				triangles[i * 3 + 1] = i + 1;
				triangles[i * 3 + 2] = (i + 1) % detail + 1;
			}
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			GetComponent<MeshFilter>().mesh = mesh;
		}
	}
}