using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour
{
    [System.Serializable]
    public class NoiseParams{
        [Range(0.0f, 1000.0f)]
        public float amplitudeScale;
        [Range(0.0f, 1000.0f)]
        public float frequencyScale;
    }

    [Range(1.0f, 1000.0f)]
    public float size = 100f;

    [Range(3, 255)]
    public int resolution = 100;

    [Range(0.1f, 1.0f)]
    public float AmplitudeScale = 10.0f;

    //* Noise Layers
    public NoiseParams[] noiseLayers;

    private Mesh terrainMesh = null;

    public bool ClampToTerrain = false;

    [Range(-5.0f, 5.0f)]
    public float clampingValue = 0.0f;

    private void OnValidate(){
        GenerateMesh();
    }

    public void GenerateMesh()
    {
        //? Check if the mesh exists
        if (terrainMesh == null){
            terrainMesh = new Mesh(); 
            terrainMesh.name = "Terrain Mesh";
        }
        else
            terrainMesh.Clear();

        //? List of vertices
        List<Vector3> vertices = new List<Vector3>();
        //? List of triangles
        List<int> triangles = new List<int>();
        //? List of UVs
        List<Vector2> uvs = new List<Vector2>();

        //* Loop
        /* TODO
        /*  set the uvs
        */

        //? Create the vertices and uvs
        for (int y_coord = 0; y_coord <= resolution; y_coord++)
        {
            for (int x_coord = 0; x_coord <= resolution; x_coord++)
            {
                float x = x_coord * (size / (float)resolution);
                float y = y_coord * (size / (float)resolution);
                //? Randomize the height of the vertex 
                //float z = Random.Range(0.0f, 1.0f) * amplitudeScale;
                float z = 0f;

                for (int i = 0; i < noiseLayers.Length; i++){
                    z += (Mathf.PerlinNoise(x / noiseLayers[i].frequencyScale, y / noiseLayers[i].frequencyScale) - 0.5f) 
                    * noiseLayers[i].amplitudeScale;
                }

                if (ClampToTerrain &&  z < clampingValue){
                    z = clampingValue;
                }

                z *= AmplitudeScale;
                
                Vector3 vertex = new Vector3(x, z, y);
                vertices.Add(vertex);

                Vector2 uv = new Vector2(x_coord / (float)resolution, y_coord / (float)resolution);
                uvs.Add(uv);
            }
        }

        
        //? Create the triangles
        for (int x_coord = 0; x_coord < resolution; x_coord++)
        {
            for (int y_coord = 0; y_coord < resolution; y_coord++)
            {
                int topLeft = x_coord + y_coord * (resolution + 1);
                int topRight = topLeft + 1;
                int bottomLeft = topLeft + resolution + 1;
                int bottomRight = bottomLeft + 1;

                //* 1st Triangle
                triangles.Add(topLeft);
                triangles.Add(bottomLeft);
                triangles.Add(topRight);

                //* 2nd Triangle
                triangles.Add(topRight);
                triangles.Add(bottomLeft);
                triangles.Add(bottomRight);
            }
        }

        //? Assign the vertices and triangles
        terrainMesh.SetVertices(vertices);
        terrainMesh.SetTriangles(triangles, 0);
        terrainMesh.SetUVs(0, uvs);
        terrainMesh.RecalculateNormals();
        //? Assign the mesh
        GetComponent<MeshFilter>().sharedMesh = terrainMesh;
    }
}
