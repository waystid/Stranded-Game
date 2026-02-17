# Dev Console + Island Grid System - 2026-02-16

## Quick Reference

- **Date**: 2026-02-16
- **Type**: Feature Implementation
- **Status**: ✅ Complete
- **Branch**: feature/camera-controller
- **Session**: 4
- **Duration**: ~4 hours
- **Next Session**: Terraforming / Grid Placement System

---

## Objective

**Goal:** Build a toggleable in-game developer console for runtime parameter tuning, add a 3D grid overlay, and establish the island grid coordinate system (ACNH-style 45°-rotated 64×64 grid) as the foundation for future terraforming and object placement.

**Why:**
1. Needed rapid iteration on walk speed, camera zoom, curvature, day/night without leaving play mode
2. Grid system is the architectural prerequisite for terraforming, tree placement, and building
3. Camera at 45° yaw + Island at 45° rotation = grid lines appear horizontal/vertical on screen (ACNH look)

**Success Criteria:**
- [x] Backtick toggles dev console panel from anywhere in game
- [x] All runtime parameters adjustable without recompiling
- [x] Grid overlay drawn aligned to the 64×64 island in island local space
- [x] Trees and rocks at integer grid cell positions with 1×1 BoxColliders

---

## Implementation

### Phase 1: Dev Console Panel (Programmatic Canvas)

**Description:** Built entire UI in C# — no prefabs, no scene dependencies. Canvas persists across scene loads.

**Architecture:**
- `DevConsole.cs` creates Canvas (Screen Space Overlay, sortOrder=999, DontDestroyOnLoad) in `Awake()`
- Panel toggled by backtick key + persistent top-right corner button
- `CanvasGroup.blocksRaycasts` flips on show/hide (no alpha animation needed)
- `PlayerPrefs` stores remapped keycodes

**Sections:**
```
── PLAYER ──    WalkSpeed slider (1–20)
── CAMERA ──    Zoom Distance (4–35)
── DAY/NIGHT ── Enable toggle, Cycle Duration (10–600s), Time of Day (0–1)
── WORLD ──     Curvature (0–0.008), Show Grid toggle, Cell Size (1–16), Grid Alpha (0–1)
── KEY BINDINGS── Console / Sprint / Interact / Jump rebind buttons
                  [Reset Defaults]
```

**Parameter wiring (Start):**
| Slider | Target | Property |
|--------|--------|----------|
| Walk Speed | `FindObjectOfType<CharacterMovement>()` | `.WalkSpeed` |
| Camera Zoom | `FindObjectOfType<CinemachinePositionComposer>()` | `.CameraDistance` |
| Day Duration | `FindObjectOfType<DayNightCycle>()` | `.dayDurationSeconds` |
| Time of Day | `FindObjectOfType<DayNightCycle>()` | `.timeOfDay` |
| Curvature | `sharedMaterial.SetFloat("_Curvature")` | CurvedWorldGrass + CurvedWorldNature |

**Key rebinding:** Sprint/Interact/Jump hook into TDE's `InputManager.Instance.RunButtonDown()` etc. so remapped keys work alongside TDE's built-in axis bindings (OR semantics).

---

### Phase 2: Blank Panel Fix (3 Root Causes)

After first play test, the panel opened but content was invisible.

**Bug 1 — Text on same GO as Image:**
`AddFullButton()` called `go.AddComponent<Text>()` on the same GameObject as `Image`. Unity disallows multiple `Graphic` components on one object.
```
Fix: Replace AddComponent<Text> with MakeText(go, label) which creates a CHILD GameObject.
```

**Bug 2 — VerticalLayoutGroup.childControlHeight = false:**
Children had 0px height because VLG wasn't controlling child heights.
```
Fix: childControlHeight = true
```

**Bug 3 — Viewport Image.color = Color.clear:**
The `Mask` component uses the Image alpha to define the visible region. With alpha=0, everything inside was clipped out.
```
Fix: color = Color.white + showMaskGraphic = false
```

**Bug 4 — ContentSizeFitter not calculating in Awake:**
Layout dimensions weren't computed until the next frame, so the scroll view had zero height on first render.
```
Fix: Canvas.ForceUpdateCanvases() + LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect)
```

---

### Phase 3: Grid Overlay

**Description:** `GridOverlay.cs` draws a flat XZ grid using `GL.Lines` in `OnRenderObject()`.

