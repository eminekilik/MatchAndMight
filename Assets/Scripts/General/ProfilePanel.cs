using UnityEngine;
using TMPro;

public class ProfilePanel : MonoBehaviour
{
    public GameObject profilePanel;

    public PlayerData playerData;

    public TMP_Text attackText;
    public TMP_Text healthText;
    public TMP_Text manaText;

    void Start()
    {
        profilePanel.SetActive(false);
    }

    public void OpenPanel()
    {
        UpdateStats();
        profilePanel.SetActive(true);
    }

    public void ClosePanel()
    {
        profilePanel.SetActive(false);
    }

    void UpdateStats()
    {
        attackText.text = playerData.currentAttack.ToString();
        healthText.text = playerData.currentHealth.ToString();
        manaText.text = playerData.currentManaGain.ToString();
    }
}