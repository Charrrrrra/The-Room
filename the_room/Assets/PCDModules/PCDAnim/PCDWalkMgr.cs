using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/*
 * Update foot position, left or right foot state, notify other components to play animation
 */
[RequireComponent(typeof(PCDSkeleton))]
[RequireComponent(typeof(PCDParameter))]
public class PCDWalkMgr : MonoBehaviour
{
	public enum WalkState { Standing, Steping };
	[Header("Main Setting")]
	public PCDWalkSetting animSetting;
	public bool[] getOwnershipOnAwake = new bool[5] {true,true,true,true,true};
	public string defaultAnim = "Walk";
	public WalkState walkState = WalkState.Standing;

	[Header("Feature Setting")]
	public bool autoDriveStepLoop = true;
	public bool enableJumpWalk = false;
	public bool enableSpeedWeightStepOffset = true;

	[Space]
	[Header("Effect Binding")]
	public GameObject stepLEffectPrefab;
	public GameObject stepREffectPrefab;
	public GameObject startStepLEffectPrefab;
	public GameObject startStepREffectPrefab;

	[Space]
	[Header("Comp Binding")]
	public PCDAnimator animator;
	public PCDSkeleton skeleton;
	public PCDParameter pcdPara;

	[Space]
	[Header("Look At Setting")]
	public Transform bodyLookAtTarget;
	public Transform headLookAtTarget;

	[Space]
	[Header("Driver Info")]
	public PCDFootDriver lFootDriver;
	public PCDFootDriver rFootDriver;
	public PCDBoneDriver lHandDriver;
	public PCDBoneDriver rHandDriver;
	public PCDBodyDriver bodyDriver;

	private PCDAnimReader curAnimReader;
	private PCDKFReader curKFReader;
	private Rigidbody rb;
	[SerializeField]
	private string curKeyFrame = "Idle";

	public float rootScale => skeleton.GetBone("Root").transform.lossyScale.x;
	public float scaleDeltaTime => Time.deltaTime / rootScale * Mathf.Max(pcdPara.speed / animSetting.baseSpeed, 0.5f);
	private bool isAnyFootOutRange => lFootDriver.GetDisToTargetPos() >= animSetting.stepSetting.stepTriggerDis * rootScale 
									  || rFootDriver.GetDisToTargetPos() >= animSetting.stepSetting.stepTriggerDis * rootScale;
    private bool isAnyFootNotReset => isAnyFootOutRange;
    // private bool isAnyFootNotReset => isAnyFootOutRange || Mathf.Abs(skeleton.GetBone("LFoot").transform.position.y - skeleton.GetBone("Root").transform.position.y) > 0.01f || Mathf.Abs(skeleton.GetBone("RFoot").transform.position.y - skeleton.GetBone("Root").transform.position.y) > 0.01f;

	public Vector3 footForward => Quaternion.AngleAxis(90.0f, Vector3.down) * (skeleton.humanBone.rFoot.transform.position - skeleton.humanBone.lFoot.transform.position).normalized;
	public bool isNeedMove => pcdPara.speed >= animSetting.stepSetting.stepTriggerSpeed;
	public bool isNeedRotate => !isNeedMove && Vector3.Angle(footForward, skeleton.humanBone.root.transform.forward) >= animSetting.stepSetting.stepTriggerAngle;
	private float stepIntervalCount = 0f;

	private void Awake() {
		animator = GetComponent<PCDAnimator>();
		skeleton = GetComponent<PCDSkeleton>();
		pcdPara = GetComponent<PCDParameter>();
		rb = GetComponentInParent<Rigidbody>();

		if (skeleton.GetBone("Body"))
			bodyDriver = new PCDBodyDriver(this, skeleton.GetBone("Body"), getOwnershipOnAwake[0]);
		if (skeleton.GetBone("LFoot"))
			lFootDriver = new PCDFootDriver(this, skeleton.GetBone("LFoot"), getOwnershipOnAwake[1]);
		if (skeleton.GetBone("RFoot"))
			rFootDriver = new PCDFootDriver(this, skeleton.GetBone("RFoot"), getOwnershipOnAwake[2]);
		if (skeleton.GetBone("LHand"))
			lHandDriver = new PCDBoneDriver(skeleton.GetBone("LHand"), getOwnershipOnAwake[3]);
		if (skeleton.GetBone("RHand"))
			rHandDriver = new PCDBoneDriver(skeleton.GetBone("RHand"), getOwnershipOnAwake[4]);
	}

