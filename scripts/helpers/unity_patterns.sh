#!/bin/bash
# Unity Development Patterns - Common MCP Command Templates
# Usage: source scripts/helpers/unity_patterns.sh

# Note: These are reference patterns - actual Unity MCP calls must be made through Claude Code tools

# Find assets by pattern
unity_find_assets() {
    cat <<EOF
Unity Asset Search Pattern:
  Use: Glob tool with pattern "Assets/**/*${1}*"

  Example searches:
    - Find Astronaut models: Assets/**/Astronaut*.fbx
    - Find all prefabs: Assets/**/*.prefab
    - Find Pandazole trees: Assets/Pandazole*/**/*Tree*.prefab
EOF
}

# Create prefab pattern
unity_create_prefab_pattern() {
    cat <<EOF
Unity Prefab Creation Pattern:
  1. Create base GameObject:
     manage_gameobject(action="create", name="${1}", position=[0,0,0])

  2. Add components:
     manage_components(action="add", target="${1}", component_type="${2}")

  3. Configure component properties:
     manage_components(action="set_property", target="${1}",
                      component_type="${2}", property="${3}", value="${4}")

  4. Save as prefab:
     manage_gameobject(action="create", name="${1}",
                      save_as_prefab=true, prefab_folder="Assets/Prefabs")

Reference prefabs:
  - Player: Assets/TopDownEngine/Demos/Loft3D/Prefabs/PlayableCharacters/LoftSuspenders.prefab
  - Interactive item: Assets/Prefabs/EnergyCrystalPicker.prefab
  - Tree with interaction: Assets/Prefabs/AlienTree.prefab
EOF
}

# Scene hierarchy pattern
unity_scene_pattern() {
    cat <<EOF
Unity Scene Organization Pattern:

Recommended hierarchy:
  Scene Root
    ├── Environment
    │   ├── Lighting (Directional Light)
    │   ├── Terrain
    │   ├── Skybox
    │   └── Props
    ├── Gameplay
    │   ├── Player
    │   ├── NPCs
    │   └── InteractiveObjects
    ├── UI
    │   └── Canvas
    └── Managers
        ├── GameManager
        └── LevelManager

Commands to create zones:
  1. Create parent: manage_gameobject(action="create", name="ZoneName")
  2. Create children with parent: manage_gameobject(action="create", name="Child", parent="ZoneName")
  3. Position objects: manage_gameobject(action="modify", target="Child", position=[x,y,z])
EOF
}

# Component reference
unity_component_reference() {
    cat <<EOF
TopDown Engine Component Quick Reference:

Player Setup (see LoftSuspenders.prefab):
  - Character (main controller)
  - CharacterMovement
  - CharacterAbility implementations
  - Health
  - Animator
  - CapsuleCollider + Rigidbody

Interactive Objects:
  - ButtonActivated (for interactions)
  - Loot (for items that drop loot)
  - Animator (for visual feedback)
  - Colliders: Trigger for detection, non-trigger for physics

Pickable Items (3-component pattern):
  1. ItemPicker (parent with trigger collider) - Player detects this
  2. Item (child with InventoryItem) - Actual item data
  3. Visual (grandchild with model/sprite) - What player sees

Zone Pattern:
  - DialogueZone for text interactions
  - ButtonActivated for triggerable objects
  - AutoRespawn for respawning items
  - ConditionalActivation for prerequisites
EOF
}

# Asset import checklist
unity_asset_checklist() {
    cat <<EOF
Unity Asset Import Checklist:

3D Models (.fbx, .obj):
  1. Check scale (usually 1)
  2. Set rig type (Humanoid for characters, Generic for props)
  3. Configure materials
  4. Extract textures if embedded
  5. Generate colliders if needed

Animations:
  1. Set animation type (Humanoid/Generic)
  2. Loop settings
  3. Root motion (disable for TopDown Engine)
  4. Events for gameplay triggers

Prefabs from asset packs:
  1. Check component compatibility with TopDown Engine
  2. Add required colliders
  3. Set layer (Player, Ground, Interactable, etc.)
  4. Configure tags as needed
  5. Test in TestMap scene first

Commands:
  - Read asset info: manage_asset(action="get_info", path="Assets/...")
  - Search assets: manage_asset(action="search", path="Assets", filter_type="Prefab")
  - Import settings: Check .meta files with Read tool
EOF
}

