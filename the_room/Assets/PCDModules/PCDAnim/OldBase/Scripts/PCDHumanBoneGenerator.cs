using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCDHumanBoneGenerator : MonoBehaviour {
    public PCDHuman human;
    public bool generateHumanBone;
    public float pelvisHeight = 0.85f;
    public float pelvisWidth = 0.5f;
    public float shoulderHeight = 1.65f;
    public float shoulderWidth = 0.5f;
    public Vector3 lPelvisPosLocal => new Vector3(-0.5f * pelvisWidth, pelvisHeight, 0);
    public Vector3 rPelvisPosLocal => new Vector3(0.5f * pelvisWidth, pelvisHeight, 0);
    public Vector3 lShoulderPosLocal => new Vector3(-0.5f * shoulderWidth, shoulderHeight, 0);
    public Vector3 rShoulderPosLocal => new Vector3(0.5f * shoulderWidth, shoulderHeight, 0);

    
}
