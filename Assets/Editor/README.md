# Galactic Crossing - Editor Automation Scripts

This folder contains Unity Editor scripts that automate the setup of the Galactic Crossing MVP.

## Quick Start

1. Open your Unity project
2. Go to **Tools > Galactic Crossing > Setup MVP**
3. Click "Yes, Setup MVP" in the dialog
4. Wait 2-3 minutes while the scripts run
5. Press Play to test!

## Scripts Overview

### 1. GalacticCrossingSetup.cs
**Main menu interface for all automation operations**

**Menu Items:**
- `Tools > Galactic Crossing > Setup MVP` - Runs complete setup
- `Tools > Galactic Crossing > 1. Scene Setup Only` - Only sets up scene
- `Tools > Galactic Crossing > 2. Create Assets Only` - Only creates assets
- `Tools > Galactic Crossing > 3. Configure Scene Objects` - Only configures objects
- `Tools > Galactic Crossing > Validate Setup` - Opens validation window
- `Tools > Galactic Crossing > About` - Shows info window

**Features:**
- Progress bar showing setup steps
- Logging for each operation
- Error handling and recovery
- Validation of setup completion

### 2. SceneSetupAutomation.cs
**Handles scene manipulation and cleanup**

**Operations:**
- Duplicates Loft3D scene to `Assets/Scenes/GalacticCrossingMVP.unity`
- Removes all AI enemies (searches by `AIBrain` and `Character` components)
- Removes all weapon pickups and weapon-related objects
- Adds `GridManager` GameObject with proper configuration
- Configures player character:
  - Removes `CharacterHandleWeapon` component
  - Adds `CharacterGridMovement` component
  - Configures movement for "weighted" Animal Crossing-style feel
- Configures camera for "Rolling Log" view

**Key Methods:**
- `SetupScene()` - Runs all scene setup operations
- `RemoveAIEnemies()` - Finds and removes AI-controlled characters
- `RemoveWeapons()` - Finds and removes weapon-related objects
- `AddGridManager()` - Creates and configures GridManager
- `ConfigurePlayerCharacter()` - Modifies player components
- `ConfigureCameraSettings()` - Adjusts Cinemachine camera

### 3. AssetCreationAutomation.cs
**Creates ScriptableObjects and prefabs**

**Creates:**

#### Items (ScriptableObjects)
- **ScrapMetal** - Basic crafting resource
  - Location: `Assets/Resources/Items/ScrapMetal.asset`
  - Stackable (max 30)
  - Cannot be used directly (crafting only)

- **EnergyCrystal** - Energy resource
  - Location: `Assets/Resources/Items/EnergyCrystal.asset`
  - Stackable (max 30)
  - Cannot be used directly (crafting only)

- **AlienBerry** - Consumable health item
  - Location: `Assets/Resources/Items/AlienBerry.asset`
  - Stackable (max 10)
  - Usable (restores stamina)
  - Uses `AlienBerryItem.cs` class if available

#### Prefabs
- **ScrapMetalPicker** - Pickup for scrap metal
  - Location: `Assets/Prefabs/ScrapMetalPicker.prefab`
  - Has `ItemPicker` component
  - Trigger collider configured

- **EnergyCrystalPicker** - Pickup for energy crystals
  - Location: `Assets/Prefabs/EnergyCrystalPicker.prefab`
  - Has `ItemPicker` component
  - Trigger collider configured

- **AlienBerryPicker** - Pickup for alien berries
  - Location: `Assets/Prefabs/AlienBerryPicker.prefab`
  - Has `ItemPicker` component
  - Trigger collider configured

- **AlienTree** - Interactive tree that drops berries
  - Location: `Assets/Prefabs/AlienTree.prefab`
  - Has `TreeShakeZone` component (if class exists)
  - Drops AlienBerry items when shaken

