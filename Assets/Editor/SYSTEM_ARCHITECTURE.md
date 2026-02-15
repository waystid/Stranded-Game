# Material Automation System - Architecture

## System Overview

The Material Automation System consists of two primary components working together to automate the conversion of Loft3D materials for the curved world shader.

```
┌─────────────────────────────────────────────────────────────────────┐
│                    MATERIAL AUTOMATION SYSTEM                       │
├─────────────────────────────────────────────────────────────────────┤
│                                                                     │
│  ┌────────────────────────────┐    ┌──────────────────────────┐   │
│  │  ShaderHelperUtility.cs    │◄───│ MaterialConversion       │   │
│  │  (Static Utility Class)    │    │ Automation.cs            │   │
│  │                            │    │ (Editor Window)          │   │
│  │  - FindMaterialsInDirectory│    │                          │   │
│  │  - DuplicateMaterial       │    │  - GUI Interface         │   │
│  │  - AssignShaderToMaterial  │    │  - Batch Operations      │   │
│  │  - ValidateMaterial        │    │  - Priority Tracking     │   │
│  │  - Material Classification │    │  - Status Management     │   │
│  └────────────────────────────┘    └──────────────────────────┘   │
│               ▲                                  │                  │
│               │                                  │                  │
│               │         ┌────────────────────────▼────────┐         │
│               │         │   Unity Editor Menu System     │         │
│               │         │   Tools > Galactic Crossing >  │         │
│               │         └────────────────────────────────┘         │
│               │                                                     │
│               ▼                                                     │
│  ┌─────────────────────────────────────────────────────────┐      │
│  │             Unity Asset Database (Materials)            │      │
│  │  /Assets/TopDownEngine/Demos/Loft3D/Materials/          │      │
│  └─────────────────────────────────────────────────────────┘      │
│                                                                     │
└─────────────────────────────────────────────────────────────────────┘
```

---

## Component Breakdown

### 1. ShaderHelperUtility.cs
**Type:** Static Utility Class
**Role:** Reusable helper methods for material operations
**Location:** `/Assets/Editor/ShaderHelperUtility.cs`

#### Functional Modules

```
ShaderHelperUtility
├── Material Discovery
│   ├── FindMaterialsInDirectory(path, recursive)
│   ├── FindAllMeshRenderers(includeInactive)
│   └── FindAllRenderers(includeInactive)
│
├── Material Operations
│   ├── DuplicateMaterial(original, suffix, targetDir)
│   ├── AssignMaterialToGameObject(gameObject, material, index)
│   └── AssignShaderToMaterial(material, shader, fallback)
│
├── Validation
│   ├── ValidateMaterialAssignment(gameObject, warn)
│   └── ShaderExists(shaderName)
│
└── Material Classification
    ├── ShouldExcludeMaterial(material)
    └── GetMaterialPriority(material)
```

**Key Features:**
- No state (pure functions)
- Defensive programming (null checks)
- Detailed logging
- AssetDatabase integration

---

### 2. MaterialConversionAutomation.cs
**Type:** Editor Window
**Role:** GUI-based batch conversion tool
**Location:** `/Assets/Editor/MaterialConversionAutomation.cs`

#### User Interface Components

```
MaterialConversionAutomation (Editor Window)
├── Header Section
│   └── Help text and instructions
│
├── Toolbar
│   ├── Refresh List button
│   ├── Include Subdirectories toggle
│   ├── Auto-Assign Shader toggle
│   └── Show Excluded toggle
│
├── Summary Stats
│   ├── Total Materials
│   ├── Already Converted
│   └── To Convert
│
├── Material List (Scrollable)
│   └── For each material:
│       ├── Material reference field
│       ├── Priority label (CRITICAL/HIGH/MEDIUM)
│       ├── Status (Pending/Converted/Excluded)
│       ├── Convert button
│       └── Select button (if converted)
│
└── Footer Section
    ├── "Convert All Pending Materials" button
    └── Shader status indicator
```

#### Data Flow

