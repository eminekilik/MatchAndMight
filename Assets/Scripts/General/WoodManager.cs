using UnityEngine;
using UnityEngine.UI;

public class WoodManager : MonoBehaviour
{
    public static WoodManager Instance;

    public int totalWood = 0;
    public Text woodText; // UI Text

    void Awake()
    {
        Instance = this;
    }

    public void AddWood(int amount)
    {
        totalWood += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        woodText.text = "Odun: " + totalWood;
    }
}