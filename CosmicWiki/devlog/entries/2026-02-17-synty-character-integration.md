# Synty Character Integration + HumanCustomPlayer - 2026-02-17

## Quick Reference

- **Date**: 2026-02-17
- **Type**: Feature Implementation
- **Status**: ✅ Complete
- **Branch**: features/007-character-creator
- **Session**: 6
- **Duration**: ~1.5 hours
- **Related Wiki**: [Synty Sidekick Agent](../../agents/synty-sidekick-agent.md)
- **Next Session**: In-game Synty Character Customizer (Phase B) — part picker UI

---

## Objective

**Goal:** Verify Synty Sidekick exported characters animate correctly in-game via Humanoid retargeting, then swap the sandbox player from AstronautPlayer to a Synty character.

**Why:**
1. Confirm the Synty → Unity export pipeline is usable for development characters
2. Replace the placeholder astronaut with a Synty human for feature 007 testing
3. Establish the pattern for future character swaps

**Success Criteria:**
- [x] Exported Human-Custom character drives TDE animations without errors
- [x] Character spawns in SandboxShowcase as the player
- [x] Full movement + animation system working

---

## Implementation

### Phase 1: Animation Retargeting Verification

**Description:** Test whether a character exported from the Synty Sidekick Character Tool can be driven by the TDE Animator controller via Humanoid retargeting.

**Steps:**
1. Exported a randomized character from the Synty Sidekick Character Tool → `Assets/Synty/Exports/Human-Custom/`
2. Instantiated `Human-Custom.prefab` in CharacterCreator scene
3. Confirmed `isHuman: true` on exported Avatar — Humanoid rig, ready for retargeting
4. Assigned `LoftSuitAnimatorController` to the Animator component
5. Entered play mode — `hasBoundPlayables: true`, `layerCount: 1`, all 21 parameters loaded, no errors

**Result:** ✅ Retargeting works out of the box. Any Synty Sidekick export is immediately compatible with TDE animation controllers.

---

### Phase 2: HumanCustomPlayer Prefab

**Description:** Create a player prefab using Human-Custom as the visual, keeping all TDE components from SidekickPlayer.

**Steps:**
1. Instantiated `SidekickPlayer.prefab` as base
2. Disabled `SyntyMesh` (HumanSpecies_01 nested prefab)
3. Added `Human-Custom.prefab` as child of `SuitModel` (named `HumanCustomMesh`)
4. Removed `Animator` from `HumanCustomMesh` (SuitModel Animator drives it)
5. Set `SuitModel.Animator.avatar = Human-Custom-avatar.asset`
6. Saved as `Assets/Prefabs/HumanCustomPlayer.prefab`
7. Updated `LevelManager.PlayerPrefabs[0]` → `HumanCustomPlayer.prefab`

**Blocker encountered:** No visible animations despite Animator running.

---

### Phase 3: avatarRoot Bug Fix

**Description:** Diagnosed and fixed missing animations.

**Root Cause:** Unity's Humanoid Animator scans ALL descendants (including inactive) during `avatarRoot` discovery. `SyntyMesh` (disabled) was found before `HumanCustomMesh` because it contained a `root` bone child matching the avatar definition.

**Diagnosis:** In play mode, `Animator.avatarRoot = SyntyMesh (disabled)` — animations were driving invisible disabled bones.

**Fix:** Deleted `SyntyMesh` entirely from the prefab instead of just disabling it. `avatarRoot` immediately resolved to `HumanCustomMesh`.

**Verified:** `avatarRoot = HumanCustomMesh`, `hasBoundPlayables: true`, character visible in animated idle pose in screenshot.

---

## Technical Details

### HumanCustomPlayer Prefab Structure

```
HumanCustomPlayer (root — 20 TDE components incl. CharacterCustomizer)
  └─ SuitModel (Animator + Human-Custom-avatar + WeaponIK + CharacterAnimationFeedbacks)
       ├─ WeaponAttachmentContainer
       │    └─ WeaponAttachment (MMWiggle)
       ├─ Feedbacks (Walk/Run/Damage/Death particle feedbacks)
       └─ HumanCustomMesh (Human-Custom export — SkinnedMeshRenderer + bone hierarchy)
```

### Animator Configuration (SuitModel)

```
Component: UnityEngine.Animator
runtimeAnimatorController: Assets/TopDownEngine/Demos/Colonel/Animations/ColonelAnimator.controller
avatar: Assets/Synty/Exports/Human-Custom/Meshes/Human-Custom-avatar.asset
avatarRoot: HumanCustomMesh  ← CRITICAL: must be the active mesh container
isHuman: true
hasBoundPlayables: true
layerCount: 4
parameterCount: 42
```

