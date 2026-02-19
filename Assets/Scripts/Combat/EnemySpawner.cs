using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject[] enemyPrefabs;

    [Header("Spawn Point")]
    public Transform spawnPoint;

    void Start()
    {
        SpawnEnemy();
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("Enemy Prefabs boþ!");
            return;
        }

        int index = Random.Range(0, enemyPrefabs.Length);

        GameObject spawnedEnemy = Instantiate(
            enemyPrefabs[index],
            spawnPoint.position,
            Quaternion.identity
        );

        // CombatManager'a bildir
        //CombatManager.Instance.SetEnemy(spawnedEnemy.transform);
        CombatManager.Instance.effects.SetEnemy(spawnedEnemy.transform);

    }

}
