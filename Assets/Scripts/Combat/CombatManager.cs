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
    public Image screenFlashImage;

    // Cached values
    Vector3 enemyBarBaseScale;
    Vector3 playerBarBaseScale;

    Coroutine enemyHitRoutine;
    Coroutine enemyShakeRoutine;

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

        enemyBarBaseScale = enemyHPBarRect.localScale;
        playerBarBaseScale = playerHPBarRect.localScale;

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
        switch (currentState)
        {
            case CombatState.PlayerTurn:
                turnText.text = "PLAYER TURN";
                break;
            case CombatState.EnemyTurn:
                turnText.text = "ENEMY TURN";
                break;
            case CombatState.Win:
                turnText.text = "YOU WIN";
                break;
            case CombatState.Lose:
                turnText.text = "YOU LOSE";
                break;
        }
    }

    public void OnMatch(GemType type, int count)
    {
        if (currentState != CombatState.PlayerTurn)
            return;

        switch (type)
        {
            case GemType.Red:
                enemyHP -= count * redDamage;

                StartCoroutine(SpawnPlayerImpactProjectile());
                TriggerEnemyHitEffect();
                TriggerEnemyShake();
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

    #region Enemy Hit Effects

    void TriggerEnemyHitEffect()
    {
        if (enemyHitRoutine != null)
            StopCoroutine(enemyHitRoutine);

        enemyHitRoutine = StartCoroutine(EnemyHPHitEffect());
    }

    IEnumerator EnemyHPHitEffect()
    {
        if (enemyHPBar.fillRect == null)
            yield break;

        Image fillImage = enemyHPBar.fillRect.GetComponent<Image>();
        if (fillImage == null)
            yield break;

        Color originalColor = fillImage.color;

        fillImage.color = Color.red;
        enemyHPBarRect.localScale = enemyBarBaseScale * flashScaleAmount;

        yield return new WaitForSeconds(flashDuration);

        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            float t = elapsed / flashDuration;

            fillImage.color = Color.Lerp(Color.red, originalColor, t);
            enemyHPBarRect.localScale =
                Vector3.Lerp(enemyBarBaseScale * flashScaleAmount, enemyBarBaseScale, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        fillImage.color = originalColor;
        enemyHPBarRect.localScale = enemyBarBaseScale;

        enemyHitRoutine = null;
    }

    void TriggerEnemyShake()
    {
        if (enemyShakeRoutine != null)
            StopCoroutine(enemyShakeRoutine);

        enemyShakeRoutine = StartCoroutine(EnemyBarShake());
    }

    IEnumerator EnemyBarShake()
    {
        Vector3 originalPos = enemyHPBarRect.localPosition;

        float duration = 0.15f;
        float strength = 8f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * strength;
            float offsetY = Random.Range(-1f, 1f) * strength;

            enemyHPBarRect.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        enemyHPBarRect.localPosition = originalPos;
        enemyShakeRoutine = null;
    }

    #endregion

    IEnumerator SpawnPlayerImpactProjectile()
    {
        if (!enemyHPBarRect || !playerHPBarRect || !canvas)
            yield break;

        GameObject impact = new GameObject("PlayerImpact");
        impact.transform.SetParent(canvas.transform, false);

        Image img = impact.AddComponent<Image>();
        img.color = Color.red;

        RectTransform rect = impact.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(50, 50);

        Vector3 startPos = playerHPBarRect.position;
        Vector3 endPos = enemyHPBarRect.position;

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
        GameObject impact = new GameObject("Impact");
        impact.transform.SetParent(canvas.transform, false);

        Image img = impact.AddComponent<Image>();
        img.color = Color.red;

        RectTransform rect = impact.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(50, 50);

        Vector3 startPos = enemyHPBarRect.position;
        Vector3 endPos = playerHPBarRect.position;

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
        Image fillImage = playerHPBar.fillRect.GetComponent<Image>();
        Color originalColor = fillImage.color;

        playerHPBarRect.localScale = playerBarBaseScale * flashScaleAmount;
        fillImage.color = Color.red;

        yield return new WaitForSeconds(flashDuration);

        float elapsed = 0f;

        while (elapsed < flashDuration)
        {
            float t = elapsed / flashDuration;

            fillImage.color = Color.Lerp(Color.red, originalColor, t);
            playerHPBarRect.localScale =
                Vector3.Lerp(playerBarBaseScale * flashScaleAmount, playerBarBaseScale, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        fillImage.color = originalColor;
        playerHPBarRect.localScale = playerBarBaseScale;
    }

    IEnumerator ScreenImpact()
    {
        Vector3 originalScale = mainCamera.transform.localScale;

        float zoomAmount = 0.03f;
        float zoomTime = 0.08f;
        float returnTime = 0.18f;
        float flashTime = 0.06f;

        screenFlashImage.color = new Color(1, 1, 1, 0.35f);

        float t = 0;
        while (t < zoomTime)
        {
            mainCamera.transform.localScale =
                Vector3.Lerp(originalScale, originalScale * (1 + zoomAmount), t / zoomTime);

            t += Time.deltaTime;
            yield return null;
        }

        float flashElapsed = 0;
        while (flashElapsed < flashTime)
        {
            float a = Mathf.Lerp(0.35f, 0f, flashElapsed / flashTime);
            screenFlashImage.color = new Color(1, 1, 1, a);

            flashElapsed += Time.deltaTime;
            yield return null;
        }

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
        }
        else if (playerHP <= 0)
        {
            currentState = CombatState.Lose;
            UpdateTurnUI();
        }
    }
}
