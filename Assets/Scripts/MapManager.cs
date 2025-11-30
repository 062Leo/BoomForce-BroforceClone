using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [SerializeField] private Tilemap map;
    [SerializeField] private List<TileData> tileDatas;
    private static Dictionary<TileBase, TileData> dataFromTiles;

    private void Awake()
    {
        dataFromTiles = new Dictionary<TileBase, TileData>();
        foreach (var tileData in tileDatas)
        {
            foreach (var tile in tileData.tiles)
            {
                if (!dataFromTiles.ContainsKey(tile))
                {
                    dataFromTiles.Add(tile, tileData);
                }
                else
                {
                    Debug.LogWarning($"Tile {tile.name} is already in the dictionary.");
                }
            }
        }
    }

    public float GetTileHealth(Vector2 worldPosition)
    {
        Vector3Int gridPosition = map.WorldToCell(worldPosition);
        TileBase tile = map.GetTile(gridPosition);
        if (tile == null)
        {
            Debug.LogWarning($"No tile found at position {worldPosition}");
            return 0;
        }

        if (!dataFromTiles.ContainsKey(tile))
        {
            Debug.LogError($"Tile {tile.name} at position {worldPosition} not found in dictionary.");
            return 0;
        }

        float health = dataFromTiles[tile].health;
        return health;
    }

    public bool isThereATile(Vector2 worldPosition)
    {
        Vector3Int gridPosition = map.WorldToCell(worldPosition);
        TileBase tile = map.GetTile(gridPosition);
        if (tile == null)
        {
            return false;
        }
        return true;
    }

    public bool getTileDestroyable(Vector2 worldPosition)
    {
        try
        {
            Vector3Int gridPosition = map.WorldToCell(worldPosition);
            TileBase tile = map.GetTile(gridPosition);

            if (tile != null && dataFromTiles.ContainsKey(tile))
            {
                return dataFromTiles[tile].destroyable;
            }
            else
            {
                Debug.Log("Tile oder Tile-Daten nicht gefunden.");
                return false; // Oder handle den Fehler entsprechend.
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Ein Fehler ist aufgetreten: " + ex.Message);
            return false; // Oder handle den Fehler entsprechend.
        }
    }
    public float getTileExplosionTimer(Vector2 worldPosition)
    {
        try
        {
            Vector3Int gridPosition = map.WorldToCell(worldPosition);
            TileBase tile = map.GetTile(gridPosition);

            if (tile != null && dataFromTiles.ContainsKey(tile))
            {
                return dataFromTiles[tile].explosionTimer;
            }
            else
            {
                Debug.Log("Tile oder Tile-Daten nicht gefunden.");
                return 0f; // Oder handle den Fehler entsprechend.
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Ein Fehler ist aufgetreten: " + ex.Message);
            return 0f; // Oder handle den Fehler entsprechend.
        }
    }
    public string getTileName(Vector2 worldPosition)
    {
        try
        {
            Vector3Int gridPosition = map.WorldToCell(worldPosition);
            TileBase tile = map.GetTile(gridPosition);

            return dataFromTiles[tile].tileName;
        }
        catch (Exception)
        {
            return "error"; 
        }

    }
}
