using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Scene-level data provider that exposes the list of available generator prefabs.
/// This component is responsible only for supplying generator prefabs to the
/// <see cref="GeneratorManager"/> at game start or when new generators become available.
/// 
/// It does NOT manage state, fire events, or coordinate UI. It simply acts as a
/// content source for generator prefabs.
/// </summary>
public class GeneratorInventory : MonoBehaviour
{
    #region Serialized Fields

    [FormerlySerializedAs("_availableGenerators")]
    [Header("Generator Prefabs")]
    [Tooltip("List of generator prefabs available to the player. The first entry will be used at game start.")]
    [SerializeField] 
    private List<GameObject> availableGenerators = new();

    #endregion


    #region Unity Lifecycle

    /// <summary>
    /// On scene start, provides the initial generator prefab to the
    /// <see cref="GeneratorManager"/> so the UI can display it and input
    /// interactions can begin immediately.
    /// </summary>
    private void Start()
    {
        if (availableGenerators.Count == 0)
        {
            Debug.LogWarning($"{nameof(GeneratorInventory)}: No generator prefabs assigned.");
            return;
        }

        var generatorManager = FindAnyObjectByType<GeneratorManager>();
        if (generatorManager == null)
        {
            Debug.LogWarning($"{nameof(GeneratorInventory)}: No GeneratorManager found in the scene.");
            return;
        }

        // Provide the first generator to the manager
        generatorManager.AddGenerator(availableGenerators[0]);
    }

    #endregion


    #region Public API

    /// <summary>
    /// Retrieves the next generator prefab in the inventory, if available.
    /// This method does not modify internal state; it simply exposes data.
    /// </summary>
    /// <param name="index">The index of the generator to retrieve.</param>
    /// <returns>
    /// The generator prefab at the specified index, or <c>null</c> if the index is invalid.
    /// </returns>
    public GameObject GetGeneratorAtIndex(int index)
    {
        if (index < 0 || index >= availableGenerators.Count)
            return null;

        return availableGenerators[index];
    }

    /// <summary>
    /// Returns the total number of generator prefabs available in the inventory.
    /// </summary>
    public int GeneratorCount => availableGenerators.Count;

    #endregion
}
