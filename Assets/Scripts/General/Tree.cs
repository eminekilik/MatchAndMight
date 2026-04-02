using UnityEngine;

public class Tree : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;
    public bool IsDestroyed { get; private set; } = false;

    private Animator animator; // EKLENDÝ

    [Header("Collider Reference")]
    public BoxCollider2D treeCollider;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>(); // EKLENDÝ
    }

    public void Hit()
    {
        if (IsDestroyed) return;

        currentHealth--;

        if (currentHealth <= 0)
        {
            CutDown();
        }
    }

    void CutDown()
    {
        IsDestroyed = true;
        Debug.Log("Aðaç kesildi");

        animator.SetTrigger("Cut"); // Destroy yerine animasyon
        ShrinkCollider();
    }

    void ShrinkCollider()
    {
        treeCollider.size = new Vector2(0.5f, treeCollider.size.y * 0.3f);
        treeCollider.offset = new Vector2(treeCollider.offset.x, treeCollider.offset.y - 1.8f);

        Collider2D parentCol = GetComponent<Collider2D>();
        if (parentCol != null)
            parentCol.enabled = false;
    }
}