```
User Opens Window
       │
       ▼
OnEnable() → RefreshMaterialList()
       │
       ▼
ShaderHelperUtility.FindMaterialsInDirectory()
       │
       ▼
For Each Material:
  - Check if excluded (particles/VFX)
  - Determine priority (CRITICAL/HIGH/MEDIUM)
  - Check if already converted
  - Create MaterialConversionItem
       │
       ▼
Display in GUI (sorted by priority)
       │
       ▼
User Clicks "Convert All"
       │
       ▼
For Each Pending Material:
  - ConvertSingleMaterial()
    └── ShaderHelperUtility.DuplicateMaterial()
    └── ShaderHelperUtility.AssignShaderToMaterial()
       │
       ▼
Update UI Status
Save Assets
Refresh AssetDatabase
```

---

## Menu System Architecture

### Menu Items Registered

```
Unity Editor Menu Bar
└── Tools
    └── Galactic Crossing
        ├── Convert Materials to Curved World
        │   → Opens MaterialConversionAutomation window
        │
        ├── Batch Assign Materials to Scene
        │   → Calls MaterialConversionAutomation.BatchAssignMaterialsToScene()
        │   → Uses ShaderHelperUtility for scene operations
        │
        ├── Validate Scene Materials
        │   → Calls MaterialValidationTools.ValidateSceneMaterials()
        │   → Generates validation report
        │
        └── List All Material Conversions
            → Calls MaterialValidationTools.ListAllMaterialConversions()
            → Logs detailed material info to console
```

### MenuItem Attributes

```csharp
[MenuItem("Tools/Galactic Crossing/Convert Materials to Curved World")]
public static void ShowWindow() { ... }

[MenuItem("Tools/Galactic Crossing/Batch Assign Materials to Scene")]
public static void BatchAssignMaterialsToScene() { ... }

[MenuItem("Tools/Galactic Crossing/Validate Scene Materials")]
public static void ValidateSceneMaterials() { ... }

[MenuItem("Tools/Galactic Crossing/List All Material Conversions")]
public static void ListAllMaterialConversions() { ... }
```

---

## Data Flow: Material Conversion Process

### Phase 1: Discovery
```
User Action: Open Conversion Window
       │
       ▼
┌──────────────────────────────────────────────┐
│ ShaderHelperUtility.FindMaterialsInDirectory │
├──────────────────────────────────────────────┤
│ Input:  "Assets/.../Loft3D/Materials"        │
│ Output: List<Material> (38 materials)        │
└──────────────────────────────────────────────┘
       │
       ▼
For Each Material:
  ├── ShaderHelperUtility.ShouldExcludeMaterial()
  │   └── Returns: true/false (exclude particles/VFX)
  │
  └── ShaderHelperUtility.GetMaterialPriority()
      └── Returns: "CRITICAL" | "HIGH" | "MEDIUM" | "LOW"
       │
       ▼
┌──────────────────────────────────────────────┐
│ MaterialConversionItem Created               │
├──────────────────────────────────────────────┤
│ - originalMaterial: Material                 │
│ - convertedMaterial: Material (if exists)    │
│ - isExcluded: bool                           │
│ - isConverted: bool                          │
│ - priority: string                           │
└──────────────────────────────────────────────┘
```

### Phase 2: Conversion
```
User Action: Click "Convert All Pending Materials"
       │
       ▼
For Each Non-Excluded, Non-Converted Material:
       │
       ▼
┌──────────────────────────────────────────────┐
│ ShaderHelperUtility.DuplicateMaterial()      │
├──────────────────────────────────────────────┤
│ Input:  LoftGroundMaterial.mat               │
│ Suffix: "_CurvedWorld"                       │
│ Action: AssetDatabase.CopyAsset()            │
│ Output: LoftGroundMaterial_CurvedWorld.mat   │
└──────────────────────────────────────────────┘
       │
       ▼
┌──────────────────────────────────────────────┐
│ ShaderHelperUtility.AssignShaderToMaterial() │
├──────────────────────────────────────────────┤
│ Primary:  "Shader Graphs/CurvedWorldLit"     │
│ Fallback: "Universal Render Pipeline/Lit"    │
│ Action:   material.shader = Shader.Find()    │
└──────────────────────────────────────────────┘
       │
       ▼
Update MaterialConversionItem:
  - isConverted = true
  - convertedMaterial = duplicated material
       │
       ▼
AssetDatabase.SaveAssets()
AssetDatabase.Refresh()
```

