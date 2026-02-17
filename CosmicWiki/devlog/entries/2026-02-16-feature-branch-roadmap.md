# Feature Branch Roadmap + GridCell Foundation - 2026-02-16

## Quick Reference

- **Date**: 2026-02-16
- **Type**: Milestone + Feature Implementation
- **Status**: ✅ Complete
- **Branch**: `features/001-world` (committed and pushed)
- **Session**: 6
- **Related Wiki**: [Branch Workflow](../guides/branch-workflow.md), [Terraforming System](../guides/terraforming-system.md)
- **Next Session**: TerrainPainter.cs + FloraPlacement.cs on `features/001-world`

---

## Objective

**Goal:** Establish the full 7-feature development roadmap with dedicated branches, wiki
infrastructure, and the foundational grid data layer for Feature 001.

**Why:**
1. Working foundation (camera, grid, console) is merged — time to scale into parallel features
2. Wiki agents/guides needed before each feature session to give Claude context immediately
3. GridCell + GridCursor are prerequisites for every world-interaction system (building, tools, NPCs)

**Success Criteria:**
- [x] feature/camera-controller merged to main via PR
- [x] `feature-base` umbrella branch + 7 sub-branches created and pushed
- [x] 4 wiki agent files written (synty-sidekick, pandazole, tde-ai, kevin-iglesias)
- [x] 7 feature guide stubs written
- [x] 10 wiki pages written (terrain-types, flora-catalog, 3 items, building-catalog, character-creator + 2 infra guides)
- [x] GridCell.cs compiles cleanly
- [x] GridCursor.cs compiles cleanly
- [x] IslandGridManager extended with GetCell/SetCell API

---

## Implementation

### Phase 1: Git Branch Structure

**Description:** Merge current work, create umbrella + 7 feature branches.

**Steps:**
1. Pushed `feature/camera-controller` (already clean)
2. Created PR #2 with full summary, merged to `main`
3. Pulled `main` locally, created `feature-base` umbrella branch
4. Created `features/001-world` through `features/007-character-creator` off `feature-base`

**Blocker encountered:**
- Git ref namespace conflict: cannot have both `features` branch and `features/NNN` sub-branch
- Git treats refs like a filesystem — `features` as a file conflicts with `features/` as a directory
- **Solution:** renamed umbrella to `feature-base`

---

### Phase 2: Wiki Infrastructure

**Description:** Created all agent files and guide stubs before any feature work begins.
This gives future sessions immediate context without re-reading code.

**New Files (CosmicWiki/agents/):**
- `synty-sidekick-agent.md` — SQLite DB schema, shader color property names (`_Color_Skin` etc.), mesh swap pattern, TDE ability checklist
- `pandazole-agent.md` — full catalog of all 6 packs: Nature, Food, Crafting, City, Farm, Dungeon
- `tde-ai-agent.md` — AIBrain/AIAction/AIDecision component table, villager state machine diagram, MMPath waypoint setup
- `kevin-iglesias-agent.md` — animation clip catalog with durations, animator controller swap pattern, animation event integration

**New Files (CosmicWiki/guides/):**
- `branch-workflow.md` — branch structure, merge strategy, dependency table
- `asset-integration-overview.md` — asset pack → feature map
- `terraforming-system.md`, `inventory-system.md`, `camera-system.md`, `npc-system.md`, `building-system.md`, `tool-system.md`, `synty-sidekick-integration.md`

**New Files (CosmicWiki/pages/):**
- `world/terrain-types.md`, `world/flora-catalog.md`
- `items/energy-crystal.md`, `items/ferrite-core.md`, `items/alien-berry.md`
- `buildings/building-catalog.md`, `character/character-creator.md`

---

### Phase 3: GridCell + GridCursor (Feature 001 Foundation)

**Description:** The grid data layer is a prerequisite for buildings (005), tools (006),
and villager NavMesh (004). Implemented on `features/001-world`.

#### GridCell.cs
```csharp
// Pure data class (not a MonoBehaviour)
public enum TerrainType { Flat, Cliff, Water, Beach, Radiation }

public class GridCell
{
    public TerrainType terrainType;
    public int heightLevel;         // 0 = ground, 1-3 = cliff tiers
    public GameObject occupant;     // tree/rock/building

    public bool IsOccupied => occupant != null;
    public bool IsWalkable  => terrainType != Cliff && terrainType != Water && !IsOccupied;
    public bool IsInteractable => terrainType != Radiation;
}
```

#### GridCursor.cs
- Raycasts from mouse or screen center against `groundLayer`
- Calls `IslandGridManager.Instance.WorldToCell(hitPoint)` → `CellToWorld(cell)`
- Moves a Quad highlight to snapped cell center
- Uses `MaterialPropertyBlock` to color quad: white=walkable, red=blocked
- Exposes `HoveredCell` (Vector2Int) and `HoveredWorldPos` for other systems

#### IslandGridManager.cs (extended)
- Added `Dictionary<Vector2Int, GridCell> _cells` (lazy init — default Flat)
- Added `GetCell(cell)`, `SetCell(cell, data)`, `SetTerrainType(cell, type)`
- No breaking changes to existing API

---

## Technical Details

### Grid Cell Color Feedback
```csharp
// GridCursor: uses MaterialPropertyBlock (not new material per frame)
var block = new MaterialPropertyBlock();
_quadRenderer.GetPropertyBlock(block);
block.SetColor(ColorPropID, isWalkable ? walkableColor : blockedColor);
_quadRenderer.SetPropertyBlock(block);
```

