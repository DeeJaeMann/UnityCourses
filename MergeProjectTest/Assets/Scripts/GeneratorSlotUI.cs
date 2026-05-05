using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the visual representation of the generator slot UI element.
/// This component is responsible only for UI behavior:
/// displaying the generatory icon, hiding it when no generator is available,
/// and exposing UI events (click, drag)for higher-level systems to use.
/// </summary>
public class GeneratorSlotUI : MonoBehaviour
{
    /// <summary>
    /// Reference to the Image component used to display the generator icon.
    /// This should be assigned to the "Icon" child object of the GeneratorSlot
    /// </summary>
    [Header("UI References")] [SerializeField]
    private Image iconImage;

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
    }

    /// <summary>
    /// Called continuously while dragging the slot icon.
    /// This will later update the drag preview position.
    /// Currently implemented as a placeholder for future logic.
    /// </summary>
    public void OnDrag()
    {
        // Placeholder for future drag logic.
    }

    /// <summary>
    /// Called when the drag operation ends.
    /// This will later attempt placement or cancel the drag.
    /// Currently implemented as a placeholder for future logic.
    /// </summary>
    public void OnEndDrag()
    {
        // Placeholder for future drag logic.
    }
}
