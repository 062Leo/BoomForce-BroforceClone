using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class UDMapManager : MonoBehaviour
{
    [SerializeField] private Tilemap UDMap;
    [SerializeField] private List<TileDataUD> UDTileData;
    private static Dictionary<TileBase, TileDataUD> UDDataFromTiles;

    private void Awake()
    {
        UDDataFromTiles = new Dictionary<TileBase, TileDataUD>();
        foreach (var tileData in UDTileData)
        {
            foreach (var tile in tileData.tiles)
            {
                if (!UDDataFromTiles.ContainsKey(tile))
                {
                    UDDataFromTiles.Add(tile, tileData);
                }
                else
                {
                    Debug.LogWarning($"Tile {tile.name} is already in the dictionary.");
                }
            }
        }
    }

    public string getTileName(Vector2 worldPosition)
    {
        try
        {
            Vector3Int gridPosition = UDMap.WorldToCell(worldPosition);
            TileBase tile = UDMap.GetTile(gridPosition);

            return UDDataFromTiles[tile].tileName;
        }
        catch (Exception)
        {
            return "error";
        }
    }
}
