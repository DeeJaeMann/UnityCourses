using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// EditMode test suite for <see cref="BoardManager"/>.
/// Only includes tests that do not require Tilemap behavior or PlayMode execution.
/// Tilemap-dependent logic is intentionally excluded and will be tested in PlayMode.
/// </summary>
public class BoardManager_EditModeTests
{
    #region Fields

    private GameObject _root;
    private BoardManager _board;

    #endregion


    #region SetupTeardown

    /// <summary>
    /// Creates a fresh <see cref="BoardManager"/> instance before each test.
    /// Tilemaps and pooling are not initialized here because they require PlayMode.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _root = new GameObject("BoardManager_TestObject");
        _board = _root.AddComponent<BoardManager>();

        // Manually initialize the private dictionary because Awake() will not run in EditMode.
        typeof(BoardManager)
            .GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(_board, new Dictionary<Vector3Int, BoardItem>());
    }

    /// <summary>
    /// Destroys the temporary test object after each test.
    /// Ensures no state leaks between tests.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(_root);
    }

    #endregion


    #region IsOccupiedTests

    /// <summary>
    /// Ensures <see cref="BoardManager.IsOccupied"/> returns false
    /// when the internal dictionary contains no entry for the given cell.
    /// </summary>
    [Test]
    public void IsOccupied_ReturnsFalse_WhenCellEmpty()
    {
        var cell = new Vector3Int(1, 1, 0);

        Assert.IsFalse(
            _board.IsOccupied(cell),
            "IsOccupied should return false when the internal dictionary contains no entry for the given cell."
        );
    }

    /// <summary>
    /// Ensures <see cref="BoardManager.IsOccupied"/> returns true
    /// when an item is manually inserted into the internal dictionary.
    /// This avoids Tilemap and pooling dependencies.
    /// </summary>
    [Test]
    public void IsOccupied_ReturnsTrue_WhenItemManuallyAdded()
    {
        var cell = new Vector3Int(2, 2, 0);
        BoardItem item = CreateItem();

        Dictionary<Vector3Int, BoardItem> dict = GetInternalDictionary();
        dict[cell] = item;

        Assert.IsTrue(
            _board.IsOccupied(cell),
            "IsOccupied should return true when the internal dictionary contains an entry for the given cell."
        );
    }

    #endregion


    #region GetItemTests

    /// <summary>
    /// Ensures <see cref="BoardManager.GetItem"/> returns null
    /// when the cell contains no item.
    /// </summary>
    [Test]
    public void GetItem_ReturnsNull_WhenCellEmpty()
    {
        var cell = new Vector3Int(3, 3, 0);

        Assert.IsNull(
            _board.GetItem(cell),
            "GetItem should return null when no item is registered at the given cell."
        );
    }

    /// <summary>
    /// Ensures <see cref="BoardManager.GetItem"/> returns the correct item
    /// when one is manually registered in the internal dictionary.
    /// </summary>
    [Test]
    public void GetItem_ReturnsItem_WhenPresent()
    {
        var cell = new Vector3Int(4, 4, 0);
        BoardItem item = CreateItem();

        Dictionary<Vector3Int, BoardItem> dict = GetInternalDictionary();
        dict[cell] = item;

        Assert.AreEqual(
            item,
            _board.GetItem(cell),
            "GetItem should return the item stored in the internal dictionary for the given cell."
        );
    }

    #endregion


    #region RemoveItemTests

    /// <summary>
    /// Ensures <see cref="BoardManager.RemoveItem"/> does nothing
    /// when the cell contains no item.
    /// </summary>
    [Test]
    public void RemoveItem_DoesNothing_WhenCellEmpty()
    {
        var cell = new Vector3Int(5, 5, 0);

        Assert.DoesNotThrow(
            () => _board.RemoveItem(cell),
            "RemoveItem should not throw when attempting to remove from an empty cell."
        );
    }

    #endregion


    #region Helpers

    /// <summary>
    /// Creates a <see cref="BoardItem"/> instance with its prefabReference assigned.
    /// This avoids needing ObjectPoolManager in EditMode.
    /// </summary>
    private BoardItem CreateItem()
    {
        var gObj = new GameObject("Item");
        BoardItem item = gObj.AddComponent<BoardItem>();

        typeof(BoardItem)
            .GetField("prefabReference", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(item, gObj);

        return item;
    }

    /// <summary>
    /// Retrieves the private <c>_items</c> dictionary from <see cref="BoardManager"/>
    /// using reflection so tests can inject items without Tilemap or pooling.
    /// </summary>
    private Dictionary<Vector3Int, BoardItem> GetInternalDictionary()
    {
        return (Dictionary<Vector3Int, BoardItem>)
            typeof(BoardManager)
                .GetField("_items", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(_board);
    }

    #endregion
}
