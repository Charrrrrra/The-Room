using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SimulateWater/Setting")]
public class SimWaterSetting : ScriptableObject {
    public float waterMergeDisOffset = 0.2f;
    public float waterMergeMaxRadius = 4.0f;
    public float waterDragStrengthPerInsetDis = 2.0f;
    public float groundDragStrengthPerRadius = 1.0f;
    public float waterMixSpeedPerInsetDis = 0.3f;
    public float waterEvapSpeed = 0.2f;
    public float waterEvapRadius = 2.0f;
    public float waterDrainSpeed = 0.3f;
    public float waterDrainRadius = 0.4f;
}
