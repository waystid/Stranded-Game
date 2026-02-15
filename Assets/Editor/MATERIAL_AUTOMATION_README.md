# Material Automation System - Documentation

## Overview

The Material Automation System provides automated tools for converting Loft3D materials to curved world shader variants for the Galactic Crossing project. This system is designed to streamline the material conversion process required for the "Rolling Log" curved world effect described in the game design document.

## Files Created

### 1. **ShaderHelperUtility.cs**
Location: `/Assets/Editor/ShaderHelperUtility.cs`

A static utility class providing reusable helper methods for material and shader operations:

**Material Discovery:**
- `FindMaterialsInDirectory()` - Finds all materials in a directory
- `FindAllMeshRenderers()` - Finds all renderers in the scene
- `FindAllRenderers()` - Finds all renderer types (Mesh, Skinned, etc.)

**Material Operations:**
- `DuplicateMaterial()` - Creates material duplicates with suffix
- `AssignMaterialToGameObject()` - Assigns materials to renderers
- `AssignShaderToMaterial()` - Assigns shaders with fallback support

**Validation:**
- `ValidateMaterialAssignment()` - Checks prefab safety
- `ShaderExists()` - Verifies shader availability
- `ShouldExcludeMaterial()` - Identifies materials to skip (particles, VFX, UI)
- `GetMaterialPriority()` - Categorizes materials by priority (CRITICAL, HIGH, MEDIUM, LOW)

### 2. **MaterialConversionAutomation.cs**
Location: `/Assets/Editor/MaterialConversionAutomation.cs`

The main editor window tool for batch material conversion with a comprehensive GUI.

**Menu Items:**
- `Tools > Galactic Crossing > Convert Materials to Curved World` - Opens the main conversion window
- `Tools > Galactic Crossing > Batch Assign Materials to Scene` - Replaces scene materials with curved variants
- `Tools > Galactic Crossing > Validate Scene Materials` - Generates material validation report
- `Tools > Galactic Crossing > List All Material Conversions` - Logs detailed material information

## Usage Guide

### Step 1: Convert Materials

1. Open Unity
2. Navigate to `Tools > Galactic Crossing > Convert Materials to Curved World`
3. The Material Conversion window will open showing all Loft3D materials

**Window Features:**
- **Refresh List** - Rescans the materials directory
- **Include Subdirectories** - Toggle to search in subfolders (furniture materials, etc.)
- **Auto-Assign Shader** - Automatically assigns curved world shader to duplicates
- **Show Excluded** - Display materials excluded from conversion (particles, VFX, UI)

**Material List Columns:**
- **Material** - The original material asset
- **Priority** - CRITICAL (ground), HIGH (walls), MEDIUM (furniture), LOW (other)
- **Status** - Pending, Converted, or Excluded
- **Actions** - Convert individual materials or select converted variants

4. Click **"Convert All Pending Materials"** to batch convert all materials
5. Individual materials can be converted using the "Convert" button per row

**What Happens During Conversion:**
- Original material is duplicated (e.g., `LoftGroundMaterial.mat` → `LoftGroundMaterial_CurvedWorld.mat`)
- Duplicates are created in the same directory as the original
- Curved world shader is assigned if available (fallback to URP/Lit if not)
- Conversion is logged to the console with priority levels

### Step 2: Assign Materials to Scene Objects

After converting materials, you need to assign them to scene objects:

**Method A: Batch Assignment (Recommended)**
1. Open your Loft3D scene
2. Navigate to `Tools > Galactic Crossing > Batch Assign Materials to Scene`
3. Click "Proceed" to confirm
4. All renderers in the scene will have their materials replaced with `_CurvedWorld` variants (if available)

**Method B: Manual Assignment**
1. Select a GameObject in the scene
2. In the Inspector, locate the Renderer component
3. Replace materials in the Materials array with their `_CurvedWorld` counterparts

### Step 3: Validate Conversion

To verify all materials are properly converted:

1. Navigate to `Tools > Galactic Crossing > Validate Scene Materials`
2. Review the validation report showing:
   - Total renderers
   - Count of curved world materials
   - Count of standard materials (not yet converted)
   - Missing materials

## Material Priority Levels

Based on the game design document, materials are categorized by priority:

### CRITICAL Priority
**Ground Materials** - Essential for gameplay, highest visual impact
- `LoftGroundMaterial.mat`
- `LoftGroundMaterialAlt.mat`
- `LoftGroundReflective.mat`

### HIGH Priority
**Wall Materials** - Core environment visuals
- `LoftWallMaterialBlue.mat`
- `LoftWallMaterialGreen.mat`
- `LoftWallMaterialPink.mat`
- `LoftWallMaterialYellow.mat`
- `LoftWallTopMaterial.mat`
- `LoftWallBottomMaterial.mat`

### MEDIUM Priority
**Furniture & Character Materials** - Props and interactive objects
- `LoftCouchMaterial.mat`
- `LoftLampMaterial.mat`
- `LoftMetalMaterial.mat`
- `LoftPlantMaterial.mat`
- `LoftScreenMaterial.mat`
- `LoftWood.mat`
- Gate, Teleporter, Ice materials

### Excluded Materials
**Particles, VFX, and UI** - Should NOT use curved world shader
- `LoftSmokeMaterial.mat` (particle)
- `LoftCollectionExplosionMaterial.mat` (VFX)
- `LoftReticleMaterial.mat` (UI)
- `LoftBlood.mat` (particle)
- `LoftClickParticlesMaterial.mat` (particle)
- `LoftWalkParticles.mat` (particle)
- `LoftExplosionMaterial.mat` (VFX)
- `LoftExplosionRemnantsMaterial.mat` (VFX)
- `LoftPotExplosionMaterial.mat` (VFX)

