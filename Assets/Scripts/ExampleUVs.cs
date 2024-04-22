using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleUVs : MonoBehaviour
{
    private void Awake()
    {
        Mesh mesh = new Mesh();
        mesh.name = "QuadRing";


        List<Vector3> points = new List<Vector3>() { 
            new Vector3(-1, 1),
            new Vector3(1, 1),
            new Vector3(-1, -1),
            new Vector3(1, -1)
        };

        int[] tris = new int[]
        {
            2, 0, 1,
            2, 1, 3
        };

        List<Vector2> uvs = new List<Vector2>()
        {
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(1, 0),
        };

        List<Vector3> normals = new List<Vector3>() { 
            new Vector3(0, 0, 1),
            new Vector3(0, 0, 1),
            new Vector3(0, 0, 1),
            new Vector3(0, 0, 1),
        };

        mesh.RecalculateNormals();
        mesh.SetVertices(points);
        mesh.triangles = tris;
        mesh.SetUVs(0, uvs);
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
