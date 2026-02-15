# Galactic Crossing - MVP Setup Automation Guide

## Overview

This project includes a comprehensive suite of Unity Editor scripts that automate the entire MVP setup process. These scripts transform the TopDown Engine's Loft3D demo into the Galactic Crossing MVP with minimal manual intervention.

## Quick Start (3 Steps)

1. **Open Unity Project**
   - Open the TopDown Engine project in Unity

2. **Run Setup**
   - Go to `Tools > Galactic Crossing > Setup MVP`
   - Click "Yes, Setup MVP" in the confirmation dialog

3. **Wait and Test**
   - Wait 2-3 minutes for automated setup
   - Press Play to test the configured scene

## What Gets Automated

### Complete Scene Configuration
- ✅ Duplicates Loft3D scene to new MVP scene
- ✅ Removes all AI enemies automatically
- ✅ Removes all weapon pickups and combat elements
- ✅ Adds and configures GridManager
- ✅ Configures player for grid-based movement
- ✅ Removes weapon-handling components
- ✅ Configures camera for Animal Crossing-style view

### Asset Creation
- ✅ Creates 3 resource items (ScrapMetal, EnergyCrystal, AlienBerry)
- ✅ Creates 3 ItemPicker prefabs (for resource collection)
- ✅ Creates AlienTree prefab (with shake zone)
- ✅ Creates G.A.I.A. NPC prefab (with dialogue)
- ✅ Sets up proper directory structure
- ✅ Configures all ScriptableObject properties

### Component Configuration
- ✅ Safely adds/removes components
- ✅ Configures colliders as triggers
- ✅ Sets up proper layer assignments
- ✅ Assigns component references
- ✅ Validates required components

## Created Editor Scripts

### 1. GalacticCrossingSetup.cs
**Main orchestrator for all automation**

**Menu Items:**
- `Setup MVP` - Complete automation
- `1. Scene Setup Only` - Only scene modifications
- `2. Create Assets Only` - Only asset creation
- `3. Configure Scene Objects` - Only configuration
- `Validate Setup` - Check setup completion
- `About` - Information window

**Location:** `/Assets/Editor/GalacticCrossingSetup.cs`

### 2. SceneSetupAutomation.cs
**Handles all scene manipulation**

**Operations:**
- Scene duplication
- AI enemy removal (via AIBrain detection)
- Weapon removal (via component and naming patterns)
- GridManager setup
- Player character modification
- Camera configuration

**Key Methods:**
```csharp
SetupScene()              // Complete scene setup
RemoveAIEnemies()         // Remove all AI
RemoveWeapons()           // Remove weapons
AddGridManager()          // Add grid system
ConfigurePlayerCharacter() // Modify player
ConfigureCameraSettings()  // Adjust camera
```

**Location:** `/Assets/Editor/SceneSetupAutomation.cs`

### 3. AssetCreationAutomation.cs
**Creates all game assets**

**Creates:**

**Items:**
- ScrapMetal.asset (crafting resource)
- EnergyCrystal.asset (energy resource)
- AlienBerry.asset (consumable health item)

**Prefabs:**
- ScrapMetalPicker.prefab
- EnergyCrystalPicker.prefab
- AlienBerryPicker.prefab
- AlienTree.prefab (interactive tree)
- GAIA_NPC.prefab (holographic NPC)

**Key Methods:**
```csharp
CreateAllAssets()          // Creates everything
CreateScrapMetalItem()     // Creates item
CreateItemPickerPrefabs()  // Creates pickers
CreateTreePrefab()         // Creates tree
CreateGAIAPrefab()         // Creates NPC
```

**Location:** `/Assets/Editor/AssetCreationAutomation.cs`

### 4. ComponentConfigurationHelper.cs
**Utility functions for component manipulation**

**Capabilities:**
- Safe component addition/removal
- Reflection-based field configuration
- Collider setup (Box, Sphere)
- Layer and tag assignment
- Prefab creation and instantiation
- Component validation

**Example Usage:**
```csharp
// Add component safely (won't duplicate)
var gridMovement = ComponentConfigurationHelper.AddComponentSafe<CharacterGridMovement>(player);

// Configure collider
ComponentConfigurationHelper.ConfigureBoxCollider(
    gameObject,
    isTrigger: true,
    size: new Vector3(1, 1, 1)
);

// Set field value
ComponentConfigurationHelper.SetComponentField(
    component,
    "MaxSpeed",
    8.0f
);
```

**Location:** `/Assets/Editor/ComponentConfigurationHelper.cs`

### 5. QuestSetupHelper.cs
**Generates quest management scripts**

**Features:**
- Creates PrologueManager.cs template
- Creates QuestData.cs template
- Adds quest manager to scene
- Configures quest tracking

