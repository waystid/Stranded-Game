# Terrain Types

**Feature:** 001-world | **Script:** `GridCell.cs`

## Overview

Each cell in the 64×64 island grid has a `TerrainType` that determines:
- Visual appearance (material/shader)
- Walkability (NavMesh area type)
- Valid actions (what tools/buildings can be used here)

## Type Reference

### Flat
```
TerrainType.Flat
ACNH Equivalent: Grass / Standard Ground
Material: IslandGround_Flat (Pandazole TileGround_02)
Walkable: Yes
Valid Actions: Flora placement, building, digging, farming
```

### Cliff
```
TerrainType.Cliff
ACNH Equivalent: Cliff / Elevated Terrain
Material: IslandGround_Cliff (darker, rocky variant)
Walkable: No (blocks path, requires ladder/ramp)
Valid Actions: Terraform (dig to lower), none else
Height Levels: 1–3
```

### Water (Nebula Pool)
```
TerrainType.Water
ACNH Equivalent: River / Sea / Pond
Material: Water shader tile
Walkable: No
Valid Actions: Fishing (Plasma Seiner), water crafting
```

### Beach (Stellar Shore)
```
TerrainType.Beach
ACNH Equivalent: Beach / Sandy Shore
Material: Sandy/pale material
Walkable: Yes
Valid Actions: Beachcombing, shell collection
```

### Radiation Belt
```
TerrainType.Radiation
ACNH Equivalent: Edge of island / restricted zone
Material: Glowing hazard overlay
Walkable: Yes (but deals periodic damage)
Valid Actions: Restricted — special suit required
Location: Island perimeter (outer 2 cells)
```

## Transitions

- `Flat` → `Cliff`: requires Mineral Extractor tool (dig up)
- `Cliff` → `Flat`: requires Mineral Extractor tool (dig down)
- Any type → `Water`: requires special unlock
- `Beach` is auto-assigned at island edge adjacent to water

## Related

- `CosmicWiki/guides/terraforming-system.md`
- `Assets/Scripts/Environment/GridCell.cs`
- `Assets/Scripts/Environment/TerrainPainter.cs`