### Phase 3: Scene Assignment
```
User Action: Click "Batch Assign Materials to Scene"
       │
       ▼
┌──────────────────────────────────────────────┐
│ ShaderHelperUtility.FindAllRenderers()       │
├──────────────────────────────────────────────┤
│ Output: Renderer[] (all renderers in scene)  │
└──────────────────────────────────────────────┘
       │
       ▼
For Each Renderer:
  For Each Material Slot:
       │
       ▼
    Get Current Material Path
       │
       ▼
    Construct Curved World Path:
    path.Replace(".mat", "_CurvedWorld.mat")
       │
       ▼
    Load Curved World Material
       │
       ▼
    If Found:
      ┌──────────────────────────────────────────┐
      │ ShaderHelperUtility.                     │
      │   AssignMaterialToGameObject()           │
      ├──────────────────────────────────────────┤
      │ renderer.sharedMaterials[i] = curved     │
      └──────────────────────────────────────────┘
       │
       ▼
Undo.RecordObjects() (enable undo)
Display Completion Dialog
```

---

## Material Classification System

### Priority Calculation Algorithm

```python
def GetMaterialPriority(material_name):
    name_lower = material_name.lower()

    # CRITICAL - Ground materials
    if "ground" in name_lower:
        return "CRITICAL"

    # HIGH - Wall materials
    if "wall" in name_lower:
        return "HIGH"

    # MEDIUM - Furniture/Props
    if any(keyword in name_lower for keyword in
           ["furniture", "couch", "lamp", "metal",
            "plant", "screen", "wood"]):
        return "MEDIUM"

    # Default
    return "LOW"
```

### Exclusion Algorithm

```python
def ShouldExcludeMaterial(material_name):
    name_lower = material_name.lower()

    exclusion_keywords = [
        "particle", "explosion", "smoke", "reticle",
        "ui", "blood", "click", "walk", "remnant",
        "vfx", "effect"
    ]

    for keyword in exclusion_keywords:
        if keyword in name_lower:
            return True  # Exclude this material

    return False  # Include this material
```

---

## Error Handling and Validation

### Null Safety
```csharp
// All methods perform null checks
if (material == null)
{
    Debug.LogError("Cannot process null material");
    return null;
}
```

### Path Validation
```csharp
// Verify directory exists before searching
if (!AssetDatabase.IsValidFolder(path))
{
    Debug.LogError($"Directory not found: {path}");
    return new List<Material>();
}
```

### Shader Fallback
```csharp
// Primary shader attempt
Shader shader = Shader.Find(shaderName);

if (shader == null && fallbackShaderName != null)
{
    // Try fallback
    shader = Shader.Find(fallbackShaderName);
    Debug.LogWarning($"Using fallback shader: {fallbackShaderName}");
}
```

### Prefab Override Detection
```csharp
PrefabInstanceStatus status = PrefabUtility.GetPrefabInstanceStatus(gameObject);

if (status == PrefabInstanceStatus.Connected)
{
    Debug.LogWarning("Material assignment will create prefab override");
}
```

---

## Logging System

### Log Levels Used

```
DEBUG.LOG    → Informational messages (successful operations)
DEBUG.LOGWARNING → Non-critical issues (shader not found, prefab override)
DEBUG.LOGERROR   → Critical failures (null reference, file not found)
```

### Example Log Output

```
[ShaderHelper] Found 38 materials in Assets/TopDownEngine/Demos/Loft3D/Materials
[ShaderHelper] Excluding material 'LoftBlood' (contains keyword: blood)
[ShaderHelper] Successfully duplicated: LoftGroundMaterial -> LoftGroundMaterial_CurvedWorld
[ShaderHelper] Assigned shader 'Universal Render Pipeline/Lit' to material 'LoftGroundMaterial_CurvedWorld'
[MaterialConversion] Successfully converted: LoftGroundMaterial (CRITICAL priority)
[MaterialConversion] Batch conversion complete: 31 converted, 0 failed
```

---

## Undo System Integration

