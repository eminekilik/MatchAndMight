using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Base Stats")]
    public int baseHealth = 100;
    public int baseAttack = 2;
    public int baseMana = 100;

    [Header("Growth Per Level")]
    public int healthPerLevel = 15;
    public int attackPerLevel = 1;
    public int manaPerLevel = 5;

    [Header("Upgrade Bonuses")]
    public int bonusHealth = 0;
    public int bonusAttack = 0;
    public int bonusMana = 0;
}