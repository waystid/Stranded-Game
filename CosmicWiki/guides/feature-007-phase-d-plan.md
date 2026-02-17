# Feature 007 Phase D — In-Game SidekickRuntime Visual Loader

**Status:** ✅ Complete
**Branch:** features/007-character-creator
**Prerequisite:** Phase C complete ✅
**Session estimate:** 1 session

---

## Goal

Replace the static baked `Human-Custom` mesh on `HumanCustomPlayer` at runtime with a
`SidekickRuntime.CreateCharacter()` result built from the player's saved preset indices
(head / upper body / lower body). The player should animate correctly via the existing
`ColonelAnimator.controller` and receive colors via the existing `CharacterCustomizer`.

---

## Key Findings (from pre-session investigation)

### HumanCustomPlayer structure
```
HumanCustomPlayer (root — CharacterController, TDE, CharacterCustomizer)
└── SuitModel  (Animator: ColonelAnimator.controller + Human-Custom-avatar, localScale 1.5)
    ├── HumanCustomMesh  (localScale 0.6667 — compensates SuitModel 1.5×)
    │   └── mesh  (SkinnedMeshRenderer)
    ├── WeaponAttachmentContainer
    └── Feedbacks
```

### SidekickRuntime.CreateCharacter() output structure
```
[tempGO root]  (Animator: SK_BaseModel avatar, no controller)
└── root        (skeleton root bone — full Synty bone hierarchy)
├── SK_HUMN_HEAD_HU01   (SkinnedMeshRenderer — flat child)
├── SK_HUMN_TORSO_HU01  (SkinnedMeshRenderer — flat child)
└── ...                 (all part SMRs as flat children)
```

Source: `Assets/Synty/SidekickCharacters/Scripts/Runtime/API/SidekickRuntime.cs`
(lines 241–302 — CreateCharacter spawns root + copies Animator from _baseModel)

### Avatar compatibility — CONFIRMED COMPATIBLE
- `Human-Custom-avatar.asset` and `SK_BaseModel` share **identical bone names and structure**
- Human-Custom is an export derivative of the same Synty rig
- Swapping SuitModel's Animator.avatar to the SK_BaseModel avatar is safe
- ColonelAnimator.controller is a standard humanoid controller — works with either avatar

### Scale
- SuitModel localScale = **1.5**
- HumanCustomMesh localScale = **0.6667** (= 1 ÷ 1.5) → net world scale 1.0
- SidekickRuntime output is sized for world scale **1.0**
- **Fix:** place everything in a `SidekickMesh` container at localScale 0.6667 under SuitModel

### Relevant paths
| Asset | Path |
|-------|------|
| Animation controller | `Assets/TopDownEngine/Demos/Colonel/Animations/ColonelAnimator.controller` |
| Human-Custom avatar | `Assets/Human-Custom/Meshes/Human-Custom-avatar.asset` |
| SK_BaseModel prefab | `Assets/Synty/SidekickCharacters/Resources/Meshes/SK_BaseModel.fbx` |
| HumanCustomPlayer | `Assets/Prefabs/HumanCustomPlayer.prefab` |

---

## Architecture

### New component: `SyntyCharacterLoader.cs`

- **Attach to:** `HumanCustomPlayer` root (alongside `CharacterCustomizer`)
- **Runs in:** `Awake()` — fires BEFORE TDE's `CharacterAnimator.Start()` initialises
- **Result:** visual swap is invisible to TDE; all gameplay components work unchanged

---

## Swap Algorithm

```
Awake()
│
├─ No saved data? → return (HumanCustomMesh stays as fallback)
│
├─ Load PlayerCharacterData (preset indices + colors)
│
├─ InitSidekickRuntime()
│   ├─ new DatabaseManager()
│   ├─ Resources.Load SK_BaseModel + M_BaseMaterial
│   └─ new SidekickRuntime(baseModel, baseMat, null, db)
│       SidekickRuntime.PopulateToolData(runtime)
│
├─ Collect parts from saved preset indices
│   (same CollectPresetParts() logic as CharacterCreatorController)
│
├─ tempGO = runtime.CreateCharacter("_SidekickTemp", parts, false, true)
│   Position tempGO at (9999,9999,9999) to keep it off-screen during swap
│
├─ Find SuitModel + SuitModel.GetComponent<Animator>()
│
├─ Extract from tempGO:
│   ├─ syntyAvatar  ← tempGO.GetComponent<Animator>().avatar
│   ├─ skeletonRoot ← tempGO.transform.Find("root")
│   └─ smrs[]       ← tempGO.GetComponentsInChildren<SkinnedMeshRenderer>(true)
│
├─ Destroy HumanCustomMesh (suitModel.Find("HumanCustomMesh"))
│
├─ Create container:
│   var container = new GameObject("SidekickMesh")
│   container.SetParent(suitModel, worldPositionStays: false)
│   container.localScale = Vector3.one * (1f / 1.5f)   // = 0.6667
│
├─ Reparent into container (worldPositionStays: false):
│   ├─ skeletonRoot.SetParent(container, false)
│   └─ foreach smr → smr.transform.SetParent(container, false)
│
├─ Update Animator:
│   ├─ animator.avatar = syntyAvatar
│   ├─ animator.Rebind()
│   └─ animator.Update(0f)   // force bind pass
│
└─ Destroy(tempGO)           // now empty — skeleton + SMRs were reparented out
```

