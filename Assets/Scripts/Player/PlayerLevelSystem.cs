using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        // Slider
        GameObject sliderGO = GameObject.Find(sliderName);
        if (sliderGO != null)
            xpSlider = sliderGO.GetComponent<Slider>();

        // Level Text
        GameObject levelGO = GameObject.Find("Level_Text");
        if (levelGO != null)
            levelText = levelGO.GetComponent<TMPro.TextMeshProUGUI>();

        UpdateUI();
    }

    public void AddXP(int amount)
    {
        currentXP += amount;

        while (currentXP >= requiredXP)
        {
            LevelUp();
        }

        if (xpSlider != null)
            UpdateUI();
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