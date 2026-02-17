# Curved World Shader + Skybox + Day/Night Cycle - 2026-02-16

## Quick Reference

- **Date**: 2026-02-16
- **Type**: Visual Feature Implementation
- **Status**: ✅ Complete
- **Branch**: feature/camera-controller
- **Session**: 4
- **Related Wiki**: [World Expansion + Bug Fixes](./2026-02-17-world-expansion-bugfixes.md)
- **Next Session**: TBD

---

## Objective

**Goal:** Three visual features to match ACNH aesthetic on the 64×64 island.

**Success Criteria:**
- [x] Curved world vertex shader applied to IslandGround, 8 trees (trunk + canopy), 4 rocks
- [x] FarlandSkies CloudyCrown_01_Midday skybox set as scene skybox
- [x] DayNightCycle MonoBehaviour on DirectionalLight animating sun rotation + color + intensity
- [x] No new console errors
- [x] Scene saved

---

## Implementation

### Feature 1: Curved World Shader

**File:** `Assets/Shaders/CurvedWorld.shader`

A Unity surface shader that bends geometry downward proportional to the **squared XZ distance** from the camera. Classic ACNH horizon-curvature effect.

**Key technique:**
```hlsl
void vert(inout appdata_full v)
{
    float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
    float2 camDist = worldPos.xz - _WorldSpaceCameraPos.xz;
    float distSq = dot(camDist, camDist);
    worldPos.y -= distSq * _Curvature;
    v.vertex = mul(unity_WorldToObject, worldPos);
}
```

**Properties:**
- `_Color` — flat color (white for vertex-color objects, grass green for ground)
- `_Curvature` — set to `0.002` on both materials (tunable per-material)
- `_MainTex` — optional texture slot
- `_Metallic`, `_Glossiness` — standard PBR

**Vertex color support:** The surf function multiplies `IN.color` (baked vertex colors) by `_Color`. This means Pandazole trees/rocks keep their original vertex-color shading while gaining curvature. Ground (no vertex colors → defaults to white) uses `_Color = (0.28, 0.62, 0.18)` for grass.

**Two materials created:**
| Material | Shader | `_Color` | `_Curvature` | Applied to |
|----------|--------|----------|-------------|------------|
| `CurvedWorldGrass.mat` | Custom/CurvedWorld | (0.28, 0.62, 0.18, 1) | 0.002 | IslandGround |
| `CurvedWorldNature.mat` | Custom/CurvedWorld | (1, 1, 1, 1) | 0.002 | Tree_01–08 (root + canopy), Rock_01–04 |

**Important:** Each Pandazole tree has TWO renderers — root (trunk) and one child (canopy `_p2_Spring`). Both must have CurvedWorldNature applied. Total: 8 trunk + 8 canopy + 4 rocks = **21 renderer assignments**.

---

### Feature 2: Skybox

**Asset:** `Assets/FarlandSkies/Skyboxes/CloudyCrown_01_Midday/CloudyCrown_Midday.mat`

Set at runtime by `DayNightCycle.ApplySkybox()` in `Awake()`.

**Fallback pattern:**
```csharp
void ApplySkybox()
{
    if (skyboxMaterial != null)
    {
        RenderSettings.skybox = skyboxMaterial;
        return;
    }
#if UNITY_EDITOR
    var mat = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(
        "Assets/FarlandSkies/Skyboxes/CloudyCrown_01_Midday/CloudyCrown_Midday.mat");
    if (mat != null)
        RenderSettings.skybox = mat;
#endif
}
```

The `skyboxMaterial` field was also wired via MCP `manage_components set_property`. Double safety: serialized ref + AssetDatabase fallback.

**FarlandSkies available variants (for future use):**
- `CloudyCrown_01_Midday` — bright blue, white clouds ✅ (selected)
- `CloudyCrown_02_Evening` — warm orange/pink
- `CloudyCrown_03_Midnight` — dark blue/indigo
- `CloudyCrown_04_Sunset` — deep orange
- `CloudyCrown_05_Daybreak` — pale pink/lavender
- Plus PlainColor (Blue, Orange, Purple, Red) and Gradient variants

---

### Feature 3: Day/Night Cycle

**File:** `Assets/Scripts/Environment/DayNightCycle.cs`
**Attached to:** `Lights/DirectionalLight` (instance ID: 169120)

