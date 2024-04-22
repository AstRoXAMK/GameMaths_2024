using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Vectors : MonoBehaviour
{
    public GameObject unitCircle;

    private void DrawVector(Vector3 pos, Vector3 v, Color c, float thickness = 0.0f)
    {
        //Gizmos.color = c;
        //Gizmos.DrawLine(pos, pos + v);
        // Arrow head?
        Handles.color = c;
        Handles.DrawLine(pos, pos + v, thickness);

        // Compute the "rough" endpoint for the cone
        // Normalize the vector (its magnitude becomes 1)
        Vector3 n = v.normalized;
        n = n * 0.35f; // Now the length is 35cm

        Handles.ConeHandleCap(0, pos + v - n, Quaternion.LookRotation(v), 0.5f, EventType.Repaint);

    }

    private void OnDrawGizmos()
    {
        // x-axis
        DrawVector(Vector3.zero, new Vector3(5, 0, 0), Color.red);

        // y-axis
        DrawVector(Vector3.zero, new Vector3(0, 5, 0), Color.green);

        // vector line
        DrawVector(new Vector3(3, 3, 0), new Vector3(4, 3, 0), Color.magenta);

        // Unit circle
    }
}
