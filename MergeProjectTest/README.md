# MergeProjectTest

Demo project built with **Unity 6000.4.5f1**  
This project is used for learning and experimentation. It focuses on clean architecture, test driven workflows, and Unity’s modern systems.

## Project Goals

- Use object pooling for mobile friendly performance  
- Use the Unity Test Framework for EditMode and PlayMode tests  
- Practice clean separation between game code and test code  
- Learn how to structure Unity projects for long term growth 🙂

## Assembly Definitions and Test Visibility

Unity 6000 uses assembly definitions to control how scripts are compiled.  
Tests cannot see your game scripts unless you create an assembly definition for them.

### Why this matters

Unity compiles scripts into separate assemblies.  
Your PlayMode and EditMode tests run in their own assemblies.  
If your game scripts are not in an assembly definition, the test assemblies cannot reference them.  
This causes missing type errors when writing tests.

### How to fix it

1. Create an assembly definition file in the **Scripts** folder  
   - Right click the folder  
   - Select **Create > Assembly Definition**  
   - Name it something like `GameScripts`

2. Open your test assembly definition  
   - Add a reference to `GameScripts`  
   - This allows the tests to see all classes inside the Scripts folder

3. Save the project  
   - Unity recompiles the assemblies  
   - Your tests can now access your game code

This setup keeps your code organized and makes the test environment predictable.

## Testing Approach

The project uses both EditMode and PlayMode tests.

- **EditMode tests** run fast and check logic that does not need the Unity scene  
- **PlayMode tests** run inside a temporary scene and check Unity specific behavior  
  such as cameras, tilemaps, input, and object placement

Each test suite creates only the objects it needs.  
This keeps tests isolated and prevents cross test contamination.

## Current Features

- InputManager rewritten for the new Input System  
- BoardManager and Tilemap created during PlayMode tests  
- Clean Setup and Teardown patterns for reliable test runs  
- GeneratorSlotUI tests that verify event subscriptions and UI updates  
- Placeholder tests for placement logic that will be expanded later

## Future Work

- Improve XML documentation across all scripts  
- Add helper utilities for PlayMode input simulation  
- Expand placement tests to verify tilemap behavior  
- Add more examples for students learning Unity testing
