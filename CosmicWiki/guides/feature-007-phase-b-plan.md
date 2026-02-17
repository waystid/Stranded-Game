# Feature 007 Phase B — In-Game Synty Character Customizer

**Status:** 🚧 In Progress (Session 1 complete)
**Branch:** features/007-character-creator
**Prerequisite:** Phase A complete (SidekickPlayer + CharacterCustomizer ✅)

---

## Overview

Replace the static color-swatch `CharacterCreatorUI` with a full in-game part picker that mirrors the Synty Sidekick Character Tool UI — letting players select individual body parts (head, upper body, lower body, accessories) from the Synty part library at runtime.

---

## Goals

1. **Part Selection** — Players pick head, upper body, lower body pieces from categorized lists
2. **Live Preview** — Character in the scene updates instantly as parts are swapped
3. **Color Picker** — Skin, hair, primary/secondary outfit color applied via Synty shader properties
4. **Species Switching** — Cycle between available species prefabs
5. **Persistence** — Selections saved via `PlayerCharacterData` and applied on game start

---

## Architecture Overview

```
CharacterCreator scene
  ├─ CharacterCreatorController.cs    (scene root — replaces CharacterCreatorUI.cs)
  ├─ SyntyPartDatabase.cs             (ScriptableObject — loads .sk data or static lists)
  ├─ SyntyPartPicker.cs               (UI panel — one per body category)
  └─ PreviewCharacter (GameObject)
       └─ SidekickPlayer prefab (preview instance, physics disabled)
            └─ SuitModel → SyntyMesh (CharacterCustomizer drives this)
```

---

## Phase B Steps

### Step 1 — Understand Synty Runtime API

**Investigate:**
- `Assets/Synty/SidekickCharacters/` — locate runtime part-swap scripts
- Look for `SyntyCharacter.cs`, `SidekickManager.cs`, or similar runtime part-swap API
- Check if Synty exposes `SetPart(category, index)` or similar method
- Check `Side_Kick_Data.db` — SQLite database containing all part definitions

**Key files to read:**
```
Assets/Synty/SidekickCharacters/Scripts/
Assets/Synty/SidekickCharacters/Database/
```

**Expected outcome:** Understand whether Synty has a runtime part-swap API or if we need to build one by swapping SkinnedMeshRenderer meshes manually.

---

### Step 2 — Inventory the Available Parts

**Goal:** Know what parts exist for Human species.

**Approach A (if Synty has runtime API):**
- Use the existing API to query available parts per category

**Approach B (manual inventory):**
- Glob `Assets/Synty/SidekickCharacters/Characters/HumanSpecies/**/*.prefab`
- Group by naming convention: `Head_*`, `UpperBody_*`, `LowerBody_*`, `Hair_*`, `Beard_*`, `FaceAccessory_*`
- Build a `SyntyPartDatabase` ScriptableObject with these lists

**Data structure:**
```csharp
[System.Serializable]
public class SyntyPartEntry
{
    public string partName;
    public string category;   // Head, UpperBody, LowerBody, Hair, etc.
    public Mesh mesh;
    public int partIndex;
}
```

---

### Step 3 — Runtime Part Swap System

**Goal:** Swap a body part mesh on the live preview character without destroying/recreating the whole prefab.

**Approach:** The Synty character uses `SkinnedMeshRenderer` components for each part. Swapping a part = replacing `SkinnedMeshRenderer.sharedMesh` + `sharedMaterials` on the correct child.

```csharp
// Pseudocode for part swap
void SwapPart(string category, Mesh newMesh, Material[] newMats)
{
    var smr = GetRendererForCategory(category);
    smr.sharedMesh = newMesh;
    smr.sharedMaterials = newMats;
}
```

**Bone sharing:** Synty characters share a single skeleton. All part meshes use the same bone array. SkinnedMeshRenderer bones do NOT need to change when swapping meshes — only `sharedMesh` and `sharedMaterials`.

**Important:** Preserve `rootBone` and `bones` array when swapping — only swap mesh/materials.

---

### Step 4 — CharacterCreatorController.cs

**Replace** `CharacterCreatorUI.cs` with a new controller that:

