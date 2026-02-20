using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatEffectsController : MonoBehaviour
{
    public Transform enemyWorldTarget;
    public Transform playerWorldTarget;
    public GameObject damageTextPrefab;
    public Canvas worldCanvas;
    public Image screenFlashImage;

    [Header("Power")]
    public GameObject powerReadyText;

    SpriteRenderer enemySprite;
    Coroutine playerPunchRoutine;
    Coroutine enemyHitRoutine;
    Vector3 enemyBaseScale;
    Vector3 enemyBasePosition;

    public void SetEnemy(Transform enemyTransform)
    {
        enemyWorldTarget = enemyTransform;
        enemySprite = enemyWorldTarget.GetComponentInChildren<SpriteRenderer>();
        enemyBaseScale = enemyTransform.localScale;
        enemyBasePosition = enemyTransform.position;
    }

    public void ShowPowerReady()
    {
        if (powerReadyText != null)
            powerReadyText.SetActive(true);
    }

    public void HidePowerReady()
    {
        if (powerReadyText != null)
            powerReadyText.SetActive(false);
    }

    public void TriggerEnemyHitJuice()
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

            float offsetX = Random.Range(-shakeAmount, shakeAmount);
            enemyWorldTarget.position =
                enemyBasePosition + new Vector3(offsetX, 0, 0);

            float ease = 1 - Mathf.Pow(1 - t, 3);
            enemyWorldTarget.localScale =
                Vector3.Lerp(punchScale, enemyBaseScale, ease);

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

    public void TriggerPlayerHitJuice()
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

    public void ShowDamageText(int damage, Vector3 spawnPos)
    {
        if (damageTextPrefab == null || worldCanvas == null)
            return;

        GameObject textObj =
            Instantiate(damageTextPrefab, spawnPos, Quaternion.identity, worldCanvas.transform);

        TMP_Text tmp = textObj.GetComponent<TMP_Text>();
        tmp.text = "-" + damage.ToString();

        StartCoroutine(DamageAnim(textObj));
    }

    public void ShowDamageText(int damage, Vector3 spawnPos, Color color)
    {
        if (damageTextPrefab == null || worldCanvas == null)
            return;

        GameObject textObj =
            Instantiate(damageTextPrefab, spawnPos, Quaternion.identity, worldCanvas.transform);

        TMP_Text tmp = textObj.GetComponent<TMP_Text>();
        tmp.text = "-" + damage.ToString();
        tmp.color = color;

        StartCoroutine(DamageAnim(textObj));
    }

    public void UIPunch(Transform target)
    {
        StartCoroutine(UIPunchRoutine(target));
    }

    IEnumerator UIPunchRoutine(Transform target)
    {
        float duration = 0.2f;
        float elapsed = 0;

        Vector3 baseScale = Vector3.one;
        Vector3 punch = baseScale * 1.2f;

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

    public void ShowGainText(int amount, Vector3 spawnPos, Color color)
    {
        if (damageTextPrefab == null || worldCanvas == null)
            return;

        GameObject textObj =
            Instantiate(damageTextPrefab, spawnPos, Quaternion.identity, worldCanvas.transform);

        TMP_Text tmp = textObj.GetComponent<TMP_Text>();
        tmp.text = "+" + amount.ToString();
        tmp.color = color;

        StartCoroutine(DamageAnim(textObj));
    }

    IEnumerator DamageAnim(GameObject obj)
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

    public void ScreenFlash()
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

    public void ClearEnemy()
    {
        if (enemyHitRoutine != null)
            StopCoroutine(enemyHitRoutine);

        enemyWorldTarget = null;
        enemySprite = null;
    }
}
