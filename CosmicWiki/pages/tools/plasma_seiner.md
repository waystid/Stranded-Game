# Plasma Seiner

> **ACNH Equivalent:** [Fishing Rod](https://nookipedia.com/wiki/Fishing_Rod)

---

## Quick Reference

| Property | ACNH Value | Cosmic Colony Value |
|----------|-----------|---------------------|
| **Name** | Fishing Rod | Plasma Seiner |
| **Category** | Tool | Gathering Tool |
| **Buy Price** | 400 Bells (Flimsy)<br>Crafted (Normal) | 400 Credits (Basic)<br>Crafted (Standard) |
| **Durability** | 10 uses (Flimsy)<br>30 uses (Normal) | 10 uses (Basic)<br>30 uses (Standard) |
| **Tool Type** | Fishing | Energy Organism Capture |
| **Crafted From** | 5 Tree Branch | 5 Bio-Fiber |

---

## 🌌 Cosmic Lore

The **Plasma Seiner** is an electromagnetic field projector disguised as a handheld device. It generates a localized containment field that can safely capture energy-based organisms from plasma flows and nebula pools without destabilizing their molecular structure.

### In-Universe Context

Developed by the Galactic Fisheries Commission, the Plasma Seiner represents centuries of xenobiology research. The device works by:
1. **Scanning** the plasma for energy signatures
2. **Projecting** an attractive electromagnetic pulse (the "cast")
3. **Engaging** in a resonance battle with the organism
4. **Containing** the organism in a stable energy matrix

Colonists quickly learn that patience and timing are essential—rushing the resonance phase can cause the organism to destabilize and escape.

**Z.O.E.'s DIY Workshop Description:**
> "Every good colonist needs a reliable Plasma Seiner. This basic model won't last forever, but it's perfect for learning the art of energy fishing!"

---

## 📊 Data Fields (LLM-Friendly)

```json
{
  "id": "tool_plasma_seiner_001",
  "acnh_data": {
    "name": "Fishing Rod",
    "category": "tool",
    "variants": [
      {
        "name": "Flimsy Fishing Rod",
        "buy_price": 400,
        "durability": 10,
        "crafting": ["5x Tree Branch"]
      },
      {
        "name": "Fishing Rod",
        "durability": 30,
        "crafting": ["1x Flimsy Fishing Rod", "1x Iron Nugget"]
      },
      {
        "name": "Golden Fishing Rod",
        "durability": 90,
        "unlock_condition": "Catch all fish",
        "crafting": ["1x Fishing Rod", "1x Gold Nugget"]
      }
    ],
    "primary_function": "Catch fish from rivers, ponds, and ocean",
    "gameplay_mechanic": "Cast line, wait for bite, press A when bobber goes under"
  },
  "cosmic_data": {
    "name": "Plasma Seiner",
    "category": "gathering_tool",
    "variants": [
      {
        "name": "Basic Plasma Seiner",
        "buy_price_credits": 400,
        "durability": 10,
        "crafting": ["5x Bio-Fiber"],
        "color_scheme": "Gray and Blue"
      },
      {
        "name": "Standard Plasma Seiner",
        "durability": 30,
        "crafting": ["1x Basic Plasma Seiner", "1x Ferrite Core"],
        "color_scheme": "Silver and Cyan"
      },
      {
        "name": "Quantum Plasma Seiner",
        "durability": 90,
        "unlock_condition": "Register all Nebula Organisms in Galactic Archive",
        "crafting": ["1x Standard Plasma Seiner", "1x Aurelium Crystal"],
        "color_scheme": "Gold and Purple",
        "special_ability": "Never breaks during catch attempts"
      }
    ],
    "primary_function": "Capture Nebula Organisms from Plasma Flows and Nebula Pools",
    "gameplay_mechanic": "Project field, wait for organism to approach, engage in resonance minigame",
    "lore": "Electromagnetic containment device for safely capturing energy-based life forms",
    "visual_theme": {
      "model_style": "Handheld device with holographic projection array",
      "particle_effects": ["electromagnetic field projection", "energy ripples", "resonance pulses"],
      "sound_design": ["casting hum", "organism bite beep", "resonance struggle sounds", "capture confirmation"]
    }
  },
  "technical_implementation": {
    "primary_class": "Weapon",
    "scriptable_object_type": "WeaponItem (extended as FishingTool)",
    "required_components": [
      "CharacterHandleWeapon",
      "FishingToolAbility",
      "DurabilitySystem",
      "MMFeedbacks"
    ],
    "setup_steps": [
      {
        "step": 1,
        "description": "Create WeaponItem ScriptableObject for Plasma Seiner",
        "code_example": "Create in Assets/Resources/Weapons/Tools/PlasmaSeiner.asset"
      },
      {
        "step": 2,
        "description": "Set up FishingToolAbility component",
        "code_example": "Inherits from CharacterAbility, handles cast, bite detection, minigame"
      },
      {
        "step": 3,
        "description": "Create durability system that decreases on successful catch",
        "code_example": "DurabilitySystem.cs tracks uses, breaks tool at 0"
      },
      {
        "step": 4,
        "description": "Design fishing minigame UI and logic",
        "code_example": "Resonance bar, timing window, difficulty scaling"
      },
      {
        "step": 5,
        "description": "Integrate with spawn system for Nebula Organisms",
        "code_example": "FishingZone triggers spawn based on zone type and rarity tables"
      }
    ],
    "prefab_structure": {
      "PlasmaSeiner_Equipped": {
        "parent": "Player Hand Attachment",
        "components": ["SpriteRenderer", "Animator"],
        "children": [
          {
            "name": "EMField_Projectile",
            "components": ["Projectile", "ParticleSystem"],
            "description": "The cast line/field projection"
          },
          {
            "name": "Bobber_Visual",
            "components": ["SpriteRenderer", "Animator"],
            "description": "Visual indicator in plasma"
          }
        ]
      }
    },
    "integration_notes": "The Plasma Seiner should be the first tool the player crafts. Tutorial should teach: 1) How to equip, 2) How to identify fishing zones, 3) Casting mechanics, 4) Minigame timing, 5) Catching and collecting"
  },
  "wiki_metadata": {
    "nookipedia_url": "https://nookipedia.com/wiki/Fishing_Rod",
    "created_date": "2026-02-16",
    "last_updated": "2026-02-16",
    "tags": ["tool", "fishing", "gathering", "craftable", "durability", "progression"],
    "related_items": [
      "plasma_eel",
      "bio_fiber",
      "ferrite_core",
      "aurelium_crystal",
      "galactic_archive"
    ]
  }
}
```

---

## 🎮 Technical Implementation (Unity + TopDown Engine)

### Primary Class: `Weapon` (Extended as Fishing Tool)

**Setup Overview:**
The Plasma Seiner is implemented as a specialized Weapon that doesn't deal damage but instead triggers a fishing minigame. It uses the TopDown Engine's CharacterHandleWeapon system for equipping and the Projectile system for casting the electromagnetic field.

### Step-by-Step Configuration

#### 1. Create the WeaponItem ScriptableObject

Navigate to: `Assets/Resources/Weapons/Tools/`

Right-click → Create → TopDown Engine → WeaponItem

**Settings:**
- **Item Name:** Plasma Seiner (Basic)
- **Item ID:** `plasma_seiner_basic`
- **Item Class:** GatheringTool
- **Target Uses:** 10 (durability)
- **Weapon Type:** Projectile
- **Weapon Settings:**
  - Projectile Spawn Offset: (0.5, 0, 0)
  - Projectile Speed: 8
  - Reload Time: 1.5s

#### 2. Create Custom Fishing Tool Ability

Create new C# script: `FishingToolAbility.cs`

This extends `CharacterAbility` and handles:
- Detecting fishing zones
- Casting the line
- Waiting for bite
- Triggering minigame
- Handling success/failure

```csharp
using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

public class FishingToolAbility : CharacterAbility
{
    [Header("Fishing Settings")]
    public float maxCastDistance = 5f;
    public LayerMask fishingZoneMask;
    public GameObject bobberPrefab;

    private GameObject currentBobber;
    private FishingZone currentZone;
    private bool lineIsCast = false;
    private bool waitingForBite = false;

    protected override void HandleInput()
    {
        if (_inputManager.ShootButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
        {
            if (!lineIsCast)
            {
                CastLine();
            }
            else if (waitingForBite)
            {
                // Player pressed too early, scare away organism
                ReelInEmpty();
            }
        }

        if (_inputManager.ShootButton.State.CurrentState == MMInput.ButtonStates.ButtonUp)
        {
            if (waitingForBite && currentZone != null && currentZone.HasBite)
            {
                // Correct timing! Start minigame
                StartFishingMinigame();
            }
        }
    }

    public void CastLine()
    {
        // Raycast to find fishing zone
        Vector2 aimDirection = _character.IsFacingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            aimDirection,
            maxCastDistance,
            fishingZoneMask
        );

        if (hit.collider != null)
        {
            currentZone = hit.collider.GetComponent<FishingZone>();
            if (currentZone != null)
            {
                // Spawn bobber at hit point
                currentBobber = Instantiate(bobberPrefab, hit.point, Quaternion.identity);
                lineIsCast = true;
                waitingForBite = true;

                // Trigger zone to start bite timer
                currentZone.StartBiteTimer();
            }
        }
    }

    public void StartFishingMinigame()
    {
        // Load minigame UI
        FishingMinigameUI.Instance.StartMinigame(
            currentZone.GetRandomCatch(),
            OnMinigameSuccess,
            OnMinigameFailure
        );
    }

    private void OnMinigameSuccess(GameObject caughtItem)
    {
        // Add item to inventory
        // Decrease tool durability
        // Play success feedback
        ReelIn();
    }

    private void OnMinigameFailure()
    {
        // Play failure feedback
        // Decrease tool durability (optional)
        ReelIn();
    }

    private void ReelIn()
    {
        if (currentBobber != null)
        {
            Destroy(currentBobber);
        }
        lineIsCast = false;
        waitingForBite = false;
        currentZone = null;
    }

    private void ReelInEmpty()
    {
        // Scared away the organism
        MMFeedbacksEvent.Trigger(/* failure feedback */);
        ReelIn();
    }
}
```

#### 3. Create Fishing Zone System

Create `FishingZone.cs` component for plasma flows and nebula pools:

```csharp
using UnityEngine;
using System.Collections;

public class FishingZone : MonoBehaviour
{
    [Header("Zone Settings")]
    public string zoneType = "PlasmaFlow"; // or "NebulaPool"
    public FishingSpawnTable spawnTable;

    [Header("Bite Timing")]
    public float minBiteTime = 2f;
    public float maxBiteTime = 10f;

    public bool HasBite { get; private set; }

    private Coroutine biteCoroutine;

    public void StartBiteTimer()
    {
        if (biteCoroutine != null)
        {
            StopCoroutine(biteCoroutine);
        }
        biteCoroutine = StartCoroutine(WaitForBite());
    }

    private IEnumerator WaitForBite()
    {
        HasBite = false;
        float waitTime = Random.Range(minBiteTime, maxBiteTime);
        yield return new WaitForSeconds(waitTime);

        // BITE!
        HasBite = true;
        // Play visual/audio feedback (bobber dips, beep sound)
        MMFeedbacksEvent.Trigger(/* bite feedback */);
    }

    public GameObject GetRandomCatch()
    {
        return spawnTable.GetRandomCatch(zoneType);
    }
}
```

#### 4. Create Fishing Minigame UI

The minigame shows a resonance bar with a moving target zone. Player must press/hold button to keep the marker in the zone.

**UI Elements:**
- Background bar
- Safe zone indicator (moves based on organism difficulty)
- Player marker (controlled by button presses)
- Timer bar

**Logic:**
```csharp
public class FishingMinigameUI : MonoBehaviour
{
    [Header("Minigame Settings")]
    public float minigameDuration = 5f;
    public float successThreshold = 70f; // Percent of time in safe zone

    private float timeInZone = 0f;
    private float totalTime = 0f;
    private bool minigameActive = false;

    public void StartMinigame(GameObject targetCatch, System.Action<GameObject> onSuccess, System.Action onFailure)
    {
        // Initialize UI
        // Start timer
        StartCoroutine(RunMinigame(targetCatch, onSuccess, onFailure));
    }

    private IEnumerator RunMinigame(GameObject targetCatch, System.Action<GameObject> onSuccess, System.Action onFailure)
    {
        minigameActive = true;
        totalTime = 0f;
        timeInZone = 0f;

        while (totalTime < minigameDuration)
        {
            totalTime += Time.deltaTime;

            // Check if player marker is in safe zone
            if (IsMarkerInSafeZone())
            {
                timeInZone += Time.deltaTime;
            }

            // Update UI
            UpdateUI();

            yield return null;
        }

        minigameActive = false;

        // Calculate success
        float successPercent = (timeInZone / totalTime) * 100f;
        if (successPercent >= successThreshold)
        {
            onSuccess?.Invoke(targetCatch);
        }
        else
        {
            onFailure?.Invoke();
        }

        // Close UI
    }
}
```

#### 5. Durability System

Create `ToolDurability.cs`:

```csharp
using UnityEngine;
using MoreMountains.InventoryEngine;

public class ToolDurability : MonoBehaviour
{
    public int maxUses = 10;
    public int currentUses = 10;

    public void DecreaseDurability()
    {
        currentUses--;

        if (currentUses <= 0)
        {
            BreakTool();
        }
    }

    private void BreakTool()
    {
        // Play break animation/sound
        // Remove from inventory
        // Notify player
        MMInventoryEvent.Trigger(MMInventoryEventType.Destroy, null, /* tool item */, 1, 0);
    }
}
```

### Complete Code Example

See individual sections above for detailed code. The full integration requires:
1. FishingToolAbility component on player
2. FishingZone components on plasma flow/pool colliders
3. FishingMinigameUI canvas
4. ToolDurability tracking
5. Spawn tables for each zone type

### Prefab Structure
```
PlasmaSeiner_Equipped
├─ SpriteRenderer (Tool sprite)
├─ Animator (Equip/cast animations)
│
├─ CastPoint (Empty GameObject)
│  └─ Transform (Position for projectile spawn)
│
└─ EM_Field_Prefab (Instantiated on cast)
   ├─ Projectile (Component)
   ├─ SpriteRenderer (Energy field visual)
   ├─ ParticleSystem (Electromagnetic effects)
   └─ Collider2D (Trigger for fishing zones)

Bobber_Prefab (Instantiated at cast location)
├─ SpriteRenderer (Floating indicator)
├─ Animator (Idle bobbing, bite reaction)
├─ ParticleSystem (Plasma ripples)
└─ AudioSource (Splash, bite sounds)
```

---

## 🔄 Translation Actions

### 🌟 Translate Lore

**Original ACNH Description:**
> "A simple fishing rod. Cast your line and wait for a bite!"

**Cosmic Translation:**
> "A basic electromagnetic field projector. Calibrate your resonance and wait for an organism to approach!"

**Translation Guide:**
- Fishing rod → Electromagnetic projector
- Cast line → Project field / Calibrate resonance
- Wait for bite → Wait for organism to approach
- Water/ocean → Plasma flows / energy streams
- Keep the accessible, tutorial-friendly tone

**Puns to Adapt:**
- ACNH: "I caught a sea bass! No, wait—it's at least a C+!"
- Cosmic: "I captured a Plasma Eel! Its energy signature reads... C-grade at best!"

### 📐 Define Additional Data Fields

Extend fishing mechanics with custom data:

```json
{
  "advanced_mechanics": {
    "cast_accuracy": {
      "perfect_cast_bonus": 1.2,
      "poor_cast_penalty": 0.8
    },
    "weather_modifiers": {
      "solar_storm": {"rare_chance": 1.5},
      "meteor_shower": {"legendary_chance": 2.0}
    },
    "skill_progression": {
      "xp_per_catch": 10,
      "level_benefits": [
        {"level": 5, "benefit": "Increased rare catch rate"},
        {"level": 10, "benefit": "Faster bite times"}
      ]
    }
  },
  "cosmetic_variants": [
    {
      "name": "Nebula Pattern Seiner",
      "unlock": "Catch 50 organisms",
      "color_scheme": "Purple and pink"
    }
  ]
}
```

### 🔧 Extend Unity Implementation

**Advanced Features to Add:**

1. **Skill Progression System:**
```csharp
public class FishingSkillSystem : MonoBehaviour
{
    public int fishingLevel = 1;
    public int totalCatches = 0;

    public void OnSuccessfulCatch()
    {
        totalCatches++;
        // Award XP, check for level up
        // Unlock new abilities (faster casts, better odds)
    }
}
```

2. **Perfect Cast Mechanic:**
- Add a timing window for "perfect casts"
- Visual indicator shows optimal cast zone
- Rewards: Higher rarity rolls, instant bites

3. **Multiple Fishing Zones:**
```csharp
public enum FishingZoneType
{
    PlasmaFlow,      // Basic organisms
    NebulaPool,      // Medium rarity
    DeepPlasma,      // Requires upgraded seiner
    CosmicVortex,    // Legendary organisms
    CrystalGeyser    // Special crystalline catches
}
```

4. **Weather Integration:**
- Solar storms increase rare spawns
- Meteor showers unlock special organisms
- Radiation clouds affect bite frequency

---

## 📎 Related Items

- **[Plasma Eel](../nebula_organisms/plasma_eel.md)** - Common catch
- **[Bio-Fiber](../materials/bio_fiber.md)** - Crafting material
- **[Ferrite Core](../materials/ferrite_core.md)** - Upgrade material
- **[Aurelium Crystal](../materials/aurelium_crystal.md)** - Golden version material
- **[Galactic Archive](../buildings/galactic_archive.md)** - Donate catches here
- **[Fishing Zone Guide](../../guides/fishing_zones.md)** - All fishing locations

---

## 🏷️ Tags

`tool` `fishing` `gathering` `craftable` `durability` `progression` `minigame` `essential` `beginner`

---

<sub>**Wiki Metadata:**</sub>
<sub>Created: 2026-02-16 | Last Updated: 2026-02-16 | ID: `tool_plasma_seiner_001`</sub>

---

## 🤖 LLM Agent Integration

### Query Examples for Claude Code:

```bash
# Read full tool implementation
Read CosmicWiki/pages/tools/plasma_seiner.md

# Extract fishing minigame code
Grep for "FishingMinigameUI" in CosmicWiki/pages/tools/plasma_seiner.md -A 30

# Find all fishing-related files
Grep for "fishing" in CosmicWiki/pages/**/*.md

# Get crafting recipe
Grep for "crafting" in CosmicWiki/pages/tools/plasma_seiner.md
```

### Agent Implementation Checklist:

When implementing the Plasma Seiner system, agents should:

- [ ] Create WeaponItem ScriptableObject
- [ ] Implement FishingToolAbility.cs
- [ ] Create FishingZone.cs component
- [ ] Build FishingMinigameUI prefab
- [ ] Set up ToolDurability system
- [ ] Create spawn tables for each zone
- [ ] Add tutorial sequence
- [ ] Configure MMFeedbacks for visual polish
- [ ] Test durability and breakage
- [ ] Verify all zone types spawn correctly

---

**Need to extend this system?**
Reference: `CosmicWiki/data/acnh_cosmic_mapping.json` → tools → fishing_rod
