using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// PlayMode test suite for <see cref="ObjectPoolManager"/>.
/// Covers all runtime-dependent behavior including instantiation,
/// activation, deactivation, and singleton initialization.
/// </summary>
public class ObjectPoolManager_PlayModeTests
{
    #region Fields

    private GameObject _root;
    private ObjectPoolManager _pool;

    #endregion

    #region SetupTeardown

    /// <summary>
    /// Creates a real ObjectPoolManager instance in a PlayMode scene.
    /// Awake() runs normally in PlayMode, so the singleton and dictionary
    /// are initialized automatically.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _root = new GameObject("PoolManager_PlayMode");
        _pool = _root.AddComponent<ObjectPoolManager>();

        // Awake() runs automatically in PlayMode, so Instance is set.
    }

    /// <summary>
    /// Cleans up the scene and resets the singleton.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        typeof(ObjectPoolManager)
            .GetProperty("Instance", BindingFlags.Static | BindingFlags.Public)
            ?.SetValue(null, null);

        Object.DestroyImmediate(_root);
    }

    #endregion

    #region SingletonTests

    /// <summary>
    /// Ensures Awake() assigns the singleton instance in PlayMode.
    /// </summary>
    [Test]
    public void Instance_IsAssigned_ByAwake()
    {
        Assert.IsNotNull(
            ObjectPoolManager.Instance,
            "ObjectPoolManager.Instance should be assigned automatically by Awake() in PlayMode."
        );

        Assert.AreSame(
            _pool,
            ObjectPoolManager.Instance,
            "ObjectPoolManager.Instance should reference the created component."
        );
    }

    #endregion

    #region GetPrefabTests

    /// <summary>
    /// Ensures GetPrefab instantiates a new object when the pool is empty.
    /// </summary>
    [UnityTest]
    public System.Collections.IEnumerator GetPrefab_CreatesNewInstance_WhenPoolEmpty()
    {
        var prefab = new GameObject("TestPrefab");

        GameObject instance = _pool.GetPrefab(prefab);

        Assert.IsNotNull(instance, "GetPrefab should instantiate a new object when the pool is empty.");
        Assert.AreNotSame(prefab, instance, "GetPrefab should not return the prefab itself.");
        Assert.IsTrue(instance.activeSelf, "Newly created instances should be active.");

        yield return null;
    }

    /// <summary>
    /// Ensures GetPrefab returns a pooled instance when one exists.
    /// </summary>
    [UnityTest]
    public System.Collections.IEnumerator GetPrefab_ReturnsPooledInstance_WhenAvailable()
    {
        var prefab = new GameObject("TestPrefab");
        var pooledObj = new GameObject("PooledInstance");
        pooledObj.SetActive(false);

        // Insert into pool via reflection
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

        yield return null;
    }

    #endregion

    #region ReturnPrefabTests

    /// <summary>
    /// Ensures ReturnPrefab deactivates the object and enqueues it.
    /// </summary>
    [UnityTest]
    public System.Collections.IEnumerator ReturnPrefab_StoresInstanceInPool()
    {
        var prefab = new GameObject("TestPrefab");
        var instance = new GameObject("Instance");

        _pool.ReturnPrefab(prefab, instance);

        Dictionary<GameObject, Queue<GameObject>> pools = GetInternalPoolDictionary();

        Assert.IsTrue(
            pools.ContainsKey(prefab),
            "ReturnPrefab should create a pool entry for the prefab."
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

        yield return null;
    }

    #endregion

    #region PrewarmTests

    /// <summary>
    /// Ensures Prewarm creates inactive pooled instances.
    /// </summary>
    [UnityTest]
    public System.Collections.IEnumerator Prewarm_CreatesInactiveInstances()
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

        yield return null;
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
