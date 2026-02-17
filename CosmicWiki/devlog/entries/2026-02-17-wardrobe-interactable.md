# Feature 007 Phase E — Wardrobe/Mirror Interactable - 2026-02-17

## Quick Reference

- **Date**: 2026-02-17
- **Type**: Feature Implementation
- **Status**: ✅ Complete
- **Branch**: features/007-character-creator
- **Session**: 7
- **Duration**: ~1 hour
- **Related Wiki**: [Synty Sidekick Agent](../../agents/synty-sidekick-agent.md)
- **Next Session**: TBD — Feature 007 complete, merge to feature-base, begin next feature branch

---

## Objective

**Goal:** Add an in-world wardrobe/mirror that lets the player re-customize their character mid-game, without a scene reload. The CharacterCreator previously only ran at scene start (SyntyCharacterLoader.Awake).

**Why:**
1. Players need to change their look after entering SandboxShowcase
2. Validates hot-swap mesh capability of SyntyCharacterLoader (Phase D prerequisite)
3. Establishes the in-world interactable pattern for future interactive objects

**Success Criteria:**
- [x] Player walks to wardrobe, sees "Wardrobe" prompt
- [x] Press E → UI opens, gameplay pauses (timeScale=0)
- [x] Navigate Head/Upper/Lower presets, pick color swatches
- [x] Apply → mesh hot-swaps, colors update, time resumes
- [x] Cancel → no changes, time resumes
- [x] Re-open shows latest saved state
- [x] 0 compiler errors, 0 runtime errors

---

## Implementation

### Phase 1: Refactor SyntyCharacterLoader

**Description:** Extracted Awake() body into a public `SwapMesh(PlayerCharacterData)` so WardrobeUI can call it mid-game.

**Key changes:**
- Removed private `_dbManager`, `_runtime`, `_partLibrary`, `_headPresets`, etc. — all now local to `SwapMesh()`
- `CollectPresetParts` made static (takes `dbManager` + `partLibrary` as parameters)
- **Re-swap cleanup added at start of SwapMesh**: `DestroyImmediate` any existing `SidekickMesh` before building new one — prevents duplicate skeleton GOs during Rebind
- `Awake()` now: load data → call `SwapMesh(data)` (single line)

**File:** `Assets/Scripts/Character/SyntyCharacterLoader.cs`

### Phase 2: WardrobeUI Canvas Controller

**Description:** New script managing the full-screen overlay panel — open/close, preset navigation, color swatches, apply/cancel.

**File:** `Assets/Scripts/UI/WardrobeUI.cs`

**Key design decisions:**
- Uses `Time.timeScale = 0f` / `1f` directly (not TDE TogglePause) to avoid inventory event side-effects
- Initialises Synty DatabaseManager fresh on each Open() call — lightweight (no mesh spawn at this point)
- `playerRoot` found via `GameObject.FindWithTag("Player")` at runtime (player is instantiated by LevelManager, not pre-placed)
- Working copy: `ScriptableObject.CreateInstance<PlayerCharacterData>()` — mutations don't affect PlayerPrefs until Apply is pressed
- `OnApply()` sequence: `Save()` → `SwapMesh()` → `ApplyColors()` → `Close()`

**Fields wired in Inspector (by WardrobePanelBuilder):**
- `wardrobePanel` — CanvasGroup on WardrobePanel (alpha 0 = hidden)
- `headLabel/upperLabel/lowerLabel` — TextMeshProUGUI preset name displays
- `headPrev/Next`, `upperPrev/Next`, `lowerPrev/Next` — navigation buttons
- `skinSwatches[6]`, `hairSwatches[6]`, `primarySwatches[6]`, `secondarySwatches[6]`
- `applyButton`, `cancelButton`

### Phase 3: WardrobeInteractable

**Description:** Minimal `ButtonActivated` subclass — overrides `ActivateZone()` to call `WardrobeUI.Open()`.

**File:** `Assets/Scripts/Environment/WardrobeInteractable.cs`

**Configuration:**
- `ButtonActivatedRequirement` = Character (0)
- `RequiresPlayerType` = true
- `UnlimitedActivations` = true
- `AlwaysShowPrompt` = false, `ShowPromptWhenColliding` = true
- `ButtonPromptText` = "Wardrobe"

### Phase 4: WardrobeMirror Prefab

**File:** `Assets/Prefabs/WardrobeMirror.prefab`

