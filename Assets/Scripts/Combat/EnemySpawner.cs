using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject[] enemyPrefabs;

    [Header("Spawn Point")]
    public Transform spawnPoint;

    private GameObject currentEnemy;

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

        currentEnemy = Instantiate(
            enemyPrefabs[index],
            spawnPoint.position,
            Quaternion.identity
        );

        CombatManager.Instance.effects.SetEnemy(currentEnemy.transform);
    }

    public void DestroyCurrentEnemy()
    {
        if (currentEnemy != null)
        {
            CombatManager.Instance.effects.ClearEnemy();
            Destroy(currentEnemy);
        }
    }
}