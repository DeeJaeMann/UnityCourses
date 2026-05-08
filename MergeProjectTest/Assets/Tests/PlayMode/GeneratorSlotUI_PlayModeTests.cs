using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// PlayMode test suite for <see cref="GeneratorSlotUI"/>.
/// Validates UI behavior, sprite assignment, event handling,
/// and safe forwarding of click/drag events.
/// </summary>
public class GeneratorSlotUI_PlayModeTests
{
    #region Fields

    private GameObject _slotRootObject;
    private GeneratorSlotUI _slotUI;
    private Image _iconImage;
    private GeneratorManager _generatorManager;

    #endregion


    #region Setup / Teardown

    /// <summary>
    /// Creates a minimal UI environment including:
    /// <list type="bullet">
    /// <item><description>A <see cref="Canvas"/> for UI rendering</description></item>
    /// <item><description>An <see cref="EventSystem"/> for UI events</description></item>
    /// <item><description>A <see cref="GeneratorManager"/> instance</description></item>
    /// <item><description>A <see cref="GeneratorSlotUI"/> with an injected icon image</description></item>
    /// </list>
    /// </summary>
    [SetUp]
    public void Setup()
    {
        // Canvas
        var canvasObject = new GameObject("Canvas", typeof(Canvas));
        canvasObject.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;

        // EventSystem
        var eventSystemObject = new GameObject("EventSystem", typeof(EventSystem));
        eventSystemObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

        // GeneratorManager
        var managerObject = new GameObject("GeneratorManager");
        _generatorManager = managerObject.AddComponent<GeneratorManager>();

        // Slot root
        _slotRootObject = new GameObject("GeneratorSlotUI");
        _slotRootObject.SetActive(false);

        // Icon child
        var iconObject = new GameObject("Icon");
        iconObject.transform.SetParent(_slotRootObject.transform);

        _iconImage = iconObject.AddComponent<Image>();
        _slotUI = _slotRootObject.AddComponent<GeneratorSlotUI>();

        // Inject serialized field
        typeof(GeneratorSlotUI)
            .GetField("iconImage", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(_slotUI, _iconImage);

        _slotRootObject.SetActive(true);
    }

    /// <summary>
    /// Cleans up all scene objects created during <see cref="Setup"/>.
    /// </summary>
    [TearDown]
    public void Teardown()
    {
        if (_slotRootObject != null)
            Object.Destroy(_slotRootObject);

        if (_generatorManager != null)
            Object.Destroy(_generatorManager.gameObject);

        var canvas = GameObject.Find("Canvas");
        if (canvas != null)
            Object.Destroy(canvas);

        var eventSystem = GameObject.Find("EventSystem");
        if (eventSystem != null)
            Object.Destroy(eventSystem);
    }

    #endregion


    #region Awake Tests

    /// <summary>
    /// Ensures <see cref="GeneratorSlotUI.Awake"/> clears the icon sprite
    /// so the slot always begins in a known empty state.
    /// </summary>
    [UnityTest]
    public IEnumerator Awake_ClearsSprite()
    {
        // Arrange
        _iconImage.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);

        Object.DestroyImmediate(_slotUI);
        _slotRootObject.SetActive(false);

        _slotUI = _slotRootObject.AddComponent<GeneratorSlotUI>();
        typeof(GeneratorSlotUI)
            .GetField("iconImage", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(_slotUI, _iconImage);

        // Act
        _slotRootObject.SetActive(true);
        yield return null;

        // Assert
        Assert.IsNull(_iconImage.sprite, "Awake() should clear the icon sprite.");
    }

    #endregion


    #region UI Update Tests

    /// <summary>
    /// Ensures that when a generator prefab with a <see cref="SpriteRenderer"/>
    /// is reported to the slot, the UI updates accordingly.
    /// </summary>
    [UnityTest]
    public IEnumerator HandleGeneratorChanged_UpdatesUI_WithSprite()
    {
        // Arrange
        var prefab = new GameObject("Prefab");
        var spriteRenderer = prefab.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), Vector2.zero);

        // Act
        typeof(GeneratorSlotUI)
            .GetMethod("HandleGeneratorChanged", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.Invoke(_slotUI, new object[] { prefab });

        yield return null;

        // Assert
        Assert.AreSame(prefab, _slotUI.GetCurrentPrefab(),
            "Slot should store the current prefab when the generator changes.");
        Assert.AreEqual(spriteRenderer.sprite, _iconImage.sprite,
            "Slot icon should match the prefab's SpriteRenderer sprite.");
    }

    /// <summary>
    /// Ensures that when a generator prefab lacks a <see cref="SpriteRenderer"/>,
    /// the slot clears its icon.
    /// </summary>
    [UnityTest]
    public IEnumerator HandleGeneratorChanged_NoSpriteRenderer_ResultsInNullSprite()
    {
        // Arrange
        var prefab = new GameObject("Prefab_NoSpriteRenderer");

        // Act
        _generatorManager.AddGenerator(prefab);
        yield return null;

        // Assert
        Assert.IsNull(_iconImage.sprite,
            "Slot icon should be cleared when the prefab has no SpriteRenderer.");
    }

    #endregion


    #region Destruction Tests

