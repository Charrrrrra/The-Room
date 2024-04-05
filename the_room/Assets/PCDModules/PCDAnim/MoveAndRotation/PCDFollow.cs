using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class PCDFollow : MonoBehaviour {
    public bool useUpdate;
    public bool useLateUpdate;
    public bool useFixedUpdate;
    public Vector3 allowedError;
    public Transform followTarget;
    public Vector3 followOffset;
    public bool localOffset;
    public bool lerp;
    public float lerpSpeed = 5.0f;
    public bool lockY = false;
    public float lockYValue = 0;

    void Update() {
        if (useUpdate) {
            UpdateFollow(Time.deltaTime);
        }
    }

    void LateUpdate() {
        if (useLateUpdate) {
            UpdateFollow(Time.deltaTime);
        }
    }

    void FixedUpdate() {
        if (useFixedUpdate) {
            UpdateFollow(Time.fixedDeltaTime);
        }
    }

    void UpdateFollow(float deltaTime) {
        Vector3 targetPos = followTarget.position + (localOffset ? followTarget.rotation * followOffset : followOffset);
        Vector3 diff = targetPos - transform.position;
        if (Mathf.Abs(diff.x) < allowedError.x)
            targetPos.x = transform.position.x;
        if (Mathf.Abs(diff.y) < allowedError.y)
            targetPos.y = transform.position.y;
        if (Mathf.Abs(diff.z) < allowedError.z)
            targetPos.z = transform.position.z;

        if (lockY) {
            targetPos.y = lockYValue;
        }
        if (lerp)
            targetPos = Vector3.Lerp(transform.position, targetPos, lerpSpeed * deltaTime);
        transform.position = targetPos;
    }
}
