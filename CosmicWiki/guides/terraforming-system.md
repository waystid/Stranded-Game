# Terraforming System Guide (Feature 001)

**Branch:** `features/001-world`

## Overview

The island is a 64×64 grid managed by `IslandGridManager`. Each cell has a `GridCell` data
object tracking terrain type, height, and occupant. The terraforming system lets players
paint terrain types and place flora onto cells.

## Components

| Script | Role |
|--------|------|
| `IslandGridManager.cs` | Grid authority — WorldToCell, SnapToGrid |
| `GridCell.cs` | Cell data: TerrainType, height, occupant |
| `GridCursor.cs` | Raycast → hover highlight over active cell |
| `TerrainPainter.cs` | Paint terrain type, trigger NavMesh rebake |
| `FloraPlacement.cs` | Snap Pandazole flora prefabs to grid cells |

## Terrain Types

| Type | Cosmic Name | Visual | ACNH Equiv |
|------|-------------|--------|-----------|
| `Flat` | Standard Ground | IslandGround material | Grass |
| `Cliff` | Rock Shelf | Dark/rocky material | Cliff |
| `Water` | Nebula Pool | Water shader tile | Water |
| `Beach` | Stellar Shore | Sandy material | Beach |
| `Radiation` | Radiation Belt | Glowing hazard overlay | Edge of island |

## Cell Data Structure

```csharp
// GridCell.cs
public enum TerrainType { Flat, Cliff, Water, Beach, Radiation }

public class GridCell
{
    public TerrainType terrainType;
    public int heightLevel;         // 0 = sea level, 1–3 = elevated
    public GameObject occupant;     // tree/rock/building occupying this cell
    public bool isWalkable;         // derived from terrainType + occupant
}
```

## Grid Cursor

- Raycasts from screen center (player viewpoint) or mouse position
- Calls `IslandGridManager.WorldToCell(hitPoint)` → gets `(x, z)` cell index
- Moves highlight quad to `IslandGridManager.CellToWorld(x, z)`
- DevConsole can toggle cursor visibility

## Flora Placement

Flora snaps to cell center:
```csharp
// FloraPlacement.cs
Vector3 worldPos = gridManager.CellToWorld(cellX, cellZ);
worldPos.y = 0f; // ground level
Instantiate(floraPrefab, worldPos, Quaternion.identity, islandRoot);
cell.occupant = placedFlora;
```

## NavMesh Integration

After terrain paint:
1. Mark cliff/water cells as `Not Walkable`
2. Call `NavMeshSurface.BuildNavMesh()` to rebake
3. Villager `NavMeshAgent` automatically re-paths

## Status

- [x] GridCell.cs — data structure
- [x] GridCursor.cs — hover highlight
- [x] TerrainPainter.cs — paint system
- [x] FloraPlacement.cs — flora snapping
- [ ] NavMesh rebake integration