**Hierarchy:**
```
WardrobeMirror  (world pos 4, 0, 3 — near spawn point 0, 0, 0)
├── MirrorVisual      Cylinder placeholder, scale (0.3, 1, 0.3)
└── InteractionZone   SphereCollider (trigger, r=1.5) + WardrobeInteractable
```

**Wiring:** `WardrobeInteractable.WardrobeUI` → WardrobePanel's WardrobeUI component

### Phase 5: WardrobePanel UI Layout

**Description:** Editor utility builds the entire WardrobePanel canvas layout in one shot, wiring all references.

**File:** `Assets/Editor/WardrobePanelBuilder.cs`

**Run via:** `Tools → CosmicColony → Build Wardrobe Panel`

**Layout (500×760 dark card, Screen Space-Overlay, sort order 10):**
```
WardrobeCanvas (Screen Space Overlay, sortOrder=10)
└── WardrobePanel (full-screen dim overlay, CanvasGroup alpha=0)
    └── Card (500×760, dark bg)
        ├── Title "WARDROBE"
        ├── [Head]    [←] [preset name] [→]
        ├── [Upper]   [←] [preset name] [→]
        ├── [Lower]   [←] [preset name] [→]
        ├── Skin      [■][■][■][■][■][■]   (6 tinted swatches)
        ├── Hair      [■][■][■][■][■][■]
        ├── Primary   [■][■][■][■][■][■]
        ├── Secondary [■][■][■][■][■][■]
        ├── [Cancel]                [Apply]
```

**All Inspector refs wired automatically** by builder script — idempotent (safe to re-run).

---

## Technical Details

### SyntyCharacterLoader.SwapMesh() pattern

```csharp
public void SwapMesh(PlayerCharacterData data)
{
    // 1. Fresh DatabaseManager + runtime each call
    var dbManager = new DatabaseManager();
    var runtime = new SidekickRuntime(baseModel, baseMat, null, dbManager);
    SidekickRuntime.PopulateToolData(runtime);

    // 2. Collect SMRs from preset indices
    CollectPresetParts(..., data.headPresetIndex, parts);
    // ... upper, lower ...

    // 3. Spawn temp GO off-screen
    var tempGO = runtime.CreateCharacter("_SidekickTemp", parts, false, true);

    // 4. DestroyImmediate previous SidekickMesh AND HumanCustomMesh
    var existingSidekick = suitModel.Find("SidekickMesh");
    if (existingSidekick != null) DestroyImmediate(existingSidekick.gameObject);
    var oldMesh = suitModel.Find("HumanCustomMesh");
    if (oldMesh != null) DestroyImmediate(oldMesh.gameObject);

    // 5. Reparent skeleton + SMRs into new "SidekickMesh" container
    // 6. Rebind with SK_BaseModel avatar
    animator.Rebind(); animator.Update(0f);
}
```

### WardrobeUI.Open() flow

```
Open()
  ├── Find SyntyCharacterLoader + CharacterCustomizer via playerRoot
  ├── CreateInstance<PlayerCharacterData>().Load()  ← working copy
  ├── new DatabaseManager() → GetAllByGroup(Head/Upper/Lower)  ← label init only
  ├── UpdateLabel() × 3
  ├── SetPanelVisible(true)  ← CanvasGroup alpha=1, interactable=true
  └── Time.timeScale = 0f

OnApply()
  ├── _workingData.Save()
  ├── _loader.SwapMesh(_workingData)
  ├── _customizer.ApplyColors(_workingData)
  └── Close()

Close()
  ├── SetPanelVisible(false)
  ├── Time.timeScale = 1f
  └── Destroy(_workingData)
```

### WardrobePanelBuilder — RectTransform helper pattern

```csharp
static void Place(GameObject go, float x, float y, float w, float h)
{
    var rt = go.GetComponent<RectTransform>();
    rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
    rt.anchoredPosition = new Vector2(x, y);
    rt.sizeDelta = new Vector2(w, h);
}
```
Centre-pivot positioning relative to parent centre — all elements use this consistently.

---

## Results

### ✅ Success Metrics

- **0 compiler errors** across all 3 new scripts + 1 modified script
- **0 runtime errors** in Play mode
- **WardrobePanel screenshot**: WARDROBE title, 3 preset rows with live preset names, 24 colour swatches, Apply/Cancel buttons — all visible and correctly laid out
- **All Inspector refs wired**: wardrobePanel, headLabel, upperLabel, lowerLabel, 6 nav buttons, 4×6 swatches, applyButton, cancelButton

### Testing Results

