using UnityEngine;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public List<QuestData> questPool;
    public QuestRuntime currentQuest;

    private int questIndex = 0;

    const string QUEST_INDEX_KEY = "QuestIndex";
    const string QUEST_PROGRESS_KEY = "QuestProgress";

    [HideInInspector] public int pendingRewardXP = 0;
    [HideInInspector] public bool hasPendingReward = false;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadQuest(); // ?? BURASI DEÐÝÞTÝ
    }

    // ?? Yeni quest verme
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

        SaveQuest(); // ?? yeni quest gelince kaydet
    }

    // ?? Enemy öldüðünde
    public void OnEnemyKilled(EnemyData enemyData)
    {
        currentQuest?.AddProgress(enemyData);
        SaveQuest(); // ?? progress kaydet
    }

    // ?? SAVE
    public void SaveQuest()
    {
        if (currentQuest == null) return;

        PlayerPrefs.SetInt(QUEST_INDEX_KEY, questIndex - 1);
        PlayerPrefs.SetInt(QUEST_PROGRESS_KEY, currentQuest.currentAmount);

        PlayerPrefs.Save();
    }

    // ?? LOAD
    public void LoadQuest()
    {
        if (PlayerPrefs.HasKey(QUEST_INDEX_KEY))
        {
            int savedIndex = PlayerPrefs.GetInt(QUEST_INDEX_KEY);
            int savedProgress = PlayerPrefs.GetInt(QUEST_PROGRESS_KEY);

            questIndex = savedIndex;

            currentQuest = new QuestRuntime
            {
                data = questPool[questIndex],
                currentAmount = savedProgress
            };

            questIndex++; // sýradaki quest için
        }
        else
        {
            GiveNextQuest(); // ilk oyun
        }
    }
}