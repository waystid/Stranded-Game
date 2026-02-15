# Material Conversion List

## Overview
This document lists all 38 materials found in the Loft3D demo and their conversion status for the Galactic Crossing curved world shader system.

---

## CRITICAL Priority (2 materials)

### Ground Materials
These materials are essential for gameplay and have the highest visual impact due to screen coverage.

| Original Material | Curved World Variant | Location | Notes |
|------------------|---------------------|----------|-------|
| `LoftGroundMaterial.mat` | `LoftGroundMaterial_CurvedWorld.mat` | `/Materials/` | Primary ground surface - HIGHEST PRIORITY |
| `LoftGroundMaterialAlt.mat` | `LoftGroundMaterialAlt_CurvedWorld.mat` | `/Materials/` | Alternate ground surface |
| `LoftGroundReflective.mat` | `LoftGroundReflective_CurvedWorld.mat` | `/Materials/Loft/` | Reflective ground variant |

**Total: 3 materials**

---

## HIGH Priority (6 materials)

### Wall Materials
Core environment visuals that define the level boundaries and aesthetics.

| Original Material | Curved World Variant | Location | Notes |
|------------------|---------------------|----------|-------|
| `LoftWallMaterialBlue.mat` | `LoftWallMaterialBlue_CurvedWorld.mat` | `/Materials/` | Blue wall variant |
| `LoftWallMaterialGreen.mat` | `LoftWallMaterialGreen_CurvedWorld.mat` | `/Materials/` | Green wall variant |
| `LoftWallMaterialPink.mat` | `LoftWallMaterialPink_CurvedWorld.mat` | `/Materials/` | Pink wall variant |
| `LoftWallMaterialYellow.mat` | `LoftWallMaterialYellow_CurvedWorld.mat` | `/Materials/` | Yellow wall variant |
| `LoftWallTopMaterial.mat` | `LoftWallTopMaterial_CurvedWorld.mat` | `/Materials/` | Wall top surface |
| `LoftWallBottomMaterial.mat` | `LoftWallBottomMaterial_CurvedWorld.mat` | `/Materials/` | Wall bottom surface |

**Total: 6 materials**

---

## MEDIUM Priority (19 materials)

### Furniture Materials
Props, decorations, and interactive objects.

| Original Material | Curved World Variant | Location | Notes |
|------------------|---------------------|----------|-------|
| `LoftCouchMaterial.mat` | `LoftCouchMaterial_CurvedWorld.mat` | `/Materials/Loft/LoftFurniture/` | Couch furniture |
| `LoftLampMaterial.mat` | `LoftLampMaterial_CurvedWorld.mat` | `/Materials/Loft/LoftFurniture/` | Lamp objects |
| `LoftLightMetalMaterial.mat` | `LoftLightMetalMaterial_CurvedWorld.mat` | `/Materials/Loft/LoftFurniture/` | Light metal surfaces |
| `LoftMetalMaterial.mat` | `LoftMetalMaterial_CurvedWorld.mat` | `/Materials/Loft/LoftFurniture/` | Standard metal |
| `LoftPlantMaterial.mat` | `LoftPlantMaterial_CurvedWorld.mat` | `/Materials/Loft/LoftFurniture/` | Plant decorations |
| `LoftScreenMaterial.mat` | `LoftScreenMaterial_CurvedWorld.mat` | `/Materials/Loft/LoftFurniture/` | Screen/monitor surfaces |
| `LoftWood.mat` | `LoftWood_CurvedWorld.mat` | `/Materials/Loft/LoftFurniture/` | Wooden surfaces |

### Environmental Objects

| Original Material | Curved World Variant | Location | Notes |
|------------------|---------------------|----------|-------|
| `LoftGateMaterial.mat` | `LoftGateMaterial_CurvedWorld.mat` | `/Materials/` | Gate objects |
| `LoftIceMaterial.mat` | `LoftIceMaterial_CurvedWorld.mat` | `/Materials/` | Ice surfaces |
| `LoftPathMaterial1.mat` | `LoftPathMaterial1_CurvedWorld.mat` | `/Materials/Loft/` | Path variant 1 |
| `LoftPathMaterial2.mat` | `LoftPathMaterial2_CurvedWorld.mat` | `/Materials/Loft/` | Path variant 2 |

### Teleporter Materials

| Original Material | Curved World Variant | Location | Notes |
|------------------|---------------------|----------|-------|
| `LoftTeleporterBlue.mat` | `LoftTeleporterBlue_CurvedWorld.mat` | `/Materials/` | Blue teleporter |
| `LoftTeleporterOrange.mat` | `LoftTeleporterOrange_CurvedWorld.mat` | `/Materials/` | Orange teleporter |
| `LoftTeleporterYellow.mat` | `LoftTeleporterYellow_CurvedWorld.mat` | `/Materials/` | Yellow teleporter |

### Collectible Materials

| Original Material | Curved World Variant | Location | Notes |
|------------------|---------------------|----------|-------|
| `LoftCoinMaterial.mat` | `LoftCoinMaterial_CurvedWorld.mat` | `/Materials/` | Coin pickups |
| `LoftAssaultRifleAmmoMaterial.mat` | `LoftAssaultRifleAmmoMaterial_CurvedWorld.mat` | `/Materials/` | Ammo pickup |
| `LoftHandgunAmmoMaterial.mat` | `LoftHandgunAmmoMaterial_CurvedWorld.mat` | `/Materials/` | Ammo pickup |
| `LoftShotgunRifleAmmoMaterial.mat` | `LoftShotgunRifleAmmoMaterial_CurvedWorld.mat` | `/Materials/` | Ammo pickup |
| `LoftSniperRifleAmmoMaterial.mat` | `LoftSniperRifleAmmoMaterial_CurvedWorld.mat` | `/Materials/` | Ammo pickup |

