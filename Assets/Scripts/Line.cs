using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[ExecuteInEditMode, RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Line : MonoBehaviour
{
    public float thickness = 0.15f;
    public bool connect = true;
    [Min(5)] public int circleResolution = 15;

    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] inputPosition;
    private List<Vector2> posList = new List<Vector2>();
    [SerializeReference] private MeshRenderer meshRenderer;
    [SerializeReference] private MeshFilter meshFilter;


    private void Awake()
    {
        meshFilter.mesh = new Mesh();
        inputPosition = new Vector2[] {transform.position};
    }

    public void SetPositions(Vector2[] positions, bool transformToLocalPos = true)
    {
        inputPosition = positions;
        vertices = new Vector3[positions.Length * 4];
        triangles = new int[(vertices.Length - 2) * 3];

        //transform to local position
        if(transformToLocalPos)
        {
            var matrix = transform.worldToLocalMatrix;
            for (int i = 0; i < positions.Length; i++)
                positions[i] = matrix.MultiplyPoint(positions[i]);
        }

        for (int i = 0; i < positions.Length -1; i++)
        {
            var line = (positions[i + 1] - positions[i]).normalized;
            var normal = new Vector2(-line.y, line.x);
            
            var vi = i * 4; //vertice index
            vertices[vi + 0] = positions[i] - normal * thickness;
            vertices[vi + 1] = positions[i] + normal * thickness;
            vertices[vi + 2] = positions[i + 1] - normal * thickness;
            vertices[vi + 3] = positions[i + 1] + normal * thickness;

            var ti = i * 6; //triangle index
            triangles[ti + 0] = vi + 0;
            triangles[ti + 1] = vi + 1;
            triangles[ti + 2] = vi + 2;
            triangles[ti + 3] = vi + 2;
            triangles[ti + 4] = vi + 1;
            triangles[ti + 5] = vi + 3;
        }

        if(connect)
            AddCircles();

        meshFilter.mesh.Clear();
        meshFilter.mesh.SetVertices(vertices);
        meshFilter.mesh.SetTriangles(triangles, 0);
        meshFilter.mesh.RecalculateNormals();
    }

    private void AddCircles()
    {
        var unitCircle = GetCircle(thickness);
        var circleVertCount = unitCircle.Length;
        var circleTrisCount = (circleVertCount - 2) * 3;
        var circlesCount = inputPosition.Length;
        var circles = new Vector2[circlesCount][];
        for (int i = 0; i < circlesCount; i++)
            circles[i] = MoveCircle(unitCircle, inputPosition[i]);

        //add circles vertices to mesh vertices
        var origVertCount = vertices.Length;
        Array.Resize(ref vertices, vertices.Length + circlesCount * circleVertCount);
    
        var vertIndex = origVertCount;
        for (int i = 0; i < circlesCount; i++)
        {
            for (int j = 0; j < circleVertCount; j++, vertIndex++)
            {
                vertices[vertIndex] = circles[i][j];
            }
        }

        //add circles triangles to mesh triangles
        var origTrisCount = triangles.Length;
        Array.Resize(ref triangles, triangles.Length + circlesCount * circleTrisCount);

        var trisIndex = origTrisCount;
        for (int i = 0; i < circlesCount; i++)
        {
            var startIndex = origVertCount + i * circleVertCount;
            var tris = GetTrianglesFromCircle(startIndex, circleVertCount);
            for (int j = 0; j < circleTrisCount; j++, trisIndex++)
            {
                triangles[trisIndex] = tris[j];
            }
        }
    }

    private Vector2[] GetCircle(float radius)
    {
        var positions = new Vector2[circleResolution + 2];
        var angle = Mathf.Deg2Rad * 360 / circleResolution;
        positions[0] = Vector2.zero;

        for (int i = 0; i < circleResolution; i++)
        {
            var a = -angle * i;
            positions[i + 1] = new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius;
        }

        positions[circleResolution +1] = positions[1]; //connect the end
        return positions;
    }

    private int[] GetTrianglesFromCircle(int startindex, int vertices)
    {
        var tris = new int[(vertices -1) * 3];
        for (int i = 1; i < vertices -1; i++)
        {
            var ti = (i -1) * 3;
            tris[ti] = 0 + startindex;
            tris[ti + 1] = i + startindex;
            tris[ti + 2] = i + 1 + startindex;
        }

        return tris;
    }

    private Vector2[] MoveCircle(Vector2[] circle, Vector2 position)
    {
        var nCircle = new Vector2[circle.Length];
        for (int i = 0; i < circle.Length; i++)
            nCircle[i] = circle[i] + position;

        return nCircle;
    }

    private void Reset()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        var material = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Line.mat");
        material.SetTexture("tex", Texture2D.whiteTexture);
        meshRenderer.material = material;
        
        inputPosition = new Vector2[] { transform.position };
    }
}