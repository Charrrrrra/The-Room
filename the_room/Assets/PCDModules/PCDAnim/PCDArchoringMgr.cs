using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(PCDSkeleton))]
[RequireComponent(typeof(PCDParameter))]
public class PCDArchoringMgr : MonoBehaviour
{
    public bool[] getOwnershipOnAwake = new bool[5] {false,false,false,false,false};
    private PCDAnimator animator;
    public PCDSkeleton skeleton;

    private Dictionary<string, PCDBoneDriver> nameDict;
    private void Awake() {
        animator = GetComponent<PCDAnimator>();
        skeleton = GetComponent<PCDSkeleton>();

        body = new PCDBoneDriver(skeleton.GetBone("Body"), getOwnershipOnAwake[0]);
        lFoot = new PCDBoneDriver(skeleton.GetBone("LFoot"), getOwnershipOnAwake[1]);
        rFoot = new PCDBoneDriver(skeleton.GetBone("RFoot"), getOwnershipOnAwake[2]);
        lHand = new PCDBoneDriver(skeleton.GetBone("LHand"), getOwnershipOnAwake[3]);
        rHand = new PCDBoneDriver(skeleton.GetBone("RHand"), getOwnershipOnAwake[4]);

        nameDict = new Dictionary<string, PCDBoneDriver> {
            {"Body", body},
            {"LFoot", lFoot},
            {"RFoot", rFoot},
            {"LHand", lHand},
            {"RHand", rHand}
        };
        targetDict = new Dictionary<string, Transform>();
        lerpRemTimeDict = new Dictionary<string, float>();
    }

    [SerializeField] private PCDBoneDriver body;
    [SerializeField] private PCDBoneDriver lFoot;
	[SerializeField] private PCDBoneDriver rFoot;
	[SerializeField] private PCDBoneDriver lHand;
	[SerializeField] private PCDBoneDriver rHand;

    public Transform testTrans;

    public void UpdateArchoring() {
        
        if (Input.GetKeyUp(KeyCode.M)) {
            BoneArchoringToTransform("LHand", testTrans);
            BoneArchoringToTransform("RHand", testTrans);
        }
        if (Input.GetKeyUp(KeyCode.N)) {
            ResetBoneFromArchoring("LHand");
            ResetBoneFromArchoring("RHand");
        }
       

        foreach (var partName in nameDict.Keys) {
            var driver = nameDict[partName];
            if (!driver.CheckBoneOwnership())
                continue;

            if (targetDict.TryGetValue(partName, out Transform target) && target != null) {
                float lerpTime = lerpRemTimeDict[partName];
                if (lerpTime > 0.001f) {
                    lerpRemTimeDict[partName] -= Time.deltaTime;
                    float posVelocity = (target.position - driver.attachedBone.transform.position).magnitude / lerpTime;
                    Vector3 posRes = Vector3.Lerp(driver.attachedBone.transform.position, target.position, Time.deltaTime * posVelocity);
                    driver.SetGlobalPosition(posRes);
                } else {
                    driver.SetGlobalPosition(target.position);
                }
                driver.SetGlobalRotation(target.rotation);
            }
        }
    }

    private Dictionary<string, Transform> targetDict;
    private Dictionary<string, float> lerpRemTimeDict;
    public void BoneArchoringToTransform(string partName, Transform target, float lerpTime = 0.06f) {
        nameDict[partName].attachedBone.transform.DOKill();
        nameDict[partName].TryGetOwnership();
        targetDict[partName] = target;
        lerpRemTimeDict[partName] = lerpTime;
    }

    public void ResetBoneFromArchoring(string partName) {
        targetDict[partName] = null;
        nameDict[partName].ReturnOwnership();
    }
}