    /// <summary>
    /// Ensures the slot unsubscribes from <see cref="GeneratorManager.OnGeneratorChanged"/>
    /// when destroyed.
    /// </summary>
    [UnityTest]
    public IEnumerator OnDestroy_UnsubscribesFromGeneratorManager()
    {
        // Arrange
        var prefab = new GameObject("Prefab");
        _generatorManager.AddGenerator(prefab);
        yield return null;

        // Act
        Object.Destroy(_slotUI);
        yield return null;

        _generatorManager.AddGenerator(prefab);
        yield return null;

        // Assert
        Assert.IsNull(_iconImage.sprite,
            "Slot should not update its icon after being destroyed.");
    }

    #endregion


    #region Click Tests

    /// <summary>
    /// Ensures clicking the slot forwards the correct prefab to the <see cref="InputManager"/>.
    /// </summary>
    [UnityTest]
    public IEnumerator OnSlotClicked_InvokesInputManager()
    {
        // Arrange
        var inputManagerObject = new GameObject("InputManager");
        var inputManager = inputManagerObject.AddComponent<InputManager>();

        typeof(GeneratorSlotUI)
            .GetField("inputManager", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(_slotUI, inputManager);

        var prefab = new GameObject("Prefab");

        typeof(GeneratorSlotUI)
            .GetMethod("HandleGeneratorChanged", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.Invoke(_slotUI, new object[] { prefab });

        // Act
        _slotUI.OnSlotClicked();
        yield return null;

        // Assert
        Assert.AreSame(prefab, inputManager.ActivePrefab,
            "OnSlotClicked should forward the current prefab to the InputManager.");
    }

    #endregion


    #region Drag Tests

    /// <summary>
    /// Ensures beginning a drag operation forwards the correct prefab to the <see cref="InputManager"/>.
    /// </summary>
    [UnityTest]
    public IEnumerator OnBeginDrag_InvokesInputManager()
    {
        // Arrange
        var inputManagerObject = new GameObject("InputManager");
        var inputManager = inputManagerObject.AddComponent<InputManager>();

        typeof(GeneratorSlotUI)
            .GetField("inputManager", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(_slotUI, inputManager);

        var prefab = new GameObject("Prefab");

        typeof(GeneratorSlotUI)
            .GetMethod("HandleGeneratorChanged", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.Invoke(_slotUI, new object[] { prefab });

        var eventData = new PointerEventData(EventSystem.current);

        // Act
        _slotUI.OnBeginDrag(eventData);
        yield return null;

        // Assert
        Assert.AreSame(prefab, inputManager.ActivePrefab,
            "OnBeginDrag should forward the current prefab to the InputManager.");
    }

    /// <summary>
    /// Ensures dragging over the slot does not throw and safely forwards drag events.
    /// </summary>
    [UnityTest]
    public IEnumerator OnDrag_UpdatesDragPosition()
    {
        // Arrange
        var inputManagerObject = new GameObject("InputManager");
        var inputManager = inputManagerObject.AddComponent<InputManager>();

        typeof(GeneratorSlotUI)
            .GetField("inputManager", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(_slotUI, inputManager);

        var eventData = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(123, 456)
        };

        // Act + Assert
        Assert.DoesNotThrow(() => _slotUI.OnDrag(eventData),
            "OnDrag should safely forward drag events.");
        yield return null;
    }

    /// <summary>
    /// Ensures ending a drag operation safely forwards the event to the <see cref="InputManager"/>.
    /// </summary>
    [UnityTest]
    public IEnumerator OnEndDrag_InvokesInputManager()
    {
        // Arrange
        var inputManagerObject = new GameObject("InputManager");
        var inputManager = inputManagerObject.AddComponent<InputManager>();

        typeof(GeneratorSlotUI)
            .GetField("inputManager", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(_slotUI, inputManager);

        var prefab = new GameObject("Prefab");

        typeof(GeneratorSlotUI)
            .GetMethod("HandleGeneratorChanged", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.Invoke(_slotUI, new object[] { prefab });

        inputManager.BeginDragPlacement(prefab);

        var eventData = new PointerEventData(EventSystem.current)
        {
            position = new Vector2(200, 300)
        };

        // Act + Assert
        Assert.DoesNotThrow(() => _slotUI.OnEndDrag(eventData),
            "OnEndDrag should safely forward drag-end events.");
        yield return null;
    }

    #endregion


    #region No-InputManager Safety Tests

    /// <summary>
    /// Ensures all drag-related methods safely no-op when no <see cref="InputManager"/> is assigned.
    /// </summary>
    [UnityTest]
    public IEnumerator DragMethods_NoInputManager_DoNothing_PlayMode()
    {
        // Arrange
        var testRoot = new GameObject("SlotUI_TestRoot");
        var eventSystem = EventSystem.current ?? new GameObject("EventSystem").AddComponent<EventSystem>();

        var slotUI = testRoot.AddComponent<GeneratorSlotUI>();
        var iconImage = testRoot.AddComponent<Image>();

        typeof(GeneratorSlotUI)
            .GetField("iconImage", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.SetValue(slotUI, iconImage);

        var eventData = new PointerEventData(eventSystem);

        yield return null;

        // Act + Assert
        Assert.DoesNotThrow(() => slotUI.OnBeginDrag(eventData),
            "OnBeginDrag should not throw when no InputManager is assigned.");
        Assert.DoesNotThrow(() => slotUI.OnDrag(eventData),
            "OnDrag should not throw when no InputManager is assigned.");
        Assert.DoesNotThrow(() => slotUI.OnEndDrag(eventData),
            "OnEndDrag should not throw when no InputManager is assigned.");
    }

    #endregion
}
