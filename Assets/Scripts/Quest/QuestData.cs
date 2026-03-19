using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest System/Quest")]
public class QuestData : ScriptableObject
{
    public string questName;

    public EnemyData targetEnemy;
    public int requiredAmount;

    public int rewardXP;

    public Sprite questIcon; // ?? EKLEDÝK
}