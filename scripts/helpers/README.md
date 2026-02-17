# Development Helper Scripts

This directory contains shell helper scripts for streamlining Cosmic Colony development.

## Available Helpers

### wiki_query.sh

Shell functions for querying the CosmicWiki system.

**Load:**
```bash
source scripts/helpers/wiki_query.sh
```

**Key Functions:**
- `wiki_find <name>` - Find wiki pages
- `wiki_find_rarity <rarity>` - Search by rarity
- `wiki_find_class <class>` - Search by class
- `wiki_summary <file>` - Quick item overview
- `wiki_extract_json <file>` - Extract JSON data
- `wiki_help` - Show all functions

### unity_patterns.sh

Reference templates for Unity MCP operations and TopDown Engine workflows.

**Load:**
```bash
source scripts/helpers/unity_patterns.sh
```

**Key Functions:**
- `unity_workflow_player_setup` - Complete player character setup
- `unity_workflow_interactive_object` - Interactive object creation
- `unity_workflow_zone_creation` - Showcase zone building
- `unity_component_reference` - TopDown Engine components
- `unity_patterns_help` - Show all patterns

## Quick Start

```bash
# Load both helpers
source scripts/helpers/wiki_query.sh
source scripts/helpers/unity_patterns.sh

# Query wiki
wiki_find plasma_eel

# Get workflow template
unity_workflow_player_setup
```

## Documentation

- **Complete Guide**: `docs/quick-reference/HELPERS_GUIDE.md`
- **Quick Reference**: `docs/quick-reference/development-shortcuts.md`
- **Project Memory**: `.claude/.../memory/MEMORY.md`

## Usage with Claude Code

These helpers work with Claude Code's Bash tool:

```
User: "Load wiki helpers and find plasma eel"
Claude: [Runs: source scripts/helpers/wiki_query.sh && wiki_find plasma_eel]
```

See `docs/quick-reference/HELPERS_GUIDE.md` for complete integration examples.

---

**Last Updated:** 2026-02-16
