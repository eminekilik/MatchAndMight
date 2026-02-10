using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchDestroyer : MonoBehaviour
{
    public static MatchDestroyer Instance;

    void Awake()
    {
        Instance = this;
    }

    public void ResolveMatches()
    {
        List<Gem> matches = MatchManager.Instance.GetAllMatches();

        if (matches.Count == 0)
            return;

        StartCoroutine(DestroyMatchesCoroutine(matches));
    }

    IEnumerator DestroyMatchesCoroutine(List<Gem> matches)
    {
        foreach (Gem gem in matches)
        {
            if (gem != null)
                StartCoroutine(DestroyGem(gem));
        }

        yield return new WaitForSeconds(0.25f);

        // ?? burada ileride gravity çaðýracaðýz
        GravityManager.Instance.ApplyGravity();

    }

    IEnumerator DestroyGem(Gem gem)
    {
        float t = 0f;
        Vector3 startScale = gem.transform.localScale;

        while (t < 1f)
        {
            t += Time.deltaTime * 6f;
            gem.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }

        Tile tile = gem.currentTile;
        tile.currentGem = null;

        Destroy(gem.gameObject);
    }
}
