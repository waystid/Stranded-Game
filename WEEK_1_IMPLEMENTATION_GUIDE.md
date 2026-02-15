# Week 1 Implementation Guide: Galactic Crossing MVP Foundation

## Foundation Agent Deliverable
**Status**: Complete - Manual Implementation Required in Unity Editor
**Date**: 2026-02-15

---

## Overview

This guide provides detailed, step-by-step instructions for implementing Week 1 of the Galactic Crossing MVP. Since Unity scene files and prefabs cannot be directly edited via scripts, all modifications must be performed manually in the Unity Editor. This document serves as your comprehensive checklist.

---

## Table of Contents

1. [Scene Setup](#1-scene-setup)
2. [GridManager Configuration](#2-gridmanager-configuration)
3. [Character Prefab Modification](#3-character-prefab-modification)
4. [Combat Element Removal](#4-combat-element-removal)
5. [Verification Checklist](#5-verification-checklist)

---

## 1. Scene Setup

### 1.1 Duplicate the Loft3D Scene

**Source Scene**: `/Assets/TopDownEngine/Demos/Loft3D/Loft3D.unity`
**Target Scene**: `/Assets/Scenes/GalacticCrossingMVP.unity`

**Steps**:

1. **Create the Scenes Directory** (if it doesn't exist):
   - In Unity Project window, navigate to `Assets/`
   - Right-click → Create → Folder
   - Name it: `Scenes`

2. **Duplicate Loft3D Scene**:
   - Navigate to `Assets/TopDownEngine/Demos/Loft3D/`
   - Select `Loft3D.unity`
   - Press `Ctrl+D` (Windows) or `Cmd+D` (Mac) to duplicate
   - Rename the duplicated scene to: `GalacticCrossingMVP`

3. **Move to Scenes Folder**:
   - Drag `GalacticCrossingMVP.unity` from `Loft3D/` to `Assets/Scenes/`

**Why Manual?**: Unity scene files (.unity) are binary/YAML hybrid files with complex internal references. Direct file copying breaks GameObject references and scene dependencies. Unity's internal duplication system maintains all these connections.

---

## 2. GridManager Configuration

### 2.1 Add GridManager to Scene

**Objective**: Enable grid-based movement for the character controller.

**Steps**:

1. **Open the Scene**:
   - Double-click `Assets/Scenes/GalacticCrossingMVP.unity`

2. **Create GridManager GameObject**:
   - In Hierarchy window, right-click → Create Empty
   - Rename to: `GridManager`
   - Position: (0, 0, 0) - This will be your grid origin

3. **Add GridManager Component**:
   - With `GridManager` selected in Hierarchy
   - In Inspector, click `Add Component`
   - Search for: `Grid Manager`
   - Select: `MoreMountains.TopDownEngine.GridManager`

4. **Configure GridManager Properties**:

   **Grid Settings**:
   - **Grid Origin**: Drag the `GridManager` GameObject into this field (self-reference)
   - **Grid Unit Size**: `1.0`
     - *Reasoning*: 1-unit cells match the character scale and provide precise movement control for the "cozy" feel

   **Debug Settings**:
   - **Draw Debug Grid**: `TRUE` (checked)
   - **Debug Draw Mode**: `ThreeD`
   - **Debug Grid Size**: `30` (default)
   - **Cell Border Color**: Cyan (default is fine)
   - **Inner Color**: Cyan with alpha 0.3 (default is fine)

**Reference Example**: See `Assets/TopDownEngine/Demos/Minimal3D/MinimalGrid3D.unity` for a working grid setup.

**Technical Note**: The GridManager is a singleton that manages cell occupation, preventing characters from overlapping on the same grid cell. It's essential for turn-based or grid-locked movement.

---

## 3. Character Prefab Modification

### 3.1 Locate LoftSuspenders Prefab

**Source Prefab**: `/Assets/TopDownEngine/Demos/Loft3D/Prefabs/PlayableCharacters/LoftSuspenders.prefab`

**Steps**:

1. **Open Prefab for Editing**:
   - Navigate to `Assets/TopDownEngine/Demos/Loft3D/Prefabs/PlayableCharacters/`
   - Double-click `LoftSuspenders.prefab` to enter Prefab Mode

### 3.2 Remove CharacterHandleWeapon Ability

**Objective**: Eliminate all combat functionality to enforce non-violent gameplay.

**Steps**:

1. **Locate the Component**:
   - In Prefab Mode, select the root `LoftSuspenders` GameObject
   - In Inspector, scroll through components to find: `Character Handle Weapon`

2. **Remove Component**:
   - Click the three-dot menu (⋮) next to `Character Handle Weapon`
   - Select `Remove Component`
   - Confirm the removal

**What This Does**: Removes weapon aiming, firing, reloading, and weapon switching capabilities. The character can no longer use the weapon system.

### 3.3 Replace CharacterMovement with CharacterGridMovement

**Objective**: Convert from free analog movement to grid-locked, turn-based style movement.

**Steps**:

#### A. Remove CharacterMovement

1. **Find Component**:
   - In Inspector (still in Prefab Mode)
   - Locate: `Character Movement`

2. **Remove It**:
   - Three-dot menu (⋮) → `Remove Component`
   - **IMPORTANT**: Note any custom settings before removing (unlikely in Loft3D demo)

#### B. Add CharacterGridMovement

1. **Add Component**:
   - Click `Add Component`
   - Search for: `Character Grid Movement`
   - Select: `MoreMountains.TopDownEngine.CharacterGridMovement`

2. **Configure Movement Settings**:

   **Movement Parameters**:
   - **Maximum Speed**: `5.5`
     - *From GDD*: Creates relaxed pacing, slower than default TDE (8-10)
   - **Acceleration**: `15.0`
     - *From GDD*: Introduces 0.3s ramp-up time for deliberate, weighted feel
   - **Current Speed**: `0` (this is read-only, leave as-is)
   - **Maximum Speed Multiplier**: `1.0` (default)
   - **Acceleration Multiplier**: `1.0` (default)

   **Input Settings**:
   - **Input Mode**: `Input Manager`
   - **Use Input Buffer**: `TRUE` (checked)
   - **Buffer Size**: `2`
     - *From GDD*: Allows player to queue directional inputs for responsive grid turns
   - **Fast Direction Changes**: `TRUE` (allows U-turns)
   - **Idle Threshold**: `0.05` (default, or adjust for sensitivity)
   - **Normalized Input**: `FALSE`
   - **Stop On Input Release**: `TRUE`

   **Grid Settings**:
   - **Obstacle Detection Offset**: `(0, 0.5, 0)` (default for 3D)

**Reference Files**:
- **Component Script**: `/Assets/TopDownEngine/Common/Scripts/Characters/CharacterAbilities/CharacterGridMovement.cs`
- **Example Scene**: `MinimalGrid3D.unity` demonstrates grid movement in action

### 3.4 Configure CharacterOrientation3D

**Objective**: Control how the character model rotates to face movement direction with weighted, smooth turning.

**Steps**:

1. **Locate Component**:
   - In Inspector (Prefab Mode)
   - Find: `Character Orientation 3D`
   - If it doesn't exist, add it: `Add Component` → `Character Orientation 3D`

2. **Configure Rotation Settings**:

   **Rotation Mode**:
   - **Rotation Mode**: `Movement Direction`
     - *This makes the character face the direction they're moving, not the weapon*
   - **Character Rotation Authorized**: `TRUE`

   **Movement Direction Settings**:
   - **Should Rotate To Face Movement Direction**: `TRUE` (checked)
   - **Movement Rotation Speed**: `Smooth`
     - *Options: Instant, Smooth, SmoothAbsolute*
   - **Movement Rotating Model**: Leave empty to auto-assign the character model
   - **Rotate To Face Movement Direction Speed**: `8.0`
     - *From GDD*: Lower than default TDE (20+) creates weighted turning animation
   - **Absolute Threshold Movement**: `0.5` (only used if SmoothAbsolute mode)

   **Weapon Direction Settings** (Disable since we removed weapons):
   - **Should Rotate To Face Weapon Direction**: `FALSE` (unchecked)

**Reference Files**:
- **Component Script**: `/Assets/TopDownEngine/Common/Scripts/Characters/CharacterAbilities/CharacterOrientation3D.cs`

### 3.5 Save Prefab Changes

**Steps**:
1. Press `Ctrl+S` (Windows) or `Cmd+S` (Mac) to save
2. Exit Prefab Mode by clicking the left arrow `←` in the Hierarchy breadcrumb

---

## 4. Combat Element Removal

### 4.1 AI Enemies to Remove

**Location**: Throughout the `GalacticCrossingMVP` scene hierarchy

**Enemy Prefabs Used in Loft3D** (Remove all instances):
- `LoftPatrolAndShootAI`
- `LoftSuitRagdollAI`
- `PatrolSeekAndSwordAI`
- `LoftSuitAI`
- `LoftTieMouseDrivenPathfinderAI`
- `Loft3DBlueCube` (hostile cube AI)
- `LoftPatrolAndPathfinderAI`
- `LoftPatrolSeekAndDestroyAI`
- `LoftPatrolSeekAndDestroyReloadingAI`
- `LoftPatrolConeSeekAndDestroyAI`
- `LoftTieAI`

**Prefab Paths**: `/Assets/TopDownEngine/Demos/Loft3D/Prefabs/AI/`

**Removal Steps**:
1. Open `GalacticCrossingMVP.unity`
2. In Hierarchy, search for each AI name above (use search box)
3. Select each found GameObject
4. Press `Delete` or right-click → `Delete`
5. Repeat for all AI enemies

**Alternative Approach**:
- Create a parent GameObject called `_ToDelete`
- Drag all AI enemies under it
- Delete the parent (faster bulk deletion)

### 4.2 Weapon Pickups to Remove

**Item Picker Prefabs** (Remove all instances):
- `LoftWeaponCrateSword`
- `LoftAmmoCrateAssaultRifle`
- `LoftAmmoCrateHandgun`
- `LoftWeaponCrateAssaultRifle`
- `LoftWeaponCrateShotgun`
- `LoftWeaponCrateSniperRifle`
- `LoftAmmoCrateShotgun`
- `LoftAmmoCrateSniperRifle`
- `LoftWeaponCrateHandgun`

**Prefab Paths**: `/Assets/TopDownEngine/Demos/Loft3D/Prefabs/ItemPickers/`

**Removal Steps**:
1. Search Hierarchy for "Weapon" and "Ammo"
2. Delete all found weapon/ammo crates
3. Verify by searching "Crate" to catch any remaining combat pickups

**Keep These Item Pickers**:
- `LoftStimpack` - Will be repurposed as "Alien Berry" health consumable

### 4.3 Combat Props to Remove

**Explosive Objects**:
- `LoftBlueExplosive1x1` (any explosive barrels/props)
- Search for "Explosive" in Hierarchy and delete

**What to KEEP**:

**Environment & Furniture**:
- All furniture prefabs (beds, chairs, tables, desks, sofas, etc.)
  - Location: `Prefabs/Props/LoftFurniture/`
  - Examples: `LoftBed`, `LoftBarChair`, `LoftTable`, `LoftMicrowave`, `LoftKitchenCabinetCorner`, etc.

**Lighting**:
- All light sources (Directional Light, Point Lights, Area Lights)
- Keep entire lighting setup for atmosphere

**Inventory/UI Systems**:
- Inventory Engine components
- UI Canvases
- Camera system (we'll modify this later for the "Rolling Log" effect)

**Level Geometry**:
- Walls, floors, ceilings
- Doors (we'll repurpose these for hab module entrances)
- Pathways and ground planes

### 4.4 Scene Organization After Cleanup

**Recommended Hierarchy Structure**:
```
GalacticCrossingMVP
├── Managers
│   ├── GridManager
│   ├── LevelManager
│   └── GameManager
├── Player
│   └── LoftSuspenders (modified)
├── Environment
│   ├── Lighting
│   ├── Walls
│   ├── Floors
│   └── Props (Furniture)
├── UI
│   └── Canvas (Inventory, HUD)
└── Cameras
    └── CM vcam1
```

**Organization Steps**:
1. Create empty GameObjects for each category (Managers, Environment, etc.)
2. Drag related objects under appropriate parents
3. This keeps the Hierarchy clean and manageable

---

## 5. Verification Checklist

### 5.1 GridManager Verification

- [ ] GridManager GameObject exists in scene
- [ ] GridManager component attached with correct settings:
  - [ ] Grid Unit Size = 1.0
  - [ ] Draw Debug Grid = TRUE
  - [ ] Debug Draw Mode = ThreeD
- [ ] Grid origin is at (0, 0, 0) or your chosen starting point
- [ ] When you enter Play Mode, you see a cyan grid overlay on the ground

### 5.2 Character Prefab Verification

Open `LoftSuspenders.prefab` and verify:

- [ ] **CharacterHandleWeapon component is REMOVED**
- [ ] **CharacterMovement component is REMOVED**
- [ ] **CharacterGridMovement component EXISTS with settings**:
  - [ ] Maximum Speed = 5.5
  - [ ] Acceleration = 15.0
  - [ ] Use Input Buffer = TRUE
  - [ ] Buffer Size = 2
- [ ] **CharacterOrientation3D component configured**:
  - [ ] Rotation Mode = Movement Direction
  - [ ] Rotate To Face Movement Direction Speed = 8.0
  - [ ] Should Rotate To Face Weapon Direction = FALSE

### 5.3 Scene Cleanup Verification

In `GalacticCrossingMVP.unity`:

- [ ] **No AI enemies present** (search "AI" in Hierarchy)
- [ ] **No weapon pickups** (search "Weapon", "Ammo", "Crate")
- [ ] **No explosive props** (search "Explosive")
- [ ] **Furniture still present** (check Environment/Props)
- [ ] **Lighting intact** (scene should still be lit)
- [ ] **Inventory UI functional** (Canvas objects present)
- [ ] **Camera system exists** (Cinemachine vcam)

### 5.4 Playtest Verification

**Enter Play Mode and Test**:

1. **Grid Movement**:
   - [ ] Character moves in grid-locked squares (not free analog)
   - [ ] Movement feels weighted, not instant
   - [ ] Character takes ~0.3s to accelerate to full speed
   - [ ] Character drifts slightly when you release movement input
   - [ ] Cyan debug grid is visible on the ground

2. **Rotation**:
   - [ ] Character smoothly rotates to face movement direction
   - [ ] Rotation is not instant (takes a few frames)
   - [ ] Character does not rotate toward weapon aim (weapon system disabled)

3. **Interaction**:
   - [ ] Character can walk through scene without weapon prompts
   - [ ] No weapon UI appears
   - [ ] Inventory still accessible (if present in original scene)

4. **Environment**:
   - [ ] No enemies spawn or exist
   - [ ] No weapon crates in view
   - [ ] Furniture and decorative elements remain
   - [ ] Scene feels peaceful and non-threatening

---

## Technical Reference

### Component Locations

All referenced components are part of the TopDown Engine:

- **GridManager**:
  - Script: `/Assets/TopDownEngine/Common/Scripts/Managers/GridManager.cs`
  - Namespace: `MoreMountains.TopDownEngine`

- **CharacterGridMovement**:
  - Script: `/Assets/TopDownEngine/Common/Scripts/Characters/CharacterAbilities/CharacterGridMovement.cs`
  - Namespace: `MoreMountains.TopDownEngine`

- **CharacterOrientation3D**:
  - Script: `/Assets/TopDownEngine/Common/Scripts/Characters/CharacterAbilities/CharacterOrientation3D.cs`
  - Namespace: `MoreMountains.TopDownEngine`

### Example Scenes for Reference

- **MinimalGrid3D.unity**:
  - Path: `/Assets/TopDownEngine/Demos/Minimal3D/MinimalGrid3D.unity`
  - Shows: Basic 3D grid movement setup

- **Explodudes.unity**:
  - Path: `/Assets/TopDownEngine/Demos/Explodudes/Explodudes.unity`
  - Shows: Grid movement with bombs (can reference GridManager setup)

### Key Settings Summary (From GDD)

| Setting | Value | Reasoning |
|---------|-------|-----------|
| Walk Speed | 5.5 | Relaxed pacing encourages exploration |
| Acceleration | 15.0 | Creates 0.3s ramp-up for weighted feel |
| Grid Unit Size | 1.0 | Matches character scale for precise control |
| Rotation Speed | 8.0 | Smooth turning arcs (vs. instant 20+) |
| Input Buffer Size | 2 | Allows queuing 2 directional inputs ahead |

---

## Next Steps (Week 2 Preview)

After completing Week 1, you'll have:
- A non-combat scene with grid-based movement
- A modified character controller optimized for "cozy" gameplay
- Clean environment ready for resource placement

**Week 2 will focus on**:
- Creating resource gathering system (debris, crystals)
- Implementing G.A.I.A. NPC dialogue system
- Building the "Hab Kit" placement mechanic
- Quest tracking UI for gathering tasks

---

## Troubleshooting

### Issue: Grid not visible in Scene view

**Solution**:
- Make sure GridManager's `DrawDebugGrid` is TRUE
- Check that you're in Play Mode (debug grid only draws at runtime)
- Verify Scene view has Gizmos enabled (top-right Gizmos button)

### Issue: Character moves freely instead of grid-locked

**Solution**:
- Ensure CharacterMovement component was fully removed
- Confirm CharacterGridMovement is present and enabled
- Check that GridManager exists in the scene
- Verify Grid Unit Size is not 0

### Issue: Character doesn't rotate smoothly

**Solution**:
- Check CharacterOrientation3D is present
- Verify Rotation Mode = Movement Direction
- Confirm Rotation Speed is set to 8.0 (not 0)
- Check that Movement Rotating Model references the character model

### Issue: Prefab changes not appearing in scene

**Solution**:
- Make sure you saved the prefab (Ctrl/Cmd+S)
- If using a prefab instance, check for overrides (blue lines in Inspector)
- Consider "Revert" prefab instance to match prefab, then re-apply scene-specific changes

---

## File Paths Reference

**Scenes**:
- Source: `/Assets/TopDownEngine/Demos/Loft3D/Loft3D.unity`
- Target: `/Assets/Scenes/GalacticCrossingMVP.unity`

**Prefabs**:
- Character: `/Assets/TopDownEngine/Demos/Loft3D/Prefabs/PlayableCharacters/LoftSuspenders.prefab`
- AI Enemies: `/Assets/TopDownEngine/Demos/Loft3D/Prefabs/AI/`
- Weapon Pickups: `/Assets/TopDownEngine/Demos/Loft3D/Prefabs/ItemPickers/`
- Furniture: `/Assets/TopDownEngine/Demos/Loft3D/Prefabs/Props/LoftFurniture/`

**Scripts**:
- GridManager: `/Assets/TopDownEngine/Common/Scripts/Managers/GridManager.cs`
- CharacterGridMovement: `/Assets/TopDownEngine/Common/Scripts/Characters/CharacterAbilities/CharacterGridMovement.cs`
- CharacterOrientation3D: `/Assets/TopDownEngine/Common/Scripts/Characters/CharacterAbilities/CharacterOrientation3D.cs`

---

## End of Week 1 Implementation Guide

**Completion Criteria**: All checklist items verified, playtest successful, ready for Week 2 resource gathering implementation.

**Estimated Time**: 2-3 hours for manual Unity Editor work

**Questions or Issues?**: Refer to TopDown Engine documentation at https://topdown-engine-docs.moremountains.com/
