using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeLerp {
    public const float frame = 60.0f;
    public static Vector3 LerpV3(Vector3 a, Vector3 b, float t) {
        return Vector3.Lerp(a,b,t * (Time.deltaTime * frame));
    }

    public static float Lerp(float a, float b, float t) {
        return Mathf.Lerp(a,b,t * (Time.deltaTime * frame));
    }
}
