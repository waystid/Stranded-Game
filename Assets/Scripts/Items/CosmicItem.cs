using UnityEngine;
using MoreMountains.InventoryEngine;

/// <summary>
/// Cosmic Colony item type enum — covers all current and planned item categories.
/// Blueprint, Seed, Key reserved for future features.
/// </summary>
public enum CosmicItemType
{
    Resource,
    Consumable,
    Tool,
    Furniture,
    Currency,
    Blueprint,
    Seed,
    Key
}

/// <summary>
/// Base class for all Cosmic Colony items. Extends TDE InventoryItem with
/// game-specific metadata (type, grid footprint for furniture/buildings).
/// 
/// Use TDE's inherited Prefab field as the drop picker prefab.
/// Use MaximumStack for stacking limits.
/// </summary>
public class CosmicItem : InventoryItem
{
    [Header("Cosmic Colony")]
    [Tooltip("Category of this item — drives behaviour and UI representation")]
    public CosmicItemType ItemType;

    [Tooltip("Grid cells this item occupies when placed. Default {(0,0)} = 1x1. " +
             "For future multi-tile furniture/buildings (Feature 005).")]
    public Vector2Int[] GridFootprint = new Vector2Int[] { Vector2Int.zero };
}