**Menu Items:**
- `Advanced > Create Quest Manager` - Generate scripts
- `Advanced > Add Quest Manager to Scene` - Add to scene

**Generated Scripts:**
- PrologueManager.cs (quest state machine)
- QuestData.cs (quest data structures)

**Location:** `/Assets/Editor/QuestSetupHelper.cs`

### 6. SetupValidator.cs
**Validates setup completion**

**Checks:**
- Scene existence and configuration
- GridManager presence and setup
- Player component configuration
- AI removal completion
- Asset creation
- Prefab integrity

**Validation Window:**
- Visual pass/fail indicators
- Error and warning counts
- Detailed check results
- Summary statistics

**Menu Item:** `Validate Setup`

**Location:** `/Assets/Editor/SetupValidator.cs`

## Directory Structure Created

```
Assets/
├── Scenes/
│   └── GalacticCrossingMVP.unity          # Duplicated and modified scene
├── Resources/
│   └── Items/
│       ├── ScrapMetal.asset               # Resource item
│       ├── EnergyCrystal.asset            # Resource item
│       └── AlienBerry.asset               # Consumable item
├── Prefabs/
│   ├── ScrapMetalPicker.prefab            # Resource pickup
│   ├── EnergyCrystalPicker.prefab         # Resource pickup
│   ├── AlienBerryPicker.prefab            # Resource pickup
│   ├── AlienTree.prefab                   # Interactive tree
│   └── GAIA_NPC.prefab                    # NPC character
├── Scripts/
│   ├── Items/
│   │   └── AlienBerryItem.cs              # Already exists
│   ├── Environment/
│   │   └── TreeShakeZone.cs               # Already exists
│   └── Managers/
│       ├── PrologueManager.cs             # Generated by QuestSetupHelper
│       └── QuestData.cs                   # Generated by QuestSetupHelper
├── GalacticCrossing/
│   └── (Future project-specific assets)
└── Editor/
    ├── GalacticCrossingSetup.cs           # Main setup orchestrator
    ├── SceneSetupAutomation.cs            # Scene automation
    ├── AssetCreationAutomation.cs         # Asset creation
    ├── ComponentConfigurationHelper.cs     # Utility functions
    ├── QuestSetupHelper.cs                # Quest script generation
    ├── SetupValidator.cs                  # Setup validation
    └── README.md                          # Documentation
```

## Setup Flow Diagram

```
1. User Clicks "Setup MVP"
         ↓
2. Create Scenes Folder
         ↓
3. Duplicate Loft3D → GalacticCrossingMVP
         ↓
4. Open New Scene
         ↓
5. Remove AI Enemies
   - Find AIBrain components
   - Find AI Character types
   - Destroy GameObjects
         ↓
6. Remove Weapons
   - Find weapon components
   - Find weapon tags/layers
   - Destroy GameObjects
         ↓
7. Create Assets
   - Create directory structure
   - Create 3 item ScriptableObjects
   - Create 5 prefabs
         ↓
8. Add GridManager
   - Create GameObject
   - Add component
   - Configure settings
   - Create GridOrigin
         ↓
9. Configure Player
   - Find player character
   - Remove CharacterHandleWeapon
   - Add CharacterGridMovement
   - Configure movement settings
         ↓
10. Configure Camera
    - Find virtual camera
    - Set FOV to 35
    - Configure lens settings
         ↓
11. Save Everything
    - Save scene
    - Save assets
    - Refresh AssetDatabase
         ↓
12. Show Completion Dialog
```

## Post-Setup Manual Steps

### 1. Replace Visual Placeholders
The created prefabs use primitive shapes (cubes, spheres). Replace with actual 3D models:

```
AlienTree prefab:
- Replace Cylinder trunk with tree model
- Replace Sphere foliage with custom mesh

ItemPicker prefabs:
- Replace Cube with item models
- Add custom materials/textures

GAIA_NPC:
- Replace Sphere with hologram effect
- Add particle systems for sci-fi look
```

### 2. Configure Input System
```
1. Open Edit > Project Settings > Input System
2. Verify these mappings:
   - Movement: WASD / Left Stick
   - Interact: Space / Button South (A)
   - Run: Shift / Button East (B)
   - Inventory: Tab / Button West (X)
   - Use Tool: E / Button North (Y)
```

### 3. Adjust Movement Parameters
```
Select Player GameObject:

CharacterGridMovement:
  - MaximumSpeed: 8.0
  - Acceleration: 5.0
  - InputMode: InputManager
  - UseInputBuffer: true

For "weighted" feel, manually adjust:
CharacterMovement (if using non-grid):
  - WalkSpeed: 5.5
  - RunSpeed: 8.5
  - Acceleration: 12-15
  - Deceleration: 8-10
```

