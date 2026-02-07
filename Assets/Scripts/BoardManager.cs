using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Board Size")]
    public int width = 6;
    public int height = 6;
    public float tileSize = 1f;

    [Header("Prefabs")]
    public Tile tilePrefab;
    public Gem[] gemPrefabs;

    private Tile[,] tiles;

    void Start()
    {
        CreateBoard();
        SpawnInitialGems();
    }

    void CreateBoard()
    {
        tiles = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 position = new Vector2(x * tileSize, y * tileSize);

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
            Quaternion.identity
        );

        gem.currentTile = tiles[x, y];
        tiles[x, y].currentGem = gem;
    }
}
