using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMgr : SingletonMono<SpawnMgr> {
    /* Interfaces */
    static public GameObject SpawnGameObject(GameObject gameObject, Transform parent = null, bool isLocalOnly = false) {
        return Instance.SpawnGameObjectImpl(gameObject, parent, isLocalOnly);
    }

    static public GameObject SpawnGameObject(GameObject gameObject, Vector3 position, Quaternion rotation, Transform parent = null, bool isLocalOnly = false) {
        return Instance.SpawnGameObjectImpl(gameObject, position, rotation, parent, isLocalOnly);
    }

    /* Implements */
    protected virtual GameObject SpawnGameObjectImpl(GameObject gameObject, Transform parent, bool isLocalOnly) {
        return Instantiate(gameObject, parent);
    }

    protected virtual GameObject SpawnGameObjectImpl(GameObject gameObject, Vector3 position, Quaternion rotation, Transform parent, bool isLocalOnly) {
        return Instantiate(gameObject, position, rotation, parent);
    }
}
