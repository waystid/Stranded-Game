# Ferrite Core

> **ACNH Equivalent:** [Iron Nugget](https://nookipedia.com/wiki/Iron_Nugget)

---

## Quick Reference

| Property | ACNH Value | Cosmic Colony Value |
|----------|-----------|---------------------|
| **Name** | Iron Nugget | Ferrite Core |
| **Category** | Crafting Material | Metallic Compound |
| **Sell Price** | 375 Bells | 375 Credits |
| **Source** | Hit rocks with shovel/axe | Mine Moon-Rock nodes |
| **Max Stack** | 99 | 99 |
| **Rarity** | Uncommon | Uncommon |

---

## 🌌 Cosmic Lore

**Ferrite Cores** are essential metallic elements found within Moon-Rock deposits across the planet. These naturally occurring ferromagnetic compounds are vital for crafting advanced tools, machinery, and structural components.

### In-Universe Context

When colonists first arrived, Ferrite Cores were immediately recognized as critical resources. The high iron content makes them perfect for:
- Tool durability upgrades
- Building structural frames
- Energy conduit construction
- Fabrication system components

**Z.O.E.'s Material Database:**
> "Ferrite Cores are the backbone of any successful colony. Always keep at least 30 in storage—you never know when you'll need to upgrade your equipment or construct a new facility."

**Geological Note:**
Moon-Rocks are impact-compressed mineral deposits. Each rock can be struck up to 8 times before depleting, yielding a random mix of materials including Ferrite Cores (approximately 15% drop rate per hit).

---

## 📊 Data Fields (LLM-Friendly)

```json
{
  "id": "material_ferrite_core_001",
  "acnh_data": {
    "name": "Iron Nugget",
    "category": "material",
    "sell_price": 375,
    "source": "Rocks (hit with shovel or axe)",
    "daily_limit": "Approximately 30 from 6 rocks per day",
    "uses": [
      "Tool upgrades",
      "Furniture crafting",
      "Building construction"
    ],
    "rarity": "Uncommon"
  },
  "cosmic_data": {
    "name": "Ferrite Core",
    "category": "metallic_compound",
    "lore": "Essential ferromagnetic element extracted from Moon-Rock deposits. Critical for advanced fabrication.",
    "sell_price_credits": 375,
    "source": "Moon-Rock nodes (mining)",
    "spawn_mechanics": {
      "drop_rate": 0.15,
      "per_rock_maximum": 8,
      "daily_rock_respawns": 6,
      "guaranteed_minimum_per_day": 1
    },
    "uses": [
      "Tool tier upgrades (Basic → Standard)",
      "Habitat expansion materials",
      "Fabricator component construction",
      "Trade Hub upgrades"
    ],
    "rarity": "Uncommon",
    "visual_theme": {
      "color_palette": ["#4A4A4A", "#7C7C7C", "#A0A0A0"],
      "model_style": "Metallic crystalline chunk with magnetic field shimmer",
      "particle_effects": ["metallic glint", "subtle magnetic distortion"]
    }
  },
  "technical_implementation": {
    "primary_class": "InventoryItem",
    "scriptable_object_type": "InventoryItem (Material)",
    "required_components": [
      "PickableItem (for ground drops)",
      "SpriteRenderer",
      "Collider2D"
    ],
    "setup_steps": [
      {
        "step": 1,
        "description": "Create InventoryItem ScriptableObject for Ferrite Core",
        "code_example": "Assets/Resources/Inventory/Materials/FerriteCore.asset"
      },
      {
        "step": 2,
        "description": "Add to MoonRock loot table with 15% drop weight",
        "code_example": "MoonRockLootTable: {material: 'ferrite_core', weight: 15, min: 1, max: 1}"
      },
      {
        "step": 3,
        "description": "Configure as crafting ingredient in recipe system",
        "code_example": "Register in CraftingDatabase with item_id: 'ferrite_core'"
      },
      {
        "step": 4,
        "description": "Set up stack limit and inventory category",
        "code_example": "MaxStack: 99, Category: 'Materials/Metallic'"
      }
    ],
    "prefab_structure": {
      "FerriteCore_Pickup": {
        "components": ["PickableItem", "SpriteRenderer", "CircleCollider2D"],
        "children": [
          {
            "name": "GlintEffect",
            "components": ["ParticleSystem (metallic sparkles)"]
          }
        ]
      }
    },
    "integration_notes": "Ferrite Cores should be tracked by a MaterialCounter UI element. When mining rocks, use MMFeedback for impact effects and material pop-out animations."
  },
  "wiki_metadata": {
    "nookipedia_url": "https://nookipedia.com/wiki/Iron_Nugget",
    "created_date": "2026-02-16",
    "last_updated": "2026-02-16",
    "tags": ["material", "mining", "crafting", "metallic", "uncommon", "daily-renewable"],
    "related_items": [
      "moon_rock",
      "mineral_extractor",
      "plasma_seiner_standard",
      "plasma_cutter_standard",
      "habitat_expansion_kit"
    ]
  }
}
```

---

## 🎮 Technical Implementation (Unity + TopDown Engine)

### Primary Class: `InventoryItem`

**Setup Overview:**
Ferrite Cores are standard crafting materials that drop from Moon-Rock nodes when the player uses the Mineral Extractor. They're automatically added to the player's inventory upon collection.

### Implementation

```csharp
// Add to MoonRock.cs (the rock destructible object)

using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

public class MoonRock : Health
{
    [Header("Moon Rock Settings")]
    public int maxHits = 8;
    public LootTable lootTable;

    private int currentHits = 0;

    public override void Damage(float damage, GameObject instigator,
        float flickerDuration, float invincibilityDuration)
    {
        currentHits++;

        // Spawn loot from table
        GameObject loot = lootTable.GetRandomLoot();
        if (loot != null)
        {
            Instantiate(loot, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }

        // Visual feedback
        MMFeedbacks hitFeedback = GetComponent<MMFeedbacks>();
        hitFeedback?.PlayFeedbacks();

        // Check if depleted
        if (currentHits >= maxHits)
        {
            DepletRock();
        }
    }

    private void DepletRock()
    {
        // Play depletion animation
        // Disable collider
        // Start respawn timer (24-hour in-game time)
        StartCoroutine(RespawnAfterDelay());
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(/* 1 in-game day */);
        currentHits = 0;
        // Re-enable rock
    }
}

// Loot Table Example
[System.Serializable]
public class LootTable
{
    public LootEntry[] entries;

    public GameObject GetRandomLoot()
    {
        float totalWeight = 0f;
        foreach (var entry in entries)
        {
            totalWeight += entry.weight;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float currentWeight = 0f;

        foreach (var entry in entries)
        {
            currentWeight += entry.weight;
            if (randomValue <= currentWeight)
            {
                return entry.itemPrefab;
            }
        }

        return null;
    }
}

[System.Serializable]
public class LootEntry
{
    public GameObject itemPrefab; // E.g., Ferrite Core prefab
    public float weight;          // E.g., 15 for 15% chance
}
```

**In Editor Setup:**
1. Create Moon-Rock prefab with Health component
2. Add MoonRock.cs script
3. Configure loot table:
   - Moon-Rock: 50 weight
   - Nano-Clay: 25 weight
   - Ferrite Core: 15 weight
   - Aurelium Crystal: 2 weight
4. Set maxHits to 8

---

## 🔄 Translation Actions

### 🌟 Translate Lore

**Original ACNH Description:**
> "Strike a rock with a shovel or axe, and you might get one of these!"

**Cosmic Translation:**
> "Strike a Moon-Rock node with your Mineral Extractor—you might uncover one of these valuable cores!"

**Translation Adaptations:**
- Iron → Ferrite (more sci-fi term)
- Nugget → Core (suggests technological use)
- Rocks → Moon-Rock nodes (planetary geology)
- Daily renewability maintained (important gameplay loop)

### 📐 Define Additional Data Fields

```json
{
  "gameplay_extensions": {
    "quality_variants": [
      {"name": "Low-Grade Ferrite", "sell_multiplier": 0.7},
      {"name": "Standard Ferrite", "sell_multiplier": 1.0},
      {"name": "High-Grade Ferrite", "sell_multiplier": 1.5}
    ],
    "processing": {
      "refinery_input": "ferrite_core",
      "refinery_output": "refined_ferrite",
      "processing_time": 30,
      "yield_multiplier": 2
    }
  }
}
```

---

## 📎 Related Items

- **[Moon-Rock](./moon_rock.md)** - Source node
- **[Mineral Extractor](../tools/mineral_extractor.md)** - Required tool
- **[Standard Plasma Seiner](../tools/plasma_seiner.md)** - Crafted using Ferrite Core
- **[Habitat Expansion Kit](../furniture/habitat_expansion.md)** - Requires Ferrite Cores

---

<sub>**Wiki Metadata:**</sub>
<sub>Created: 2026-02-16 | Last Updated: 2026-02-16 | ID: `material_ferrite_core_001`</sub>

---

## 🤖 LLM Agent Integration

```bash
# Quick material lookup
Grep for '"name": "Ferrite Core"' in CosmicWiki/pages/materials/*.md

# Find all recipes using Ferrite Core
Grep for "ferrite_core" in CosmicWiki/pages/**/*.md

# Get drop rate data
Grep for "drop_rate" in CosmicWiki/pages/materials/ferrite_core.md
```
