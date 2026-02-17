# World Expansion + Bug Fixes - 2026-02-17 (Part 2)

## Quick Reference

- **Date**: 2026-02-17
- **Type**: Bug Fix + Feature Enhancement
- **Status**: ✅ Complete
- **Branch**: feature/camera-controller
- **Session**: 3
- **Related Wiki**: [ACNH Camera + Island](./2026-02-17-acnh-camera-island.md)
- **Next Session**: Curved World Shader + Skybox + Day/Night Cycle

---

## Objective

**Goal:** Fix two blocking bugs discovered after completing the ACNH camera session, then expand the play space from 16×16 to 64×64 and tighten the camera for a closer, more intimate feel.

**Success Criteria:**
- [x] Synty compiler error resolved (blocked Play mode)
- [x] Duplicate player character fixed (one spawns immediately, one spawns later)
- [x] World expanded to 64×64
- [x] Camera tightened (closer + narrower FOV)
- [x] Scene saved and pushed to git

---

## Implementation

### Fix 1: Synty SidekickCharacters Compiler Error

**Error:**
```
Assets/Synty/SidekickCharacters/Scripts/Editor/ModularCharacterWindow.cs(25,13):
error CS0234: The type or namespace name 'VisualScripting' does not exist in the namespace 'Unity'
```

**Root cause:** Synty's editor scripts use `Unity.VisualScripting.YamlDotNet.Serialization` (Serializer/Deserializer), which lives in the `com.unity.visualscripting` package. The package wasn't installed.

**Attempt 1 (failed):** Added `com.unity.visualscripting: 1.9.5` to `Packages/manifest.json`. Package Manager installed it but the error persisted.

**Attempt 2 (failed):** Created a new `SyntyEditorScripts.asmdef` in the same folder. Unity rejected it — a folder can only have one `.asmdef`.

**Fix (success):** Modified the existing `Assets/Synty/SidekickCharacters/Scripts/Editor/SidekickCharacters.Editor.asmdef`:
```json
"defineConstraints": ["UNITY_VISUAL_SCRIPTING_PRESENT"],
"versionDefines": [
    {
        "name": "com.unity.visualscripting",
        "expression": "",
        "define": "UNITY_VISUAL_SCRIPTING_PRESENT"
    }
]
```
When Visual Scripting is absent, `UNITY_VISUAL_SCRIPTING_PRESENT` is never defined → the entire assembly is excluded from compilation → no error.

---

### Fix 2: Duplicate Player Characters

**Symptom:** Two player characters appear in Play mode — one immediately, one spawns in shortly after.

**Root cause:** Both of these existed simultaneously:
1. `AstronautPlayerEdit` — a pre-placed character instance in the scene root (tagged Player, all TDE components), visible immediately
2. `LevelManager.PlayerPrefabs = ["Assets/Prefabs/AstronautPlayer.prefab"]` — spawns a second character from `InitialSpawnPoint` after the intro fade

**Fix:** Deleted the pre-placed `AstronautPlayerEdit` from the scene. LevelManager now exclusively manages spawning. One player only.

**TopDown Engine pattern confirmed:** Use either pre-placed characters (set in `LevelManager.SceneCharacters`) OR `PlayerPrefabs` spawn — not both.

---

### Feature: 64×64 World Expansion

**IslandGround** scale updated: `(1.6, 1, 1.6)` → `(6.4, 1, 6.4)`

World bounds: `X: -32 to +32, Z: -32 to +32`

**Trees repositioned to 64×64 perimeter:**
| Name    | Position       |
|---------|----------------|
| Tree_01 | (-26, 0, -26)  |
| Tree_02 | (22, 0, -28)   |
| Tree_03 | (28, 0, 4)     |
| Tree_04 | (24, 0, 24)    |
| Tree_05 | (0, 0, 28)     |
| Tree_06 | (-24, 0, 20)   |
| Tree_07 | (-28, 0, -8)   |
| Tree_08 | (8, 0, -24)    |