	private void Start() {
		InitPose();
	}

	public void InitPose() {
		SetAnim(defaultAnim);
		curKFReader = curAnimReader.GetKeyFrameReader(curKeyFrame);
		bodyDriver.SetBodyPosLocal(curKFReader.GetBoneInfo("Body").localPosition);
		lHandDriver.SetLocalPosition(curKFReader.GetBoneInfo("LHand").localPosition);
		rHandDriver.SetLocalPosition(curKFReader.GetBoneInfo("RHand").localPosition);
		lHandDriver.SetLocalRotation(curKFReader.GetBoneInfo("LHand").localRotation);
		rHandDriver.SetLocalRotation(curKFReader.GetBoneInfo("RHand").localRotation);
		lFootDriver.SetLocalPosition(curKFReader.GetBoneInfo("LFoot").localPosition);
		lFootDriver.SetFootPos(skeleton.GetBone("Root").transform.position + skeleton.GetBone("Root").transform.rotation * curKFReader.GetBoneInfo("LFoot").localPosition);
		rFootDriver.SetLocalPosition(curKFReader.GetBoneInfo("RFoot").localPosition);
		rFootDriver.SetFootPos(skeleton.GetBone("Root").transform.position + skeleton.GetBone("Root").transform.rotation * curKFReader.GetBoneInfo("RFoot").localPosition);
	}
	
	public virtual void UpdateWalk() {


		if (autoDriveStepLoop) {
			UpdateStepPass();
		}

		curKFReader = GetKFReaderSafe(curKeyFrame);

		UpdateFootPass();
		UpdateBodyPass();
		UpdateHeadLookAtPass();

	}

#region StepPass
	
	private void UpdateStepPass() {

		UpdateBodyDriveStepTarget();

		if (walkState == WalkState.Steping) {
			return;
		}

		// Step Interval
		stepIntervalCount += scaleDeltaTime;
		if (stepIntervalCount < animSetting.stepSetting.stepInterval)
			return;

		// 这里需要处理 Step 和 Stand
		// Debug.DrawLine(transform.position, transform.position + footForward * 10.0f);
		
		if (!isNeedMove && !isNeedRotate) {
			bool isStepLeft = CheckLeftStepFirst();
			if (isAnyFootNotReset)
				if (enableJumpWalk) {
					Step(true,
						GetFootBodyDriveStepTarPos(true, curKeyFrame),
						skeleton.GetBone("Body").transform.rotation * GetKFReaderSafe(curKeyFrame).GetBoneInfo("LFoot").localRotation,
						"Idle");
					Step(false,
						GetFootBodyDriveStepTarPos(false, curKeyFrame),
						skeleton.GetBone("Body").transform.rotation * GetKFReaderSafe(curKeyFrame).GetBoneInfo("RFoot").localRotation,
						"Idle");
				} else {
					Step(isStepLeft,
						GetFootBodyDriveStepTarPos(isStepLeft, curKeyFrame),
						skeleton.GetBone("Body").transform.rotation * GetKFReaderSafe(curKeyFrame).GetBoneInfo("LFoot").localRotation,
						"Idle");
				}
		} else {
			/* Next step */
			walkState = WalkState.Steping;
			bool isStepLeft = CheckLeftStepFirst();
			// 这里更新 Target Pos 是为了在 Walk State 变动之后重新计算 isStepLeft
			if (enableJumpWalk) {
				Step(true,
					GetFootBodyDriveStepTarPos(true, curKeyFrame),
					skeleton.GetBone("Body").transform.rotation * GetKFReaderSafe(curKeyFrame).GetBoneInfo("LFoot").localRotation,
					"LStep");
				Step(false,
					GetFootBodyDriveStepTarPos(false, curKeyFrame),
					skeleton.GetBone("Body").transform.rotation * GetKFReaderSafe(curKeyFrame).GetBoneInfo("RFoot").localRotation,
					"RStep");
			} else {
				Step(isStepLeft,
					GetFootBodyDriveStepTarPos(isStepLeft, curKeyFrame),
					skeleton.GetBone("Body").transform.rotation * GetKFReaderSafe(curKeyFrame).GetBoneInfo("LFoot").localRotation,
					isStepLeft ? "LStep" : "RStep");
			}
		}

		void UpdateBodyDriveStepTarget() {
			/* Update BoneInfo */

			/* Update Left-Foot */
			lFootDriver.SetStepInfo(GetFootBodyDriveStepTarPos(true, curKeyFrame), skeleton.GetBone("Body").transform.rotation * GetKFReaderSafe(curKeyFrame).GetBoneInfo("LFoot").localRotation);
			/* Update Right-Foot */
			rFootDriver.SetStepInfo(GetFootBodyDriveStepTarPos(false, curKeyFrame), skeleton.GetBone("Body").transform.rotation * GetKFReaderSafe(curKeyFrame).GetBoneInfo("RFoot").localRotation);

		}
			
	}

