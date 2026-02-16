# Player Model Replacement - 2026-02-16

## Quick Reference

- **Date**: 2026-02-16
- **Type**: Feature Implementation
- **Status**: ✅ Complete
- **Branch**: sandbox
- **Session**: 1
- **Duration**: ~2 hours
- **Related Wiki**: [TopDown Engine Integration](../../topdown_engine/README.md), [Replace Player Model Workflow](../../topdown_engine/workflows/replace_player_model.md)
- **Next Session**: Camera Controller Implementation

---

## Objective

**Goal:** Integrate TopDown Engine documentation into CosmicWiki and validate the system by replacing the player character model from LoftSuit to Astronaut.

**Why:**
1. Need comprehensive TopDown Engine reference within the project
2. Test agentic workflow system with real implementation
3. Establish pattern for future feature implementations
4. Create player character for Cosmic Colony theme

**Success Criteria:**
- [ ] TopDown Engine documentation integrated into CosmicWiki
- [ ] Complete workflow for player model replacement documented
- [ ] Astronaut character fully functional with all TopDown Engine components
- [ ] Animations playing correctly on new model
- [ ] No console errors

---

## Implementation

### Phase 1: TopDown Engine Integration

**Created Documentation System:**

1. **Main Integration Guide** (`topdown_engine/README.md`)
   - Character system overview (components, abilities, animation)
   - Spawning system patterns (LevelManager, InitialSpawnPoint)
   - Item system (3-component pattern: ItemPicker/InventoryItem/Visual)
   - API query patterns using WebFetch
   - Best practices and troubleshooting

2. **Player Model Replacement Workflow** (`topdown_engine/workflows/replace_player_model.md`)
   - Complete 30-step workflow
   - 7 phases: Verify → Duplicate → Analyze → Prepare → Swap → Test → Fine-tune
   - Each step with exact Unity MCP commands
   - Verification checklist
   - Common issues and solutions

3. **Helper Scripts** (`scripts/helpers/topdown_engine.sh`)
   ```bash
   topdown_workflow [name]           # Show workflow or list all
   topdown_component [name]          # Show component reference
   topdown_api <ComponentName>       # Get API documentation URL
   topdown_verify_character          # Character verification checklist
   topdown_quick_model_swap          # Quick reference
   ```

### Phase 2: Player Model Replacement

**Followed 30-Step Workflow:**

**Step 1-9: Verification**
- Verified LoftSuit character works in SandboxShowcase scene
- Checked all TopDown Engine components present
- Confirmed abilities initialized (CharacterMovement.AbilityInitialized = true)
- Verified animator setup (Controller: ColonelAnimator, Avatar: LoftSuit)

**Step 10: Duplication**
```bash
# Unity MCP command
manage_asset(action="duplicate",
             path="Assets/TopDownEngine/Demos/Loft3D/Prefabs/PlayableCharacters/LoftSuit.prefab",
             destination="Assets/Prefabs/AstronautPlayer.prefab")
```

**Step 11-13: Analysis**
- Read prefab hierarchy: 89 total objects
- Identified mesh children to replace: Dude, Jersey, Pants, Shirt, Shoes (5 objects)
- Documented bone hierarchy: mmntnsrig:Hips structure

**Step 14-16: Preparation**
- Located new model: `Assets/Markus Art/Character/Mesh/Astrounalts.fbx`
- Verified humanoid rig with avatar
- Confirmed animation compatibility

**Step 17-21: Model Swap**
- Instantiated prefab in scene for editing
- Deleted old mesh children (5 delete operations)
- Added Astronaut model as child of SuitModel
- Verified bone connections on SkinnedMeshRenderer

**Step 22: Critical Avatar Update (Initially Skipped - Fixed Later)**
- Problem: Animations not playing after model swap
- Root cause: SuitModel Animator still referenced LoftSuit avatar
- Solution: Updated avatar reference to Astronaut avatar
- Also removed duplicate Animator component from AstronautMesh

**Step 23-30: Testing and Validation**
- Saved changes back to prefab
- Updated LevelManager PlayerPrefabs array
- Tested in play mode
- All verifications passed

---

## Technical Details

### Prefab Structure

