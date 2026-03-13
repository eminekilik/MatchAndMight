using UnityEngine;

public class UpgradePanel : MonoBehaviour
{
    public GameObject panel;

    public int healthUpgrade = 20;
    public int attackUpgrade = 3;
    public int manaUpgrade = 10;

    public void OpenPanel()
    {
        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void UpgradeHealth()
    {
        CombatManager.Instance.playerData.bonusHealth += healthUpgrade;
        ClosePanel();
    }

    public void UpgradeAttack()
    {
        CombatManager.Instance.playerData.bonusAttack += attackUpgrade;
        ClosePanel();
    }

    public void UpgradeMana()
    {
        CombatManager.Instance.playerData.bonusMana += manaUpgrade;
        ClosePanel();
    }

    void ClosePanel()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }
}