	private bool CheckLeftStepFirst() {
		float lFootToTargetDis = Vector3.Distance(lFootDriver.targetPos, lFootDriver.curPos);
		float rFootToTargetDis = Vector3.Distance(rFootDriver.targetPos, rFootDriver.curPos);
		Vector3 lFootToTargetDir = lFootDriver.curPos - lFootDriver.targetPos;
		Vector3 rFootToTargetDir = rFootDriver.curPos - lFootDriver.targetPos;

		if (Vector3.Dot(lFootToTargetDir, pcdPara.moveDir) < 0 &&
		 	Vector3.Dot(rFootToTargetDir, pcdPara.moveDir) < 0) {
			if (!isNeedRotate && Mathf.Abs(lFootToTargetDis - rFootToTargetDis) < animSetting.stepSetting.stepOffsetDis / 2.0f) {
				return !animSetting.stepSetting.stepRightFootFirst;
			} else {
				return lFootToTargetDis > rFootToTargetDis;
			}
		} else if (Vector3.Dot(lFootToTargetDir, pcdPara.moveDir) >= 0 &&
		 		   Vector3.Dot(rFootToTargetDir, pcdPara.moveDir) >= 0) {
			return lFootToTargetDis > rFootToTargetDis;
		} else {
			if (Vector3.Dot(lFootToTargetDir, pcdPara.moveDir) < 0)
				return true;
			if (Vector3.Dot(rFootToTargetDir, pcdPara.moveDir) < 0)
				return false;
		}

		if (!isNeedRotate && Mathf.Abs(lFootToTargetDis - rFootToTargetDis) < animSetting.stepSetting.stepOffsetDis / 2.0f) {
			return !animSetting.stepSetting.stepRightFootFirst;
		} else {
			return lFootToTargetDis > rFootToTargetDis;
		}

	}
	
