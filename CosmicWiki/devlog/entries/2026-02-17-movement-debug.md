# Devlog: Movement Debug Sprint
**Date:** 2026-02-17 (afternoon)
**Branch:** `features/003-npc-shop`

---

## What We Tried

Two full sessions attempting to fix character movement after the world cleanup sprint.
The character spawns correctly, all systems initialize cleanly, but WASD input produces no visible movement.

### Session 1 — InputManager + Cliff Tiles + Water

**Changes made:**
- Added `InputManager` GO to Managers group (PlayerID=Player1, InputForcedMode=Desktop)
- Fixed a YAML serialization bug: MCP `set_property` stores strings with wrapping quotes (`PlayerID: '"Player1"'`); corrected by directly editing scene file → `PlayerID: Player1`
- Set `InitialSpawnPoint` world Y = 0.5 (was at Y=0, below tile surface)
- Swapped `IslandConfiguration.asset` preset from `BaseBlockPresetGreen` → `CliffTilesPreset_A` (GUID: `e6decb8e...`) — island now renders with green tops + brown/sandy cliff sides ✅
- Added `WaterPlane` Quad (60×60, Y=-0.5, layer=Water) with `matRiverWater.mat` — animated caustics/ripples ✅
- Tuned water `_Color_Surface` to ocean blue (0.1, 0.55, 0.85) ✅

**Result:** CliffTiles and water work. Character still can't move.

### Session 2 — Camera-Relative Input + Center Spawn

**Investigation:**

Deep-dived TDE InputManager source. Confirmed:
- `LinkedInputManager` IS correctly set on the spawned character in play mode
- `PlayerID: Player1` correct
- `InputForcedMode = Desktop (1)` correct
- `AutoMobileDetection = false`
- All CharacterAbilities enabled, AbilityPermitted=1
- ConditionState = Normal (0)
- No runtime errors

Found that `Player1_Horizontal` / `Player1_Vertical` axes are mapped to A/D and S/W in `ProjectSettings/InputManager.asset`.

**Root cause hypothesis:** WASD moves in world-space axes (W=+Z, S=-Z). Camera is at fixed 45° yaw. Movement IS happening but in a direction that looks like nothing from the user's perspective (character moves at 45° toward/away from camera, not screen-forward).

**Changes made:**
- `ACNHCameraFollow.Start()` now calls `inputManager.SetCamera(_cam, true)` — registers the camera with TDE's InputManager so `RotateInputBasedOnCameraDirection` uses the camera's yaw=45°
- `InputManager.RotateInputBasedOnCameraDirection = true` — WASD now rotates 45° before being applied to movement (W = screen-up = world NE direction)
- `InitialSpawnPoint` moved from world (0, 0.5, 0) to world (20, 2, 0) — island center area (TWC local ~cell 14,14)

**Result:** Character still not moving after these changes.

---

## What We Discovered

### Critical Finding: TWC `colliderType: 0` = No Tile Physics

`Assets/Data/IslandConfiguration.asset` has `colliderType: 0` (None). This means the generated CliffTile meshes are **visual-only** — they have no physics colliders. The character doesn't land on the cliff tiles; it falls through all of them and lands on the `GroundCollider` (a 120×120 BoxCollider at world Y=0).

The character is standing at world Y≈0, visually embedded inside cliff geometry that sits at Y=0+. The user sees the character appearing stuck inside the cliff mesh with no apparent response to input — even if movement IS working, it's happening invisibly inside geometry.

**This is the highest-confidence fix for next session: `colliderType: 0` → `1`.**

### Secondary: Game View Focus

Unity Editor does not forward keyboard input to a playing game unless the **Game View window is clicked/focused** after pressing Play. We cannot verify this via MCP but it's worth testing immediately next session.

### Possible: SetCamera Registration Timing

`ACNHCameraFollow.Start()` calls `FindFirstObjectByType<InputManager>()`. If there's a Script Execution Order conflict, this could return null and the camera-relative rotation wouldn't activate. Need to verify `_targetCamera` is non-null in play mode.

---

## Current State

```
✅ CliffTile island: green tops + brown/sandy cliffs
✅ WaterPlane: animated river-style shader surrounding island
✅ InputManager: Player1, Desktop, no auto-mobile
✅ RotateInputBasedOnCameraDirection = true
✅ ACNHCameraFollow registers with InputManager.SetCamera()
✅ Spawn at world (20, 2, 0) = island center
❌ TWC tile colliders disabled (colliderType=0) — must fix
❌ Character movement unconfirmed
```

---

## Git Commits This Session

```
cf72c48d Fix character movement: camera-relative input + center spawn
c4a3bd8d Fix character movement + add cliff tiles and water plane
```

---

## Next Session Plan

See `docs/NEXT_SESSION_START.md` for detailed action plan.

**TL;DR:**
1. Click Game View after Play (test focus first)
2. Set `colliderType: 1` in IslandConfiguration.asset
3. Adjust spawn Y after tile colliders work
4. Verify `_targetCamera` is set on InputManager
5. If still broken: change `CharacterOrientation3D.RotationMode` 3→1
