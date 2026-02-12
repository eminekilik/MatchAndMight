using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance;

    public enum CombatState
    {
        PlayerTurn,
        EnemyTurn,
        Busy,
        Win,
        Lose
    }

    public CombatState currentState;

    [Header("Player")]
    public int playerMaxHP = 100;
    public int playerHP;
    public int playerMaxMana = 100;
    public int playerMana;

    [Header("Enemy")]
    public int enemyMaxHP = 100;
    public int enemyHP;

    [Header("Gem Values")]
    public int redDamage = 5;
    public int blueMana = 3;
    public int greenHeal = 4;

    [Header("UI Bars")]
    public Slider playerHPBar;
    public Slider playerManaBar;
    public Slider enemyHPBar;

    [Header("Turn UI")]
    public TMP_Text turnText;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        MatchDestroyer.Instance.OnMatchesResolved += HandleMatches;
        MatchDestroyer.Instance.OnResolveFinished += HandleResolveFinished;

        currentState = CombatState.PlayerTurn;

        playerHP = playerMaxHP;
        enemyHP = enemyMaxHP;
        playerMana = 0;

        SetupSliders();
        UpdateUI();
        UpdateTurnUI();
    }

    void HandleMatches(Dictionary<GemType, int> matchCounts)
    {
        if (currentState != CombatState.PlayerTurn)
            return;

        foreach (var pair in matchCounts)
        {
            OnMatch(pair.Key, pair.Value);
        }
    }

    void HandleResolveFinished()
    {
        if (currentState == CombatState.PlayerTurn)
            EndPlayerTurn();
    }


    void SetupSliders()
    {
        playerHPBar.maxValue = playerMaxHP;
        enemyHPBar.maxValue = enemyMaxHP;
        playerManaBar.maxValue = playerMaxMana;
    }

    void UpdateTurnUI()
    {
        if (currentState == CombatState.PlayerTurn)
            turnText.text = "PLAYER TURN";
        else if (currentState == CombatState.EnemyTurn)
            turnText.text = "ENEMY TURN";
        else if (currentState == CombatState.Win)
            turnText.text = "YOU WIN";
        else if (currentState == CombatState.Lose)
            turnText.text = "YOU LOSE";
    }

    // MATCH GELDÝÐÝNDE ÇALIÞIR
    public void OnMatch(GemType type, int count)
    {
        if (currentState != CombatState.PlayerTurn)
            return;

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
        if (playerHP < 0) playerHP = 0;
        if (playerHP > playerMaxHP) playerHP = playerMaxHP;
        if (playerMana > playerMaxMana) playerMana = playerMaxMana;
    }

    void UpdateUI()
    {
        playerHPBar.value = playerHP;
        playerManaBar.value = playerMana;
        enemyHPBar.value = enemyHP;
    }

    // PLAYER TURNU BÝTTÝÐÝNDE ÇAÐIR
    public void EndPlayerTurn()
    {
        if (currentState != CombatState.PlayerTurn)
            return;

        currentState = CombatState.Busy;   // ?? önemli
        UpdateTurnUI();

        CheckCombatEnd();
        if (currentState == CombatState.Win || currentState == CombatState.Lose)
            return;

        StartCoroutine(StartEnemyTurnDelayed());
    }

    IEnumerator StartEnemyTurnDelayed()
    {
        yield return new WaitForSeconds(0.4f);

        currentState = CombatState.EnemyTurn;
        UpdateTurnUI();

        StartCoroutine(EnemyTurnRoutine());
    }

    IEnumerator EnemyTurnRoutine()
    {
        yield return new WaitForSeconds(0.6f);

        int damage = Random.Range(6, 14);
        playerHP -= damage;

        ClampValues();
        UpdateUI();

        yield return new WaitForSeconds(0.8f);

        CheckCombatEnd();

        if (currentState != CombatState.Win && currentState != CombatState.Lose)
        {
            currentState = CombatState.PlayerTurn;
            UpdateTurnUI();
        }
    }

    void CheckCombatEnd()
    {
        UpdateTurnUI();

        if (enemyHP <= 0)
        {
            currentState = CombatState.Win;
            UpdateTurnUI();
            Debug.Log("YOU WIN");
        }
        else if (playerHP <= 0)
        {
            currentState = CombatState.Lose;
            UpdateTurnUI();
            Debug.Log("YOU LOSE");
        }
    }
}
