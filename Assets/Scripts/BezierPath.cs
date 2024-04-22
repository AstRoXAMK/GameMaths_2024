using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BezierPath : MonoBehaviour
{
    [SerializeField]
    public Mesh2D shape2D;
    [Range(0.1f, 100.0f)]
    public float roadScale = 1.0f;

    public BezierPoint[] points;

    public bool ClosedPath = false;
    public bool drawSegments = false;
    public bool loopTrack = false;

    [Range(0.0f, 1.0f)]
    public float t_stimulate = 0.0f;
    private float elapsedTime = 0.0f;
    public float speed = 1.0f;

    [Range(11, 255)]
    public int slices = 32;

    public GameObject MyObjectPrefab;

    public Mesh mesh;


    // Create the local function to get the position and rotation of the object via 
    // OrientationPoint struct
    OrientationPoint getBezierOrientation(float t, Vector3 first_a, Vector3 first_c, 
        Vector3 second_c, Vector3 second_a)
    {
        OrientationPoint op;

        Vector3 a = Vector3.Lerp(first_a, first_c, t);
        Vector3 b = Vector3.Lerp(first_c, second_c, t);
        Vector3 c = Vector3.Lerp(second_c, second_a, t);

        Vector3 d = Vector3.Lerp(a, b, t);
        Vector3 e = Vector3.Lerp(b, c, t);

        Vector3 position = Vector3.Lerp(d, e, t);
        op.position = position;

        Quaternion rotation = Quaternion.LookRotation(e - d);
        op.rotation = rotation;

        return op;
    }

    // Function to create the Race Track
    OrientationPoint CreateRaceTrack(int seg_start, float t_value)
    {
        Vector3 first_a = points[seg_start].getAnchorPoint();
        Vector3 second_a = points[seg_start + 1].getAnchorPoint();
        Vector3 first_c = points[seg_start].getSecondControlPoint();
        Vector3 second_c = points[seg_start + 1].getFirstControlPoint();

        return getBezierOrientation(t_value, first_a, first_c, second_c, second_a);
    }

    private void Update(){
        if (MyObjectPrefab != null)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= speed)
            {
                elapsedTime = 0.0f;
            }
            int numOfPoints = points.Length;
            float partTime = speed / numOfPoints;
            int currentPart = (int)(elapsedTime / partTime);
            float updatedElapsedTime = elapsedTime - (currentPart * partTime);

            float t = updatedElapsedTime / partTime;
            int secondIndex;

            if (currentPart < numOfPoints - 1 || !ClosedPath)
            {
                secondIndex = currentPart + 1;
            }
            else
            {
                secondIndex = 0;
            }

            OrientationPoint op = getBezierOrientation(t, points[currentPart].getAnchorPoint(),
                points[currentPart].getSecondControlPoint(), points[secondIndex].getFirstControlPoint(),
                points[secondIndex].getAnchorPoint());

            MyObjectPrefab.transform.position = op.position;
            MyObjectPrefab.transform.rotation = op.rotation;
        }
    }

    // Draw the path and the object in the Unity Editor
    private void OnDrawGizmos()
    {
        if (mesh == null)
            mesh = new Mesh();
        else
            mesh.Clear();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        

        // Ensure nOfPoints is updated in case points array has changed
        int nOfPoints = points.Length;
        int nSeg = nOfPoints - 1;

        // Draw all Bezier curves in the path
        for (int i = 0; i < nSeg; i++)
        {
            Vector3 first_anchor = points[i].getAnchorPoint();
            Vector3 second_anchor = points[i + 1].getAnchorPoint();

            Vector3 first_control = points[i].getSecondControlPoint();
            Vector3 second_control = points[i + 1].getFirstControlPoint();

            if(drawSegments)
                Handles.DrawBezier(first_anchor, second_anchor, first_control, second_control, Color.green, texture: null, 5f);
        }

        // If the path is closed, draw the curve connecting the last point back to the first
        if (ClosedPath)
        {
            Vector3 first_anchor = points[nSeg].getAnchorPoint();
            Vector3 second_anchor = points[0].getAnchorPoint();

            Vector3 first_control = points[nSeg].getSecondControlPoint();
            Vector3 second_control = points[0].getFirstControlPoint();

            

            if (drawSegments){
                Handles.DrawBezier(first_anchor, second_anchor, first_control, second_control, Color.green, texture: null, 5f);
            }

            //Vector3 first_vertex

            for (int i = 0; i < shape2D.VertexCount - 2; i += 2)
            {
                int first_start = slices * shape2D.VertexCount + i + 1;
                int first_end = first_start + 1;

                int second_start = i + 1;
                int second_end = second_start + 1;

                // 1st triangle
                triangles.Add(first_start);
                triangles.Add(second_start);
                triangles.Add(second_end);

                // 2nd triangle
                triangles.Add(first_start);
                triangles.Add(second_end);
                triangles.Add(first_end);

            }
        }

        List<Vector3> pathPositions = new List<Vector3>();

        // Loop through the slices and draw the road
        for (int slice = 0; slice <= slices; slice++)
        {
            float t_slice = (float)slice / (float)slices; // 0.0f ... 1.0f
            float t_slice_next = (float)(slice + 1) / (float)slices; // 0.0f ... 1.0f

            // Get the segment index
            int seg_start = Mathf.FloorToInt(t_slice * (nSeg));
            int seg_start_next = Mathf.FloorToInt(t_slice_next * (nSeg));
            if (seg_start >= nSeg)
            {
                seg_start = nSeg - 1;
            }
            if (seg_start_next >= nSeg)
            {
                seg_start_next = nSeg - 1;
            }

            //float t_value = (float)(nOfPoints -1) * (t_slice - 1.0f / (float)(nSeg));
            //float t_value = (t_slice - 1.0f / (float)(nSeg) / (1.0f / (float)(nSeg)));
            float t_value = nSeg * t_slice - seg_start;
            float t_value_next = nSeg * t_slice_next - seg_start_next;

            // Control the object position and rotation
            OrientationPoint op = CreateRaceTrack(seg_start, t_value);
            // Draw the road connective lines
            OrientationPoint op_next = CreateRaceTrack(seg_start_next, t_value_next);
            
            pathPositions.Add(op.position);

            Gizmos.color = Color.red;
            // Draw the road
            for (int i = 0; i < shape2D.VertexCount; i++)
            {
                int j = (i + 2) % shape2D.VertexCount;
                Vector3 roadPoint = shape2D.vertices[i].point;
                Vector3 roadPoint_next = shape2D.vertices[j].point;

                // Scalar multiplication for the road scale
                Vector3 transformed_point = op.GetLocalToWorldPoint(roadPoint * roadScale);
                vertices.Add(transformed_point);
                uvs.Add(new Vector2 (shape2D.vertices[i].u, t_value));

                Vector3 transformed_point_next = op.GetLocalToWorldPoint(roadPoint_next * roadScale);

                Vector3 transformed_point_between = op_next.GetLocalToWorldPoint(roadPoint * roadScale);

                if (drawSegments)
                {
                    Handles.color = Color.white;
                    Handles.DrawLine(transformed_point, transformed_point_next, 3f);
                    Handles.color = Color.cyan;
                    Handles.DrawLine(transformed_point, transformed_point_between, 1f);
                }
            }

            for (int i = 0; i < shape2D.VertexCount - 2; i += 2)
            {
                if (slice == slices)
                {
                    break;
                }

                int first_start = slice * shape2D.VertexCount + i + 1;
                int first_end = first_start + 1;

                int second_start = first_start + shape2D.VertexCount;
                int second_end = first_end + shape2D.VertexCount;

                // 1st triangle
                triangles.Add(first_start);
                triangles.Add(second_start);
                triangles.Add(second_end);

                // 2nd triangle
                triangles.Add(first_start);
                triangles.Add(second_end);
                triangles.Add(first_end);
            }
            
            if (slice < slices)
            {
                int index_start = slice * shape2D.VertexCount + 15;
                int index_end = slice * shape2D.VertexCount;

                int next_start = (slice + 1) * shape2D.VertexCount + 15;
                int next_end = (slice + 1) * shape2D.VertexCount;
                
                // 1st triangle
                triangles.Add(index_start);
                triangles.Add(next_start);
                triangles.Add(next_end);

                // 2nd triangle
                triangles.Add(index_start);
                triangles.Add(next_end);
                triangles.Add(index_end);
            }
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().sharedMesh = mesh;
    }
}