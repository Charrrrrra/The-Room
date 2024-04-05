using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReceiver : MonoBehaviour {
    public List<string> damageTagFilter;
    public virtual void DamageBy(DamageArea other) {
        Debug.Log(gameObject.name + " DamageBy " + other.gameObject.name);
        if (CheckDamageCond(other)) {
            Vector3 hitPoint = GetComponent<Collider>().ClosestPoint(other.transform.position);
            SendMessageUpwards("HandleDamage", new DamageInfo(hitPoint, other.attacker, other.damage, other.damageTag), SendMessageOptions.DontRequireReceiver);
            // GetComponentInParent<Rigidbody>()?.AddForce(other.attacker.forward * 100.0f);
        }
    }

    public virtual bool CheckDamageCond(DamageArea other) {
        return damageTagFilter == null || damageTagFilter.Count < 1 || (damageTagFilter != null && damageTagFilter.Count > 0 && damageTagFilter.Contains(other.damageTag));
    }
}

public struct DamageInfo {
    public DamageInfo(Vector3 hitPoint, Transform attacker, float damage, string damageMsg) {
        this.hitPoint = hitPoint;
        this.attacker = attacker;
        this.damage = damage;
        this.damageTag = damageMsg;
    }
    public Vector3 hitPoint;
    public Transform attacker;
    public float damage;
    public string damageTag;
}