using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Central registry of all Cosmic Colony items. Provides ID-based lookup
/// and type-filtered queries used by crafting, shop, and quest systems.
/// 
/// Create one instance: Assets/Data/ItemRegistry.asset
/// Call Init() on game start, or it auto-initialises on first Get() call.
/// </summary>
[CreateAssetMenu(menuName = "CosmicColony/ItemRegistry")]
public class ItemRegistry : ScriptableObject
{
    [Tooltip("All items in the game — populate this list in the inspector")]
    public List<CosmicItem> Items = new List<CosmicItem>();

    private Dictionary<string, CosmicItem> _lookup;

    /// <summary>Build the ID lookup dictionary. Called automatically if needed.</summary>
    public void Init()
    {
        _lookup = Items.ToDictionary(i => i.ItemID);
    }

    /// <summary>Get an item by its ItemID. Returns null if not found.</summary>
    public CosmicItem Get(string id)
    {
        if (_lookup == null) Init();
        return _lookup.TryGetValue(id, out var v) ? v : null;
    }

    /// <summary>Get all items of a given type (e.g. all tools, all resources).</summary>
    public IEnumerable<CosmicItem> GetByType(CosmicItemType type)
    {
        return Items.Where(i => i.ItemType == type);
    }
}