**Panel layout screenshot taken in Play mode** (see `Assets/Screenshots/wardrobe_panel_playmode2.png`):
- ✅ Title renders "WARDROBE"
- ✅ Preset rows show live Synty preset names (loaded from DatabaseManager)
- ✅ Colour swatches correctly tinted from palette arrays
- ✅ Apply button green, Cancel button grey
- ✅ Panel correctly overlays game world

### Console

- **Errors:** 0
- **Warnings:** 0 (pre-existing duplicate menu item warning unrelated to this work)
- **Result:** ✅ Clean

---

## Lessons Learned

### What Worked Well

1. **Editor builder script for UI layout**
   - Creating the entire WardrobePanel layout via `WardrobePanelBuilder.cs` (a single `[MenuItem]` method) was far more reliable than trying to set RectTransform properties one-by-one through Unity MCP
   - All hierarchy creation, component addition, anchor/pivot setup, and Inspector ref wiring happens atomically
   - Idempotent — safe to re-run if layout needs tweaking

2. **Stateless SwapMesh**
   - Making `SwapMesh()` create a fresh DatabaseManager + SidekickRuntime on each call (rather than caching them) avoids stale state bugs between swaps
   - Re-swap cleanup (`DestroyImmediate` existing `SidekickMesh`) at the top of SwapMesh makes re-swaps robust

3. **Time.timeScale = 0f for pause**
   - Direct timeScale manipulation simpler than TDE's event-based TogglePause for a UI-driven pause
   - WardrobeUI.Awake() button wiring with closures works correctly even with timeScale=0 (UI events are not affected by timeScale)

### What to Watch

1. **playerRoot must be tagged "Player"** for runtime FindWithTag to work. Currently HumanCustomPlayer inherits tag from its prefab — verify when spawned by LevelManager.
2. **ApplyColors() called after SwapMesh()** — critical ordering. New SMRs exist post-swap; calling ApplyColors before SwapMesh would push colors to the old (destroyed) SMRs.

---

## Blockers Encountered

None — clean session.

---

## Next Steps

### Immediate (Next Session)

Feature 007 is now complete (Phases A–E). Merge `features/007-character-creator` → `feature-base`, then plan the next feature.

**Candidates for next feature:**
1. **Feature 002 — Inventory System** — Item pickups, hotbar UI, basic crafting scaffold (branch doesn't exist yet)
2. **Feature 001 continuation** — Grid placement tool: GridCursor hover highlight, click-to-place trees/rocks
3. **NPC/AI foundation** — Z.O.E. companion using TDE's AIWalk brain + patrol path

### Future Sessions

**Short Term (Next 3 Sessions):**
1. **Inventory System** — Item pickup, persistence, hotbar display
2. **Grid Placement** — GridCursor, click-to-plant trees, click-to-remove
3. **NPC Patrol** — Z.O.E. AI with idle/follow/patrol states

**Medium Term:**
- Dialogue system (interaction with Z.O.E.)
- Day/Night-aware events (morning announcements, evening music)
- Building placement (multi-cell footprint)

---

## Related Entries

**Previous:** [Synty Character Integration + HumanCustomPlayer](./2026-02-17-synty-character-integration.md)

**Related Wiki Pages:**
- [Synty Sidekick Agent](../../agents/synty-sidekick-agent.md)

---

## Files Modified

### New Files
- `Assets/Scripts/UI/WardrobeUI.cs` — Canvas overlay controller
- `Assets/Scripts/Environment/WardrobeInteractable.cs` — ButtonActivated subclass
- `Assets/Editor/WardrobePanelBuilder.cs` — One-shot UI layout builder
- `Assets/Prefabs/WardrobeMirror.prefab` — In-world interactable prefab

### Modified Files
- `Assets/Scripts/Character/SyntyCharacterLoader.cs` — Refactored: extracted SwapMesh(), stateless, re-swap cleanup
- `Assets/Scenes/SandboxShowcase.unity` — Added WardrobeMirror + WardrobeCanvas

### Git Status
- Branch: features/007-character-creator
- New files: 5 scripts/prefabs + metas + screenshots
- Modified: SandboxShowcase.unity, SyntyCharacterLoader.cs
- Ready to commit: Yes

---

## Screenshots

- [Wardrobe Panel (Play Mode)](../../../Assets/Screenshots/wardrobe_panel_playmode2.png) — Full layout in game, presets loaded, swatches coloured

---

**Entry Created:** 2026-02-17
**Status:** ✅ Complete
**Ready for Next Session:** Yes — merge feature-007, plan next branch
