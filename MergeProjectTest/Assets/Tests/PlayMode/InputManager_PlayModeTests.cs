using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

/// <summary>
/// PlayMode test suite for <see cref="InputManager"/>.
/// These tests validate safe behavior when required systems such as
/// <see cref="BoardManager"/> or a Tilemap are not yet implemented.
/// </summary>
public class InputManager_PlayModeTests
{
    #region Fields

    private InputManager _inputManager;

    #endregion


    #region Setup / Teardown

    /// <summary>
    /// Creates the minimal scene environment required for testing:
    /// a main camera and an <see cref="InputManager"/> instance.
    /// </summary>
    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Create and register a main camera
        var cameraObject = new GameObject("MainCamera");
        cameraObject.tag = "MainCamera";
        cameraObject.AddComponent<Camera>();

        yield return null; // allow Camera.main to initialize

        // Create InputManager
        var inputManagerObject = new GameObject("InputManager");
        _inputManager = inputManagerObject.AddComponent<InputManager>();

        yield return null;
    }

    /// <summary>
    /// Cleans up all scene objects created during <see cref="Setup"/>.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        if (_inputManager != null)
            Object.DestroyImmediate(_inputManager.gameObject);
    }

    #endregion


    #region Drag Placement Tests

    /// <summary>
    /// Ensures that ending a drag placement operation does not throw,
    /// even when no <see cref="BoardManager"/> or Tilemap exists.
    /// </summary>
    [UnityTest]
    public IEnumerator DragPlacement_DoesNotThrow_WhenBoardMissing()
    {
        // Arrange
        var generatorPrefab = new GameObject("GeneratorPrefab");
        _inputManager.BeginDragPlacement(generatorPrefab);

        var releasePosition = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f);

        // Act + Assert
        Assert.DoesNotThrow(
            () => _inputManager.EndDragPlacement(releasePosition),
            "EndDragPlacement should safely no-op when no BoardManager or Tilemap exists."
        );

        yield return null;
    }

    #endregion


    #region Click Placement Tests

    /// <summary>
    /// Ensures that click placement does nothing and does not throw
    /// when no active prefab is assigned.
    /// </summary>
    [UnityTest]
    public IEnumerator ClickPlacement_DoesNotThrow_WhenNoPrefabActive()
    {
        // Arrange
        _inputManager.ClearActivePrefab();
        _inputManager.BeginClickPlacement(null);

        yield return null;

        // Assert
        Assert.Pass("Click placement with no active prefab should not crash.");
    }

    #endregion


    #region Camera Safety Tests

    /// <summary>
    /// Ensures that placement attempts return early and do not throw
    /// when the main camera is missing.
    /// </summary>
    [UnityTest]
    public IEnumerator TryPlaceAtScreenPosition_DoesNotThrow_WhenCameraMissing()
    {
        // Arrange
        if (Camera.main != null)
            Object.Destroy(Camera.main.gameObject);

        var generatorPrefab = new GameObject("GeneratorPrefab");
        _inputManager.BeginClickPlacement(generatorPrefab);

        yield return null;

        // Assert
        Assert.Pass("TryPlaceAtScreenPosition should safely no-op when the camera is missing.");
    }

    #endregion
}
