using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimWaterDebuger : MonoBehaviour {
    public GameObject simWaterPrefab;
    public Transform waterSpawnPoint;
    public Vector3 spawnOffset = Vector3.up;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if (simWaterPrefab) {
                SpawnMgr.SpawnGameObject(simWaterPrefab, waterSpawnPoint.position + spawnOffset, Quaternion.identity);
            }
        }   
        if (Input.GetKey(KeyCode.Mouse1)) {
            if (simWaterPrefab) {
                SpawnMgr.SpawnGameObject(simWaterPrefab, waterSpawnPoint.position + spawnOffset, Quaternion.identity);
            }
        }   
        if (Mathf.Abs(Input.mouseScrollDelta.y) > 0) {
            if (simWaterPrefab) {
                SpawnMgr.SpawnGameObject(simWaterPrefab, waterSpawnPoint.position + spawnOffset, Quaternion.identity);
            }
        }   
    }
}
