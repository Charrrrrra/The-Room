using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
public class PCDHuman : MonoBehaviour {

    [System.Serializable]
    public class PCDHumanBoneSetting {

        public Transform root;

        public Transform head;

        public Transform body;

        public Transform lPelvis;
        public Transform rPelvis;

        public Transform lFoot;
        public Transform rFoot;

        public Transform lShoulder;
        public Transform rShoulder;

        public Transform lHand;
        public Transform rHand;

        public Transform poseLayerParent;
        public HumanPoseLayer[] poseLayers;
        public Dictionary<string, int> poseNameToLayerMap;

        public Vector3 bodyOriPosLocal;
        public Vector3 lPelvisOriPosLocal;
        public Vector3 rPelvisOriPosLocal;
        public Vector3 lShoulderOriPosLocal;
        public Vector3 rShoulderOriPosLocal;


        public bool isLLegActive;
        public bool isRLegActive;
        public bool isLArmActive;
        public bool isRArmActive;

        public Quaternion toLocalRot => Quaternion.Inverse(root.rotation);
        public float rootScale => root.localScale.x;

        public void Init() {

            poseNameToLayerMap = new();
            
            if (poseLayerParent && poseLayerParent.childCount > 0) {
                poseLayers = new HumanPoseLayer[poseLayerParent.childCount];
                for (int i = 0; i < poseLayerParent.childCount; i++) {
                    poseLayers[i] = new HumanPoseLayer(poseLayerParent.GetChild(i));
                    poseNameToLayerMap.Add(poseLayerParent.GetChild(i).gameObject.name, i);
                }
            }

            isLLegActive = lPelvis != null && lPelvis.gameObject.activeSelf && lFoot != null && lFoot.gameObject.activeSelf;
            isRLegActive = rPelvis != null && rPelvis.gameObject.activeSelf && rFoot != null && rFoot.gameObject.activeSelf;
            isLArmActive = lShoulder != null && lShoulder.gameObject.activeSelf && lHand != null && lHand.gameObject.activeSelf;
            isRArmActive = rShoulder != null && rShoulder.gameObject.activeSelf && rHand != null && rHand.gameObject.activeSelf;

            bodyOriPosLocal = body.localPosition;

            if (isLLegActive) {
                lPelvisOriPosLocal = poseLayers[0].poses[0].lFoot.localPosition - poseLayers[0].poses[0].body.localPosition;
            }

            if (isRLegActive) {
                rPelvisOriPosLocal = poseLayers[0].poses[0].rFoot.localPosition - poseLayers[0].poses[0].body.localPosition;
            }

            if (isLArmActive) {
                lShoulderOriPosLocal = poseLayers[0].poses[0].lHand.localPosition - poseLayers[0].poses[0].body.localPosition;
            }

            if (isRArmActive) {
                rShoulderOriPosLocal = poseLayers[0].poses[0].rHand.localPosition - poseLayers[0].poses[0].body.localPosition;
            }

        }

    }

    [System.Serializable]
    public class HumanPose {
        public Transform parent;
        public HumanPose(Transform parent) {
            if (parent == null) {
                return;
            }
            this.parent = parent;
            body = parent.GetChild(0);
            lHand = parent.GetChild(1);
            rHand = parent.GetChild(2);
            lFoot = parent.GetChild(3);
            rFoot = parent.GetChild(4);
        }
        public Transform body;
        public Transform lHand;
        public Transform rHand;
        public Transform lFoot;
        public Transform rFoot;

    }

    [System.Serializable]
    public class HumanPoseLayer {
        public Transform parent;
        public HumanPose[] poses;
        public HumanPoseLayer(Transform parent) {
            this.parent = parent;
            poses = new HumanPose[parent.childCount];
            for (int i = 0; i < parent.childCount; i++) {
                poses[i] = new HumanPose(parent.GetChild(i));
            }
        }
    }
    
    [System.Serializable]
    public class PoseInfo {
        public Vector3 velocity;
        public Vector3 moveDir;
        // 待添加功能：用目标运动方向代替当前运动方向来判断Roll动画
        public float turnAngle;
        public float speed;
    }

    [System.Serializable]
    public class AnimSetting {
        public float oriSpeed = 5.0f;
        public bool stepRightFootFirst = false;
        [Tooltip("在 Step 过程中 CurFootPos 根据 StepProcess 数值 Lerp 到 TarFootPos 的曲线")]
        public AnimationCurve footPosCurve;
        [Tooltip("在 Step 过程中 FootPosY 根据 StepProcess 数值的 PosY 增量曲线")]
        public AnimationCurve footHeightCurve;
        [Tooltip("一次 Step 会让 Foot 移动多远")]
        public float stepTargetOffset = 0.45f;
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

        [Tooltip("在 Step 过程中 CurHandPos 根据 StepProcess 数值 Lerp 到 TarHandPos 的曲线")]
        public AnimationCurve handPosCurve;
        [Tooltip("一次 Step 手部运动从开始到结束的时间，通常和 StepDuration 一致")]
        public float handPoseDuration = 0.2f;

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

        [Range(0, 1.0f)]
        public float lookAtWeight_head = 0.25f;
        [Range(0, 1.0f)]
        public float lookAtWeight_body = 0.75f;
    }

}
