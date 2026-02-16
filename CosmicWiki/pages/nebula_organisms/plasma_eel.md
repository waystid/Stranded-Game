# Plasma Eel

> **ACNH Equivalent:** [Sea Bass](https://nookipedia.com/wiki/Sea_Bass)

---

## Quick Reference

| Property | ACNH Value | Cosmic Colony Value |
|----------|-----------|---------------------|
| **Name** | Sea Bass | Plasma Eel |
| **Category** | Fish | Nebula Organism |
| **Sell Price** | 400 Bells | 400 Credits |
| **Rarity** | Common | Common |
| **Location** | Ocean | Plasma Flows |
| **Time** | All Day | All Day |
| **Shadow Size** | Large | Energy Signature: Large |

---

## 🌌 Cosmic Lore

The **Plasma Eel** is one of the most abundant energy-based organisms found in the planet's plasma flows. Despite its prevalence, it serves as an essential food source for larger nebula predators and a stable income source for beginning colonists.

### In-Universe Context

When the first colonists arrived on crash-landed planets, the Plasma Eel was among the first life forms documented. Its bioluminescent body creates a distinctive ripple pattern in plasma streams, making it easily identifiable even from a distance.

Many veteran colonists joke that "catching a Plasma Eel is like finding money in the couch cushions" - they're everywhere, but you're always happy to sell them for Credits at the Trade Hub.

**Scientific Classification:**
- **Phylum:** Energeticus
- **Class:** Plasmidae
- **Habitat:** Shallow to medium-depth plasma flows
- **Diet:** Absorbs ambient cosmic radiation

---

## 📊 Data Fields (LLM-Friendly)

```json
{
  "id": "nebula_organism_001",
  "acnh_data": {
    "name": "Sea Bass",
    "category": "fish",
    "sell_price": 400,
    "location": "Ocean",
    "availability": {
      "months_northern": [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12],
      "months_southern": [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12],
      "time": "All day"
    },
    "size": "Large",
    "rarity": "Common"
  },
  "cosmic_data": {
    "name": "Plasma Eel",
    "category": "nebula_organism",
    "lore": "A common energy-based life form that undulates through plasma flows. Bioluminescent and easily spotted.",
    "sell_price_credits": 400,
    "location": "Plasma Flows, Cosmic Ocean",
    "planet_zones": ["Plasma Flows", "Deep Plasma Trenches"],
    "spawn_conditions": {
      "time_of_day": "All day",
      "weather": ["Any"],
      "season": "All seasons"
    },
    "energy_signature": "Large",
    "rarity": "Common",
    "visual_theme": {
      "color_palette": ["#4A90E2", "#7ED4FF", "#00FFFF"],
      "particle_effects": ["glowing", "trailing particles", "electric pulse"],
      "model_style": "Elongated serpentine form with translucent body and visible energy core"
    }
  },
  "technical_implementation": {
    "primary_class": "PickableItem",
    "scriptable_object_type": "InventoryItem",
    "required_components": [
      "SpriteRenderer",
      "Collider2D",
      "Rigidbody2D"
    ],
    "setup_steps": [
      {
        "step": 1,
        "description": "Create a new InventoryItem ScriptableObject",
        "code_example": "Assets/Resources/Inventory/NebuleOrganisms/PlasmaEel.asset"
      },
      {
        "step": 2,
        "description": "Configure PickableItem component on prefab",
        "code_example": "Set ItemID to 'plasma_eel', Quantity to 1"
      },
      {
        "step": 3,
        "description": "Set up fishing spawn system",
        "code_example": "Add to FishingSpawnTable with 40% weight in Plasma Flow zones"
      },
      {
        "step": 4,
        "description": "Configure visual effects",
        "code_example": "Attach ParticleSystem for glow trail, set color to cyan gradient"
      }
    ],
    "prefab_structure": {
      "PlasmaEel_Prefab": {
        "components": ["PickableItem", "SpriteRenderer", "Collider2D"],
        "children": [
          {
            "name": "GlowEffect",
            "components": ["ParticleSystem", "Light2D"]
          },
          {
            "name": "EnergyCore",
            "components": ["SpriteRenderer (glowing core)"]
          }
        ]
      }
    },
    "integration_notes": "When caught, trigger MMFeedback with screen flash and plasma ripple effect. Play energy absorption sound. Add to Galactic Archive collection tracking."
  },
  "wiki_metadata": {
    "nookipedia_url": "https://nookipedia.com/wiki/Sea_Bass",
    "created_date": "2026-02-16",
    "last_updated": "2026-02-16",
    "tags": ["fish", "common", "plasma", "all-day", "ocean", "energy-organism"],
    "related_items": [
      "plasma_seiner",
      "galactic_archive",
      "trade_hub",
      "deep_void_leviathan"
    ]
  }
}
```

---

## 🎮 Technical Implementation (Unity + TopDown Engine)

### Primary Class: `PickableItem`

**Setup Overview:**
The Plasma Eel is implemented as a PickableItem that spawns during fishing interactions. When the player successfully completes the fishing minigame in a Plasma Flow zone, the spawn system rolls on the FishingSpawnTable and can spawn a Plasma Eel based on its weight percentage.

### Step-by-Step Configuration

#### 1. Create the InventoryItem ScriptableObject

Navigate to: `Assets/Resources/Inventory/NebulaOrganisms/`

Right-click → Create → TopDown Engine → InventoryItem

**Settings:**
- **Item Name:** Plasma Eel
- **Item ID:** `plasma_eel`
- **Item Class:** NebulaOrganism
- **Max Stack:** 99
- **Icon:** PlasmaEel_Icon.png
- **Sell Price:** 400
- **Can Be Sold:** Yes
- **Can Be Dropped:** No (energy organisms dissipate)

#### 2. Create the Prefab

Create a new GameObject: `PlasmaEel_Catchable`

**Components:**
- **Transform:** Default position
- **SpriteRenderer:**
  - Sprite: PlasmaEel_Sprite
  - Color: Cyan tint (#4A90E2)
  - Sorting Layer: Items
- **PickableItem:**
  - Item to Pick: Reference to PlasmaEel InventoryItem
  - Quantity: 1
  - Pick Feedback: PlaySoundFeedback + ParticleFeedback

#### 3. Add Visual Effects

Create child object: `GlowEffect`
- **ParticleSystem:**
  - Duration: 2.0
  - Start Color: Cyan gradient
  - Emission Rate: 20
  - Shape: Sphere
  - Size Over Lifetime: Decreasing curve
- **Light2D:**
  - Color: Cyan
  - Intensity: 0.8
  - Radius: 3.0

#### 4. Configure Fishing Integration

Open: `FishingManager.cs` or your fishing system

Add to spawn table:
```csharp
public FishingSpawnEntry[] plasmaFlowSpawnTable = new FishingSpawnEntry[] {
    new FishingSpawnEntry {
        itemPrefab = plasmaEelPrefab,
        spawnWeight = 40, // 40% chance
        minimumFishingLevel = 0,
        requiredZone = "PlasmaFlow"
    }
};
```

### Code Example

```csharp
using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.InventoryEngine;

/// <summary>
/// Specialized PickableItem for Nebula Organisms with energy absorption effects
/// </summary>
public class NebulaOrganismPickable : PickableItem
{
    [Header("Nebula Organism Settings")]
    public ParticleSystem energyAbsorptionEffect;
    public AudioClip energyCollectionSound;

    protected override void Pick(GameObject picker)
    {
        // Play energy absorption visual effect
        if (energyAbsorptionEffect != null)
        {
            energyAbsorptionEffect.Play();
        }

        // Play collection sound
        if (energyCollectionSound != null)
        {
            MMSoundManagerSoundPlayEvent.Trigger(energyCollectionSound,
                MMSoundManager.MMSoundManagerTracks.Sfx,
                this.transform.position);
        }

        // Add to Galactic Archive if first time collecting
        GalacticArchive.Instance.RegisterSpecimen(ItemID);

        // Standard pickup behavior
        base.Pick(picker);
    }
}
```

### Prefab Structure
```
PlasmaEel_Catchable (GameObject)
├─ PickableItem (Component)
├─ SpriteRenderer (Component)
├─ BoxCollider2D (Component)
├─ Rigidbody2D (Component - Kinematic)
│
├─ GlowEffect (Child GameObject)
│  ├─ ParticleSystem
│  └─ Light2D
│
└─ EnergyCore (Child GameObject)
   └─ SpriteRenderer (Glowing core sprite)
```

---

## 🔄 Translation Actions

### 🌟 Translate Lore
**Original ACNH Description:**
> "A sea bass! No, wait—it's at least a C+!"

**Cosmic Translation:**
> "A Plasma Eel! Common, but its energy signature is stable—at least C-grade material."

**Translation Guide:**
- Sea → Plasma
- Ocean life → Energy-based organism
- Fishing pun → Sci-fi academic grading system
- Maintain the "common but useful" theme
- Keep the slightly humorous tone

### 📐 Define Additional Data Fields

Need to track more gameplay data? Extend the JSON:

```json
{
  "gameplay_mechanics": {
    "fishing_difficulty": 1,
    "escape_chance": 0.1,
    "minigame_speed": "slow",
    "bait_preference": "energy_lure_basic"
  },
  "galactic_archive_info": {
    "donation_reward_credits": 100,
    "first_time_bonus": true,
    "display_case": "Plasma Flow Exhibit A1"
  },
  "crafting_uses": [
    {
      "recipe": "energy_cell_basic",
      "quantity_required": 3
    }
  ]
}
```

### 🔧 Extend Unity Implementation

**Additional Components to Consider:**

1. **Animation:**
   - Add Animator component with swimming idle animation
   - Create caught/flailing animation for collection

2. **Advanced Physics:**
   - Implement realistic plasma flow physics
   - Add buoyancy simulation in plasma

3. **AI Behavior (Optional):**
   - Add swimming AI for free-roaming eels
   - Implement flee behavior when player approaches

4. **Sound Design:**
   - Ambient electrical hum
   - Collection/absorption sound effect
   - Splash effect when breaking plasma surface

**Example Advanced Setup:**
```csharp
// Add swimming behavior before being caught
public class PlasmaEelBehavior : MonoBehaviour
{
    public float swimSpeed = 2f;
    public float wanderRadius = 5f;

    private Vector2 targetPosition;

    void Start()
    {
        SetRandomTargetPosition();
    }

    void Update()
    {
        // Simple wandering behavior
        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            swimSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, targetPosition) < 0.5f)
        {
            SetRandomTargetPosition();
        }
    }

    void SetRandomTargetPosition()
    {
        Vector2 randomDir = Random.insideUnitCircle * wanderRadius;
        targetPosition = (Vector2)transform.position + randomDir;
    }
}
```

---

## 📎 Related Items

- **[Plasma Seiner](../tools/plasma_seiner.md)** - Tool used to catch Plasma Eels
- **[Galactic Archive](../buildings/galactic_archive.md)** - Donate specimens here
- **[Trade Hub](../buildings/trade_hub.md)** - Sell for 400 Credits
- **[Deep Void Leviathan](./deep_void_leviathan.md)** - Rare predator that hunts Plasma Eels
- **[Energy Cell (Basic)](../materials/energy_cell_basic.md)** - Crafted using Plasma Eels

---

## 🏷️ Tags

`nebula-organism` `common` `plasma-flow` `all-day` `fishing` `energy-based` `bioluminescent` `beginner-friendly`

---

<sub>**Wiki Metadata:**</sub>
<sub>Created: 2026-02-16 | Last Updated: 2026-02-16 | ID: `nebula_organism_001`</sub>

---

## 🤖 LLM Agent Integration

### Query Examples for Claude Code:

```bash
# Finding this item data
Read the file: CosmicWiki/pages/nebula_organisms/plasma_eel.md

# Getting JSON data only
Grep for "```json" in CosmicWiki/pages/nebula_organisms/plasma_eel.md -A 100

# Implementation reference
Grep for "Technical Implementation" in CosmicWiki/pages/nebula_organisms/plasma_eel.md -A 50

# Find all common rarity organisms
Grep for '"rarity": "Common"' in CosmicWiki/pages/nebula_organisms/*.md
```

### Agent Usage Pattern:
1. **Research Phase**: Read this wiki page to understand the Plasma Eel
2. **Planning Phase**: Extract JSON data and implementation notes
3. **Implementation Phase**: Follow step-by-step technical guide to create the InventoryItem and prefab
4. **Validation Phase**: Check against data fields for completeness (all required components, correct pricing, etc.)

### Integration with Game Code:

When implementing fishing mechanics, reference this page:
```csharp
// In your FishingSystem.cs
// Reference: CosmicWiki/pages/nebula_organisms/plasma_eel.md

public void GenerateCatch(string zoneType)
{
    if (zoneType == "PlasmaFlow")
    {
        // 40% chance for Plasma Eel (common)
        // See wiki for full spawn table
    }
}
```

---

**Need to update this page?**
Follow the schema at: `CosmicWiki/data/schemas/item_schema.json`
