using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PhysicalPlacementForObject : MonoBehaviour
{
    public GameObject Player;

    private void OnDrawGizmos(){
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit)){
            //Gizmos.color = Color.magenta;
            Handles.color = Color.magenta;
            Handles.DrawLine(transform.position, hit.point, 2f);
            Handles.color = Color.green;
            Handles.DrawLine(hit.point, hit.point + 2f * hit.normal, 3f);

            Vector3 left = Vector3.Cross(hit.normal,transform.forward);
            Handles.color = Color.red;
            Handles.DrawLine(hit.point, hit.point + 2f * left, 3f);

            Vector3 forward = Vector3.Cross(left, hit.normal);
            Handles.color = Color.blue;
            Handles.DrawLine(hit.point, hit.point + 2f * forward, 3f);

            if (Player != null){
                Player.transform.position = hit.point;
                Quaternion rotation = Quaternion.LookRotation(forward, hit.normal);
                Player.transform.rotation = rotation;
            }
        }
    }
}
