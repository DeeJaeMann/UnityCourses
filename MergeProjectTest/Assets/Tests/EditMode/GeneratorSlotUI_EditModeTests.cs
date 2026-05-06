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
    private GameObject _testObject;
    private GeneratorSlotUI _slotUI;
    private Image _iconImage;

    /// <summary>
    /// Creates a fresh <see cref="GameObject"/> and attaches the required
    /// <see cref="GeneratorSlotUI"/> and <see cref="Image"/> components
    /// before each test is executed.
    /// </summary>
    [SetUp]
    public void Setup()
    {
        _testObject = new GameObject("GeneratorSlotUI_TestObject");
        
        // Add the Image component first so it can be assigned to the UI Script
        _iconImage = _testObject.AddComponent<Image>();
        
        // Add the UI Script
        _slotUI = _testObject.AddComponent<GeneratorSlotUI>();
        
        // Use reflection to assign the private serialized field "iconImage"
        typeof(GeneratorSlotUI)
            .GetField("iconImage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.SetValue(_slotUI, _iconImage);
    }

    /// <summary>
    /// Cleans up the test GameObject after each test to ensure isolation
    /// and prevent cross-test contamination.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(_testObject);
    }

    /// <summary>
    /// Validates that <see cref="GeneratorSlotUI.Awake"/> clears the assigned sprite
    /// when the component initializes. This ensures the slot always begins in a
    /// known empty state before any generator data is applied.
    /// </summary>
    [Test]
    public void Awake_ClearsSprite()
    {
        // Create root object
        var root = new GameObject("GeneratorSlotUI_TestObject");
        _slotUI = root.AddComponent<GeneratorSlotUI>();
        
        // Create child Icon object
        var iconGObj = new GameObject("Icon");
        iconGObj.transform.SetParent(root.transform);
        _iconImage = iconGObj.AddComponent<Image>();
        
        // Inject the private field via reflection
        typeof(GeneratorSlotUI)
            .GetField("iconImage", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(_slotUI, _iconImage);
        
        // Act - directly invoke Awake
        MethodInfo awakeMethod = typeof(GeneratorSlotUI)
            .GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        awakeMethod!.Invoke(_slotUI, null);
        
        // Assert
        Assert.IsNull(_iconImage.sprite, "Awake should clear the iconImage sprite.");
    }

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
}
