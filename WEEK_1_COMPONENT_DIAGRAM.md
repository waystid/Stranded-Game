# Week 1 Component Architecture Diagram

## Character Component Stack

### BEFORE (Loft3D Combat Character)
```
┌─────────────────────────────────────┐
│      LoftSuspenders Prefab          │
├─────────────────────────────────────┤
│ Core Components:                    │
│  └─ Character                       │
│  └─ TopDownController3D             │
│  └─ Health                          │
│  └─ Animator                        │
├─────────────────────────────────────┤
│ Movement System:                    │
│  └─ CharacterMovement               │ ← REMOVE
│      • Walk Speed: 8-10            │
│      • Acceleration: 50+ (instant) │
│      • Free analog movement        │
├─────────────────────────────────────┤
│ Rotation System:                    │
│  └─ CharacterOrientation3D          │
│      • RotationMode: Both          │
│      • MovementSpeed: 20 (instant) │
│      • WeaponSpeed: 20 (instant)   │
├─────────────────────────────────────┤
│ Combat System:                      │
│  └─ CharacterHandleWeapon           │ ← REMOVE
│      • Weapon aiming               │
│      • Firing control              │
│      • Reload mechanics            │
│      • Weapon switching            │
├─────────────────────────────────────┤
│ Other Abilities:                    │
│  └─ CharacterInventory              │ ← KEEP
│  └─ CharacterRun                    │ ← KEEP
│  └─ CharacterDash3D                 │ ← KEEP (optional)
│  └─ CharacterCrouch                 │ ← KEEP (optional)
└─────────────────────────────────────┘
```

### AFTER (Galactic Crossing Cozy Character)
```
┌─────────────────────────────────────┐
│      LoftSuspenders Prefab          │
│         (Modified)                  │
├─────────────────────────────────────┤
│ Core Components:                    │
│  └─ Character                       │
│  └─ TopDownController3D             │
│  └─ Health                          │
│  └─ Animator                        │
├─────────────────────────────────────┤
│ Movement System:                    │
│  └─ CharacterGridMovement           │ ← NEW
│      • MaximumSpeed: 5.5           │
│      • Acceleration: 15.0          │
│      • Grid-locked movement        │
│      • UseInputBuffer: TRUE        │
│      • BufferSize: 2               │
├─────────────────────────────────────┤
│ Rotation System:                    │
│  └─ CharacterOrientation3D          │ ← MODIFIED
│      • RotationMode: MovementDirection
│      • MovementRotationSpeed: Smooth
│      • RotateToFaceMovementSpeed: 8.0
│      • ShouldRotateToFaceWeapon: FALSE
├─────────────────────────────────────┤
│ Exploration System:                 │
│  └─ CharacterInventory              │
│  └─ CharacterRun                    │
│  └─ [Future: CharacterGather]      │
│  └─ [Future: CharacterCraft]       │
└─────────────────────────────────────┘
```

---

## Scene Architecture

### BEFORE (Loft3D Combat Scene)
```
Loft3D.unity
├─── Managers
│    ├─ LevelManager
│    ├─ GameManager
│    └─ InputManager
│
├─── Player
│    └─ LoftSuspenders
│        • Combat-ready
│        • Free movement
│
├─── Enemies (REMOVE ALL)
│    ├─ LoftPatrolAndShootAI
│    ├─ LoftSuitRagdollAI
│    ├─ PatrolSeekAndSwordAI
│    ├─ LoftSuitAI
│    └─ [8 more AI types...]
│
├─── Combat Items (REMOVE ALL)
│    ├─ LoftWeaponCrateSword
│    ├─ LoftAmmoCrateAssaultRifle
│    └─ [8 more weapon/ammo types...]
│
├─── Environment (KEEP)
│    ├─ Lighting
│    ├─ Walls
│    ├─ Floors
│    └─ Furniture
│         ├─ LoftBed
│         ├─ LoftTable
│         └─ [15+ furniture types...]
│
├─── UI (KEEP)
│    └─ Canvas
│         ├─ InventoryGUI
│         └─ HUD
│
└─── Camera (KEEP)
     └─ CM vcam1 (Cinemachine)
```

