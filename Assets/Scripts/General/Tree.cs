using UnityEngine;

public class Tree : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;
    public bool IsDestroyed { get; private set; } = false;

    void Start()
    {
        currentHealth = maxHealth;
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
        Destroy(gameObject);
    }
}