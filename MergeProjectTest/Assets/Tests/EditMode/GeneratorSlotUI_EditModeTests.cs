using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;

/// <summary>
/// Provides EditMode unit tests for the <see cref="GeneratorSlotUI"/> component.
/// EditMode tests run inside the Unity Editor without requiring scene loading,
/// making them ideal for validating pure C# logic, initialization behavior,
/// and data transformations.
/// </summary>
public class GeneratorSlotUIEditModeTests
{
    #region Fields
    
    private GameObject _root;
    private GeneratorSlotUI _slotUI;
    private Image _iconImage;

    #endregion
    #region SetupTeardown
    
    /// <summary>
    /// Creates a fresh <see cref="GameObject"/> and attaches the required
    /// <see cref="GeneratorSlotUI"/> and <see cref="Image"/> components
    /// before each test is executed.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _root = new GameObject("GeneratorSlotUI_TestObject");
        
        // Add the Image component first so it can be assigned to the UI Script
        _iconImage = _root.AddComponent<Image>();
        
        // Add the UI Script
        _slotUI = _root.AddComponent<GeneratorSlotUI>();
        
        // Use reflection to assign the private serialized field "iconImage"
        typeof(GeneratorSlotUI)
            .GetField("iconImage", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(_slotUI, _iconImage);
    }

