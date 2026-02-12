using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;

    [Header("Board Size")]
    public int width = 6;
    public int height = 6;
    public float tileSize = 1f;

    [Header("Prefabs")]
    public Tile tilePrefab;
    public Gem[] gemPrefabs;

    private Tile[,] tiles;

    public float swapDuration = 0.15f;
    private bool isShuffling;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CreateBoard();
        SpawnInitialGems();
    }

    #region Board Creation
    void CreateBoard()
    {
        tiles = new Tile[width, height];

        float boardWidth = (width - 1) * tileSize;
        float xOffset = boardWidth / 2f;
        float bottomPadding = 1f;

        float cameraBottom = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;

        float startY = cameraBottom + (tileSize / 2f) + bottomPadding;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 position = new Vector2(
                    x * tileSize - xOffset,
                    startY + y * tileSize
                );

                Tile tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                tile.x = x;
                tile.y = y;

                tiles[x, y] = tile;
            }
        }
    }
    #endregion

    #region Gem Spawning
    void SpawnInitialGems()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                SpawnGemFromTop(x, y);

        StartCoroutine(StartBoardResolve());
    }

    IEnumerator StartBoardResolve()
    {
        yield return new WaitForSeconds(0.2f);
        GravityManager.Instance.ApplyGravity();
    }

    public void SpawnGemFromTop(int x, int y)
    {
        Gem prefab = gemPrefabs[Random.Range(0, gemPrefabs.Length)];
        Vector3 spawnPos = GetTile(x, height - 1).transform.position + Vector3.up * tileSize;

        Gem gem = Instantiate(prefab, spawnPos, Quaternion.identity, GetTile(x, y).transform);

        gem.currentTile = GetTile(x, y);
        GetTile(x, y).currentGem = gem;

        StartCoroutine(MoveNewGem(gem, GetTile(x, y).transform.position));
    }

    IEnumerator MoveNewGem(Gem gem, Vector3 targetPos)
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
    #endregion

    #region Swap
    public void TrySwapFromDirection(Gem gem, Vector2 dir)
    {
        int targetX = gem.currentTile.x;
        int targetY = gem.currentTile.y;

        if (dir == Vector2.right) targetX++;
        if (dir == Vector2.left) targetX--;
        if (dir == Vector2.up) targetY++;
        if (dir == Vector2.down) targetY--;

        if (targetX < 0 || targetX >= width || targetY < 0 || targetY >= height)
            return;

        Gem otherGem = tiles[targetX, targetY].currentGem;
        if (otherGem != null)
            SwapGems(gem, otherGem);
    }

    void SwapGems(Gem a, Gem b)
    {
        StartCoroutine(SwapCoroutine(a, b, true));
    }

    IEnumerator SwapCoroutine(Gem a, Gem b, bool checkMatch)
    {
        Tile tileA = a.currentTile;
        Tile tileB = b.currentTile;

        Vector3 posA = a.transform.position;
        Vector3 posB = b.transform.position;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / swapDuration;
            a.transform.position = Vector3.Lerp(posA, posB, t);
            b.transform.position = Vector3.Lerp(posB, posA, t);
            yield return null;
        }

        a.transform.SetParent(tileB.transform);
        b.transform.SetParent(tileA.transform);

        a.transform.position = posB;
        b.transform.position = posA;

        a.currentTile = tileB;
        b.currentTile = tileA;

        tileA.currentGem = b;
        tileB.currentGem = a;

        if (checkMatch)
            MatchDestroyer.Instance.ResolveMatches();
    }
    #endregion

    #region Tile Access
    public Tile GetTile(int x, int y) => tiles[x, y];
    public int Width => width;
    public int Height => height;
    #endregion

    #region Board Checks / Shuffle
    public bool HasPossibleMove()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Gem gem = tiles[x, y].currentGem;
                if (gem == null) continue;

                if (x < width - 1 && CheckSwapForMatch(tiles[x, y], tiles[x + 1, y]))
                    return true;

                if (y < height - 1 && CheckSwapForMatch(tiles[x, y], tiles[x, y + 1]))
                    return true;
            }
        }
        return false;
    }

    bool CheckSwapForMatch(Tile a, Tile b)
    {
        if (a.currentGem == null || b.currentGem == null) return false;

        Gem gemA = a.currentGem;
        Gem gemB = b.currentGem;

        a.currentGem = gemB;
        b.currentGem = gemA;

        gemA.currentTile = b;
        gemB.currentTile = a;

        bool hasMatch = MatchManager.Instance.HasMatchAt(a) || MatchManager.Instance.HasMatchAt(b);

        a.currentGem = gemA;
        b.currentGem = gemB;
        gemA.currentTile = a;
        gemB.currentTile = b;

        return hasMatch;
    }

    public void ShuffleBoard()
    {
        if (isShuffling) return;
        StartCoroutine(ShuffleRoutine());
    }

    IEnumerator ShuffleRoutine_Internal()
    {
        List<Gem> gems = new List<Gem>();
        List<Tile> tileList = new List<Tile>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tiles[x, y].currentGem != null)
                    gems.Add(tiles[x, y].currentGem);

                tileList.Add(tiles[x, y]);
            }
        }

        for (int i = 0; i < tileList.Count; i++)
        {
            int rnd = Random.Range(i, tileList.Count);
            (tileList[i], tileList[rnd]) = (tileList[rnd], tileList[i]);
        }

        Dictionary<Gem, Vector3> startPos = new Dictionary<Gem, Vector3>();
        Dictionary<Gem, Tile> targetTile = new Dictionary<Gem, Tile>();

        for (int i = 0; i < gems.Count; i++)
        {
            startPos[gems[i]] = gems[i].transform.position;
            targetTile[gems[i]] = tileList[i];
        }

        foreach (Tile t in tiles)
            t.currentGem = null;

        float tAnim = 0f;
        float duration = 0.35f;

        while (tAnim < 1f)
        {
            tAnim += Time.deltaTime / duration;

            foreach (Gem gem in gems)
            {
                gem.transform.position = Vector3.Lerp(
                    startPos[gem],
                    targetTile[gem].transform.position,
                    tAnim
                );
            }

            yield return null;
        }

        foreach (Gem gem in gems)
        {
            Tile t = targetTile[gem];
            gem.currentTile = t;
            t.currentGem = gem;
            gem.transform.SetParent(t.transform);
            gem.transform.position = t.transform.position;
        }
    }


    IEnumerator ShuffleRoutine()
    {
        isShuffling = true;

        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(ShakeAllGems());
        yield return new WaitForSeconds(0.15f);

        yield return StartCoroutine(ShuffleRoutine_Internal());

        int safetyCounter = 0;
        while (!HasPossibleMove() && safetyCounter < 5)
        {
            safetyCounter++;
            yield return new WaitForSeconds(0.15f);
            yield return StartCoroutine(ShuffleRoutine_Internal());
        }

        isShuffling = false;
    }



    public IEnumerator ShakeAllGems()
    {
        float duration = 0.4f;
        float strength = 0.05f;   // daha hafif
        float frequency = 18f;    // sallanma h?z?

        Dictionary<Gem, Vector3> startPos = new Dictionary<Gem, Vector3>();
        Dictionary<Gem, float> phaseOffset = new Dictionary<Gem, float>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (tiles[x, y].currentGem != null)
                {
                    Gem gem = tiles[x, y].currentGem;
                    startPos[gem] = gem.transform.position;
                    phaseOffset[gem] = Random.Range(0f, Mathf.PI * 2f);
                }
            }
        }

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;

            // ba?ta güçlü, sonda yava?ça sönsün
            float damp = Mathf.Sin(normalized * Mathf.PI);

            foreach (var gem in startPos.Keys)
            {
                float phase = phaseOffset[gem];
                float xOffset = Mathf.Sin((t * frequency) + phase) * strength * damp;
                float yOffset = Mathf.Cos((t * frequency * 0.5f) + phase) * (strength * 0.3f) * damp;

                gem.transform.position = startPos[gem] + new Vector3(xOffset, yOffset, 0f);
            }

            yield return null;
        }

        foreach (var pair in startPos)
            pair.Key.transform.position = pair.Value;
    }

    #endregion
}
