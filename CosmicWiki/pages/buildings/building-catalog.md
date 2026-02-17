# Building Catalog

**Feature:** 005-buildings-and-houses

## Overview

All buildings snap to the island grid via `IslandGridManager`. Each has a footprint
(cell count), build cost, and optional interior scene.

## Civic Buildings

### Command Center
```json
{
  "id": "command_center",
  "cosmic_name": "Command Center",
  "acnh_equivalent": "Resident Services",
  "footprint": [4, 4],
  "build_cost": {"ferrite_core": 50, "energy_crystal": 20},
  "state": "Complete (pre-placed at island start)",
  "has_interior": true,
  "interior_scene": "CommandCenter_Interior",
  "asset": "Assets/Pandazole_Ultimate_Pack/Pandazole City Town Pack/Prefabs/Buildings/Municipal_01.prefab"
}
```

### Galactic Archive
```json
{
  "id": "galactic_archive",
  "cosmic_name": "Galactic Archive",
  "acnh_equivalent": "Museum",
  "footprint": [5, 5],
  "build_cost": {"ferrite_core": 80, "energy_crystal": 40, "stardust": 50000},
  "state": "Buildable (unlock via quest)",
  "has_interior": true,
  "asset": "Assets/ithappy/Cartoon_City_Free/Prefabs/Buildings/"
}
```

## Commerce Buildings

### Trade Hub
```json
{
  "id": "trade_hub",
  "cosmic_name": "Trade Hub",
  "acnh_equivalent": "Nook's Cranny",
  "footprint": [3, 3],
  "build_cost": {"ferrite_core": 30, "energy_crystal": 10},
  "upgrades": 3,
  "asset": "Assets/Pandazole_Ultimate_Pack/Pandazole City Town Pack/Prefabs/Buildings/Shop_02.prefab"
}
```

### Nano-Fabricator
```json
{
  "id": "nano_fabricator",
  "cosmic_name": "Nano-Fabricator",
  "acnh_equivalent": "Able Sisters",
  "footprint": [3, 3],
  "build_cost": {"ferrite_core": 25, "energy_crystal": 15},
  "special": "Wardrobe customization (Feature 007)",
  "asset": "Assets/Pandazole_Ultimate_Pack/Pandazole City Town Pack/Prefabs/Buildings/Shop_01.prefab"
}
```

## Residential

### Player House
```json
{
  "id": "player_house",
  "cosmic_name": "Player House",
  "acnh_equivalent": "Player Home",
  "footprint": [3, 3],
  "build_cost": {"ferrite_core": 30},
  "has_interior": true,
  "interior_upgrades": ["Kitchen", "Back Room", "Basement", "Left Wing", "Right Wing"],
  "asset": "Assets/Pandazole_Ultimate_Pack/Pandazole City Town Pack/Prefabs/Buildings/House_03.prefab"
}
```

## Agriculture

### Farm Plot
```json
{
  "id": "farm_plot",
  "cosmic_name": "Farm Plot",
  "acnh_equivalent": "Farm / Garden",
  "footprint": [4, 4],
  "build_cost": {"ferrite_core": 15},
  "crop_cells": 9,
  "asset": "Assets/Pandazole_Ultimate_Pack/Pandazole Farm Ranch Pack/Prefabs/Buildings/FarmHouse_01.prefab"
}
```

## Related

- `CosmicWiki/guides/building-system.md` — Placement + state machine
- `CosmicWiki/agents/pandazole-agent.md` — Pandazole asset paths
- `Assets/Scripts/Buildings/BuildingData.cs` — ScriptableObject definitions