### Special Effects (Non-Particle)

| Original Material | Curved World Variant | Location | Notes |
|------------------|---------------------|----------|-------|
| `LoftConeOfVisionMaterial.mat` | `LoftConeOfVisionMaterial_CurvedWorld.mat` | `/Materials/` | AI vision cone |
| `LoftHDRRed.mat` | `LoftHDRRed_CurvedWorld.mat` | `/Materials/Loft/` | HDR red material |
| `LoftReflectSurface.mat` | `LoftReflectSurface_CurvedWorld.mat` | `/Materials/Loft/` | Reflective surface |

**Total: 22 materials**

---

## EXCLUDED Materials (10 materials)

### Particle Systems
These materials use particle shaders and should NOT be converted to curved world variants.

| Material Name | Reason for Exclusion | Location |
|--------------|---------------------|----------|
| `LoftBlood.mat` | Particle system | `/Materials/` |
| `LoftClickParticlesMaterial.mat` | Particle system | `/Materials/` |
| `LoftWalkParticles.mat` | Particle system | `/Materials/` |

### VFX Materials
Visual effects that should remain screen-space aligned.

| Material Name | Reason for Exclusion | Location |
|--------------|---------------------|----------|
| `LoftCollectionExplosionMaterial.mat` | VFX explosion | `/Materials/` |
| `LoftExplosionMaterial.mat` | VFX explosion | `/Materials/` |
| `LoftExplosionRemnantsMaterial.mat` | VFX debris | `/Materials/` |
| `LoftPotExplosionMaterial.mat` | VFX explosion | `/Materials/` |

### UI Materials
User interface materials that should remain screen-space.

| Material Name | Reason for Exclusion | Location |
|--------------|---------------------|----------|
| `LoftReticleMaterial.mat` | UI reticle | `/Materials/` |

**Total: 8 materials**

---

## Summary Statistics

| Category | Count | Percentage |
|----------|-------|------------|
| **CRITICAL Priority** | 3 | 9.4% |
| **HIGH Priority** | 6 | 18.8% |
| **MEDIUM Priority** | 22 | 68.8% |
| **EXCLUDED** | 8 | ~21% (of 38 total) |
| **Total Convertible** | 31 | 81.6% |
| **Total Materials** | 38 | 100% |

---

## Conversion Workflow

### Phase 1: CRITICAL Materials
Convert ground materials first as they have the highest visual impact:
1. `LoftGroundMaterial.mat`
2. `LoftGroundMaterialAlt.mat`
3. `LoftGroundReflective.mat`

**Test Point:** Verify curved world effect is visible on ground plane.

### Phase 2: HIGH Priority Materials
Convert wall materials to ensure environment curvature:
1. All `LoftWallMaterial*.mat` variants
2. `LoftWallTopMaterial.mat`
3. `LoftWallBottomMaterial.mat`

**Test Point:** Verify walls curve correctly at distance from camera.

### Phase 3: MEDIUM Priority Materials
Batch convert all furniture, environmental, and collectible materials.

**Test Point:** Verify all visible objects participate in world curvature.

### Phase 4: Validation
- Run `Tools > Galactic Crossing > Validate Scene Materials`
- Verify 31 materials converted, 8 excluded
- Check that no particle/VFX materials were converted

---

## Material Shader Requirements

### Curved World Shader Properties Required
The curved world shader must support these properties to maintain Loft3D visual style:

1. **Base Color** - Albedo/diffuse color (all materials)
2. **Metallic/Smoothness** - PBR properties (metal materials)
3. **Emission** - Self-illumination (teleporter, screens)
4. **Transparency** - Alpha blending (ice, vision cone)
5. **Curve Amount** - World curvature strength (0.001 - 0.01)

### Shader Graph Inputs to Preserve
When creating the curved world shader, ensure compatibility with:
- `_BaseColor` or `_Color` property
- `_MainTex` or `_BaseMap` texture
- `_Metallic` and `_Smoothness` values
- `_EmissionColor` for glowing materials

---

## File Size Impact

### Before Conversion
- Total Material Files: 38 `.mat` files
- Estimated Total Size: ~80 KB (38 × ~2KB average)

### After Conversion
- Original Materials: 38 `.mat` files
- Curved World Variants: 31 `.mat` files
- Total Files: 69 `.mat` files
- Estimated Total Size: ~140 KB

**Storage Impact:** +60 KB (negligible)

---

## Automation Benefits

Using the Material Automation scripts provides:

1. **Time Savings:** Manual conversion would take ~2-3 hours; automated takes ~2 minutes
2. **Consistency:** All materials follow same naming convention
3. **Validation:** Automatic exclusion of particle/VFX materials
4. **Undo Support:** Easy rollback if needed
5. **Priority Tracking:** Clear visibility of critical vs. optional conversions
6. **Batch Operations:** Convert 31 materials with single click

---

## Next Steps After Conversion

1. ✅ Run material conversion automation
2. ⬜ Create Curved World Shader Graph (see GDD Section 5.2)
3. ⬜ Test shader on CRITICAL ground materials
4. ⬜ Tune `CurveAmount` parameter (recommended: 0.005)
5. ⬜ Batch assign to Loft3D scene
6. ⬜ Validate scene materials (31 curved, 8 standard)
7. ⬜ Performance test on target hardware
8. ⬜ Apply to all Loft3D scenes/prefabs

---

**Document Version:** 1.0
**Last Updated:** 2026-02-15
**Project:** Galactic Crossing - Material Automation System
