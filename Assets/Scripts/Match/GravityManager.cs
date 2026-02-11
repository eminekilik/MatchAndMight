using System.Collections;
using UnityEngine;

public class GravityManager : MonoBehaviour
{
    public static GravityManager Instance;

    BoardManager board;

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

    IEnumerator GravityCoroutine()
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

        yield return new WaitForSeconds(0.2f);

        // 1?? once match var mi
        if (MatchManager.Instance.HasAnyMatch())
        {
            MatchDestroyer.Instance.ResolveMatches();

            yield break;
        }

        // 2?? match yoksa hamle var mi
        if (!BoardManager.Instance.HasPossibleMove())
        {
            BoardManager.Instance.ShuffleBoard();
        }
    }

    void MoveGemDown(Gem gem, Tile targetTile)
    {
        Tile fromTile = gem.currentTile;

        fromTile.currentGem = null;
        targetTile.currentGem = gem;

        gem.currentTile = targetTile;
        gem.transform.SetParent(targetTile.transform);

        StartCoroutine(MoveAnimation(gem, targetTile.transform.position));
    }

    IEnumerator MoveAnimation(Gem gem, Vector3 targetPos)
    {
        Vector3 startPos = gem.transform.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * 8f;
            gem.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        gem.transform.position = targetPos;
    }

    void SpawnNewGems()
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
