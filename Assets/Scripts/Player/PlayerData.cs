using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Base Stats")]
    public int baseHealth = 100;
    public int baseAttack = 2;
    public int baseManaGain = 3;

    [Header("Growth Per Level")]
    public int healthPerLevel = 15;
    public int attackPerLevel = 1;
    public int manaPerLevel = 5;

    [Header("Upgrade Bonuses")]
    public int bonusHealth = 0;
    public int bonusAttack = 0;
    public int bonusMana = 0;

    [Header("Current Stats")]
    public int currentHealth;
    public int currentAttack;
    public int currentManaGain;

    public void RecalculateStats(int level)
    {
        currentHealth = baseHealth
            + (healthPerLevel * (level - 1))
            + bonusHealth;

        currentAttack = baseAttack
            + (attackPerLevel * (level - 1))
            + bonusAttack;

        currentManaGain = baseManaGain
            + (manaPerLevel * (level - 1))
            + bonusMana;
    }
}