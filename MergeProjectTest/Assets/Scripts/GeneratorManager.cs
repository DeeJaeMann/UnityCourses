using System;
using UnityEngine;

/// <summary>
/// Global state machine responsible for tracking the currently active generator
/// and notifying listeners (UI, input systems, etc.) whenever the active generator changes.
/// 
/// This component does NOT store the full list of available generators.
/// That responsibility belongs to <see cref="GeneratorInventory"/>.
/// 
/// The manager simply:
/// <list type="bullet">
/// <item><description>Receives generators from the inventory</description></item>
/// <item><description>Tracks the current generator index</description></item>
/// <item><description>Exposes the active generator</description></item>
/// <item><description>Raises <see cref="OnGeneratorChanged"/> when the active generator updates</description></item>
/// </list>
/// </summary>
public class GeneratorManager : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The list of generators provided by <see cref="GeneratorInventory"/>.
    /// This list is append‑only and represents the order in which generators
    /// become available to the player.
    /// </summary>
    private readonly System.Collections.Generic.List<GameObject> _generatorSequence = new();

    /// <summary>
    /// Index of the currently active generator within <see cref="_generatorSequence"/>.
    /// </summary>
    private int _currentGeneratorIndex = 0;

    #endregion


    #region Events

    /// <summary>
    /// Fired whenever the active generator changes.
    /// Subscribers (UI, input systems) should update their state accordingly.
    /// </summary>
    public event Action<GameObject> OnGeneratorChanged;

    #endregion


    #region Public Properties

    /// <summary>
    /// Returns the currently active generator prefab, or <c>null</c> if no generators
    /// have been provided yet.
    /// </summary>
    public GameObject CurrentGenerator =>
        (_generatorSequence.Count > 0 && _currentGeneratorIndex < _generatorSequence.Count)
            ? _generatorSequence[_currentGeneratorIndex]
            : null;

    #endregion


    #region Public API

    /// <summary>
    /// Adds a generator prefab to the sequence. This method is called exclusively
    /// by <see cref="GeneratorInventory"/> when new generators become available.
    /// 
    /// If this is the first generator added, the manager immediately notifies listeners.
    /// </summary>
    /// <param name="generatorPrefab">The generator prefab to add.</param>
    public void AddGenerator(GameObject generatorPrefab)
    {
        if (generatorPrefab is null)
        {
            return;
        }

        _generatorSequence.Add(generatorPrefab);

        // If this is the first generator, notify listeners immediately
        if (_generatorSequence.Count == 1)
        {
            OnGeneratorChanged?.Invoke(CurrentGenerator);           
        }

    }

    /// <summary>
    /// Advances to the next generator in the sequence. This is typically called
    /// after the player places the current generator on the board.
    /// 
    /// If already at the final generator, the manager remains on the last entry.
    /// </summary>
    public void Advance()
    {
        if (_generatorSequence.Count == 0)
            return;

        // Clamp to last generator
        int nextIndex = Mathf.Min(_currentGeneratorIndex + 1, _generatorSequence.Count - 1);

        // Only fire event if the generator actually changed
        if (nextIndex != _currentGeneratorIndex)
        {
            _currentGeneratorIndex = nextIndex;
            OnGeneratorChanged?.Invoke(CurrentGenerator);
        }
    }

    #endregion
}
