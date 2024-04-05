using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCDRotationAnim : PCDTransformAnim {
    public Vector3 startScale = Vector3.zero;
    public Vector3 targetScale = Vector3.one;
    public Vector3 rotateSpeed = Vector3.zero;
    private bool isStartRotate;

    public override void PlayAnim() {
        isStartRotate = true;
    }

    public override void StopAnim() {
        isStartRotate = false;
    }

    void Update() {
        if (isStartRotate) {
            transform.localEulerAngles += rotateSpeed * Time.deltaTime;
        }
    }
}
