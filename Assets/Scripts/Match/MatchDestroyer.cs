using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchDestroyer : MonoBehaviour
{
    public static MatchDestroyer Instance;

    public System.Action<Dictionary<GemType, int>> OnMatchesResolved;
    public System.Action OnResolveFinished;

    void Awake()
    {
        Instance = this;
    }

    public void ResolveMatches()
    {
        StartCoroutine(ResolveLoop());
    }

    private IEnumerator ResolveLoop()
    {
        while (true)
        {
            List<Gem> matches = MatchManager.Instance.GetAllMatches();
            if (matches.Count == 0) break;

            yield return StartCoroutine(DestroyMatchesCoroutine(matches));
            yield return new WaitForSeconds(0.15f);
        }

        OnResolveFinished?.Invoke();
    }

    private IEnumerator DestroyMatchesCoroutine(List<Gem> matches)
    {
        matches = new List<Gem>(new HashSet<Gem>(matches));
        Dictionary<GemType, int> matchCounts = new Dictionary<GemType, int>();

        foreach (Gem gem in matches)
        {
            if (gem == null) continue;

            if (!matchCounts.ContainsKey(gem.gemType))
                matchCounts[gem.gemType] = 0;

            matchCounts[gem.gemType]++;
        }

        OnMatchesResolved?.Invoke(matchCounts);

        foreach (Gem gem in matches)
        {
            if (gem != null)
                StartCoroutine(DestroyGem(gem));
        }

        yield return new WaitForSeconds(0.25f);
        GravityManager.Instance.ApplyGravity();
        yield return new WaitForSeconds(0.25f);
    }

    private IEnumerator DestroyGem(Gem gem)
    {
        if (gem == null) yield break;

        float t = 0f;
        Vector3 startScale = gem.transform.localScale;

        while (t < 1f)
        {
            if (gem == null) yield break;

            t += Time.deltaTime * 6f;
            gem.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }

        if (gem == null) yield break;

        if (gem.currentTile != null)
            gem.currentTile.currentGem = null;

        Destroy(gem.gameObject);
    }
}
