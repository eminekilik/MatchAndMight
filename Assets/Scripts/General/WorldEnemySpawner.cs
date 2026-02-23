using System.Collections.Generic;
using UnityEngine;

public class WorldEnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject[] enemyPrefabs;
    public int enemyCount = 5;

    [Header("Spawn Area")]
    public BoxCollider2D spawnArea;

    [Header("Spawn Rules")]
    public float minDistanceBetweenEnemies = 1.5f;

    private List<Vector3> usedPositions = new List<Vector3>();

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        if (enemyPrefabs.Length == 0 || spawnArea == null)
        {
            Debug.LogWarning("Eksik ayar var!");
            return;
        }

        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 randomPos = GetRandomPosition();
            GameObject randomEnemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Instantiate(randomEnemy, randomPos, Quaternion.identity);
        }
    }

    Vector3 GetRandomPosition()
    {
        Vector3 randomPos;
        int safety = 0;

        Bounds bounds = spawnArea.bounds;

        do
        {
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);

            randomPos = new Vector3(randomX, randomY, 0);

            safety++;
            if (safety > 100) break;

        } while (!IsPositionValid(randomPos));

        usedPositions.Add(randomPos);
        return randomPos;
    }

    bool IsPositionValid(Vector3 pos)
    {
        foreach (Vector3 usedPos in usedPositions)
        {
            if (Vector3.Distance(pos, usedPos) < minDistanceBetweenEnemies)
                return false;
        }

        return true;
    }
}