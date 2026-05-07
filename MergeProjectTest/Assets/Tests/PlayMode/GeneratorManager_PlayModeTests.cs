using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// PlayMode test suite for <see cref="GeneratorManager"/>.
/// Covers runtime behavior that depends on MonoBehaviour lifecycle
/// and event invocation inside a real PlayMode scene.
/// </summary>
public class GeneratorManager_PlayModeTests
{
    private GameObject _root;
    private GeneratorManager _manager;

    /// <summary>
    /// Creates a real GeneratorManager instance in a PlayMode scene.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _root = new GameObject("GeneratorManager_PlayMode");
        _manager = _root.AddComponent<GeneratorManager>();
    }

    /// <summary>
    /// Cleans up the scene after each test.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(_root);
    }

    #region Event Behavior Tests

    /// <summary>
    /// Ensures OnGeneratorChanged fires correctly when the first generator is added,
    /// using real PlayMode event invocation.
    /// </summary>
    [UnityTest]
    public System.Collections.IEnumerator OnGeneratorChanged_Fires_WhenFirstGeneratorAdded()
    {
        var prefab = new GameObject("GeneratorA");

        GameObject received = null;
        _manager.OnGeneratorChanged += g => received = g;

        _manager.AddGenerator(prefab);

        // Wait one frame to allow Unity event dispatch
        yield return null;

        Assert.AreSame(
            prefab,
            received,
            "OnGeneratorChanged should fire when the first generator is added in PlayMode."
        );
    }

    /// <summary>
    /// Ensures OnGeneratorChanged fires when advancing to the next generator
    /// inside a real PlayMode scene.
    /// </summary>
    [UnityTest]
    public System.Collections.IEnumerator OnGeneratorChanged_Fires_WhenAdvancing()
    {
        var prefabA = new GameObject("GeneratorA");
        var prefabB = new GameObject("GeneratorB");

        _manager.AddGenerator(prefabA);
        _manager.AddGenerator(prefabB);

        GameObject received = null;
        _manager.OnGeneratorChanged += g => received = g;

        _manager.Advance();

        // Wait one frame for event dispatch
        yield return null;

        Assert.AreSame(
            prefabB,
            received,
            "OnGeneratorChanged should fire when advancing to the next generator in PlayMode."
        );
    }

    #endregion
    #region Lifecycle Behavior Tests

    /// <summary>
    /// Ensures the component initializes correctly in a PlayMode scene
    /// and Current returns null before any generators are added.
    /// </summary>
    [UnityTest]
    public System.Collections.IEnumerator Current_IsNull_OnStart()
    {
        // Wait one frame to simulate scene start
        yield return null;

        Assert.IsNull(
            _manager.Current,
            "Current should be null at scene start before any generators are added."
        );
    }
    #endregion
}
