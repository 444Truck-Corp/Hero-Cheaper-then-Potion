using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapData : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private int _width;
    [SerializeField] private int _height;

    public bool[,] Tiles;

    private void Reset()
    {
        _tilemap = GetComponent<Tilemap>();
    }

    private void Awake()
    {
        InitializeTileArray(new Vector3Int(0, 0));
    }

    public void InitializeTileArray(Vector3Int start)
    {
        Tiles = new bool[_width, _height];

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                Vector3Int tilePosition = new(start.x + x, start.y - y - 1, 0);
                Tiles[x, y] = _tilemap.HasTile(tilePosition);
            }
        }
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