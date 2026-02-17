# Workflow: Replace Player Character Model

## Overview

Replace the visual model of a TopDown Engine player character while preserving all functionality, animations, and component configuration.

## Prerequisites

- Working player character prefab (e.g., LoftSuit, LoftSuspenders)
- New character model with humanoid rig (e.g., Astronaut)
- Animation controller configured for new model (or retargetable from original)

## Important Principles

**CRITICAL**: Always follow this sequence:
1. Get original character fully working in scene
2. Verify all controls and animations work
3. ONLY THEN swap the model
4. Test after each step

**Never** swap the model before verifying the original works!

## Step-by-Step Workflow

### Phase 1: Verify Original Character Works

**Goal**: Confirm the source prefab is fully functional before making any changes.

1. **Load scene with character**
   ```
   Scene: Loft3D.unity (or SandboxShowcase.unity)
   ```

2. **Check LevelManager spawning**
   ```
   find_gameobjects(search_method="by_name", search_term="LevelManager")
   Read component: LevelManager
   Verify: PlayerPrefabs array contains character prefab
   Example: ["Assets/TopDownEngine/Demos/Loft3D/Prefabs/PlayableCharacters/LoftSuit.prefab"]
   ```

3. **Enter play mode**
   ```
   manage_editor(action="play")
   ```

4. **Verify player spawned**
   ```
   find_gameobjects(search_method="by_tag", search_term="Player")
   Should return: 1 GameObject with Player tag
   ```

5. **Check player components**
   ```
   Read: GameObject with Player tag
   Verify components present:
   - Character
   - CharacterController
   - TopDownController3D
   - CharacterMovement
   - CharacterOrientation3D
   - All ability components (CharacterRun, CharacterJump3D, etc.)
   - Health
   - Animator (on Model child)
   ```

6. **Verify abilities initialized**
   ```
   Read component: CharacterMovement
   Check: AbilityInitialized = true

   Read component: CharacterOrientation3D
   Check: AbilityInitialized = true
   ```

7. **Verify animator setup**
   ```
   Find Model child GameObject
   Read component: Animator
   Check:
   - runtimeAnimatorController is set
   - avatar is assigned
   - All parameters exist (xSpeed, ySpeed, zSpeed, Walking, Running, etc.)
   ```

8. **Test controls** (manual - user testing)
   - Movement with WASD/arrows works
   - Camera rotation works
   - Animations play (idle, walking, running)
   - Jump works (if applicable)
   - Dash works (if applicable)

9. **Exit play mode**
   ```
   manage_editor(action="stop")
   ```

**CHECKPOINT**: If anything above fails, STOP and fix it before proceeding.

### Phase 2: Duplicate and Prepare New Prefab

**Goal**: Create a copy of the working prefab to modify.

10. **Duplicate the player prefab**
    ```
    manage_asset(action="duplicate",
                 path="Assets/TopDownEngine/Demos/Loft3D/Prefabs/PlayableCharacters/LoftSuit.prefab",
                 destination="Assets/Prefabs/AstronautPlayer.prefab")
    ```

11. **Open prefab for editing**
    ```
    # Unity will need to open prefab in isolation mode
    # Or we can instantiate it in scene to edit
    manage_prefabs(action="get_hierarchy",
                   prefab_path="Assets/Prefabs/AstronautPlayer.prefab")
    ```

### Phase 3: Identify Model Structure

**Goal**: Understand the mesh hierarchy before replacing it.

12. **Read prefab hierarchy**
    ```
    manage_prefabs(action="get_hierarchy",
                   prefab_path="Assets/Prefabs/AstronautPlayer.prefab")

    Expected structure:
    LoftSuit (root)
    ├── Model (Animator)
    │   ├── Dude (SkinnedMeshRenderer)
    │   ├── Pants (SkinnedMeshRenderer)
    │   ├── Shirt (SkinnedMeshRenderer)
    │   ├── Shoes (SkinnedMeshRenderer)
    │   ├── ... (other mesh parts)
    │   └── Armature (bone hierarchy)
    ├── WalkParticles
    └── TouchTheGroundParticles
    ```

13. **Note critical settings**
    - Animator controller name
    - Avatar reference
    - Bone hierarchy root name
    - Layer settings
    - Any material references

### Phase 4: Prepare New Model

**Goal**: Ensure the new model has compatible rig and animations.

14. **Locate new model**
    ```
    Glob: pattern="Assets/Markus Art/Character/Mesh/Astrounalts.fbx"
    ```

15. **Check new model import settings**
    ```
    manage_asset(action="get_info",
                 path="Assets/Markus Art/Character/Mesh/Astrounalts.fbx")

    Verify:
    - Rig Type: Humanoid
    - Animation Type: Generic or Humanoid
    - Has avatar defined
    ```

16. **Test animation compatibility** (optional)
    ```
    # Check if new model's rig can use original animations
    # This may require creating test animator controller
    ```

### Phase 5: Swap the Model

**Goal**: Replace mesh children while preserving animator setup.

17. **Open prefab in scene for editing**
    ```
    manage_gameobject(action="create",
                      name="AstronautPlayerEdit",
                      prefab_path="Assets/Prefabs/AstronautPlayer.prefab",
                      position=[0, 0, 0])
    ```

