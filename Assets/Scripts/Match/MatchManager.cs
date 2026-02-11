using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchManager : MonoBehaviour
{
    public static MatchManager Instance;

    BoardManager board;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        board = BoardManager.Instance;
    }

    public bool HasMatchAt(Tile tile)
    {
        return GetMatchAt(tile).Count >= 3;
    }

    public List<Gem> GetMatchAt(Tile tile)
    {
        List<Gem> result = new List<Gem>();

        if (tile.currentGem == null)
            return result;

        GemType type = tile.currentGem.gemType;

        // YATAY
        List<Gem> horizontal = new List<Gem>();
        horizontal.Add(tile.currentGem);

        int x = tile.x - 1;
        while (x >= 0)
        {
            Tile t = board.GetTile(x, tile.y);
            if (t.currentGem != null && t.currentGem.gemType == type)
                horizontal.Add(t.currentGem);
            else break;
            x--;
        }

        x = tile.x + 1;
        while (x < board.Width)
        {
            Tile t = board.GetTile(x, tile.y);
            if (t.currentGem != null && t.currentGem.gemType == type)
                horizontal.Add(t.currentGem);
            else break;
            x++;
        }

        if (horizontal.Count >= 3)
            result.AddRange(horizontal);

        // DÝKEY
        List<Gem> vertical = new List<Gem>();
        vertical.Add(tile.currentGem);

        int y = tile.y - 1;
        while (y >= 0)
        {
            Tile t = board.GetTile(tile.x, y);
            if (t.currentGem != null && t.currentGem.gemType == type)
                vertical.Add(t.currentGem);
            else break;
            y--;
        }

        y = tile.y + 1;
        while (y < board.Height)
        {
            Tile t = board.GetTile(tile.x, y);
            if (t.currentGem != null && t.currentGem.gemType == type)
                vertical.Add(t.currentGem);
            else break;
            y++;
        }

        if (vertical.Count >= 3)
            result.AddRange(vertical);

        return result.Distinct().ToList();
    }

    public List<Gem> GetAllMatches()
    {
        List<Gem> matches = new List<Gem>();

        for (int x = 0; x < board.Width; x++)
        {
            for (int y = 0; y < board.Height; y++)
            {
                Tile tile = board.GetTile(x, y);
                if (tile.currentGem == null) continue;

                var match = GetMatchAt(tile);
                if (match.Count >= 3)
                    matches.AddRange(match);
            }
        }

        return matches.Distinct().ToList();
    }

    public bool HasAnyMatch()
    {
        return GetAllMatches().Count > 0;
    }

}