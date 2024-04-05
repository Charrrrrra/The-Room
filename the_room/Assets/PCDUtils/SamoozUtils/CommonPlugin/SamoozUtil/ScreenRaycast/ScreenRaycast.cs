using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenRaycast : SingletonMono<ScreenRaycast> {
    public bool activeGizmos;
    [Min(0)]
    public float maxDistance;
    private RaycastHit curFrameLayerHitInfo;
    private bool isRaycastThisFrame;

    private void Awake() {
        ResetRaycastInfo();
    }

    void LateUpdate() {
        ResetRaycastInfo();
    }

    void ResetRaycastInfo() {
        curFrameLayerHitInfo = new RaycastHit();
        isRaycastThisFrame = false;
    }

    void UpdateCurFrameHitInfo() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        Physics.Raycast(ray.origin, ray.direction, out hitInfo, maxDistance);
        if (hitInfo.collider != null) {
            curFrameLayerHitInfo = hitInfo;
        }
        isRaycastThisFrame = true;
    }

    public bool GetCurFrameHitInfo(out RaycastHit hitInfo) {
        if (!isRaycastThisFrame) {
            UpdateCurFrameHitInfo();
        }
        hitInfo = curFrameLayerHitInfo;
        return hitInfo.collider != null;
    }

    private void OnDrawGizmos() {
        if (!activeGizmos) {
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(ray.origin, ray.direction * maxDistance);
    }
    
}
