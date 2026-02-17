# Cosmic Colony Wiki

> **🌌 A 1:1 recreation of Nookipedia for Cosmic Colony**
>
> Translates Animal Crossing: New Horizons data into the Cosmic/Planet theme with LLM-friendly structure for Claude Code agents.

---

## Overview

The **Cosmic Colony Wiki** is a comprehensive knowledge base for the game that:

1. **Maps ACNH to Cosmic Colony** - 1:1 translation of all items, mechanics, and systems
2. **LLM-Friendly Format** - Structured JSON + Markdown for easy AI agent parsing
3. **Implementation-Ready** - Complete Unity/TopDown Engine setup guides
4. **Interactive Translation** - Tools for adapting ACNH lore to cosmic theme

---

## Quick Start

### For Players/Designers

Browse the wiki to understand game content:

```bash
# View the master index
cat CosmicWiki/WIKI_INDEX.md

# Browse a category
ls CosmicWiki/pages/nebula_organisms/

# Read an item page
cat CosmicWiki/pages/nebula_organisms/plasma_eel.md
```

### For Developers

Implement features using the wiki as reference:

```bash
# Find the item you need to implement
grep -r "Plasma Eel" CosmicWiki/pages/

# Read the implementation guide
cat CosmicWiki/pages/nebula_organisms/plasma_eel.md

# Extract JSON data for validation
grep -A 100 '```json' CosmicWiki/pages/nebula_organisms/plasma_eel.md
```

### For Claude Code Agents

See the complete integration guide:

```bash
cat CosmicWiki/guides/CLAUDE_CODE_INTEGRATION.md
```

---

## Structure

```
CosmicWiki/
│
├── README.md                          # This file
├── WIKI_INDEX.md                      # Master index (auto-generated)
│
├── data/
│   ├── acnh_cosmic_mapping.json       # Core translation mapping
│   └── schemas/
│       └── item_schema.json           # JSON schema for items
│
├── pages/                             # All wiki pages
│   ├── nebula_organisms/              # Fish → Nebula Organisms
│   │   ├── plasma_eel.md
│   │   └── ...
│   ├── micro_drones/                  # Bugs → Micro-Drones
│   ├── ancient_artifacts/             # Fossils → Ancient Artifacts
│   ├── tools/                         # Tools
│   │   ├── plasma_seiner.md
│   │   └── ...
│   ├── materials/                     # Crafting materials
│   │   ├── ferrite_core.md
│   │   └── ...
│   ├── furniture/                     # Furniture items
│   ├── buildings/                     # Structures
│   └── npcs/                          # Characters
│
├── topdown_engine/                    # TopDown Engine integration
│   ├── README.md                      # Integration overview
│   ├── workflows/                     # Implementation workflows
│   │   ├── replace_player_model.md
│   │   ├── create_pickable_item.md
│   │   └── create_interactive_object.md
│   ├── components/                    # Component references
│   │   ├── character_abilities.md
│   │   └── item_components.md
│   └── patterns/                      # Implementation patterns
│       └── 3_component_item.md
│
├── devlog/                            # Development log (Agentic tracking)
│   ├── README.md                      # DevLog system overview
│   ├── DEVLOG_INDEX.md                # Master index of all entries
│   ├── devlog-agent.md                # DevLog agent documentation
│   ├── entries/                       # Individual devlog entries
│   │   └── 2026-02-16-player-model-replacement.md
│   ├── templates/                     # Entry templates
│   │   └── devlog-entry-template.md
│   └── scripts/
│       └── generate_devlog_index.py   # Index generator
│
├── templates/
│   └── wiki_page_template.md          # Template for new pages
│
├── scripts/
│   ├── generate_wiki_page.py          # Page generator
│   └── generate_index.py              # Index generator
│
└── guides/
    └── CLAUDE_CODE_INTEGRATION.md     # Agent integration guide
```

---

## Core Features

### 1. ACNH to Cosmic Mapping

Every ACNH element has a cosmic equivalent:

| ACNH | Cosmic Colony |
|------|---------------|
| Island | Crash Site Planet |
| Fish | Nebula Organisms |
| Bugs | Micro-Drones |
| Fossils | Ancient Artifacts |
| Bells | Credits |
| Nook Miles | Pioneer Points |
| Tom Nook | Z.O.E. (AI) |

See `data/acnh_cosmic_mapping.json` for the complete mapping.

### 2. LLM-Friendly Data Format

Each wiki page contains:

```markdown
# Item Name

## Quick Reference Table
[Human-readable comparison]

## Cosmic Lore
[Narrative description]

## Data Fields (LLM-Friendly)
```json
{
  "id": "item_id",
  "acnh_data": { ... },
  "cosmic_data": { ... },
  "technical_implementation": { ... }
}
```

