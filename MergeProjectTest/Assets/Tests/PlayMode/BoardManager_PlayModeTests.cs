using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.TestTools;

/// <summary>
/// PlayMode test suite for <see cref="BoardManager"/>.
/// Covers all Tilemap-dependent and pooling-dependent behavior.
/// </summary>
public class BoardManager_PlayModeTests
{
    #region Fields

    private GameObject _root;
    private BoardManager _board;

    private GameObject _tilemapGObj;
    private Tilemap _boardTilemap;
    private Tilemap _overlayTilemap;

    private GameObject _poolGObj;
    private ObjectPoolManager _pool;

    private Tile _testTile;

    #endregion

    #region SetupTeardown

    /// <summary>
    /// Creates a full runtime environment including:
    /// - BoardManager
    /// - Real Tilemaps
    /// - Real ObjectPoolManager singleton
    /// </summary>
    [SetUp]
    public void Setup()
    {
        // Root object
        _root = new GameObject("BoardManager_TestObject");
        _board = _root.AddComponent<BoardManager>();

        // Tilemap setup
        _tilemapGObj = new GameObject("Tilemaps");
        _boardTilemap = _tilemapGObj.AddComponent<Tilemap>();
        _tilemapGObj.AddComponent<TilemapRenderer>();

        _overlayTilemap = new GameObject("OverlayTilemap").AddComponent<Tilemap>();
        _overlayTilemap.gameObject.AddComponent<TilemapRenderer>();

        _board.boardTilemap = _boardTilemap;
        _board.overlayTilemap = _overlayTilemap;

        // Pool setup
        _poolGObj = new GameObject("ObjectPoolManager");
        _pool = _poolGObj.AddComponent<ObjectPoolManager>();

        // Force singleton assignment
        typeof(ObjectPoolManager)
            .GetProperty("Instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
            ?.SetValue(null, _pool);

        // Test tile
        _testTile = ScriptableObject.CreateInstance<Tile>();
    }

    /// <summary>
    /// Cleans up all created objects after each test.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(_root);
        Object.DestroyImmediate(_tilemapGObj);
        Object.DestroyImmediate(_overlayTilemap.gameObject);
        Object.DestroyImmediate(_poolGObj);
    }

    #endregion
       #region IsInsideBoardTests

    /// <summary>
    /// Ensures IsInsideBoard returns true when the board tilemap contains a tile.
    /// </summary>
    [Test]
    public void IsInsideBoard_ReturnsTrue_WhenTileExists()
    {
        var cell = new Vector3Int(0, 0, 0);
        _boardTilemap.SetTile(cell, _testTile);

        Assert.IsTrue(
            _board.IsInsideBoard(cell),
            "IsInsideBoard should return true when the board tilemap contains a tile at the given cell."
        );
    }

    /// <summary>
    /// Ensures IsInsideBoard returns false when the board tilemap has no tile.
    /// </summary>
    [Test]
    public void IsInsideBoard_ReturnsFalse_WhenTileMissing()
    {
        var cell = new Vector3Int(1, 1, 0);

        Assert.IsFalse(
            _board.IsInsideBoard(cell),
            "IsInsideBoard should return false when the board tilemap does not contain a tile at the given cell."
        );
    }

    #endregion

    #region IsLockedTests

    /// <summary>
    /// Ensures IsLocked returns true when the overlay tilemap contains a tile.
    /// </summary>
    [Test]
    public void IsLocked_ReturnsTrue_WhenOverlayTileExists()
    {
        var cell = new Vector3Int(0, 0, 0);
        _overlayTilemap.SetTile(cell, _testTile);

        Assert.IsTrue(
            _board.IsLocked(cell),
            "IsLocked should return true when the overlay tilemap contains a tile at the given cell."
        );
    }

    /// <summary>
    /// Ensures IsLocked returns false when the overlay tilemap has no tile.
    /// </summary>
    [Test]
    public void IsLocked_ReturnsFalse_WhenOverlayTileMissing()
    {
        var cell = new Vector3Int(1, 1, 0);

        Assert.IsFalse(
            _board.IsLocked(cell),
            "IsLocked should return false when the overlay tilemap does not contain a tile at the given cell."
        );
    }

    #endregion

    #region CanPlaceTests

    /// <summary>
    /// Ensures CanPlace returns false when the cell is outside the board.
    /// </summary>
    [Test]
    public void CanPlace_ReturnsFalse_WhenOutsideBoard()
    {
        var cell = new Vector3Int(0, 0, 0);

        Assert.IsFalse(
            _board.CanPlace(cell),
            "CanPlace should return false when the board tilemap has no tile at the given cell."
        );
    }