```csharp
public class CharacterCreatorController : MonoBehaviour
{
    [Header("Preview")]
    public Transform previewRoot;
    public float previewRotationSpeed = 30f;

    [Header("Character Instance")]
    private GameObject _previewInstance;
    private CharacterCustomizer _customizer;
    private SyntyPartSwapper _partSwapper;  // new component

    [Header("UI")]
    public SyntyPartPicker headPicker;
    public SyntyPartPicker upperBodyPicker;
    public SyntyPartPicker lowerBodyPicker;
    public SyntyPartPicker hairPicker;
    public ColorPicker skinColorPicker;
    public ColorPicker hairColorPicker;
    public ColorPicker primaryColorPicker;
    public ColorPicker secondaryColorPicker;
    public InputField nameInput;

    [Header("Species")]
    public GameObject[] speciesPrefabs;
    // ...
}
```

---

### Step 5 — UI Layout

**Scene hierarchy for CharacterCreator:**

```
CharacterCreator (scene)
  ├─ Lights
  ├─ Camera
  ├─ PreviewRoot (empty, rotates in Update)
  │    └─ [spawned character instance]
  └─ UI (Canvas)
       ├─ LeftPanel
       │    ├─ SpeciesRow (prev/label/next)
       │    ├─ HeadPicker (scroll list)
       │    ├─ UpperBodyPicker
       │    └─ LowerBodyPicker
       ├─ RightPanel
       │    ├─ HairPicker
       │    ├─ SkinColorRow (8 swatches)
       │    ├─ HairColorRow
       │    ├─ PrimaryColorRow
       │    └─ SecondaryColorRow
       └─ BottomPanel
            ├─ NameInput
            └─ BeginAdventureButton
```

**UI style:** Match the Synty tool aesthetic (dark theme, part thumbnails, scroll lists).

---

### Step 6 — Color Integration

**Use existing `CharacterCustomizer.cs`** — it already sets:
- `_Color_Skin`
- `_Color_Hair`
- `_Color_Primary`
- `_Color_Secondary`

Wire color swatch buttons to call `_customizer.ApplyColors(skin, hair, primary, secondary)`.

For extended color picker: consider a Unity UI color wheel or simple HSV sliders instead of fixed swatches.

---

### Step 7 — Save / Load

**Use existing `PlayerCharacterData.cs`** (ScriptableObject + PlayerPrefs).

**Extend it** to store part selections:

```csharp
// Add to PlayerCharacterData:
public int headIndex;
public int upperBodyIndex;
public int lowerBodyIndex;
public int hairIndex;
public int beardIndex;
// etc.
```

On `OnBeginAdventure()`:
1. Save all indices + colors + name via `PlayerCharacterData.Save()`
2. Load scene

On game start in `SandboxShowcase`:
- `SidekickPlayer` reads `PlayerCharacterData` and applies saved parts + colors

---

### Step 8 — SandboxShowcase Integration

Add a `CharacterLoader.cs` to `SidekickPlayer` (or `SandboxShowcase` scene bootstrap):

```csharp
void Start()
{
    if (!PlayerCharacterData.HasSavedData()) return;
    var data = ScriptableObject.CreateInstance<PlayerCharacterData>();
    data.Load();

    var swapper = GetComponentInChildren<SyntyPartSwapper>();
    swapper.ApplySavedParts(data);

    var customizer = GetComponentInChildren<CharacterCustomizer>();
    customizer.ApplyColors(data.skinColor, data.hairColor, data.primaryColor, data.secondaryColor);
}
```

---

## Open Questions Before Starting

1. **Does Synty have a runtime part-swap API?**
   - Search `Assets/Synty/SidekickCharacters/Scripts/` for public API methods
   - If yes: use it. If no: build `SyntyPartSwapper.cs`

2. **What parts does HumanSpecies_01 support?**
   - Glob `Assets/Synty/SidekickCharacters/Characters/HumanSpecies/HumanSpecies_01/`
   - List mesh assets per body category

3. **Does the `CharacterCreator.unity` scene need a Camera and lighting setup?**
   - Yes — it's currently empty (rootCount: 0)
   - Need: Directional light, Camera, PostProcessing (optional)

4. **Do we use the exported `Human-Custom.prefab` or the original `HumanSpecies_01.prefab` as the creator preview?**
   - Recommendation: Use `HumanSpecies_01.prefab` — it has modular parts that can be individually swapped
   - `Human-Custom.prefab` is a baked single-mesh and cannot have parts swapped

