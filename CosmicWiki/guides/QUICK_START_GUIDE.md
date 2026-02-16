# Cosmic Colony Wiki - Quick Start Guide

> **🚀 Get started with the wiki in 5 minutes**

---

## For Game Designers

### View Existing Content

```bash
# See all available pages
cat CosmicWiki/WIKI_INDEX.md

# Browse by category
ls CosmicWiki/pages/nebula_organisms/
ls CosmicWiki/pages/tools/
ls CosmicWiki/pages/materials/

# Read a specific item
cat CosmicWiki/pages/nebula_organisms/plasma_eel.md
```

### Understand the Translation System

Open the master mapping file:

```bash
cat CosmicWiki/data/acnh_cosmic_mapping.json
```

This file shows how every ACNH element translates to Cosmic Colony:
- **Bells** → **Credits**
- **Fish** → **Nebula Organisms**
- **Fishing Rod** → **Plasma Seiner**
- etc.

---

## For Developers

### 1. Find the Item You Need

```bash
# By ACNH name
grep -r "Sea Bass" CosmicWiki/pages/

# By Cosmic name
grep -r "Plasma Eel" CosmicWiki/pages/

# By category
ls CosmicWiki/pages/tools/
```

### 2. Read the Implementation Guide

```bash
cat CosmicWiki/pages/nebula_organisms/plasma_eel.md
```

Look for the **"Technical Implementation"** section - it has:
- Required Unity components
- Step-by-step setup
- Code examples
- Prefab structure

### 3. Extract the Data

```bash
# Get the JSON data block
grep -A 100 '```json' CosmicWiki/pages/nebula_organisms/plasma_eel.md
```

Use this to:
- Set ScriptableObject values
- Validate your implementation
- Ensure consistency

### 4. Implement

Follow the step-by-step guide in the wiki page.

---

## For Claude Code Agents

### Basic Workflow

```bash
# 1. Research - Read the wiki page
Read CosmicWiki/pages/tools/plasma_seiner.md

# 2. Plan - Extract requirements
Grep for "required_components" in CosmicWiki/pages/tools/plasma_seiner.md

# 3. Implement - Follow the guide
# (Step-by-step in wiki page)

# 4. Validate - Check against data
Grep for "sell_price_credits" in CosmicWiki/pages/tools/plasma_seiner.md
```

### Common Queries

```bash
# Find items by rarity
Grep for '"rarity": "Rare"' in CosmicWiki/pages/**/*.md

# Find items by price
Grep for '"sell_price_credits": 400' in CosmicWiki/pages/**/*.md

# Find items by TopDown Engine class
Grep for '"primary_class": "PickableItem"' in CosmicWiki/pages/**/*.md

