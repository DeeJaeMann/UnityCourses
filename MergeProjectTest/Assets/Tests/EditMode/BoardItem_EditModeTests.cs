using System.Reflection;
using UnityEngine;
using NUnit.Framework;

/// <summary>
/// EditMode test suite for <see cref="BoardItem"/>.
/// These tests validate property behavior, prefab reference access,
/// and <see cref="BoardItem.ResetItem"/> logic in a lightweight,
/// deterministic environment without require PlayMode or scene objects.
/// </summary>
public class BoardItem_EditModeTests
{
    private GameObject _GObj;
    private BoardItem _item;

    /// <summary>
    /// Creates a fresh <see cref="GameObject"/> and attaches a
    /// <see cref="BoardItem"/> component before each test.
    /// Ensures each test runs in complete isolation with no shared state.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _GObj = new GameObject("BoardItem_TestObject");
        _item = _GObj.AddComponent<BoardItem>();
    }

    /// <summary>
    /// Immediately destroys the temporary test object created in <see cref="SetUp"/>.
    /// Uses <see cref="Object.DestroyImmediate(Object)"/> to ensure cleanup occurs
    /// synchronously in EditMode.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_GObj);
    }

    /// <summary>
    /// Verifies that a newly created <see cref="BoardItem"/> begins with a
    /// <see cref="BoardItem.CellPosition"/> of <see cref="Vector3.zero"/>.
    /// Unity initializes auto-properties to their default values, and this test
    /// ensures that assumption remains correct.
    /// </summary>
    [Test]
    public void CellPosition_DefaultsToZero()
    {
        Assert.AreEqual(Vector3Int.zero, _item.CellPosition,
            "A new BoardItem should default CellPosition to Vector3Int.zero.");
    }

    /// <summary>
    /// Ensures that <see cref="BoardItem.CellPosition"/> can be assigned and retrieved
    /// correctly. This confirms that the auto-property behaves as expected and that
    /// no additional logic interferes with assignment.
    /// </summary>
    [Test]
    public void CellPosition_CanBeAssigned()
    {
        var position = new Vector3Int(3, 5, 0);
        _item.CellPosition = position;
        
        Assert.AreEqual(position, _item.CellPosition,
            "CellPosition should return the value that was assigned.");
    }

    /// <summary>
    /// Validates that <see cref="BoardItem.PrefabReference"/> correctly returns the
    /// private serialized field <c>prefabReference</c>. Reflection is used to assign
    /// the field because it is not publicly settable.
    /// </summary>
    [Test]
    public void PrefabReference_ReturnsSerializedField()
    {
        var prefab = new GameObject("PrefabMock");
        
        typeof(BoardItem)
            .GetField("prefabReference", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.SetValue(_item, prefab);
        
        Assert.AreEqual(prefab, _item.PrefabReference,
            "PrefabReference should return the serialized prefabReference field.");
    }

    /// <summary>
    /// Ensures that calling <see cref="BoardItem.ResetItem"/> resets the
    /// <see cref="BoardItem.CellPosition"/> to <see cref="Vector3Int.zero"/>.
    /// This guarantees that board state is properly cleared when items are reused.
    /// </summary>
    [Test]
    public void ResetItem_ClearsCellPosition()
    {
        _item.CellPosition = new Vector3Int(2, 2, 0);
        _item.ResetItem();
        Assert.AreEqual(Vector3Int.zero, _item.CellPosition,
            "ResetItem should reset CellPosition to Vector3Int.zero.");
    }

    /// <summary>
    /// Confirms that <see cref="BoardItem.ResetItem"/> does not modify the
    /// <see cref="BoardItem.PrefabReference"/> value. Resetting board state should
    /// not affect the item's pool identity or prefab association.
    /// </summary>
    [Test]
    public void ResetItem_DoesNotModifyPrefabReference()
    {
        var prefab = new GameObject("PrefabMock");
        typeof(BoardItem)
            .GetField("prefabReference", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.SetValue(_item, prefab);
        _item.ResetItem();
        Assert.AreEqual(prefab, _item.PrefabReference,
            "ResetItem should not modify the PrefabReference value.");
    }
}
