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

### 📋 Phase B: In-Game Part Picker (Next)

Replace the static color-swatch `CharacterCreatorUI.cs` with a real part picker
driven by the Synty part library — letting players pick head, body, legs, hair.

**Full plan:** `CosmicWiki/guides/feature-007-phase-b-plan.md`

**Start checklist:**
1. Read `Assets/Synty/SidekickCharacters/Scripts/` — find runtime API
2. Glob `HumanSpecies_01` parts directory — inventory available meshes
3. Build `SyntyPartSwapper.cs` (or wrap Synty API)
4. Build `CharacterCreatorController.cs` + UI
5. Wire save/load to `PlayerCharacterData`

### 📋 Phase C: Character Creator Scene (Full UI)

1. Set up `Assets/Scenes/CharacterCreator.unity` (camera, lights, UI canvas)
2. Part picker panels (head, upper body, lower body, hair, accessories)
3. Color pickers — Skin Tone, Hair, Outfit Primary, Outfit Secondary
4. Name input (max 12 chars)
5. Species selector (HumanSpecies01–04, Starter01–04)
6. [Begin Adventure] → `PlayerCharacterData.Save()` → `LoadScene("SandboxShowcase")`

### 🔮 Phase D: In-World Wardrobe (Future)

1. Depends on Feature 005 (Nano-Fabricator building)
2. Interact (E) → opens CharacterCreator in "edit mode"
3. Edit mode: color/parts only — no name change
4. Apply → update live player via `CharacterCustomizer` + `SyntyPartSwapper`

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

## New Scripts

| Script | Path |
|--------|------|
| `CharacterCreatorUI.cs` | `Assets/Scripts/UI/CharacterCreatorUI.cs` |
| `PlayerCharacterData.cs` | `Assets/Scripts/Character/PlayerCharacterData.cs` |
| `CharacterCustomizer.cs` | `Assets/Scripts/Character/CharacterCustomizer.cs` |

## Status

- [x] Phase A: SidekickPlayer.prefab model swap
- [ ] Phase B: animation retarget verification
- [ ] Phase C: CharacterCreator scene + UI
- [ ] Phase D: in-world wardrobe (depends on Feature 005)
- [ ] All CharacterAbility regression tests pass
