using UnityEngine;

public class IslandGate : MonoBehaviour
{
    [Header("Gate Settings")]
    public int requiredLevel = 5;

    private Collider2D gateCollider;
    private bool isUnlocked = false;

    private void Awake()
    {
        gateCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;

        if (isUnlocked)
            return;

        int playerLevel = PlayerLevelSystem.Instance.level;

        if (playerLevel >= requiredLevel)
        {
            UnlockGate();
        }
        else
        {
            LockedFeedback();
        }
    }

    void UnlockGate()
    {
        isUnlocked = true;
        gateCollider.enabled = false; // Artýk geçilebilir
        Debug.Log("Gate unlocked!");
    }

    void LockedFeedback()
    {
        Debug.Log("Level " + requiredLevel + " required.");
        // Buraya UI popup baðlayabiliriz
    }
}