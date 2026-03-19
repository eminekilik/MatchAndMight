using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    public TMP_Text questText;
    public Image targetImage;

    void Update()
    {
        var quest = QuestManager.Instance.currentQuest;

        if (quest == null) return;

        questText.text =
    $"{quest.data.questName} ({quest.currentAmount}/{quest.data.requiredAmount})";

        // ?? Artýk quest icon kullanýyoruz
        if (quest.data.questIcon != null)
        {
            targetImage.sprite = quest.data.questIcon;
        }
    }
}