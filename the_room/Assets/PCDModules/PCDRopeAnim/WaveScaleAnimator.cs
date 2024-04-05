using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveScaleAnimator : MonoBehaviour
{
    public GameObject[] bodySegments; // 蛆虫的体节数组
    public float peakMagnitude = 0.2f; // 波峰的最大幅度
    public float baseWaveInterval = 2f; // 基本心跳周期的持续时间
    public float waveIntervalOffsetRange = 0.5f; // 心跳周期间隔的随机偏移范围
    public float waveWidth = 1f; // 控制波形宽度
    private Vector3[] originalScales; // 存储每个体节原始的localScale
    private float waveInterval; // 实际使用的心跳周期间隔
    private float lastWaveTime; // 记录上次波动的时间

    void Start()
    {
        originalScales = new Vector3[bodySegments.Length];
        for (int i = 0; i < bodySegments.Length; i++)
        {
            originalScales[i] = bodySegments[i].transform.localScale;
        }
        
        // 在基本心跳周期上加上一个随机偏移量
        waveInterval = baseWaveInterval + Random.Range(-waveIntervalOffsetRange, waveIntervalOffsetRange);
        lastWaveTime = Time.time;
    }

    void Update()
    {
        if (Time.time - lastWaveTime >= waveInterval)
        {
            StartCoroutine(AnimateSegmentsWave());
            lastWaveTime = Time.time;
            // Optionally, adjust waveInterval again for the next cycle
            waveInterval = baseWaveInterval + Random.Range(-waveIntervalOffsetRange, waveIntervalOffsetRange);
        }
    }

    IEnumerator AnimateSegmentsWave()
    {
        float delayBetweenSegments = waveWidth / bodySegments.Length;
        for (int i = 0; i < bodySegments.Length; i++)
        {
            StartCoroutine(ScaleSegment(bodySegments[i], originalScales[i], i * delayBetweenSegments));
        }
        yield return new WaitForSeconds(waveWidth);
    }

    IEnumerator ScaleSegment(GameObject segment, Vector3 originalScale, float startDelay)
    {
        yield return new WaitForSeconds(startDelay);

        float duration = waveWidth;
        float time = 0;
        while (time < duration)
        {
            float phase = (time / duration) * Mathf.PI * 2;
            float scale = Mathf.Sin(phase) * peakMagnitude;
            if (scale < 0) scale = 0; // Ensure no negative scaling
            segment.transform.localScale = originalScale * (1 + scale);
            time += Time.deltaTime;
            yield return null;
        }

        segment.transform.localScale = originalScale;
    }
}