### Undo Registration
```csharp
// Before modifying materials
Undo.RecordObject(material, "Convert Material to Curved World");

// Before modifying scene objects
Undo.RecordObjects(renderers, "Batch Assign Curved World Materials");
```

### Undo Behavior
- Material duplication: Cannot undo (creates new asset)
- Shader assignment: Can undo material changes
- Scene assignment: Can undo renderer changes with Ctrl+Z

---

## Performance Characteristics

### Time Complexity

| Operation | Complexity | Notes |
|-----------|-----------|-------|
| Find Materials | O(n) | n = files in directory |
| Duplicate Material | O(1) | Single file copy |
| Assign Shader | O(1) | Property assignment |
| Scene Assignment | O(m×k) | m = renderers, k = material slots |

### Expected Performance

- **Material Discovery:** ~100ms (38 materials)
- **Single Conversion:** ~50ms per material
- **Batch Conversion:** ~2 seconds (31 materials)
- **Scene Assignment:** ~500ms (typical Loft3D scene)

### Memory Usage

- **Editor Window:** ~2 MB (material references + UI)
- **Material Assets:** ~2 KB per material
- **Total Impact:** Negligible (<5 MB)

---

## Extension Points

### Adding Custom Exclusion Rules

```csharp
// In ShaderHelperUtility.cs
public static bool ShouldExcludeMaterial(Material material)
{
    string[] exclusionKeywords = new string[]
    {
        // ... existing keywords ...
        "custom_keyword_here"  // Add your keyword
    };
}
```

### Adding Custom Priority Levels

```csharp
// In ShaderHelperUtility.cs
public static string GetMaterialPriority(Material material)
{
    string materialName = material.name.ToLower();

    // Add custom priority logic
    if (materialName.Contains("your_keyword"))
    {
        return "CRITICAL";
    }
}
```

### Custom Post-Conversion Actions

```csharp
// In MaterialConversionAutomation.cs
private void ConvertSingleMaterial(MaterialConversionItem item)
{
    // ... existing conversion code ...

    // Add custom logic here
    if (duplicatedMaterial.name.Contains("Ground"))
    {
        // Set custom properties for ground materials
        duplicatedMaterial.SetFloat("_CurveAmount", 0.01f);
    }
}
```

---

## Dependencies

### Unity Packages Required
- **UnityEngine** - Core Unity engine
- **UnityEditor** - Editor scripting API
- **System.Collections.Generic** - List, Dictionary
- **System.Linq** - LINQ queries
- **System.IO** - File path operations

### TopDown Engine Integration
- No TDE dependencies (standalone tool)
- Works with any Unity project structure
- Only requires materials to exist in target directory

### URP Integration
- Fallback shader: "Universal Render Pipeline/Lit"
- Compatible with URP material properties
- Works with Shader Graph shaders

---

## Testing Strategy

### Manual Testing Checklist

- [ ] Window opens from menu
- [ ] Materials list populates
- [ ] Priority labels correct (CRITICAL/HIGH/MEDIUM)
- [ ] Excluded materials identified correctly
- [ ] Single material conversion works
- [ ] Batch conversion works
- [ ] Shader assignment (when shader exists)
- [ ] Fallback shader assignment (when shader missing)
- [ ] Scene assignment works
- [ ] Validation report accurate
- [ ] Undo works for scene assignment
- [ ] Console logs are informative

### Edge Cases to Test

1. **Empty materials directory** → Should show "0 materials found"
2. **Shader not found** → Should use fallback, log warning
3. **Material already converted** → Should skip, mark as converted
4. **Prefab instance modification** → Should work, log warning
5. **Missing material references** → Should handle gracefully

---

## Code Metrics

| Metric | Value |
|--------|-------|
| Total Lines of Code | 902 |
| ShaderHelperUtility.cs | 384 lines |
| MaterialConversionAutomation.cs | 518 lines |
| Public Methods | 15 |
| Private Methods | 8 |
| MenuItem Registrations | 4 |
| Documentation Comments | 45+ |

---

**Document Version:** 1.0
**Last Updated:** 2026-02-15
**Architecture Tier:** Editor Tools
**Complexity Level:** Medium
