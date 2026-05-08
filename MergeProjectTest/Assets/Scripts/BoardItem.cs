using UnityEngine;

/// <summary>
/// Represents an item placed on the board, such as a generator or fruit.
/// Stores the tile cell position so the <see cref="BoardManager"/> can track occupancy.
/// 
/// This component is intentionally lightweight. It contains only the data and
/// reset logic required for pooled board items. Additional behavior such as
/// merge state, animations, timers, or upgrade logic can be added in future
/// iterations by extending this class.
/// </summary>
public class BoardItem : MonoBehaviour
{
    #region Properties

    /// <summary>
    /// The tile cell position this item occupies on the board.
    /// Updated by the <see cref="BoardManager"/> when the item is placed or moved.
    /// </summary>
    public Vector3Int CellPosition { get; set; }

    #endregion


    #region Serialized Fields

    [Header("Prefab Reference")]
    [Tooltip("The prefab type this item belongs to. Required so the ObjectPoolManager knows which pool to return it to.")]
    [SerializeField]
    private GameObject prefabReference;

    /// <summary>
    /// The prefab type this item belongs to.
    /// Required so the <see cref="ObjectPoolManager"/> knows which pool to return it to.
    /// </summary>
    public GameObject PrefabReference => prefabReference;

    #endregion


    #region Reset Logic

    /// <summary>
    /// Resets the item's state before reuse.
    /// Called automatically by the <see cref="BoardManager"/> when retrieving
    /// an instance from the object pool.
    /// 
    /// Additional reset logic (merge state, animations, timers, etc.)
    /// can be added here as the game evolves.
    /// </summary>
    public virtual void ResetItem()
    {
        CellPosition = Vector3Int.zero;
        // Future expansion: reset animations, merge state, timers, etc.
    }

    #endregion
}