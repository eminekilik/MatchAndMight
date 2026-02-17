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
    public int enemyMaxHP = 1000;
    public int enemyHP;

    [Header("Gem Values")]
    public int redDamage = 2;
    public int blueMana = 3;
    public int greenHeal = 4;

    [Header("UI Bars")]
    public Slider playerHPBar;
    public Slider playerManaBar;
    public Slider enemyHPBar;

    [Header("Turn UI")]
    public TMP_Text turnText;

    [Header("Impact Settings")]
    public float flashDuration = 0.12f;
    public float flashScaleAmount = 1.05f;

    [Header("Screen Impact")]
    public Image screenFlashImage;

    [Header("Board Lock Visual")]
    public GameObject boardDarkOverlay;

    [Header("Projectile Prefabs")]
    public GameObject playerProjectilePrefab;
    public GameObject enemyProjectilePrefab;

    [Header("World Targets")]
    public Transform enemyWorldTarget;
    public Transform playerWorldTarget;

    Vector3 enemyBarBaseScale;
    Vector3 playerBarBaseScale;

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

        enemyBarBaseScale = enemyHPBar.transform.localScale;
        playerBarBaseScale = playerHPBar.transform.localScale;

        SetupSliders();
        UpdateUI();
        UpdateTurnUI();

        if (boardDarkOverlay != null)
            boardDarkOverlay.SetActive(false);
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

    void SetupSliders()
    {
        playerHPBar.maxValue = playerMaxHP;
        enemyHPBar.maxValue = enemyMaxHP;
        playerManaBar.maxValue = playerMaxMana;
    }

    void UpdateTurnUI()
    {
        switch (currentState)
        {
            case CombatState.PlayerTurn: turnText.text = "PLAYER TURN"; break;
            case CombatState.EnemyTurn: turnText.text = "ENEMY TURN"; break;
            case CombatState.Win: turnText.text = "YOU WIN"; break;
            case CombatState.Lose: turnText.text = "YOU LOSE"; break;
        }
    }

    public void OnMatch(GemType type, int count, Vector3 worldPos)
    {
        if (currentState != CombatState.PlayerTurn)
            return;

        switch (type)
        {
            case GemType.Red:
                int totalDamage = redDamage + (count - 3);
                enemyHP -= totalDamage;

                FirePlayerProjectile(worldPos);
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

    void FirePlayerProjectile(Vector3 startPos)
    {
        if (playerProjectilePrefab == null || enemyWorldTarget == null)
            return;

        GameObject proj = Instantiate(playerProjectilePrefab, startPos, Quaternion.identity);

        EnergyProjectile ep = proj.GetComponent<EnergyProjectile>();
        if (ep != null)
            ep.SetTarget(enemyWorldTarget);
    }

    void FireEnemyProjectile()
    {
        if (enemyProjectilePrefab == null || playerWorldTarget == null || enemyWorldTarget == null)
            return;

        GameObject proj = Instantiate(enemyProjectilePrefab, enemyWorldTarget.position, Quaternion.identity);

        EnergyProjectile ep = proj.GetComponent<EnergyProjectile>();
        if (ep != null)
            ep.SetTarget(playerWorldTarget);
    }

    void ClampValues()
    {
        enemyHP = Mathf.Clamp(enemyHP, 0, enemyMaxHP);
        playerHP = Mathf.Clamp(playerHP, 0, playerMaxHP);
        playerMana = Mathf.Clamp(playerMana, 0, playerMaxMana);
    }

    void UpdateUI()
    {
        playerHPBar.value = playerHP;
        playerManaBar.value = playerMana;
        enemyHPBar.value = enemyHP;
    }

    public void EndPlayerTurn()
    {
        if (currentState != CombatState.PlayerTurn)
            return;

        currentState = CombatState.EnemyTurn;
        UpdateTurnUI();
        UpdateBoardLockVisual();

        StartCoroutine(EnemyTurnRoutine());
    }

    IEnumerator EnemyTurnRoutine()
    {
        yield return new WaitForSeconds(0.6f);

        int damage = Random.Range(6, 14);
        playerHP -= damage;

        ClampValues();
        UpdateUI();

        FireEnemyProjectile();

        yield return new WaitForSeconds(0.8f);

        currentState = CombatState.PlayerTurn;
        UpdateTurnUI();
        UpdateBoardLockVisual();
    }

    void UpdateBoardLockVisual()
    {
        if (boardDarkOverlay == null)
            return;

        boardDarkOverlay.SetActive(currentState != CombatState.PlayerTurn);
    }
}
