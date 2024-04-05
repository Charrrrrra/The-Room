using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Line3D : MonoBehaviour {
    public Transform startPointTarget;
    public Transform endPointTarget;
    [Range(0.1f, 1.0f)]
    public float width;
    private Transform line3DMesh;
    private Transform startPoint;
    private Transform endPoint;

    public Vector3 startToEnd => endPoint.position - startPoint.position;

    void Awake() {
        if (line3DMesh = transform.GetChild(0)) {
            startPoint = line3DMesh.GetChild(0).GetChild(0);
            endPoint = line3DMesh.GetChild(0).GetChild(1);
        }
    }

    void LateUpdate() {
        
        if (startPointTarget)
            startPoint.position = startPointTarget.position;

        if (endPointTarget)
            endPoint.position = endPointTarget.position;

        startPoint.rotation = Quaternion.LookRotation(startToEnd, Vector3.up);
        endPoint.rotation = Quaternion.LookRotation(startToEnd, Vector3.up);

        startPoint.localScale = Vector3.one * width;
        endPoint.localScale = Vector3.one * width;

    }
}
