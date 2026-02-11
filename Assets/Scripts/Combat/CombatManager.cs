using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    [Header("Player")]
    public int playerMaxHP = 100;
    public int playerHP;
    public int playerMana = 0;

    [Header("Enemy")]
    public int enemyMaxHP = 100;
    public int enemyHP;

    [Header("Gem Values")]
    public int redDamage = 5;
    public int blueMana = 3;
    public int greenHeal = 4;

    [Header("UI")]
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI playerManaText;
    public TextMeshProUGUI enemyHPText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerHP = playerMaxHP;
        enemyHP = enemyMaxHP;
        UpdateUI();
    }

    public void OnMatch(GemType type, int count)
    {
        switch (type)
        {
            case GemType.Red:
                enemyHP -= count * redDamage;
                break;

            case GemType.Blue:
                playerMana += count * blueMana;
                break;

            case GemType.Green:
                playerHP += count * greenHeal;
                break;
        }

        ClampValues();
        UpdateUI();
    }

    void ClampValues()
    {
        if (enemyHP < 0) enemyHP = 0;
        if (playerHP > playerMaxHP) playerHP = playerMaxHP;
    }

    void UpdateUI()
    {
        playerHPText.text = "Player HP: " + playerHP;
        playerManaText.text = "Mana: " + playerMana;
        enemyHPText.text = "Enemy HP: " + enemyHP;
    }
}
