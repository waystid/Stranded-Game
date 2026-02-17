#!/bin/bash
# TopDown Engine Helper Functions
# Usage: source scripts/helpers/topdown_engine.sh

TDE_ROOT="CosmicWiki/topdown_engine"
TDE_API_BASE="https://topdown-engine-docs.moremountains.com/API"

# Get workflow by name
topdown_workflow() {
    local workflow_name="$1"

    if [ -z "$workflow_name" ]; then
        echo "Available workflows:"
        ls "$TDE_ROOT/workflows/" 2>/dev/null | sed 's/.md$//'
        return 0
    fi

    local workflow_file="$TDE_ROOT/workflows/${workflow_name}.md"
    if [ -f "$workflow_file" ]; then
        cat "$workflow_file"
    else
        echo "Workflow not found: $workflow_name"
        echo "Available workflows:"
        ls "$TDE_ROOT/workflows/" 2>/dev/null | sed 's/.md$//'
    fi
}

# Quick reference for component
topdown_component() {
    local component_name="$1"

    if [ -z "$component_name" ]; then
        echo "Available component references:"
        ls "$TDE_ROOT/components/" 2>/dev/null | sed 's/.md$//'
        return 0
    fi

    local component_file="$TDE_ROOT/components/${component_name}.md"
    if [ -f "$component_file" ]; then
        cat "$component_file"
    else
        echo "Component reference not found: $component_name"
        echo "Try: character, movement, abilities, ai, items"
    fi
}

# Get pattern reference
topdown_pattern() {
    local pattern_name="$1"

    if [ -z "$pattern_name" ]; then
        echo "Available patterns:"
        ls "$TDE_ROOT/patterns/" 2>/dev/null | sed 's/.md$//'
        return 0
    fi

    local pattern_file="$TDE_ROOT/patterns/${pattern_name}.md"
    if [ -f "$pattern_file" ]; then
        cat "$pattern_file"
    else
        echo "Pattern not found: $pattern_name"
        echo "Available patterns:"
        ls "$TDE_ROOT/patterns/" 2>/dev/null | sed 's/.md$//'
    fi
}

# Construct API URL for component
topdown_api_url() {
    local component_name="$1"

    # Convert component name to API format
    # Example: "Character" -> "class_more_mountains_1_1_top_down_engine_1_1_character.html"
    local formatted_name=$(echo "$component_name" | tr '[:upper:]' '[:lower:]')
    echo "${TDE_API_BASE}/class_more_mountains_1_1_top_down_engine_1_1${formatted_name}.html"
}

# Show API URL for component
topdown_api() {
    local component_name="$1"

    if [ -z "$component_name" ]; then
        echo "Usage: topdown_api <ComponentName>"
        echo "Example: topdown_api Character"
        echo ""
        echo "Common components:"
        echo "  Character, CharacterMovement, CharacterAbility"
        echo "  LevelManager, GameManager, Health"
        echo "  ItemPicker, InventoryItem, Loot"
        return 0
    fi

    local url=$(topdown_api_url "$component_name")
    echo "API Documentation: $url"
    echo ""
    echo "To fetch documentation:"
    echo "  WebFetch: url=\"$url\" prompt=\"Extract component overview and public API\""
}

