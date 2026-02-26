using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyDatabase database;

    [Header("Spawn Point")]
    public Transform spawnPoint;

    private GameObject currentEnemy;

    void Start()
    {
        SpawnEnemy();
    }

    void SpawnEnemy()
    {
        string id = BattleData.selectedEnemyID;

        if (string.IsNullOrEmpty(id))
        {
            Debug.LogWarning("Enemy ID boþ!");
            return;
        }

        EnemyData data = database.GetEnemyByID(id);

        if (data == null)
        {
            Debug.LogWarning("Enemy data bulunamadý!");
            return;
        }

        currentEnemy = Instantiate(
            data.battlePrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        CombatManager.Instance.ui.SetEnemyBarActive(true);
        CombatManager.Instance.effects.SetEnemy(currentEnemy.transform);

        Animator anim = currentEnemy.GetComponent<Animator>();

        if (anim != null)
        {
            CombatManager.Instance.SetEnemyAnimator(anim);
        }
    }

    public void DestroyCurrentEnemy()
    {
        if (currentEnemy != null)
        {
            CombatManager.Instance.effects.ClearEnemy();
            CombatManager.Instance.ui.SetEnemyBarActive(false);
            Destroy(currentEnemy);
        }
    }
}