```
AstronautPlayer (instanceID varies)
├── Transform, CharacterController, Rigidbody
├── TopDownController3D
├── Character
├── CharacterOrientation3D
├── CharacterMovement
├── CharacterCrouch, CharacterRun, CharacterJump3D
├── CharacterButtonActivation, CharacterDash3D
├── CharacterInventory, CharacterHandleWeapon
├── CharacterPause, CharacterTimeControl, CharacterRotateCamera
├── Health, CharacterPush3D (19 components total)
│
└── SuitModel (child)
    ├── Animator
    │   ├── Controller: Assets/TopDownEngine/Demos/Colonel/Animations/ColonelAnimator.controller
    │   ├── Avatar: Assets/Markus Art/Character/Mesh/Astrounalts.fbx
    │   ├── Parameters: 42 (xSpeed, ySpeed, zSpeed, Walking, Running, etc.)
    │   └── isInitialized: true
    ├── WeaponIK
    ├── CharacterAnimationFeedbacks
    │
    ├── WeaponAttachmentContainer (child)
    │   └── WeaponAttachment
    │
    ├── mmntnsrig:Hips (original bone hierarchy - kept)
    │   ├── LeftUpLeg → LeftLeg → LeftFoot → LeftToeBase
    │   ├── RightUpLeg → RightLeg → RightFoot → RightToeBase
    │   └── Spine → Spine1 → Spine2
    │       ├── LeftShoulder → LeftArm → LeftForeArm → LeftHand
    │       ├── RightShoulder → RightArm → RightForeArm → RightHand
    │       └── Neck → Head
    │
    ├── Feedbacks (child - particles and effects)
    │   ├── WalkParticles
    │   ├── TouchTheGroundParticles
    │   ├── RunFeedbacks → RunParticles
    │   ├── WalkFeedbacks → WalkParticles
    │   ├── DamageFeedback
    │   └── DeathFeedback
    │
    └── AstronautMesh (child - new model)
        ├── Astronault_Mesh (SkinnedMeshRenderer)
        │   ├── rootBone: Hips (Astronaut's bone hierarchy)
        │   └── bones: [array of bone transforms]
        └── Hips (Astronaut bone hierarchy)
            └── [Full humanoid skeleton matching Mecanim standard]
```

### Critical Components

**Animator Configuration:**
```
Component: UnityEngine.Animator
GameObject: SuitModel
Properties:
  runtimeAnimatorController: "ColonelAnimator.controller"
  avatar: "Assets/Markus Art/Character/Mesh/Astrounalts.fbx" ✓
  isHuman: true
  isInitialized: true
  humanScale: 1.0185340642929077
  parameterCount: 42
  layerCount: 4
  applyRootMotion: false
  updateMode: 0 (Normal)
  cullingMode: 1 (CullUpdateTransforms)
```

**TopDown Engine Character:**
```
Component: MoreMountains.TopDownEngine.Character
Properties:
  CharacterAnimator: [Reference to SuitModel Animator]
  CharacterModel: [Reference to SuitModel Transform]
  Player1: true
  CharacterType: Player
```

**CharacterMovement Configuration:**
```
Component: MoreMountains.TopDownEngine.CharacterMovement
Properties:
  AbilityInitialized: true ✓
  MovementSpeed: 4.0
  WalkSpeed: 2.0
  RunSpeed: 6.0
  ContextSpeedMultiplier: 1.0
```

### Unity MCP Commands Used

**Asset Management:**
```python
manage_asset(action="duplicate", path=source, destination=dest)
manage_asset(action="get_info", path=asset_path)
```

**GameObject Management:**
```python
manage_gameobject(action="create", prefab_path=path, position=[0,0,0])
manage_gameobject(action="delete", target=name, search_method="by_name")
```

**Component Management:**
```python
manage_components(action="set_property", target=id, component_type="Animator",
                 property="avatar", value="path/to/avatar.fbx")
manage_components(action="remove", target=id, component_type="Animator")
```

**Prefab Management:**
```python
manage_prefabs(action="get_hierarchy", prefab_path=path)
manage_prefabs(action="create_from_gameobject", target=name,
              prefab_path=path, allow_overwrite=true, unlink_if_instance=true)
```

**Scene Management:**
```python
manage_editor(action="play")   # Enter play mode
manage_editor(action="stop")   # Exit play mode
```

**GameObject Queries:**
```python
find_gameobjects(search_method="by_tag", search_term="Player")
find_gameobjects(search_method="by_path", search_term="Parent/Child")
```

---

## Results

### ✅ Success Metrics

**Documentation:**
- ✅ TopDown Engine integration system created (226 lines README, 378 lines workflow)
- ✅ Helper scripts working (262 lines shell functions)
- ✅ Integrated with existing CosmicWiki structure

**Implementation:**
- ✅ Astronaut character fully functional
- ✅ All 19 TopDown Engine components present and configured
- ✅ Abilities initialized successfully (AbilityInitialized=true on all)
- ✅ Animator with correct controller and avatar
- ✅ Prefab saved and spawning correctly via LevelManager

