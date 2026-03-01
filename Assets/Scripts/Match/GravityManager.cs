using System.Collections;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
    public static GravityManager Instance;

    private BoardManager board;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        board = BoardManager.Instance;
    }

    public void ApplyGravity()
    {
        StartCoroutine(GravityCoroutine());
    }

    private IEnumerator GravityCoroutine()
    {
        bool moved;

        do
        {
            moved = false;

            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height - 1; y++)
                {
                    Tile current = board.GetTile(x, y);
                    Tile above = board.GetTile(x, y + 1);

                    if (current.currentGem == null && above.currentGem != null)
                    {
                        MoveGemDown(above.currentGem, current);
                        moved = true;
                    }
                }
            }

            yield return new WaitForSeconds(0.05f);

        } while (moved);

        SpawnNewGems();

        yield return new WaitForSeconds(0.25f);

        if (MatchManager.Instance.HasAnyMatch())
        {
            MatchDestroyer.Instance.ResolveMatches();
            yield break;
        }

        if (!BoardRuleChecker.Instance.HasPossibleMove())
        {
            board.ShuffleBoard();
        }
    }

    private void MoveGemDown(Gem gem, Tile targetTile)
    {
        Tile fromTile = gem.currentTile;

        fromTile.currentGem = null;
        targetTile.currentGem = gem;

        gem.currentTile = targetTile;
        gem.transform.SetParent(targetTile.transform);

        StartCoroutine(MoveAnimation(gem, targetTile.transform.position));
    }

    private IEnumerator MoveAnimation(Gem gem, Vector3 targetPos)
    {
        if (gem == null) yield break; // baþta yoksa dur

        Vector3 startPos = gem.transform.position;
        float t = 0f;

        while (t < 1f)
        {
            if (gem == null) yield break; // yok edilirse Coroutine’i sonlandýr
            t += Time.deltaTime * 8f;
            gem.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        if (gem != null)
            gem.transform.position = targetPos;
    }

    private void SpawnNewGems()
    {
        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height; y++)
            {
                Tile tile = board.GetTile(x, y);
                if (tile.currentGem == null)
                {
                    board.SpawnGemFromTop(x, y);
                }
            }
        }
    }
}
