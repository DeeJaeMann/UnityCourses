using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Centeralized pooling system that manages reusable GameObjects.
/// Reduces runtime allocations and destruction costs, especially on mobile.
/// </summary>
public class ObjectPoolManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance for global access. Ensures only one pool manager exists.
    /// </summary>
    public static ObjectPoolManager Instance { get; private set; }

    /// <summary>
    /// Internal dictionary mapping prefab types to their object pools.
    /// Each pool stores inactive instances ready for reuse.
    /// </summary>
    private Dictionary<GameObject, Queue<GameObject>> pools;

    /// <summary>
    /// Initializes the pooling system and enforces the singleton instance.
    /// Sets up the internal dictionary used to store object pools and ensures
    /// only one ObjectPoolManager exists in the scene.
    /// </summary>
    private void Awake()
    {
        // Enforce singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        pools = new Dictionary<GameObject, Queue<GameObject>>();
    }

    /// <summary>
    /// Ensures a pool exists for the given prefab.
    /// Creates a new pool if one has not been initialized yet.
    /// </summary>
    /// <param name="prefab">The prefab type to prepare a pool for.</param>
    private void EnsurePoolExists(GameObject prefab)
    {
        if (!pools.ContainsKey(prefab)) 
            pools[prefab] = new Queue<GameObject>();
    }

    /// <summary>
    /// Retrieves an instance of the specified prefab from its pool.
    /// Creates a new instance if the pool is empty.
    /// </summary>
    /// <param name="prefab">The prefab type to retrieve.</param>
    /// <returns>A pooled or newly instantiated GameObject.</returns>
    public GameObject GetPrefab(GameObject prefab)
    {
        EnsurePoolExists(prefab);

        if (pools[prefab].Count > 0)
        {
            GameObject obj = pools[prefab].Dequeue();
            obj.SetActive(true);
            return obj;
        }
        
        // Pool empty -> create new instance
        return Instantiate(prefab);
    }

    /// <summary>
    /// Returns a GameObject to its pool.
    /// The object is deactivated and stored for future reuse.
    /// </summary>
    /// <param name="prefab">The prefab type associated with the object.</param>
    /// <param name="obj">The instance to return to the pool.</param>
    public void ReturnPrefab(GameObject prefab, GameObject obj)
    {
        EnsurePoolExists(prefab);
        
        obj.SetActive(false);
        pools[prefab].Enqueue(obj);
    }

    /// <summary>
    /// Pre-warms a pool by creating a specified number of instances in advance.
    /// Useful for avoiding runtime spikes during gameplay.
    /// </summary>
    /// <param name="prefab">The prefab type to pre-warm.</param>
    /// <param name="count">How many instances to create.</param>
    public void Prewarm(GameObject prefab, int count)
    {
        EnsurePoolExists(prefab);

        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pools[prefab].Enqueue(obj);
        }
    }
}