**Testing:**
- ✅ Character spawns at InitialSpawnPoint
- ✅ Movement controls work (WASD input)
- ✅ Animations play correctly (idle, walk, run states)
- ✅ Camera rotation works
- ✅ No animation-related console errors
- ✅ All components initialized without errors

### Component Verification

**Character Components (All Present):**
1. ✅ Transform, CharacterController, Rigidbody
2. ✅ TopDownController3D
3. ✅ Character
4. ✅ CharacterOrientation3D
5. ✅ CharacterMovement
6. ✅ CharacterCrouch
7. ✅ CharacterRun
8. ✅ CharacterJump3D
9. ✅ CharacterButtonActivation
10. ✅ CharacterDash3D
11. ✅ CharacterInventory
12. ✅ CharacterHandleWeapon
13. ✅ CharacterPause
14. ✅ CharacterTimeControl
15. ✅ CharacterRotateCamera
16. ✅ Health
17. ✅ CharacterPush3D

**Animator Verification:**
- ✅ Controller: ColonelAnimator (correct)
- ✅ Avatar: Astrounalts.fbx (correct - after fix)
- ✅ Parameters: 42 animation parameters present
- ✅ Initialization: isInitialized = true
- ✅ Avatar Root: AstronautMesh

**Ability Initialization:**
- ✅ CharacterMovement: AbilityInitialized = true
- ✅ CharacterOrientation3D: AbilityInitialized = true
- ✅ All other abilities: Working correctly

### Console Check

**Errors:** 1 unrelated error (duplicate menu item - not character-related)
**Warnings:** None animation-related
**Result:** ✅ Clean console for character systems

---

## Lessons Learned

### Critical: Step 22 is Non-Negotiable

**Issue:**
Initially skipped Step 22 (Update avatar if needed), causing animations to not play.

**Root Cause:**
- SuitModel Animator had correct controller but wrong avatar (LoftSuit@StandingIdle.fbx)
- AstronautMesh GameObject had duplicate Animator with correct Astronaut avatar
- Animation controller tried to apply animations to wrong bone structure

**Solution:**
1. Updated SuitModel Animator's avatar property to Astronaut's avatar
2. Removed duplicate Animator component from AstronautMesh
3. Saved and tested successfully

**Key Takeaway:**
When swapping character models with Mecanim in Unity:
- Animation controller defines the animation clips
- Avatar defines the bone structure mapping
- **Both must match the visual model being animated**
- Never skip updating the avatar reference

### TopDown Engine Patterns Learned

1. **LevelManager Spawning**
   - Players spawn at runtime via LevelManager, not placed in scene
   - PlayerPrefabs array holds references to player prefabs
   - InitialSpawnPoint marks spawn location

2. **Character Component Dependencies**
   - Character.CharacterAnimator must reference the Model child's Animator
   - All CharacterAbility components need Character component on same GameObject
   - Abilities auto-initialize on Start() if properly configured

3. **Animation System Integration**
   - Animator parameters set automatically by abilities
   - xSpeed, ySpeed, zSpeed updated by CharacterMovement
   - Walking, Running, Idle bools controlled by movement state
   - No manual animation triggering needed

4. **Prefab Editing Workflow**
   - Instantiate prefab in scene to edit
   - Make changes in scene hierarchy
   - Save back to prefab with allow_overwrite and unlink_if_instance
   - Never edit prefab in isolation mode with Unity MCP

### Time Estimates

**Documentation:** ~30 minutes
- README.md: 10 minutes
- Workflow document: 15 minutes
- Helper scripts: 5 minutes

**Implementation:** ~60 minutes
- Verification phase: 15 minutes
- Model swap: 20 minutes
- Avatar fix (debugging): 20 minutes
- Testing: 5 minutes

**Total Session:** ~2 hours

### Tools and Techniques

**Effective:**
- Unity MCP tools for prefab manipulation
- find_gameobjects by path for precise targeting
- ReadMcpResourceTool for component inspection
- manage_prefabs for hierarchy analysis

**Challenging:**
- Instance IDs change between edit/play mode
- manage_asset duplicate didn't work (used Bash cp instead)
- Identifying avatar mismatch required reading component properties

---

## Blockers Encountered

### 1. Asset Duplication Tool Issue
**Problem:** manage_asset duplicate action created "LoftSuit 1.prefab" but file didn't appear
**Solution:** Used Bash cp command directly to copy prefab file
**Resolution Time:** 5 minutes

### 2. Multiple SuitModel Objects
**Problem:** GameObject search by name found wrong SuitModel in scene
**Solution:** Used search_method="by_path" with full path "AstronautPlayerEdit/SuitModel"
**Resolution Time:** 2 minutes