### AFTER (GalacticCrossingMVP Scene)
```
GalacticCrossingMVP.unity
├─── Managers
│    ├─ GridManager              ← NEW
│    │   • Grid Unit Size: 1.0
│    │   • Draw Debug Grid: TRUE
│    ├─ LevelManager
│    ├─ GameManager
│    └─ InputManager
│
├─── Player
│    └─ LoftSuspenders (Modified)
│        • Grid movement
│        • Non-combat
│
├─── Environment
│    ├─ Lighting
│    ├─ Walls
│    ├─ Floors
│    └─ Furniture
│         ├─ LoftBed
│         ├─ LoftTable
│         └─ [All preserved...]
│
├─── Resources (Week 2)
│    ├─ [Debris spawns]
│    ├─ [Crystal spawns]
│    └─ [LoftStimpack → AlienBerry]
│
├─── NPCs (Week 2)
│    ├─ [G.A.I.A. hologram]
│    └─ [Passive wildlife]
│
├─── UI
│    └─ Canvas
│         ├─ InventoryGUI
│         ├─ HUD
│         └─ [Quest tracker - Week 2]
│
└─── Camera
     └─ CM vcam1
          • Follow Offset: (0, 10, -7)
          • FOV: 30-40
          • [Curved world shader - Week 3]
```

---

## GridManager System Flow

```
┌──────────────────────────────────────────────────┐
│              GridManager (Singleton)              │
│                                                   │
│  Grid Origin: Transform at (0, 0, 0)             │
│  Grid Unit Size: 1.0                             │
│                                                   │
│  ┌─────────────────────────────────────────────┐ │
│  │  Data Structures                            │ │
│  │                                             │ │
│  │  OccupiedGridCells: List<Vector3>          │ │
│  │  [Contains positions of all occupied cells] │ │
│  │                                             │ │
│  │  LastPositions: Dict<GameObject, Vector3Int>│ │
│  │  [Tracks where each object was last]       │ │
│  │                                             │ │
│  │  NextPositions: Dict<GameObject, Vector3Int>│ │
│  │  [Tracks where each object is heading]     │ │
│  └─────────────────────────────────────────────┘ │
│                                                   │
│  ┌─────────────────────────────────────────────┐ │
│  │  Public API                                 │ │
│  │                                             │ │
│  │  WorldToCellCoordinates(worldPos)          │ │
│  │    → Returns Vector3Int grid coordinates   │ │
│  │                                             │ │
│  │  CellToWorldCoordinates(cellPos)           │ │
│  │    → Returns Vector3 world position        │ │
│  │                                             │ │
│  │  CellIsOccupied(cellPos)                   │ │
│  │    → Returns true/false                    │ │
│  │                                             │ │
│  │  OccupyCell(cellPos)                       │ │
│  │  FreeCell(cellPos)                         │ │
│  └─────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────┘
           │                    │
           ▼                    ▼
    ┌─────────────┐      ┌─────────────┐
    │  Character  │      │  Furniture  │
    │  (Player)   │      │  (Static)   │
    └─────────────┘      └─────────────┘
```

---

## CharacterGridMovement Behavior Flow