# List all workflows
topdown_list_workflows() {
    echo "TopDown Engine Workflows:"
    echo ""
    if [ -d "$TDE_ROOT/workflows" ]; then
        for workflow in "$TDE_ROOT/workflows"/*.md; do
            if [ -f "$workflow" ]; then
                local name=$(basename "$workflow" .md)
                local desc=$(grep "^##" "$workflow" | head -1 | sed 's/^## //')
                printf "  %-30s %s\n" "$name" "$desc"
            fi
        done
    else
        echo "  No workflows found. Directory: $TDE_ROOT/workflows"
    fi
}

# List all component references
topdown_list_components() {
    echo "TopDown Engine Component References:"
    echo ""
    if [ -d "$TDE_ROOT/components" ]; then
        for component in "$TDE_ROOT/components"/*.md; do
            if [ -f "$component" ]; then
                local name=$(basename "$component" .md)
                printf "  - %s\n" "$name"
            fi
        done
    else
        echo "  No component references found. Directory: $TDE_ROOT/components"
    fi
}

# List all patterns
topdown_list_patterns() {
    echo "TopDown Engine Patterns:"
    echo ""
    if [ -d "$TDE_ROOT/patterns" ]; then
        for pattern in "$TDE_ROOT/patterns"/*.md; do
            if [ -f "$pattern" ]; then
                local name=$(basename "$pattern" .md)
                printf "  - %s\n" "$name"
            fi
        done
    else
        echo "  No patterns found. Directory: $TDE_ROOT/patterns"
    fi
}

# Verify character setup in scene
topdown_verify_character() {
    cat <<'EOF'
TopDown Engine Character Verification Checklist:

1. Find player in scene:
   find_gameobjects(search_method="by_tag", search_term="Player")

2. Check core components present:
   Read: GameObject with Player tag
   Verify: Character, CharacterController, TopDownController3D, CharacterMovement

3. Check abilities initialized:
   Read component: CharacterMovement
   Check: AbilityInitialized = true

4. Verify animator setup:
   Find Model child GameObject
   Read component: Animator
   Check: runtimeAnimatorController is set

5. Check console for errors:
   read_console(types=["error"])

6. Test in play mode:
   manage_editor(action="play")
   - Test movement (WASD/arrows)
   - Check animations play
   - Verify camera control works
   manage_editor(action="stop")
EOF
}

# Quick player model swap workflow
topdown_quick_model_swap() {
    cat <<'EOF'
Quick Player Model Swap (assumes original works):

1. Duplicate working prefab:
   manage_asset(action="duplicate",
                path="<original_prefab_path>",
                destination="Assets/Prefabs/NewPlayer.prefab")

2. Instantiate in scene:
   manage_gameobject(action="create", name="NewPlayer",
                     prefab_path="Assets/Prefabs/NewPlayer.prefab")

3. Find Model child and delete mesh children:
   find_gameobjects(search_method="by_name", search_term="Dude")
   manage_gameobject(action="delete", target="Dude")
   # Repeat for all mesh parts (Pants, Shirt, etc.)

4. Add new model as child of Model:
   manage_gameobject(action="create", name="NewMesh",
                     parent="Model",
                     prefab_path="<new_model_path>")

5. Verify animator:
   Read: Model Animator component
   Check: controller and avatar still assigned

6. Save back to prefab:
   manage_prefabs(action="create_from_gameobject",
                  target="NewPlayer",
                  prefab_path="Assets/Prefabs/NewPlayer.prefab",
                  allow_overwrite=true)

7. Update LevelManager:
   manage_components(action="set_property",
                    target="LevelManager",
                    component_type="LevelManager",
                    property="PlayerPrefabs",
                    value=["Assets/Prefabs/NewPlayer.prefab"])

8. Test in play mode
EOF
}

# Show help
topdown_help() {
    cat <<'EOF'
TopDown Engine Helper Functions

Workflow Management:
  topdown_workflow [name]           - Show workflow (or list all)
  topdown_list_workflows            - List all available workflows
  topdown_quick_model_swap          - Quick reference for model swap

Component References:
  topdown_component [name]          - Show component reference
  topdown_list_components           - List all component references
  topdown_api <ComponentName>       - Get API documentation URL

Patterns:
  topdown_pattern [name]            - Show implementation pattern
  topdown_list_patterns             - List all available patterns

Verification:
  topdown_verify_character          - Show character verification checklist

Examples:
  topdown_workflow replace_player_model    # Full workflow
  topdown_component character_abilities    # Component reference
  topdown_api Character                    # Get API URL
  topdown_quick_model_swap                 # Quick steps

Documentation:
  Local: CosmicWiki/topdown_engine/
  Online: https://topdown-engine-docs.moremountains.com/API/
EOF
}

# Main help message
echo "TopDown Engine helpers loaded. Type 'topdown_help' for usage."
