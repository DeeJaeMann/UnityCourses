using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

/// <summary>
/// Manages board state, tile occupancy, and item placement.
/// Provides a single source of truth for all board interactions.
/// </summary>
public class BoardManager : MonoBehaviour
{
    /// <summary>
    /// The base tilemap that defines the playable board area.
    /// Used to validate cell positions and convert between world and grid coordinates.
    /// </summary>
    [Header("References")] public Tilemap boardTilemap;

    /// <summary>
    /// The overlay tilemap used for locked or highlighted cells.
    /// A cell is considered locked if this tilemap contains a tile at that position.
    /// </summary>
    public Tilemap overlayTilemap;

    /// <summary>
    /// Tracks all active items placed on the board, keyed by their tile cell position.
    /// Provides fast lookups for occupancy, merging, and movement.
    /// </summary>
    private Dictionary<Vector3Int, BoardItem> items;

    /// <summary>
    /// Initialize the internal item dictionary used to track board occupancy.
    /// Called automatically when the object is created.
    /// </summary>
    private void Awake()
    {
        items = new Dictionary<Vector3Int, BoardItem>();
    }

    /// <summary>
    /// Determines whether the given cell is part of the playable board.
    /// A cell is valid if the base tilemap contains a tile at that position.
    /// </summary>
    /// <param name="cell">The tile cell position to check.</param>
    /// <returns>True if the cell is inside the board; otherwise false.</returns>
    public bool IsInsideBoard(Vector3Int cell)
    {
        return boardTilemap.HasTile(cell);
    }

    /// <summary>
    /// Determines whether the given cell is locked by an overlay tile.
    /// Locked cells cannot accept item placement or movement.
    /// </summary>
    /// <param name="cell">The tile cell position to check.</param>
    /// <returns>True if the cell is locked; otherwise false.</returns>
    public bool IsLocked(Vector3Int cell)
    {
        return overlayTilemap.HasTile(cell);
    }

    /// <summary>
    /// Checks whether an item already occupies the specified cell.
    /// Used to prevent overlapping items on the board.
    /// </summary>
    /// <param name="cell">The tile cell position to check.</param>
    /// <returns>True if the cell contains an item; otherwise false.</returns>
    public bool IsOccupied(Vector3Int cell)
    {
        return items.ContainsKey(cell);
    }

    /// <summary>
    /// Determines whether an item can be placed at the given cell.
    /// A valid placement requires the cell to be inside the board, unlocked, and unoccupied.
    /// </summary>
    /// <param name="cell">The tile cell position to validate.</param>
    /// <returns>True if placement is allowed; otherwise false.</returns>
    public bool CanPlace(Vector3Int cell)
    {
        return IsInsideBoard(cell) && !IsLocked(cell) && !IsOccupied(cell);
    }

    /// <summary>
    /// Attempts to place an item prefab at the specified cell.
    /// Retrieves an instance from the object pool, positions it at the cell center,
    /// resets its state, and registers it in the board state.
    /// </summary>
    /// <param name="prefab">The item prefab to retrieve from the pool.</param>
    /// <param name="cell">The tile cell position where the item should be placed.</param>
    /// <returns>True if placement succeeded; otherwise false.</returns>
    public bool TryPlaceItem(GameObject prefab, Vector3Int cell)
    {
        if (!CanPlace(cell)) 
            return false;

        Vector3 worldPos = boardTilemap.GetCellCenterWorld(cell);
        // Refactored for mobile first design
        // GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity);
        GameObject obj = ObjectPoolManager.Instance.GetPrefab(prefab);
        obj.transform.position = worldPos;

        BoardItem item = obj.GetComponent<BoardItem>();
        item.ResetItem();
        item.CellPosition = cell;

        items[cell] = item;

        return true;
    }

    /// <summary>
    /// Removes the item at the specified cell, if one exists.
    /// Returns the object to the pool and clears the occupancy record.
    /// </summary>
    /// <param name="cell">The tile cell position to clear.</param>
    public void RemoveItem(Vector3Int cell)
    {
        if (!items.ContainsKey(cell)) return;
        
        // Refactored for mobile first design
        // Destroy(items[cell].gameObject);
        BoardItem item = items[cell];
        ObjectPoolManager.Instance.ReturnPrefab(item.PrefabReference, item.gameObject);
        items.Remove(cell);
    }

    /// <summary>
    /// Retrieves the item occupying the specified cell.
    /// Returns null if the cell is empty or outside the board.
    /// </summary>
    /// <param name="cell">The tile cell position to query.</param>
    /// <returns>The BoardItem at the cell, or null if none exists.</returns>
    public BoardItem GetItem(Vector3Int cell)
    {
        items.TryGetValue(cell, out BoardItem item);
        return item;
    }
}
