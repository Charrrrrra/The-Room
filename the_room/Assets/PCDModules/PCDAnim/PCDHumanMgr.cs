using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCDHumanMgr : MonoBehaviour
{
    public PCDSkeleton skeleton;
    public PCDParameter pcdPara;
    public PCDAnimator animator;
    public PCDWalkMgr walkMgr;
    public PCDPoseMgr poseMgr;
    public PCDArchoringMgr archoringMgr;
    public Animator uanimator;
    void Awake() {
        skeleton = GetComponent<PCDSkeleton>();
        pcdPara = GetComponent<PCDParameter>();
        animator = GetComponent<PCDAnimator>();
        walkMgr = GetComponent<PCDWalkMgr>();
        poseMgr = GetComponent<PCDPoseMgr>();
        archoringMgr = GetComponent<PCDArchoringMgr>();
        uanimator = GetComponent<Animator>();
        if (uanimator) {
            uanimator.enabled = false;
        }
    }

    void Update() {
        pcdPara?.UpdateParameter();
        walkMgr?.UpdateWalk();
        archoringMgr?.UpdateArchoring();
    }

}
