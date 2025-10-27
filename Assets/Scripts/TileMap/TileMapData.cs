using HeroPotion;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapData : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private int _width;
    [SerializeField] private int _height;
    [SerializeField] private bool[] _serializedTiles;

    public bool[,] Tiles { get; private set; }

    private void Reset()
    {
        _tilemap = GetComponent<Tilemap>();
    }

    private void Awake()
    {
        if (_serializedTiles != null && _serializedTiles.Length == _width * _height)
        {
            Tiles = new bool[_width, _height];
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    Tiles[x, y] = _serializedTiles[y * _width + x];
                }
            }

            TileMapManager.Instance.InitializeAStar(Tiles);
        }
        else
        {
            Debug.LogError("TileData가 비었습니다!");
        }
    }

    [ContextMenu("Bake Tile Data")]
    public void BakeTileData()
    {
        if (_tilemap == null || _width == 0 || _height == 0)
        {
            Debug.LogError("TileData를 구울 수 없습니다.");
            return;
        }

        _serializedTiles = new bool[_width * _height];
        Vector3Int start = new Vector3Int(0, 0);

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                Vector3Int tilePosition = new(start.x + x, start.y - y - 1, 0);
                _serializedTiles[y * _width + x] = _tilemap.HasTile(tilePosition);
            }
        }
        Debug.Log("TileData를 성공적으로 구웠습니다.");
    }

    private void DebugTileArray()
    {
        string value = "";

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                value += Tiles[x, y] ? "1" : "0";
            }
            value += "\n";
        }

        Debug.Log(value);
    }
}