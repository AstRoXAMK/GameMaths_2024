using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadRing : MonoBehaviour
{

    // float valuies for inner and outer radius
    [Range(0.01f, 2)]
    public float innerRadius;
    [Range(0.01f, 2)]
    public float thickness;
    float outerRadius => innerRadius + thickness;
    [Range(3, 50)]
    public int angSegCount = 3;
    int VertexCount => angSegCount * 2;

    Mesh mesh;

    void OnDrawGizmosSelected()
    {
        DrawWireCircle(transform.position, transform.rotation, outerRadius, angSegCount);
        DrawWireCircle(transform.position, transform.rotation, innerRadius, angSegCount);
    }

    const float TAU = 6.28318530718f;

    public static Vector2 GetVectorFromAngle(float angle)
    {
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    public static void DrawWireCircle(Vector3 pos, Quaternion rotation, float radius, int detail = 32)
    {
        Vector3[] points3D = new Vector3[detail];
        for (int i = 0; i < detail; i++)
        {
            float t = i / (float)detail;
            float angle = t * TAU;

            Vector2 point2D = GetVectorFromAngle(angle) * radius;
            points3D[i] = pos + rotation * point2D;
        }

        for (int i = 0; i < detail - 1; i++)
        {
            Gizmos.DrawLine(points3D[i], points3D[i + 1]);
        }

        Gizmos.DrawLine(points3D[detail - 1], points3D[0]);
    }

    void Awake()
    {
        mesh = new Mesh();
        mesh.name = "QuadRing";
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    void Update() => GenerateMesh();

    void GenerateMesh()
    {
        mesh.Clear();

        int vCount = VertexCount;

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        for (int i = 0; i < angSegCount; i++)
        {
            float t = i / (float)angSegCount;
            float angle = t * TAU;
            Vector2 dir = GetVectorFromAngle(angle);

            vertices.Add((Vector3)(dir * outerRadius));
            vertices.Add((Vector3)(dir * innerRadius));

            normals.Add(Vector3.forward);
            normals.Add(Vector3.forward);

            uvs.Add(new Vector2(t, 1));
            uvs.Add(new Vector2(t, 0));
        }

        List<int> triangles = new List<int>();
        for (int i = 0; i < angSegCount; i++)
        {
            int root = i * 2;
            int indexInner = root + 1;
            int indexOuterNext = (root + 2) % vCount;
            int indexInnerNext = (root + 3) % vCount;
            
            triangles.Add(root);
            triangles.Add(indexOuterNext);
            triangles.Add(indexInnerNext);
            
            triangles.Add(root);
            triangles.Add(indexInnerNext);
            triangles.Add(indexInner);

        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetNormals(normals);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();
    }
}