# Common workflows
unity_workflow_player_setup() {
    cat <<EOF
Workflow: Set Up New Player Character

1. Locate character model:
   Glob: Assets/**/Astronaut*.fbx

2. Read reference prefab:
   Read: Assets/TopDownEngine/Demos/Loft3D/Prefabs/PlayableCharacters/LoftSuspenders.prefab

3. Create GameObject in scene:
   manage_gameobject(action="create", name="AstronautPlayer",
                    prefab_path=<path_from_step_1>)

4. Copy components from reference:
   Read reference, then add each component:
   manage_components(action="add", target="AstronautPlayer", component_type=<component>)

5. Configure movement:
   manage_components(action="set_property", target="AstronautPlayer",
                    component_type="CharacterMovement",
                    property="MovementSpeed", value=6.0)

6. Set up animator:
   manage_animation(action="animator_get_info", target="AstronautPlayer")
   Configure states based on available animations

7. Configure collider:
   manage_components(action="set_property", target="AstronautPlayer",
                    component_type="CapsuleCollider",
                    properties={center: [0, 1, 0], radius: 0.5, height: 2})

8. Test in scene:
   manage_editor(action="play")
   Check console: read_console()

9. Save as prefab:
   manage_prefabs(action="create_from_gameobject",
                 target="AstronautPlayer",
                 prefab_path="Assets/Prefabs/AstronautPlayer.prefab")
EOF
}

unity_workflow_interactive_object() {
    cat <<EOF
Workflow: Create Interactive Object (e.g., Shaking Tree)

1. Find asset model:
   Glob: Assets/Pandazole*/**/*Tree*.prefab

2. Create in scene:
   manage_gameobject(action="create", name="PandazoleTreeInteractive",
                    prefab_path=<tree_path>, position=[5, 0, 5])

3. Add interaction zone:
   manage_components(action="add", target="PandazoleTreeInteractive",
                    component_type="ButtonActivated")

4. Configure interaction:
   manage_components(action="set_property",
                    target="PandazoleTreeInteractive",
                    component_type="ButtonActivated",
                    properties={
                      RequiresPlayerCollision: true,
                      ButtonPromptText: "Shake Tree"
                    })

5. Add trigger collider:
   manage_components(action="add", target="PandazoleTreeInteractive",
                    component_type="SphereCollider")
   manage_components(action="set_property",
                    target="PandazoleTreeInteractive",
                    component_type="SphereCollider",
                    properties={isTrigger: true, radius: 2.0})

6. Add loot drops:
   manage_components(action="add", target="PandazoleTreeInteractive",
                    component_type="Loot")
   Configure loot table

7. Add animation feedback:
   manage_animation(action="animator_set_trigger",
                   target="PandazoleTreeInteractive",
                   trigger="Shake")

8. Test interaction:
   manage_editor(action="play")
EOF
}

unity_workflow_zone_creation() {
    cat <<EOF
Workflow: Create Showcase Zone

1. Create zone parent:
   manage_gameobject(action="create", name="MovementShowcaseZone",
                    position=[0, 0, 0])

2. Add terrain/ground:
   manage_gameobject(action="create", name="Ground",
                    parent="MovementShowcaseZone",
                    primitive_type="Plane",
                    scale=[10, 1, 10],
                    position=[0, 0, 0])

3. Apply material/texture:
   manage_material(action="assign_material_to_renderer",
                  target="Ground",
                  material_path="Assets/Materials/GrassTexture.mat")

4. Place decorative objects:
   For each decoration:
     manage_gameobject(action="create", name="Decoration_1",
                      parent="MovementShowcaseZone",
                      prefab_path=<decoration_path>,
                      position=[x, 0, z])

5. Add gameplay objects:
   Create interactive objects following interactive_object workflow

6. Configure lighting:
   manage_gameobject(action="create", name="ZoneLight",
                    parent="MovementShowcaseZone",
                    component_type="Light")
   manage_components(action="set_property", target="ZoneLight",
                    component_type="Light",
                    properties={type: "Directional", intensity: 1.0})

7. Add zone boundaries (optional):
   Create invisible walls with colliders

8. Test zone:
   manage_scene(action="save")
   manage_editor(action="play")
EOF
}

# Help function
unity_patterns_help() {
    cat <<EOF
Unity Development Patterns Reference

Available pattern guides:
  unity_find_assets <search_term>      - Asset search patterns
  unity_create_prefab_pattern <name>   - Prefab creation workflow
  unity_scene_pattern                  - Scene organization structure
  unity_component_reference            - TopDown Engine components
  unity_asset_checklist                - Asset import checklist
  unity_workflow_player_setup          - Complete player character setup
  unity_workflow_interactive_object    - Interactive object creation
  unity_workflow_zone_creation         - Showcase zone creation
  unity_patterns_help                  - Show this help

These are reference patterns. Actual Unity operations require
using Claude Code's Unity MCP tools (manage_*, find_*, etc.)

Quick Tips:
  - Always read reference prefabs before creating new ones
  - Use TestMap scene for prototyping
  - Check console after changes: read_console()
  - Refresh Unity after script changes: refresh_unity()
  - Save prefabs after testing them in scene
EOF
}

echo "Unity pattern reference loaded. Type 'unity_patterns_help' for usage."