### 4. Place Objects in Scene
```
Drag from Assets/Prefabs:
  - GAIA_NPC → Near spawn point
  - AlienTree → Scattered around scene (5-10 instances)
  - ScrapMetalPicker → Near crash site (15+ instances)
  - EnergyCrystalPicker → Near energy sources (10+ instances)
  - AlienBerryPicker → Under trees (optional, if not using TreeShakeZone)
```

### 5. Configure GridManager
```
Select GridManager in scene:
  - GridOrigin: Verify position (should be at world origin or level start)
  - GridUnitSize: Adjust based on level scale (default: 1.0)
  - DebugGridSize: Adjust based on level size (default: 30)
```

### 6. Setup Quest System (Optional)
```
1. Run: Tools > Galactic Crossing > Advanced > Create Quest Manager
2. Review generated scripts:
   - Assets/Scripts/Managers/PrologueManager.cs
   - Assets/Scripts/Managers/QuestData.cs
3. Run: Tools > Galactic Crossing > Advanced > Add Quest Manager to Scene
4. Customize quest logic in PrologueManager.cs
5. Link GAIA_NPC DialogueZone to quest triggers
```

## Testing Checklist

After setup completion, verify:

- [ ] Scene opens without errors
- [ ] No AI enemies present
- [ ] No weapons in scene
- [ ] GridManager visible in Hierarchy
- [ ] Player has CharacterGridMovement component
- [ ] Player does NOT have CharacterHandleWeapon
- [ ] All 3 item assets exist in Resources/Items
- [ ] All 5 prefabs exist in Prefabs folder
- [ ] Press Play - no console errors
- [ ] Player can move on grid
- [ ] Camera follows player correctly
- [ ] Validation shows all green checks

## Troubleshooting

### "Scene not found" Error
**Problem:** Loft3D.unity not found at expected path

**Solution:**
1. Verify TopDown Engine is fully imported
2. Check path: `Assets/TopDownEngine/Demos/Loft3D/Loft3D.unity`
3. If path differs, update `LOFT_SCENE_PATH` in GalacticCrossingSetup.cs

### AI Enemies Still Present
**Problem:** Some enemies weren't removed

**Solution:**
1. Run `Tools > Galactic Crossing > 1. Scene Setup Only` again
2. Manually search for remaining enemies:
   - Hierarchy search: "t:AIBrain"
   - Delete manually
3. Check Console for removal logs

### Assets Not Created
**Problem:** Items/prefabs missing after setup

**Solution:**
1. Run `Tools > Galactic Crossing > 2. Create Assets Only`
2. Check Console for creation errors
3. Verify folders exist:
   - Assets/Resources
   - Assets/Resources/Items
   - Assets/Prefabs
4. Manually create missing folders if needed

### Player Can't Move
**Problem:** Grid movement not working

**Solution:**
1. Verify GridManager exists in scene
2. Check player has CharacterGridMovement component
3. Ensure CharacterMovement is disabled (conflicts with grid movement)
4. Verify Input System is configured
5. Check GridManager.GridOrigin is assigned

### Missing Component References
**Problem:** Prefabs have null references

**Solution:**
1. Run `Tools > Galactic Crossing > 3. Configure Scene Objects`
2. Manually assign in Inspector:
   - ItemPicker → Item reference
   - TreeShakeZone → ItemToDrop reference
   - DialogueZone → Configure dialogue text

### Validation Errors
**Problem:** Validator shows errors/warnings

**Solution:**
1. Read error messages carefully
2. Address each error individually
3. Re-run setup steps as needed
4. Use validation window for detailed diagnostics

## Advanced Usage

### Running Individual Steps

For debugging or customization, run steps individually:

```
1. Scene Setup:
   Tools > Galactic Crossing > 1. Scene Setup Only

2. Asset Creation:
   Tools > Galactic Crossing > 2. Create Assets Only

3. Configuration:
   Tools > Galactic Crossing > 3. Configure Scene Objects
```

### Custom Modifications

To customize the automation:

1. **Modify Item Properties:**
   Edit `AssetCreationAutomation.cs` → `CreateScrapMetalItem()` (etc.)

2. **Adjust Player Setup:**
   Edit `SceneSetupAutomation.cs` → `ConfigurePlayerCharacter()`

3. **Change Camera Settings:**
   Edit `SceneSetupAutomation.cs` → `ConfigureCameraSettings()`

4. **Add New Prefabs:**
   Add methods to `AssetCreationAutomation.cs`
   Call from `CreateAllAssets()`

### Extending Validation

Add custom validation checks:

