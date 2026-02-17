# Inventory System Guide (Feature 002)

**Branch:** `features/002-items`

## Overview

Item collection uses TopDown Engine's built-in `InventoryPickableItem` system, extended with
Cosmic Colony item types. The UI adds a 4×8 grid inventory panel and a 4-slot hotbar.

## Components

| Script | Role |
|--------|------|
| `ItemDatabase.cs` | ScriptableObject registry of all item types |
| `ItemPickup.cs` | Extends TDE InventoryPickableItem |
| `InventoryPanel.cs` | 4×8 grid UI + 4-slot hotbar |

## TDE Integration

Base classes used:
- `InventoryPickableItem` — `Assets/TopDownEngine/Common/Scripts/Items/InventoryPickableItem.cs`
- `InventoryItem` — item data component (child of picker)
- `ItemPicker` — parent with trigger collider

See pickable item pattern: `Assets/Prefabs/EnergyCrystalPicker.prefab`

## Item Catalog

| Cosmic Item | ACNH Equiv | Visual Asset | Rarity |
|------------|-----------|-------------|--------|
| Energy Crystal | Star Fragment | SineVFX Crystalsv01–v10 | Uncommon |
| Ferrite Core | Iron Nugget | Pandazole crafting props | Common |
| Alien Berry | Fruit | Pandazole food models | Common |
| Plasma Cell | Bell Bag | SineVFX effect | Rare |
| Stardust | Bells | Coin.cs (TDE) | Common |

## Prefab Pattern (3-Component Hierarchy)

```
EnergyCrystalPicker           ← ItemPicker + SphereCollider (trigger r=0.5)
  └── ItemData                ← InventoryItem + item data (name, quantity, icon)
       └── CrystalVisual      ← SineVFX crystal mesh + particle effect
```

## Inventory UI

- **Open:** I key
- **Layout:** 4 columns × 8 rows = 32 slots
- **Hotbar:** Bottom row (slots 1–4), accessible during play via number keys
- **Item icons:** Sprite assigned per ItemData ScriptableObject

## Status

- [ ] ItemDatabase.cs — ScriptableObject registry
- [ ] ItemPickup.cs — TDE extension
- [ ] InventoryPanel.cs — UI grid
- [ ] Item wiki pages (energy-crystal, ferrite-core, alien-berry)
- [ ] EnergyCrystalPicker pattern confirmed as template
