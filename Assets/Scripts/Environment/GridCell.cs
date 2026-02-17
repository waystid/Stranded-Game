using UnityEngine;

/// <summary>
/// Data container for a single cell in the island grid.
/// Stored and accessed via IslandGridManager's cell registry.
/// Not a MonoBehaviour — pure data class.
/// </summary>
[System.Serializable]
public class GridCell
{
    // ── Terrain ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Terrain classification for this cell. Determines visuals, walkability,
    /// and valid player actions (planting, building, digging, fishing, etc).
    /// ACNH equivalents: Flat=Grass, Cliff=Elevated, Water=River/Sea, Beach=Shore
    /// </summary>
    public TerrainType terrainType = TerrainType.Flat;

    /// <summary>
    /// Vertical height tier. 0 = sea level / ground floor.
    /// 1–3 = elevated cliff tiers (each tier = 1 unit up in world space).
    /// Only relevant when terrainType = Cliff.
    /// </summary>
    public int heightLevel = 0;

    // ── Occupancy ────────────────────────────────────────────────────────────

    /// <summary>
    /// The GameObject currently occupying this cell (tree, rock, building, etc).
    /// Null = cell is empty. Use IsOccupied to check.
    /// </summary>
    public GameObject occupant = null;

    /// <summary>
    /// True if this cell has an occupant (flora, building, or other blocking object).
    /// </summary>
    public bool IsOccupied => occupant != null;

    // ── Derived Properties ───────────────────────────────────────────────────

    /// <summary>
    /// Whether the player character can walk through this cell.
    /// Flat unoccupied = walkable. Cliff, Water, occupied = not walkable.
    /// Used when rebaking NavMesh via TerrainPainter.
    /// </summary>
    public bool IsWalkable
    {
        get
        {
            if (terrainType == TerrainType.Cliff)   return false;
            if (terrainType == TerrainType.Water)   return false;
            if (IsOccupied)                          return false;
            return true;
        }
    }

    /// <summary>
    /// Whether tools can interact with this cell (fishing, farming, terraforming).
    /// Radiation Belt cells restrict standard tool use.
    /// </summary>
    public bool IsInteractable => terrainType != TerrainType.Radiation;

    // ── Constructor ───────────────────────────────────────────────────────────

    public GridCell() { }

    public GridCell(TerrainType type, int height = 0)
    {
        terrainType = type;
        heightLevel = height;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Clear the occupant reference (does not destroy the GameObject).
    /// Call this after the occupant is removed from the scene.
    /// </summary>
    public void ClearOccupant()
    {
        occupant = null;
    }

    public override string ToString()
    {
        return $"GridCell(type={terrainType}, h={heightLevel}, occupied={IsOccupied})";
    }
}

/// <summary>
/// Terrain classification for island grid cells.
/// Maps to ACNH terrain categories with Cosmic Colony theming.
/// </summary>
public enum TerrainType
{
    /// <summary>Standard flat ground. Supports flora, buildings, farming.</summary>
    Flat,

    /// <summary>Elevated terrain tier. Blocks pathing without ramp/ladder.</summary>
    Cliff,

    /// <summary>Nebula Pool — water surface. Enables fishing. Blocks movement.</summary>
    Water,

    /// <summary>Stellar Shore — sandy beach border. Walkable, no building.</summary>
    Beach,

    /// <summary>Radiation Belt — island perimeter hazard. Damages player over time.</summary>
    Radiation
}
