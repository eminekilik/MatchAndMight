using UnityEngine;

public class EnemyDatabase : MonoBehaviour
{
    public EnemyData[] allEnemies;

    public EnemyData GetEnemyByID(string id)
    {
        foreach (var enemy in allEnemies)
        {
            if (enemy.enemyID == id)
                return enemy;
        }

        return null;
    }
}