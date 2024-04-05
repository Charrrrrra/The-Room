using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PCDAnim/WalkSetting")]
public class PCDWalkSetting : ScriptableObject
{
    [Tooltip("TimeScale 为 1 时的 speed")]
    public float baseSpeed = 5.0f;
    [Space]
    public StepSetting stepSetting;
    [Space]
    public FootSetting footSetting;
    [Space]
    public HandSetting handSetting;
    [Space]
    public BodySetting bodySetting;
    [Space]
    public LookAtSetting lookAtSetting;


    [System.Serializable]
    public class StepSetting {
        [Tooltip("触发 Step 时如果两个 Foot 的条件相同先迈哪只脚")]
        public bool stepRightFootFirst = false;
        [Tooltip("一次 Step 会让 Foot 移动多远")]
        public float stepOffsetDis = 0.45f;
        [Tooltip("一次 Step 从开始到结束的时间")]
        public float stepDuration = 0.18f;
        [Tooltip("当一次 Step 结束后的冷却时间")]
        public float stepInterval = 0.05f;
        [Tooltip("当 Foot 和 TargetFootPos 的距离超过这个数值时触发 Step")]
        public float stepTriggerDis = 1.2f;
        [Tooltip("当 Rigidbody 的 speed 超过这个数值时触发 Step")]
        public float stepTriggerSpeed = 1f;
        [Tooltip("当 CurVelocity 和 LastVelocity 的夹角超过这个数值时触发 Step")]
        public float stepTriggerAngle = 35.0f;
    }

    [System.Serializable]
    public class FootSetting {
        [Tooltip("在 Step 过程中 CurFootPos 根据 StepProcess 数值 Lerp 到 TarFootPos 的曲线")]
        public AnimationCurve footPosCurve;
        [Tooltip("在 Step 过程中 FootPosY 根据 StepProcess 数值的 PosY 增量曲线")]
        public AnimationCurve footHeightCurve;
    }

    [System.Serializable]
    public class HandSetting {
        [Tooltip("在 Step 过程中 CurHandPos 根据 StepProcess 数值 Lerp 到 TarHandPos 的曲线")]
        public AnimationCurve handPosCurve;
        [Tooltip("一次 Step 手部运动从开始到结束的时间，通常和 StepDuration 一致")]
        public float handPoseDuration = 0.2f;
    }

    [System.Serializable]
    public class BodySetting {
        [Tooltip("在 Step 过程中 BodyPosY 根据 StepProcess 数值的 PosY 增量曲线")]
        public AnimationCurve bodyHeightCurve;
        [Tooltip("身体 Spring 的 strength")]
        public float bodySpringStrength = 500f;
        [Tooltip("身体 Spring 的 damp")]
        public float bodySpringDamp = 10f;
        [Tooltip("身体 RotZ 根据 CurVelocity 和 LastVelocity 的夹角大小的 EulerZ 偏移量")]
        public float bodyRollAngle = 10f;
        public float bodyRotSpeed = 8f;
        public float bodyOffsetSpeed = 5.0f;
    }

    [System.Serializable]
    public class LookAtSetting {
        [Range(0, 1.0f)]
        public float lookAtWeight_head = 0.25f;
        [Range(0, 1.0f)]
        public float lookAtWeight_body = 0.75f;
    }
    
}

