using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DelayDestorySelf : MonoBehaviour {
    public float delayTime = 2.0f;
    public GameObject destroyEffectPrefab;
    private float delayTimeCount;
    void OnEnable() {
        delayTimeCount = 0;
    }

    void Update() {
        delayTimeCount += Time.deltaTime;
        if (delayTimeCount >= delayTime) {
            if (destroyEffectPrefab) {
                SpawnMgr.SpawnGameObject(destroyEffectPrefab, transform.position, transform.rotation);
            }
            GameObject.Destroy(gameObject);
            DestorySelf();
        }
    }

    public void DestorySelf() {
        DOTween.Kill(transform);
        GameObject.Destroy(gameObject);
    }
}
