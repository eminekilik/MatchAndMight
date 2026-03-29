using System.Collections.Generic;
using UnityEngine;

public class TreeSpawner : MonoBehaviour
{
    [Header("Tree Settings")]
    public GameObject[] treePrefabs;
    public int treeCount = 10;

    [Header("Spawn Area Settings")]
    public float spawnWidth = 15f;
    public float spawnHeight = 10f;

    [Header("Spawn Rules")]
    public float minDistanceBetweenTrees = 2f;

    private List<Vector3> usedPositions = new List<Vector3>();

    void Start()
    {
        SpawnTrees();
    }

    void SpawnTrees()
    {
        if (treePrefabs.Length == 0)
        {
            Debug.LogWarning("Tree prefab yok!");
            return;
        }

        List<GameObject> spawnPool = new List<GameObject>();

        // Prefablarý karýþtýrarak dolduruyoruz
        while (spawnPool.Count < treeCount)
        {
            List<GameObject> tempList = new List<GameObject>(treePrefabs);

            for (int i = 0; i < tempList.Count; i++)
            {
                GameObject temp = tempList[i];
                int randomIndex = Random.Range(i, tempList.Count);
                tempList[i] = tempList[randomIndex];
                tempList[randomIndex] = temp;
            }

            foreach (var tree in tempList)
            {
                if (spawnPool.Count < treeCount)
                    spawnPool.Add(tree);
            }
        }

        // Spawn iþlemi
        foreach (var tree in spawnPool)
        {
            Vector3 randomPos = GetRandomPosition();
            Instantiate(tree, randomPos, Quaternion.identity);
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
            if (Vector3.Distance(pos, usedPos) < minDistanceBetweenTrees)
                return false;
        }

        return true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnWidth, spawnHeight, 0));
    }
}