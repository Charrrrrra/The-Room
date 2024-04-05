using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeAnimator : MonoBehaviour
{
    public Rigidbody[] segments; // 毛毛虫的身体部分刚体数组
    public int fixedSegmentIndex = 5; // 假定第6个部分被捏住
    public float baseTorqueStrength = 10f; // 扭矩的基础强度
    public float baseWaveSpeed = 5f; // 控制扭矩波动的基础速度
    public float strengthFalloff = 0.1f; // 扭矩强度随距离的衰减系数
    public float torqueAmplitudeOffset = 5f; // 扭矩强度振幅的偏移量
    public float torqueFrequency = 1f; // 扭矩强度变化的频率
    private float sineWaveOffset; // 正弦波的随机偏移量

    void Start()
    {
        // 为每个毛毛虫生成一个随机的正弦波偏移量
        sineWaveOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    void FixedUpdate()
    {
        float maxDistance = segments.Length - 1 - fixedSegmentIndex;
        maxDistance = Mathf.Max(maxDistance, fixedSegmentIndex); // 从中间到两端的最大距离

        // 使用正弦函数动态调整扭矩强度，加入随机偏移量，基于偏移而非乘数
        float dynamicTorqueStrength = baseTorqueStrength;
        // float dynamicTorqueStrength = baseTorqueStrength + (Mathf.Sin((Time.time * torqueFrequency) + sineWaveOffset) * torqueAmplitudeOffset);

        for (int i = 0; i < segments.Length; i++)
        {
            if (i != fixedSegmentIndex) // 跳过被固定的部分
            {
                float distanceToFixed = Mathf.Abs(i - fixedSegmentIndex);
                float normalizedDistance = distanceToFixed / maxDistance;
                float distanceFalloff = 1f;
                // float distanceFalloff = 1 - (normalizedDistance * strengthFalloff);

                float wave = Mathf.Sin(Time.time * baseWaveSpeed + i * distanceToFixed + sineWaveOffset) * dynamicTorqueStrength * distanceFalloff;
                Vector3 localTorqueDirection = (i < fixedSegmentIndex ? Vector3.left : Vector3.right) * wave;
                Vector3 worldTorqueDirection = segments[i].transform.TransformDirection(localTorqueDirection);
                segments[i].AddTorque(worldTorqueDirection, ForceMode.Force);
            }
        }
    }
}