18. **Delete old mesh children** (keep Armature and Animator)
    ```
    find_gameobjects(search_method="by_name", search_term="Dude")
    manage_gameobject(action="delete", target="Dude")

    # Repeat for: Pants, Shirt, Shoes, etc.
    # KEEP: Model (Animator parent), Armature (bone hierarchy)
    ```

19. **Add new model as child of Model**
    ```
    manage_gameobject(action="create",
                      name="AstronautMesh",
                      parent="Model",
                      prefab_path="Assets/Markus Art/Character/Mesh/Astrounalts.fbx",
                      position=[0, 0, 0])
    ```

20. **Verify bone connections**
    ```
    Read: AstronautMesh SkinnedMeshRenderer
    Check: rootBone matches armature structure
    Check: bones array is populated
    ```

21. **Check animator still works**
    ```
    Read: Model Animator component
    Verify:
    - runtimeAnimatorController still assigned
    - avatar still assigned (may need updating)
    - All parameters still present
    ```

22. **Update avatar if needed**
    ```
    If new model has different avatar:
    manage_components(action="set_property",
                     target="Model",
                     component_type="Animator",
                     property="avatar",
                     value="<path_to_new_avatar>")
    ```

### Phase 6: Test New Character

**Goal**: Verify the model swap didn't break anything.

23. **Save changes back to prefab**
    ```
    manage_prefabs(action="create_from_gameobject",
                   target="AstronautPlayerEdit",
                   prefab_path="Assets/Prefabs/AstronautPlayer.prefab",
                   allow_overwrite=true)
    ```

24. **Update LevelManager to use new prefab**
    ```
    manage_components(action="set_property",
                     target="LevelManager",
                     component_type="LevelManager",
                     property="PlayerPrefabs",
                     value=["Assets/Prefabs/AstronautPlayer.prefab"])
    ```

25. **Enter play mode**
    ```
    manage_editor(action="play")
    ```

26. **Verify new character spawned**
    ```
    find_gameobjects(search_method="by_tag", search_term="Player")
    Read: Player GameObject
    Check name is "AstronautPlayer"
    ```

27. **Check all components initialized**
    ```
    Read component: CharacterMovement
    Verify: AbilityInitialized = true

    Read component: Animator
    Verify: Controller and avatar set correctly
    ```

28. **Check console for errors**
    ```
    read_console(types=["error"])
    Should be no new errors related to character
    ```

29. **Test controls** (manual - user testing)
    - Movement works with new model
    - Animations play correctly on new mesh
    - Camera control works
    - All abilities function

30. **Exit play mode**
    ```
    manage_editor(action="stop")
    ```

### Phase 7: Fine-Tuning (Optional)

31. **Adjust collider if needed**
    ```
    If new model has different proportions:
    manage_components(action="set_property",
                     target="AstronautPlayer",
                     component_type="CapsuleCollider",
                     properties={height: 2.0, radius: 0.5, center: [0, 1, 0]})
    ```

32. **Adjust animation speeds**
    ```
    If animations look too fast/slow:
    manage_animation(action="animator_set_speed",
                    target="AstronautPlayer",
                    speed=1.0)
    ```

33. **Update materials**
    ```
    If model materials need adjustment:
    manage_material(action="assign_material_to_renderer",
                   target="AstronautMesh",
                   material_path="<path_to_material>")
    ```

## Verification Checklist

After completing workflow, verify:

- [ ] Player spawns successfully in play mode
- [ ] All TopDown Engine components present
- [ ] Abilities initialize correctly (AbilityInitialized=true)
- [ ] Animator has controller and avatar assigned
- [ ] Movement controls work (WASD/arrows)
- [ ] Camera rotation works
- [ ] Animations play (idle, walk, run, jump, etc.)
- [ ] Model is visible in scene
- [ ] No errors in console related to character
- [ ] Collider fits new model properly
- [ ] Jump/dash abilities work (if applicable)

## Common Issues

### Animations Don't Play
**Cause**: Avatar mismatch or animator not retargeted
**Fix**:
- Verify new model's avatar is compatible
- Update Animator.avatar reference
- Check animation controller uses Generic or matches rig type

### Character Falls Through Ground
**Cause**: Collider not adjusted for new model
**Fix**:
- Adjust CapsuleCollider height/radius
- Verify CharacterController is enabled
- Check Ground layer collision matrix

### Abilities Don't Initialize
**Cause**: Component references broken during swap
**Fix**:
- Check Character.CharacterAnimator points to Model
- Verify Character.CharacterModel reference
- Re-initialize by exiting and re-entering play mode

### Model Not Visible
**Cause**: Materials not assigned or mesh disabled
**Fix**:
- Check SkinnedMeshRenderer enabled
- Verify materials assigned to mesh
- Check model isn't behind camera

## API References

- **Character Component**: https://topdown-engine-docs.moremountains.com/API/class_more_mountains_1_1_top_down_engine_1_1_character.html
- **CharacterAbility**: https://topdown-engine-docs.moremountains.com/API/class_more_mountains_1_1_top_down_engine_1_1_character_ability.html
- **LevelManager**: https://topdown-engine-docs.moremountains.com/API/class_more_mountains_1_1_top_down_engine_1_1_level_manager.html

## Related Workflows

- `create_new_character.md` - Creating character from scratch
- `animation_setup.md` - Setting up animator controllers
- `ability_configuration.md` - Configuring character abilities

---

**Last Updated**: 2026-02-16
**Test Status**: Ready for testing with Astronaut model
