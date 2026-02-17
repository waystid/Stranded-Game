# Development Helpers Guide

## Overview

This project uses **helper scripts and quick-reference patterns** to streamline development with CosmicWiki and Unity/TopDown Engine integration.

Unlike custom Claude Code skills (which require MCP server configuration), these helpers work immediately and provide:
- Shell functions for wiki queries
- Command templates for Unity operations
- Quick-reference workflows
- Reusable patterns

## Setup

### Load Helpers

In your terminal or through Claude Code's Bash tool:

```bash
# Load wiki query functions
source scripts/helpers/wiki_query.sh

# Load Unity pattern references
source scripts/helpers/unity_patterns.sh
```

You can also add these to your shell profile (~/.zshrc or ~/.bashrc) for automatic loading:

```bash
# Add to ~/.zshrc
alias load-cosmic='cd /path/to/TopDown\ Engine && source scripts/helpers/wiki_query.sh && source scripts/helpers/unity_patterns.sh'
```

## Wiki Helper Functions

### Available Functions

After loading `wiki_query.sh`:

| Function | Purpose | Example |
|----------|---------|---------|
| `wiki_find <name>` | Find wiki page by item name | `wiki_find plasma_eel` |
| `wiki_find_acnh <name>` | Find by ACNH equivalent | `wiki_find_acnh "Sea Bass"` |
| `wiki_find_rarity <rarity>` | Find all items of rarity | `wiki_find_rarity Rare` |
| `wiki_find_class <class>` | Find by primary class | `wiki_find_class PickableItem` |
| `wiki_extract_json <file>` | Extract JSON data block | `wiki_extract_json CosmicWiki/pages/...` |
| `wiki_summary <file>` | Quick overview | `wiki_summary CosmicWiki/pages/...` |
| `wiki_list_category <cat>` | List all in category | `wiki_list_category tools` |
| `wiki_search [opts]` | Multi-criteria search | `wiki_search --rarity Rare --class Tool` |
| `wiki_help` | Show help | `wiki_help` |

### Example Workflows

**Find and read an item:**
```bash
# Find the wiki page
wiki_find plasma_eel
# Output: CosmicWiki/pages/nebula_organisms/plasma_eel.md

# Get quick summary
wiki_summary CosmicWiki/pages/nebula_organisms/plasma_eel.md
# Shows: name, ACNH equivalent, rarity, class, price

# Extract full JSON data
wiki_extract_json CosmicWiki/pages/nebula_organisms/plasma_eel.md
# Returns: clean JSON block for parsing
```

**Search by criteria:**
```bash
# Find all rare pickable items
wiki_search --rarity Rare --class PickableItem

# Find items worth at least 1000 credits
wiki_find_price_min 1000

# Find items by ACNH name
wiki_find_acnh "Red Snapper"
```

**List category contents:**
```bash
# See all nebula organisms
wiki_list_category nebula_organisms

# See all tools
wiki_list_category tools
```

## Unity Pattern References

### Available Patterns

After loading `unity_patterns.sh`:

| Function | Purpose |
|----------|---------|
| `unity_find_assets <term>` | Asset search patterns |
| `unity_create_prefab_pattern <name>` | Prefab creation workflow |
| `unity_scene_pattern` | Scene organization structure |
| `unity_component_reference` | TopDown Engine components |
| `unity_asset_checklist` | Asset import checklist |
| `unity_workflow_player_setup` | Complete player setup |
| `unity_workflow_interactive_object` | Interactive object workflow |
| `unity_workflow_zone_creation` | Zone creation workflow |
| `unity_patterns_help` | Show help |

### Example Usage

**Get workflow template:**
```bash
# Show complete player setup workflow
unity_workflow_player_setup

# Show interactive object creation
unity_workflow_interactive_object

# Show zone creation pattern
unity_workflow_zone_creation
```

**Quick reference:**
```bash
# See TopDown Engine component patterns
unity_component_reference

# See scene organization structure
unity_scene_pattern

# Asset import checklist
unity_asset_checklist
```

These functions output **reference patterns** - templates showing the correct Unity MCP tool calls. You then use these patterns with Claude Code's actual Unity MCP tools.

## Integration with Claude Code

### Using Helpers in Claude Code

When working with Claude Code, you can:

1. **Load helpers at session start:**
   ```
   User: "Load the wiki and Unity helpers"
   Claude: [Runs source commands for both helpers]
   ```

2. **Query wiki before implementation:**
   ```
   User: "Find the plasma eel in the wiki and show me its specs"
   Claude: [Uses wiki_find + wiki_summary or direct tools]
   ```

3. **Get workflow templates:**
   ```
   User: "Show me the pattern for creating an interactive object"
   Claude: [Runs unity_workflow_interactive_object, shows template]
   ```

4. **Use patterns as reference while implementing:**
   ```
   User: "Create a new tree interactive object following the pattern"
   Claude: [References pattern, then uses actual Unity MCP tools]
   ```

### Direct Tool Usage

You can also use Claude Code tools directly without helpers:

**Wiki queries:**
```
Grep: pattern="plasma_eel" path="CosmicWiki/pages"
Read: file_path="CosmicWiki/pages/nebula_organisms/plasma_eel.md"
Grep: pattern="```json" path="CosmicWiki/pages/nebula_organisms/plasma_eel.md" -A 50
```

**Unity operations:**
```
Glob: pattern="Assets/**/Astronaut*.fbx"
Read: file_path="Assets/TopDownEngine/.../LoftSuspenders.prefab"
manage_gameobject(action="create", name="AstronautPlayer", ...)
manage_components(action="add", target="AstronautPlayer", ...)
```

The helpers just make these patterns faster to reference.

## Quick Reference Document

See `docs/quick-reference/development-shortcuts.md` for:
- All common patterns
- Quick lookup tables
- Workflow checklists
- Keyboard shortcuts
- Integration workflows

This is your **primary reference** during development.

## Complete Example Workflow

### Implementing a New Item from Wiki

**Goal:** Implement Plasma Eel from CosmicWiki as a pickable item in Unity

**Step 1: Research (Wiki)**
```bash
# Load helpers
source scripts/helpers/wiki_query.sh

