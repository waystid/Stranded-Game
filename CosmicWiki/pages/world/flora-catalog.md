# Flora Catalog

**Feature:** 001-world | **Asset:** Pandazole Nature Environment Pack

## Overview

Flora placed by `FloraPlacement.cs` onto `Flat` terrain cells. Each occupies one grid cell.
Prefabs source: `Assets/Pandazole_Ultimate_Pack/Pandazole Nature Environment Pack/`

## Trees

| Cosmic Name | ACNH Equiv | Prefab | Collider Size |
|------------|-----------|--------|--------------|
| Xeno-Oak | Oak Tree | `Tree_Fall_03.prefab` | 1×4×1, Y center=2 |
| Xeno-Pine | Cedar Tree | `Tree_Winter_02.prefab` | 1×4×1, Y center=2 |
| Gloom-Trunk | Dead Tree | `Tree_Fall_08.prefab` | 1×3×1, Y center=1.5 |
| Spore-Burst | Mushroom Tree | `Mushroom_04.prefab` | 1×2×1, Y center=1 |

**Choppable trees** (Plasma Cutter drops Ferrite Core):
- Xeno-Oak, Xeno-Pine, Gloom-Trunk

**Permanent trees** (cannot be chopped):
- Spore-Burst (decorative / quest)

## Rocks

| Cosmic Name | ACNH Equiv | Prefab | Collider Size |
|------------|-----------|--------|--------------|
| Mineral Node | Rock | `TileGround_Rock01.prefab` | 1×1×1, Y center=0.5 |
| Crystal Vein | Large Rock | `TileGround_Rock03.prefab` | 1×1×1, Y center=0.5 |

**Hits with Mineral Extractor:**
- Mineral Node: 6 hits → drops Ferrite Core ×3, Stardust ×1
- Crystal Vein: 8 hits → drops Energy Crystal ×1, Ferrite Core ×2

## Ground Flora

| Name | Prefab | Grid | Effect |
|------|--------|------|--------|
| Star Grass | `Grass_02.prefab` | Decorative (no occupant) | None |
| Cactus Bloom | `Cactus_03.prefab` | 1 cell occupant | Blocks path |
| Coral Cluster | `Coral_02.prefab` | Water-edge cells | Decorative |

## Placement Rules

- Flora only on `TerrainType.Flat` cells (except Coral → Water-edge)
- Only 1 flora per cell (`cell.occupant != null` = occupied)
- Trees are Navigation Obstacle (rebake NavMesh after placement)
- Rocks are Navigation Static + Not Walkable

## Related

- `CosmicWiki/agents/pandazole-agent.md` — Full Pandazole path catalog
- `Assets/Scripts/Environment/FloraPlacement.cs`
- `CosmicWiki/guides/terraforming-system.md`