**Key choices:**
- `[RequireComponent(typeof(Camera))]` — attaches to MainCamera
- `Hidden/Internal-Colored` shader with alpha blending enabled (always available in Unity)
- Center axes drawn in cyan, others in white
- `IslandRoot` field: when set, calls `GL.MultMatrix(IslandRoot.localToWorldMatrix)` so the grid draws in the island's rotated local space

**DevConsole wiring:**
```csharp
_grid = mainCam.GetComponent<GridOverlay>() ?? mainCam.gameObject.AddComponent<GridOverlay>();
_grid.enabled  = DEF_GRID_SHOW;   // false by default
_grid.CellSize = DEF_GRID_CELL;   // 1 unit = 1 grid cell
_grid.IslandRoot = _gridManager.IslandRoot;
```

---

### Phase 4: Island Grid System

**Design:** Camera has 45° yaw (NW view). Island parent rotated 45° around Y. Result: grid lines appear horizontal/vertical on screen — exactly the ACNH diamond perspective.

**Local ↔ World formula (Island Y=45°):**
```
world_x = (local_x + local_z) × 0.7071
world_z = (-local_x + local_z) × 0.7071

local_x = (world_x - world_z) × 0.7071
local_z = (world_x + world_z) × 0.7071
```

**Steps:**
1. Created `Island` empty GO at (0,0,0) with Y=45°
2. Added `IslandGridManager` component (see blocker below for naming)
3. Set `IslandRoot = Island.transform`
4. Parented IslandGround, Tree_01–08, Rock_01–04 under Island
5. Repositioned each tree/rock to integer local grid coordinates (within ±28 of center)
6. Replaced all MeshColliders with BoxColliders

**Final tree positions (island local space):**

| Object | Local (x, 0, z) | World (x, 0, z) |
|--------|-----------------|-----------------|
| Tree_01 | (-24, 0, -24) | (-33.94, 0, 0) |
| Tree_02 | (24, 0, -24) | (0, 0, -33.94) |
| Tree_03 | (24, 0, 4) | (19.80, 0, -14.14) |
| Tree_04 | (20, 0, 24) | (31.11, 0, 2.83) |
| Tree_05 | (0, 0, 28) | (19.80, 0, 19.80) |
| Tree_06 | (-24, 0, 18) | (-4.24, 0, 29.70) |
| Tree_07 | (-28, 0, -8) | (-25.46, 0, 14.14) |
| Tree_08 | (8, 0, -22) | (-9.90, 0, -21.21) |
| Rock_01 | (10, 0, 8) | (12.73, 0, -1.41) |
| Rock_02 | (-14, 0, 12) | (-1.41, 0, 18.38) |
| Rock_03 | (16, 0, -12) | (2.83, 0, -19.80) |
| Rock_04 | (-8, 0, -18) | (-18.38, 0, -7.07) |

**BoxCollider sizes:**
- Trees: size (1, 4, 1), center (0, 2, 0) — 1×1 footprint, 4 units tall
- Rocks: size (1, 1, 1), center (0, 0.5, 0) — 1×1×1 cube

---

## Technical Details

### Component Configuration

**IslandGridManager:**
```
Component: IslandGridManager
GameObject: Island
Properties:
  GridSize:   64          (cells: -32 to 31 on each axis)
  CellSize:   1.0         (1 world unit per cell in island-local space)
  IslandRoot: Island      (self-reference)
Public API:
  WorldToCell(Vector3) → Vector2Int
  CellToWorld(Vector2Int) → Vector3
  SnapToGrid(Vector3) → Vector3
  IsInBounds(Vector2Int) → bool
```

**GridOverlay:**
```
Component: GridOverlay
GameObject: Main Camera
Properties:
  WorldSize:  64f
  CellSize:   1f          (set by DevConsole)
  IslandRoot: Island      (set by DevConsole.Start)
  GridColor:  white/0.25a
  AxisColor:  cyan/0.6a
  YOffset:    0.05
```

### Scene Hierarchy

```
SandboxShowcase
├── Island  (Y=45°, IslandGridManager)
│   ├── IslandGround
│   ├── Tree_01 … Tree_08  (BoxCollider 1×4×1)
│   └── Rock_01 … Rock_04  (BoxCollider 1×1×1)
├── DevConsole  (DevConsole.cs)
├── 3DCameras
│   └── … Main Camera  (GridOverlay.cs)
└── …
```

---

## Blockers Encountered

