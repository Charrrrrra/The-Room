using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBMouseInteracter : MonoBehaviour
{
    public bool autoDrop;
    public float autoDropTime = 5.0f;
    public PBPickable focusingPickable;
    public PBPickable pickingPickable;
    public Transform tarPickableObjTarget;
    public Joint pickingObjJoint;
    public bool isPicking => pickingObjJoint.connectedBody != null;
    private RaycastCursor raycastCursor;
    private float autoDropTimeCount;

    void Start() {
        raycastCursor = RaycastCursor.TryGetInstance();
    }

    void Update() {
        UpdateFocusingPass();
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if (focusingPickable) {
                Pick(focusingPickable);
                autoDropTimeCount = 0;
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            if (isPicking) {
                Drop();
            }
        }
        if (isPicking && autoDrop) {
            autoDropTimeCount += Time.deltaTime;
            if (autoDropTimeCount >= autoDropTime) {
                Drop();
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