    /// <summary>
    /// Ensures CanPlace returns false when the cell is locked.
    /// </summary>
    [Test]
    public void CanPlace_ReturnsFalse_WhenLocked()
    {
        var cell = new Vector3Int(0, 0, 0);
        _boardTilemap.SetTile(cell, _testTile);
        _overlayTilemap.SetTile(cell, _testTile);

        Assert.IsFalse(
            _board.CanPlace(cell),
            "CanPlace should return false when the overlay tilemap contains a tile at the given cell."
        );
    }

    /// <summary>
    /// Ensures CanPlace returns false when the cell is occupied.
    /// </summary>
    [Test]
    public void CanPlace_ReturnsFalse_WhenOccupied()
    {
        var cell = new Vector3Int(0, 0, 0);
        _boardTilemap.SetTile(cell, _testTile);

        // Place an item manually
        var prefab = new GameObject("ItemPrefab");
        prefab.AddComponent<BoardItem>();

        _board.TryPlaceItem(prefab, cell);

        Assert.IsFalse(
            _board.CanPlace(cell),
            "CanPlace should return false when the cell is already occupied."
        );
    }

    /// <summary>
    /// Ensures CanPlace returns true when the cell is valid, unlocked, and unoccupied.
    /// </summary>
    [Test]
    public void CanPlace_ReturnsTrue_WhenValid()
    {
        var cell = new Vector3Int(0, 0, 0);
        _boardTilemap.SetTile(cell, _testTile);

        Assert.IsTrue(
            _board.CanPlace(cell),
            "CanPlace should return true when the cell is inside the board, unlocked, and unoccupied."
        );
    }

    #endregion
    #region TryPlaceItemTests

    /// <summary>
    /// Ensures TryPlaceItem returns false when placement is not allowed.
    /// </summary>
    [Test]
    public void TryPlaceItem_Fails_WhenCannotPlace()
    {
        var prefab = new GameObject("ItemPrefab");
        prefab.AddComponent<BoardItem>();

        var cell = new Vector3Int(0, 0, 0);

        Assert.IsFalse(
            _board.TryPlaceItem(prefab, cell),
            "TryPlaceItem should return false when CanPlace returns false."
        );
    }

    /// <summary>
    /// Ensures TryPlaceItem positions the item at the correct world location.
    /// </summary>
    [Test]
    public void TryPlaceItem_SpawnsItemAtCorrectWorldPosition()
    {
        var cell = new Vector3Int(0, 0, 0);
        _boardTilemap.SetTile(cell, _testTile);

        var prefab = new GameObject("ItemPrefab");
        prefab.AddComponent<BoardItem>();

        _board.TryPlaceItem(prefab, cell);

        var item = _board.GetItem(cell);

        Assert.AreEqual(
            _boardTilemap.GetCellCenterWorld(cell),
            item.transform.position,
            "TryPlaceItem should position the spawned item at the tile's world center."
        );
    }

    /// <summary>
    /// Ensures TryPlaceItem registers the item in the internal dictionary.
    /// </summary>
    [Test]
    public void TryPlaceItem_RegistersItemInDictionary()
    {
        var cell = new Vector3Int(0, 0, 0);
        _boardTilemap.SetTile(cell, _testTile);

        var prefab = new GameObject("ItemPrefab");
        prefab.AddComponent<BoardItem>();

        _board.TryPlaceItem(prefab, cell);

        Assert.IsNotNull(
            _board.GetItem(cell),
            "TryPlaceItem should register the placed item in the internal dictionary."
        );
    }

    #endregion

    #region RemoveItemTests

    /// <summary>
    /// Ensures RemoveItem returns the item to the pool and removes it from the dictionary.
    /// </summary>
    [Test]
    public void RemoveItem_RemovesItemAndReturnsToPool()
    {
        var cell = new Vector3Int(0, 0, 0);
        _boardTilemap.SetTile(cell, _testTile);

        var prefab = new GameObject("ItemPrefab");
        prefab.AddComponent<BoardItem>();

        _board.TryPlaceItem(prefab, cell);

        BoardItem item = _board.GetItem(cell);

        _board.RemoveItem(cell);

        Assert.IsFalse(
            _board.IsOccupied(cell),
            "RemoveItem should remove the item from the internal dictionary."
        );

        Assert.IsFalse(
            item.gameObject.activeSelf,
            "RemoveItem should deactivate the item when returning it to the pool."
        );
    }

    #endregion
}