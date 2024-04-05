using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TransfromLimiter_MoveOnLine : MonoBehaviour {
    public Transform movingPoint;
    public Transform lineStartPoint;
    public Transform lineEndPoint;
    public Vector3 lineStartToEnd => lineEndPoint.position - lineStartPoint.position;
    public Vector3 lineStartToMoving => movingPoint.position - lineStartPoint.position;
    public float process => lineStartToMoving.magnitude / lineStartToEnd.magnitude;
    private Vector3 limitedPos;

    void Update() {
        limitedPos = lineStartPoint.position + Mathf.Clamp(Vector3.Dot(lineStartToMoving, lineStartToEnd.normalized) , 0, lineStartToEnd.magnitude) * lineStartToEnd.normalized;
        movingPoint.position = limitedPos;
    }

}