**Key design:**
- `timeOfDay` — normalized 0–1 (0=midnight, 0.25=sunrise, 0.5=noon, 0.75=sunset)
- `dayDurationSeconds = 120` — 2-minute full cycle (adjustable in Inspector)
- Sun rotation: `sunAngle = (timeOfDay - 0.25f) * 360f` → X axis of Directional Light
- Color + intensity sampled from hardcoded keyframe arrays (no Gradient/AnimationCurve dependencies)

**Color gradient keyframes:**
| timeOfDay | Description | Color |
|-----------|-------------|-------|
| 0.00 | Midnight | `(0.05, 0.05, 0.20)` dark blue |
| 0.20 | Sunrise | `(1.00, 0.45, 0.15)` deep orange |
| 0.30 | Morning | `(1.00, 0.90, 0.75)` warm white |
| 0.50 | Noon | `(1.00, 1.00, 1.00)` pure white |
| 0.70 | Afternoon | `(1.00, 0.90, 0.70)` warm white |
| 0.80 | Sunset | `(1.00, 0.40, 0.10)` orange-red |
| 0.90 | Dusk | `(0.25, 0.10, 0.25)` purple-dark |

**Intensity keyframes:** 0.02 (night) → 0.50 (sunrise) → 1.20 (noon) → 0.02 (night)

---

## Technical Details

### Curved World Shader — Coordinate Space

The curvature is applied in **world space** then converted back to **object space**:
```
world_pos = ObjectToWorld * vertex
world_pos.y -= dot(world_pos.xz - camera.xz, world_pos.xz - camera.xz) * curvature
vertex = WorldToObject * world_pos
```
This correctly handles objects at any rotation/scale — curvature is always relative to world XZ distance from camera regardless of object transform.

### Curvature Coefficient Tuning

For a 64×64 world with camera distance 14 and FOV 45°:
- At distance 32 (far edge of island): `y_offset = 32² × 0.002 = 2.048 units` drop
- At distance 10 (nearby tree): `y_offset = 10² × 0.002 = 0.2 units` — barely visible
- This creates the subtle horizon-curve ACNH feel without distorting near objects

Inspector tip: Increase `_Curvature` to 0.004-0.005 for stronger effect, decrease to 0.001 for subtler look.

### Day/Night Sun Rotation Math

```
sunAngle = (timeOfDay - 0.25) * 360°

t=0.00 → -90°  (midnight, light from below)
t=0.25 →   0°  (sunrise, light from horizon)
t=0.50 →  90°  (noon, light from zenith)
t=0.75 → 180°  (sunset, light from other horizon)
t=1.00 → 270°  (midnight again, loop)
```

Y axis fixed at -30° (NNW) to match the 45° camera angle, giving diagonal shadows.

---

## Results

- ✅ CurvedWorld.shader compiled with no errors
- ✅ DayNightCycle.cs compiled with no errors
- ✅ CurvedWorldGrass.mat → IslandGround
- ✅ CurvedWorldNature.mat → 8 tree trunks + 8 canopies + 4 rocks (21 total)
- ✅ DayNightCycle added to DirectionalLight, skyboxMaterial wired to CloudyCrown_01_Midday
- ✅ Scene saved
- ✅ No new console errors

---

## Files Created

### New Files
- `Assets/Shaders/CurvedWorld.shader` — custom surface shader with vertex curvature
- `Assets/Scripts/Environment/DayNightCycle.cs` — day/night cycle MonoBehaviour
- `Assets/Materials/CurvedWorldGrass.mat` — curved ground material (grass green)
- `Assets/Materials/CurvedWorldNature.mat` — curved nature material (vertex color passthrough)

### Modified Files
- `Assets/Scenes/SandboxShowcase.unity` — materials applied, DayNightCycle added

---

## Next Steps

### Suggested (Next Session)

1. **Curvature tuning** — enter Play mode, adjust `_Curvature` to taste
2. **Skybox blending by time of day** — swap skybox material based on `timeOfDay` range (Midday / Evening / Sunset)
3. **Ambient light** — animate `RenderSettings.ambientLight` in sync with sun color for night mood
4. **Player items** — AlienBerryPicker prefab spotted open in IDE — implement berry pickables
5. **SineVFX crystals** — add ambient particle effects around rocks/trees

---

## Related Entries

**Previous:** [World Expansion + Bug Fixes](./2026-02-17-world-expansion-bugfixes.md)
**Next:** TBD

---

**Entry Created:** 2026-02-16
**Status:** ✅ Complete
