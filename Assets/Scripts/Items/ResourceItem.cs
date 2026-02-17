using UnityEngine;

/// <summary>
/// Stackable raw material item (ore, crystals, scrap, stardust currency).
/// No overrides needed — InventoryItem base handles pick/drop/stack.
/// Configure MaximumStack in the SO asset inspector.
/// </summary>
[CreateAssetMenu(menuName = "CosmicColony/Items/Resource")]
public class ResourceItem : CosmicItem
{
    private void Reset()
    {
        ItemType = CosmicItemType.Resource;
        Consumable = false;
        Droppable = true;
    }
}
