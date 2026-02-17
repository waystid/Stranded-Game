# Synty SidekickCharacter Integration Guide (Feature 007)

**Branch:** `features/007-character-creator`

## Overview

Replace AstronautPlayer placeholder with a Synty SidekickCharacter, add a game-start
character creator scene, and in-world wardrobe via Nano-Fabricator.

See `CosmicWiki/agents/synty-sidekick-agent.md` for asset paths, shader properties, and DB schema.

## Implementation Phases

### Phase A: Model Swap
1. Duplicate `Assets/Prefabs/AstronautPlayer.prefab` → `SidekickPlayer.prefab`
2. In `SidekickPlayer`: replace `SkinnedMeshRenderer` with HumanSpecies01 mesh
3. Update `Animator.avatar` to Sidekick Humanoid avatar
4. Verify all `CharacterAbilities` still present and functional
5. Test in PlayMode: walk, run, jump, interact

### Phase B: Animation Retarget
1. Mecanim auto-retargets if both FBXs use Humanoid rig — no clip copying needed
2. Wire Kevin Iglesias tool animations (Feature 006 dependency)
3. Test animation blending: idle→walk→run, jump, gather

### Phase C: Character Creator Scene
1. Create `Assets/Scenes/CharacterCreator.unity`
2. UI elements:
   - Rotating character preview (RenderTexture → RawImage)
   - Species selector: HumanSpecies01–04, Starter01–04 (8 options)
   - Color pickers: Skin Tone, Hair Color, Outfit Primary, Outfit Secondary
   - Name input field (max 12 chars)
   - [Begin Adventure] button
3. Confirm → save to PlayerPrefs → `SceneManager.LoadScene("SandboxShowcase")`

### Phase D: In-World Wardrobe
1. Depends on Feature 005 (Nano-Fabricator building)
2. Interact (E) → loads CharacterCreator scene in "edit mode"
3. Edit mode: color pickers only — no species or name change
4. Apply → update `CharacterCustomizer` on active player

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
