using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int enemyMaxHP = 1000;
    public int enemyHP;

    [Header("Gem Values")]
    public int redDamage = 2;
    public int blueMana = 3;
    public int greenHeal = 4;

    [Header("Controllers")]
    public CombatUIController ui;
    public CombatProjectileController projectiles;
    public CombatEffectsController effects;

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

        ui.SetupSliders(playerMaxHP, enemyMaxHP, playerMaxMana);
        ui.UpdateUI(playerHP, playerMana, enemyHP);
        ui.UpdateTurnUI(currentState);
    }

    void HandleMatches(Dictionary<GemType, int> matchCounts, Vector3 worldPos)
    {
        if (currentState != CombatState.PlayerTurn)
            return;

        foreach (var pair in matchCounts)
            OnMatch(pair.Key, pair.Value, worldPos);
    }

    void HandleResolveFinished()
    {
        if (currentState == CombatState.PlayerTurn)
            EndPlayerTurn();
    }

    public void OnMatch(GemType type, int count, Vector3 worldPos)
    {
        if (currentState != CombatState.PlayerTurn)
            return;

        switch (type)
        {
            case GemType.Red:
                int totalDamage = redDamage + (count - 3);
                projectiles.FirePlayerProjectile(worldPos, totalDamage, effects.enemyWorldTarget);
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

    public void ClampValues()
    {
        enemyHP = Mathf.Clamp(enemyHP, 0, enemyMaxHP);
        playerHP = Mathf.Clamp(playerHP, 0, playerMaxHP);
        playerMana = Mathf.Clamp(playerMana, 0, playerMaxMana);
    }

    public void UpdateUI()
    {
        ui.UpdateUI(playerHP, playerMana, enemyHP);
    }

    public void EndPlayerTurn()
    {
        if (currentState != CombatState.PlayerTurn)
            return;

        currentState = CombatState.EnemyTurn;
        ui.UpdateTurnUI(currentState);
        ui.UpdateBoardLockVisual(currentState);

        StartCoroutine(EnemyTurnRoutine());
    }

    IEnumerator EnemyTurnRoutine()
    {
        yield return new WaitForSeconds(1f);

        projectiles.FireEnemyProjectile(effects.enemyWorldTarget, effects.playerWorldTarget);

        yield return new WaitForSeconds(1f);

        currentState = CombatState.PlayerTurn;
        ui.UpdateTurnUI(currentState);
        ui.UpdateBoardLockVisual(currentState);
    }
}
