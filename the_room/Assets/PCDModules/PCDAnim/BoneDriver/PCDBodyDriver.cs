using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PCDBodyDriver : PCDBoneDriver
{
    public float headLookAtWeight;
    public float bodyLookAtWeight;
    private PCDHumanMgr PCDHuman;
    private PCDWalkMgr walkMgr => PCDHuman.walkMgr;
    private PCDParameter PCDPara;
    private PCDSkeleton skeleton;
    public Vector3 bodySpringVelocity;
    public Vector3 bodyOffsetLocal;

    public PCDBodyDriver(PCDWalkMgr walkMgr, PCDBone body, bool autoTryGetOwnship = true) : base(body, autoTryGetOwnship) {
        PCDHuman = walkMgr.GetComponent<PCDHumanMgr>();
        PCDPara = walkMgr.pcdPara;
        skeleton = walkMgr.skeleton;
    }

    private Vector3 bodyKFPosLocal;
    private Vector3 bodyTargetPosLocal;
    private Vector3 bodyPosLocalRes;
    private Quaternion bodyFKRotLocal;
    private Quaternion bodyTargetRotLocal;
    private Quaternion bodyRotLocalRes;

    public void SetBodyPosLocal(Vector3 bodyPosLocal) {
        bodyTargetPosLocal = bodyPosLocal;
    }

    public void UpdateBodyPosPass(PCDKFReader kfReader, PCDFootDriver activeFoot) {
        /*
         * P1: 从 KeyFrame 读取基本位置和旋转 -> Animation & (Idle / LStep / RStep)
         * P2：根据 LStep 还是 RStep，获取 Foot -> 计算 targetPos
         * P3: 根据走路姿态，计算 Roll 旋转偏移量
         * P4: 根据 targetPos -> 弹簧/Lerp 到这个地方，计算实际位置
         */

        bodyKFPosLocal = kfReader.GetBoneInfo("Body").localPosition;

        /* BODYPOS: Body UpDown */
        bodyTargetPosLocal = bodyKFPosLocal;
        if (activeFoot != null) {
            bodyTargetPosLocal = bodyKFPosLocal + Vector3.up * walkMgr.animSetting.bodySetting.bodyHeightCurve.Evaluate(activeFoot.GetProcess());
        }

        /* BODYPOS: Body Offset XZ */
        Vector3 bodyTargetOffsetLocal = bodyKFPosLocal.ClearY();
        bodyOffsetLocal = Vector3.Lerp(bodyOffsetLocal, bodyTargetOffsetLocal, Time.deltaTime * walkMgr.animSetting.bodySetting.bodyOffsetSpeed);

        /* BODYPOS: Body Sprint */
        if (walkMgr.animSetting.bodySetting.bodySpringStrength > 0) {
            float toTargetY = bodyTargetPosLocal.y - bodyPosLocalRes.y;
            float dragForceY = toTargetY * walkMgr.animSetting.bodySetting.bodySpringStrength;
            float dampForceY = -bodySpringVelocity.y * walkMgr.animSetting.bodySetting.bodySpringDamp;
            dragForceY += dampForceY;
            bodySpringVelocity += Vector3.up * dragForceY * Mathf.Min(Time.deltaTime, 0.0125f);
            bodySpringVelocity = bodySpringVelocity.normalized * Mathf.Min(30.0f, bodySpringVelocity.magnitude);
        }

        /* RES: Get Body Pos Res */
        if (walkMgr.animSetting.bodySetting.bodySpringStrength > 0) {
            bodyPosLocalRes += bodySpringVelocity * Mathf.Min(Time.deltaTime, 0.0125f);
        } else {
            bodyPosLocalRes = bodyTargetPosLocal;
        }

        /* Actual setup value */
        SetLocalPosition(bodyPosLocalRes + bodyOffsetLocal);
    }

    public void UpdateBodyRotPass(PCDKFReader kfReader) {

        PCDBone rootBone = skeleton.GetBone("Root");

        bodyFKRotLocal = kfReader.GetBoneInfo("Body").localRotation;

        /* BODYROT: Body Roll  */
        bodyTargetRotLocal = bodyFKRotLocal;
        if (Vector3.Angle(PCDPara.moveDir, rootBone.transform.forward) < 180.1f) {
            float bodyRollProcess = Mathf.Clamp((Mathf.Abs(Vector3.SignedAngle(PCDPara.moveDir, rootBone.transform.forward, Vector3.up)) * 1.5f) / 120.0f, 0, 1.0f);
            bodyRollProcess = Mathf.Sin(bodyRollProcess * Mathf.PI);
            bodyRollProcess = PCDPara.speed < 0.5f ? 0 : bodyRollProcess;
            float bodyRollSign = Mathf.Sign(Vector3.SignedAngle(PCDPara.moveDir, rootBone.transform.forward, Vector3.up));
            Quaternion bodyRoll = Quaternion.AngleAxis(bodyRollSign * bodyRollProcess * walkMgr.animSetting.bodySetting.bodyRollAngle, Vector3.forward);
            bodyTargetRotLocal = bodyRoll * bodyFKRotLocal;
        }


        PCDBone bodyBone = skeleton.GetBone("Body");

        /* RES: Get Body Rot Res */
        bodyRotLocalRes = Quaternion.Slerp(bodyBone.transform.localRotation, bodyTargetRotLocal, Time.deltaTime * walkMgr.animSetting.bodySetting.bodyRotSpeed);
        SetLocalRotation(bodyRotLocalRes);
    }

    public void UpdateBodyLookAtPass(Transform bodyLookAtTarget) {
        PCDBone rootBone = skeleton.GetBone("Root");
        /* BODYROT: Body Rotate To LookAtTarget At Yaw */
        if (bodyLookAtTarget) {
            Vector3 toLookAtTargetLocal = Quaternion.Inverse(rootBone.transform.rotation) * (bodyLookAtTarget.position - rootBone.transform.position).ClearY();
            // bodyTargetRotLocal = Quaternion.Lerp(bodyTargetRotLocal, Quaternion.LookRotation(toLookAtTargetLocal, Vector3.up), walkMgr.animSetting.lookAtWeight_body);
            bodyTargetRotLocal = Quaternion.Lerp(bodyTargetRotLocal, bodyFKRotLocal * Quaternion.LookRotation(toLookAtTargetLocal, Vector3.up), walkMgr.animSetting.lookAtSetting.lookAtWeight_body);
            
            PCDBone bodyBone = skeleton.GetBone("Body");
            bodyRotLocalRes = Quaternion.Slerp(bodyBone.transform.localRotation, bodyTargetRotLocal, Time.deltaTime * walkMgr.animSetting.bodySetting.bodyRotSpeed);
            SetLocalRotation(bodyRotLocalRes);
        } 
    }

}
