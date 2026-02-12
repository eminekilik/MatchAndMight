using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardRuleChecker : MonoBehaviour
{
    public static BoardRuleChecker Instance;

    private BoardManager board;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        board = BoardManager.Instance;
    }

    public bool HasPossibleMove()
    {
        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height; y++)
            {
                Gem gem = board.GetTile(x, y).currentGem;
                if (gem == null) continue;

                // Sað
                if (x < board.Width - 1 && CheckSwapForMatch(board.GetTile(x, y), board.GetTile(x + 1, y)))
                    return true;
                // Yukarý
                if (y < board.Height - 1 && CheckSwapForMatch(board.GetTile(x, y), board.GetTile(x, y + 1)))
                    return true;
            }
        }
        return false;
    }

    private bool CheckSwapForMatch(Tile a, Tile b)
    {
        if (a.currentGem == null || b.currentGem == null) return false;

        Gem gemA = a.currentGem;
        Gem gemB = b.currentGem;

        // Sahte swap
        a.currentGem = gemB;
        b.currentGem = gemA;
        gemA.currentTile = b;
        gemB.currentTile = a;

        bool hasMatch = MatchManager.Instance.HasMatchAt(a) || MatchManager.Instance.HasMatchAt(b);

        // Geri al
        a.currentGem = gemA;
        b.currentGem = gemB;
        gemA.currentTile = a;
        gemB.currentTile = b;

        return hasMatch;
    }
}
