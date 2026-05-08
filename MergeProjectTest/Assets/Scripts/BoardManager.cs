using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

/// <summary>
/// Manages board state, tile occupancy, and item placement.
/// Provides a single source of truth for all board interactions,
/// including placement validation, occupancy tracking, and item retrieval.
/// </summary>
public class BoardManager : MonoBehaviour
{
    #region Serialized Fields

    [Header("Tilemap References")]
    [Tooltip("The base tilemap that defines the playable board area.")]
    [SerializeField] 
    public Tilemap boardTilemap;

    [Tooltip("The overlay tilemap used for locked or highlighted cells.")]
    [SerializeField] 
    public Tilemap overlayTilemap;

    #endregion


    #region Private Fields

    /// <summary>
    /// Tracks all active items placed on the board, keyed by their tile cell position.
    /// Provides fast lookups for occupancy, merging, and movement.
    /// </summary>
    private Dictionary<Vector3Int, BoardItem> _items;

    #endregion


    #region Unity Lifecycle

    /// <summary>
    /// Initializes the internal item dictionary used to track board occupancy.
    /// </summary>
    private void Awake()
    {
        _items = new Dictionary<Vector3Int, BoardItem>();
    }

    #endregion


    #region Board Validation

    /// <summary>
    /// Determines whether the given cell is part of the playable board.
    /// A cell is valid if the base tilemap contains a tile at that position.
    /// </summary>
    /// <param name="cell">The tile cell position to check.</param>
    /// <returns>True if the cell is inside the board; otherwise false.</returns>
    public bool IsInsideBoard(Vector3Int cell)
    {
        return boardTilemap != null && boardTilemap.HasTile(cell);
    }

    /// <summary>
    /// Determines whether the given cell is locked by an overlay tile.
    /// Locked cells cannot accept item placement or movement.
    /// </summary>
    /// <param name="cell">The tile cell position to check.</param>
    /// <returns>True if the cell is locked; otherwise false.</returns>
    public bool IsLocked(Vector3Int cell)
    {
        return overlayTilemap != null && overlayTilemap.HasTile(cell);
    }

    /// <summary>
    /// Checks whether an item already occupies the specified cell.
    /// Used to prevent overlapping items on the board.
    /// </summary>
    /// <param name="cell">The tile cell position to check.</param>
    /// <returns>True if the cell contains an item; otherwise false.</returns>
    public bool IsOccupied(Vector3Int cell)
    {
        return _items.ContainsKey(cell);
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

    #endregion


    #region Placement Logic

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

        if (prefab == null)
            return false;

        Vector3 worldPosition = boardTilemap.GetCellCenterWorld(cell);

        // Retrieve pooled instance
        GameObject instance = ObjectPoolManager.Instance.GetPrefab(prefab);
        instance.transform.position = worldPosition;

        BoardItem boardItem = instance.GetComponent<BoardItem>();
        boardItem.ResetItem();
        boardItem.CellPosition = cell;

        _items[cell] = boardItem;

        return true;
    }

    /// <summary>
    /// Removes the item at the specified cell, if one exists.
    /// Returns the object to the pool and clears the occupancy record.
    /// </summary>
    /// <param name="cell">The tile cell position to clear.</param>
    public void RemoveItem(Vector3Int cell)
    {
        if (!_items.TryGetValue(cell, out BoardItem item))
            return;

        ObjectPoolManager.Instance.ReturnPrefab(item.PrefabReference, item.gameObject);
        _items.Remove(cell);
    }

    #endregion


    #region Item Retrieval

    /// <summary>
    /// Retrieves the item occupying the specified cell.
    /// Returns null if the cell is empty or outside the board.
    /// </summary>
    /// <param name="cell">The tile cell position to query.</param>
    /// <returns>The <see cref="BoardItem"/> at the cell, or null if none exists.</returns>
    public BoardItem GetItem(Vector3Int cell)
    {
        _items.TryGetValue(cell, out BoardItem item);
        return item;
    }

    #endregion
}
