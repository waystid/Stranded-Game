# Next Session: Fix Character Movement

**Date:** Next session after 2026-02-17 (Movement Debug Sprint)
**Branch:** `features/003-npc-shop`
**Previous Session:** Two attempts at movement fix — InputManager added, camera-relative input wired, spawn moved to center. Character still not moving.

---

## Current State

```
✅ InputManager in scene (Player1, Desktop, RotateInputBasedOnCameraDirection=true)
✅ LinkedInputManager confirmed set in play mode (no null)
✅ No runtime errors on Play
✅ Spawn moved to world (20,2,0) = island center area
✅ CliffTiles rendering green tops + brown sides
✅ WaterPlane surrounding island
❌ Character cannot move (WASD unresponsive)
❌ TWC tile colliders are DISABLED (colliderType=0) — character falls into cliff geometry
```

---

## Root Cause Analysis (Most Likely Culprits, in Order)

### #1 — TWC `colliderType: 0` (Highest Confidence)

`Assets/Data/IslandConfiguration.asset` line 38: `colliderType: 0`

In TileWorldCreator v4, `colliderType: 0 = None`. This means the generated cliff tiles have **no physics colliders**. The character falls through all tile geometry and lands on the flat `GroundCollider` (120×120 BoxCollider at world Y=0, layer=Ground). The character is then visually buried inside cliff meshes that sit above Y=0. Movement may be working but invisible because the character is inside geometry.

**Fix:** Change `colliderType` to `1` (Box) in IslandConfiguration, then adjust spawn Y so the character lands on the cliff tile tops.

How to find the correct value: open the CliffIsland demo config and check what colliderType it uses:
```
Assets/TileWorldCreator/_Samples/CliffIsland URP/CliffIslandConfiguration.asset
```

### #2 — Game View Focus (Simple but Often Missed)

Unity Editor does NOT forward keyboard input to the game unless the **Game View window is focused**. After clicking Play, the user must click inside the Game View panel before WASD registers.

**Test:** Press Play → click once inside the Game View → then try WASD.

### #3 — SetCamera() / RotateInputBasedOnCameraDirection Not Effective

`ACNHCameraFollow.Start()` was updated to call `inputManager.SetCamera(_cam, true)`, which sets `_targetCamera` on the InputManager. TDE's `ApplyCameraRotation()` then reads `_targetCamera.transform.localEulerAngles.y` = 45° and rotates input.

**Potential failure mode:** `FindFirstObjectByType<InputManager>()` in `ACNHCameraFollow.Start()` might return null if InputManager hasn't been instantiated yet (unlikely but possible due to Script Execution Order).

**Verify in play mode:** Read InputManager component → check `_targetCamera` field is not null.

If null: change `ACNHCameraFollow` to use `LateStart()` (coroutine with `yield return null`) or add a retry in `LateUpdate()` (similar to how Target is retried).

### #4 — CharacterOrientation3D Mouse Aim (RotationMode 3)

`HumanCustomPlayer.prefab`: `CharacterOrientation3D.RotationMode = 3` (LookAtMouseOrController). This performs a raycast from mouse position to find the character's facing direction. If the raycast uses a wrong layer mask and misses, the orientation ability may throw a silent exception that interrupts `ProcessAbility()` chain.

**Check:** Look at `CharacterOrientation3D` source → what happens when `LookAtMouseOrController` raycast fails?

**Quick fix if this is the issue:** Change RotationMode to `1` (RotateToFaceMovementDirection) which doesn't need a raycast.

### #5 — Spawn Height After Collider Fix

Once `colliderType` is fixed and tile colliders exist, spawn at world Y=2 will cause the character to fall. Cliff tile tops will be at some positive Y (likely Y=0.5 or Y=1). Need to confirm the cliff tile mesh height and set spawn Y = tile_top + 0.5.

---

## Action Plan

### Step 0: Game View Focus Test (2 minutes)
1. Press Play
2. Click inside Game View window
3. Press WASD
→ If movement works: the issue was just focus. Skip to Step 3.

