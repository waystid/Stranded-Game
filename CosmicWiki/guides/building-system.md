# Building System Guide (Feature 005)

**Branch:** `features/005-buildings-and-houses`

## Overview

Grid-snapped building placement using `IslandGridManager`, building state machine
(Blueprint → Construction → Complete), and optional interior scene loading.

## Building Types

| Building | Cosmic Name | ACNH Equiv | Footprint | Asset |
|---------|------------|-----------|----------|-------|
| Command Center | Command Center | Resident Services | 4×4 | Pandazole City Municipal_01 |
| Trade Hub | Trade Hub | Nook's Cranny | 3×3 | Pandazole City Shop_02 |
| Nano-Fabricator | Nano-Fabricator | Able Sisters | 3×3 | Pandazole City |
| Galactic Archive | Galactic Archive | Museum | 5×5 | ithappy City |
| Farm Plot | Farm Plot | Farm | 4×4 | Pandazole Farm |
| House | Player House | Player Home | 3×3 | Pandazole City House_03 |

## Building States

```csharp
enum BuildingState { Blueprint, UnderConstruction, Complete }
```

- **Blueprint:** Ghost (semi-transparent) preview placed on grid
- **UnderConstruction:** Opaque, scaffold material, timer countdown
- **Complete:** Final appearance, interactive

## Components

| Script | Role |
|--------|------|
| `BuildingData.cs` | ScriptableObject per building type: footprint, prefab, interior scene |
| `BuildingPlacer.cs` | Ghost preview + grid snap + confirm placement |
| `BuildingManager.cs` | Registry, save/load building states |
| `BuildingInterior.cs` | Trigger scene load for house interiors |

## Placement Flow

1. Press **B** → Building picker UI opens
2. Select building type → ghost preview follows `GridCursor` (Feature 001 dependency)
3. Ghost snaps to `IslandGridManager.SnapToGrid()` — footprint cells highlighted
4. **Click** to place → building spawns in Blueprint state
5. Provide materials → transitions to UnderConstruction
6. Timer completes → Complete state

## Grid Footprint

```csharp
// BuildingData.cs
public Vector2Int footprint;  // e.g. (3,3) for House

// BuildingPlacer checks all footprint cells are:
// - TerrainType.Flat
// - No existing occupant
// Then marks all cells as occupied = this building
```

## Interior Scenes

- Buildings with `interior = true` in `BuildingData` load a separate scene on interact
- Interior scenes: `Assets/Scenes/Interiors/House_[PlayerName].unity`
- `BuildingInterior.cs` handles additive scene load / unload

## Assets Used

- **Pandazole City Town Pack** — `Assets/Pandazole_Ultimate_Pack/Pandazole City Town Pack/`
- **Pandazole Farm Ranch Pack** — `Assets/Pandazole_Ultimate_Pack/Pandazole Farm Ranch Pack/`
- **ithappy Cartoon City** — `Assets/ithappy/Cartoon_City_Free/`
- **IslandGridManager** — Feature 001 dependency

## Status

- [ ] BuildingData.cs ScriptableObject
- [ ] BuildingPlacer.cs placement + ghost
- [ ] BuildingManager.cs registry + save
- [ ] BuildingInterior.cs scene loading
- [ ] Building picker UI
- [ ] Building catalog wiki page
