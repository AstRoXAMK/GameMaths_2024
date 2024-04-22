using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingLaser : MonoBehaviour
{
    [Range(1f, 100f)]
    public int Bounces = 3;

    private void OnDrawGizmos()
    {
        RaycastHit hit;
        Vector3 direction = transform.right;
        Vector3 position = transform.position;

        for (int i = 0; i < Bounces; i++)
        {
            if (Physics.Raycast(position, direction, out hit))
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(position, hit.point);
                Vector3 normal = hit.normal;

                Vector3 incoming = -direction;
                Vector3 reflected = 2 * Vector3.Dot(incoming, normal) * normal - incoming;

                position = hit.point;
                direction = reflected;
            }
        }
    }
}
