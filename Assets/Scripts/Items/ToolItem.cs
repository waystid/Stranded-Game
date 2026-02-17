using UnityEngine;
using MoreMountains.InventoryEngine;

/// <summary>
/// Tool item that equips/unequips via ToolController.
/// Feature 006 (Tool Actions) will add grid-action behaviour.
/// AnimatorControllerOverridePath reserved for tool-specific animations.
/// </summary>
[CreateAssetMenu(menuName = "CosmicColony/Items/Tool")]
public class ToolItem : CosmicItem
{
    [Header("Tool")]
    [Tooltip("Path to AnimatorOverrideController for tool animations (Feature 006)")]
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
