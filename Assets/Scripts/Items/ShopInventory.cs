using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ScriptableObject that defines a shop's stock and Stardust prices.
/// Create via Assets menu: Create > CosmicColony > Shop Inventory
/// </summary>
[CreateAssetMenu(menuName = "CosmicColony/Shop Inventory")]
public class ShopInventory : ScriptableObject
{
    [System.Serializable]
    public struct ShopEntry
    {
        [Tooltip("The item available for purchase.")]
        public CosmicItem Item;

        [Tooltip("Cost in Stardust to buy this item.")]
        public int StardustBuyPrice;

        [Tooltip("Stardust earned when selling this item. 0 = not sellable.")]
        public int StardustSellPrice;
    }

    [Tooltip("All items this shop sells and their prices.")]
    public List<ShopEntry> Stock = new List<ShopEntry>();

    /// <summary>
    /// Look up a shop entry by item ID. Returns null if not in stock.
    /// </summary>
    public ShopEntry? GetEntry(string itemID)
    {
        foreach (var entry in Stock)
        {
            if (entry.Item != null && entry.Item.ItemID == itemID)
                return entry;
        }
        return null;
    }
}