### 1. Blank Dev Console Panel
**Problem:** Panel opened but all content was invisible.
**Investigation:** Narrowed to 4 independent root causes (see Phase 2 above).
**Resolution Time:** ~45 minutes
**Prevention:** Always use `MakeText(go, label)` child pattern for button labels; always set `childControlHeight = true` on VLGs; never use `Color.clear` on a `Mask` Image.

### 2. GridManager Name Conflict
**Problem:** `MoreMountains.TopDownEngine.GridManager` already exists. Adding a `GridManager` class caused ambiguous type errors in editor scripts.
**Solution:** Renamed custom class to `IslandGridManager`.
**Resolution Time:** ~5 minutes

### 3. MCP `position` Sets Local (Not World) for Parented Objects
**Problem:** `manage_gameobject modify position=[x,y,z]` sets **local** position when the object has a parent, despite documentation saying "world position".
**Solution:** Pass the desired integer local grid coordinates directly as the `position` value.
**Prevention:** When parenting objects under a rotated parent via MCP, always pass local coordinates to `position`.

---

## Lessons Learned

### What Worked Well

1. **Programmatic UI creation**
   - No prefab dependency, portable across scenes
   - Entire console in one file = easy to modify

2. **GL.Lines for grid overlay**
   - Zero draw calls overhead, no mesh required
   - `GL.MultMatrix(IslandRoot.localToWorldMatrix)` elegantly handles rotated island alignment

3. **Integer local grid positions**
   - Trees/rocks at clean integer coordinates (e.g. -24, 0, -24) = trivial future snapping
   - BoxColliders with explicit 1×1 footprint instead of mesh-derived = predictable physics

### What Didn't Work

1. **`Color.clear` on Mask viewport** — silently clips all content with no error
2. **Sending world positions to MCP for children** — tool uses local space despite docs

### Critical Steps

1. **`GL.MultMatrix` before GL.Begin** — must set matrix before drawing, not after
2. **`Canvas.ForceUpdateCanvases()`** — required after programmatic UI creation in `Awake()` or layout won't be computed until next frame
3. **Rename to avoid TDE conflicts** — always search for existing TDE types before naming custom classes

---

## Next Steps

### Immediate (Next Session)

**Topic:** Grid Placement / Terraforming Foundation

**Objective:** Allow player to interact with grid cells — hover highlight, selection, and basic terrain type assignment (ground/cliff/water).

**Approach:**
1. `GridCell` data structure (terrain type, occupant, height)
2. `GridCursor` — raycast from mouse/screen center → `IslandGridManager.WorldToCell()` → highlight hovered cell
3. Simple terrain paint mode (cycle through ground/cliff/water by pressing key)

### Future Sessions

**Short Term (Next 3 Sessions):**
1. **Cliff/Water Terrain** — Height variations, water shader
2. **Tree Planting** — Place/remove trees at empty cells
3. **Building Placement** — Multi-cell footprint support

**Medium Term:**
- NPC pathing respects grid terrain types
- Inventory integration for building materials
- Save/load grid state

---

## Files Modified

### New Files
- `Assets/Scripts/Environment/IslandGridManager.cs` — 64×64 grid singleton, WorldToCell/CellToWorld API
- `CosmicWiki/devlog/entries/2026-02-16-dev-console-island-grid.md` — this entry

### Modified Files
- `Assets/Scripts/UI/DevConsole.cs` — Added GridManager wiring, DEF_GRID_CELL=1, IslandGridManager field
- `Assets/Scripts/UI/GridOverlay.cs` — Added IslandRoot field + GL.MultMatrix alignment
- `Assets/Editor/SceneSetupAutomation.cs` — Updated GridManager → IslandGridManager API
- `Assets/Editor/SetupValidator.cs` — Updated GridManager → IslandGridManager API
- `Assets/Scenes/SandboxShowcase.unity` — Island GO, all objects reparented + repositioned

### Git Status
- Branch: feature/camera-controller
- Modified: 5 files
- New: 2 files
- Ready to commit: Yes

---

## Metrics

**Lines of Code:** ~800 (DevConsole.cs) + ~90 (GridOverlay.cs) + ~95 (IslandGridManager.cs)
**Features Completed:** 5 (console, blank fix, grid overlay, reset defaults, island grid)
**Blockers Resolved:** 4
**Scene Objects Modified:** 13 (Island, IslandGround, 8 trees, 4 rocks)

---

**Entry Created:** 2026-02-16
**Status:** ✅ Complete
**Ready for Next Session:** Yes — grid is architecturally correct for terraforming
