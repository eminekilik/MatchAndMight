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

    [Header("UI References")]
    public RectTransform playerHPBarRect;
    public RectTransform enemyHPBarRect;
    public Canvas canvas;
    public Camera mainCamera;

    [Header("Impact Settings")]
    public float flashDuration = 0.12f;
    public float flashScaleAmount = 1.05f;

    [Header("Screen Impact")]
    public Image screenFlashImage; // full screen beyaz image


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
            OnMatch(pair.Key, pair.Value);
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

    public void EndPlayerTurn()
    {
        if (currentState != CombatState.PlayerTurn)
            return;

        currentState = CombatState.Busy;
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

        StartCoroutine(SpawnImpactProjectile());
        StartCoroutine(HPHitEffect());
        StartCoroutine(ScreenImpact());

        yield return new WaitForSeconds(0.8f);

        CheckCombatEnd();

        if (currentState != CombatState.Win && currentState != CombatState.Lose)
        {
            currentState = CombatState.PlayerTurn;
            UpdateTurnUI();
        }
    }

    IEnumerator SpawnImpactProjectile()
    {
        if (enemyHPBarRect == null || playerHPBarRect == null || canvas == null)
            yield break;

        GameObject impact = new GameObject("Impact");
        impact.transform.SetParent(canvas.transform, false);

        Image img = impact.AddComponent<Image>();
        img.color = Color.red;

        RectTransform rect = impact.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(50, 50);

        Vector3 startPos = enemyHPBarRect.position;
        Vector3 endPos = playerHPBarRect.position;

        rect.position = startPos;

        float duration = 0.25f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            Vector3 arc = Vector3.up * Mathf.Sin(t * Mathf.PI) * 40f;
            rect.position = Vector3.Lerp(startPos, endPos, t) + arc;

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(impact);
    }

    IEnumerator HPHitEffect()
    {
        if (playerHPBar == null || playerHPBar.fillRect == null)
            yield break;

        Image fillImage = playerHPBar.fillRect.GetComponent<Image>();
        if (fillImage == null)
            yield break;

        Color originalColor = fillImage.color;
        Vector3 originalScale = playerHPBarRect.localScale;

        fillImage.color = Color.red;
        playerHPBarRect.localScale = originalScale * flashScaleAmount;

        yield return new WaitForSeconds(flashDuration);

        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            fillImage.color = Color.Lerp(Color.red, originalColor, elapsed / flashDuration);
            playerHPBarRect.localScale =
                Vector3.Lerp(originalScale * flashScaleAmount, originalScale, elapsed / flashDuration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        fillImage.color = originalColor;
        playerHPBarRect.localScale = originalScale;
    }

    IEnumerator ScreenImpact()
    {
        if (screenFlashImage == null || mainCamera == null)
            yield break;

        Vector3 originalScale = mainCamera.transform.localScale;

        float zoomAmount = 0.03f;     // çok küçük
        float zoomTime = 0.08f;
        float returnTime = 0.18f;

        float flashTime = 0.06f;

        // 1?? Flash
        screenFlashImage.color = new Color(1, 1, 1, 0.35f);

        // 2?? Çok hafif zoom in
        float t = 0;
        while (t < zoomTime)
        {
            mainCamera.transform.localScale =
                Vector3.Lerp(originalScale, originalScale * (1 + zoomAmount), t / zoomTime);

            t += Time.deltaTime;
            yield return null;
        }

        // Flash fade
        float flashElapsed = 0;
        while (flashElapsed < flashTime)
        {
            float a = Mathf.Lerp(0.35f, 0f, flashElapsed / flashTime);
            screenFlashImage.color = new Color(1, 1, 1, a);

            flashElapsed += Time.deltaTime;
            yield return null;
        }

        // 3?? Smooth geri dönüþ
        t = 0;
        while (t < returnTime)
        {
            float ease = 1 - Mathf.Pow(1 - (t / returnTime), 3);
            mainCamera.transform.localScale =
                Vector3.Lerp(originalScale * (1 + zoomAmount), originalScale, ease);

            t += Time.deltaTime;
            yield return null;
        }

        screenFlashImage.color = new Color(1, 1, 1, 0);
        mainCamera.transform.localScale = originalScale;
    }


    void CheckCombatEnd()
    {
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