### Export Package Contents

```
Assets/Synty/Exports/Human-Custom/
  Human-Custom.prefab       ← instantiated as HumanCustomMesh
  Human-Custom.sk           ← Synty save file for re-editing
  Meshes/
    Human-Custom.asset      ← combined SkinnedMeshRenderer mesh
    Human-Custom-avatar.asset  ← Humanoid avatar (isHuman: true)
  Materials/Human-Custom.mat
  Textures/T_Human-CustomColorMap.png  ← baked color atlas
```

---

## Results

### ✅ Success Metrics

- ✅ Animation retargeting confirmed working (no errors, hasBoundPlayables: true)
- ✅ Human-Custom character spawns as player in SandboxShowcase
- ✅ Movement and animations playing correctly
- ✅ No console errors (only pre-existing DamageFeedback renderer warning)

### Console/Errors

**Errors:** 0
**Warnings:** 1 (pre-existing FlickerFeedback renderer — unrelated)
**Result:** ✅ Clean

---

## Lessons Learned

### What Worked Well

1. **Synty Humanoid Export**
   - Synty Sidekick exports Humanoid avatars by default
   - Zero configuration needed for Mecanim retargeting
   - Any TDE controller works immediately on exported characters

2. **SidekickPlayer as base prefab**
   - Already has all TDE components correctly wired
   - Swap only the mesh child, keep everything else

### Critical Steps

1. **Delete old SyntyMesh, never just disable**
   - Unity finds disabled GameObjects during `avatarRoot` discovery
   - Disabled SyntyMesh will be chosen as avatarRoot even over an active HumanCustomMesh
   - **Always delete** the old skeleton GO, never just `setActive(false)`

2. **Remove Animator from the mesh child**
   - Human-Custom.prefab ships with its own Animator
   - SuitModel Animator drives the rig — second Animator on HumanCustomMesh creates conflicts
   - Remove it during prefab construction

---

## Blockers Encountered

### 1. avatarRoot Binding to Disabled SyntyMesh

**Problem:** Character moved but showed no animations. SuitModel Animator running (hasBoundPlayables: true) but driving invisible disabled bones.

**Investigation:** Checked `Animator.avatarRoot` in play mode → returned `SyntyMesh` (disabled) instead of `HumanCustomMesh`.

**Solution:** Deleted SyntyMesh GO entirely from the prefab. Unity then correctly bound avatarRoot to HumanCustomMesh.

**Resolution Time:** ~20 min

**Prevention:** Always delete old skeleton GOs rather than disabling. Document this in team patterns.

---

## Next Steps

### Immediate (Next Session — Feature 007 Phase B)

**Topic:** In-Game Synty Character Customizer

**Objective:** Replace the static `CharacterCreatorUI.cs` color-swatch approach with a real part-picker that drives the Synty Sidekick runtime API — letting players swap head, upper body, lower body parts in-game.

**Reference:**
- [Phase B Plan](../../guides/feature-007-phase-b-plan.md)
- `Assets/Scripts/UI/CharacterCreatorUI.cs` — existing UI scaffold
- `Assets/Prefabs/HumanCustomPlayer.prefab` — base player prefab

### Future Sessions

**Short Term:**
1. **Phase B** — In-game part picker (head, upper, lower body)
2. **Phase C** — Color picker wired to Synty shader properties
3. **Phase D** — Species switching (Human I–IV, Starter I–IV)

---

## Related Entries

**Previous:** [2026-02-17-world-expansion-bugfixes.md](./2026-02-17-world-expansion-bugfixes.md)
**Related Wiki:** [Synty Sidekick Agent](../../agents/synty-sidekick-agent.md)

---

## Files Modified

### New Files
- `Assets/Synty/Exports/Human-Custom/` — Synty export package
- `Assets/Prefabs/HumanCustomPlayer.prefab` — Synty character player prefab
- `Assets/Scripts/UI/CharacterCreatorUI.cs` — Phase C UI scaffold (not yet wired to scene)
- `Assets/Scenes/CharacterCreator.unity` — empty scene stub

### Modified Files
- `Assets/Scenes/SandboxShowcase.unity` — LevelManager PlayerPrefabs[0] → HumanCustomPlayer
- `Assets/Synty/SidekickCharacters/Database/Side_Kick_Data.db` — updated by Synty tool

---

**Entry Created:** 2026-02-17
**Status:** ✅ Complete
**Ready for Next Session:** Yes — Phase B plan written at `CosmicWiki/guides/feature-007-phase-b-plan.md`
