# Alien Berry

```json
{
  "id": "alien_berry",
  "cosmic_name": "Alien Berry",
  "acnh_equivalent": "Fruit",
  "category": "Consumable",
  "rarity": "Common",
  "stack_size": 10,
  "sell_value": 100,
  "currency": "Stardust",
  "sources": ["Berry Bush flora (harvest)", "Farm Plot crop", "Villager gift"],
  "uses": ["Eat to restore 1 HP", "Cooking ingredient", "Sell for Stardust"],
  "visual_asset": "Assets/Pandazole_Ultimate_Pack/Pandazole Kitchen Food/Prefabs/Fruit/Fruit_03.prefab",
  "icon_sprite": "TBD",
  "variants": ["Red Berry", "Blue Berry", "Gold Berry", "Neon Berry"]
}
```

## Description

Strange bioluminescent berries that grow on island bushes and cultivated farm plots.
Each color variant provides slightly different bonuses when consumed. Gold Berries sell
for triple the standard value.

## Variants

| Variant | Color | Sell Value | Bonus Effect |
|---------|-------|-----------|-------------|
| Red Berry | Red | 100 Stardust | Restore 1 HP |
| Blue Berry | Blue | 100 Stardust | Speed +10% for 30s |
| Gold Berry | Gold | 300 Stardust | None (sell only) |
| Neon Berry | Cyan | 150 Stardust | Night vision 60s |

## Obtaining

1. **Berry Bush flora** — harvestable plant (regrows after 3 in-game days)
2. **Farm Plot crop** — grow with Hydration Disperser, yield ×5 per harvest
3. **Villager gift** — Peppy and Normal personality villagers give berries

## Visual

Pandazole Kitchen Food pack `Fruit_03.prefab` as base. Different material tints per variant:
- Red: default material
- Blue: `_BaseColor = (0.2, 0.4, 1.0)`
- Gold: `_BaseColor = (1.0, 0.85, 0.0)`
- Neon: `_BaseColor = (0.0, 1.0, 0.9)`

## Related

- `CosmicWiki/guides/inventory-system.md`
- `CosmicWiki/agents/pandazole-agent.md`
