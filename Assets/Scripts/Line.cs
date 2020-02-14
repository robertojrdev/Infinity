using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    public Material material;
    public float thickness = 0.5f;
    public bool connect = false;
    public bool fillWithCircle = true;
    [Min(0)] public int circleResolution = 15;

    private Vector3[] positions;
    private Vector2[] inputPosition;
    private List<Vector2> posList = new List<Vector2>();

    public void SetPositions(params Vector2[] positions)
    {
        inputPosition = positions;
        List<Vector3> quads = new List<Vector3>();
        Vector3[] vert = new Vector3[4];
        var camPos = (Vector2)Camera.main.transform.position;
        var thickness = this.thickness / 2;
        var connected = false;
        for (int i = 0; i < positions.Length - 1; i++)
        {
            var line = (positions[i + 1] - positions[i]).normalized;
            var normal = new Vector2(-line.y, line.x);

            if (connect && connected) //if the previous line is connected to this one
            {
                //then connect to previous end vertices
                vert[0] = quads[i * 4 - 1];
                vert[1] = quads[i * 4 - 2];
            }
            else
            {
                //do a simple quad...
                vert[0] = positions[i] - normal * thickness;
                vert[1] = positions[i] + normal * thickness;
            }

            if (connect && i < positions.Length - 2) //if is not the last line
            {
                var nextLine = (positions[i + 2] - positions[i + 1]).normalized;
                var tan = (line + nextLine).normalized;
                var miter = new Vector2(-tan.y, tan.x);
                var thick = thickness / Vector2.Dot(miter, normal);

                if (thick < thickness * 2) //if is not too sharp
                {
                    //then connect to the next line
                    vert[2] = positions[i + 1] + miter * thick;
                    vert[3] = positions[i + 1] - miter * thick;
                    connected = true;
                }
                else
                {
                    //break connection
                    vert[2] = positions[i + 1] + normal * thickness;
                    vert[3] = positions[i + 1] - normal * thickness;
                    connected = false;
                }

            }
            else
            {
                //close a simple quad
                vert[2] = positions[i + 1] + normal * thickness;
                vert[3] = positions[i + 1] - normal * thickness;
                connected = false;
            }

            quads.AddRange(vert);
        }

        this.positions = quads.ToArray();
    }

    private Vector2[] GetCircle(float radius)
    {
        var positions = new List<Vector2>();
        var angle = Mathf.Deg2Rad * 360 / circleResolution;
        
        positions.Add(Vector2.zero);
        positions.Add(Vector2.right * radius);
        positions.Add(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius);

        for (int i = 1; i <= circleResolution; i++)
        {
            var a = -angle * i;
            positions.Add(Vector2.zero);
            positions.Add(positions[i * 3 - 1]);
            positions.Add(new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * radius);
        }
        return positions.ToArray();
    }

    private Vector2[] MoveCircle(Vector2[] circle, Vector2 position)
    {
        var nCircle = new Vector2[circle.Length];
        for (int i = 0; i < circle.Length; i++)
            nCircle[i] = circle[i] + position;

        return nCircle;
    }

    private void OnRenderObject()
    {
        if (!material)
            return;

        GL.PushMatrix();
        material.SetPass(0);

        GL.Begin(GL.QUADS);
        foreach (var pos in positions)
            GL.Vertex(pos);

        GL.End();

        if(fillWithCircle)
        {
            GL.Begin(GL.TRIANGLES);
            var circle = GetCircle(thickness / 2);

            foreach (var pos in inputPosition)
            {
                var nCircle = MoveCircle(circle, pos);
                foreach (var item in nCircle)
                {
                    GL.Vertex(item);
                }
            }

            GL.End();
        }

        GL.PopMatrix();
    }

    private void Reset()
    {
        positions = new Vector3[] { transform.position };
        material = new Material(Shader.Find("Standard"));
        material.color = Color.white;
    }
}