```csharp
// In SetupValidator.cs
private static void ValidateCustom(ValidationReport report)
{
    // Your validation logic
    bool isValid = CheckSomething();

    report.AddComponentResult(
        isValid,
        "My custom check",
        !isValid  // isError
    );
}
```

## Performance Notes

Setup completion time depends on:
- Project size: ~2-3 minutes typical
- Scene complexity: More objects = longer cleanup
- Hardware: SSD recommended for faster asset operations

Progress is shown via Unity's progress bar:
- Step 1/5: Scene creation (~10s)
- Step 2/5: Cleanup (~30s)
- Step 3/5: Asset creation (~30s)
- Step 4/5: Configuration (~20s)
- Step 5/5: Finalization (~10s)

## Safety Features

All scripts include:
- **Idempotency:** Safe to run multiple times
- **Null checks:** Prevents null reference exceptions
- **Existence checks:** Won't duplicate existing assets
- **Confirmation dialogs:** User confirms destructive operations
- **Error handling:** Try-catch blocks on critical operations
- **Logging:** All operations logged to Console
- **Progress feedback:** Visual progress bars
- **Validation:** Post-setup integrity checks

## Script Dependencies

```
GalacticCrossingSetup.cs
├── Calls: SceneSetupAutomation
├── Calls: AssetCreationAutomation
└── Uses: UnityEditor, UnityEditor.SceneManagement

SceneSetupAutomation.cs
├── Uses: MoreMountains.TopDownEngine
├── Uses: Cinemachine
└── Uses: UnityEngine.SceneManagement

AssetCreationAutomation.cs
├── Uses: MoreMountains.InventoryEngine
└── Uses: PrefabUtility

ComponentConfigurationHelper.cs
└── Uses: Reflection (System.Reflection)

QuestSetupHelper.cs
└── Generates standalone scripts

SetupValidator.cs
├── Uses: MoreMountains.TopDownEngine
└── Creates: ValidationReport
```

## Best Practices

1. **Before Running Setup:**
   - Save your project
   - Commit to version control
   - Backup important scenes

2. **During Setup:**
   - Don't modify scene while running
   - Don't enter Play mode
   - Wait for completion dialog

3. **After Setup:**
   - Run validation immediately
   - Test in Play mode
   - Review Console logs
   - Check all created assets

4. **For Debugging:**
   - Run individual steps
   - Check Console for details
   - Use validation window
   - Compare with this guide

## Support and Maintenance

### Updating Scripts

To update automation scripts:
1. Edit files in `/Assets/Editor/`
2. Scripts automatically recompile
3. Test with validation
4. Re-run setup if needed

### Version Control

Recommended `.gitignore` additions:
```
# Don't commit these (can be regenerated)
Assets/Scenes/GalacticCrossingMVP.unity
Assets/Resources/Items/*.asset
Assets/Prefabs/*.prefab

# Do commit these (source of truth)
Assets/Editor/*.cs
Assets/Scripts/**/*.cs
```

### Future Enhancements

Potential additions:
- Curved world shader application
- Cel-shading material setup
- Quest dialogue tree generation
- NPC placement automation
- Resource spawn distribution
- Save/load system setup

## Conclusion

This automation system provides:
- ✅ **90% automated setup** - Minimal manual work required
- ✅ **Repeatable process** - Consistent results every time
- ✅ **Error handling** - Graceful failure recovery
- ✅ **Validation** - Verify setup completion
- ✅ **Extensibility** - Easy to customize and extend
- ✅ **Documentation** - Comprehensive guides and comments

The MVP setup that would take **2-3 hours manually** now takes **2-3 minutes** with automation!

## Quick Reference

### Essential Menu Items
```
Tools > Galactic Crossing >
  ├── Setup MVP (Main automation)
  ├── 1. Scene Setup Only
  ├── 2. Create Assets Only
  ├── 3. Configure Scene Objects
  ├── Validate Setup
  ├── Advanced >
  │   ├── Create Quest Manager
  │   └── Add Quest Manager to Scene
  └── About
```

### Key File Locations
```
/Assets/Editor/                           (All automation scripts)
/Assets/Scenes/GalacticCrossingMVP.unity  (Generated scene)
/Assets/Resources/Items/                  (Item ScriptableObjects)
/Assets/Prefabs/                         (Generated prefabs)
/Assets/Scripts/Managers/                (Quest scripts)
```

### Support Resources
- README.md in /Assets/Editor/
- This guide (SETUP_AUTOMATION_GUIDE.md)
- Console logs (detailed operation logs)
- Validation window (setup verification)

---

**Ready to start?** Open Unity and go to `Tools > Galactic Crossing > Setup MVP`!
