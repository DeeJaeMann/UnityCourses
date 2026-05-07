using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// PlayMode test suite for <see cref="GeneratorSlotUI"/>.
/// These tests validate real Unity lifecycle behavior, sprite assignment,
/// and UI state changes using actual scene objects.
/// </summary>
public class GeneratorSlotUI_PlayModeTests
{
    #region Fields
    
    private GameObject _root;
    private GeneratorSlotUI _slotUI;
    private Image _iconImage;
    private GeneratorManager _manager;
    
    #endregion
    #region SetupTeardown

    /// <summary>
    /// Sets up a complete UI environment for each test, including:
    /// <list type="bullet">
    /// <item><description>A <see cref="Canvas"/> for UI rendering</description></item>
    /// <item><description>An <see cref="EventSystem"/> for UI events</description></item>
    /// <item><description>A root object containing <see cref="GeneratorSlotUI"/></description></item>
    /// <item><description>A child Icon object containing the <see cref="Image"/> used by the component</description></item>
    /// </list>
    /// The method also forces <see cref="GeneratorSlotUI.Awake"/> to execute by toggling
    /// the active state of the root object.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        // Create Canvas (required for UI)
        var canvasGObj = new GameObject("Canvas", typeof(Canvas));
        canvasGObj.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        
        // Create EventSystem (required for drag/click events)
        var eventSystemGObj = new GameObject("EventSystem", typeof(EventSystem));
        eventSystemGObj.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        
        // Manager must exist before SlotUI activates
        var managerObj = new GameObject("GeneratorManager");
        _manager = managerObj.AddComponent<GeneratorManager>();
        
        // Create root object with component
        _root = new GameObject("GeneratorSlotUI");
        _root.SetActive(false);
        
        // Create child Icon object
        var iconGObj = new GameObject("Icon");
        iconGObj.transform.SetParent(_root.transform);
        
        _iconImage = iconGObj.AddComponent<Image>();
        _slotUI = _root.AddComponent<GeneratorSlotUI>();
        
        // Assign via serialized field
        typeof(GeneratorSlotUI)
            .GetField("iconImage", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(_slotUI, _iconImage);
        
        // Force Unity to run Awake() in PlayMode
        _root.SetActive(true);
    }

    /// <summary>
    /// Cleans up all created objects after each test to ensure isolation
    /// and prevent cross-test contamination.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        // Destroy root object first
        if (_root != null)
            Object.Destroy(_root);

        GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include);

        foreach (GameObject obj in allObjects)
        {
            if (obj.hideFlags == HideFlags.NotEditable || obj.hideFlags == HideFlags.HideAndDontSave)
                continue;
            
            Object.Destroy(obj);
        }
    }
    
    #endregion
    #region LifecycleTests

    /// <summary>
    /// Validates that <see cref="GeneratorSlotUI.Awake"/> clears the icon sprite
    /// when the component initializes in PlayMode.
    /// This ensures the slot always begins in a known empty state.
    /// </summary>
    /// <returns>A coroutine used by the Unity Test Framework.</returns>
    [UnityTest]
    public IEnumerator Awake_ClearsSprite()
    {
        _iconImage.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);

        // Recreate the component so Awake runs AFTER sprite assignment
        Object.DestroyImmediate(_slotUI);
        _root.SetActive(false);
        _slotUI = _root.AddComponent<GeneratorSlotUI>();

        // Reassign serialized field
        typeof(GeneratorSlotUI)
            .GetField("iconImage", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(_slotUI, _iconImage);
        
        _root.SetActive(true);
        
        yield return null;
        
        Assert.IsNull(_iconImage.sprite, "Awake() should clear the iconImage sprite");
    }
    
    #endregion
    #region EventSubscriptions

    /// <summary>
    /// Verifies that <see cref="GeneratorSlotUI"/> successfully subscribes to
    /// <see cref="GeneratorManager.OnGeneratorChanged"/> during <c>Start()</c>
    /// and updates its UI state when the manager reports a new active generator.  
    /// 
    /// This test requires PlayMode because it depends on:
    /// <list type="bullet">
    /// <item><description>Unity's lifecycle invoking <c>Start()</c> automatically</description></item>
    /// <item><description>Real event dispatch timing across frames</description></item>
    /// <item><description>Sprite extraction from an actual prefab instance</description></item>
    /// </list>
    /// The test confirms that the slot:
    /// <list type="bullet">
    /// <item><description>Stores the newly active prefab via <see cref="GeneratorSlotUI.GetCurrentPrefab"/></description></item>
    /// <item><description>Updates the displayed icon to match the prefab's <see cref="SpriteRenderer"/> sprite</description></item>
    /// </list>
    /// </summary>
    [UnityTest]
    public IEnumerator Start_SubscribesToGeneratorManager_AndUpdatesUI()
    {
        // Create prefab with sprite
        var prefab = new GameObject("Prefab");
        SpriteRenderer spriteRenderer = prefab.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
        
        // Add generator -> should trigger event -> UI updates
        _manager.AddGenerator(prefab);
        
        yield return null;
        
        Assert.AreSame(prefab, _slotUI.GetCurrentPrefab());
        Assert.AreEqual(spriteRenderer.sprite, _iconImage.sprite);
    }
    
    #endregion
    #region SelectGeneratorSpriteTests
    // TODO: Verify removal -> present in EditMode Tests?
    // /// <summary>
    // /// Ensures that calling <see cref="GeneratorSlotUI.SetGeneratorSprite"/> with a null
    // /// argument clears the sprite reference in PlayMode, matching the intended behavior
    // /// of hiding the icon when no generator is available.
    // /// </summary>
    // /// <returns>A coroutine used by the Unity Test Framework.</returns>
    // [UnityTest]
    // public IEnumerator SetGeneratorSprite_Null_ClearsSprite()
    // {
    //    
    //     _iconImage.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
    //     
    //     _slotUI.SetGeneratorSprite(null);
    //     
    //     yield return null;
    //     
    //     Assert.IsNull(_iconImage.sprite, "SetGeneratorSprite(null) should clear the iconImage sprite.");
    // }
    
    // TODO: Verify -> present in EditMode Tests?
    // /// <summary>
    // /// Validates that providing a non-null sprite to
    // /// <see cref="GeneratorSlotUI.SetGeneratorSprite"/> assigns the sprite and forces
    // /// the icon alpha to 1. This ensures the icon is fully visible when a generator
    // /// is present.
    // /// </summary>
    // /// <returns>A coroutine used by the Unity Test Framework.</returns>
    // [UnityTest]
    // public IEnumerator SetGeneratorSprite_AssignsSprite_AndForcesAlpha()
    // {
    //     var sprite = Sprite.Create(Texture2D.redTexture, new Rect(0, 0, 1, 1), Vector2.zero);
    //
    //     Color color = _iconImage.color;
    //     color.a = 0.25f;
    //     _iconImage.color = color;
    //
    //     _slotUI.SetGeneratorSprite(sprite);
    //     
    //     yield return null;
    //     
    //     Assert.AreEqual(sprite, _iconImage.sprite, "SetGeneratorSprite should assign the provided sprite.");
    //     Assert.AreEqual(1f, _iconImage.color.a, "SetGeneratorSprite should force the icon alpha to 1");
    // }
    
    #endregion
    #region SpriteExtractionTests

    /// <summary>
    /// Confirms that when <see cref="GeneratorManager"/> reports a newly active
    /// generator prefab that lacks a <see cref="SpriteRenderer"/>, the
    /// <see cref="GeneratorSlotUI"/> correctly updates its UI by clearing the icon
    /// sprite.  
    /// 
    /// This behavior is validated in PlayMode because it depends on:
    /// <list type="bullet">
    /// <item><description>Unity's lifecycle invoking <c>Start()</c> and establishing the event subscription</description></item>
    /// <item><description>Real-time event dispatch from <see cref="GeneratorManager"/></description></item>
    /// <item><description>Frame-based UI updates applied to the <see cref="Image"/> component</description></item>
    /// </list>
    /// The test ensures that the slot does not display stale or invalid visual data
    /// when the active generator provides no sprite source.
    /// </summary>

    [UnityTest]
    public IEnumerator HandleGeneratorChanged_NoSpriteRenderer_ResultsInNullSpite()
    {
        var prefab = new GameObject("Prefab_NoSpriteRenderer");
        _manager.AddGenerator(prefab);
        yield return null;
        Assert.IsNull(_iconImage.sprite);
    }
    
    #endregion
}