# Find all related items
Grep for '"related_items"' in CosmicWiki/pages/**/*.md
```

**Complete guide**: `CosmicWiki/guides/CLAUDE_CODE_INTEGRATION.md`

---

## Creating New Wiki Pages

### Method 1: Interactive Mode (Easiest)

```bash
python3 CosmicWiki/scripts/generate_wiki_page.py --interactive
```

Follow the prompts to create a new page.

### Method 2: From JSON Data

1. Create a JSON file with your item data:

```json
{
  "id": "my_item_001",
  "acnh_data": {
    "name": "ACNH Item Name",
    "category": "fish",
    "sell_price": 500
  },
  "cosmic_data": {
    "name": "Cosmic Item Name",
    "category": "nebula_organism",
    "lore": "Lore description here...",
    "sell_price_credits": 500
  },
  "technical_implementation": {
    "primary_class": "PickableItem",
    "setup_steps": []
  },
  "wiki_metadata": {
    "created_date": "2026-02-16",
    "tags": ["tag1", "tag2"]
  }
}
```

2. Generate the page:

```bash
python3 CosmicWiki/scripts/generate_wiki_page.py --data my_item.json
```

3. Update the index:

```bash
python3 CosmicWiki/scripts/generate_index.py
```

### Method 3: Manual Creation

1. Copy the template:

```bash
cp CosmicWiki/templates/wiki_page_template.md CosmicWiki/pages/[category]/my_item.md
```

2. Fill in the placeholders ({{cosmic_name}}, etc.)

3. Update the index:

```bash
python3 CosmicWiki/scripts/generate_index.py
```

---

## Example Pages

### Fish Example
```bash
cat CosmicWiki/pages/nebula_organisms/plasma_eel.md
```

**What it shows:**
- Collectible item (PickableItem)
- Fishing spawn system
- Galactic Archive integration
- Visual effects setup

### Tool Example
```bash
cat CosmicWiki/pages/tools/plasma_seiner.md
```

**What it shows:**
- Complex tool system
- Custom CharacterAbility
- Minigame integration
- Durability system

### Material Example
```bash
cat CosmicWiki/pages/materials/ferrite_core.md
```

**What it shows:**
- Simple crafting material
- Loot table integration
- Mining mechanics
- Resource gathering

---

## Translation Guidelines

### Theme Translation

**ACNH** → **Cosmic Colony**

| Original | Translation | Keep | Change |
|----------|-------------|------|--------|
| Island | Planet/Colony | ✓ Cozy feel | ✗ Water → Plasma |
| Ocean | Plasma Flows | ✓ Relaxed pace | ✗ Natural → Sci-fi |
| Trees | Xeno-Flora | ✓ Collection loop | ✗ Animals → Aliens |
| Fish | Nebula Organisms | ✓ Daily routine | ✗ Wood → Bio-Fiber |

### Lore Translation

**Good:**
- "A sea bass! At least a C+!" → "A Plasma Eel! Energy signature: C-grade at best!"

**Bad:**
- "A sea bass! At least a C+!" → "An electromagnetic energy-based organism of the Plasmidae phylum!"

**Why:**
- ✓ Maintains humor and personality
- ✓ Converts theme while keeping tone
- ✗ Too technical/serious loses the feel

---

## Common Tasks

### Task: Add a New Fish

1. **Reference the mapping:**
```bash
cat CosmicWiki/data/acnh_cosmic_mapping.json
# See: collectible_categories → fish
```

2. **Look at existing example:**
```bash
cat CosmicWiki/pages/nebula_organisms/plasma_eel.md
```

3. **Create data JSON:**
```json
{
  "id": "nebula_organism_002",
  "acnh_data": {
    "name": "Your ACNH Fish Name",
    "category": "fish",
    "sell_price": 800,
    "rarity": "Uncommon"
  },
  "cosmic_data": {
    "name": "Your Cosmic Name",
    "category": "nebula_organism",
    "lore": "Lore about this energy organism...",
    "sell_price_credits": 800,
    "rarity": "Uncommon"
  }
}
```

4. **Generate:**
```bash
python3 CosmicWiki/scripts/generate_wiki_page.py --data your_fish.json
python3 CosmicWiki/scripts/generate_index.py
```

### Task: Add a New Tool

1. **Reference the mapping:**
```bash
cat CosmicWiki/data/acnh_cosmic_mapping.json
# See: tools section
```

2. **Look at existing example:**
```bash
cat CosmicWiki/pages/tools/plasma_seiner.md
```

3. **Key differences from collectibles:**
- Tools are `Weapon` class, not `PickableItem`
- Need `CharacterAbility` component
- Have durability systems
- More complex implementation

4. **Generate and customize:**
```bash
python3 CosmicWiki/scripts/generate_wiki_page.py --interactive
# Select "tool" as category
# Add detailed implementation steps
```

### Task: Update Existing Page

**Option 1: Direct edit**
```bash
# Edit the markdown file directly
vim CosmicWiki/pages/nebula_organisms/plasma_eel.md
```

**Option 2: Update JSON and regenerate**
```bash
# Update your JSON data
# Regenerate the page
python3 CosmicWiki/scripts/generate_wiki_page.py --data updated_plasma_eel.json
```

**Don't forget to update index:**
```bash
python3 CosmicWiki/scripts/generate_index.py
```

---

## Tips & Tricks

### Finding Content

```bash
# Search by keyword
grep -r "plasma" CosmicWiki/pages/

# Search by ACNH name
grep -r "ACNH Equivalent.*Sea Bass" CosmicWiki/pages/

# List all pages in category
ls CosmicWiki/pages/nebula_organisms/

# View the index
cat CosmicWiki/WIKI_INDEX.md
```

### Data Validation

```bash
# Check JSON syntax
python3 -m json.tool CosmicWiki/data/acnh_cosmic_mapping.json

# Validate against schema (if you have jsonschema installed)
jsonschema -i my_item.json CosmicWiki/data/schemas/item_schema.json
```

### Batch Operations

```bash
# Generate multiple pages from a directory of JSON files
for file in data/*.json; do
    python3 CosmicWiki/scripts/generate_wiki_page.py --data "$file"
done

# Update index once at the end
python3 CosmicWiki/scripts/generate_index.py
```

---

## Troubleshooting

### "Script won't run"

Make sure it's executable:
```bash
chmod +x CosmicWiki/scripts/*.py
```

Or run with python3:
```bash
python3 CosmicWiki/scripts/generate_wiki_page.py
```

### "Can't find the template"

Scripts expect to be run from the project root:
```bash
cd /home/user/Stranded-Game
python3 CosmicWiki/scripts/generate_wiki_page.py
```

### "Index is missing pages"

Regenerate the index:
```bash
python3 CosmicWiki/scripts/generate_index.py
```

### "JSON format error"

Validate your JSON:
```bash
python3 -m json.tool your_data.json
```

---

## Next Steps

1. **Explore the examples:**
   - Fish: `CosmicWiki/pages/nebula_organisms/plasma_eel.md`
   - Tool: `CosmicWiki/pages/tools/plasma_seiner.md`
   - Material: `CosmicWiki/pages/materials/ferrite_core.md`

2. **Read the guides:**
   - Main README: `CosmicWiki/README.md`
   - Agent Integration: `CosmicWiki/guides/CLAUDE_CODE_INTEGRATION.md`

3. **Start creating:**
   ```bash
   python3 CosmicWiki/scripts/generate_wiki_page.py --interactive
   ```

4. **Build the game!** 🚀

---

**Questions?** Check `CosmicWiki/README.md` or the full integration guide.

**For Claude Code agents:** See `CosmicWiki/guides/CLAUDE_CODE_INTEGRATION.md`

🌌 **Happy creating!**
