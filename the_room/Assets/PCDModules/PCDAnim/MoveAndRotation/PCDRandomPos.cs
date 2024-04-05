using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCDRandomPos : MonoBehaviour
{
    public Vector3 randomPosValue = Vector3.up;
    public Vector3 randomEulerValue = Vector3.up;
    public Vector2 randomScaleValueRange = new Vector2(0.9f, 1.1f);
    protected virtual void OnEnable() {
        transform.position += Random.Range(0f, 1f) * randomPosValue;
        transform.localEulerAngles += Random.Range(0f, 360f) * randomEulerValue;
        transform.localScale *= Random.Range(randomScaleValueRange.x, randomScaleValueRange.y);
    }
}