# Find and read the item
wiki_find plasma_eel
# → CosmicWiki/pages/nebula_organisms/plasma_eel.md

# Get summary
wiki_summary CosmicWiki/pages/nebula_organisms/plasma_eel.md
# Output:
# Name: Plasma Eel
# ACNH: Sea Bass
# Rarity: Common
# Class: PickableItem
# Price: 400 credits

# Extract full JSON
wiki_extract_json CosmicWiki/pages/nebula_organisms/plasma_eel.md > plasma_eel_data.json
```

**Step 2: Find Assets (Unity)**
```bash
# Find suitable eel/fish model
Glob: pattern="Assets/**/*Eel*.fbx"
Glob: pattern="Assets/**/*Fish*.prefab"

# Read reference prefab
Read: file_path="Assets/Prefabs/EnergyCrystalPicker.prefab"
```

**Step 3: Get Workflow Pattern**
```bash
source scripts/helpers/unity_patterns.sh
unity_component_reference  # See pickable item pattern
```

**Step 4: Implement in Unity**

Following the 3-component pickable item pattern:

```
1. Create parent (ItemPicker):
   manage_gameobject(action="create", name="PlasmaEelPicker")
   manage_components(action="add", target="PlasmaEelPicker", component_type="ItemPicker")
   manage_components(action="add", target="PlasmaEelPicker", component_type="SphereCollider")
   manage_components(action="set_property", target="PlasmaEelPicker",
                    component_type="SphereCollider",
                    properties={isTrigger: true, radius: 1.5})

2. Create child (Item):
   manage_gameobject(action="create", name="PlasmaEelItem", parent="PlasmaEelPicker")
   manage_components(action="add", target="PlasmaEelItem", component_type="InventoryItem")
   manage_components(action="set_property", target="PlasmaEelItem",
                    component_type="InventoryItem",
                    properties={
                      ItemName: "Plasma Eel",
                      ItemPrice: 400,
                      Rarity: "Common"
                    })

3. Create grandchild (Visual):
   manage_gameobject(action="create", name="EelModel",
                    parent="PlasmaEelItem",
                    prefab_path="Assets/.../EelModel.fbx",
                    position=[0, 0, 0])
```

**Step 5: Test**
```
manage_editor(action="play")
read_console(types=["error"])
```

**Step 6: Save**
```
manage_prefabs(action="create_from_gameobject",
              target="PlasmaEelPicker",
              prefab_path="Assets/Prefabs/PlasmaEelPicker.prefab")
```

**Step 7: Validate**
- Check price: 400 credits ✓
- Check rarity: Common ✓
- Check class: PickableItem ✓
- Test pickup in scene ✓

## Tips

### For Wiki Operations
- Always read the full wiki page, not just JSON
- Check `related_items` for dependencies
- Follow specs exactly (especially prices and rarities)
- Use `wiki_summary` for quick checks during implementation

### For Unity Operations
- Load patterns reference at session start
- Read reference prefabs before creating similar objects
- Test in TestMap scene before production
- Check console after every significant change
- Save prefabs only after successful testing

### Performance
- Use helpers for quick queries (1-5 results)
- Use direct tools for bulk operations with pagination
- Wiki functions use grep/find - fast for small searches
- For comprehensive codebase exploration, use Task tool with Explore agent

## Troubleshooting

**Helpers not working:**
- Check you ran `source` command
- Verify scripts exist: `ls scripts/helpers/`
- Check permissions: `chmod +x scripts/helpers/*.sh`

**Wiki queries returning nothing:**
- Verify CosmicWiki/ directory exists
- Check search term spelling
- Try broader search: `wiki_list_category <category>`

**Unity patterns not applicable:**
- Patterns are templates, not exact commands
- Adapt to your specific use case
- Check reference prefabs for current structure
- Unity MCP tools have latest API - patterns may lag

## Extending the Helpers

### Add New Wiki Function

Edit `scripts/helpers/wiki_query.sh`:

```bash
wiki_find_by_spawn_rate() {
    local min_rate="$1"
    grep -r "\"spawn_rate\":" "$WIKI_ROOT/pages" 2>/dev/null | \
        awk -F: -v min="$min_rate" '$NF+0 >= min {print $1}' | sort -u
}
```

### Add New Unity Pattern

Edit `scripts/helpers/unity_patterns.sh`:

```bash
unity_workflow_custom() {
    cat <<EOF
Your custom workflow here...
EOF
}
```

### Update Quick Reference

Edit `docs/quick-reference/development-shortcuts.md` to add new patterns.

## Related Documentation

- **CosmicWiki Integration Guide**: `CosmicWiki/guides/CLAUDE_CODE_INTEGRATION.md`
- **CosmicWiki README**: `CosmicWiki/README.md`
- **Development Shortcuts**: `docs/quick-reference/development-shortcuts.md`
- **TopDown Engine Docs**: `Assets/TopDownEngine/Documentation/`
- **Project Memory**: `.claude/.../memory/MEMORY.md`

---

**Quick Start:**
```bash
# Load helpers
source scripts/helpers/wiki_query.sh
source scripts/helpers/unity_patterns.sh

# Test wiki query
wiki_find plasma_eel

# Get Unity workflow template
unity_workflow_player_setup

# See all available functions
wiki_help
unity_patterns_help
```

**Last Updated:** 2026-02-16
