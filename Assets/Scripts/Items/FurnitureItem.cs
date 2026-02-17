using UnityEngine;

/// <summary>
/// Furniture item that enters placement mode when used.
/// Feature 005 (Building/Placement) will implement PlacementController.
/// PlacedPrefab is the grid-placed world object; Footprint defines its tile size.
/// </summary>
[CreateAssetMenu(menuName = "CosmicColony/Items/Furniture")]
public class FurnitureItem : CosmicItem
{
    [Header("Furniture Placement")]
    [Tooltip("The prefab that appears on the island grid when this furniture is placed")]
    public GameObject PlacedPrefab;

    [Tooltip("How many grid tiles this furniture occupies (width x height)")]
    public Vector2Int Footprint = new Vector2Int(1, 1);

    private void Reset()
    {
        ItemType = CosmicItemType.Furniture;
        Usable = true;
        Consumable = false;
        Droppable = false;
    }

    /// <summary>
    /// Entering placement mode is a Feature 005 responsibility.
    /// Returns false so the item is NOT consumed — player keeps it until placed.
    /// </summary>
    public override bool Use(string playerID)
    {
        Debug.Log("[FurnitureItem] Placement mode stub — Feature 005 will implement PlacementController");
        return false;
    }
}