- **GAIA_NPC** - G.A.I.A. hologram NPC
  - Location: `Assets/Prefabs/GAIA_NPC.prefab`
  - Has `DialogueZone` component
  - Semi-transparent holographic appearance
  - Trigger collider for interaction zone

**Key Methods:**
- `CreateAllAssets()` - Creates all items and prefabs
- `CreateScrapMetalItem()` - Creates ScrapMetal ScriptableObject
- `CreateEnergyCrystalItem()` - Creates EnergyCrystal ScriptableObject
- `CreateAlienBerryItem()` - Creates AlienBerry ScriptableObject
- `CreateItemPickerPrefabs()` - Creates all ItemPicker prefabs
- `CreateTreePrefab()` - Creates AlienTree prefab
- `CreateGAIAPrefab()` - Creates GAIA_NPC prefab

### 4. ComponentConfigurationHelper.cs
**Utility methods for component manipulation**

**Component Management:**
- `AddComponentSafe<T>()` - Adds component only if it doesn't exist
- `RemoveComponentSafe<T>()` - Removes component if it exists
- `RemoveAllComponents<T>()` - Removes all components of type

**Field Configuration:**
- `SetComponentField()` - Sets field value using reflection
- `GetComponentField()` - Gets field value using reflection
- `AssignReference()` - Assigns object reference to field
- `FindAndAssignReference<T>()` - Finds and assigns component reference

**Collider Setup:**
- `ConfigureBoxCollider()` - Configures BoxCollider with parameters
- `ConfigureSphereCollider()` - Configures SphereCollider with parameters

**Layer and Tag:**
- `SetLayer()` - Sets GameObject layer (optionally including children)
- `SetTag()` - Sets GameObject tag

**Prefab Utilities:**
- `CreatePrefabVariant()` - Creates prefab from GameObject
- `InstantiatePrefab()` - Instantiates prefab in scene

**Validation:**
- `ValidateComponents()` - Checks if required components exist
- `LogComponentStructure()` - Logs all components on GameObject

## Setup Process Details

### Complete Setup Flow

1. **Scene Creation**
   - Creates `Assets/Scenes` folder if needed
   - Duplicates `Loft3D.unity` to `GalacticCrossingMVP.unity`
   - Opens the new scene

2. **Scene Cleanup**
   - Finds all AI enemies (via `AIBrain` component)
   - Finds enemy characters (via `Character` component with `CharacterType.AI`)
   - Removes all identified enemies
   - Finds weapon pickups and related objects
   - Removes all weapons

3. **Asset Creation**
   - Creates directory structure:
     - `Assets/Resources/Items/`
     - `Assets/Prefabs/`
     - `Assets/GalacticCrossing/`
   - Creates 3 item ScriptableObjects
   - Creates 5 prefabs (3 pickers, 1 tree, 1 NPC)

4. **Scene Configuration**
   - Adds GridManager GameObject
   - Configures GridManager settings:
     - GridUnitSize: 1.0
     - DebugGridSize: 30
     - DrawDebugGrid: true
   - Finds player character
   - Removes weapon-handling components
   - Adds grid movement component
   - Configures camera FOV and position

5. **Finalization**
   - Saves all scenes
   - Saves all assets
   - Refreshes Asset Database
   - Shows completion dialog

## Idempotency

All scripts are designed to be **idempotent** - safe to run multiple times:

- Won't create duplicate assets (checks if they exist first)
- Won't add duplicate components (uses safe add methods)
- Won't remove non-existent objects (checks before removal)
- Shows warnings for already-existing items

## Error Handling

Each script includes:
- Null reference checks
- Try-catch blocks for critical operations
- Clear error messages in Console
- Progress bar cleanup on error
- User-friendly error dialogs

## Manual Steps After Setup

After running the automated setup, you may need to:

1. **Assign Custom Models**
   - Replace primitive shapes in prefabs with actual 3D models
   - Update materials and textures

2. **Configure Input System**
   - Go to Project Settings > Input System
   - Verify button mappings match your control scheme

