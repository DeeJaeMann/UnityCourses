using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles all player input related to placing generators on the board.
/// This manager receives events from <see cref="GeneratorSlotUI"/> and
/// coordinates placement validation, coordinate conversion, and generator advancement.
/// </summary>
public class InputManager : MonoBehaviour
{
    private Camera _mainCamera;
    
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
    #region State

    /// <summary>
    /// Represents the current input mode handled by the <see cref="InputManager"/>.
    /// The state determines how user interactions are interpreted during generator
    /// placement operations.
    /// <list type="bullet">
    /// <item><description><c>Idle</c> — No placement is active.</description></item>
    /// <item><description><c>ClickPlacement</c> — A generator is awaiting a single click to place it.</description></item>
    /// <item><description><c>DragPlacement</c> — A generator is being dragged and will be placed on release.</description></item>
    /// </list>
    /// </summary>
    private enum InputState
    {
        Idle,
        ClickPlacement,
        DragPlacement
    }

    /// <summary>
    /// Tracks the current <see cref="InputState"/> used by the <see cref="InputManager"/>.
    /// This value determines how incoming input events are processed and ensures that
    /// click‑to‑place and drag‑to‑place behaviors remain mutually exclusive.
    /// </summary>
    private InputState _state = InputState.Idle;

    /// <summary>
    /// The prefab currently being placed.
    /// </summary>
    private GameObject _activePrefab;

    #endregion
    
    #region Placement Entry Points (Called by GeneratorSlotUI)

    /// <summary>
    /// Begins click-to-place mode for the given prefab.
    /// </summary>
    /// <param name="prefab">The generator prefab selected for placement.</param>
    public void BeginClickPlacement(GameObject prefab)
    {
        _activePrefab = prefab;
        _state = InputState.ClickPlacement;
    }

    /// <summary>
    /// Begins drat-to-place mode for the given generator prefab.
    /// </summary>
    /// <param name="prefab">The generator prefab selected for placement.</param>
    public void BeginDragPlacement(GameObject prefab)
    {
        _activePrefab = prefab;
        _state = InputState.DragPlacement;
    }

    /// <summary>
    /// Updates drag placement preview position.
    /// </summary>
    /// <param name="screenPosition">The current mouse position in screen coordinates.</param>
    public void UpdateDragPlacement(Vector3 screenPosition)
    {
        if (_state != InputState.DragPlacement) return;
        
        // TODO: Add drag placement
    }

    /// <summary>
    /// Ends drag‑to‑place mode and attempts to place the active generator at the given screen position.
    /// </summary>
    /// <param name="screenPosition">The mouse position in screen coordinates at drag release.</param>
    public void EndDragPlacement(Vector3 screenPosition)
    {
        if (_state != InputState.DragPlacement) return;
        TryPlaceAtScreenPosition(screenPosition);
        _state = InputState.Idle;
    }
    
    #endregion
    
    #region Click Placement

    /// <summary>
    /// Handles click‑to‑place input and attempts placement when the left mouse button is pressed.
    /// </summary>
    private void Update()
    {
        // TODO: Verify old input system?
        // if (_state == InputState.ClickPlacement && Input.GetMouseButtonDown(0))
        // {
        //     // Acceptable here because placement only occurs on a single click, not every frame.
        //     TryPlaceAtScreenPosition(Input.mousePosition);
        //     _state = InputState.Idle;
        // }
        if (_state == InputState.ClickPlacement)
        {
            // NEW INPUT SYSTEM CLICK
            if (Mouse.current?.leftButton.wasPressedThisFrame == true)
            {
                // Mouse position is read only on discrete click events, not every frame.
                Vector2 mousePos = Mouse.current.position.ReadValue();
                // Placement logic runs only on user-initiated actions, never per-frame polling.
                TryPlaceAtScreenPosition(mousePos);
                _state = InputState.Idle;
            }
        }
    }
    
    #endregion
    
    #region Placement Logic

    /// <summary>
    /// Attempts to place the active generator at the given screen position.
    /// </summary>
    /// <param name="screenPosition">The mouse position in screen coordinates.</param>
    private void TryPlaceAtScreenPosition(Vector3 screenPosition)
    {
        if (_activePrefab is null) return;
        if (_mainCamera is null) return;
        
        Vector3 worldPosition = _mainCamera.ScreenToWorldPoint(screenPosition);
        var board = FindAnyObjectByType<BoardManager>();
        if (board is null || board.boardTilemap is null) return;
        
        Vector3Int cellPosition = board.boardTilemap.WorldToCell(worldPosition);

        if (board.CanPlace(cellPosition))
        {
            // Placement occurs only on user action, not per frame.
            board.TryPlaceItem(_activePrefab, cellPosition);
            var generator = FindAnyObjectByType<GeneratorManager>();
            if (generator is not null)
            {
                // Generator advancement is infrequent and tied to placement events.
                generator.Advance();
            }
        }
    }
    
    #endregion
    #region Test Accessors
    
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
