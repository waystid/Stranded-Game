# Claude Code Agent Integration Guide

> **🤖 How to use the Cosmic Colony Wiki with Claude Code agents**

This guide explains how Claude Code agents should interact with the Cosmic Colony Wiki when implementing game features.

---

## Table of Contents

1. [Overview](#overview)
2. [Wiki Structure](#wiki-structure)
3. [Query Patterns](#query-patterns)
4. [Implementation Workflow](#implementation-workflow)
5. [Code Generation Guidelines](#code-generation-guidelines)
6. [Best Practices](#best-practices)

---

## Overview

The Cosmic Colony Wiki is designed to be **LLM-friendly**, meaning it's structured to be easily parsed and understood by AI agents like Claude Code. Each wiki page contains:

- **Structured JSON data** (machine-readable)
- **Markdown documentation** (human-readable)
- **Unity implementation guides** (code examples)
- **Lore and translation notes** (creative context)

### Design Philosophy

1. **Single Source of Truth**: The wiki is the authoritative reference for all game content
2. **Separation of Concerns**: Data (JSON) is separate from presentation (Markdown)
3. **Implementation-Ready**: Every item includes complete Unity/TopDown Engine setup instructions
4. **Discoverable**: Consistent structure makes searching and parsing easy

---

## Wiki Structure

```
CosmicWiki/
├── data/
│   ├── acnh_cosmic_mapping.json      # Master mapping file
│   └── schemas/
│       └── item_schema.json          # JSON schema for validation
├── pages/
│   ├── nebula_organisms/             # Fish equivalents
│   ├── micro_drones/                 # Bug equivalents
│   ├── ancient_artifacts/            # Fossil equivalents
│   ├── tools/                        # Tools
│   ├── materials/                    # Crafting materials
│   ├── furniture/                    # Furniture items
│   ├── buildings/                    # Structures
│   └── npcs/                         # Characters
├── templates/
│   └── wiki_page_template.md         # Page template
├── scripts/
│   ├── generate_wiki_page.py         # Page generator
│   └── generate_index.py             # Index generator
└── guides/
    └── CLAUDE_CODE_INTEGRATION.md    # This file
```

---

## Query Patterns

### Basic Queries

#### Find a Specific Item

```bash
# By Cosmic name
Read CosmicWiki/pages/nebula_organisms/plasma_eel.md

# By ACNH name
Grep for "ACNH Equivalent.*Sea Bass" in CosmicWiki/pages/**/*.md
```

#### Get JSON Data Only

```bash
# Extract JSON block from a page
Grep for "```json" in CosmicWiki/pages/nebula_organisms/plasma_eel.md -A 100
```

#### Find Items by Category

```bash
# All fishing-related items
Glob CosmicWiki/pages/nebula_organisms/*.md

# All tools
Glob CosmicWiki/pages/tools/*.md
```

### Advanced Queries

#### Find by Rarity

```bash
# All rare items
Grep for '"rarity": "Rare"' in CosmicWiki/pages/**/*.md

# All common items
Grep for '"rarity": "Common"' in CosmicWiki/pages/**/*.md
```

#### Find by Price Range

```bash
# Items worth 1000+ Credits
Grep for '"sell_price_credits": [0-9]{4,}' in CosmicWiki/pages/**/*.md
```

#### Find by Implementation Class

```bash
# All PickableItems
Grep for '"primary_class": "PickableItem"' in CosmicWiki/pages/**/*.md

# All Weapons (tools)
Grep for '"primary_class": "Weapon"' in CosmicWiki/pages/**/*.md
```

#### Find by Tag

```bash
# All beginner-friendly items
Grep for "beginner" in CosmicWiki/pages/**/*.md

# All craftable items
Grep for "craftable" in CosmicWiki/pages/**/*.md
```

---

## Implementation Workflow

When implementing a game feature based on wiki content, follow this workflow:

### 1. Research Phase

**Goal:** Understand what you're building

```bash
# Example: Implementing the Plasma Seiner (fishing rod)

# Step 1: Read the full wiki page
Read CosmicWiki/pages/tools/plasma_seiner.md

# Step 2: Check related items
Read CosmicWiki/pages/nebula_organisms/plasma_eel.md

# Step 3: Review the mapping reference
Read CosmicWiki/data/acnh_cosmic_mapping.json
```

**What to look for:**
- Primary class and components needed
- Dependencies (other items, systems)
- Lore context (for naming, visual design)
- Related items that need to work together

### 2. Planning Phase

**Goal:** Create implementation checklist

Extract the following from the wiki page:

1. **Required Components** (from `technical_implementation.required_components`)
2. **Setup Steps** (from `technical_implementation.setup_steps`)
3. **Dependencies** (from `wiki_metadata.related_items`)

**Example:**

```json
// From plasma_seiner.md
{
  "required_components": [
    "CharacterHandleWeapon",
    "FishingToolAbility",
    "DurabilitySystem",
    "MMFeedbacks"
  ],
  "dependencies": [
    "FishingZone system",
    "Spawn tables",
    "Minigame UI"
  ]
}
```

**Create a checklist:**

- [ ] Create WeaponItem ScriptableObject
- [ ] Implement FishingToolAbility.cs
- [ ] Create FishingZone.cs component
- [ ] Build FishingMinigameUI prefab
- [ ] Set up ToolDurability system
- [ ] Create spawn tables
- [ ] Add tutorial sequence

### 3. Implementation Phase

**Goal:** Write the code

Follow the step-by-step guide in the wiki page's "Technical Implementation" section.

**Key sections to reference:**

1. **Setup Overview** - High-level architecture
2. **Step-by-Step Configuration** - Detailed instructions
3. **Code Examples** - Copy-paste starting points
4. **Prefab Structure** - GameObject hierarchy

**Tips:**

- Don't skip steps
- Use the provided code examples as templates
- Follow the prefab structure exactly
- Check `integration_notes` for important gotchas

### 4. Validation Phase

**Goal:** Ensure completeness

```bash
# Check that all JSON fields are implemented
# Example: Verify Plasma Eel has all required properties

# 1. Get the JSON data
Grep for "```json" in CosmicWiki/pages/nebula_organisms/plasma_eel.md -A 100

# 2. Check your ScriptableObject has matching fields
# 3. Verify sell price, rarity, spawn conditions, etc.
```

**Validation checklist:**

- [ ] All JSON fields are represented in the ScriptableObject
- [ ] Visual theme matches (colors, effects)
- [ ] Sell price is correct
- [ ] Rarity/spawn rates match
- [ ] Related items are connected (crafting recipes, etc.)

---

## Code Generation Guidelines

### Using Wiki Code Examples

Wiki pages include code examples. Here's how to use them:

#### 1. Code Examples Are Templates

```csharp
// ❌ DON'T: Copy-paste without understanding
public class FishingToolAbility : CharacterAbility
{
    // Copied code without modifications
}

// ✅ DO: Adapt to your specific needs
public class FishingToolAbility : CharacterAbility
{
    [Header("Fishing Settings")]
    public float maxCastDistance = 5f;  // Adjusted for your game scale
    public LayerMask fishingZoneMask;   // Set in inspector

    // ... customize further ...
}
```

#### 2. Follow the Architecture Pattern

```csharp
// The wiki shows this pattern for tools:
// Tool (Weapon) → Ability (CharacterAbility) → Minigame/System

// Always maintain this separation:
// - Tool = the item data
// - Ability = the player action
// - System = the game mechanic
```

#### 3. Extend, Don't Replace

```csharp
// Wiki provides base implementation
public class FishingToolAbility : CharacterAbility
{
    public void CastLine() { /* ... */ }
}

// You can extend with advanced features
public class AdvancedFishingToolAbility : FishingToolAbility
{
    public override void CastLine()
    {
        // Add skill-based accuracy check
        base.CastLine();
    }
}
```

### Integrating Multiple Wiki Items

When implementing features that involve multiple wiki items:

**Example: Fishing System**

1. **Plasma Seiner** (tool) → Read `tools/plasma_seiner.md`
2. **Plasma Eel** (catchable) → Read `nebula_organisms/plasma_eel.md`
3. **Galactic Archive** (building) → Read `buildings/galactic_archive.md`

**Integration points to check:**

```bash
# Find all "related_items" references
Grep for "plasma_seiner" in CosmicWiki/pages/**/*.md

# Check for shared systems
Grep for "FishingZone" in CosmicWiki/pages/**/*.md
```

**Create shared systems:**

```csharp
// FishingManager.cs - Coordinates all fishing-related items
public class FishingManager : MonoBehaviour
{
    // References from wiki pages:
    public FishingSpawnTable[] spawnTables;  // From plasma_eel.md
    public FishingZone[] zones;              // From plasma_seiner.md
    public GalacticArchive archive;          // From galactic_archive.md

    // Coordinate interactions
}
```

---

## Best Practices

### 1. Always Start with the Wiki

```
❌ DON'T: Implement features based on assumptions
✅ DO: Read the wiki page first, then implement
```

**Why:** The wiki contains design decisions, lore context, and technical requirements that ensure consistency across the game.

### 2. Respect the ACNH → Cosmic Mapping

```bash
# Check the master mapping
Read CosmicWiki/data/acnh_cosmic_mapping.json
```

**Why:** The mapping ensures thematic consistency. Don't invent new names or systems without checking the mapping first.

### 3. Update the Wiki When Extending

If you implement features beyond what's documented:

```python
# Update the wiki page
python CosmicWiki/scripts/generate_wiki_page.py --data updated_data.json

# Regenerate the index
python CosmicWiki/scripts/generate_index.py
```

**Why:** The wiki should always reflect the current state of the game.

### 4. Use JSON Data for Validation

```csharp
// In your tests, validate against wiki data
[Test]
public void PlasmaEel_HasCorrectSellPrice()
{
    var wikiData = LoadWikiJSON("plasma_eel");
    var item = Resources.Load<InventoryItem>("PlasmaEel");

    Assert.AreEqual(
        wikiData["cosmic_data"]["sell_price_credits"],
        item.SellPrice
    );
}
```

**Why:** Keeps implementation in sync with design.

### 5. Follow the Prefab Structure

```
✅ DO: Match the wiki's prefab hierarchy exactly

PlasmaEel_Catchable
├─ PickableItem
├─ SpriteRenderer
├─ BoxCollider2D
└─ GlowEffect (child)
   ├─ ParticleSystem
   └─ Light2D

❌ DON'T: Reorganize without documenting why
```

**Why:** Consistent structure makes debugging and collaboration easier.

---

## Example: Complete Implementation Flow

Let's walk through implementing the **Plasma Eel** from start to finish.

### Step 1: Research

```bash
# Read the wiki page
Read CosmicWiki/pages/nebula_organisms/plasma_eel.md

# Check related items
Read CosmicWiki/pages/tools/plasma_seiner.md
Read CosmicWiki/data/acnh_cosmic_mapping.json
```

**Key findings:**
- Primary class: `PickableItem`
- Sell price: 400 Credits
- Spawns in: Plasma Flows
- Spawn weight: 40% (common)
- Requires: Fishing system

### Step 2: Plan

**Checklist:**
- [ ] Create InventoryItem ScriptableObject
- [ ] Create catchable prefab
- [ ] Add to fishing spawn table
- [ ] Configure visual effects
- [ ] Set up MMFeedback
- [ ] Test fishing interaction

### Step 3: Implement

#### A. Create ScriptableObject

```
Assets → Create → TopDown Engine → InventoryItem

Settings (from wiki JSON):
- Name: "Plasma Eel"
- Item ID: "plasma_eel"
- Max Stack: 99
- Sell Price: 400
- Icon: PlasmaEel_Icon
```

#### B. Create Prefab

```
1. Create GameObject: "PlasmaEel_Catchable"
2. Add components:
   - PickableItem (item = PlasmaEel ScriptableObject)
   - SpriteRenderer (sprite = PlasmaEel_Sprite, color = #4A90E2)
   - BoxCollider2D
3. Create child "GlowEffect":
   - ParticleSystem (cyan, 20 emission rate)
   - Light2D (cyan, intensity 0.8)
```

#### C. Add to Spawn Table

```csharp
// In FishingManager.cs
public FishingSpawnEntry[] plasmaFlowTable = new[] {
    new FishingSpawnEntry {
        itemPrefab = plasmaEelPrefab,
        spawnWeight = 40,  // From wiki: 40% chance
        minimumLevel = 0,
        requiredZone = "PlasmaFlow"
    }
};
```

### Step 4: Validate

```
✅ Sell price: 400 Credits (matches wiki)
✅ Rarity: Common (matches wiki)
✅ Spawn weight: 40% (matches wiki)
✅ Visual: Cyan glow (matches wiki color_palette)
✅ Can be donated to Galactic Archive (matches related_items)
```

---

## Troubleshooting

### "I can't find the item I need"

```bash
# Search by ACNH name
Grep for "ACNH Equivalent.*fishing rod" in CosmicWiki/pages/**/*.md

# Search by Cosmic name
find CosmicWiki/pages -name "*plasma*.md"

# Check the index
Read CosmicWiki/WIKI_INDEX.md
```

### "The wiki is missing information"

1. Check if the page exists:
   ```bash
   ls CosmicWiki/pages/[category]/
   ```

2. If it doesn't exist, create it:
   ```bash
   python CosmicWiki/scripts/generate_wiki_page.py --interactive
   ```

3. Update the index:
   ```bash
   python CosmicWiki/scripts/generate_index.py
   ```

### "The code example doesn't work"

1. **Check TopDown Engine version**: Code examples assume TDE 2.x
2. **Verify namespace imports**: Ensure you have `using MoreMountains.TopDownEngine;`
3. **Adapt, don't copy**: Code examples are templates, not production code

---

## Quick Reference Commands

### Finding Items

```bash
# By name
Grep for "Plasma Eel" in CosmicWiki/pages/**/*.md

# By ACNH equivalent
Grep for "Sea Bass" in CosmicWiki/pages/**/*.md

# By category
ls CosmicWiki/pages/nebula_organisms/

# By class
Grep for '"primary_class": "PickableItem"' in CosmicWiki/pages/**/*.md
```

### Extracting Data

```bash
# Get JSON only
Grep for "```json" in CosmicWiki/pages/nebula_organisms/plasma_eel.md -A 100

# Get implementation code
Grep for "Technical Implementation" in CosmicWiki/pages/tools/plasma_seiner.md -A 50

# Get all related items
Grep for "related_items" in CosmicWiki/pages/**/*.md
```

### Updating Wiki

```bash
# Generate new page
python CosmicWiki/scripts/generate_wiki_page.py --data my_item.json

# Interactive creation
python CosmicWiki/scripts/generate_wiki_page.py --interactive

# Rebuild index
python CosmicWiki/scripts/generate_index.py
```

---

## Summary

The Cosmic Colony Wiki is your **single source of truth** for:
- Game data (prices, rarities, stats)
- Lore and theming
- Technical implementation
- ACNH translation mapping

**Always:**
1. Read the wiki before implementing
2. Follow the technical guide
3. Validate against JSON data
4. Update the wiki when extending

**Remember:**
- Wiki = Design Document + Implementation Guide
- JSON = Machine-readable data
- Markdown = Human-readable documentation
- Code Examples = Implementation templates

---

**Happy coding! 🚀**

<sub>For questions or wiki updates, refer to `CosmicWiki/README.md`</sub>
