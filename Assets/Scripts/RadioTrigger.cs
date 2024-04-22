using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RadioTrigger : MonoBehaviour
{
    public GameObject playerObject;
    public float radius = 0.0f;
    public GameObject LookAt;

    public List<GameObject> gameObjects = new List<GameObject>();

    [Range(0, 180f)]
    public float angleThreshold = 45f;

    public float height = 20.0f;

    void Start()
    {
        AddGameObjectsToList(playerObject);
        AddGameObjectsToList(LookAt);
    }

    public void AddGameObjectsToList(GameObject newGameObject)
    {
        if (newGameObject != null && !gameObjects.Contains(newGameObject))
        {
            gameObjects.Add(newGameObject);
        }
    }

    private void DrawUnitCircle(Vector3 center, Color color)
    {
        Handles.color = color;
        Handles.DrawWireArc(center, Vector3.forward, Vector3.up,360, radius);
    }

    private void OnDrawGizmos()
    {
        Vector3 npc = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, 0);
        Vector3 playerNorm = new Vector3(playerObject.transform.position.x, playerObject.transform.position.y, 0);
        Vector3 lookAtNorm = new Vector3(LookAt.transform.position.x, LookAt.transform.position.y, 0);

        Vector3 v = playerNorm - npc;
        Vector3 l = lookAtNorm - npc;
        Vector3 v_norm = v.normalized;
        Vector3 l_norm = l.normalized;

        float playerLookDotProd = Vector3.Dot(v_norm, l_norm);

        DrawUnitCircle(this.gameObject.transform.position, Color.green);
        //Drawing.DrawVector(Vector3.zero, this.gameObject.transform.position, Color.white, 1.5f);

        foreach (GameObject go in gameObjects)
        {
            //Drawing.DrawVector(Vector3.zero, go.transform.position, Color.white, 1.5f);
            Drawing.DrawVector(this.gameObject.transform.position, go.transform.position - this.gameObject.transform.position, Color.cyan, 1.5f);

            if (Vector3.Distance(go.transform.position, this.gameObject.transform.position) < radius)
            {
                DrawUnitCircle(this.gameObject.transform.position, Color.red);
            }
        }

        float DotProdThreshold = Mathf.Cos(Mathf.Deg2Rad * angleThreshold);

        // Debug.Log("Angle: " + Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(v_norm, l_norm)));

        if (playerLookDotProd >= DotProdThreshold && (playerObject.transform.position.z >= -height && playerObject.transform.position.z <= height))
        {
            Drawing.DrawVector(this.gameObject.transform.position, playerObject.transform.position - this.gameObject.transform.position, Color.red);
        }

        Drawing.DrawVector(npc, v_norm, Color.magenta, 1.5f);
        Drawing.DrawVector(npc, l_norm, Color.magenta, 1.5f);

        // Draw a wedge
        Handles.color = Color.white;
        // Draw upper and lower discs
        Handles.DrawWireDisc(npc - height / 2.0f * Vector3.forward, Vector3.forward, radius);
        Handles.DrawWireDisc(npc + height / 2.0f * Vector3.forward, Vector3.forward, radius);

        // A quaternion for rotation
        Quaternion q_rot = Quaternion.Euler(0.0f, 0.0f, angleThreshold);
        Quaternion q_rot_minus = Quaternion.Euler(0.0f, 0.0f, -angleThreshold);
        
        Vector3 rotated = q_rot * l_norm;
        Drawing.DrawVector(npc, rotated * radius, Color.yellow, 2.0f);
        
        Vector3 rotated_too = q_rot_minus * l_norm;
        Drawing.DrawVector(npc, rotated_too * radius, Color.yellow, 2.0f);

        // Upper Wedge Area
        Gizmos.DrawLine(npc + Vector3.forward * height / 2.0f, npc + Vector3.forward * height / 2.0f + rotated * radius);
        Gizmos.DrawLine(npc + Vector3.forward * height / 2.0f, npc + Vector3.forward * height / 2.0f + rotated_too * radius);

        // Lower Wegde Area
        Gizmos.DrawLine(npc - Vector3.forward * height / 2.0f, npc - Vector3.forward * height / 2.0f + rotated * radius);
        Gizmos.DrawLine(npc - Vector3.forward * height / 2.0f, npc - Vector3.forward * height / 2.0f + rotated_too * radius);

        // Center line
        Gizmos.DrawLine(npc + Vector3.forward * height / 2.0f, npc - Vector3.forward * height / 2.0f);

        //
        Gizmos.DrawLine(npc + Vector3.forward * height / 2.0f + rotated * radius, npc - Vector3.forward * height / 2.0f + rotated * radius);
        Gizmos.DrawLine(npc + Vector3.forward * height / 2.0f + rotated_too * radius, npc - Vector3.forward * height / 2.0f + rotated_too * radius);
    }
}