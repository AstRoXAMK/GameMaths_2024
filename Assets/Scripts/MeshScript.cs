using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshScript : MonoBehaviour
{
    [Range(3, 100)]
    public int Segments;
    [Range(0, 100)]
    public float innerRadius;
    [Range(0.1f, 50)]
    public float thickness;

    private void OnValidate()
    {
        GenerateDonut();
    }

    private void GenerateDonut()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().sharedMesh = mesh;

        float outerRadius = innerRadius + thickness;
        Vector3[] vertices = new Vector3[Segments*2];
        int[] triangles = new int[Segments*6];

        for (int i = 0; i < Segments; i++)
        {
            float angle = 2 * Mathf.PI * i / Segments;
            float xInner = Mathf.Cos(angle) * innerRadius;
            float yInner = Mathf.Sin(angle) * innerRadius;
            float xOuter = Mathf.Cos(angle) * outerRadius;
            float yOuter = Mathf.Sin(angle) * outerRadius;

            // define vertices for the donut
            vertices[i * 2] = new Vector3(xInner, yInner, 0);
            vertices[i * 2 + 1] = new Vector3(xOuter, yOuter, 0);

            // define triangles for the donut
            int nextIndex = (i + 1) % Segments;
            int currentIndex = i * 2;
            int nextIndexDouble = nextIndex * 2;

            // Define triagnles to create the donut
            triangles[i * 6] = currentIndex;
            triangles[i * 6 + 1] = currentIndex + 1;
            triangles[i * 6 + 2] = nextIndexDouble % (Segments * 2);

            triangles[i * 6 + 3] = currentIndex + 1;
            triangles[i * 6 + 4] = nextIndexDouble % (Segments * 2) + 1;
            triangles[i * 6 + 5] = nextIndexDouble % (Segments * 2);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