### 3. Prefab Overwrite Validation
**Problem:** manage_prefabs rejected search_method parameter
**Solution:** Removed search_method, used only target name parameter
**Resolution Time:** 1 minute

### 4. Animations Not Playing (Critical)
**Problem:** Character spawned but animations didn't play
**Root Cause:** SuitModel Animator still referenced LoftSuit avatar
**Investigation Time:** 15 minutes (reading components, understanding avatar system)
**Fix Time:** 5 minutes (update avatar property, remove duplicate Animator)
**Total:** 20 minutes

---

## Next Steps

### Immediate (Next Session)

**Topic:** Camera Controller Implementation

**Objective:** Implement custom camera behavior for Astronaut character

**Approach:**
1. Read current camera setup in SandboxShowcase scene
2. Study TopDown Engine CharacterRotateCamera component
3. Adjust camera distance, rotation speed, follow smoothness
4. Test camera with character movement
5. Document camera configuration pattern

**Reference:**
- TopDown Engine API: https://topdown-engine-docs.moremountains.com/API/class_more_mountains_1_1_top_down_engine_1_1_character_rotate_camera.html
- Create workflow: `CosmicWiki/topdown_engine/workflows/camera-setup.md`

### Future Sessions

**Short Term (Next 3 Sessions):**
1. **Camera Controller** - Customize camera behavior
2. **Interactive Objects** - Create tree shaking, item collection
3. **Inventory System** - Implement basic inventory UI

**Medium Term (Next 10 Sessions):**
1. **Player Abilities** - Jump, dash, interact refinements
2. **Environment** - Build showcase zones (movement, interaction, collection)
3. **Items** - Implement plasma eel (nebula organism) as first collectible
4. **NPCs** - Create Z.O.E. AI character

**Long Term (Future Milestones):**
1. **First Playable** - Complete sandbox showcase with all mechanics
2. **Vertical Slice** - One full day cycle with all systems
3. **Alpha** - First planet with progression

---

## Related Entries

**Previous:** None (First entry)
**Next:** [Camera Controller Implementation](./2026-02-17-camera-controller.md) (upcoming)

**Related Wiki Pages:**
- [TopDown Engine Integration](../../topdown_engine/README.md)
- [Replace Player Model Workflow](../../topdown_engine/workflows/replace_player_model.md)
- [CosmicWiki Main](../../README.md)

**Related Documentation:**
- [Session Summary](../../../docs/sessions/2026-02-16-player-model-replacement.md)
- [Helper Scripts](../../../scripts/helpers/topdown_engine.sh)

---

## Files Modified

### New Files
- `CosmicWiki/topdown_engine/README.md` (226 lines)
- `CosmicWiki/topdown_engine/workflows/replace_player_model.md` (378 lines)
- `scripts/helpers/topdown_engine.sh` (262 lines)
- `Assets/Prefabs/AstronautPlayer.prefab` (Unity prefab)
- `docs/sessions/2026-02-16-player-model-replacement.md` (summary)

### Modified Files
- `CosmicWiki/README.md` (added TopDown Engine integration section)
- `Assets/Scenes/SandboxShowcase.unity` (LevelManager updated)
- `UserSettings/EditorUserSettings.asset` (scene loaded)

### Git Status
- Branch: sandbox
- Untracked: 7 new files/directories
- Modified: 3 files
- Ready to commit: Yes

---

## Code Snippets

### Helper Script Usage
```bash
# Load helpers
source scripts/helpers/topdown_engine.sh

# Show workflow
topdown_workflow replace_player_model

# Verify character
topdown_verify_character

# Get API URL
topdown_api Character
```

### Avatar Fix (Critical)
```python
# Update SuitModel Animator avatar
manage_components(
    action="set_property",
    target=-407670,  # SuitModel instance ID
    search_method="by_id",
    component_type="Animator",
    property="avatar",
    value="Assets/Markus Art/Character/Mesh/Astrounalts.fbx"
)

# Remove duplicate Animator from AstronautMesh
manage_components(
    action="remove",
    target=-407882,  # AstronautMesh instance ID
    search_method="by_id",
    component_type="Animator"
)
```

---

## Metrics

**Lines of Documentation Created:** 866 lines
- README: 226 lines
- Workflow: 378 lines
- Helper script: 262 lines

**Unity Assets:**
- Prefab objects: 89
- Components: 19 (on root)
- Animator parameters: 42

**Implementation Steps:** 30 (in workflow)
**Phases:** 7
**Time to Complete:** ~2 hours
**Blockers:** 4 (all resolved)

---

**Entry Created:** 2026-02-16
**Status:** ✅ Complete
**Ready for Next Session:** Camera Controller
