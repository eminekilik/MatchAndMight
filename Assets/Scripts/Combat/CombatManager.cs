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

    [Header("Damage Text")]
    public GameObject damageTextPrefab;
    public Canvas worldCanvas;

    Vector3 enemyOriginalScale;
    Vector3 playerOriginalScale;
    [Header("Enemy Visual")]
    SpriteRenderer enemySprite;
    Coroutine playerPunchRoutine;
    Vector3 playerBaseScale;
    Coroutine enemyHitRoutine;
    Vector3 enemyBaseScale;
    Vector3 enemyBasePosition;




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

        enemyOriginalScale = enemyWorldTarget.localScale;
        playerOriginalScale = playerWorldTarget.localScale;

        SetupSliders();
        UpdateUI();
        UpdateTurnUI();

        if (screenFlashImage != null)
            screenFlashImage.color = new Color(1, 0, 0, 0);
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
                FirePlayerProjectile(worldPos, totalDamage);
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

    void FirePlayerProjectile(Vector3 startPos, int damage)
    {
        if (playerProjectilePrefab == null || enemyWorldTarget == null)
            return;

        GameObject proj = Instantiate(playerProjectilePrefab, startPos, Quaternion.identity);
        EnergyProjectile ep = proj.GetComponent<EnergyProjectile>();

        if (ep != null)
        {
            ep.SetTarget(enemyWorldTarget, () =>
            {
                enemyHP -= damage;
                ClampValues();
                UpdateUI();

                TriggerEnemyHitJuice();
                ShowDamageText(damage, enemyWorldTarget.position);
            });
        }
    }

    void FireEnemyProjectile()
    {
        if (enemyProjectilePrefab == null || playerWorldTarget == null)
            return;

        GameObject proj = Instantiate(enemyProjectilePrefab, enemyWorldTarget.position, Quaternion.identity);
        EnergyProjectile ep = proj.GetComponent<EnergyProjectile>();

        if (ep != null)
        {
            ep.SetTarget(playerWorldTarget, () =>
            {
                int damage = Random.Range(6, 14);
                playerHP -= damage;

                ClampValues();
                UpdateUI();

                TriggerPlayerHitJuice();
                ShowDamageText(damage, playerWorldTarget.position);
                ScreenFlash();
            });
        }
    }

    public void SetEnemy(Transform enemyTransform)
    {
        enemyWorldTarget = enemyTransform;

        enemyOriginalScale = enemyWorldTarget.localScale;

        enemySprite = enemyWorldTarget.GetComponentInChildren<SpriteRenderer>();
        enemyBaseScale = enemyTransform.localScale;
        enemyBasePosition = enemyTransform.position;


    }


    void TriggerEnemyHitJuice()
    {
        if (enemyWorldTarget == null) return;

        if (enemyHitRoutine != null)
            StopCoroutine(enemyHitRoutine);

        enemyWorldTarget.localScale = enemyBaseScale;

        enemyHitRoutine = StartCoroutine(EnemyHitEffect());
    }

    IEnumerator EnemyHitEffect()
    {
        float duration = 0.25f;
        float elapsed = 0;

        Color originalColor = enemySprite.color;

        float shakeAmount = 0.15f;
        Vector3 punchScale = enemyBaseScale * 1.15f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // SHAKE (HER ZAMAN BASE POSITION'DAN)
            float offsetX = Random.Range(-shakeAmount, shakeAmount);
            enemyWorldTarget.position =
                enemyBasePosition + new Vector3(offsetX, 0, 0);

            // SCALE (HER ZAMAN BASE SCALE'DEN)
            float ease = 1 - Mathf.Pow(1 - t, 3);
            enemyWorldTarget.localScale =
                Vector3.Lerp(punchScale, enemyBaseScale, ease);

            // FLASH
            if (t < 0.1f)
                enemySprite.color = Color.white;
            else
                enemySprite.color =
                    Color.Lerp(Color.white, originalColor, (t - 0.1f) / 0.9f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        enemyWorldTarget.position = enemyBasePosition;
        enemyWorldTarget.localScale = enemyBaseScale;
        enemySprite.color = originalColor;
    }


    void TriggerPlayerHitJuice()
    {
        if (playerWorldTarget == null) return;

        if (playerPunchRoutine != null)
            StopCoroutine(playerPunchRoutine);

        playerPunchRoutine =
            StartCoroutine(PunchScale(playerWorldTarget, Vector3.one, 1.15f));
    }



    IEnumerator PunchScale(Transform target, Vector3 baseScale, float multiplier)
    {
        float duration = 0.18f;
        float elapsed = 0;

        Vector3 punch = baseScale * multiplier;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float ease = 1 - Mathf.Pow(1 - t, 3);

            target.localScale = Vector3.Lerp(punch, baseScale, ease);

            elapsed += Time.deltaTime;
            yield return null;
        }

        target.localScale = baseScale;
    }


    void ShowDamageText(int damage, Vector3 spawnPos)
    {
        if (damageTextPrefab == null || worldCanvas == null)
            return;

        GameObject textObj =
            Instantiate(damageTextPrefab, spawnPos, Quaternion.identity, worldCanvas.transform);

        TMP_Text tmp = textObj.GetComponent<TMP_Text>();
        tmp.text = "-" + damage.ToString();

        StartCoroutine(BattleCampDamageAnim(textObj));
    }

    IEnumerator BattleCampDamageAnim(GameObject obj)
    {
        float duration = 1.0f;
        float elapsed = 0;

        Vector3 startPos = obj.transform.position;

        float randomX = Random.Range(-1.5f, 1.5f);
        float height = Random.Range(3f, 4f);

        CanvasGroup cg = obj.AddComponent<CanvasGroup>();

        obj.transform.localScale = Vector3.one * 0.3f;
        obj.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-25f, 25f));

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            float x = Mathf.Lerp(0, randomX, t);
            float y = (height * t) - (4f * t * t);

            obj.transform.position = startPos + new Vector3(x, y, 0);

            if (t < 0.15f)
            {
                float pop = Mathf.Lerp(0.3f, 1.8f, t / 0.15f);
                obj.transform.localScale = Vector3.one * pop;
            }
            else
            {
                float shrink = Mathf.Lerp(1.8f, 0.8f, (t - 0.15f) / 0.85f);
                obj.transform.localScale = Vector3.one * shrink;
            }

            if (t > 0.6f)
                cg.alpha = 1 - ((t - 0.6f) / 0.4f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(obj);
    }

    void ScreenFlash()
    {
        if (screenFlashImage == null)
            return;

        StartCoroutine(ScreenFlashRoutine());
    }

    IEnumerator ScreenFlashRoutine()
    {
        float duration = 0.2f;
        float elapsed = 0;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            screenFlashImage.color = new Color(1, 1, 1, 0.6f * (1 - t));

            elapsed += Time.deltaTime;
            yield return null;
        }

        screenFlashImage.color = new Color(1, 1, 1, 0);
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
        yield return new WaitForSeconds(1f);

        FireEnemyProjectile();

        yield return new WaitForSeconds(1f);

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
