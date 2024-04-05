using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBMouseInteracter : MonoBehaviour
{
    public PBPickable focusingPickable;
    public PBPickable pickingPickable;
    public Transform tarPickableObjTarget;
    public Joint pickingObjJoint;
    public bool isPicking => pickingObjJoint.connectedBody != null;
    private RaycastCursor raycastCursor;

    void Start() {
        raycastCursor = RaycastCursor.TryGetInstance();
    }

    void Update() {
        UpdateFocusingPass();
        if (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1)) {
            if (isPicking) {
                Drop();
            } else if (focusingPickable) {
                Pick(focusingPickable);
            }
        }
    }

    void FixedUpdate() {
        pickingObjJoint.GetComponent<Rigidbody>().MovePosition(tarPickableObjTarget.position);
    }

    public void Pick(PBPickable pickable) {
        if (isPicking) {
            return;
        }
        if (!pickable.rb) {
            return;
        }
        pickingPickable = pickable;
        pickingPickable.OnPick();
        pickingObjJoint.connectedBody = pickingPickable.rb;
    }

    public void Drop() {
        if (!isPicking) {
            return;
        }
        pickingPickable.OnDrop();
        pickingPickable = null;
        pickingObjJoint.connectedBody = null;
    }

    protected virtual void UpdateFocusingPass() {
        focusingPickable = null;
        if (raycastCursor.focusingObj) {
            focusingPickable = raycastCursor.focusingObj.GetComponent<PBPickable>();
        }
    }

}
