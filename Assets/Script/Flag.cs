using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour 
{
    public MeshFilter circleFilter;
    public float stripWidth;
    public int detail;
    public Transform pivotTransform;
    public float pivotHeight;
    public Vector2Int position;
    public GameObject flagGreen;
    public GameObject flagRed;
    public GameObject flagWhite;
    public ParticleSystem system;
    private Mesh circleMesh;
    private float pivotVelocity;
    private float pivotPosition;
    private void Awake()
    {
        circleMesh = new Mesh();
        circleFilter.mesh = circleMesh;
        GameFlow.flag = this;
        transform.position = (Vector2)position;
    }
    private void Start()
    {
        GenerateCircleMesh(circleMesh, ValueLoader.flagRange, ValueLoader.flagRange + stripWidth, detail);
    }
    private void Update()
    {
        pivotPosition = Mathf.SmoothDamp(pivotPosition, (Mathf.Abs((float)GameFlow.flagCapture / ValueLoader.flagCaptureTurnLimit) * pivotHeight), ref pivotVelocity, 1f);
        pivotTransform.localPosition = -Vector3.forward * pivotPosition;
        if (GameFlow.flagCapture == 0)
        {
            flagGreen.SetActive(false);
            flagRed.SetActive(false);
            flagWhite.SetActive(true);
        }
        else if (GameFlow.flagCapture > 0)
        {
            flagGreen.SetActive(true);
            flagRed.SetActive(false);
            flagWhite.SetActive(false);
        }
        else
        {
            flagGreen.SetActive(false);
            flagRed.SetActive(true);
            flagWhite.SetActive(false);
        }
    }
    public void Play()
    {
        system.Play();
    }
    private void GenerateCircleMesh(Mesh m, float radius1, float radius2, int detail)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
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
        m.SetTriangles(triangles, 0);
        m.RecalculateNormals();
        m.RecalculateBounds();
    }

}
