using NUnit.Framework;
using UnityEngine;

/// <summary>
/// EditMode test suite for <see cref="GeneratorManager"/>.
/// Covers all logic that does not require PlayMode execution,
/// including generator queue behavior, event firing, and index handling.
/// </summary>
public class GeneratorManager_EditModeTests
{
    private GameObject _root;
    private GeneratorManager _manager;

    /// <summary>
    /// Creates a fresh GeneratorManager instance before each test.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _root = new GameObject("GeneratorManager_TestObject");
        _manager = _root.AddComponent<GeneratorManager>();
    }

    /// <summary>
    /// Cleans up the test object after each test.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(_root);
    }

    #region Property Tests

    /// <summary>
    /// Ensures Current returns null when no generators are available.
    /// </summary>
    [Test]
    public void Current_ReturnsNull_WhenListEmpty()
    {
        Assert.IsNull(
            _manager.Current,
            "Current should return null when no generators have been added."
        );
    }

    /// <summary>
    /// Ensures Current returns the first generator when one is added.
    /// </summary>
    [Test]
    public void Current_ReturnsFirstGenerator_WhenOneAdded()
    {
        GameObject prefab = new GameObject("GeneratorA");

        _manager.AddGenerator(prefab);

        Assert.AreSame(
            prefab,
            _manager.Current,
            "Current should return the first generator added to the list."
        );
    }

    #endregion
    #region AddGenerator Tests

    /// <summary>
    /// Ensures AddGenerator fires OnGeneratorChanged when adding the first generator.
    /// </summary>
    [Test]
    public void AddGenerator_FiresEvent_WhenFirstGeneratorAdded()
    {
        var prefab = new GameObject("GeneratorA");

        GameObject received = null;
        _manager.OnGeneratorChanged += g => received = g;

        _manager.AddGenerator(prefab);

        Assert.AreSame(
            prefab,
            received,
            "AddGenerator should fire OnGeneratorChanged when the first generator is added."
        );
    }

    /// <summary>
    /// Ensures AddGenerator does not fire OnGeneratorChanged when adding additional generators.
    /// </summary>
    [Test]
    public void AddGenerator_DoesNotFireEvent_WhenNotFirst()
    {
        var prefabA = new GameObject("GeneratorA");
        var prefabB = new GameObject("GeneratorB");

        int eventCount = 0;
        _manager.OnGeneratorChanged += _ => eventCount++;

        _manager.AddGenerator(prefabA); // Should fire
        _manager.AddGenerator(prefabB); // Should NOT fire

        Assert.AreEqual(
            1,
            eventCount,
            "AddGenerator should only fire OnGeneratorChanged when adding the first generator."
        );
    }

    #endregion
    #region Advance Tests

    /// <summary>
    /// Ensures Advance does nothing when no generators exist.
    /// </summary>
    [Test]
    public void Advance_DoesNothing_WhenListEmpty()
    {
        int eventCount = 0;
        _manager.OnGeneratorChanged += _ => eventCount++;

        _manager.Advance();

        Assert.AreEqual(
            0,
            eventCount,
            "Advance should not fire events when no generators exist."
        );
        Assert.IsNull(_manager.Current);
    }

    /// <summary>
    /// Ensures Advance does not move past the last generator.
    /// </summary>
    [Test]
    public void Advance_ClampsAtLastGenerator()
    {
        var prefabA = new GameObject("GeneratorA");
        var prefabB = new GameObject("GeneratorB");

        _manager.AddGenerator(prefabA);
        _manager.AddGenerator(prefabB);

        _manager.Advance(); // Move to B
        _manager.Advance(); // Should stay on B

        Assert.AreSame(
            prefabB,
            _manager.Current,
            "Advance should clamp at the last generator in the list."
        );
    }

    /// <summary>
    /// Ensures Advance fires OnGeneratorChanged when moving to the next generator.
    /// </summary>
    [Test]
    public void Advance_FiresEvent_WhenMovingToNext()
    {
        var prefabA = new GameObject("GeneratorA");
        var prefabB = new GameObject("GeneratorB");

        _manager.AddGenerator(prefabA);
        _manager.AddGenerator(prefabB);

        GameObject received = null;
        _manager.OnGeneratorChanged += g => received = g;

        _manager.Advance();

        Assert.AreSame(
            prefabB,
            received,
            "Advance should fire OnGeneratorChanged when moving to the next generator."
        );
    }
    #endregion
}
