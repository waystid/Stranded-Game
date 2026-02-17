# TopDown Engine Integration Guide

## Overview

This directory contains TopDown Engine implementation patterns, workflows, and component references for Cosmic Colony development.

## Documentation Sources

- **Official API**: https://topdown-engine-docs.moremountains.com/API/index.html
- **Character System**: https://topdown-engine-docs.moremountains.com/API/class_more_mountains_1_1_top_down_engine_1_1_character.html
- **Component Reference**: https://topdown-engine-docs.moremountains.com/API/annotated.html

## Directory Structure

```
topdown_engine/
├── README.md                    # This file
├── workflows/                   # Step-by-step implementation workflows
│   ├── replace_player_model.md  # How to swap character models
│   ├── create_pickable_item.md  # Item creation workflow
│   └── create_interactive_object.md
├── components/                  # Component references and configs
│   ├── character_abilities.md   # All CharacterAbility components
│   ├── ai_components.md         # AI Brain and Actions
│   └── item_components.md       # InventoryItem, ItemPicker, Loot
└── patterns/                    # Common implementation patterns
    ├── 3_component_item.md      # ItemPicker/Item/Visual pattern
    ├── spawning_system.md       # LevelManager spawning
    └── ability_initialization.md
```

## Quick Reference

### Character System

**Core Components** (required for playable character):
- `Character` - Main controller, manages all abilities
- `CharacterController` - Unity's built-in character controller
- `TopDownController3D` - TopDown Engine 3D controller
- `CharacterMovement` - Basic movement ability
- `CharacterOrientation3D` - Rotation and facing direction
- `Health` - Health management

**Common Abilities**:
- `CharacterRun` - Running/sprinting
- `CharacterJump3D` - Jumping in 3D
- `CharacterDash3D` - Dash ability
- `CharacterCrouch` - Crouching
- `CharacterButtonActivation` - Interact with objects
- `CharacterHandleWeapon` - Weapon handling
- `CharacterInventory` - Inventory system
- `CharacterRotateCamera` - Camera control

### Animation System

The `Character` component manages an `Animator` reference:
- Set via `CharacterAnimator` field in inspector
- Usually attached to child "Model" GameObject
- Uses Mecanim animation controller
- Parameters set automatically by abilities:
  - `xSpeed`, `ySpeed`, `zSpeed` - Movement direction
  - `Walking`, `Running`, `Idle` - State bools
  - `Jumping`, `Dashing`, `Crouching` - Action bools
  - `Grounded`, `Alive` - Status bools

### Spawning System

TopDown Engine uses **runtime spawning** via LevelManager:
- Player prefabs are NOT placed in scene
- `LevelManager` component has `PlayerPrefabs` array
- Players spawn at `InitialSpawnPoint` on scene start
- Spawned players get proper initialization automatically

### Item System

**3-Component Pattern**:
1. **ItemPicker** (parent) - Trigger collider for detection
2. **InventoryItem** (child) - Actual item data and properties
3. **Visual** (grandchild) - 3D model or sprite

Example hierarchy:
```
PlasmaEelPicker (ItemPicker, SphereCollider)
└── PlasmaEelItem (InventoryItem)
    └── EelModel (MeshRenderer)
```

## Integration with CosmicWiki Game Content

### Workflow: Wiki → Implementation

1. **Query CosmicWiki** for game content specs
   ```bash
   source scripts/helpers/wiki_query.sh
   wiki_find plasma_eel
   wiki_extract_json CosmicWiki/pages/nebula_organisms/plasma_eel.md
   ```

2. **Reference TopDown Engine workflow**
   ```bash
   # Read implementation workflow
   Read CosmicWiki/topdown_engine/workflows/create_pickable_item.md
   ```

3. **Apply specs from wiki to Unity**
   - Use `ItemName` from wiki
   - Set `ItemPrice` from wiki's `sell_price`
   - Configure rarity, spawn_rate, etc.

4. **Validate against wiki**
   - Check price matches
   - Verify rarity tier
   - Confirm visual style

## Helper Scripts

The project includes shell helpers for quick queries:

```bash
# Load helpers
source scripts/helpers/wiki_query.sh
source scripts/helpers/unity_patterns.sh
source scripts/helpers/topdown_engine.sh  # TopDown Engine specific

# Query wiki
wiki_find plasma_eel

# Get TopDown Engine workflow
topdown_workflow player_model_swap
```

## API Query Pattern

For runtime questions about TopDown Engine:

1. **Check local workflows first** (this directory)
2. **Reference official API**: https://topdown-engine-docs.moremountains.com/API/
3. **Search by component name**: e.g., "Character", "CharacterMovement"
4. **Check inheritance**: Many abilities inherit from `CharacterAbility`

### WebFetch Examples

```
# Get Character component documentation
WebFetch: url="https://topdown-engine-docs.moremountains.com/API/class_more_mountains_1_1_top_down_engine_1_1_character.html"
          prompt="Extract all public fields and methods for the Character component"

# Get CharacterAbility base class
WebFetch: url="https://topdown-engine-docs.moremountains.com/API/class_more_mountains_1_1_top_down_engine_1_1_character_ability.html"
          prompt="List initialization requirements and ability lifecycle"
```

## Best Practices

### Character Setup
1. Always use prefabs from existing demos as templates
2. Copy component configuration, don't recreate manually
3. Test in demo scene first before custom scenes
4. Verify animator references are correct
5. Check ability initialization (AbilityInitialized should be true)

### Item Creation
1. Follow 3-component pattern strictly
2. Parent has ONLY trigger collider and ItemPicker
3. Child has ONLY InventoryItem component
4. Grandchild has ONLY visual (mesh/sprite renderer)
5. Set ItemPicker `Item` reference to child

### Scene Setup
1. Always include GameManager
2. Include LevelManager with player prefab array
3. Set InitialSpawnPoint for player spawn
4. Include SoundManager for audio
5. Use proper camera system (3DCameras for 3D)

## Testing

### Verify Character Setup
```
1. Enter play mode
2. Check Player tag spawned: find_gameobjects(search_method="by_tag", search_term="Player")
3. Check abilities initialized: read component CharacterMovement, verify AbilityInitialized=true
4. Check animator: read component Animator, verify runtimeAnimatorController is set
5. Test movement: verify xSpeed/ySpeed parameters change when moving
```

### Verify Item Setup
```
1. Check ItemPicker component has Item reference set
2. Check InventoryItem has correct ItemName and ItemPrice
3. Test pickup in play mode
4. Verify item appears in inventory
```

## Troubleshooting

### Animations Not Playing
- Check `Character.CharacterAnimator` field is set
- Verify Animator on Model child has controller assigned
- Check abilities are initialized (AbilityInitialized=true)
- Verify animator parameters exist and are correct type

### Player Not Spawning
- Check LevelManager.PlayerPrefabs array is populated
- Verify InitialSpawnPoint exists in scene
- Check GameManager is present
- Look for spawn errors in console

### Items Not Pickable
- Verify ItemPicker has trigger collider (isTrigger=true)
- Check ItemPicker.Item reference points to child
- Ensure player has CharacterInventory component
- Check layers (ItemPicker should be on Interactable layer)

## Next Steps

1. Complete workflow documents for common tasks
2. Add component configuration templates
3. Document integration points with CosmicWiki content
4. Create validation scripts to check implementations match specs

---

**Last Updated**: 2026-02-16
**Version**: 1.0
