using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public List<QuestData> questPool;
    public QuestRuntime currentQuest;

    private int questIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GiveNextQuest();
    }

    public void GiveNextQuest()
    {
        if (questIndex >= questPool.Count)
        {
            Debug.Log("All quests completed!");
            return;
        }

        currentQuest = new QuestRuntime
        {
            data = questPool[questIndex],
            currentAmount = 0
        };

        questIndex++;
    }

    public void OnEnemyKilled(EnemyData enemyData)
    {
        currentQuest?.AddProgress(enemyData);
    }
}