## Technical Implementation
[Step-by-step Unity/TopDown Engine guide]
```

### 3. Interactive Translation Tools

**Translate Lore:**
- Convert ACNH descriptions to cosmic theme
- Maintain tone and gameplay feel

**Define Data Fields:**
- Structured JSON for all properties
- Validated against schema

**Technical Implementation:**
- Unity component setup
- TopDown Engine configuration
- Code examples

### 4. Integration with Claude Code

The wiki is designed for AI agents:

- **Searchable**: Consistent structure for queries
- **Parseable**: Clean JSON blocks
- **Actionable**: Complete implementation guides
- **Traceable**: Related items linked

---

## Usage Examples

### Example 1: Implementing a New Item

**Goal:** Implement the Plasma Eel (Sea Bass equivalent)

```bash
# 1. Read the wiki page
Read CosmicWiki/pages/nebula_organisms/plasma_eel.md

# 2. Extract JSON data
Grep for '```json' in CosmicWiki/pages/nebula_organisms/plasma_eel.md -A 100

# 3. Follow implementation steps
# (See "Technical Implementation" section)

# 4. Validate against data
# Check sell price, rarity, spawn conditions match JSON
```

### Example 2: Finding All Items in a Category

```bash
# List all Nebula Organisms (fish)
ls CosmicWiki/pages/nebula_organisms/

# List all Tools
ls CosmicWiki/pages/tools/

# Find all "Rare" items
Grep for '"rarity": "Rare"' in CosmicWiki/pages/**/*.md
```

### Example 3: Creating a New Wiki Page

```bash
# Interactive mode
python CosmicWiki/scripts/generate_wiki_page.py --interactive

# From JSON data
python CosmicWiki/scripts/generate_wiki_page.py --data my_item.json

# Update the index
python CosmicWiki/scripts/generate_index.py
```

---

## Key Concepts

### ACNH to Cosmic Translation Guide

**Theme Transformation:**
- **Cozy Island** → **Cosmic Planet**
- **Natural** → **Sci-Fi/Energy-based**
- **Water** → **Plasma/Energy Flows**
- **Trees** → **Xeno-Flora**
- **Animals** → **Aliens/Energy Beings**

**Tone Preservation:**
- Keep the **cozy, relaxed** feel
- Maintain **humor and personality**
- Preserve **progression pacing**
- Retain **collection/completion** mechanics

**Gameplay 1:1:**
- Same tools, same functions (but reskinned)
- Same economy (Bells → Credits)
- Same daily loop and events
- Same multiplayer structure

### TopDown Engine Integration

**NEW**: Complete TopDown Engine workflows and component references in `topdown_engine/`

All items are implemented using TopDown Engine classes:

| Item Type | TopDown Class |
|-----------|---------------|
| Collectibles | `PickableItem` |
| Tools | `Weapon` (with custom abilities) |
| Materials | `InventoryItem` |
| Furniture | `Interactable` |
| NPCs | `Character` with AI |

**Quick Access**:
```bash
# Load TopDown Engine helpers
source scripts/helpers/topdown_engine.sh

# View available workflows
topdown_list_workflows

# Get player model swap workflow
topdown_workflow replace_player_model

# Get API documentation URL
topdown_api Character
```

See `topdown_engine/README.md` for complete integration guide.

### Development Log (DevLog)

**NEW**: Agentic development tracking system in `devlog/`

Track build progress, technical decisions, and implementation patterns across sessions:

**Features:**
- **Session Continuity** - Resume work with full context from previous sessions
- **Technical Documentation** - Complete implementation details and configurations
- **Lessons Learned** - Capture what worked, what didn't, and why
- **Agentic Format** - Structured for AI agent reading and writing
- **DevLog Agent** - Specialized agent for managing development logs

**Quick Access**:
```bash
# Read latest session
cat CosmicWiki/devlog/entries/$(ls -t CosmicWiki/devlog/entries/ | head -1)

# View master index
cat CosmicWiki/devlog/DEVLOG_INDEX.md

# See all entries
ls CosmicWiki/devlog/entries/
```

**For Claude Code Agents**:
```
"Load devlog agent and summarize last session"
"Write devlog entry for today's camera controller work"
"Find all entries about animation systems"
```

See `devlog/README.md` for complete DevLog system guide.
See `devlog/devlog-agent.md` for DevLog Agent documentation.

---

## For Claude Code Agents

### Quick Reference

**Read a wiki page:**
```bash
Read CosmicWiki/pages/[category]/[item_id].md
```

**Find an item by ACNH name:**
```bash
Grep for "ACNH Equivalent.*[name]" in CosmicWiki/pages/**/*.md
```

**Extract JSON data:**
```bash
Grep for '```json' in CosmicWiki/pages/[category]/[item_id].md -A 100
```

**Find items by property:**
```bash
# By rarity
Grep for '"rarity": "Rare"' in CosmicWiki/pages/**/*.md

