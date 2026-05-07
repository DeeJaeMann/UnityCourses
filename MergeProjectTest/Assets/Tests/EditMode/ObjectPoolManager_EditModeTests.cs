using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// EditMode test suite for <see cref="ObjectPoolManager"/>.
/// Covers all pooling logic that does not require PlayMode execution.
/// </summary>
public class ObjectPoolManager_EditModeTests
{
    #region Fields

    private GameObject _root;
    private ObjectPoolManager _pool;

    #endregion

    #region SetupTeardown

    /// <summary>
    /// Creates a fresh ObjectPoolManager instance before each test.
    /// Awake() is not invoked automatically in EditMode, so the internal
    /// dictionary and singleton instance are initialized manually.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _root = new GameObject("PoolManager_TestObject");
        _pool = _root.AddComponent<ObjectPoolManager>();

        // Manually initialize singleton and dictionary
        typeof(ObjectPoolManager)
            .GetProperty("Instance", BindingFlags.Static | BindingFlags.Public)
            ?.SetValue(null, _pool);

        typeof(ObjectPoolManager)
            .GetField("pools", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(_pool, new Dictionary<GameObject, Queue<GameObject>>());
    }

    /// <summary>
    /// Cleans up the test object after each test.
    /// Ensures no state leaks between tests.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        // Reset the singleton instance via reflection
        typeof(ObjectPoolManager)
            .GetProperty("Instance", BindingFlags.Static | BindingFlags.Public)
            ?.SetValue(null, null);
        Object.DestroyImmediate(_root);
    }

    #endregion

    #region SingletonTests

    /// <summary>
    /// Ensures the singleton Instance property is assigned during setup.
    /// </summary>
    [Test]
    public void Instance_IsAssigned()
    {
        Assert.IsNotNull(
            ObjectPoolManager.Instance,
            "ObjectPoolManager.Instance should be assigned during test setup."
        );
    }

    #endregion

    #region GetPrefabTests

    /// <summary>
    /// Ensures GetPrefab creates a new instance when the pool is empty.
    /// </summary>
    [Test]
    public void GetPrefab_CreatesNewInstance_WhenPoolEmpty()
    {
        var prefab = new GameObject("TestPrefab");

        GameObject instance = _pool.GetPrefab(prefab);

        Assert.IsNotNull(instance, "GetPrefab should return a new instance when the pool is empty.");
        Assert.AreNotSame(prefab, instance, "GetPrefab should instantiate a new object, not return the prefab itself.");
        Assert.IsTrue(instance.activeSelf, "Newly created instances should be active.");
    }

    /// <summary>
    /// Ensures GetPrefab returns a pooled instance when one exists.
    /// </summary>
    [Test]
    public void GetPrefab_ReturnsPooledInstance_WhenAvailable()
    {
        var prefab = new GameObject("TestPrefab");
        var pooledObj = new GameObject("PooledInstance");
        pooledObj.SetActive(false);

        // Manually insert into pool
        Dictionary<GameObject, Queue<GameObject>> pools = GetInternalPoolDictionary();
        pools[prefab] = new Queue<GameObject>();
        pools[prefab].Enqueue(pooledObj);

        GameObject instance = _pool.GetPrefab(prefab);

        Assert.AreSame(
            pooledObj,
            instance,
            "GetPrefab should return the pooled instance when available."
        );

        Assert.IsTrue(
            instance.activeSelf,
            "Pooled instances should be reactivated when retrieved."
        );
    }

    #endregion

    #region ReturnPrefabTests

    /// <summary>
    /// Ensures ReturnPrefab deactivates the object and stores it in the pool.
    /// </summary>
    [Test]
    public void ReturnPrefab_StoresInstanceInPool()
    {
        var prefab = new GameObject("TestPrefab");
        var instance = new GameObject("Instance");

        _pool.ReturnPrefab(prefab, instance);

        Dictionary<GameObject, Queue<GameObject>> pools = GetInternalPoolDictionary();

        Assert.IsTrue(
            pools.ContainsKey(prefab),
            "ReturnPrefab should create a pool entry for the prefab if one does not exist."
        );

        Assert.AreEqual(
            1,
            pools[prefab].Count,
            "ReturnPrefab should enqueue the returned instance."
        );

        Assert.IsFalse(
            instance.activeSelf,
            "ReturnPrefab should deactivate the returned instance."
        );
    }

    #endregion

    #region PrewarmTests

    /// <summary>
    /// Ensures Prewarm creates the correct number of inactive instances.
    /// </summary>
    [Test]
    public void Prewarm_CreatesInactiveInstances()
    {
        var prefab = new GameObject("TestPrefab");

        _pool.Prewarm(prefab, 3);

        Dictionary<GameObject, Queue<GameObject>> pools = GetInternalPoolDictionary();

        Assert.IsTrue(
            pools.ContainsKey(prefab),
            "Prewarm should create a pool entry for the prefab."
        );

        Assert.AreEqual(
            3,
            pools[prefab].Count,
            "Prewarm should create the specified number of instances."
        );

        foreach (GameObject obj in pools[prefab])
        {
            Assert.IsFalse(
                obj.activeSelf,
                "Prewarm should create inactive instances."
            );
        }
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Retrieves the private pools dictionary via reflection.
    /// </summary>
    private Dictionary<GameObject, Queue<GameObject>> GetInternalPoolDictionary()
    {
        return (Dictionary<GameObject, Queue<GameObject>>)
            typeof(ObjectPoolManager)
                .GetField("pools", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.GetValue(_pool);
    }

    #endregion
}
