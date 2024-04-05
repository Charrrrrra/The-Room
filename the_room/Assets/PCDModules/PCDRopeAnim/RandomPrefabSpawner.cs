using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPrefabSpawner : MonoBehaviour
{
    public GameObject[] prefabs; // 存放预设的数组
    public int numberOfObjects = 10; // 生成的预设数量
    public float spawnRadius = 5f; // 生成的半径范围
    public float spawnInterval = 2f; // 生成的时间间隔（秒）

    private void Start()
    {
        // 开始生成预设的循环
        StartCoroutine(SpawnPrefabs());
    }

    private IEnumerator SpawnPrefabs()
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            // 在指定半径内随机生成位置
            Vector3 spawnPosition = RandomPositionWithinRadius();
            // 随机选择一个预设进行生成
            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
            // 在计算出的位置实例化预设
            SpawnMgr.SpawnGameObject(prefab, spawnPosition, Quaternion.identity);
            // 等待指定的时间间隔
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private Vector3 RandomPositionWithinRadius()
    {
        // 在半径内随机选择一个点作为生成位置
        Vector2 randomDirection = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomDirection.x, 0, randomDirection.y);
        return spawnPosition;
    }
}
