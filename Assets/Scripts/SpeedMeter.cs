using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class SpeedMeter : MonoBehaviour
{
    [Header("Game Objects")]
    public GameObject playerObject;
    public GameObject LookAt;
    public GameObject SpeedLimit;
    public GameObject OverSpeed;

    [Header("Trigger Controller")]
    [Range(0, 180f)]
    public float angleThreshold = 45f;
    public float radius = 0.0f;
    public float height = 20.0f;

    void Update()
    {
        if (Vector3.Distance(playerObject.transform.position, this.gameObject.transform.position) < radius)
        {
            StartCoroutine(Speedster());
        }
    }

    private void DrawUnitCircle(Vector3 center, Color color)
    {
        Handles.color = color;
        Handles.DrawWireArc(center, Vector3.down, Vector3.right, 360, radius);
    }

    private void OnDrawGizmos()
    {
        Vector3 npc = new Vector3(this.gameObject.transform.position.x, 0, this.gameObject.transform.position.z);
        Vector3 playerNorm = new Vector3(playerObject.transform.position.x, 0, playerObject.transform.position.z);
        Vector3 lookAtNorm = new Vector3(LookAt.transform.position.x, 0, LookAt.transform.position.z);

        Vector3 v = playerNorm - npc;
        Vector3 l = lookAtNorm - npc;
        Vector3 v_norm = v.normalized;
        Vector3 l_norm = l.normalized;

        float playerLookDotProd = Vector3.Dot(v_norm, l_norm);
        float DotProdThreshold = Mathf.Cos(Mathf.Deg2Rad * angleThreshold);

        DrawUnitCircle(this.gameObject.transform.position, Color.green);

        if (Vector3.Distance(playerObject.transform.position, this.gameObject.transform.position) < radius)
        {
            DrawUnitCircle(this.gameObject.transform.position, Color.red);
        }
        else
        {
            DrawUnitCircle(this.gameObject.transform.position, Color.green);
        }

        if (playerLookDotProd >= DotProdThreshold && (playerObject.transform.position.z >= -height && playerObject.transform.position.z <= height))
        {
            Drawing.DrawVector(this.gameObject.transform.position, playerObject.transform.position - this.gameObject.transform.position, Color.red);
        }

        // A quaternion for rotation
        Quaternion q_rot = Quaternion.Euler(0.0f, angleThreshold, 0.0f);
        Quaternion q_rot_minus = Quaternion.Euler(0.0f, -angleThreshold, 0.0f);

        Vector3 rotated = q_rot * l_norm;
        Drawing.DrawVector(npc, rotated * radius, Color.yellow, 2.0f);

        Vector3 rotated_too = q_rot_minus * l_norm;
        Drawing.DrawVector(npc, rotated_too * radius, Color.yellow, 2.0f);

        // Center line
        Gizmos.DrawLine(npc + Vector3.forward * height / 2.0f, npc - Vector3.forward * height / 2.0f);
    }

    IEnumerator Speedster()
    {
        // Set the speed limit game object to inactive after 0.5 seconds
        SpeedLimit.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        OverSpeed.SetActive(true);
    }
}