```
START
  │
  ▼
┌────────────────────────────┐
│  HandleInput()             │
│  • Read WASD/Controller    │
│  • Store _horizontalMovement
│  • Store _verticalMovement  │
└────────────┬───────────────┘
             │
             ▼
┌────────────────────────────┐
│  DetermineInputDirection() │
│  • Convert input to        │
│    GridDirections enum     │
│    (Up/Down/Left/Right)    │
│  • Store in _inputDirection│
└────────────┬───────────────┘
             │
             ▼
┌────────────────────────────┐
│  ProcessBuffer()           │
│  • Buffer input for        │
│    anticipatory turns      │
│  • Store in _bufferedDirection
└────────────┬───────────────┘
             │
             ▼
┌────────────────────────────┐
│  ApplyAcceleration()       │
│  • CurrentSpeed +=         │
│    Acceleration * deltaTime│
│  • Clamp to MaxSpeed       │
└────────────┬───────────────┘
             │
             ▼
┌────────────────────────────┐
│  HandleMovement()          │
│  ┌──────────────────────┐  │
│  │ If on perfect tile:  │  │
│  │  • Check buffered dir│  │
│  │  • Detect obstacles  │  │
│  │  • Calculate next    │  │
│  │    grid cell         │  │
│  │  • Occupy new cell   │  │
│  │  • Free old cell     │  │
│  └──────────────────────┘  │
│  ┌──────────────────────┐  │
│  │ If moving to tile:   │  │
│  │  • Move toward target│  │
│  │  • Check if arrived  │  │
│  └──────────────────────┘  │
└────────────┬───────────────┘
             │
             ▼
┌────────────────────────────┐
│  HandleState()             │
│  • Update animation state  │
│    (Walking vs Idle)       │
└────────────────────────────┘
```

---

## Movement State Machine

```
┌─────────────────────────────────────────────────┐
│           Character on Perfect Tile             │
│        (transform.position = grid center)       │
└──────────────────┬──────────────────────────────┘
                   │
                   │ Input detected
                   │ No obstacle ahead
                   ▼
┌─────────────────────────────────────────────────┐
│          Moving to Next Grid Unit               │
│   • _movingToNextGridUnit = true               │
│   • CurrentSpeed accelerating                  │
│   • Position lerping to _endWorldPosition      │
└──────────────────┬──────────────────────────────┘
                   │
                   │ Arrived at target
                   │ transform.position == _endWorldPosition
                   ▼
┌─────────────────────────────────────────────────┐
│           Character on Perfect Tile             │
│   • _movingToNextGridUnit = false              │
│   • Check buffered input                       │
│   • Repeat if input continues                  │
└─────────────────────────────────────────────────┘
```

### Special Cases

```
┌──────────────────────────────────────────┐
│  Input Buffer (UseInputBuffer = TRUE)   │
├──────────────────────────────────────────┤
│                                          │
│  Player presses RIGHT while moving UP:  │
│                                          │
│  1. UP movement continues               │
│  2. RIGHT stored in buffer              │
│  3. Upon reaching tile center:          │
│     → Check if RIGHT is obstacle-free   │
│     → If yes: Start moving RIGHT        │
│     → If no: Ignore buffer, stay idle   │
│                                          │
│  Buffer expires after 2 grid units      │
│  (BufferSize = 2)                       │
└──────────────────────────────────────────┘

┌──────────────────────────────────────────┐
│  Stop On Input Release (TRUE)            │
├──────────────────────────────────────────┤
│                                          │
│  Player releases movement stick:        │
│                                          │
│  1. Character continues to next tile    │
│  2. Stops at tile center                │
│  3. Does NOT continue in last direction │
│                                          │
│  (Animal Crossing behavior)             │
└──────────────────────────────────────────┘
```

---

## CharacterOrientation3D Rotation Logic

```
Every Frame:
  │
  ▼
┌────────────────────────────────────┐
│  Get CurrentDirection from         │
│  TopDownController                 │
│  (Vector3 of movement direction)   │
└──────────────┬─────────────────────┘
               │
               ▼
         ┌─────────────┐
         │ RotationMode│
         └──────┬──────┘
                │
      ┌─────────┴──────────┐
      ▼                    ▼
┌───────────────┐   ┌────────────────┐
│MovementDirection│   │WeaponDirection│
└────────┬──────┘   └───────┬────────┘
         │                   │
         │ (USED)            │ (DISABLED)
         ▼                   ▼
┌──────────────────┐  ┌──────────────┐
│RotationSpeed     │  │ Not used in  │
│                  │  │ cozy version │
│ Instant: Snap   │  └──────────────┘
│ Smooth: Lerp    │
│ SmoothAbsolute: │
│   Lerp+Memory   │
└────────┬─────────┘
         │
         │ (SMOOTH selected)
         ▼
┌────────────────────────────────────┐
│  Quaternion.Slerp(               │
│    currentRotation,              │
│    targetRotation,               │
│    Time.deltaTime * 8.0          │
│  )                               │
│                                  │
│  Effect: Smooth turn over        │
│  ~0.25 seconds for 180° rotation │
└────────────────────────────────────┘
```

