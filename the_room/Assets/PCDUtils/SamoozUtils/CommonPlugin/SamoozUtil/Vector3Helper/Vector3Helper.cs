using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Helper {
    static public Vector3 CopySetX(this Vector3 vec, float newX = 0f) {
        return new Vector3(newX, vec.y, vec.z);
    }

    static public Vector3 CopySetY(this Vector3 vec, float newY = 0f) {
        return new Vector3(vec.x, newY, vec.z);
    }

    static public Vector3 CopySetZ(this Vector3 vec, float newZ = 0f) {
        return new Vector3(vec.x, vec.y, newZ);
    }

    static public Vector3 CopyAddX(this Vector3 vec, float bias) {
        return new Vector3(vec.x + bias, vec.y, vec.z);
    }

    static public Vector3 CopyAddY(this Vector3 vec, float bias) {
        return new Vector3(vec.x, vec.y + bias, vec.z);
    }

    static public Vector3 CopyAddZ(this Vector3 vec, float bias) {
        return new Vector3(vec.x, vec.y, vec.z + bias);
    }
    public static Vector3 ClearY(this Vector3 vec) {
        return new Vector3(vec.x, 0, vec.z);
    }
}
