using System.Collections.Generic;
using UnityEngine;

static public class RandomWeightChoose {
    static public T Choose<T>(List<RandomWeightInfo<T>> weightInfos) {
        float totalWeight = 0;
        foreach (var info in weightInfos)
            totalWeight += info.weight;
        float randomValue = Random.Range(0, totalWeight);
        foreach (var info in weightInfos) {
            randomValue -= info.weight;
            if (randomValue <= 0)
                return info.value;
        }
        Debug.LogError("random failed, please fix this bug");
        return default(T);
    }
}

[System.Serializable]
public struct RandomWeightInfo<T> {
    public T value;
    public float weight;

    public RandomWeightInfo(T value, float weight) {
        this.value = value;
        this.weight = weight;
    }
}