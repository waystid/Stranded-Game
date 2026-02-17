# ACNH Camera Controller & Island Setup - 2026-02-17

## Quick Reference

- **Date**: 2026-02-17
- **Type**: Feature Implementation
- **Status**: ✅ Complete
- **Branch**: feature/camera-controller
- **Session**: 2
- **Related Wiki**: [TopDown Engine Integration](../../topdown_engine/README.md)
- **Next Session**: Curved world shader + skybox + day/night cycle

---

## Objective

**Goal:** Implement an Animal Crossing: New Horizons-style fixed isometric camera and build a 16×16 starter island using Pandazole nature assets.

**Success Criteria:**
- [x] ACNH-style fixed isometric camera angle (38° pitch, 45° yaw diagonal)
- [x] 16×16 grass ground plane
- [x] 8 spring trees from Pandazole placed around perimeter
- [x] 4 rounded rocks scattered naturally
- [x] CharacterRotateCamera disabled (no camera orbit from player input)
- [x] Player spawns at island center (0, 1, 0)

---

## Implementation

### Phase 1: Asset Discovery

Pandazole Ultimate Pack contents catalogued:
- **Trees**: 37 Spring variants in `Pandazole Nature Environment Pack/Prefabs/Tree_XX_Spring.prefab`
- **Rocks**: 32 SoftRock variants in `Pandazole Nature Environment Pack/Prefabs/SoftRock_XX.prefab`
- **Material**: Single shared `PandaMat.mat` (vertex-color based, not suitable for ground)

### Phase 2: Scene Preparation

Working in **SandboxShowcase** scene (already has TopDown Engine managers, LevelManager, Cinemachine camera):

**Hidden (not deleted):**
- Level/Walls, Level/Borders, Level/AI, Level/Furniture
- Level/TargetPractice, Level/Pickers, Level/Doors, Level/Ground
- Root Ground (82×80 ProBuilder mesh)

### Phase 3: 16×16 Island Ground

```
Object: IslandGround
Type: Unity Plane primitive (default 10×10 units)
Scale: (1.6, 1, 1.6) = 16×16 world units
Position: (0, 0, 0)
Layer: Ground (layer 9) - required for TopDown Engine collision
Material: Assets/Materials/IslandGrass.mat
  Shader: Standard
  Color: (0.28, 0.62, 0.18) - grass green
```

### Phase 4: Environment Dressing

**8 Spring Trees (varied types):**
| Name    | Prefab            | Position      |
|---------|-------------------|---------------|
| Tree_01 | Tree_01_Spring    | (-6, 0, -6)   |
| Tree_02 | Tree_05_Spring    | (5, 0, -7)    |
| Tree_03 | Tree_08_Spring    | (7, 0, 1)     |
| Tree_04 | Tree_11_Spring    | (6, 0, 6)     |
| Tree_05 | Tree_15_Spring    | (0, 0, 7)     |
| Tree_06 | Tree_20_Spring    | (-6, 0, 5)    |
| Tree_07 | Tree_25_Spring    | (-7, 0, -2)   |
| Tree_08 | Tree_30_Spring    | (2, 0, -6)    |

**4 Soft Rocks:**
| Name    | Prefab         | Position      |
|---------|----------------|---------------|
| Rock_01 | SoftRock_01    | (3, 0, 2)     |
| Rock_02 | SoftRock_10    | (-4, 0, 3)    |
| Rock_03 | SoftRock_20    | (4, 0, -3)    |
| Rock_04 | SoftRock_05    | (-2, 0, -5)   |

### Phase 5: ACNH Camera Setup

**Target:** CM vcam1 (Cinemachine 3 virtual camera, already in scene)
**Approach:** Modified existing Cinemachine setup rather than custom script

```
Component: CinemachineCamera (CM vcam1)
Transform Rotation: Euler(38, 45, 0)
  → Pitch 38° downward (ACNH feel)
  → Yaw 45° diagonal (NW→SE view direction)
  → Fixed: no rotation components, camera keeps this angle permanently

Component: CinemachinePositionComposer
  → CameraDistance: 20 (reduced from 35 for 16×16 space)
  → FollowsAPlayer: true (auto-assigns player on spawn)

Component: CinemachineCamera
  → Lens.FieldOfView: 55° (was 40°)
```

**Backup Script:** `Assets/Scripts/Camera/ACNHCameraFollow.cs`
- Standalone MonoBehaviour option if Cinemachine setup is swapped
- Default offset (-11, 12, -11) = same effective angle
- Configurable in Inspector: PitchAngle, YawAngle, FOV, SmoothTime

### Phase 6: Player Setup

**Disabled on AstronautPlayer.prefab:**
- `CharacterRotateCamera.enabled = false`
  - This ability was rotating the camera based on mouse/joystick input
  - With fixed camera angle, this must be off

**Spawn point repositioned:**
- InitialSpawnPoint world position: (0, 1, 0) — island center, 1 unit above ground
- Local position: (7.5, -0.5, -9.5) relative to Managers group at world (-7.5, 1.5, 9.5)

---

## Technical Details

### Camera Math

ACNH camera angle — looking from NW down at SE at ~38°:
```
Euler(38, 45, 0) → camera look direction:
  forward = (sin45·cos38, -sin38, cos45·cos38)
           = (0.559, -0.616, 0.559)
Camera offset from player = -forward × 20 = (-11.2, 12.3, -11.2)
```

