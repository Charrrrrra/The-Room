using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ComponentHelper {
    static public Vector3 GetDiffVec(this Component src, Component target) {
        return target.transform.position - src.transform.position;
	}

    static public Vector3 GetDiffVecClearY(this Component src, Component target) {
        return GetDiffVec(src, target).ClearY();
    }

    static public float GetDist(this Component src, Component target) {
        return GetDiffVec(src, target).magnitude;
    }

    static public float GetDistClearY(this Component src, Component target) {
        return GetDiffVec(src, target).ClearY().magnitude;
    }

    /// <summary>
    /// 实际结果与 FindComponentsInRange2D() 相同，性能会受到影响，但可以接受 Interface 作为类型参数。
    /// </summary>
    static public List<T> FindObjectsInRange2D<T>(this Component src, float maxDist, float maxDegree) {
        Vector3 forward = src.transform.forward.ClearY();

        List<T> res = new List<T>();
        foreach (var item in GameObject.FindObjectsOfType<Component>()) {
            if (item is T t) {
                Vector3 diff = src.GetDiffVecClearY(item);
                float degree = Mathf.Abs(Vector3.Angle(diff, forward));
                if (diff.magnitude <= maxDist && degree <= maxDegree)
                    res.Add(t);
            }
        }

        return res;
    }

    static public List<T> FindComponentsInRange2D<T>(this Component src, float maxDist, float maxDegree) where T : Component {
        Vector3 forward = src.transform.forward.ClearY();

        List<T> res = new List<T>();
        foreach (var item in GameObject.FindObjectsOfType<T>()) {
            Vector3 diff = src.GetDiffVecClearY(item);
            float degree = Mathf.Abs(Vector3.Angle(diff, forward));
            if (diff.magnitude <= maxDist && degree <= maxDegree)
                res.Add(item);
        }

        return res;
    }

    static public void SetLayerRecursively(this GameObject gameObject, string layerName) {
        gameObject.layer = LayerMask.NameToLayer(layerName);
        foreach (Transform child in gameObject.transform)
            SetLayerRecursively(child.gameObject, layerName);
    }

    static public void SetLayerRecursively(this Component component, string layerName) {
        component.gameObject.SetLayerRecursively(layerName);
    }
}
