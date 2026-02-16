# Development Shortcuts and Patterns

This document provides quick-reference patterns for common development tasks in the Cosmic Colony project.

## Wiki Operations

### Quick Wiki Queries

**Find item by name:**
```bash
# Shell function
wiki_find plasma_eel

# Or using tools directly
Grep: pattern="plasma_eel" path="CosmicWiki/pages"
```

**Find by ACNH equivalent:**
```bash
wiki_find_acnh "Sea Bass"

# Or:
Grep: pattern="ACNH Equivalent.*Sea Bass" path="CosmicWiki/pages/**/*.md"
```

**Extract JSON data:**
```bash
wiki_extract_json CosmicWiki/pages/nebula_organisms/plasma_eel.md

# Or:
Grep: pattern="```json" path="<wiki_file>" -A 100
```

**Search by criteria:**
```bash
# Rarity
Grep: pattern='"rarity": "Rare"' path="CosmicWiki/pages/**/*.md"

# Class
Grep: pattern='"primary_class": "PickableItem"' path="CosmicWiki/pages/**/*.md"

# Multiple criteria
wiki_search --rarity Rare --class PickableItem
```

### Wiki Data Structure

```
CosmicWiki/
├── data/
│   └── acnh_cosmic_mapping.json       # Master translation table
├── pages/
│   ├── nebula_organisms/              # Creatures (fish, bugs, etc.)
│   ├── tools/                         # Player equipment
│   ├── materials/                     # Crafting materials
│   ├── structures/                    # Buildings and furniture
│   └── zones/                         # Biomes and areas
├── guides/
│   └── CLAUDE_CODE_INTEGRATION.md     # Integration guide
└── scripts/
    └── generate_wiki_page.py          # Page generator
```

### Common Wiki Patterns

**Get item implementation details:**
1. `wiki_find <item_name>` → Get file path
2. `Read <file_path>` → Read full page
3. Look for "Technical Implementation" section
4. Extract JSON data block
5. Check "related_items" for dependencies

**Create new wiki page:**
```bash
cd CosmicWiki
python3 scripts/generate_wiki_page.py --interactive
```

## Unity Operations

### Asset Discovery

**Find specific assets:**
```bash
# Character models
Glob: pattern="Assets/**/Astronaut*.fbx"

# Tree prefabs
Glob: pattern="Assets/Pandazole*/**/*Tree*.prefab"

# All prefabs in pack
Glob: pattern="Assets/PolyOne/**/*.prefab"

# Animations
Glob: pattern="Assets/Kevin*/**/*Walk*.fbx"
```

**Search by asset type:**
```
manage_asset(action="search",
            path="Assets",
            filter_type="Prefab",
            page_size=50)
```

### Reference Prefabs

Always read these before creating similar objects:

**Player Character:**
```
Assets/TopDownEngine/Demos/Loft3D/Prefabs/PlayableCharacters/LoftSuspenders.prefab
```

**Pickable Item (3-component pattern):**
```
Assets/Prefabs/EnergyCrystalPicker.prefab
```

**Interactive Object:**
```
Assets/Prefabs/AlienTree.prefab
```

### Common Component Patterns

**Player Setup (from LoftSuspenders):**
- Character (main controller)
- CharacterMovement
- CharacterAbility components
- Health
- Animator
- CapsuleCollider (height: 2, radius: 0.5)
- Rigidbody (kinematic)

**Interactive Object:**
- ButtonActivated (for player interactions)
- SphereCollider (isTrigger: true, radius: 2-3)
- Animator (for visual feedback)
- Loot (for item drops)

**Pickable Item (3-component system):**
1. **Parent** (ItemPicker): Trigger collider for detection
2. **Child** (Item): InventoryItem component with item data
3. **Grandchild** (Visual): Model/sprite renderer

### Quick Workflows

**1. Set up new player character:**
```
1. Find model: Glob Assets/**/Astronaut*.fbx
2. Read reference: LoftSuspenders.prefab
3. Create GameObject: manage_gameobject(action="create", name="AstronautPlayer")
4. Add components: manage_components(action="add", ...)
5. Configure: manage_components(action="set_property", ...)
6. Test: manage_editor(action="play")
7. Save prefab: manage_prefabs(action="create_from_gameobject", ...)
```

**2. Create interactive object:**
```
1. Find asset: Glob pattern for tree/rock/etc
2. Create in scene: manage_gameobject(action="create", prefab_path=...)
3. Add ButtonActivated: manage_components(action="add", ...)
4. Add trigger collider: SphereCollider with isTrigger=true
5. Configure interaction: Set ButtonPromptText, etc.
6. Add Loot component: For items that drop
7. Test: manage_editor(action="play")
```

**3. Build showcase zone:**
```
1. Create parent: manage_gameobject(action="create", name="ZoneName")
2. Add ground: Create Plane as child, scale [10,1,10]
3. Add decorations: Place trees, rocks, props
4. Add interactive objects: Following pattern #2
5. Add lighting: Directional light in zone
6. Configure skybox: manage_material for sky
7. Test: manage_editor(action="play")
```

## Development Tools

### Helper Scripts

Load wiki helpers:
```bash
source scripts/helpers/wiki_query.sh
wiki_help  # Show available functions
```

Load Unity patterns:
```bash
source scripts/helpers/unity_patterns.sh
unity_patterns_help  # Show available patterns
```

### Console Monitoring

**Check for errors:**
```
read_console(action="get", types=["error"])
```

**Check compilation status:**
```
ReadMcpResource: server="UnityMCP" uri="mcpforunity://editor_state"
# Check isCompiling field
```

**Full console log:**
```
read_console(action="get", count=50, format="detailed")
```

### Scene Operations

**Get scene hierarchy (paginated):**
```
manage_scene(action="get_hierarchy",
            page_size=50,
            max_depth=3,
            include_transform=true)