# By price
Grep for '"sell_price_credits": 1000' in CosmicWiki/pages/**/*.md

# By class
Grep for '"primary_class": "PickableItem"' in CosmicWiki/pages/**/*.md
```

### Implementation Workflow

1. **Research Phase**: Read wiki page(s)
2. **Planning Phase**: Extract JSON data and requirements
3. **Implementation Phase**: Follow technical guide
4. **Validation Phase**: Check against JSON data

See `guides/CLAUDE_CODE_INTEGRATION.md` for complete details.

---

## Contributing

### Adding New Pages

1. Create JSON data following `data/schemas/item_schema.json`
2. Run generator: `python scripts/generate_wiki_page.py --data your_data.json`
3. Edit generated markdown to add details
4. Update index: `python scripts/generate_index.py`

### Updating Existing Pages

1. Edit the markdown directly, or
2. Update JSON and regenerate

**Important:** Keep JSON and Markdown in sync!

### Translation Guidelines

When translating ACNH content:

1. **Preserve gameplay mechanics** - Don't change how it works
2. **Adapt theme consistently** - Use the mapping guide
3. **Maintain tone** - Keep it cozy and fun
4. **Add sci-fi flavor** - Bioluminescent, energy-based, cosmic
5. **Use technical terms** - But keep it accessible

**Good Example:**
- ACNH: "A simple fishing rod. Cast your line!"
- Cosmic: "A basic electromagnetic field projector. Calibrate your resonance!"

**Bad Example:**
- ❌ "An advanced quantum entanglement fishing apparatus with multi-phase resonance modulation"
  (Too complex, loses the accessible tone)

---

## FAQ

### Q: Why a wiki instead of a database?

**A:** The wiki format is:
- **Human-readable** (designers can browse)
- **LLM-friendly** (Claude Code can parse)
- **Version-controllable** (Git integration)
- **Flexible** (Easy to extend)

### Q: How is this different from Nookipedia?

**A:** This wiki:
- Translates to cosmic theme
- Includes Unity implementation guides
- Has structured JSON data
- Is designed for game development (not just reference)

### Q: Can I use this for other game engines?

**A:** Yes! The JSON data and lore are engine-agnostic. Just ignore the TopDown Engine-specific sections.

### Q: How complete is the wiki?

**A:** Currently includes:
- ✅ Core mapping (ACNH → Cosmic)
- ✅ Sample pages (fish, tool, material)
- ✅ Templates and generators
- ⏳ Full item catalog (in progress)

### Q: How do I request a new page?

Create an issue or use the interactive generator:
```bash
python CosmicWiki/scripts/generate_wiki_page.py --interactive
```

---

## Roadmap

### Phase 1: Foundation (Current)
- [x] Core mapping system
- [x] Wiki structure
- [x] Sample pages (fish, tool, material)
- [x] Generator scripts
- [x] Claude Code integration guide

### Phase 2: Content Expansion
- [ ] All fish → Nebula Organisms
- [ ] All bugs → Micro-Drones
- [ ] All fossils → Ancient Artifacts
- [ ] All tools
- [ ] All materials
- [ ] All NPCs

### Phase 3: Advanced Features
- [ ] Interactive web version
- [ ] Search functionality
- [ ] Visual asset library
- [ ] Crafting recipe database
- [ ] Event calendar

### Phase 4: Polish
- [ ] Complete lore for all items
- [ ] Comprehensive code examples
- [ ] Video tutorials
- [ ] Community contributions

---

## Resources

### Internal
- **Master Index**: `CosmicWiki/WIKI_INDEX.md`
- **Mapping Data**: `CosmicWiki/data/acnh_cosmic_mapping.json`
- **Item Schema**: `CosmicWiki/data/schemas/item_schema.json`
- **Agent Guide**: `CosmicWiki/guides/CLAUDE_CODE_INTEGRATION.md`

### External
- **Nookipedia**: https://nookipedia.com/wiki/Animal_Crossing:_New_Horizons
- **TopDown Engine**: https://topdown-engine-docs.moremountains.com/
- **Unity Docs**: https://docs.unity3d.com/

---

## Credits

**Game Concept:** Cosmic Colony (Animal Crossing: New Horizons reimagined)
**Wiki System:** Custom-built for LLM integration
**Data Source:** Animal Crossing: New Horizons (Nintendo)
**Engine:** Unity + TopDown Engine (More Mountains)

---

## License

This wiki is for development purposes for the Cosmic Colony game project.

Animal Crossing: New Horizons is © Nintendo. This project is not affiliated with or endorsed by Nintendo.

---

**Need help?** Check the guides or create an issue.

**For Claude Code agents**: Start with `guides/CLAUDE_CODE_INTEGRATION.md`

🌌 **Happy universe-building!** 🚀
