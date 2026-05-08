using NUnit.Framework;
using UnityEngine;

/// <summary>
/// EditMode tests for <see cref="InputManager"/>.
/// These tests validate internal state transitions and guard clauses
/// without requiring Unity scene objects or MonoBehaviour collaborators.
/// </summary>
public class InputManager_EditModeTests
{
    private InputManager _input;

    [SetUp]
    public void Setup()
    {
        var gObj = new GameObject("InputManager");
        _input = gObj.AddComponent<InputManager>();
    }

    /// <summary>
    /// Ensures BeginClickPlacement sets the active prefab and state.
    /// </summary>
    [Test]
    public void BeginClickPlacement_SetsStateAndPrefab()
    {
        var prefab = new GameObject("TestPrefab");

        _input.BeginClickPlacement(prefab);

        Assert.That(_input.ActivePrefab, Is.EqualTo(prefab));
        Assert.That(_input.State, Is.EqualTo("ClickPlacement"));
    }


    /// <summary>
    /// Ensures BeginDragPlacement sets the active prefab and state.
    /// </summary>
    [Test]
    public void BeginDragPlacement_SetsStateAndPrefab()
    {
        var prefab = new GameObject("TestPrefab");

        _input.BeginDragPlacement(prefab);

        Assert.That(_input.ActivePrefab, Is.EqualTo(prefab));
        Assert.That(_input.State, Is.EqualTo("DragPlacement"));
    }

    /// <summary>
    /// Ensures EndDragPlacement does nothing when not in DragPlacement state.
    /// </summary>
    [Test]
    public void EndDragPlacement_IgnoredWhenNotDragging()
    {
        _input.EndDragPlacement(Vector3.zero);

        Assert.That(_input.State, Is.EqualTo("Idle"));
    }
    
}
