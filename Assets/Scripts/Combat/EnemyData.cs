using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyID;

    [Header("Prefabs")]
    public GameObject battlePrefab;

    [Header("Sprites")]
    public Sprite worldSprite;

    [Header("Stats")]
    public int maxHealth;
    public int attack;
}