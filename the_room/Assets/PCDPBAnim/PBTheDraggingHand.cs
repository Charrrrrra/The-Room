using System.Collections;
using System.Collections.Generic;
using PhysicsBasedCharacterController;
using UnityEngine;

public class PBTheDraggingHand : PBMousePickable
{
    public CharacterManager charaM;
    public Rigidbody handRb;
    public Joint handJoint;
    public Rigidbody animHandRb;
    public Rigidbody draggingRb;
    private Joint theHandJoint;
    private Rigidbody thehandRb;
    public bool isDragging => theHandJoint.connectedBody != null;

    protected override void Awake() {
        base.Awake();
        theHandJoint = GetComponent<Joint>();
        thehandRb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        if (!isDragging) {
            thehandRb.MovePosition(handRb.position);
        }
    }

    public override void OnPick() {
        base.OnPick();
        thehandRb.isKinematic = false;
        theHandJoint.connectedBody = draggingRb;
        handJoint.connectedBody = thehandRb;
        charaM.SetCharacterLookAt(transform);
    }

    public override void OnDrop() {
        base.OnDrop();
        thehandRb.isKinematic = true;
        theHandJoint.connectedBody = null;
        handJoint.connectedBody = animHandRb;
        charaM.ResetCharacterLookAt();
    }
}
