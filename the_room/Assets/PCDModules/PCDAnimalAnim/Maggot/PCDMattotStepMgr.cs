using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCDMattotStepMgr : MonoBehaviour
{
    public LayerMask resistLayer;
    public PCDWalkMgr walkMgr;
    public Transform stepTarget;
    public float frontStepTargetOffset = 0.5f;
    public float stepGapDis = 0.1f;
    public float backMoveMult = 0.75f;
    public float backMoveTriggerAngle = 90f;
    public bool isBackMove;
    public Transform lFoot => walkMgr.skeleton.GetBone("LFoot").transform;
    public Transform rFoot => walkMgr.skeleton.GetBone("RFoot").transform;
    public Vector3 stepDir => (stepTarget.position - lFoot.position).ClearY().normalized;
    public Vector3 backStepDir => (stepTarget.position - rFoot.position).ClearY().normalized;
    private bool isStepHead = true;
    public bool enableMouseDebug = false;
   
    void Update() {
        // step right
        isBackMove = Vector3.Angle(stepDir, (rFoot.position - lFoot.position).ClearY().normalized) > backMoveTriggerAngle;
        Debug.DrawLine(lFoot.position, stepTarget.position, isBackMove ? Color.red : Color.green);
        Debug.DrawLine(rFoot.position, rFoot.position + (lFoot.position + stepDir * frontStepTargetOffset - rFoot.position).normalized * (frontStepTargetOffset + 0.2f), Color.green);
        if (Input.GetKeyDown(KeyCode.Mouse0) && enableMouseDebug) {
            if (isStepHead) {
                if (!isBackMove) {
                    Vector3 tarPos = lFoot.position + stepDir * frontStepTargetOffset;
                    if (Physics.Raycast(rFoot.position, (tarPos - rFoot.position).normalized, out RaycastHit raycastHit, frontStepTargetOffset + 0.2f, resistLayer))
                        tarPos = lFoot.position + (tarPos - rFoot.position).normalized * (raycastHit.point - rFoot.position).ClearY().magnitude;
                    StepHead(tarPos);
                    
                } else {
                    Vector3 tarPos = rFoot.position + backStepDir * frontStepTargetOffset * backMoveMult;
                    if (Physics.Raycast(lFoot.position, (tarPos - lFoot.position).normalized, out RaycastHit raycastHit, frontStepTargetOffset * backMoveMult + 0.2f, resistLayer))
                        tarPos = rFoot.position + (tarPos - lFoot.position).normalized * (raycastHit.point - lFoot.position).ClearY().magnitude;
                    StepHeadBack(tarPos);
                }
                isStepHead = false;
            } else {
                if (!isBackMove) {
                    Vector3 tarPos = rFoot.position - stepDir * stepGapDis;
                    if (Physics.Raycast(rFoot.position, (tarPos - rFoot.position).normalized, out RaycastHit raycastHit, stepGapDis + 0.2f, resistLayer))
                        tarPos = lFoot.position + (tarPos - rFoot.position).normalized * (raycastHit.point - rFoot.position).ClearY().magnitude;
                    StepTail(tarPos);
                } else {
                    Vector3 tarPos = lFoot.position - stepDir * stepGapDis;
                    if (Physics.Raycast(rFoot.position, (tarPos - rFoot.position).normalized, out RaycastHit raycastHit, stepGapDis + 0.2f, resistLayer))
                        tarPos = lFoot.position + (tarPos - rFoot.position).normalized * (raycastHit.point - rFoot.position).ClearY().magnitude;
                    StepTailBack(tarPos);
                }
                isStepHead = true;
            }
            
        }
        // step left
        // if (Input.GetKeyDown(KeyCode.Mouse1)) {
        //     if (!isBackMove) {
        //         StepTail(rFoot.position - stepDir * stepGapDis);
        //     } else {
        //         StepTailBack(lFoot.position - stepDir * stepGapDis);
        //     }
        // }
        walkMgr.lFootDriver.targetRot = Quaternion.LookRotation((rFoot.position - lFoot.position).ClearY().normalized, Vector3.up);
        walkMgr.rFootDriver.targetRot = Quaternion.LookRotation((rFoot.position - lFoot.position).ClearY().normalized, Vector3.up);
    }

    public void StepHead(Vector3 tarPos) {
        walkMgr.Step(false, tarPos, Quaternion.identity, "RStep");
        walkMgr.lFootDriver.Stand();
    }

    public void StepHeadBack(Vector3 tarPos) {
        walkMgr.Step(true, tarPos, Quaternion.identity, "LStep");
        walkMgr.rFootDriver.Stand();
    }

    public void StepTail(Vector3 tarPos) {
        walkMgr.Step(true, tarPos, Quaternion.identity, "LStep");
        walkMgr.rFootDriver.Stand();
    }

    public void StepTailBack(Vector3 tarPos) {
        walkMgr.Step(false, tarPos, Quaternion.identity, "RStep");
        walkMgr.lFootDriver.Stand();
    }

}
