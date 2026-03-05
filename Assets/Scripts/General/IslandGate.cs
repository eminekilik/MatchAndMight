using System.Collections;
using UnityEngine;
using TMPro;

public class IslandGate : MonoBehaviour
{
    public int requiredLevel = 5;

    [Header("UI")]
    public TMP_Text warningText;

    private Collider2D gateCollider;

    void Awake()
    {
        gateCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        int playerLevel = PlayerLevelSystem.Instance.level;

        if (playerLevel >= requiredLevel)
        {
            gateCollider.enabled = false;
        }
        else
        {
            ShowWarning();
        }
    }

    void ShowWarning()
    {
        if (warningText == null)
            return;

        warningText.gameObject.SetActive(true);
        warningText.text = "You need Level " + requiredLevel + " to enter this island.";

        StartCoroutine(HideWarning());
    }

    IEnumerator HideWarning()
    {
        yield return new WaitForSeconds(2f);
        warningText.gameObject.SetActive(false);
    }
}