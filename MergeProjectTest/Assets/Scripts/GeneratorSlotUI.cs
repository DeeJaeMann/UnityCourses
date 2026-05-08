using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Handles the visual representation of the generator slot UI element.
/// This component is responsible only for UI behavior:
/// displaying the generator icon, hiding it when no generator is available,
/// and exposing UI events (click, drag)for higher-level systems to use.
/// </summary>
public class GeneratorSlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    /// <summary>
    /// Reference to the Image component used to display the generator icon.
    /// This should be assigned to the "Icon" child object of the GeneratorSlot
    /// </summary>
    [Header("UI References")] [SerializeField]
    private Image iconImage;

    /// <summary>
    /// Reference to the <see cref="InputManager"/> responsible for handling
    /// click and drag placement requests. This must be assigned in the Inspector.
    /// The slot uses this reference to notify the input system when the user
    /// clicks or drags the generator icon.
    /// </summary>
    [Header("Managers")] [SerializeField]
    private InputManager inputManager;
    
    /// <summary>
    /// The currently active generator prefab provided by the GeneratorManager.
    /// Stored so InputManager can retrieve it during click or drag placement.
    /// </summary>
    private GameObject _currentPrefab;

    /// <summary>
    /// Cached reference to the <see cref="GeneratorManager"/> that provides
    /// the currently active generator prefab. This reference is used to
    /// subscribe to generator change events so the UI updates whenever the
    /// active generator is modified. Assigned automatically at runtime using
    /// <c>FindAnyObjectByType</c>.
    /// </summary>
    [Header("Managers")] [SerializeField]
    private GeneratorManager generatorManager;
    
    /// <summary>
    /// Unity lifecycle method.
    /// Ensures the icon starts with no sprite assigned while preserving
    /// the background color and alpha configured in the Inspector.
    /// </summary>
    private void Awake()
    {
        if (iconImage is not null)
        {
            iconImage.sprite = null;
        }
        // generatorManager = FindAnyObjectByType<GeneratorManager>();
        if (generatorManager is not null) generatorManager.OnGeneratorChanged += HandleGeneratorChanged;
    }

    /// <summary>
    /// Subscribes to the GeneratorManager event so the UI updates
    /// whenever the active generator changes.
    /// </summary>
    public void Start()
    {

    }

    /// <summary>
    /// Unity lifecycle method invoked when this UI element is destroyed.
    /// Ensures the slot unsubscribes from <see cref="GeneratorManager.OnGeneratorChanged"/>
    /// to prevent dangling event handlers, memory leaks, or callbacks targeting
    /// a destroyed object.
    /// </summary>
    public void OnDestroy()
    {
        if (generatorManager is not null) generatorManager.OnGeneratorChanged -= HandleGeneratorChanged;
    }

    /// <summary>
    /// Sets the generator sprite displayed inside the slot.
    /// Passing a null sprite hides the icon entirely.
    /// </summary>
    /// <param name="sprite">The sprite to display. If null, the icon is hidden.</param>
    public void SetGeneratorSprite(Sprite sprite)
    {
        if (iconImage is null)
            return;

        // TODO: Verify why changed
        // if (sprite is null)
        // {
        //     iconImage.sprite = null;
        //     return;
        // }
        
        // Assign and show the icon
        iconImage.sprite = sprite;

        // TODO: Verify why changed
        // Color updatedColor = iconImage.color;
        // updatedColor.a = 1f;
        // iconImage.color = updatedColor;
        
        iconImage.color = new Color(iconImage.color.r, iconImage.color.g, iconImage.color.b, sprite is null ? 0f :1f);
    }

    /// <summary>
    /// Called by the Button component when the slot is clicked.
    /// This method will later notify the InputManager to begin click placement.
    /// Currently implemented as a placeholder for future logic.
    /// </summary>
    public void OnSlotClicked()
    {
        // Placeholder for future click-to-place logic.
        // Will call InputManager.BeginClickPlacement(prefab) later.
        Debug.Log("GeneratorSlotUI.OnSlotClicked");
        if (_currentPrefab is null || inputManager is null) return;
        
        inputManager.BeginClickPlacement(_currentPrefab);
    }

    /// <summary>
    /// Called when a drag operation begins on this UI element.
    /// Notifies the InputManager to begin drag placement.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("GeneratorSlotUI.OnBeginDrag");
        if (_currentPrefab is null || inputManager is null) return;
        
        inputManager.BeginDragPlacement(_currentPrefab);
    }
    
    /// <summary>
    /// Called continuously while dragging the slot icon.
    /// This will later update the drag preview position.
    /// Currently implemented as a placeholder for future logic.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        // Placeholder for future drag logic.
        Debug.Log("GeneratorSlotUI.OnDrag");
        if (inputManager is null) return;
        
        inputManager.UpdateDragPlacement(eventData.position);
    }

    /// <summary>
    /// Called when the drag operation ends.
    /// This will later attempt placement or cancel the drag.
    /// Currently implemented as a placeholder for future logic.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        // Placeholder for future drag logic.
        Debug.Log("GeneratorSlotUI.OnEndDrag");
        if (inputManager is null) return;
        
        inputManager.EndDragPlacement(eventData.position);
    }

    /// <summary>
    /// Handles updates from the GeneratorManager when the active generator changes.
    /// Extracts the sprite from the prefab and updates the UI accordingly.
    /// </summary>
    /// <param name="prefab">The newly active generator prefab</param>
    private void HandleGeneratorChanged(GameObject prefab)
    {
        _currentPrefab = prefab;
        if (prefab is null)
        {
            SetGeneratorSprite(null);
            return;
        }
        
        // Extract sprite from prefab
        // SpriteRenderer spriteRenderer = prefab.GetComponentInChildren<SpriteRenderer>();
        // SetGeneratorSprite(spriteRenderer is not null ? spriteRenderer.sprite : null);
        SpriteRenderer spriteRenderer = prefab.GetComponent<SpriteRenderer>();
        iconImage.sprite = spriteRenderer is not null ? spriteRenderer.sprite : null;
        iconImage.color = Color.white;

    }
    
    /// <summary>
    /// Returns the currently active generator prefab.
    /// Used by InputManager during click or drag placement.
    /// </summary>
    /// <returns>The active generator prefab, or null if no generator is available.</returns>
    public GameObject GetCurrentPrefab() => _currentPrefab;
}
