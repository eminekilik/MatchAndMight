using System.Collections.Generic;
using UnityEngine;

public class WorldEnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject[] enemyPrefabs;
    public int enemyCount = 5;

    [Header("Spawn Area Settings")]
    public float spawnWidth = 10f;
    public float spawnHeight = 6f;

    [Header("Spawn Rules")]
    public float minDistanceBetweenEnemies = 1.5f;

    private List<Vector3> usedPositions = new List<Vector3>();

    void Start()
    {
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        if (enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("Enemy prefab yok!");
            return;
        }

        List<GameObject> spawnPool = new List<GameObject>();

        while (spawnPool.Count < enemyCount)
        {
            List<GameObject> tempList = new List<GameObject>(enemyPrefabs);

            for (int i = 0; i < tempList.Count; i++)
            {
                GameObject temp = tempList[i];
                int randomIndex = Random.Range(i, tempList.Count);
                tempList[i] = tempList[randomIndex];
                tempList[randomIndex] = temp;
            }

            foreach (var enemy in tempList)
            {
                if (spawnPool.Count < enemyCount)
                    spawnPool.Add(enemy);
            }
        }

        foreach (var enemy in spawnPool)
        {
            Vector3 randomPos = GetRandomPosition();
            Instantiate(enemy, randomPos, Quaternion.identity);
        }
    }

    Vector3 GetRandomPosition()
    {
        Vector3 randomPos;
        int safety = 0;

        do
        {
            float randomX = Random.Range(-spawnWidth / 2f, spawnWidth / 2f);
            float randomY = Random.Range(-spawnHeight / 2f, spawnHeight / 2f);

            randomPos = transform.position + new Vector3(randomX, randomY, 0);

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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnWidth, spawnHeight, 0));
    }
}