### Cell Registry (lazy)
```csharp
public GridCell GetCell(Vector2Int cell)
{
    if (_cells.TryGetValue(cell, out GridCell existing)) return existing;
    var newCell = new GridCell(TerrainType.Flat);
    _cells[cell] = newCell;
    return newCell;
}
```

---

## Results

### Compile Status
- `GridCell.cs`: ✅ 0 errors, 0 warnings
- `GridCursor.cs`: ✅ 0 errors, 0 warnings
- `IslandGridManager.cs`: ✅ 0 errors, 0 warnings
- Console: only pre-existing duplicate menu item warning (unrelated)

### Branch Status
```
main            ← stable (includes camera-controller merge)
feature-base    ← umbrella (same as main for now)
features/001-world  ← GridCell + GridCursor + wiki (ahead by 1 commit)
features/002-007    ← clean, same as feature-base
```

---

## Lessons Learned

### What Worked Well

1. **Wiki-first approach**
   - Writing agent/guide files before feature code gives every future session immediate context
   - Agents for `tde-ai` and `kevin-iglesias` will save hours of API re-discovery

2. **GridCell as pure data (not MonoBehaviour)**
   - No GameObject overhead for 64×64 = 4096 cells
   - Dictionary is sparse — only touched cells exist in memory

3. **MaterialPropertyBlock for highlight**
   - Single shared material + property block = zero per-frame allocations
   - Critical pattern for any per-cell visual feedback

### What Didn't Work

1. **`features` as umbrella branch name**
   - Git ref namespace conflict prevents `features` branch AND `features/NNN` sub-branches
   - **Rule:** umbrella branch must not be a prefix of any sub-branch name
   - Renamed to `feature-base` — this is the permanent solution

---

## Blockers Encountered

### Git Ref Namespace Conflict
**Problem:** After creating `features` branch, couldn't push `features/001-world`
**Error:** `fatal: cannot lock ref 'refs/heads/features/001-world': 'refs/heads/features' exists`
**Solution:** Delete `features`, rename umbrella to `feature-base`
**Resolution Time:** 5 min
**Prevention:** Documented in `MEMORY.md` and `branch-workflow.md`

---

## Next Steps

### Immediate (Next Session)

**Topic:** Feature 001-world — TerrainPainter + FloraPlacement

**Objective:** Let the player visually paint terrain types and place flora prefabs onto grid cells.

**Branch:** `git checkout features/001-world`

**Approach:**
1. Create `TerrainPainter.cs` — keyboard paint mode, writes `GridCell.terrainType`, swaps material on tile
2. Create `FloraPlacement.cs` — place Pandazole prefabs at `IslandGridManager.SnapToGrid()`, set `cell.occupant`
3. Add `GridCursor` component to a GameObject in SandboxShowcase scene
4. Wire GridCursor `groundLayer` to island ground layer
5. Test: cursor follows mouse, highlights walkable/blocked correctly
6. Test: press T (terrain) → paint mode, cell changes material
7. Test: press F (flora) → place Tree_Fall_03 at hovered cell

**Reference:**
- `CosmicWiki/guides/terraforming-system.md` — full spec
- `CosmicWiki/agents/pandazole-agent.md` — tree/flora prefab paths
- `Assets/Scripts/Environment/GridCell.cs` — TerrainType enum
- `Assets/Scripts/Environment/GridCursor.cs` — HoveredCell API

### Future Sessions

**Short Term (Next 3 Sessions):**
1. **001-world complete** — TerrainPainter, FloraPlacement, NavMesh integration
2. **007-character-creator start** — SidekickPlayer.prefab Phase A (mesh swap, TDE abilities)
3. **003-camera-control** — CameraController state machine, orbital snap rotation

**Medium Term:**
1. 002-items — inventory UI + pickup system
2. 005-buildings — BuildingPlacer with grid footprint
3. 004-villagers — AIBrain patrol + dialogue panel

---

## Related Entries

**Previous:** [World Expansion + Bug Fixes](./2026-02-17-world-expansion-bugfixes.md)
**Next:** TerrainPainter + FloraPlacement (next session)

**Related Wiki Pages:**
- [Branch Workflow](../guides/branch-workflow.md)
- [Terraforming System](../guides/terraforming-system.md)
- [Asset Integration Overview](../guides/asset-integration-overview.md)

---

## Files Modified

### New Files
- `Assets/Scripts/Environment/GridCell.cs` — cell data + TerrainType enum
- `Assets/Scripts/Environment/GridCursor.cs` — raycast highlight system
- `CosmicWiki/agents/synty-sidekick-agent.md`
- `CosmicWiki/agents/pandazole-agent.md`
- `CosmicWiki/agents/tde-ai-agent.md`
- `CosmicWiki/agents/kevin-iglesias-agent.md`
- `CosmicWiki/guides/branch-workflow.md`
- `CosmicWiki/guides/asset-integration-overview.md`
- `CosmicWiki/guides/terraforming-system.md` (+ 6 more guide stubs)
- `CosmicWiki/pages/world/terrain-types.md`, `flora-catalog.md`
- `CosmicWiki/pages/items/energy-crystal.md`, `ferrite-core.md`, `alien-berry.md`
- `CosmicWiki/pages/buildings/building-catalog.md`
- `CosmicWiki/pages/character/character-creator.md`

### Modified Files
- `Assets/Scripts/Environment/IslandGridManager.cs` — added `_cells` dict + GetCell/SetCell/SetTerrainType

### Git Status
- Branch: `features/001-world`
- Commit: `00ea6fe1`
- Remote: pushed and up to date

---

**Entry Created:** 2026-02-16
**Status:** ✅ Complete
**Ready for Next Session:** Yes — `features/001-world`, implement TerrainPainter + FloraPlacement