    /// <summary>
    /// Cleans up the test GameObject after each test to ensure isolation
    /// and prevent cross-test contamination.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_root);
    }
    
    #endregion
    #region SetGeneratorSpriteTests
    // TODO: Awake = LifeCycle event -> PlayMode Test
    // /// <summary>
    // /// Validates that <see cref="GeneratorSlotUI.Awake"/> clears the assigned sprite
    // /// when the component initializes. This ensures the slot always begins in a
    // /// known empty state before any generator data is applied.
    // /// </summary>
    // [Test]
    // public void Awake_ClearsSprite()
    // {
    //     // Create root object
    //     var root = new GameObject("GeneratorSlotUI_TestObject");
    //     _slotUI = root.AddComponent<GeneratorSlotUI>();
    //     
    //     // Create child Icon object
    //     var iconGObj = new GameObject("Icon");
    //     iconGObj.transform.SetParent(root.transform);
    //     _iconImage = iconGObj.AddComponent<Image>();
    //     
    //     // Inject the private field via reflection
    //     typeof(GeneratorSlotUI)
    //         .GetField("iconImage", BindingFlags.NonPublic | BindingFlags.Instance)
    //         ?.SetValue(_slotUI, _iconImage);
    //     
    //     // Act - directly invoke Awake
    //     MethodInfo awakeMethod = typeof(GeneratorSlotUI)
    //         .GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
    //     awakeMethod!.Invoke(_slotUI, null);
    //     
    //     // Assert
    //     Assert.IsNull(_iconImage.sprite, "Awake should clear the iconImage sprite.");
    // }

    /// <summary>
    /// Ensures that calling <see cref="GeneratorSlotUI.SetGeneratorSprite"/> with a null
    /// argument clears the sprite reference, matching the intended behavior of hiding
    /// the icon when no generator is available.
    /// </summary>
    [Test]
    public void SetGeneratorSprite_Null_ClearsSprite()
    {
        // Assign a dummy sprite first
        _iconImage.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);
        
        _slotUI.SetGeneratorSprite(null);
        
        Assert.IsNull(_iconImage.sprite, "SetGeneratorSprite(null) should clear the iconImage sprite.");
    }

    /// <summary>
    /// Validates that providing a non-null sprite to
    /// <see cref="GeneratorSlotUI.SetGeneratorSprite"/> assigns the sprite and forces
    /// the icon alpha to 1. This ensures the icon is fully visible when a generator
    /// is present.
    /// </summary>
    [Test]
    public void SetGeneratorSprite_AssignsSprite_AndForcesAlpha()
    {
        // Create dummy sprite
        var sprite = Sprite.Create(Texture2D.redTexture, new Rect(0, 0, 1, 1), Vector2.zero);
        
        // Set icon alpha to something else to verify it gets overridden
        Color color = _iconImage.color;
        color.a = 0.25f;
        _iconImage.color = color;
        
        _slotUI.SetGeneratorSprite(sprite);
        
        Assert.AreEqual(sprite, _iconImage.sprite, "SetGeneratorSprite should assign the provided sprite.");
        Assert.AreEqual(1f, _iconImage.color.a, "SetGeneratorSprite should force the icon alpha to 1.");
    }
    
    #endregion
    #region HandleGeneratorChangedTests
    
    /// <summary>
    /// Validates that calling the private <c>HandleGeneratorChanged</c> method
    /// with a <c>null</c> prefab correctly resets the internal state of
    /// <see cref="GeneratorSlotUI"/>.  
    /// This ensures that:
    /// <list type="bullet">
    /// <item><description><see cref="GeneratorSlotUI.GetCurrentPrefab"/> returns <c>null</c></description></item>
    /// <item><description>The icon sprite is cleared via <see cref="GeneratorSlotUI.SetGeneratorSprite"/></description></item>
    /// </list>
    /// This test runs in EditMode because the logic is pure C# and does not
    /// require Unity lifecycle execution or scene-based UI behavior.
    /// </summary>
    [Test]
    public void HandleGeneratorChanged_Null_ClearsSprite_AndPrefab()
    {
        MethodInfo method = typeof(GeneratorSlotUI)
            .GetMethod("HandleGeneratorChanged", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(method, "HandleGeneratorChanged method not found via reflection.");
        method.Invoke(_slotUI, new object[] { null });

        Assert.IsNull(_slotUI.GetCurrentPrefab());
        Assert.IsNull(_iconImage.sprite);
    }

    /// <summary>
    /// Validates that invoking the private <c>HandleGeneratorChanged</c> method
    /// with a prefab containing a <see cref="SpriteRenderer"/> correctly updates
    /// the internal state of <see cref="GeneratorSlotUI"/>.  
    /// This test ensures that:
    /// <list type="bullet">
    /// <item><description>The provided prefab becomes the active value returned by <see cref="GeneratorSlotUI.GetCurrentPrefab"/></description></item>
    /// <item><description>The sprite extracted from the prefab's <see cref="SpriteRenderer"/> is applied to the UI icon</description></item>
    /// </list>
    /// This behavior is tested in EditMode because it involves pure logic and
    /// does not require Unity lifecycle execution or scene-based UI systems.
    /// </summary>
    [Test]
    public void HandleGeneratorChanged_AssignsPrefab_AndExtractsSprite()
    {
        // Create prefab with SpriteRenderer
        var prefab = new GameObject("Prefab");
        SpriteRenderer sr = prefab.AddComponent<SpriteRenderer>();
        sr.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);

        MethodInfo method = typeof(GeneratorSlotUI)
            .GetMethod("HandleGeneratorChanged", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method, "HandleGeneratorChanged method not found via reflection.");
        method.Invoke(_slotUI, new object[] { prefab });

        Assert.AreSame(prefab, _slotUI.GetCurrentPrefab());
        Assert.AreEqual(sr.sprite, _iconImage.sprite);
    }

    /// <summary>
    /// Ensures that calling the private <c>HandleGeneratorChanged</c> method
    /// with a prefab that does not contain a <see cref="SpriteRenderer"/>
    /// results in the UI icon being cleared.  
    /// This verifies that <see cref="GeneratorSlotUI"/> safely handles prefabs
    /// lacking visual data by assigning a <c>null</c> sprite rather than throwing
    /// errors or leaving stale UI state.
    /// </summary>
    [Test]
    public void HandleGeneratorChanged_NoSpriteRenderer_SetsNullSprite()
    {
        var prefab = new GameObject("Prefab_NoSpriteRenderer");

        MethodInfo method = typeof(GeneratorSlotUI)
            .GetMethod("HandleGeneratorChanged", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(method, "HandleGeneratorChanged method not found via reflection.");
        method.Invoke(_slotUI, new object[] { prefab });

        Assert.IsNull(_iconImage.sprite);
    }
    
    #endregion
}
