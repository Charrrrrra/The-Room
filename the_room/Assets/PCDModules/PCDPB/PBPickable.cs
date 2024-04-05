using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PBPickable : MonoBehaviour
{
    public Rigidbody rb => GetComponent<Rigidbody>();
    private bool oriUseGravity;

    protected virtual void Awake() {
        oriUseGravity = rb.useGravity;
    }

    public virtual void OnPick() {
        rb.useGravity = false;
    }

    public virtual void OnDrop() {
        rb.useGravity = oriUseGravity;
    }
}
