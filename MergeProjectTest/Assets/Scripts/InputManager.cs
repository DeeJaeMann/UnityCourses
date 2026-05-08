using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles all player input related to placing generators on the board.
/// This manager receives events from <see cref="GeneratorSlotUI"/> and
/// coordinates placement validation, coordinate conversion, and generator advancement.
/// 
/// The <see cref="InputManager"/> does NOT decide which generator is active.
/// That responsibility belongs to <see cref="GeneratorManager"/>.
/// </summary>
public class InputManager : MonoBehaviour
{
    #region Fields

    private Camera _mainCamera;

    #endregion


    #region Singleton

    /// <summary>
    /// Gets the singleton instance of the <see cref="InputManager"/>.
    /// Guaranteed to reference the single active instance in the scene,
    /// or <c>null</c> if no instance has been initialized yet.
    /// </summary>
    public static InputManager Instance { get; private set; }

    /// <summary>
    /// Unity lifecycle method used to enforce the singleton pattern.
    /// If another <see cref="InputManager"/> instance already exists,
    /// this instance is destroyed; otherwise, it becomes the global
    /// <see cref="Instance"/>.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _mainCamera = Camera.main;
    }

    #endregion


    #region Input State

    /// <summary>
    /// Represents the current input mode handled by the <see cref="InputManager"/>.
    /// </summary>
    private enum InputState
    {
        Idle,
        ClickPlacement,
        DragPlacement
    }

    /// <summary>
    /// Tracks the current <see cref="InputState"/> used by the <see cref="InputManager"/>.
    /// </summary>
    private InputState _state = InputState.Idle;

    /// <summary>
    /// The prefab currently being placed.
    /// This is provided by <see cref="GeneratorSlotUI"/> when the user selects a generator.
    /// </summary>
    private GameObject _activePrefab;

    #endregion


    #region Placement Entry Points (Called by GeneratorSlotUI)

    /// <summary>
    /// Begins click‑to‑place mode for the given generator prefab.
    /// </summary>
    /// <param name="prefab">The generator prefab selected for placement.</param>
    public void BeginClickPlacement(GameObject prefab)
    {
        _activePrefab = prefab;
        _state = InputState.ClickPlacement;
    }

    /// <summary>
    /// Begins drag‑to‑place mode for the given generator prefab.
    /// </summary>
    /// <param name="prefab">The generator prefab selected for placement.</param>
    public void BeginDragPlacement(GameObject prefab)
    {
        _activePrefab = prefab;
        _state = InputState.DragPlacement;
    }

    /// <summary>
    /// Updates drag placement preview position.
    /// (Preview visuals will be implemented in a future iteration.)
    /// </summary>
    /// <param name="screenPosition">The current mouse position in screen coordinates.</param>
    public void UpdateDragPlacement(Vector3 screenPosition)
    {
        if (_state != InputState.DragPlacement)
            return;

        // TODO: Add drag placement preview visualization.
    }

    /// <summary>
    /// Ends drag‑to‑place mode and attempts to place the active generator at the given screen position.
    /// </summary>
    /// <param name="screenPosition">The mouse position in screen coordinates at drag release.</param>
    public void EndDragPlacement(Vector3 screenPosition)
    {
        if (_state != InputState.DragPlacement)
            return;

        TryPlaceAtScreenPosition(screenPosition);
        _state = InputState.Idle;
    }

    #endregion


    #region Click Placement (New Input System)

    /// <summary>
    /// Handles click‑to‑place input and attempts placement when the left mouse button is pressed.
    /// </summary>
    private void Update()
    {
        if (_state != InputState.ClickPlacement)
            return;

        if (Mouse.current?.leftButton.wasPressedThisFrame == true)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            TryPlaceAtScreenPosition(mousePosition);
            _state = InputState.Idle;
        }
    }

    #endregion


    #region Placement Logic

    /// <summary>
    /// Attempts to place the active generator at the given screen position.
    /// Performs camera validation, coordinate conversion, tilemap lookup,
    /// and placement validation before spawning the generator.
    /// </summary>
    /// <param name="screenPosition">The mouse position in screen coordinates.</param>
    private void TryPlaceAtScreenPosition(Vector3 screenPosition)
    {
        if (_activePrefab == null)
            return;

        if (_mainCamera == null)
            return;

        Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(screenPosition);

        var boardManager = FindAnyObjectByType<BoardManager>();
        if (boardManager == null || boardManager.boardTilemap == null)
            return;

        Vector3Int cellPosition = boardManager.boardTilemap.WorldToCell(worldPosition);

        if (boardManager.CanPlace(cellPosition))
        {
            boardManager.TryPlaceItem(_activePrefab, cellPosition);

            var generatorManager = FindAnyObjectByType<GeneratorManager>();
            generatorManager?.Advance();
        }
    }

    #endregion


    #region Test Accessors (Editor Only)

#if UNITY_EDITOR

    /// <summary>
    /// Exposes the currently active prefab for EditMode tests.
    /// </summary>
    public GameObject ActivePrefab => _activePrefab;

    /// <summary>
    /// Exposes the current input state for EditMode tests.
    /// </summary>
    public string State => _state.ToString();

    /// <summary>
    /// Clears the active prefab for EditMode tests.
    /// </summary>
    public void ClearActivePrefab() => _activePrefab = null;

    /// <summary>
    /// Clears the cached camera reference for EditMode tests.
    /// </summary>
    public void ForceClearCamera() => _mainCamera = null;

#endif

    #endregion
}
