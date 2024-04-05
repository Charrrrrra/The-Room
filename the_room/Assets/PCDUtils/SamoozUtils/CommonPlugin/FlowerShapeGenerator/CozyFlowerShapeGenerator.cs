using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CozyFlowerShapeGenerator : MonoBehaviour {
    public GameObject flowerPrefab;
    public bool overrideEuler;
    public Vector3 overrideEulerValue;
    public float scale = 1.0f;
    public bool generateCircle;
    public float radius = 2.0f;
    public int flowerNumPerCircle = 12;
    public bool generateSquare;
    public float squareSize = 4.0f;
    public int flowerNumPerSide = 7;

    void Start() {
        
    }

    void Update()
    {
        
    }
    

    void OnValidate() {
        if (generateSquare) {
            GameObject flowerHandle = new GameObject("FlowerSquare");
            flowerHandle.transform.SetParent(transform);
            flowerHandle.transform.localPosition = Vector3.zero;
            Vector3[] poss = GenerateSquarePositions(flowerNumPerSide, squareSize);
            foreach (Vector3 pos in poss) {
                Quaternion tarRot;
                if (overrideEuler) {
                    tarRot = Quaternion.Euler(overrideEulerValue);
                } else {
                    tarRot = Quaternion.LookRotation(-pos, Vector3.up);
                }
                SpawnMgr.SpawnGameObject(flowerPrefab, transform.position + pos, tarRot, flowerHandle.transform).transform.localScale = Vector3.one * scale;
            }
            generateSquare = false;
        }
        if (generateCircle) {
            GameObject flowerHandle = new GameObject("FlowerCircle");
            flowerHandle.transform.SetParent(transform);
            flowerHandle.transform.localPosition = Vector3.zero;
            Vector3[] poss = GenerateRingPositions(radius, flowerNumPerCircle);
            foreach (Vector3 pos in poss) {
                Quaternion tarRot;
                if (overrideEuler) {
                    tarRot = Quaternion.Euler(overrideEulerValue);
                } else {
                    tarRot = Quaternion.LookRotation(-pos, Vector3.up);
                }
                SpawnMgr.SpawnGameObject(flowerPrefab, transform.position + pos, tarRot, flowerHandle.transform).transform.localScale = Vector3.one * scale;
            }
            generateCircle = false;
        }
    }

    public Vector3[] GenerateSquarePositions(int numPositionsPerSide, float squareSize)
    {
        // 计算每条边上的位置数量
        int numPositionsPerEdge = numPositionsPerSide - 1;

        // 计算每个位置之间的间距
        float edgeSpacing = squareSize / numPositionsPerEdge;

        // 初始化位置数组
        Vector3[] positions = new Vector3[numPositionsPerSide * 4 - 4];

        // 生成顶边上的位置
        for (int i = 0; i < numPositionsPerSide; i++)
        {
            float x = -squareSize / 2f + i * edgeSpacing;
            positions[i] = new Vector3(x, 0f, squareSize / 2f);
        }

        // 生成右边的位置
        for (int i = 1; i < numPositionsPerSide - 1; i++)
        {
            float z = squareSize / 2f - i * edgeSpacing;
            positions[i + numPositionsPerSide - 1] = new Vector3(squareSize / 2f, 0f, z);
        }

        // 生成底边上的位置
        for (int i = numPositionsPerSide - 1; i >= 0; i--)
        {
            float x = squareSize / 2f - i * edgeSpacing;
            positions[i + 2 * numPositionsPerSide - 2] = new Vector3(x, 0f, -squareSize / 2f);
        }

        // 生成左边的位置
        for (int i = numPositionsPerSide - 2; i > 0; i--)
        {
            float z = -squareSize / 2f + i * edgeSpacing;
            positions[i + 3 * numPositionsPerSide - 3] = new Vector3(-squareSize / 2f, 0f, z);
        }

        return positions;
    }

    public Vector3[] GenerateRingPositions(float radius, int numPositions)
    {
        // 初始化位置数组
        Vector3[] positions = new Vector3[numPositions];

        // 计算角度间隔
        float angleStep = 360f / numPositions;

        // 生成位置
        for (int i = 0; i < numPositions; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = radius * Mathf.Cos(angle);
            float z = radius * Mathf.Sin(angle);
            positions[i] = new Vector3(x, 0f, z);
        }

        return positions;
    }

}
