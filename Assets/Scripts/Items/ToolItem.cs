using UnityEngine;
using MoreMountains.InventoryEngine;

/// <summary>
/// What the tool does when used on a grid cell (Feature 006).
/// </summary>
public enum ToolActionType
{
    None,
    Chop,   // Destroys Flora-tagged occupant → drops resource picker
    Mine,   // Destroys Rock-tagged occupant → drops resource picker
    Dig,    // Changes cell TerrainType to Water
    Water,  // Waters a Seed cell (future farming use)
    Fish    // Initiates fishing on a Water cell (future)
}

/// <summary>
/// Tool item that equips/unequips via ToolController and executes grid actions.
/// AnimatorControllerOverridePath reserved for tool-specific animations.
/// </summary>
[CreateAssetMenu(menuName = "CosmicColony/Items/Tool")]
public class ToolItem : CosmicItem
{
    [Header("Tool")]
    [Tooltip("What this tool does when used on a grid cell.")]
    public ToolActionType ActionType = ToolActionType.None;

    [Tooltip("Prefab to drop when this tool successfully acts on a cell (e.g. berry picker, ferrite picker).")]
    public GameObject DropPrefab;

    [Tooltip("Path to AnimatorOverrideController for tool animations")]
    public string AnimatorControllerOverridePath;

    [Tooltip("Icon shown in the hotbar when this tool is selected")]
    public Sprite HotbarIcon;

    private void Reset()
    {
        ItemType = CosmicItemType.Tool;
        Usable = false;
        Consumable = false;
        Droppable = false;
        MaximumStack = 1;
    }

    /// <summary>
    /// Activates the tool via ToolController on the player.
    /// Feature 006 will add grid-action handling inside ToolController.
    /// </summary>
    public override bool Equip(string playerID)
    {
        ToolController tc = FindToolController();
        if (tc != null)
        {
            tc.EquipTool(this);
        }
        else
        {
            Debug.LogWarning("[ToolItem] No ToolController found in scene.");
        }
        return true;
    }

    public override bool UnEquip(string playerID)
    {
        ToolController tc = FindToolController();
        if (tc != null)
        {
            tc.UnequipTool();
        }
        return true;
    }

    private ToolController FindToolController()
    {
        return Object.FindObjectOfType<ToolController>();
    }
}
