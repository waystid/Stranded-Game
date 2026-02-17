# Pandazole Asset Agent

> **🤖 Specialized Agent for Pandazole Ultimate Pack Integration**
>
> Full catalog of all 6 Pandazole packs with prefab paths, material info, and recommended uses.

---

## Overview

The **Pandazole Agent** provides a searchable catalog of the Pandazole Ultimate Pack assets
used across multiple features: world flora (001), items (002), buildings (005), and tools (006).

---

## Pack Catalog

### 1. Pandazole Nature Environment Pack
**Feature use:** 001-world (flora, terrain tiles)

```
Assets/Pandazole_Ultimate_Pack/Pandazole Nature Environment Pack/
  Prefabs/
    Trees/
      Tree_Fall_01–10.prefab     ← Autumn variants
      Tree_Winter_01–05.prefab   ← Winter/dead variants
    Plants/
      Cactus_01–05.prefab
      Mushroom_01–08.prefab
      Coral_01–06.prefab
      Grass_01–10.prefab
    Terrain/
      TileGround_01–08.prefab    ← Ground tile variants
      TileHex_01–04.prefab       ← Hex terrain tiles
  Materials/
  Textures/
```

**Recommended prefabs for island:**
- `Tree_Fall_03.prefab` — standard island tree
- `Mushroom_04.prefab` — alien mushroom flora
- `Grass_02.prefab` — ground cover (scale 2-3x)
- `TileGround_02.prefab` — standard terrain tile

---

### 2. Pandazole Kitchen Food Pack
**Feature use:** 002-items (consumable item visuals)

```
Assets/Pandazole_Ultimate_Pack/Pandazole Kitchen Food/
  Prefabs/
    Fruit/
      Fruit_01–08.prefab         ← Alien Berry source models
    Vegetables/
    Prepared/
    Props/
```

**Item mapping:**
- `Fruit_03.prefab` → Alien Berry item visual

---

### 3. Pandazole Survival Crafting Pack
**Feature use:** 002-items (crafting materials), 006-tools (tool props)

```
Assets/Pandazole_Ultimate_Pack/Pandazole Survival Crafting Pack/
  Prefabs/
    Tools/
      Axe_01.prefab              ← Plasma Cutter visual
      Shovel_01.prefab           ← Mineral Extractor visual
      Fishing_Rod_01.prefab      ← Plasma Seiner visual
    Resources/
      Crystal_01–05.prefab       ← Ferrite Core source models
      Rock_01–08.prefab
    Crafting/
      Workbench_01.prefab
```

**Tool visual mapping:**
| Cosmic Tool | Pandazole Asset |
|-------------|----------------|
| Plasma Cutter | `Tools/Axe_01.prefab` |
| Mineral Extractor | `Tools/Shovel_01.prefab` |
| Plasma Seiner | `Tools/Fishing_Rod_01.prefab` |

---

### 4. Pandazole City Town Pack
**Feature use:** 005-buildings-and-houses

```
Assets/Pandazole_Ultimate_Pack/Pandazole City Town Pack/
  Prefabs/
    Buildings/
      House_01–06.prefab         ← Player House
      Shop_01–04.prefab          ← Trade Hub
      Municipal_01–02.prefab     ← Command Center
    Props/
      Bench_01–04.prefab
      Lamppost_01–02.prefab
      Fence_01–06.prefab
    Roads/
```

**Building mapping:**
| Cosmic Building | Pandazole Asset |
|----------------|----------------|
| Command Center | `Buildings/Municipal_01.prefab` |
| Trade Hub | `Buildings/Shop_02.prefab` |
| House | `Buildings/House_03.prefab` |

---

### 5. Pandazole Farm Ranch Pack
**Feature use:** 005-buildings-and-houses (farm)

```
Assets/Pandazole_Ultimate_Pack/Pandazole Farm Ranch Pack/
  Prefabs/
    Buildings/
      Barn_01–03.prefab
      Silo_01–02.prefab
      FarmHouse_01–02.prefab
    Props/
      FenceWood_01–04.prefab
      Trough_01.prefab
    Crops/
      CropField_01–04.prefab
```

---

### 6. Pandazole Dungeon Pack (if present)
**Feature use:** TBD — potential underground/cave areas

```
Assets/Pandazole_Ultimate_Pack/Pandazole Dungeon Pack/
```

---

## Material Information

All Pandazole assets use Unity's **URP Lit** shader with atlas textures.

**Customization approach:**
- Swap `_BaseMap` texture to retexture entire pack
- Adjust `_BaseColor` tint for seasonal variants
- Do NOT use instance materials per-object (too many draw calls)
- Use `MaterialPropertyBlock` for per-instance color tinting

**Batching notes:**
- All packs use static batching where `isStatic = true`
- Flora should use `GPU Instancing` enabled materials
- Keep LOD groups intact — don't break LOD chains

---

## Grid Snap Notes (001-world integration)

When placing Pandazole flora on the island grid:
- Trees: center at cell world position, Y offset = 0 (ground level)
- Rocks: center at cell world position, Y offset = 0
- Crops: 1×1 cell footprint, rotate to match island orientation (Y = 45°)

Standard scale multipliers:
- Trees: `(1, 1, 1)` native scale
- Grass: `(2, 2, 2)` for ground cover feel
- Rocks: `(0.8, 0.8, 0.8)` to `(1.5, 1.5, 1.5)` random variation

---

## Related Files

- `CosmicWiki/pages/world/flora-catalog.md` — Flora wiki page
- `CosmicWiki/pages/buildings/building-catalog.md` — Building wiki page
- `Assets/Scripts/Environment/FloraPlacement.cs` — Flora placement system
- `Assets/Scripts/Buildings/BuildingData.cs` — Building data ScriptableObjects