	public void Step(bool isStepLeft, Vector3 tarPos, Quaternion tarRot, string keyFrame, Action stepCompeleteCallBack = null) {
		
		curKeyFrame = keyFrame;
		if (isStepLeft) {

			lFootDriver.SetStepInfo(tarPos, tarRot);
		} else {
			rFootDriver.SetStepInfo(tarPos, tarRot);
		}

		DriveStep(isStepLeft, stepCompeleteCallBack);

		void DriveStep(bool isStepLeft, Action stepCompeleteCallBack = null) {
			curKFReader = GetKFReaderSafe(curKeyFrame);
			var idleKFReader = curAnimReader.GetKeyFrameReader("Idle");
			if (isStepLeft) {
				if (startStepLEffectPrefab) {
					SpawnMgr.SpawnGameObject(startStepLEffectPrefab, skeleton.humanBone.lFoot.transform.position, skeleton.humanBone.lFoot.transform.rotation);
				}
				lFootDriver.Step(() => {
					stepIntervalCount = 0f;
					walkState = WalkState.Standing;
					curKeyFrame = "Idle";
					DriveHandToKF(idleKFReader);
					if (stepLEffectPrefab) {
						SpawnMgr.SpawnGameObject(stepLEffectPrefab, skeleton.humanBone.lFoot.transform.position, skeleton.humanBone.lFoot.transform.rotation);
					}
					stepCompeleteCallBack?.Invoke();
				});
				DriveHandToKF();
			} else {
				if (startStepREffectPrefab) {
					SpawnMgr.SpawnGameObject(startStepREffectPrefab, skeleton.humanBone.rFoot.transform.position, skeleton.humanBone.rFoot.transform.rotation);
				}
				rFootDriver.Step(() => {
					stepIntervalCount = 0f;
					walkState = WalkState.Standing;
					curKeyFrame = "Idle";
					DriveHandToKF(idleKFReader);
					if (stepREffectPrefab) {
						SpawnMgr.SpawnGameObject(stepREffectPrefab, skeleton.humanBone.rFoot.transform.position, skeleton.humanBone.rFoot.transform.rotation);
					}
					stepCompeleteCallBack?.Invoke();
				});
				DriveHandToKF();
			}
		}

	}

	private Vector3 GetFootBodyDriveStepTarPos(bool isStepLeft, string keyFrame) {

		Vector3 footTargetPos = Vector3.zero;
		if (isStepLeft) {
			footTargetPos = (skeleton.GetBone("Root").transform.position + skeleton.GetBone("Root").transform.rotation * GetKFReaderSafe(keyFrame).GetBoneInfo("LFoot").localPosition * rootScale).CopySetY(skeleton.GetBone("Root").transform.position.y);
				
			if (walkState == WalkState.Steping) {
				float speedWeightOffset = enableSpeedWeightStepOffset ? 
				animSetting.stepSetting.stepOffsetDis * Mathf.Clamp01(pcdPara.speed / animSetting.baseSpeed) : animSetting.stepSetting.stepOffsetDis;
				Vector3 footTargetForward = isNeedRotate ? skeleton.humanBone.root.transform.forward : pcdPara.moveDir;
				footTargetPos += footTargetForward * speedWeightOffset * rootScale;
			}
		} else {
			footTargetPos = (skeleton.GetBone("Root").transform.position + skeleton.GetBone("Root").transform.rotation * GetKFReaderSafe(keyFrame).GetBoneInfo("RFoot").localPosition * rootScale).CopySetY(skeleton.GetBone("Root").transform.position.y);
			
			if (walkState == WalkState.Steping) {
				float speedWeightOffset = enableSpeedWeightStepOffset ? 
				animSetting.stepSetting.stepOffsetDis * Mathf.Clamp01(pcdPara.speed / animSetting.baseSpeed) : animSetting.stepSetting.stepOffsetDis;
				Vector3 footTargetForward = isNeedRotate ? skeleton.humanBone.root.transform.forward : pcdPara.moveDir;
				footTargetPos += footTargetForward * speedWeightOffset * rootScale;
			}
		}

		return footTargetPos;
	}

#endregion

	private void UpdateFootPass() {
		lFootDriver.Update();
		rFootDriver.Update();
	}

	private void UpdateBodyPass() {

		var activeFoot = curKeyFrame == "Idle" ? null : (curKeyFrame == "LStep" ? lFootDriver : rFootDriver);

		bodyDriver.UpdateBodyPosPass(curKFReader, activeFoot);
		bodyDriver.UpdateBodyRotPass(curKFReader);
		bodyDriver.UpdateBodyLookAtPass(bodyLookAtTarget);

	}

