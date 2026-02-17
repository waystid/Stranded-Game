# Synty SidekickCharacter Integration Guide (Feature 007)

**Branch:** `features/007-character-creator`

## Overview

Replace AstronautPlayer placeholder with a Synty SidekickCharacter, add a game-start
character creator scene, and in-world wardrobe via Nano-Fabricator.

See `CosmicWiki/agents/synty-sidekick-agent.md` for asset paths, shader properties, and DB schema.

## Implementation Phases

### ✅ Phase A: SidekickPlayer + Synty Mesh (Complete)

1. ✅ Duplicated `AstronautPlayer.prefab` → `SidekickPlayer.prefab`
2. ✅ Added `HumanSpecies_01.prefab` as nested `SyntyMesh` under `SuitModel`
3. ✅ Updated `Animator.avatar` → `HumanSpecies_01-avatar.asset`
4. ✅ Added `CharacterCustomizer.cs` for runtime shader color application
5. ✅ Verified all TDE `CharacterAbilities` intact

### ✅ Phase A+: Animation Retarget + HumanCustomPlayer (Complete)

1. ✅ Confirmed Synty Sidekick exports use Humanoid avatars — Mecanim retargeting works with zero config
2. ✅ Exported randomized Synty character → `Assets/Synty/Exports/Human-Custom/`
3. ✅ Created `HumanCustomPlayer.prefab` (SidekickPlayer stack + Human-Custom mesh)
4. ✅ SandboxShowcase now spawns `HumanCustomPlayer`
5. ✅ Fixed avatarRoot bug (deleted SyntyMesh instead of disabling — see agent notes)

**Key file:** `Assets/Prefabs/HumanCustomPlayer.prefab`
**Devlog:** `CosmicWiki/devlog/entries/2026-02-17-synty-character-integration.md`

### ✅ Phase B: In-Game Part Picker (Complete)

Replaced `CharacterCreatorUI.cs` with `CharacterCreatorController.cs` driven by
`SidekickRuntime` + preset groups. Head / UpperBody / LowerBody prev-next pickers,
4-channel color swatches, name input, Begin Adventure button.

**Full plan + session notes:** `CosmicWiki/guides/feature-007-phase-b-plan.md`

**Key files:**
- `Assets/Scripts/UI/CharacterCreatorController.cs` — Phase B controller
- `Assets/Scenes/CharacterCreator.unity` — built from scratch (Camera, Canvas, PreviewRoot)

### ✅ Phase C: SandboxShowcase Save/Load Integration (Complete)

1. ✅ `CharacterCreator` + `SandboxShowcase` added to Build Settings (indices 44/45)
2. ✅ `PlayerNameDisplay.cs` — reads saved name from PlayerPrefs, shows in HUD
3. ✅ Colors wired: `CharacterCustomizer` on `HumanCustomPlayer` loads PlayerPrefs at `Start()`
4. ✅ Scene flow: CharacterCreator → Begin Adventure → SandboxShowcase confirmed

**Limitation:** `HumanCustomPlayer` uses a baked single mesh — preset part indices
(head/upper/lower) don't change the mesh shape in-game. Fixed in Phase D.

### 📋 Phase D: In-Game SidekickRuntime Visual Loader (Next)

Replace baked `HumanCustomMesh` at runtime via `SidekickRuntime.CreateCharacter()`.
Player spawns with the exact preset parts chosen in the character creator.

**Full plan + code skeleton:** `CosmicWiki/guides/feature-007-phase-d-plan.md`

**New files:**
- `Assets/Scripts/Character/SyntyCharacterLoader.cs` — Awake() visual swap
- Modify `Assets/Prefabs/HumanCustomPlayer.prefab` — add component

### 🔮 Phase E: In-World Wardrobe (Future)

1. Depends on Feature 005 (Nano-Fabricator building)
2. Interact (E) → opens CharacterCreator in "edit mode"
3. Apply → hot-swap preset on live player via `SyntyCharacterLoader`

## Color System

```csharp
// CharacterCustomizer.cs
void ApplyColors(PlayerCharacterData data)
{
    foreach (var r in GetComponentsInChildren<SkinnedMeshRenderer>())
    {
        var block = new MaterialPropertyBlock();
        r.GetPropertyBlock(block);
        block.SetColor("_Color_Skin", data.skinColor);
        block.SetColor("_Color_Hair", data.hairColor);
        block.SetColor("_Color_Primary", data.primaryColor);
        r.SetPropertyBlock(block);
    }
}
```

## PlayerPrefs Keys

```
Character_Species          (int 0–7)
Character_Name             (string)
Character_Color_Skin_R     (float)
Character_Color_Skin_G     (float)
Character_Color_Skin_B     (float)
Character_Color_Hair_R     (float)
Character_Color_Hair_G     (float)
Character_Color_Hair_B     (float)
Character_Color_Primary_R  (float)
Character_Color_Primary_G  (float)
Character_Color_Primary_B  (float)
```

## Scripts

| Script | Path | Phase |
|--------|------|-------|
| `CharacterCustomizer.cs` | `Assets/Scripts/Character/CharacterCustomizer.cs` | A |
| `PlayerCharacterData.cs` | `Assets/Scripts/Character/PlayerCharacterData.cs` | A/B/C |
| `CharacterCreatorUI.cs` | `Assets/Scripts/UI/CharacterCreatorUI.cs` | A (legacy, superseded by B) |
| `CharacterCreatorController.cs` | `Assets/Scripts/UI/CharacterCreatorController.cs` | B |
| `PlayerNameDisplay.cs` | `Assets/Scripts/UI/PlayerNameDisplay.cs` | C |
| `SyntyCharacterLoader.cs` | `Assets/Scripts/Character/SyntyCharacterLoader.cs` | D (planned) |

## Status

- [x] Phase A: SidekickPlayer.prefab + CharacterCustomizer
- [x] Phase A+: HumanCustomPlayer + animation retarget confirmed
- [x] Phase B: CharacterCreator scene + SidekickRuntime part picker
- [x] Phase C: Save/load wired to SandboxShowcase, name display in HUD
- [ ] Phase D: SyntyCharacterLoader — runtime preset mesh swap in-game
- [ ] Phase E: In-world wardrobe (depends on Feature 005)