After `Awake()` returns:
- `CharacterCustomizer.Start()` fires → `GetComponentsInChildren<SkinnedMeshRenderer>()` finds the new SMRs → applies saved colors ✅
- TDE `CharacterAnimator.Start()` fires → finds Animator on SuitModel with rebound skeleton → drives animations normally ✅

---

## Code Skeleton

```csharp
using Synty.SidekickCharacters.API;
using Synty.SidekickCharacters.Database;
using Synty.SidekickCharacters.Database.DTO;
using Synty.SidekickCharacters.Enums;
using Synty.SidekickCharacters.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Phase D — Swaps the baked HumanCustomMesh with a SidekickRuntime-built character
/// assembled from the player's saved preset indices. Runs in Awake() before TDE Start().
/// Attach to HumanCustomPlayer root alongside CharacterCustomizer.
/// </summary>
public class SyntyCharacterLoader : MonoBehaviour
{
    void Awake()
    {
        if (!PlayerCharacterData.HasSavedData()) return;

        var data = ScriptableObject.CreateInstance<PlayerCharacterData>();
        data.Load();

        // ── Init Synty runtime ────────────────────────────────────────────────
        var db        = new DatabaseManager();
        var baseModel = Resources.Load<GameObject>("Meshes/SK_BaseModel");
        var baseMat   = Resources.Load<Material>("Materials/M_BaseMaterial");

        if (baseModel == null || baseMat == null)
        {
            Debug.LogWarning("[SyntyCharacterLoader] SK_BaseModel or M_BaseMaterial not found.");
            return;
        }

        var runtime = new SidekickRuntime(baseModel, baseMat, null, db);
        SidekickRuntime.PopulateToolData(runtime);
        var partLib = runtime.MappedPartDictionary;

        // ── Collect parts from saved preset indices ──────────────────────────
        var parts = new List<SkinnedMeshRenderer>();
        var headPresets  = SidekickPartPreset.GetAllByGroup(db, PartGroup.Head);
        var upperPresets = SidekickPartPreset.GetAllByGroup(db, PartGroup.UpperBody);
        var lowerPresets = SidekickPartPreset.GetAllByGroup(db, PartGroup.LowerBody);

        CollectPresetParts(runtime, db, partLib, headPresets,  data.headPresetIndex,      parts);
        CollectPresetParts(runtime, db, partLib, upperPresets, data.upperBodyPresetIndex,  parts);
        CollectPresetParts(runtime, db, partLib, lowerPresets, data.lowerBodyPresetIndex,  parts);

        if (parts.Count == 0)
        {
            Debug.LogWarning("[SyntyCharacterLoader] No parts resolved — aborting swap.");
            return;
        }

        // ── Spawn temp character ──────────────────────────────────────────────
        var tempGO = runtime.CreateCharacter("_SidekickTemp", parts, false, true);
        if (tempGO == null) return;
        tempGO.transform.position = new Vector3(9999f, 9999f, 9999f);

        // ── Find SuitModel + Animator ────────────────────────────────────────
        var suitModel = transform.Find("SuitModel");
        if (suitModel == null) { Destroy(tempGO); return; }
        var animator = suitModel.GetComponent<Animator>();

        // ── Extract avatar, skeleton root, SMRs before reparenting ──────────
        var tempAnim   = tempGO.GetComponent<Animator>();
        var syntyAvatar = tempAnim != null ? tempAnim.avatar : null;

        var skeletonRoot = tempGO.transform.Find("root");
        var smrs = tempGO.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        // ── Destroy old baked mesh ───────────────────────────────────────────
        var oldMesh = suitModel.Find("HumanCustomMesh");
        if (oldMesh != null) Destroy(oldMesh.gameObject);

        // ── Create scale container (matches old HumanCustomMesh 0.6667 scale) ─
        var container = new GameObject("SidekickMesh");
        container.transform.SetParent(suitModel, false);
        container.transform.localScale = Vector3.one * (1f / 1.5f); // 0.6667

        // ── Reparent skeleton + SMRs ─────────────────────────────────────────
        if (skeletonRoot != null)
            skeletonRoot.SetParent(container.transform, false);

        foreach (var smr in smrs)
            smr.transform.SetParent(container.transform, false);

        // ── Rebind Animator to new skeleton ──────────────────────────────────
        if (animator != null)
        {
            if (syntyAvatar != null) animator.avatar = syntyAvatar;
            animator.Rebind();
            animator.Update(0f);
        }

        // ── Cleanup ──────────────────────────────────────────────────────────
        Destroy(tempGO);

        Debug.Log("[SyntyCharacterLoader] Visual swap complete.");
    }

    // Identical to CollectPresetParts in CharacterCreatorController
    private void CollectPresetParts(
        SidekickRuntime runtime,
        DatabaseManager db,
        Dictionary<CharacterPartType, Dictionary<string, SidekickPart>> partLib,
        List<SidekickPartPreset> presets, int idx,
        List<SkinnedMeshRenderer> result)
    {
        if (presets == null || presets.Count == 0) return;
        idx = Mathf.Clamp(idx, 0, presets.Count - 1);
        var rows = SidekickPartPresetRow.GetAllByPreset(db, presets[idx]);
        foreach (var row in rows)
        {
            if (string.IsNullOrEmpty(row.PartName)) continue;
            try
            {
                var typeName = CharacterPartTypeUtils.GetTypeNameFromShortcode(row.PartType);
                var type     = Enum.Parse<CharacterPartType>(typeName);
                if (!partLib.ContainsKey(type))   continue;
                var dict = partLib[type];
                if (!dict.ContainsKey(row.PartName)) continue;
                var partGO = dict[row.PartName].GetPartModel();
                if (partGO == null) continue;
                var smr = partGO.GetComponentInChildren<SkinnedMeshRenderer>();
                if (smr != null) result.Add(smr);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SyntyCharacterLoader] Skipping '{row.PartName}': {e.Message}");
            }
        }
    }
}
```

