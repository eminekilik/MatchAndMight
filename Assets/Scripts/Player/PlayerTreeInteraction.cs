using System.Collections;
using UnityEngine;

public class PlayerTreeInteraction : MonoBehaviour
{
    public Animator animator;

    private Tree currentTree;
    public Tree CurrentTree => currentTree;

    private bool isHolding = false;

    void Update()
    {
        // Mouse basýldý
        if (Input.GetMouseButtonDown(0))
        {
            if (currentTree != null)
            {
                //animator.SetTrigger("Cut");
                isHolding = true;
            }
        }

        // Mouse basýlý tutuluyor
        //if (Input.GetMouseButton(0))
        //{
        //    if (currentTree != null)
        //    {
        //        isHolding = true;
        //    }
        //}

        // Mouse býrakýldý
        if (Input.GetMouseButtonUp(0))
        {
            isHolding = false;
        }
    }

    // Animasyonun sonuna event koyacaksýn
    public void OnCutAnimationEnd()
    {
        if (currentTree != null)
        {
            HitTree(); // sadece damage burada
        }
    }
    private bool isCutting = false;

    public void StartCutting()
    {
        if (currentTree == null) return;
        if (isCutting) return; // ?? spam engelle

        isCutting = true;
        animator.SetTrigger("Cut");

        StartCoroutine(ResetCutting());
    }

    private IEnumerator ResetCutting()
    {
        yield return new WaitForSeconds(0.5f); // animasyon sürene göre ayarla
        isCutting = false;
    }

    public void TryCut()
    {
        // artýk kullanýlmasa da dursun
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