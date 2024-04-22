using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct OrientationPoint
{
    public Vector3 position;
    public Quaternion rotation;

    public OrientationPoint(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }

    public OrientationPoint(Vector3 position, Vector3 forward)
    {
        this.position = position;
        this.rotation = Quaternion.LookRotation(forward);
    }

    public Vector3 GetLocalToWorldPoint(Vector3 LocalSpacePosition)
    {
        return position + rotation * LocalSpacePosition;
    }
}