## Shader Assignment Behavior

### When Curved World Shader Exists
If `Shader Graphs/CurvedWorldLit` shader is found in the project:
- Converted materials automatically use the curved world shader
- Material properties are preserved from the original

### When Curved World Shader Does NOT Exist
If the shader is not yet created:
- Converted materials use fallback: `Universal Render Pipeline/Lit`
- Materials can be batch-updated later when the shader is available
- System logs a warning indicating fallback usage

**To Update Shaders Later:**
After creating the curved world shader, you can:
1. Open the Material Conversion window
2. Materials will show as "Converted" but with fallback shader
3. Manually assign the curved world shader to each `_CurvedWorld` material
4. Or re-run the conversion (existing materials will be skipped)

## Undo Support

All material operations support Unity's Undo system:
- Material duplication can be undone
- Scene material assignments can be undone with `Ctrl+Z` / `Cmd+Z`
- Prefab overrides are properly tracked

## Prefab Handling

When assigning materials to prefab instances:
- The system creates prefab overrides (materials are instance-specific)
- A warning is logged to the console
- To apply changes to the prefab asset, use "Apply" in the Inspector

**Best Practice:**
- Test material assignments on scene instances first
- Once satisfied, apply to prefab assets manually
- This prevents accidental prefab modifications

## Troubleshooting

### Materials Not Appearing in List
- Click "Refresh List" button
- Verify materials exist in `/Assets/TopDownEngine/Demos/Loft3D/Materials/`
- Check "Include Subdirectories" is enabled for furniture materials

### Conversion Fails
- Check Unity console for error messages
- Verify write permissions for the Materials directory
- Ensure no materials are currently being edited

### Batch Assignment Doesn't Work
- Ensure scene is loaded and saved
- Verify `_CurvedWorld` materials exist (run conversion first)
- Check that renderers have materials assigned

### Shader Not Found Warning
- This is expected if curved world shader hasn't been created yet
- Materials will use URP/Lit fallback
- Re-assign shaders after creating the curved world shader graph

## Next Steps

After material conversion:

1. **Create Curved World Shader** - Build the shader graph as described in the GDD (Section 5.2)
2. **Test in Scene** - Open Loft3D demo scene and verify curved effect
3. **Adjust Curvature** - Tune the `CurveAmount` parameter (default: 0.005)
4. **Apply to Characters** - Ensure character materials also use curved world variants
5. **Performance Testing** - Verify shader performance on target hardware

## Material Count Reference

Total Loft3D Materials: **38 materials**
- Root Materials Directory: 27 materials
- Loft Subdirectory: 4 materials
- Furniture Subdirectory: 7 materials

**Expected Conversion:**
- Convertible: ~28 materials (excluding particles/VFX/UI)
- Excluded: ~10 materials (particles, VFX, UI)
- Total After Conversion: ~66 material files (originals + curved variants)

## Code Architecture

### ShaderHelperUtility Design Patterns
- **Static Utility Class** - No instantiation required
- **Pure Functions** - No side effects, testable
- **Defensive Programming** - Null checks, validation
- **Separation of Concerns** - Each method has single responsibility

### MaterialConversionAutomation Design Patterns
- **Editor Window Pattern** - Unity standard GUI window
- **Item-based UI** - Each material represented as a list item
- **Batch Operations** - Convert multiple materials efficiently
- **State Tracking** - Track conversion status per material

## Advanced Usage

### Scripting API

You can use the helper utilities in your own editor scripts:

```csharp
using GalacticCrossing.Editor;

// Find all materials
List<Material> materials = ShaderHelperUtility.FindMaterialsInDirectory(
    "Assets/TopDownEngine/Demos/Loft3D/Materials",
    true
);

// Duplicate a material
Material original = AssetDatabase.LoadAssetAtPath<Material>("Assets/MyMaterial.mat");
Material curved = ShaderHelperUtility.DuplicateMaterial(original, "_CurvedWorld");

// Assign shader
ShaderHelperUtility.AssignShaderToMaterial(
    curved,
    "Shader Graphs/CurvedWorldLit",
    "Universal Render Pipeline/Lit"
);

// Batch assign to scene
Renderer[] renderers = ShaderHelperUtility.FindAllRenderers(true);
foreach (Renderer r in renderers)
{
    ShaderHelperUtility.AssignMaterialToGameObject(r.gameObject, curved, 0);
}
```

### Custom Exclusion Rules

To modify which materials are excluded, edit `ShaderHelperUtility.cs`:

```csharp
public static bool ShouldExcludeMaterial(Material material)
{
    string[] exclusionKeywords = new string[]
    {
        "particle", "explosion", "smoke", "reticle", "ui",
        "blood", "click", "walk", "remnant", "vfx", "effect",
        // Add your custom keywords here
        "custom_exclude_keyword"
    };
    // ...
}
```

### Custom Priority Levels

To modify priority categorization, edit `ShaderHelperUtility.cs`:

```csharp
public static string GetMaterialPriority(Material material)
{
    string materialName = material.name.ToLower();

    // Add custom priority rules
    if (materialName.Contains("your_custom_keyword"))
    {
        return "CRITICAL";
    }
    // ...
}
```

## Support and Feedback

This system was created based on the Galactic Crossing Game Design Document specifications. For issues or feature requests, refer to the project documentation or modify the scripts directly - they are fully commented and designed for extension.

---

**Created for:** Galactic Crossing - Material Automation Agent
**Version:** 1.0
**Date:** 2026-02-15
**Unity Version:** 2021.3+ (URP)
**Dependencies:** TopDown Engine, Universal Render Pipeline
