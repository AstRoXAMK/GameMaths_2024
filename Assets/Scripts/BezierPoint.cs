using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BezierPoint : MonoBehaviour
{
    [SerializeField] Transform anchor;
    [SerializeField] Transform[] controls = new Transform[2];

    public Vector3 getAnchorPoint() => anchor.position;
    public Vector3 getFirstControlPoint() => controls[0].position;
    public Vector3 getSecondControlPoint() => controls[1].position;

    public bool drawControlPoints = false;


    private void OnDrawGizmos()
    {
        // Draw lines
        Gizmos.color = Color.white;
        Gizmos.DrawLine(getFirstControlPoint(), getAnchorPoint());
        Gizmos.DrawLine(getAnchorPoint(), getSecondControlPoint());
        //Draw Control points
        if (drawControlPoints){
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(getFirstControlPoint(), 0.1f * HandleUtility.GetHandleSize(getFirstControlPoint()));
            Gizmos.DrawSphere(getSecondControlPoint(), 0.1f * HandleUtility.GetHandleSize(getSecondControlPoint()));
        }
        //Draw Anchor point
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(getAnchorPoint(), 0.05f * HandleUtility.GetHandleSize(getAnchorPoint()));
    }
}
