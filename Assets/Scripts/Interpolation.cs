using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Interpolation : MonoBehaviour
{
    public GameObject Cube;
    public GameObject StartPoint;
    public GameObject EndPoint;

    public float interpolationTime = 5.0f;

    [Range(0f, 15f)]
    public float elapsedTime;

    private void DrawVector(Vector3 startPoint, Vector3 endPoint, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(startPoint, startPoint + endPoint);

        // Normalize and scale (*) with some float value 0.1f-0.9f
        // And minus this from the endPoint

        Vector3 offset = endPoint.normalized;
        offset = offset * 0.35f;

        Handles.color = color;
        Handles.ConeHandleCap(0, startPoint + endPoint - offset, Quaternion.LookRotation(endPoint), 0.5f, EventType.Repaint);
    }

    private void OnDrawGizmos()
    {
        DrawVector(Vector3.zero, StartPoint.transform.position, Color.green);
        DrawVector(Vector3.zero, EndPoint.transform.position, Color.red);
        // Draw a vector that follows the cube

        float t = elapsedTime / interpolationTime;

        // Restrict time to 1
        if (t > 1)
        {
            t = 1.0f;
        }

        // Compute Interpolation
        // f(t) = A*(1-t) + B*t

        Vector3 position = (1 - t) * StartPoint.transform.position + t * EndPoint.transform.position;
        Cube.transform.position = position;

        DrawVectorParts(t);
        // Calculate the vector sum
    }

    void DrawVectorParts(float t)
    {
        Vector3 partOfStartpoint = (1-t) * StartPoint.transform.position;
        Vector3 partOfEndPoint = t * EndPoint.transform.position;

        DrawVector(Vector3.zero, partOfStartpoint, Color.magenta);
        DrawVector(partOfStartpoint, partOfEndPoint, Color.magenta);
    }

    // Start is called before the first frame update
    void Start()
    {
        elapsedTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        // elapsed time
        elapsedTime += Time.deltaTime;
        // Interpolate for time

        float t = elapsedTime / interpolationTime;

        // Restrict time to 1
        if (t > 1)
        {
            t = 1.0f;
        }

        // Easing in out sine / cubic
        //if (t < 0.5f)
        //{
        //    // t = 2 * t * t; // y = 2 * t^2
        //    t = 4 * t * t * t; // y = 4 * t^3
        //}
        //else
        //{
        //    // t = 1 - 2 * (1 - t) * (1 - t);
        //    t = 1-4 * (1 - t) * (1 - t) * (1 - t);
        //}

        float d1 = 2.75f;
        float n1 = 7.5625f;

        if (t < 1 / d1)
        {
            t = n1 * t * t;
        }
        else if (t < 2 / d1)
        {
            t = n1 * (t -= 1.5f / d1) * t + 0.75f;
        }
        else if (t < 2.5 / d1)
        {
            t = n1 * (t -= 2.25f / d1) * t + 0.9375f;
        }
        else
        {
            t = n1 * (t -= 2.6525f / d1) * t + 0.984375f;
        }

        // Compute Interpolation
        // f(t) = A*(1-t) + B*t

        Vector3 position = (1-t)* StartPoint.transform.position + t *  EndPoint.transform.position;
        Cube.transform.position = position;
    }
}
