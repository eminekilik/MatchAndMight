using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WoodManager : MonoBehaviour
{
    public static WoodManager Instance;

    public int totalWood = 0;
    public TextMeshProUGUI woodText;

    [Header("Animation Settings")]
    private float scaleMultiplier = 1.7f;
    public float animDuration = 0.2f;

    private Vector3 originalScale;
    private Coroutine animCoroutine;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        originalScale = woodText.transform.localScale;
    }

    public void AddWood(int amount)
    {
        totalWood += amount;
        UpdateUI();
        PlayScaleAnimation(); // EKLENDÝ
    }

    void UpdateUI()
    {
        woodText.text = totalWood.ToString();
    }

    void PlayScaleAnimation()
    {
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);

        animCoroutine = StartCoroutine(ScaleAnim());
    }

    IEnumerator ScaleAnim()
    {
        Vector3 targetScale = originalScale * scaleMultiplier;
        float time = 0;

        // Büyüme
        while (time < animDuration)
        {
            woodText.transform.localScale = Vector3.Lerp(originalScale, targetScale, time / animDuration);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0;

        // Küçülme
        while (time < animDuration)
        {
            woodText.transform.localScale = Vector3.Lerp(targetScale, originalScale, time / animDuration);
            time += Time.deltaTime;
            yield return null;
        }

        woodText.transform.localScale = originalScale;
    }
}