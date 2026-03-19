using UnityEngine;

[System.Serializable]
public class QuestRuntime
{
    public QuestData data;
    public int currentAmount;
    public bool isCompleted;

    public void AddProgress(EnemyData enemyData)
    {
        if (isCompleted) return;

        if (enemyData == data.targetEnemy) // ?? karþýlaþtýrma
        {
            currentAmount++;

            if (currentAmount >= data.requiredAmount)
            {
                Complete();
            }
        }
    }

    void Complete()
    {
        isCompleted = true;
        Debug.Log("Quest Completed: " + data.questName);

        PlayerLevelSystem.Instance.AddXP(data.rewardXP);

        QuestManager.Instance.GiveNextQuest();
    }
}