```

**Find objects by name:**
```
find_gameobjects(search_method="by_name",
                search_term="Player")
```

**Find objects by component:**
```
find_gameobjects(search_method="by_component",
                search_term="CharacterMovement")
```

## Task Agent Usage

For complex multi-step operations, use Task tool with specialized agents:

**Explore codebase:**
```
Task(subagent_type="Explore",
     prompt="Find all Pandazole tree prefabs and list their components",
     description="Find tree prefabs")
```

**General research:**
```
Task(subagent_type="general-purpose",
     prompt="Search for all interactive object patterns in the codebase",
     description="Research interactive patterns")
```

**Execute bash operations:**
```
Task(subagent_type="Bash",
     prompt="Run wiki search for all rare items and extract their JSON data",
     description="Wiki rare items search")
```

## Integration Workflow

**From Wiki to Unity Implementation:**

1. **Research phase:**
   - Query wiki for item: `wiki_find <item>`
   - Read full page: `Read <wiki_file>`
   - Extract JSON: `wiki_extract_json <wiki_file>`
   - Note: rarity, class, price, spawn_rate, related_items

2. **Asset phase:**
   - Find suitable 3D model: `Glob Assets/**/*<suitable_model>*`
   - Check available animations: `Glob Assets/**/*<animation>*.fbx`
   - Read reference prefab for similar item type

3. **Implementation phase:**
   - Create GameObject with model
   - Add required components (based on class)
   - Configure properties (from JSON data)
   - Set up interactions (based on item type)
   - Configure spawning (from spawn_rate)
   - Test in TestMap scene

4. **Validation phase:**
   - Check console: `read_console()`
   - Test in play mode: `manage_editor(action="play")`
   - Verify against wiki specs (price, rarity match)
   - Save as prefab

5. **Documentation phase:**
   - Update implementation notes in wiki page
   - Note any deviations from spec
   - Document known issues

## Common Patterns

### Pattern: Find and Configure Asset

```
1. Glob: Find asset by pattern
2. Read: Check reference prefab structure
3. manage_gameobject: Create from prefab or primitive
4. manage_components: Add required components
5. manage_components: Configure properties
6. manage_editor: Test in play mode
7. read_console: Check for errors
8. manage_prefabs: Save as prefab
```

### Pattern: Multi-Zone Scene Setup

```
1. manage_scene: Create new scene or load existing
2. For each zone:
   a. Create zone parent GameObject
   b. Add terrain/ground
   c. Place environmental objects
   d. Add interactive objects
   e. Configure zone lighting
3. Create player spawn point
4. Configure scene camera
5. Set skybox
6. Test full scene flow
```

### Pattern: Character Animation Setup

```
1. Glob: Find animation files
2. manage_animation: Create animator controller
3. manage_animation: Add animation states
4. manage_animation: Add transitions
5. manage_animation: Set parameters
6. manage_components: Assign controller to Animator
7. Test: Play mode and trigger animations
```

## Tips and Best Practices

### Wiki Tips

- Always check related_items before implementing
- Follow sell_price and rarity specs exactly
- Use spawn_rate for placement frequency
- Validate implementations against JSON data

### Unity Tips

- Read reference prefabs before creating new ones
- Use TestMap scene for all prototyping
- Check console after every significant change
- Save prefabs after successful testing
- Use consistent naming: PascalCase for GameObjects
- Organize scene hierarchy logically (Environment, Gameplay, UI, Managers)

### Performance Tips

- Use paging for large queries (page_size=50)
- Don't request all GameObject properties at once
- Filter by criteria before reading full data
- Use specific search methods (by_id, by_name) when possible

### Error Recovery

- Check console first: `read_console()`
- Verify compilation: Check editor_state.isCompiling
- Refresh Unity: `refresh_unity(compile="request")`
- Read diagnostics: `getDiagnostics()`
- Check git status if files changed unexpectedly

## Keyboard Shortcuts

- Load wiki helpers: `source scripts/helpers/wiki_query.sh`
- Load Unity patterns: `source scripts/helpers/unity_patterns.sh`
- Quick wiki search: `wiki_find <item>`
- Quick wiki summary: `wiki_summary <file>`

## Next Steps

After setting up helpers:
1. Test wiki queries on existing items
2. Test Unity operations in TestMap scene
3. Run through one complete workflow (wiki → Unity)
4. Document any discovered issues or improvements
5. Begin sandbox scene implementation

---

**Last Updated:** 2026-02-16
**Related Docs:**
- CosmicWiki/guides/CLAUDE_CODE_INTEGRATION.md
- CosmicWiki/README.md
- scripts/helpers/wiki_query.sh
- scripts/helpers/unity_patterns.sh