---

## Grid Coordinate System

```
World Space (Unity 3D coordinates)
              Y (up)
              │
              │
              │
              └──────── X
             ╱
            ╱
           Z

Grid Space (Integer cell coordinates)

         Cell (-1, 1)  Cell (0, 1)  Cell (1, 1)
              │            │            │
         ─────┼────────────┼────────────┼─────
              │            │            │
         Cell (-1, 0)  Cell (0, 0)  Cell (1, 0)  ← Grid Origin
              │            │            │
         ─────┼────────────┼────────────┼─────
              │            │            │
         Cell (-1,-1)  Cell (0,-1)  Cell (1,-1)

Grid Unit Size = 1.0 means:
  • Each cell is 1.0 × 1.0 units
  • Character occupies center of cell
  • GridOrigin at (0, 0, 0) world space

Conversion:
  World (5.3, 0, 7.8) → Grid (5, 0, 7)
  Grid (3, 0, -2) → World (3.5, 0, -1.5)
                         (cell center)
```

---

## Debug Visualization

```
┌────────────────────────────────────────┐
│  GridManager Debug Grid                │
│  (When DrawDebugGrid = TRUE)           │
├────────────────────────────────────────┤
│                                        │
│  Scene View / Game View:               │
│                                        │
│    ┌─────┬─────┬─────┬─────┐          │
│    │  ░░ │     │  ░░ │     │          │
│    ├─────┼─────┼─────┼─────┤          │
│    │     │  ░░ │     │  ░░ │          │
│    ├─────┼─────┼─────┼─────┤          │
│    │  ░░ │     │  ░░ │     │          │
│    ├─────┼─────┼─────┼─────┤          │
│    │     │  ░░ │  ●  │  ░░ │          │
│    └─────┴─────┴─────┴─────┘          │
│                   ▲                    │
│                   │                    │
│              Character at              │
│              grid position             │
│                                        │
│  Grid lines: Cyan                      │
│  Cell fill: Cyan (alpha 0.3)           │
│  Checkerboard pattern for visibility   │
│                                        │
│  Only draws in Editor                  │
│  (OnDrawGizmos method)                 │
└────────────────────────────────────────┘
```

---

## Component Dependencies

```
                 ┌─────────────┐
                 │  Character  │
                 │  (Core)     │
                 └──────┬──────┘
                        │
        ┌───────────────┼───────────────┐
        │               │               │
        ▼               ▼               ▼
┌───────────────┐ ┌──────────┐ ┌────────────┐
│TopDownController│ │ Health  │ │ Animator  │
│     3D        │ │          │ │            │
└───────┬───────┘ └──────────┘ └────────────┘
        │
        ├─ Requires: Rigidbody
        ├─ Requires: Collider
        └─ Provides: CurrentDirection
                      CurrentMovement
                      Speed

┌──────────────────────────────────────┐
│  CharacterGridMovement               │
│  (Ability)                           │
├──────────────────────────────────────┤
│  Requires:                           │
│   ✓ TopDownController3D             │
│   ✓ GridManager (in scene)          │
│                                      │
│  Optional:                           │
│   ○ CharacterOrientation3D          │
│     (for rotation)                   │
│                                      │
│  Mutually Exclusive:                 │
│   ✗ CharacterMovement               │
│     (cannot coexist)                 │
└──────────────────────────────────────┘

┌──────────────────────────────────────┐
│  CharacterOrientation3D              │
│  (Ability)                           │
├──────────────────────────────────────┤
│  Requires:                           │
│   ✓ TopDownController3D OR          │
│   ✓ CharacterGridMovement           │
│                                      │
│  Optional:                           │
│   ○ CharacterHandleWeapon           │
│     (for weapon rotation - disabled) │
│                                      │
│  Provides:                           │
│   → Model rotation                  │
│   → Animation parameters            │
└──────────────────────────────────────┘
```

