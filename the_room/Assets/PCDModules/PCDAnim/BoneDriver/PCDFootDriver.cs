using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class PCDFootDriver : PCDBoneDriver {
	public enum State {
		Stand, Step, Pose, ArchoringAtTarget
	}

	private Transform footBone;
	private PCDWalkMgr walkMgr;
	private float stepDurationCount;
	private float stepProcess;

	public Transform archoringTarget;
	public float archoringTransition = 0;
	public float archoringTransitionCount;
	public float archoringTransitionProcess;
	public Vector3 lastPos;
	public Quaternion lastRot;

	public Vector3 archoringPos;
	public Quaternion archoringRot;
	public Vector3 targetPos;
	public Vector3 curPos;
	public Quaternion targetRot;

	public float poseDuration;
	public float poseDurationCount;
	public float poseProcess;

	public bool isSteping => sm.curState.Equals(State.Step);


	private UnityAction stepFinishCallBack;
	public StateMachine<State> sm = new StateMachine<State>(State.Stand);

	public PCDFootDriver(PCDWalkMgr human, PCDBone footBone, bool autoTryGetOwnship = false) : base(footBone, autoTryGetOwnship) {
		this.walkMgr = human;
		this.footBone = footBone.transform;
		InitSM();
	}

	public void Step(UnityAction stepFinishCallBack) {
		this.stepFinishCallBack = stepFinishCallBack;
		sm.GotoState(State.Step);
	}

	public void SetStepInfo(Vector3 targetPos, Quaternion targetRot) {
		this.targetPos = targetPos;
		this.targetRot = targetRot;
	}

	public void Update(Vector3 targetPos, Quaternion targetRot) {
		if (!CheckBoneOwnership())
			return;
		SetStepInfo(targetPos, targetRot);

		sm.UpdateStateAction();
	}

	public void Update() {
		if (!CheckBoneOwnership())
			return;

		sm.UpdateStateAction();
	}

	public float GetDisToTargetPos() {
		return Vector3.Distance(archoringPos, targetPos);
	}

	public float GetProcess() {
		return stepProcess;
	}

	public void DoPose() {
		if (sm.curState.Equals(State.ArchoringAtTarget)) {
			return;
		}
		sm.GotoState(State.Pose);
	}

	public void Stand() {
		sm.GotoState(State.Stand);
	}

	public void ArchoringAt(Transform archoringTarget, float transition = 0) {
		if (archoringTarget == null) {
			sm.GotoState(State.Stand);
			this.archoringTarget = null;
			return;
		}
		this.archoringTarget = archoringTarget;
		archoringTransition = transition;

		sm.GotoState(State.ArchoringAtTarget);
	}

	public void SetFootPos(Vector3 curFootPos) {
		if (!footBone) {
			return;
		}
		curPos = lastPos = targetPos = archoringPos = curFootPos;
		footBone.position = curPos;
	}

	private void InitSM() {

		sm.GetState(State.Stand).Bind(
			() => {
			},
			() => {
				footBone.position = archoringPos;
				footBone.rotation = targetRot;
			},
			() => {}
		);

		sm.GetState(State.Step).Bind(
			() => {
				stepDurationCount = 0;
				archoringPos = curPos;
			},
			() => {
				stepDurationCount += walkMgr.scaleDeltaTime;
				stepProcess = Mathf.Min(1.0f, stepDurationCount / walkMgr.animSetting.stepSetting.stepDuration);

				curPos = Vector3.Lerp(archoringPos, targetPos, walkMgr.animSetting.footSetting.footPosCurve.Evaluate(stepProcess));
				Debug.DrawLine(targetPos, targetPos + Vector3.up);
				curPos += Vector3.up * walkMgr.animSetting.footSetting.footHeightCurve.Evaluate(stepProcess) * walkMgr.skeleton.GetBone("Root").transform.localScale.x;

				footBone.position = curPos;
				footBone.rotation = targetRot;

				// if step finish
				if (stepProcess >= 1.0f) {
					sm.GotoState(State.Stand);
					stepFinishCallBack.Invoke();
					archoringPos = curPos;                      
				}


			},
			() => {
				archoringPos = curPos;
			}
		);

		sm.GetState(State.Pose).Bind(
			() => {
				poseDurationCount = 0;
				lastPos = footBone.position;
				lastRot = footBone.rotation;
			},
			() => {

				poseDurationCount += Time.deltaTime;
				poseProcess = Mathf.Min(1.0f, (float)(poseDurationCount / poseDuration));

				curPos = Vector3.Lerp(lastPos, targetPos, walkMgr.animSetting.handSetting.handPosCurve.Evaluate(poseProcess));
				footBone.position = curPos;
				footBone.rotation = Quaternion.Lerp(lastRot, targetRot, poseProcess);

			},
			() => {}
		);

		sm.GetState(State.ArchoringAtTarget).Bind(
			() => {
				archoringTransitionCount = 0;
				lastPos = footBone.position;
				lastRot = footBone.rotation;
			},
			() => {

				if (archoringTarget == null) {
					sm.GotoState(State.Stand);
					return;
				}

				if (archoringTransition == 0) {
					footBone.position = archoringTarget.position;
					footBone.rotation = archoringTarget.rotation;
				} else {
					archoringTransitionCount += Time.deltaTime;
					archoringTransitionProcess = Mathf.Min(1.0f, archoringTransitionCount / archoringTransition);
					footBone.position = Vector3.Lerp(lastPos, archoringTarget.position, archoringTransitionProcess);
					footBone.rotation = Quaternion.Lerp(lastRot, archoringTarget.rotation, archoringTransitionProcess);
				}

				

			},
			() => {
			}
		);

		sm.Init();
	}
}
