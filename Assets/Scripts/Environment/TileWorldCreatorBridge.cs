using UnityEngine;

/// <summary>
/// Seeds IslandGridManager cell registry from TileWorldCreator generated tile positions.
/// Called on Start() after TWC has generated the island shape.
///
/// DEPENDENCY: TileWorldCreator v4 must be imported before this activates.
/// Until then the script compiles but is a no-op (guarded by TILE_WORLD_CREATOR define).
///
/// After TileWorldCreator is imported:
///   1. Add the define TILE_WORLD_CREATOR to Project Settings > Player > Scripting Define Symbols
///   2. Assign this component to the same GO as your TileWorldCreator component
/// </summary>
public class TileWorldCreatorBridge : MonoBehaviour
{
#if TILE_WORLD_CREATOR
    [Header("Layer Names")]
    [Tooltip("Name of the blueprint layer that defines the island footprint.")]
    public string islandLayerName = "IslandShape";

    [Tooltip("Name of the water/border layer (cells outside island = water).")]
    public string waterLayerName = "WaterBorder";

    void Start()
    {
        var twc = GetComponent<TileWorldCreator.TileWorldCreator>();
        if (twc == null)
        {
            Debug.LogError("[TileWorldCreatorBridge] No TileWorldCreator component found on this GameObject.");
            return;
        }

        if (IslandGridManager.Instance == null)
        {
            Debug.LogError("[TileWorldCreatorBridge] IslandGridManager.Instance is null. Ensure it's in the scene.");
            return;
        }

        // Get all generated tile positions for the island layer
        var islandLayer = twc.GetBlueprintLayer(islandLayerName);
        if (islandLayer == null)
        {
            Debug.LogWarning($"[TileWorldCreatorBridge] Blueprint layer '{islandLayerName}' not found.");
            return;
        }

        int seeded = 0;
        // TileWorldCreator layers expose tile positions as Vector2Int grid coords
        foreach (var tilePos in islandLayer.GetTilePositions())
        {
            var cell = new Vector2Int(tilePos.x, tilePos.y);
            if (IslandGridManager.Instance.IsInBounds(cell))
            {
                IslandGridManager.Instance.SetTerrainType(cell, TerrainType.Flat);
                seeded++;
            }
        }

        Debug.Log($"[TileWorldCreatorBridge] Seeded {seeded} island cells as Flat terrain.");

        // Mark bounds cells as Beach (perimeter of island shape)
        // Water cells remain default until explicitly set elsewhere
    }
#else
    void Awake()
    {
        // TileWorldCreator not yet imported — this bridge is inactive.
        // Import TileWorldCreator v4 via Package Manager, then add
        // TILE_WORLD_CREATOR to Scripting Define Symbols to activate.
        Debug.Log("[TileWorldCreatorBridge] TileWorldCreator not imported yet. Bridge inactive.");
    }
#endif
}
