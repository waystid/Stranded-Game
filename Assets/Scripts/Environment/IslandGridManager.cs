using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton authority for the 64×64 island grid coordinate system.
/// The island parent GameObject is rotated 45° around Y so grid lines appear
/// horizontal/vertical on screen when the camera also has a 45° yaw (ACNH style).
///
/// All grid coordinates are in "island local space" — integer cells from (-32,-32)
/// to (31,31). Cell (0,0) is the island center.
///
/// Also owns the GridCell registry (terrain type, occupancy, height per cell).
/// </summary>
public class IslandGridManager : MonoBehaviour
{
    public static IslandGridManager Instance { get; private set; }

    [Header("Grid Settings")]
    [Tooltip("Total number of cells along one axis (64 = -32 to 31).")]
    public int   GridSize  = 64;

    [Tooltip("World-unit size of each cell in island local space.")]
    public float CellSize  = 1f;

    [Tooltip("The Island parent Transform (rotated 45° on Y).")]
    public Transform IslandRoot;

    // ── Cell Registry ─────────────────────────────────────────────────────────

    // Lazy-initialised dictionary; cells not in the dict are treated as default Flat.
    private readonly Dictionary<Vector2Int, GridCell> _cells = new Dictionary<Vector2Int, GridCell>();

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Convert a world-space position to an integer grid cell.
    /// Returns the cell whose center is closest to worldPos.
    /// </summary>
    public Vector2Int WorldToCell(Vector3 worldPos)
    {
        Vector3 local = WorldToLocal(worldPos);
        int cx = Mathf.FloorToInt(local.x / CellSize + 0.5f);
        int cz = Mathf.FloorToInt(local.z / CellSize + 0.5f);
        cx = Mathf.Clamp(cx, -GridSize / 2, GridSize / 2 - 1);
        cz = Mathf.Clamp(cz, -GridSize / 2, GridSize / 2 - 1);
        return new Vector2Int(cx, cz);
    }

    /// <summary>
    /// Convert a grid cell to the world-space center of that cell.
    /// </summary>
    public Vector3 CellToWorld(Vector2Int cell)
    {
        Vector3 local = new Vector3(cell.x * CellSize, 0f, cell.y * CellSize);
        return LocalToWorld(local);
    }

    /// <summary>
    /// Snap a world-space position to the nearest grid cell center.
    /// </summary>
    public Vector3 SnapToGrid(Vector3 worldPos)
    {
        return CellToWorld(WorldToCell(worldPos));
    }

    /// <summary>
    /// Return true if the cell is within island bounds.
    /// </summary>
    public bool IsInBounds(Vector2Int cell)
    {
        int half = GridSize / 2;
        return cell.x >= -half && cell.x < half &&
               cell.y >= -half && cell.y < half;
    }

    // ── Cell Registry API ─────────────────────────────────────────────────────

    /// <summary>
    /// Get the GridCell data for a cell. Returns a new default Flat cell if not yet set.
    /// </summary>
    public GridCell GetCell(Vector2Int cell)
    {
        if (_cells.TryGetValue(cell, out GridCell existing))
            return existing;
        // Lazily create default cell
        var newCell = new GridCell(TerrainType.Flat);
        _cells[cell] = newCell;
        return newCell;
    }

    /// <summary>
    /// Set or replace the GridCell data for a cell.
    /// </summary>
    public void SetCell(Vector2Int cell, GridCell data)
    {
        if (IsInBounds(cell))
            _cells[cell] = data;
    }

    /// <summary>
    /// Set just the terrain type for an existing cell (creates default if absent).
    /// </summary>
    public void SetTerrainType(Vector2Int cell, TerrainType type)
    {
        GetCell(cell).terrainType = type;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    Vector3 WorldToLocal(Vector3 worldPos)
    {
        if (IslandRoot != null)
            return IslandRoot.InverseTransformPoint(worldPos);
        return worldPos;
    }

    Vector3 LocalToWorld(Vector3 localPos)
    {
        if (IslandRoot != null)
            return IslandRoot.TransformPoint(localPos);
        return localPos;
    }
}
