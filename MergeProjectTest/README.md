# MergeProjectTest

This is a learning project built with **Unity 6000.4.5f1**.  
The goal is to practice clean architecture, testing workflows, and modern Unity systems.  
The project is kept simple so new developers can follow along and learn step by step.

---

## Project Goals

- Use object pooling for mobile friendly performance  
- Use the Unity Test Framework for EditMode and PlayMode tests  
- Learn how to structure Unity scripts for long term growth  
- Practice writing clean and testable code  
- Understand how Unity’s new Input System works  
- Build confidence with tilemaps, placement logic, and UI updates

---

## Assembly Definitions and Test Visibility

Unity 6000 uses assembly definitions to control how scripts are compiled.  
Tests cannot see your game scripts unless you create an assembly definition for them.

### Why this matters

Unity compiles scripts into separate assemblies.  
Your PlayMode and EditMode tests run in their own assemblies.  
If your game scripts are not in an assembly definition, the test assemblies cannot reference them.  
This causes missing type errors when writing tests.

### How to set it up

1. Create an assembly definition file in the **Scripts** folder  
   - Right click the folder  
   - Select **Create > Assembly Definition**  
   - Name it `GameScripts`

2. Open your test assembly definition  
   - Add a reference to `GameScripts`  
   - This allows the tests to see all classes inside the Scripts folder

3. Save the project  
   - Unity recompiles the assemblies  
   - Your tests can now access your game code

This setup keeps your code organized and makes the test environment predictable.

---

## Project Structure

A simple and clear folder layout helps new developers understand where things belong.

- Assets
  - Scripts
    - InputManager.cs
    - BoardManager.cs
    - GeneratorManager.cs
    - GeneratorSlotUI.cs
    - Scripts.asmdef
  - Tests
    - EditMode
      - EditModeTests.asmdef
      - BoardItem_EditModeTests.cs
      - BoardManager_EditModeTests.cs
      - GeneratorManager_EditModeTests.cs
      - GeneratorSlotUI_EditModeTests.cs
      - InputManager_EditModeTests.cs
      - ObjectPoolManager_EditModeTests.cs
    - PlayMode
      - PlayModeTests.asmdef
      - BoardManager_PlayModeTests.cs
      - GeneratorManager_PlayModeTests.cs
      - GeneratorSlotUi_PlayModeTests.cs
      - InputManager_PlayModeTests.cs
      - ObjectPoolManager_PlayModeTests.cs
  - Prefabs
  - Sprites
  - Scenes
  - Audio
  - TilePalettes


### What each folder is for

- **Scripts**  
  All gameplay code. This folder has its own assembly definition so tests can reference it.

- **Tests**  
  Contains EditMode and PlayMode tests. Each test folder has its own assembly definition.

- **Prefabs**  
  Stores generator prefabs and other reusable objects.

- **Sprites**  
  Stores textures and icons used by the UI.

- **Scenes**  
  Stores Unity scenes used for gameplay.

- **Audio**
  Stores audio assets for used in gameplay.

- **TilePalettes**
  Stores tile palettes used in gameplay.

---

## How to Run Tests

Unity provides two test modes.

### EditMode tests

These run outside the scene.  
They are fast and good for pure logic.

To run them:

1. Open **Window > Test Runner**  
2. Select **EditMode**  
3. Press **Run All**

### PlayMode tests

These run inside a temporary scene.  
They are used for Unity specific behavior like cameras, tilemaps, and input.

To run them:

1. Open **Window > Test Runner**  
2. Select **PlayMode**  
3. Press **Run All**

### What our tests cover

- InputManager behavior with the new Input System  
- Tilemap setup and placement flow  
- UI updates through GeneratorSlotUI  
- Event subscription and timing  
- Safe handling of missing cameras or prefabs  
- Clean Setup and Teardown patterns

---

## Input System Overview

This project uses the **new Input System**.  
It replaces the old `UnityEngine.Input` API.

### Key ideas

- Input is read through devices like `Mouse.current`  
- Input actions can be used for more advanced setups  
- Input is event driven instead of polled every frame  
- Tests must simulate input carefully

### Example from the project

```csharp
if (Mouse.current?.leftButton.wasPressedThisFrame == true)
{
    Vector2 mousePos = Mouse.current.position.ReadValue();
    TryPlaceAtScreenPosition(mousePos);
}
```

This reads the mouse position only when the user clicks.
It keeps Update simple and avoids unnecessary work.

---

## Placement Flow

The placement system uses three main components.

### InputManager

- Reads input from the new Input System
- Converts screen positions to world positions
- Calls the board to place items
- Handles click placement and drag placement

### BoardManager

- Holds the tilemap
- Converts world positions to tile coordinates
- Validates placement
- Places items on the board

### GeneratorManager

- Tracks the active generator
- Raises events when the generator changes
- Updates UI elements like GeneratorSlotUI

### GeneratorSlotUI

- Subscribes to GeneratorManager events
- Updates the displayed icon
- Stores the current prefab for placement

This flow teaches how Unity systems communicate in a clean and testable way.

---

## Test Plan

This project uses the Unity Test Framework to validate gameplay logic and Unity specific behavior.
The goal is to teach how to write reliable tests that run in both EditMode and PlayMode

### Test Strategy

The project uses two types of tests

#### EditMode Tests

EditMode tests run outside the Unity scene.
They are used for fast checks of pure logic.

Examples:
- Validating helper methods
- Checking data structures
- Verifying object pooling logic that does not depend on Unity scene objects

#### PlayMode Tests

PlayMode tests run inside a temporary scene.
They are used for Unity specific behavior.

Examples:
- Camera usage
- Tilemap conversion
- Input handling with the new Input System
- UI updates through event subscriptions
- Placement logic that depends on world positions

### Test Environment Setup

Each PlayMode test suite creates only the objects it needs.
This includes:
- A camera
- A BoardManager with a Grid and Tilemap
- A GeneratorManager
- An InputManager

This setup keeps tests isolated and avoids cross test contamination.

### Teardown Strategy

Each suite destroys only the objects it owns.
Shared systems are left intact to avoid breaking unrelated tests.
This teaches students how to manage Unity's scene lifecycle during testing.

### Test Coverage Goals

The project aims to cover:
- InputManager click and drag placement behavior
- BoardManager tilemap conversion and placement validation
- GeneratorManger event dispatch
- GeneratorSlotUI event subscription and icon updates
- Safe handling of missing cameras or prefabs
- Clean Setup and Teardown patterns

### Future Test Improvements

- Add input simulation helpers for PlayMode tests
- Expand placement tests to verify tilemap behavior
- Add integration tests for pooling and placement
- Add UI interaction tests for generator selection

---

## Current Features
- InputManager rewritten for the new Input System
- BoardManager and Tilemap created during PlayMode tests
- Clean Setup and Teardown patterns for reliable test runs
- GeneratorSlotUI tests that verify event subscriptions and UI updates
- Placeholder tests for placement logic that will be expanded later
- Assembly definitions set up for clean test visibility

---

## Future Work
- Improve XML documentaiton across all scripts
- Add helper utilities for PlayMode input simulation
- Expand placement tests to verify tilemap behavior
- Add more examples for students learning Unity testing
- Add diagrams or flow charts for visual learners

