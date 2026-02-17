using System.Collections.Generic;
using UnityEngine;
using GiantGrey.TileWorldCreator;

/// <summary>
/// Seeds IslandGridManager cell registry from TileWorldCreatorManager generated tile positions.
///
/// Attach to the same GameObject as TileWorldCreatorManager.
/// Subscribes to OnBlueprintLayersReady so the registry is seeded immediately after generation.
/// AutoGenerate = true triggers GenerateCompleteMap() on Start().
/// </summary>
[RequireComponent(typeof(TileWorldCreatorManager))]
public class TileWorldCreatorBridge : MonoBehaviour
{
    [Header("Layer Names")]
    [Tooltip("Name of the blueprint layer that defines the walkable island footprint.")]
    public string IslandLayerName = "IslandShape";

    [Header("Options")]
    [Tooltip("Automatically call GenerateCompleteMap() on Start().")]
    public bool AutoGenerate = true;

    private TileWorldCreatorManager _twc;

    void Awake()
    {
        _twc = GetComponent<TileWorldCreatorManager>();
        _twc.OnBlueprintLayersReady += OnBlueprintLayersReady;
    }

    void Start()
    {
        if (AutoGenerate)
            _twc.GenerateCompleteMap();
    }

    void OnDestroy()
    {
        if (_twc != null)
            _twc.OnBlueprintLayersReady -= OnBlueprintLayersReady;
    }

    private void OnBlueprintLayersReady()
    {
        if (IslandGridManager.Instance == null)
        {
            Debug.LogError("[TileWorldCreatorBridge] IslandGridManager.Instance is null.");
            return;
        }

        BlueprintLayer layer = _twc.GetBlueprintLayer(IslandLayerName);
        if (layer == null)
        {
            Debug.LogWarning($"[TileWorldCreatorBridge] Blueprint layer '{IslandLayerName}' not found.");
            return;
        }

        HashSet<Vector2> positions = layer.allPositions;
        if (positions == null || positions.Count == 0)
        {
            Debug.LogWarning("[TileWorldCreatorBridge] No positions on island layer after generation.");
            return;
        }

        int seeded = 0;
        foreach (Vector2 pos in positions)
        {
            var cell = new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
            if (IslandGridManager.Instance.IsInBounds(cell))
            {
                IslandGridManager.Instance.SetTerrainType(cell, TerrainType.Flat);
                seeded++;
            }
        }

        Debug.Log($"[TileWorldCreatorBridge] Seeded {seeded} island cells as Flat terrain from '{IslandLayerName}'.");
    }

    /// <summary>Trigger a new map generation at runtime (e.g. from DevConsole).</summary>
    public void Regenerate() => _twc?.GenerateCompleteMap();
}