	private void UpdateHeadLookAtPass() {
		PCDBone rootBone = skeleton.GetBone("Root");
		PCDBone bodyBone = skeleton.GetBone("Body");
		PCDBone headBone = skeleton.GetBone("Head");
        if (headBone) {
            Vector3 toTargetWeight;
            if (headLookAtTarget) {
                toTargetWeight = Vector3.Lerp(bodyBone.transform.forward, headLookAtTarget.position - headBone.transform.position, animSetting.lookAtSetting.lookAtWeight_head);
                headBone.transform.rotation = Quaternion.Slerp(headBone.transform.rotation, Quaternion.LookRotation(toTargetWeight, bodyBone.transform.up), Time.deltaTime * animSetting.bodySetting.bodyRotSpeed);
            } else {
                toTargetWeight = Vector3.Lerp(bodyBone.transform.forward, rootBone.transform.forward, animSetting.lookAtSetting.lookAtWeight_head);
                headBone.transform.rotation = Quaternion.Slerp(headBone.transform.rotation, Quaternion.LookRotation(toTargetWeight, bodyBone.transform.up), Time.deltaTime * animSetting.bodySetting.bodyRotSpeed);
            }
        }
	}

	private void LateUpdate() {
		UpdateTeleportPass();
		lastPos = skeleton.humanBone.root.transform.position;
	}

	Vector3 lastPos;
	private void UpdateTeleportPass() {
		if (Vector3.Distance(lastPos, skeleton.humanBone.root.transform.position) > 5.0f) {
			lFootDriver.SetLocalPosition(curKFReader.GetBoneInfo("LFoot").localPosition);
			lFootDriver.SetFootPos(skeleton.GetBone("Root").transform.position + skeleton.GetBone("Root").transform.rotation * curKFReader.GetBoneInfo("LFoot").localPosition);
			rFootDriver.SetLocalPosition(curKFReader.GetBoneInfo("RFoot").localPosition);
			rFootDriver.SetFootPos(skeleton.GetBone("Root").transform.position + skeleton.GetBone("Root").transform.rotation * curKFReader.GetBoneInfo("RFoot").localPosition);
		}
	}
	
	private PCDKFReader GetKFReaderSafe(string kfName) {
		PCDKFReader curKFReader = curAnimReader.GetKeyFrameReader(kfName);
		if (curKFReader == null) {
			return curAnimReader.GetKeyFrameReader("Idle");
		} else {
			return curKFReader;
		}
	}

	public void SetLookAt(Transform target) {
		SetBodyLookAt(target);
		SetHeadLookAt(target);
	}

	public void SetBodyLookAt(Transform target) {
		bodyLookAtTarget = target;
		bodyDriver.bodyLookAtWeight = animSetting.lookAtSetting.lookAtWeight_body;
		SendMessageUpwards("SetCharacterLookAt", target);
	}

	public void SetHeadLookAt(Transform target) {
		headLookAtTarget = target;
		bodyDriver.headLookAtWeight = animSetting.lookAtSetting.lookAtWeight_head;
	}

	public void SetHeadLookAt(UnityEngine.Object target) {
		headLookAtTarget = target as Transform;
		bodyDriver.headLookAtWeight = animSetting.lookAtSetting.lookAtWeight_head;
	}

	public void ResetBodyLookAt() {
		SetBodyLookAt(null);
	}

	public void ResetHeadLookAt() {
		SetHeadLookAt(null);
	}

	public void ResetLookAt() {
		bodyLookAtTarget = null;
		headLookAtTarget = null;
		SendMessageUpwards("ResetCharacterLookAt");
	}

	public void SetAnim(string animName) {
		PCDAnimReader newAnimReader = animator.GetAnimReader(animName);
		if (newAnimReader == null) {
			return;
		}
		curAnimReader = newAnimReader;
	}

	public void ResetAnimToDefault() {
		SetAnim(defaultAnim);
	}

	public void DriveHandToKF(PCDKFReader targetKFReader = null) {
		if (targetKFReader == null) {
			lHandDriver.FadeBoneToKeyFrame(curKFReader, animSetting.handSetting.handPoseDuration, animSetting.handSetting.handPosCurve);
			rHandDriver.FadeBoneToKeyFrame(curKFReader, animSetting.handSetting.handPoseDuration, animSetting.handSetting.handPosCurve);
		} else {
			lHandDriver.FadeBoneToKeyFrame(targetKFReader, animSetting.handSetting.handPoseDuration, animSetting.handSetting.handPosCurve);
			rHandDriver.FadeBoneToKeyFrame(targetKFReader, animSetting.handSetting.handPoseDuration, animSetting.handSetting.handPosCurve);
		}
	}

}