3. **Adjust GridManager**
   - Fine-tune GridUnitSize based on your level design
   - Position GridOrigin at desired world location

4. **Place Objects in Scene**
   - Drag prefabs from Assets/Prefabs into scene
   - Position GAIA_NPC, trees, and resource pickups

5. **Configure Dialogue**
   - Edit GAIA_NPC dialogue text in DialogueZone component
   - Add quest tracking logic if needed

6. **Test Movement**
   - Enter Play mode
   - Verify grid movement works correctly
   - Adjust movement parameters if needed

## Troubleshooting

### "Scene not found" error
- Ensure TopDown Engine is properly imported
- Verify Loft3D demo scene exists at expected path

### "Component not found" warning
- Some components depend on custom scripts (TreeShakeZone, AlienBerryItem)
- Create these scripts first or manually configure later

### Missing references in prefabs
- Run "Configure Scene Objects" again
- Manually assign references in Inspector

### Camera not configured
- Cinemachine must be installed and set up
- Manually adjust camera in Play mode for best view

### Player movement issues
- Verify GridManager is in scene
- Check CharacterGridMovement component is enabled
- Ensure no conflicting movement components exist

## Extending the Scripts

### Adding New Items
```csharp
private static void CreateMyNewItem()
{
    string assetPath = $"{ITEMS_PATH}/MyItem.asset";

    var item = ScriptableObject.CreateInstance<InventoryItem>();
    item.ItemID = "MyItem";
    item.ItemName = "My Item";
    item.ItemDescription = "Description here";
    item.IsStackable = true;
    item.MaximumStack = 20;

    AssetDatabase.CreateAsset(item, assetPath);
}
```

### Adding New Prefabs
```csharp
private static void CreateMyPrefab()
{
    GameObject obj = new GameObject("MyPrefab");
    // Add components...

    string prefabPath = $"{PREFABS_PATH}/MyPrefab.prefab";
    PrefabUtility.SaveAsPrefabAsset(obj, prefabPath);
    Object.DestroyImmediate(obj);
}
```

### Adding Menu Items
```csharp
[MenuItem(MENU_PATH + "My Custom Action", false, 150)]
public static void MyCustomAction()
{
    // Your code here
}
```

## Best Practices

1. **Always save before running setup**
   - Backup your project
   - Commit to version control

2. **Run individual steps for debugging**
   - Use the numbered menu items
   - Easier to identify issues

3. **Check Console for logs**
   - All operations are logged
   - Warnings indicate potential issues

4. **Validate after setup**
   - Use "Validate Setup" menu item
   - Manually test in Play mode

5. **Don't modify during execution**
   - Wait for progress bar to complete
   - Don't enter Play mode while running

## Technical Notes

### Namespaces
All scripts use the `GalacticCrossing.Editor` namespace to avoid conflicts.

### Dependencies
- UnityEngine
- UnityEditor
- UnityEditor.SceneManagement
- MoreMountains.TopDownEngine
- MoreMountains.Tools
- MoreMountains.InventoryEngine
- Cinemachine (for camera configuration)

### Reflection Usage
`ComponentConfigurationHelper` uses reflection for flexible component configuration. This allows setting private fields and working with unknown component types.

### Asset Database
All asset operations use `AssetDatabase` for proper Unity integration:
- `AssetDatabase.CreateAsset()` - Create new assets
- `AssetDatabase.SaveAssets()` - Save all pending changes
- `AssetDatabase.Refresh()` - Refresh project browser
- `AssetDatabase.LoadAssetAtPath()` - Load existing assets

## Version History

### Version 1.0 (Current)
- Initial implementation
- Complete scene setup automation
- Asset creation for MVP items
- Component configuration utilities
- Full error handling and validation

## Support

For issues or questions:
1. Check Console logs for detailed error messages
2. Review this README for troubleshooting steps
3. Verify all dependencies are installed
4. Try running individual setup steps

## License

Part of the Galactic Crossing project. See main project license.
