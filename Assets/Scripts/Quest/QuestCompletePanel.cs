using UnityEngine;
using TMPro;

public class QuestCompletePanel : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI xpText;

    private int xpAmount;
    private bool isShown = false;

    void Update()
    {
        if (QuestManager.Instance.hasPendingReward && !isShown)
        {
            xpAmount = QuestManager.Instance.pendingRewardXP;

            panel.SetActive(true);
            xpText.text = "+" + xpAmount + " XP";

            isShown = true;
        }
    }

    public void OnClickOK()
    {
        panel.SetActive(false);

        PlayerLevelSystem.Instance.AddXP(xpAmount);

        QuestManager.Instance.hasPendingReward = false;
        QuestManager.Instance.pendingRewardXP = 0;

        isShown = false; // sonraki quest için reset
    }
}