using UnityEngine;

public class PlayerTreeInteraction : MonoBehaviour
{
    public Animator animator;

    private Tree currentTree;
    public Tree CurrentTree => currentTree; // okunabilir dýþarýdan

    public void TryCut()
    {
        if (currentTree != null)
        {
            animator.SetBool("isCutting", true);
        }
        else
        {
            animator.SetBool("isCutting", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Tree tree = other.GetComponent<Tree>();
        if (tree != null)
            currentTree = tree;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Tree tree = other.GetComponent<Tree>();
        if (tree == currentTree)
            currentTree = null;
    }

    public void HitTree()
    {
        if (currentTree != null)
            currentTree.Hit();
    }
}