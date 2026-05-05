using UnityEngine;

/// <summary>
/// Represents an item placed on the board, such as a fruit or generator.
/// Stores the tile cell position so the BoardManager can track occupancy.
/// </summary>
public class BoardItem : MonoBehaviour
{
    /// <summary>
    /// The tile cell position this item occupies on the board.
    /// Updated by the BoardManager when the item is placed or moved.
    /// </summary>
    public Vector3Int CellPosition { get; set; }

    /// <summary>
    /// The prefab type this item belongs to.
    /// Required so the ObjectPoolManager knows which pool to return it to.
    /// </summary>
    [SerializeField] private GameObject prefabReference;
    
    /// <summary>
    /// The prefab type this item belongs to.
    /// Required so the ObjectPoolManager knows which pool to return it to.
    /// </summary>
    public GameObject PrefabReference => prefabReference;

    public virtual void ResetItem()
    {
        CellPosition = Vector3Int.zero;
        // TODO:Add more reset logic later (animation, timers, merge state, etc. )
    }
}
