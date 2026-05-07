using UnityEngine;
using UnityEngine.EventSystems;
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
    /// The currently active generator prefab provided by the GeneratorManager.
    /// Stored so InputManager can retrieve it during click or drag placement.
    /// </summary>
    private GameObject currentPrefab;

    /// <summary>
    /// Unity lifecycle method.
    /// Ensures the icon starts with no sprite assigned while preserving
    /// the background color and alpha configured in the Inspector.
    /// </summary>
    private void Awake()
    {
        if (iconImage != null)
        {
            iconImage.sprite = null;
        }
    }

    /// <summary>
    /// Subscribes to the GeneratorManager event so the UI updates
    /// whenever the active generator changes.
    /// </summary>
    public void Start()
    {
        GeneratorManager gm = FindAnyObjectByType<GeneratorManager>();
        if (gm != null) gm.OnGeneratorChanged += HandleGeneratorChanged;
    }

    /// <summary>
    /// Sets the generator sprite displayed inside the slot.
    /// Passing a null sprite hides the icon entirely.
    /// </summary>
    /// <param name="sprite">The sprite to display. If null, the icon is hidden.</param>
    public void SetGeneratorSprite(Sprite sprite)
    {
        if (iconImage == null)
            return;

        if (sprite == null)
        {
            iconImage.sprite = null;
            return;
        }
        
        // Assign and show the icon
        iconImage.sprite = sprite;

        Color updatedColor = iconImage.color;
        updatedColor.a = 1f;
        iconImage.color = updatedColor;
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
    }

    /// <summary>
    /// Called when a drag operation begins on this UI element.
    /// Notifies the InputManager to begin drag placement.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("GeneratorSlotUI.OnBeginDrag");
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
    }

    /// <summary>
    /// Handles updates from the GeneratorManager when the active generator changes.
    /// Extracts the sprite from the prefab and updates the UI accordingly.
    /// </summary>
    /// <param name="prefab">The newly active generator prefab</param>
    private void HandleGeneratorChanged(GameObject prefab)
    {
        currentPrefab = prefab;

        if (prefab == null)
        {
            SetGeneratorSprite(null);
            return;
        }
        
        // Extract sprite from prefab
        SpriteRenderer spriteRenderer = prefab.GetComponentInChildren<SpriteRenderer>();
        SetGeneratorSprite(spriteRenderer != null ? spriteRenderer.sprite : null);
    }
    
    /// <summary>
    /// Returns the currently active generator prefab.
    /// Used by InputManager during click or drag placement.
    /// </summary>
    /// <returns>The active generator prefab, or null if no generator is available.</returns>
    public GameObject GetCurrentPrefab() => currentPrefab;
}
