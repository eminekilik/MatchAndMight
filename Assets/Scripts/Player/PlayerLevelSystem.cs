using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Linq;

public class PlayerLevelSystem : MonoBehaviour
{
    public static PlayerLevelSystem Instance;

    [Header("Level Settings")]
    public int level = 1;
    public int currentXP = 0;
    public int requiredXP = 100;

    [Header("UI")]
    public Slider xpSlider; // Main sahnedeki slider
    public TMPro.TextMeshProUGUI levelText; // level göstermek için

    private const string sliderName = "XP_Slider"; // Main sahnedeki slider objesi

    [Header("XP Gain Text")]
    public TMPro.TextMeshProUGUI xpGainText;

    private int pendingXP = 0;
    private bool shouldAnimateXP = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // sahne yüklendiðinde tetikle
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject sliderGO = GameObject.Find(sliderName);
        if (sliderGO != null)
            xpSlider = sliderGO.GetComponent<Slider>();

        GameObject levelGO = GameObject.Find("Level_Text");
        if (levelGO != null)
            levelText = levelGO.GetComponent<TMPro.TextMeshProUGUI>();

        var roots = scene.GetRootGameObjects();
        xpGainText = roots
        .SelectMany(r => r.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true))
        .FirstOrDefault(t => t.name == "Gain_Text");

        UpdateUI();

        // ?? BURASI ÖNEMLÝ
        if (shouldAnimateXP && pendingXP > 0 && xpSlider != null)
        {
            StartCoroutine(AnimateXP(pendingXP));
            pendingXP = 0;
            shouldAnimateXP = false;
        }
    }

    public void AddXP(int amount)
    {
        pendingXP += amount;
        shouldAnimateXP = true;
    }

    System.Collections.IEnumerator AnimateXP(int amount)
    {
        if (xpGainText != null)
        {
            xpGainText.gameObject.SetActive(true);
            xpGainText.text = "+" + amount + " XP";
        }

        int remainingXP = amount;
        float animationSpeed = 100f; // saniyede kaç xp dolsun

        while (remainingXP > 0)
        {
            int step = Mathf.CeilToInt(animationSpeed * Time.deltaTime);
            step = Mathf.Min(step, remainingXP);

            currentXP += step;
            remainingXP -= step;

            if (currentXP >= requiredXP)
            {
                currentXP = requiredXP;
                UpdateUI();

                yield return new WaitForSeconds(0.2f);

                LevelUp();
            }

            UpdateUI();
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        if (xpGainText != null)
            xpGainText.gameObject.SetActive(false);
    }

    void LevelUp()
    {
        currentXP -= requiredXP;
        level++;
        requiredXP = Mathf.RoundToInt(requiredXP * 1.4f);
        Debug.Log("Level Up! Yeni Level: " + level);

        UpdateUI(); // slider ve level text güncellenir
    }

    void UpdateUI()
    {
        if (xpSlider != null)
        {
            xpSlider.maxValue = requiredXP;
            xpSlider.value = currentXP;
        }

        if (levelText != null)
            levelText.text = "Lvl " + level;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}