---

## File Plan

| File | Action | Notes |
|------|--------|-------|
| `Assets/Scripts/Character/SyntyPartSwapper.cs` | Create | Core runtime part-swap logic |
| `Assets/Scripts/Character/SyntyPartDatabase.cs` | Create | ScriptableObject — part inventory |
| `Assets/Scripts/UI/CharacterCreatorController.cs` | Create | Replaces CharacterCreatorUI.cs |
| `Assets/Scripts/UI/SyntyPartPicker.cs` | Create | Scrollable part list UI panel |
| `Assets/Scripts/Character/PlayerCharacterData.cs` | Modify | Add partIndex fields |
| `Assets/Scenes/CharacterCreator.unity` | Build out | Add camera, lights, UI canvas |
| `Assets/Prefabs/HumanCustomPlayer.prefab` | Keep | Used in SandboxShowcase |

---

## Session Start Checklist

### Session 1 (2026-02-17) — Scripts Done ✅

1. `[x]` Read `Assets/Synty/SidekickCharacters/Scripts/` — found `SidekickRuntime` + `DatabaseManager`
2. `[x]` Glob HumanSpecies_01 — single-mesh prefab; presets come from SQLite DB content packs
3. `[x]` Decision: **use SidekickRuntime.CreateCharacter() with preset groups** (no custom SwapMesh needed)
4. `[ ]` Set up CharacterCreator scene (camera, lights, UI canvas) — **NEXT SESSION**
5. `[x]` Core system: **CharacterCreatorController.cs** (`Assets/Scripts/UI/CharacterCreatorController.cs`)
   - Uses `DatabaseManager` + `SidekickRuntime.PopulateToolData()`
   - Loads `SidekickPartPreset` lists by `PartGroup.Head/UpperBody/LowerBody`
   - Prev/next buttons per group, color swatches, name input, "Begin Adventure"
   - Live preview via `CreateCharacter()` — destroy+recreate on each change (fine in creator scene)
   - Saves `headPresetIndex`, `upperBodyPresetIndex`, `lowerBodyPresetIndex` to `PlayerCharacterData`
6. `[x]` `PlayerCharacterData.cs` extended — 3 preset index fields + KEY constants + Save/Load
7. `[x]` Build UI layout in CharacterCreator.unity (Session 2 — 2026-02-17)
   - Camera (0,1.6,-3.5), Directional Light, PreviewRoot, EventSystem
   - Canvas (1920×1080 CanvasScaler) with LeftPanel (part pickers) + RightPanel (color swatches) + BottomPanel
   - AutoWire() finds all named elements scene-wide, no inspector wiring needed
   - 32 swatch buttons (8×4), 3 preset picker rows with < label > layout8. `[x]` Test part swapping live in scene — **SESSION 2 CONFIRMED WORKING**
   - SidekickRuntime loaded DB, created character with Fantasy Knight preset
   - PreviewRoot rotation working (40°/s)
   - No runtime errors
9. `[ ]` Wire save/load to SandboxShowcase — **NEXT SESSION (Phase C)**
10. `[ ]` Polish and commit

### Key Finding: No Custom SyntyPartSwapper Needed

`SidekickRuntime.CreateCharacter()` + `SidekickPartPreset` by group is the correct pattern. The demo `RuntimeColorDemo.cs` shows exactly this. No need for `SyntyPartDatabase.cs` or `SyntyPartSwapper.cs`.

Updated File Plan:

| File | Status | Notes |
|------|--------|-------|
| `Assets/Scripts/UI/CharacterCreatorController.cs` | ✅ Done | Phase B controller, preset-based |
| `Assets/Scripts/Character/PlayerCharacterData.cs` | ✅ Done | +3 preset index fields + secondary color save |
| `Assets/Scenes/CharacterCreator.unity` | 🔲 Next | Camera, lights, UI canvas, wire buttons |
| `Assets/Prefabs/HumanCustomPlayer.prefab` | Keep | SandboxShowcase player |

---

**Plan Created:** 2026-02-17
**Session 1 Completed:** 2026-02-17
**Estimated Sessions Remaining:** 1–2
**Complexity:** Medium (API pattern is clear, remaining work is scene setup + wiring)