**Rocks repositioned mid-field:**
| Name    | Position       |
|---------|----------------|
| Rock_01 | (10, 0, 8)     |
| Rock_02 | (-14, 0, 12)   |
| Rock_03 | (16, 0, -12)   |
| Rock_04 | (-8, 0, -18)   |

---

### Feature: Camera Tightened

| Property        | Before | After |
|----------------|--------|-------|
| CameraDistance | 20     | 14    |
| Lens.FieldOfView | 55°  | 45°   |

Rationale: On a 64×64 world the player should not see the full island. Tighter camera (distance 14, FOV 45°) keeps ~15-18 world units visible at a time — ACNH exploration feel where the world extends beyond the camera frame.

---

## Technical Details

### asmdef defineConstraints Pattern

To conditionally exclude a Unity assembly when an optional package is missing:
```json
{
    "defineConstraints": ["MY_DEFINE"],
    "versionDefines": [
        {
            "name": "com.unity.somepackage",
            "expression": "",
            "define": "MY_DEFINE"
        }
    ]
}
```
- `expression: ""` means "any version"
- If the package is absent, the define is never set, and the assembly is skipped entirely

### One .asmdef Per Folder Rule

Unity enforces **one `.asmdef` per directory**. If an `.asmdef` already exists in a folder, do not create a new one — modify the existing one instead.

### TopDown Engine Player Spawning

Two modes (mutually exclusive):
- **Prefab mode:** `LevelManager.PlayerPrefabs` + `InitialSpawnPoint` → LevelManager spawns and manages lifecycle
- **Scene mode:** `LevelManager.SceneCharacters` → pre-placed characters, LevelManager registers them

Mixing both = duplicate players. Check for pre-placed player instances in scene root before Play.

---

## Results

- ✅ Play mode works — no compiler errors
- ✅ Exactly one player spawns at island center (0, 1, 0)
- ✅ 64×64 grass ground plane
- ✅ Trees at perimeter ~±24-28 units, rocks scattered mid-field
- ✅ Camera distance 14, FOV 45° — tight, ACNH-style
- ✅ Scene saved and committed

---

## Files Modified

- `Assets/Scenes/SandboxShowcase.unity` — world expanded, pre-placed player removed, camera tightened
- `Assets/Synty/SidekickCharacters/Scripts/Editor/SidekickCharacters.Editor.asmdef` — added defineConstraints
- `Packages/manifest.json` — added com.unity.visualscripting 1.9.5

---

## Next Steps

### Next Session: Curved World Shader + Skybox + Day/Night Cycle

**Objective:** Make the world visually match ACNH's aesthetic.

**1. Curved World Shader**
- Vertex shader that bends geometry away from camera center (subtle Earth-curvature effect)
- Apply to: IslandGround, Trees, Rocks
- Reference: ACNH uses a ~0.002-0.005 curvature coefficient on world-space XZ distance from camera

**2. Skybox**
- Replace Unity default skybox with one of the 5 FarlandSkies variants
- Available at: `Assets/FarlandSkies/`
- Recommendation: pastel blue/warm variant to match ACNH feel
- Set via: `RenderSettings.skybox` in scene or `Window > Rendering > Lighting`

**3. Day/Night Cycle**
- Animate the Directional Light (rotation over time + color temperature shift)
- Optional: Blend skybox material properties with time of day
- Simple approach: MonoBehaviour that rotates `Lights/Directional Light` on X axis over real-time seconds

**Asset References:**
- FarlandSkies skyboxes: `Assets/FarlandSkies/` — 5 variants
- Directional Light: in `Lights ------------------------------------------------------------` group
- SineVFX crystals can add ambient particle effects

---

## Related Entries

**Previous:** [ACNH Camera + Island Setup](./2026-02-17-acnh-camera-island.md)
**Next:** Curved World Shader + Skybox + Day/Night Cycle (upcoming)

---

**Entry Created:** 2026-02-17
**Status:** ✅ Complete
**Ready for Next Session:** Curved World Shader + Skybox + Day/Night Cycle
