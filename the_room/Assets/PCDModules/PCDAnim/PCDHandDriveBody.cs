using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCDHandDriveBody : MonoBehaviour
{
    public bool enableHandDriveBodyRot;
    public float handLength = 1f;
    public Transform body;
    public Transform lHand;
    public Transform rHand;

    void Update() {
        transform.position = body.position;
        if (enableHandDriveBodyRot) {
            UpdateHandDriveBodyRotPass();
        }
    }

    
    public void UpdateHandDriveBodyRotPass() {

        Vector3 lHandToRHand = rHand.position - lHand.position;
        Vector3 bodyToLHand = lHand.position - body.position;
        Vector3 bodyToRHand = rHand.position - body.position;

        Quaternion roll = Quaternion.LookRotation(Vector3.forward, bodyToLHand.CopySetZ(0) + bodyToRHand.CopySetZ(0));
        Quaternion yaw = Quaternion.LookRotation(bodyToLHand.ClearY() + bodyToRHand.ClearY(), Vector3.up);
        Quaternion rot = yaw * roll;
        Debug.DrawLine(body.position, body.position + bodyToLHand.CopySetZ(0) + bodyToRHand.CopySetZ(0));
        Debug.DrawLine(body.position, body.position + bodyToRHand.CopySetZ(0));
        Debug.DrawLine(body.position, body.position + bodyToLHand.CopySetZ(0));
        Debug.DrawLine(body.position, body.position + bodyToLHand.ClearY() + bodyToRHand.ClearY());
        Debug.DrawLine(body.position, body.position + bodyToRHand.ClearY());
        Debug.DrawLine(body.position, body.position + bodyToLHand.ClearY());


        // Vector3 lBodyVec = bodyToLHand.magnitude < handLength 
        //                    ? bodyToLHand + Vector3.up * Mathf.Cos(Mathf.Acos(bodyToLHand.magnitude / handLength)) * handLength
        //                    : bodyToLHand;
        
        // Vector3 rBodyVec = bodyToRHand.magnitude < handLength 
        //                    ? bodyToRHand + Vector3.up * Mathf.Cos(Mathf.Acos(bodyToRHand.magnitude / handLength)) * handLength
        //                    : bodyToRHand;
        
        // Debug.DrawLine(body.position, body.position + bodyToRHand);
        // Debug.DrawLine(body.position + bodyToRHand, body.position + bodyToRHand + Vector3.up * Mathf.Cos(Mathf.Acos(bodyToRHand.magnitude / handLength)) * handLength);
        // Debug.DrawLine(body.position, body.position + bodyToRHand + Vector3.up * Mathf.Cos(Mathf.Acos(bodyToRHand.magnitude / handLength)) * handLength);
        // Debug.DrawLine(body.position, body.position + bodyToLHand);
        // Debug.DrawLine(body.position + bodyToLHand, body.position + bodyToLHand + Vector3.up * Mathf.Cos(Mathf.Acos(bodyToLHand.magnitude / handLength)) * handLength);
        // Debug.DrawLine(body.position, body.position + bodyToLHand + Vector3.up * Mathf.Cos(Mathf.Acos(bodyToLHand.magnitude / handLength)) * handLength);

        Vector3 bodyForward = Quaternion.AngleAxis(-90.0f, Vector3.up) * lHandToRHand.ClearY();
        Vector3 bodyUp = Quaternion.AngleAxis(-90.0f, lHandToRHand) * bodyForward;

        Quaternion handDriveBodyRot = Quaternion.LookRotation(bodyForward, bodyUp);

        transform.rotation = rot;

    }

}
