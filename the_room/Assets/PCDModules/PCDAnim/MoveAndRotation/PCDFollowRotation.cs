using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class PCDFollowRotation : MonoBehaviour {
    public Transform followTarget;
    public bool lateUpdate = false;

    void Update() {
        if (lateUpdate) {
            return;
        }
        transform.rotation = followTarget.rotation;
    }
    
    void LateUpdate() {
        if (!lateUpdate) {
            return;
        }
        transform.rotation = followTarget.rotation;
    }
    
}
