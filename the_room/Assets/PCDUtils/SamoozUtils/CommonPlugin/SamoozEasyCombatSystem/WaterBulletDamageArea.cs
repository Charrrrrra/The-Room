using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBulletDamageArea : DamageArea {
    public GameObject waterPrefab;
    public float gravityMult = 3.0f;
    protected override void OnCollisionEnter(Collision other) {
        if (Vector3.Angle(other.contacts[0].normal, Vector3.up) < 1.0f) {
            if (waterPrefab) {
                SpawnMgr.SpawnGameObject(waterPrefab, other.contacts[0].point.ClearY(), Quaternion.identity);
            }
        }
        base.OnCollisionEnter(other);
    }

    void FixedUpdate() {
        if (rb.useGravity) {
            rb.AddForce(Physics.gravity * (gravityMult - 1.0f));
        }
    }

}
