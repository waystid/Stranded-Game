using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GiantGrey.TileWorldCreator;

/// <summary>
/// Seeds IslandGridManager cell registry from TileWorldCreatorManager generated tile positions.
/// Also scatters Pandazole tree and rock prefabs on a random subset of island cells.
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

    [Header("Scatter — Trees (Flora)")]
    [Tooltip("Tree prefab to scatter. Spawned props automatically get tag=Flora and GridOccupant.")]
    public GameObject TreePrefab;
    [Range(0f, 0.5f)]
    [Tooltip("Fraction of island cells that get a tree (0.15 = 15%).")]
    public float TreeDensity = 0.15f;

    [Header("Scatter — Rocks")]
    [Tooltip("Rock prefab to scatter. Spawned props automatically get tag=Rock and GridOccupant.")]
    public GameObject RockPrefab;
    [Range(0f, 0.2f)]
    [Tooltip("Fraction of island cells that get a rock (0.05 = 5%).")]
    public float RockDensity = 0.05f;

    [Header("Scatter Options")]
    [Tooltip("Seed for deterministic scatter. Change to get a different layout.")]
    public int ScatterSeed = 42;
    [Tooltip("How many cells from center to keep clear of props (spawn safety zone).")]
    public int ClearRadius = 5;

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

        // Seed the grid manager
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

        // Scatter props
        ScatterProps(positions);
    }

    private void ScatterProps(HashSet<Vector2> positions)
    {
        if (TreePrefab == null && RockPrefab == null)
            return;

        // Find island center (average of all positions)
        Vector2 center = Vector2.zero;
        foreach (var p in positions) center += p;
        center /= positions.Count;

        // Filter out cells too close to center (spawn safety zone)
        var candidates = positions
            .Where(p => Vector2.Distance(p, center) > ClearRadius)
            .ToList();

        // Deterministic shuffle with seed
        var rng = new System.Random(ScatterSeed);
        for (int i = candidates.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            var tmp = candidates[i];
            candidates[i] = candidates[j];
            candidates[j] = tmp;
        }

        int treeCount = Mathf.RoundToInt(candidates.Count * TreeDensity);
        int rockCount = Mathf.RoundToInt(candidates.Count * RockDensity);

        // Create a parent GO to keep hierarchy tidy
        var propsParent = new GameObject("ScatteredProps");
        propsParent.transform.SetParent(transform, false);

        int placed = 0;

        // Trees
        if (TreePrefab != null)
        {
            for (int i = 0; i < treeCount && i < candidates.Count; i++)
            {
                PlaceProp(TreePrefab, candidates[i], "Flora", propsParent.transform);
                placed++;
            }
        }

        // Rocks (from remaining cells after trees)
        if (RockPrefab != null)
        {
            int startIdx = treeCount;
            for (int i = startIdx; i < startIdx + rockCount && i < candidates.Count; i++)
            {
                PlaceProp(RockPrefab, candidates[i], "Rock", propsParent.transform);
                placed++;
            }
        }

        Debug.Log($"[TileWorldCreatorBridge] Scattered {placed} props ({treeCount} trees, {rockCount} rocks).");
    }

    private void PlaceProp(GameObject prefab, Vector2 twcCell, string tag, Transform parent)
    {
        // TWC cell coords map to local space of this GO (TileWorldCreator at Y=45°)
        var localPos = new Vector3(twcCell.x, 0f, twcCell.y);

        var go = Instantiate(prefab, parent);
        go.transform.localPosition = localPos;
        go.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        go.tag = tag;

        // Add GridOccupant if not present (auto-registers on Start)
        if (go.GetComponent<GridOccupant>() == null)
            go.AddComponent<GridOccupant>();
    }

    /// <summary>Trigger a new map generation at runtime (e.g. from DevConsole).</summary>
    public void Regenerate() => _twc?.GenerateCompleteMap();
}