### Step 1: Enable TWC Tile Colliders

Check the CliffIsland demo for the correct colliderType value:
```
grep -n "colliderType" "Assets/TileWorldCreator/_Samples/CliffIsland URP/CliffIslandConfiguration.asset"
```

Edit `Assets/Data/IslandConfiguration.asset` line 38:
```yaml
colliderType: 0   # → change to 1 (Box) or 2 (Mesh)
```

Also check `tileColliderHeight` and `tileColliderExtrusionHeight` — may need non-zero values.

### Step 2: Adjust Spawn Y for Tile Surface

After enabling colliders, enter play mode and check where the character lands. Read the spawned character's world Y position via MCP. Set spawn Y = character_landed_Y + 0.1.

As a starting point: spawn at world (20, 3, 0) — generous height, character drops onto tiles.

### Step 3: Verify Camera-Relative Input

Enter play mode and read InputManager component. Check:
- `RotateInputBasedOnCameraDirection: true` ✓ (already saved)
- `_targetCamera`: should be the Main Camera

If `_targetCamera` is null, change ACNHCameraFollow to call SetCamera in a coroutine:
```csharp
IEnumerator RegisterCameraDelayed() {
    yield return null;  // wait one frame for InputManager.Awake()
    var im = FindFirstObjectByType<InputManager>();
    if (im != null) im.SetCamera(_cam, true);
}
// call from Start()
StartCoroutine(RegisterCameraDelayed());
```

### Step 4: Fix CharacterOrientation3D if Needed

If movement still doesn't work after Steps 1-3, check `CharacterOrientation3D`:
1. Enter play mode → check console for any silent exceptions
2. In `HumanCustomPlayer.prefab`, change `RotationMode` from `3` to `1` (RotateToFaceMovementDirection)
3. Re-test movement

### Step 5: Fallback — Test with TDE Demo Character

If HumanCustomPlayer still won't move, add `LoftSuspenders.prefab` from `Assets/TopDownEngine/Demos/Loft3D/Prefabs/PlayableCharacters/` to the scene and see if it responds to WASD. If it moves, compare its Character/InputManager configuration to HumanCustomPlayer.

---

## File Checklist for This Fix

| File | Change |
|------|--------|
| `Assets/Data/IslandConfiguration.asset` | `colliderType: 0` → `1` (enable tile physics) |
| `Assets/Scenes/SandboxShowcase.unity` | Adjust InitialSpawnPoint Y after tile collider test |
| `Assets/Scripts/Camera/ACNHCameraFollow.cs` | If SetCamera() not registering: add coroutine delay |
| `Assets/Prefabs/HumanCustomPlayer.prefab` | If Step 4 needed: CharacterOrientation3D.RotationMode 3→1 |

---

## Start Command

```
"Read NEXT_SESSION_START.md. Fix character movement:
Step 0 = confirm Game View focus.
Step 1 = enable TWC tile colliders (colliderType in IslandConfiguration.asset).
Step 2 = adjust spawn Y.
Step 3 = verify SetCamera is registered.
Step 4 = if still broken, change RotationMode 3→1 on CharacterOrientation3D."
```

---

## Reference

- **IslandConfiguration:** `Assets/Data/IslandConfiguration.asset`
- **CliffIsland demo config:** `Assets/TileWorldCreator/_Samples/CliffIsland URP/CliffIslandConfiguration.asset`
- **HumanCustomPlayer:** `Assets/Prefabs/HumanCustomPlayer.prefab`
- **ACNHCameraFollow:** `Assets/Scripts/Camera/ACNHCameraFollow.cs`
- **InputManager (scene):** Managers group → InputManager GO → MoreMountains.TopDownEngine.InputManager
- **CharacterMovement source:** `Assets/TopDownEngine/Common/Scripts/Characters/CharacterAbilities/CharacterMovement.cs`
- **Spawn world coords:** (20, 2-3, 0) = TWC local ~(14,14)
- **Managers group world pos:** (-7.5, 1.5, 9.5) — needed to calculate local spawn offsets
