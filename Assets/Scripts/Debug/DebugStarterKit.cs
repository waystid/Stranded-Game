using UnityEngine;
using MoreMountains.InventoryEngine;

/// <summary>
/// Debug-only scene GO. On Start (after player spawns), adds Stardust + test tools
/// directly into the player's inventories so the full feature stack can be tested
/// without manually picking up items.
///
/// Remove from scene before shipping.
/// </summary>
public class DebugStarterKit : MonoBehaviour
{
    [Header("Items to inject")]
    public InventoryItem StardustItem;
    public InventoryItem PlasmaCutterItem;
    public InventoryItem MineralExtractorItem;

    [Header("Settings")]
    public string PlayerID = "Player1";
    public string MainInventoryName = "PlayerMainInventory";
    public string HotbarInventoryName = "PlayerHotbarInventory";
    public int StardustAmount = 100;

    [Tooltip("How many seconds to wait after Start before injecting (lets LevelManager spawn the player).")]
    public float InjectDelay = 1f;

    private bool _injected = false;

    void Start()
    {
        Invoke(nameof(InjectItems), InjectDelay);
    }

    void InjectItems()
    {
        if (_injected) return;

        Inventory main   = Inventory.FindInventory(MainInventoryName,   PlayerID);
        Inventory hotbar = Inventory.FindInventory(HotbarInventoryName, PlayerID);

        if (main == null)
        {
            Debug.LogWarning("[DebugStarterKit] Could not find main inventory. Is the player spawned?");
            return;
        }

        // Add Stardust to main inventory
        if (StardustItem != null)
        {
            main.AddItem(StardustItem, StardustAmount);
            Debug.Log($"[DebugStarterKit] Added {StardustAmount}× {StardustItem.ItemName} to {MainInventoryName}.");
        }

        // Add tools to hotbar (slots 0 and 1)
        if (hotbar != null)
        {
            if (PlasmaCutterItem != null)
            {
                hotbar.AddItem(PlasmaCutterItem, 1);
                Debug.Log($"[DebugStarterKit] Added {PlasmaCutterItem.ItemName} to {HotbarInventoryName}.");
            }
            if (MineralExtractorItem != null)
            {
                hotbar.AddItem(MineralExtractorItem, 1);
                Debug.Log($"[DebugStarterKit] Added {MineralExtractorItem.ItemName} to {HotbarInventoryName}.");
            }
        }
        else
        {
            Debug.LogWarning("[DebugStarterKit] Could not find hotbar inventory.");
        }

        _injected = true;
    }
}
