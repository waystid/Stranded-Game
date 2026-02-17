# TerrainPainter + FloraPlacement - 2026-02-16

## Quick Reference

- **Date**: 2026-02-16
- **Type**: Feature Implementation
- **Status**: ✅ Complete
- **Branch**: `features/001-world` (committed and pushed)
- **Session**: 7
- **Related Wiki**: [Terraforming System](../guides/terraforming-system.md), [Pandazole Agent](../agents/pandazole-agent.md)
- **Next Session**: NavMesh rebake integration OR Feature 007-character-creator

---

## Objective

**Goal:** Implement TerrainPainter.cs and FloraPlacement.cs — the two interactive tools
that let the designer visually paint terrain types and place/remove Pandazole flora prefabs
on the island grid.

**Success Criteria:**
- [x] TerrainPainter.cs compiles cleanly
- [x] FloraPlacement.cs compiles cleanly
- [x] GridTools GameObject wired in SandboxShowcase (GridCursor + both tools)
- [x] GridCursor groundLayer = layer 9 (IslandGround)
- [x] FloraPlacement starter prefabs assigned (Tree_03_Fall, Tree_01_Fall, Bush_01, Bush_05)
- [x] Scene saved + committed + pushed

---

## Implementation

### TerrainPainter.cs

**Key Design Decisions:**

1. **No per-cell tile meshes** — IslandGround is a single mesh, so "swapping tile material"
   is implemented as **persistent colored quad overlays** (same MaterialPropertyBlock pattern
   as GridCursor). Zero per-frame allocations.

2. **Overlay pool** — `Dictionary<Vector2Int, GameObject>` stores overlay quads.
   Painting Flat hides (deactivates) the quad; painting any other type reactivates/creates it.

3. **Controls:**
   - T = toggle paint mode on/off
   - 1 = Flat (transparent/removes overlay)
   - 2 = Cliff (dark gray)
   - 3 = Water (blue)
   - 4 = Beach (sandy yellow)
   - 5 = Radiation (green)
   - LMB = paint selected terrain
   - RMB = erase to Flat
   - Escape = exit paint mode

4. **Data write:** `IslandGridManager.SetTerrainType(cell, type)` — no breaking changes.

### FloraPlacement.cs

**Key Design Decisions:**

1. **Inspector-assignable prefab array** — `GameObject[] floraPrefabs`. Assigned in scene:
   `Tree_03_Fall`, `Tree_01_Fall`, `Bush_01`, `Bush_05` as defaults.
   Any Pandazole prefab can be added without code changes.

2. **Auto-finds Island** — `GameObject.Find("Island")` fallback if floraRoot not set.

3. **Controls:**
   - F = toggle place mode on/off
   - 1–9 = select prefab by index
   - [ / ] = cycle through prefabs
   - LMB = place flora (walkable + unoccupied cells only)
   - RMB = remove flora (Destroy occupant, ClearOccupant)
   - Escape = exit place mode

4. **Occupant registration:** Sets `cell.occupant = placed` on placement.
   Calls `cell.ClearOccupant()` on removal. GridCursor automatically shows
   red highlight for occupied cells.

### SandboxShowcase Wiring

- **GridTools** GameObject created under `Managers`
- Components: `GridCursor` + `TerrainPainter` + `FloraPlacement`
- `GridCursor.groundLayer` = 512 (layer 9 — IslandGround)
- `FloraPlacement.floraRoot` = Island transform (instanceID -1050442)
- Flora prefabs wired (4 starter prefabs)

---

## Technical Details

### Actual Pandazole Tree Prefab Path

The wiki agent had incorrect paths (`Tree_Fall_03`). Actual naming convention is:
```
Assets/Pandazole_Ultimate_Pack/Pandazole Nature Environment Pack/Prefabs/Tree_03_Fall.prefab
```
Pattern: `Tree_{01-15}_{Fall|Spring|Winter}.prefab` — note number comes before season.

### TerrainPainter overlay rendering order

```
Ground mesh (default):  renderQueue ~2000
GridCursor highlight:   renderQueue 3000
TerrainPainter overlay: renderQueue 3001  ← sits above cursor quad
```

---

## Results

### Compile Status
- `TerrainPainter.cs`: ✅ 0 errors, 0 warnings
- `FloraPlacement.cs`: ✅ 0 errors, 0 warnings
- Console: only pre-existing duplicate menu item warning (unrelated)

### File Changes
- **New**: `Assets/Scripts/Environment/TerrainPainter.cs`
- **New**: `Assets/Scripts/Environment/FloraPlacement.cs`
- **Modified**: `Assets/Scenes/SandboxShowcase.unity` (GridTools GO + wiring)
- **Modified**: `CosmicWiki/guides/terraforming-system.md` (status checkboxes)

---

## Next Steps

### Immediate Options

**Option A: NavMesh rebake integration**
- TerrainPainter already writes correct `terrainType` to grid data
- Next: call `NavMeshSurface.BuildNavMesh()` after any paint operation
- Mark Cliff/Water cells as non-walkable in NavMesh areas

**Option B: Feature 007-character-creator**
- SidekickPlayer.prefab Phase A (mesh swap, TDE abilities)
- Independent of terraforming — can start on `features/007-character-creator`

### Short Term
1. TerrainPainter: add undo stack (Ctrl+Z reverts last paint operation)
2. FloraPlacement: show ghost/preview of selected prefab at hovered cell
3. Both tools: HUD overlay showing active mode + selected item

---

**Entry Created:** 2026-02-16
**Status:** ✅ Complete
**Commit:** `00d6fc35`