This matches `ACNHCameraFollow.Offset = (-11, 12, -11)` in the backup script.

### Cinemachine 3 Notes

- `CinemachinePositionComposer` positions camera at `CameraDistance` from tracking target
- Camera rotation is controlled by the vcam Transform (no separate aim component)
- `CinemachineCameraController.FollowsAPlayer = true` auto-assigns any spawned player
- Setting `Lens.FieldOfView` via `manage_components` requires passing the full `Lens` struct object

### World Scale Reference

```
16×16 play space:
  Unity Plane scale: (1.6, 1, 1.6)
  Player occupies: ~1×1 unit
  Island center: (0, 0, 0)
  Island bounds: X: -8 to +8, Z: -8 to +8
  Tree perimeter radius: ~6-7 units from center
  Rock scatter radius: ~2-5 units from center

Next expansion (95×65):
  Unity Plane scale: (9.5, 1, 6.5)
  Camera distance increase: ~50 (from 20)
  ACNHCameraFollow.Offset: (-27, 30, -27) approx
```

---

## Results

### ✅ All Success

- Island ground: 16×16 green grass ✓
- 8 trees around perimeter ✓
- 4 rocks scattered ✓
- Camera at ACNH angle (38°, 45°, dist=20, FOV=55°) ✓
- CharacterRotateCamera disabled ✓
- Player spawns at island center ✓
- No new console errors ✓
- Scene saved ✓

---

## Lessons Learned

### CinemachinePositionComposer Property Path for Lens
Setting `Lens.FieldOfView` with dot notation fails in MCP. Must pass the whole Lens struct:
```python
manage_components(
    action="set_property",
    target=vcam_id,
    component_type="CinemachineCamera",
    property="Lens",
    value={"FieldOfView": 55, "OrthographicSize": 10, "NearClipPlane": 0.1,
           "FarClipPlane": 5000, "Dutch": 0, "ModeOverride": 0}
)
```

### Pandazole Material on Flat Plane
PandaMat is vertex-color based (designed for detailed 3D models). On a flat plane it produces a flat grey/brown look. Always create a separate `Standard` material for ground planes.

### World Position of Children
When needing exact world position for a child GameObject, calculate local offset:
`local_needed = world_desired - parent.world_position`

---

## Blockers Encountered

### 1. Cinemachine FOV Property Path
- **Problem:** `Lens.FieldOfView` and `FieldOfView` both failed
- **Solution:** Pass full Lens struct to "Lens" property
- **Resolution:** 3 attempts, fixed

### 2. Spawn Point Off-Island
- **Problem:** InitialSpawnPoint at world (-7.5, 2.5, 9.5) — Z outside island
- **Solution:** Calculated required local offset (7.5, -0.5, -9.5) from Managers parent pos
- **Resolution:** 1 attempt, fixed

---

## Next Steps

### Immediate (Next Session)

**Topic:** Curved World Shader + Skybox + Day/Night Cycle

**Objective:** Make the world look like ACNH with:
1. **Curved world shader** — the subtle Earth-curvature effect in ACNH
   - Vertex shader that bends geometry away from center
   - Apply to ground, trees, rocks
2. **Skybox** — replace with one of the 5 FarlandSkies variants
   - Suggest: pastel blue/warm tones to match ACNH feel
3. **Day/Night cycle** — animate sun direction + color temperature
   - Directional light rotation over time
   - Skybox blend or material property change

**Reference assets available:**
- FarlandSkies: `Assets/FarlandSkies/` — 5 variants
- SineVFX crystals can add ambient particles

### Camera Handoff Notes

Ready to hand off to next session. Camera configuration:
- Scene: `Assets/Scenes/SandboxShowcase.unity`
- Camera object: `3DCameras/CM vcam1`
- Script (backup): `Assets/Scripts/Camera/ACNHCameraFollow.cs`
- All values exposed in Inspector for easy tweaking

**If camera angle needs tweaking in Inspector:**
- Pitch (steeper/shallower): change CM vcam1 Transform X rotation (currently 38°)
- Yaw (rotate diagonal): change CM vcam1 Transform Y rotation (currently 45°)
- Zoom: change CinemachinePositionComposer.CameraDistance (currently 20)
- FOV: change CinemachineCamera Lens.FieldOfView (currently 55°)

---

## Files Modified

### New Files
- `Assets/Materials/IslandGrass.mat` — green grass material
- `Assets/Scripts/Camera/ACNHCameraFollow.cs` — backup ACNH camera MonoBehaviour
- `CosmicWiki/devlog/entries/2026-02-17-acnh-camera-island.md` — this entry

### Modified Files
- `Assets/Scenes/SandboxShowcase.unity` — island built, camera configured
- `Assets/Prefabs/AstronautPlayer.prefab` — CharacterRotateCamera disabled

### Git
- Branch: `feature/camera-controller`
- Branched from: `sandbox @ dafc5bcf`

---

## Related Entries

**Previous:** [Player Model Replacement](./2026-02-16-player-model-replacement.md)
**Next:** Curved World + Skybox + Day/Night Cycle (upcoming)

---

**Entry Created:** 2026-02-17
**Status:** ✅ Complete
**Ready for Next Session:** Curved World Shader + Skybox
