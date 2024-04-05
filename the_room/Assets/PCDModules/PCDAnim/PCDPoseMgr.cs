using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(PCDSkeleton))]
[RequireComponent(typeof(PCDParameter))]
public class PCDPoseMgr : MonoBehaviour
{
    public bool[] getOwnershipOnAwake = new bool[5] {false,false,false,false,false};
    private PCDAnimator animator;

    public PCDSkeleton skeleton;
	public PCDParameter pcdPara;

    private Dictionary<string, PCDBoneDriver> nameDict;
    private void Awake() {
        animator = GetComponent<PCDAnimator>();
        skeleton = GetComponent<PCDSkeleton>();
        pcdPara = GetComponent<PCDParameter>();

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
    }

    [SerializeField] private PCDBoneDriver body;
    [SerializeField] private PCDBoneDriver lFoot;
	[SerializeField] private PCDBoneDriver rFoot;
	[SerializeField] private PCDBoneDriver lHand;
	[SerializeField] private PCDBoneDriver rHand;
    
    public void FadeToKeyFrame(PCDKFReader kfReader, bool ctlBody = true, bool ctlLHand = true, bool ctlRHand = true, bool ctlLFoot = true, bool ctlRFoot = true) {
        if (ctlBody)
            ctlBody = ctlBody && body.TryGetOwnership();
        if (ctlLFoot)
            ctlLFoot = ctlLFoot && lFoot.TryGetOwnership();
        if (ctlRFoot)
            ctlRFoot = ctlRFoot && rFoot.TryGetOwnership();
        if (ctlLHand)
            ctlLHand = ctlLHand && lHand.TryGetOwnership();
        if (ctlRHand)
            ctlRHand = ctlRHand && rHand.TryGetOwnership();

        foreach (var boneName in nameDict.Keys) {
            PCDBoneDriver driver = nameDict[boneName];
            driver.attachedBone.transform.DOKill();
            driver.FadeBoneToKeyFrame(kfReader);
        }
    }

    public void ReleasePose() {
        foreach (var driver in nameDict.Values) {
            driver.ReturnOwnership();
        }
    }
}
