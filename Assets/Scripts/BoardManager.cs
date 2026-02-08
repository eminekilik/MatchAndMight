using System.Collections;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;
    private Gem selectedGem;

    [Header("Board Size")]
    public int width = 6;
    public int height = 6;
    public float tileSize = 1f;

    [Header("Prefabs")]
    public Tile tilePrefab;
    public Gem[] gemPrefabs;

    private Tile[,] tiles;

    public float swapDuration = 0.15f;
    bool isSwapping = false;


    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CreateBoard();
        SpawnInitialGems();
    }

    void CreateBoard()
    {
        tiles = new Tile[width, height];

        float xOffset = (width - 1) * tileSize / 2f;
        float yOffset = (height - 1) * tileSize / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 position = new Vector2(
                    x * tileSize - xOffset,
                    y * tileSize - yOffset
                );

                Tile tile = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                tile.x = x;
                tile.y = y;

                tiles[x, y] = tile;
            }
        }
    }

    void SpawnInitialGems()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                SpawnGemAt(x, y);
            }
        }
    }

    void SpawnGemAt(int x, int y)
    {
        Gem gemPrefab = gemPrefabs[Random.Range(0, gemPrefabs.Length)];

        Gem gem = Instantiate(
            gemPrefab,
            tiles[x, y].transform.position,
            Quaternion.identity,
            tiles[x, y].transform   // ?? parent burasý
        );

        gem.currentTile = tiles[x, y];
        tiles[x, y].currentGem = gem;
    }

    public void SelectGem(Gem gem)
    {
        if (selectedGem == null)
        {
            selectedGem = gem;
            Debug.Log("Gem seçildi");
            return;
        }

        TrySwap(selectedGem, gem);
        selectedGem = null;
    }

    void TrySwap(Gem a, Gem b)
    {
        int dx = Mathf.Abs(a.currentTile.x - b.currentTile.x);
        int dy = Mathf.Abs(a.currentTile.y - b.currentTile.y);

        // sadece yan yanaysa
        if (dx + dy == 1)
        {
            SwapGems(a, b);
        }
    }

    void SwapGems(Gem a, Gem b)
    {
        if (isSwapping) return;
        StartCoroutine(SwapCoroutine(a, b));
    }

    IEnumerator SwapCoroutine(Gem a, Gem b)
    {
        isSwapping = true;

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

        // parent deðiþtir
        a.transform.SetParent(tileB.transform);
        b.transform.SetParent(tileA.transform);

        // kesin pozisyon
        a.transform.position = posB;
        b.transform.position = posA;

        // referanslarý güncelle
        a.currentTile = tileB;
        b.currentTile = tileA;

        tileA.currentGem = b;
        tileB.currentGem = a;

        isSwapping = false;
    }

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
        {
            SwapGems(gem, otherGem);
        }
    }


}
