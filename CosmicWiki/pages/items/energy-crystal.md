# Energy Crystal

```json
{
  "id": "energy_crystal",
  "cosmic_name": "Energy Crystal",
  "acnh_equivalent": "Star Fragment",
  "category": "Resource",
  "rarity": "Uncommon",
  "stack_size": 10,
  "sell_value": 200,
  "currency": "Stardust",
  "sources": ["Crystal Vein rock hit (1×)", "SineVFX Crystal node pickup"],
  "uses": ["Crafting: Plasma Cell ×1 needs Crystal ×3", "Building material: Nano-Fabricator"],
  "visual_asset": "Assets/SineVFX/TranslucentCrystals/Prefabs/Crystalsv01.prefab",
  "pickup_prefab": "Assets/Prefabs/EnergyCrystalPicker.prefab",
  "icon_sprite": "TBD"
}
```

## Description

Translucent crystalline formations that pulse with raw cosmic energy. Found embedded in
Crystal Vein rocks across the island, or as rare floating nodes after meteor showers.
Essential for advanced technology crafting.

## Obtaining

1. **Crystal Vein rock** — hit with Mineral Extractor 8 times
2. **Pickup node** — spawns at random Flat cells after in-game meteor shower event
3. **Trade** — purchase from Trade Hub (10× Stardust each)

## Visual

SineVFX `TranslucentCrystals` pack variants v01–v10 provide 10 distinct crystal appearances.
The pickup node uses `Crystalsv01.prefab` as default; rare variants use `Crystalsv07–v10`.

## Pickup Pattern

Uses `EnergyCrystalPicker.prefab` as template (3-component hierarchy):
```
EnergyCrystalPicker           ← ItemPicker + SphereCollider trigger
  └── ItemData                ← InventoryItem (name="Energy Crystal", quantity=1)
       └── CrystalVisual      ← SineVFX mesh + glow particle
```

## Uses

| Recipe | Ingredients | Output |
|--------|-------------|--------|
| Plasma Cell | Crystal ×3 | Plasma Cell ×1 |
| Nano-Fabricator | Crystal ×15, Ferrite ×10 | Building |

## Related

- `CosmicWiki/pages/items/ferrite-core.md`
- `CosmicWiki/guides/inventory-system.md`
- `Assets/SineVFX/TranslucentCrystals/`
