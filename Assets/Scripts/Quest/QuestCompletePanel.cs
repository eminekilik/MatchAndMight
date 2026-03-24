using UnityEngine;
using TMPro;

public class QuestCompletePanel : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI xpText;

    private int xpAmount;

    public void OpenPanel(int amount)
    {
        xpAmount = amount;

        panel.SetActive(true);
        xpText.text = "+" + xpAmount + " XP";
    }

    public void OnClickOK()
    {
        panel.SetActive(false);

        PlayerLevelSystem.Instance.AddXPWithAnimate(xpAmount);

        QuestManager.Instance.hasPendingReward = false;
        QuestManager.Instance.pendingRewardXP = 0;
    }
}