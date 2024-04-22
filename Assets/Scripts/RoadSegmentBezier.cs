using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[RequireComponent(typeof(MeshFilter))]
public class RoadSegmentBezier : MonoBehaviour
{
    public Mesh2D shape2D;

    [Range(0, 1)]
    public float t_value = 0;

    public Transform[] controlPoints = new Transform[4];

    Vector3 GetPosition(int i) => controlPoints[i].position;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        for (int i = 0; i < controlPoints.Length; i++)
        {
            Gizmos.DrawSphere(GetPosition(i), 0.1f);
        }

        Handles.DrawBezier(GetPosition(0), GetPosition(3), GetPosition(1), GetPosition(2), Color.green, EditorGUIUtility.whiteTexture, 2f);

        Gizmos.color = Color.red;

        OrientationPoint bezierPoint = getBezierOrientation(t_value);
        Handles.PositionHandle(bezierPoint.position, bezierPoint.rotation);

        void DrawPoint(Vector2 localPosition) => Gizmos.DrawSphere(bezierPoint.GetLocalToWorldPoint(localPosition), 0.06f);

        Vector3[] localVerticies =  shape2D.vertices.Select(v => bezierPoint.GetLocalToWorldPoint(v.point)).ToArray();

        for (int i = 0; i < shape2D.lineIndicies.Length; i+=2)
        {
            Vector3 a = localVerticies[shape2D.lineIndicies[i]];
            Vector3 b = localVerticies[shape2D.lineIndicies[i + 1]];
            Gizmos.color = Color.white;
            Gizmos.DrawLine(a, b);
        }

        for (int i = 0; i < shape2D.vertices.Length; i++)
        {
            Gizmos.color = Color.red;
            DrawPoint(shape2D.vertices[i].point);
        }

        //DrawPoint(Vector3.right * 0.4f);
        //DrawPoint(Vector3.right * 0.2f);
        //DrawPoint(Vector3.right * 0.0f);
        //DrawPoint(Vector3.right * -0.2f);
        //DrawPoint(Vector3.right * -0.4f);
        //DrawPoint(Vector3.up * 0.2f);
        //DrawPoint(Vector3.up * 0.4f);

        Gizmos.color = Color.white;
    }

    OrientationPoint getBezierOrientation(float t)
    {
        Vector3 p0 = GetPosition(0);
        Vector3 p1 = GetPosition(1);
        Vector3 p2 = GetPosition(2);
        Vector3 p3 = GetPosition(3);

        Vector3 a = Vector3.Lerp(p0, p1, t);
        Vector3 b = Vector3.Lerp(p1, p2, t);
        Vector3 c = Vector3.Lerp(p2, p3, t);

        Vector3 d = Vector3.Lerp(a, b, t);
        Vector3 e = Vector3.Lerp(b, c, t);

        Vector3 position = Vector3.Lerp(d, e, t);
        Vector3 tangent = (e - d).normalized;

        return new OrientationPoint(position, tangent);
    }
    

}