---

## Input Flow (New Input System)

```
┌────────────────────────────────────────┐
│  Unity Input System                    │
│  (New Input System Package)            │
└──────────────┬─────────────────────────┘
               │
               ▼
┌────────────────────────────────────────┐
│  Input Action Asset                    │
│  (PlayerControls)                      │
│                                        │
│  Actions:                              │
│   • Movement (Vector2)                 │
│   • Interact (Button)                  │
│   • Run (Button)                       │
│   • Inventory (Button)                 │
└──────────────┬─────────────────────────┘
               │
               ▼
┌────────────────────────────────────────┐
│  InputManager (TDE)                    │
│  • Processes Input Actions             │
│  • Sends to Player Character           │
└──────────────┬─────────────────────────┘
               │
               ▼
┌────────────────────────────────────────┐
│  Character.HandleInput()               │
│  • Distributes to all Abilities        │
└──────────────┬─────────────────────────┘
               │
               ▼
┌────────────────────────────────────────┐
│  CharacterGridMovement.HandleInput()   │
│  • Receives _horizontalInput          │
│  • Receives _verticalInput            │
│  • Stores for ProcessAbility()        │
└────────────────────────────────────────┘
```

---

## Ability Execution Order

```
Every Frame (Update):

1. HandleInput()
   └─ All abilities receive input

2. EarlyProcessAbility()
   └─ Pre-processing phase

3. ProcessAbility()          ← Main logic
   ├─ CharacterGridMovement
   │  ├─ DetermineInputDirection()
   │  ├─ ApplyAcceleration()
   │  ├─ HandleMovement()
   │  └─ HandleState()
   │
   └─ CharacterOrientation3D
      ├─ RotateToFaceMovementDirection()
      └─ RotateModel()

4. LateProcessAbility()
   └─ Post-processing phase

5. UpdateAnimator()
   └─ Send parameters to Animator
      ├─ Speed (from GridMovement)
      ├─ Walking (bool)
      └─ YRotationSpeed (from Orientation)
```

---

## File Reference Map

```
Project Root
│
├─ WEEK_1_IMPLEMENTATION_GUIDE.md     ← Full detailed guide
├─ WEEK_1_QUICK_REFERENCE.md          ← Quick checklist
├─ WEEK_1_TECHNICAL_ANALYSIS.md       ← Code analysis
└─ WEEK_1_COMPONENT_DIAGRAM.md        ← This file (visual reference)

Unity Project
│
├─ Assets/
│  ├─ Scenes/
│  │  └─ GalacticCrossingMVP.unity   ← Create this
│  │
│  └─ TopDownEngine/
│     ├─ Common/Scripts/
│     │  ├─ Managers/
│     │  │  └─ GridManager.cs        ← Read for understanding
│     │  │
│     │  └─ Characters/CharacterAbilities/
│     │     ├─ CharacterGridMovement.cs    ← Study movement logic
│     │     └─ CharacterOrientation3D.cs   ← Study rotation logic
│     │
│     └─ Demos/Loft3D/
│        ├─ Loft3D.unity              ← Source scene
│        │
│        └─ Prefabs/
│           ├─ PlayableCharacters/
│           │  └─ LoftSuspenders.prefab   ← Modify this
│           │
│           ├─ AI/                    ← Delete from scene
│           │  └─ [11 enemy prefabs]
│           │
│           └─ ItemPickers/           ← Delete weapons from scene
│              └─ [10 weapon/ammo prefabs]
```

---

**Diagram Version**: 1.0
**Date**: 2026-02-15
**Use**: Visual reference during Week 1 implementation
