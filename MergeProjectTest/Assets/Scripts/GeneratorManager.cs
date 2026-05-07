using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages the queue of available generators and exposes the current one.
/// Notifies UI when the active generator changes.
/// </summary>
public class GeneratorManager : MonoBehaviour
{
    [Header("Available Generators")] [SerializeField] private List<GameObject> availableGenerators = new();

    private int currentIndex = 0;

    /// <summary>
    /// Fired whenever the active generator changes.
    /// </summary>
    public event Action<GameObject> OnGeneratorChanged;

    /// <summary>
    /// Returns the currently active generator prefab.
    /// </summary>
    public GameObject Current =>
        (availableGenerators.Count > 0 && currentIndex < availableGenerators.Count)
            ? availableGenerators[currentIndex]
            : null;

    /// <summary>
    /// Moves to the next generator in the list.
    /// Called after a generator is placed.
    /// </summary>
    public void Advance()
    {
        if (availableGenerators.Count == 0) return;
        
        currentIndex = Mathf.Min(currentIndex + 1, availableGenerators.Count - 1);
        OnGeneratorChanged?.Invoke(Current);
    }

    /// <summary>
    /// Adds a new generator to the queue (future reward system).
    /// </summary>
    /// <param name="prefab">The generator prefab to add to the queue.</param>
    public void AddGenerator(GameObject prefab)
    {
        availableGenerators.Add(prefab);
        
        // If this is the first generator, notify UI
        if (availableGenerators.Count == 1) OnGeneratorChanged?.Invoke(Current);
    }
}
