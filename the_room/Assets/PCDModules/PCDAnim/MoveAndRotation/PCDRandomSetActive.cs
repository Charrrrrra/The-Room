using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCDRandomSetActive : MonoBehaviour
{
    public float activePossibility = 0.5f;
    void OnEnable() {
        gameObject.SetActive(Random.Range(0f, 1f) < activePossibility);
    }
}
