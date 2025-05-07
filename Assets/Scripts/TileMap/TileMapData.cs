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
        DebugTileArray();
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

    //111111111111111111111111
    //111111111111111111111011
    //110001110111100000010001
    //000000000000000000000000
    //000000000000000000000001
    //001110001110001110000000
    //001110001110001110000000
    //000000000000000000000000
    //000000000000000000000000
    //001110001110001110000000
    //001110001110001110000000
    //100000000000000000000000
    //100000000000000000000000
    //111111111111111111000000
    //100000000000001110000000
    //111111110000000000000000
}