---

## Risks & Mitigations

| Risk | Mitigation |
|------|-----------|
| SMR bone refs broken after reparenting | Skeleton + SMRs moved into same container — Transform refs remain valid |
| Animator.Rebind() resets animation state | Runs in Awake() before TDE Start() — no state to reset |
| ColonelAnimator expects specific bones | Use SK_BaseModel avatar (see Post-Implementation Notes) |
| Scale mismatch (character too big/small) | Container at exactly 0.6667 mirrors old HumanCustomMesh scale |
| No saved data / DB failure | Guard clauses return early → HumanCustomMesh stays as fallback |
| SidekickRuntime too slow for Awake | DB load is synchronous SQLite — fast enough (< 1 frame) |
| tempGO SMRs already under `root` child | `GetComponentsInChildren()` captures all SMRs before any reparenting occurs |

---

## File Plan

| File | Action |
|------|--------|
| `Assets/Scripts/Character/SyntyCharacterLoader.cs` | **Create** |
| `Assets/Prefabs/HumanCustomPlayer.prefab` | **Add** `SyntyCharacterLoader` component to root |

---

## Session Checklist

- [x] Create `SyntyCharacterLoader.cs` (code skeleton above)
- [x] `validate_script` — confirm 0 compile errors
- [x] Add `SyntyCharacterLoader` component to `HumanCustomPlayer.prefab` root
- [x] Open `SandboxShowcase`, enter Play mode
- [x] Screenshot: character appears with correct preset parts
- [x] Confirm animation plays (idle, walk)
- [x] Confirm colors applied (CharacterCustomizer fires after Awake)
- [x] Confirm scale correct (same apparent height as before)
- [x] Check console — 0 errors (`[SyntyCharacterLoader] Visual swap complete.`)
- [x] Commit + push

---

## Future (Phase E)

- Expose a preset-change API on `SyntyCharacterLoader` for mid-game costume changes
- Support species switching (HumanSpecies02–04, Starter variants)
- Add accessory slots (beard, glasses, hat)

---

---

## Post-Implementation Notes (Animation Fix)

### T-pose bug — two root causes

**Bug 1: Avatar bone name mismatch**

The pre-session plan stated `Human-Custom-avatar` and `SK_BaseModel` share identical bone names. This was **incorrect**. In practice, `Rebind()` with `Human-Custom-avatar` produced a silent T-pose because the avatar's bone mapping did not match the raw Synty `SK_BaseModel` skeleton names.

**Fix:** Load the SK_BaseModel avatar directly from the prefab asset:
```csharp
var skBaseAvatar = baseModel.GetComponent<Animator>()?.avatar;
// ...
if (skBaseAvatar != null) animator.avatar = skBaseAvatar;
animator.Rebind();
```
The SK_BaseModel prefab asset's avatar exactly describes the Synty bone hierarchy. The avatar is a `ScriptableObject` (asset reference), so it survives `Destroy(tempGO)` safely.

**Bug 2: Deferred `Destroy()`**

`Destroy(oldMesh.gameObject)` is deferred to end-of-frame in Unity. During `Rebind()`, the old `HumanCustomMesh` skeleton was still present, causing a binding conflict.

**Fix:** Use `DestroyImmediate(oldMesh.gameObject)` so the old skeleton is gone before `Rebind()` runs.

### Diagnostic log after fix
```
[SyntyCharacterLoader] Rebind complete. avatar=SK_BaseModelAvatar, isHuman=True
[SyntyCharacterLoader] Visual swap complete.
```

---

**Plan created:** 2026-02-17
**Animation fix:** 2026-02-17
**Complexity:** Medium (research done, algorithm clear, 1 new script + 1 prefab change)
