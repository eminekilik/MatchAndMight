using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIController : MonoBehaviour
{
    public Slider playerHPBar;
    public Slider playerManaBar;
    public Slider enemyHPBar;
    public TMP_Text turnText;
    public GameObject boardDarkOverlay;

    public void SetupSliders(int playerMaxHP, int enemyMaxHP, int playerMaxMana)
    {
        playerHPBar.maxValue = playerMaxHP;
        enemyHPBar.maxValue = enemyMaxHP;
        playerManaBar.maxValue = playerMaxMana;
    }

    public void UpdateUI(int playerHP, int playerMana, int enemyHP)
    {
        playerHPBar.value = playerHP;
        playerManaBar.value = playerMana;
        enemyHPBar.value = enemyHP;
    }

    public void UpdateTurnUI(CombatManager.CombatState state)
    {
        switch (state)
        {
            case CombatManager.CombatState.PlayerTurn: turnText.text = "PLAYER TURN"; break;
            case CombatManager.CombatState.EnemyTurn: turnText.text = "ENEMY TURN"; break;
            case CombatManager.CombatState.Win: turnText.text = "YOU WIN"; break;
            case CombatManager.CombatState.Lose: turnText.text = "YOU LOSE"; break;
        }
    }

    public void UpdateBoardLockVisual(CombatManager.CombatState state)
    {
        if (boardDarkOverlay == null)
            return;

        boardDarkOverlay.SetActive(state != CombatManager.CombatState.PlayerTurn);
    }
}
