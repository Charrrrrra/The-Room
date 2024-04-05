using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class DamageArea : MonoBehaviour {
    public float damage = 10.0f;
    public string damageTag;
    public bool activeOnStart;
    public bool destoryOnDamage;
    public bool destoryOnHit;
    public GameObject damageEffectPrefab;
    public GameObject hitEffectPrefab;
    public Transform[] tailEffects;
    public Transform attacker;
    [SerializeField]
    protected Rigidbody rb;
    private Collider coll;
    private bool isDamageDetectActive;

    void OnEnable() {
        InitPhysic();
        if (activeOnStart) {
            SetDamageDetectActive(true);
        }
    }

    private void Update() {
        if (coll.enabled && !isDamageDetectActive) {
            coll.enabled = false;
        }
    }

    protected virtual void OnTriggerEnter(Collider other) {
        // Debug.Log(other.gameObject.name);
        // Debug.Log(other.GetComponent<DamageReceiver>());
        if (other.GetComponent<DamageReceiver>()) {
            // Debug.Log("DamageReceive By " + other.gameObject.name);
            other.GetComponent<DamageReceiver>().DamageBy(this);
            if (destoryOnDamage) {
                GameObject.Destroy(gameObject);
                if (tailEffects != null && tailEffects.Length > 0) {
                foreach (Transform tail in tailEffects) {
                    tail.parent = null;
                }
            }
            }
            if (damageEffectPrefab) {
                SpawnMgr.SpawnGameObject(damageEffectPrefab, transform.position, transform.rotation);
            }
        }
    }

    protected virtual void OnCollisionEnter(Collision other) {
        if (destoryOnHit) {
            if (tailEffects != null && tailEffects.Length > 0) {
                foreach (Transform tail in tailEffects) {
                    tail.parent = null;
                }
            }
            rb.velocity = Vector3.zero;
            GameObject.Destroy(gameObject);
        }
        if (hitEffectPrefab) {
            SpawnMgr.SpawnGameObject(hitEffectPrefab, other.contacts[0].point, Quaternion.LookRotation(rb.velocity.ClearY(), other.contacts[0].normal));
        }
    }

    private void InitPhysic() {
        rb = GetComponentInParent<Rigidbody>() ? GetComponentInParent<Rigidbody>() : GetComponent<Rigidbody>();
        coll = GetComponent<Collider>();

        // rb.constraints = RigidbodyConstraints.FreezeAll;
        coll.isTrigger = true;
        SetDamageDetectActive(false);
    }

    public void SetDamageDetectActive(bool active) {
        isDamageDetectActive = active;
        coll.enabled = active;
    }

}
