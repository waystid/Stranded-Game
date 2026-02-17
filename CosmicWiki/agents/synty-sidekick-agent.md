# Synty SidekickCharacters Agent

> **🤖 Specialized Agent for Synty SidekickCharacter Integration**
>
> Handles character mesh swapping, SQLite database queries, shader color system, and rig setup for Feature 007.

---

## Overview

The **Synty Sidekick Agent** integrates Synty SidekickCharacters into Cosmic Colony as the
player character, replacing the AstronautPlayer placeholder. It manages the full pipeline:
species selection → mesh swap → shader color → animator retarget → TDE ability preservation.

---

## Asset Paths

```
Assets/Synty/SidekickCharacters/
  HumanSpecies/
    01/  ← HumanSpecies01 prefabs + meshes
    02/
    03/
    04/
  Starter/
    01/  ← Starter01 prefabs + meshes
    02/
    03/
    04/
  Database/
    Side_Kick_Data.db      ← SQLite character database
  Resources/
    Shaders/               ← Synty color mask shaders
  _Demos/
    Prefabs/
      SK_FacialDemoCharacter.prefab  ← Demo/reference prefab
```

---

## SQLite Database Schema

The `Side_Kick_Data.db` file contains character part metadata.

**Querying (bash, for reference only):**
```bash
sqlite3 Assets/Synty/SidekickCharacters/Database/Side_Kick_Data.db ".tables"
sqlite3 Assets/Synty/SidekickCharacters/Database/Side_Kick_Data.db "SELECT * FROM Characters LIMIT 5;"
```

**Key tables to query:**
- `Characters` — character variant list with species IDs
- `Parts` — mesh/material part catalog per character
- `ColorSets` — default color presets per species

---

## Shader Color System

Synty uses a **color mask shader** with up to 4 independent color channels per material.

**Shader property names:**
```csharp
material.SetColor("_Color_Skin", skinColor);      // Channel 1: skin tone
material.SetColor("_Color_Hair", hairColor);       // Channel 2: hair color
material.SetColor("_Color_Primary", primaryColor); // Channel 3: outfit primary
material.SetColor("_Color_Secondary", secondaryColor); // Channel 4: outfit secondary
```

**Note:** Property names may vary by shader variant — verify in Shader Graph or Inspector.
Common alternates: `_Color1`, `_Color2`, `_Color3`, `_Color4`

**Runtime application:**
```csharp
// Apply to all renderers on character
foreach (var renderer in character.GetComponentsInChildren<SkinnedMeshRenderer>())
{
    renderer.sharedMaterial.SetColor("_Color_Skin", data.skinColor);
    renderer.sharedMaterial.SetColor("_Color_Hair", data.hairColor);
    renderer.sharedMaterial.SetColor("_Color_Primary", data.primaryColor);
}
```

**PlayerPrefs storage keys:**
```
Character_Species        (int: 0-7, maps to HumanSpecies01-04, Starter01-04)
Character_Color_Skin_R/G/B
Character_Color_Hair_R/G/B
Character_Color_Primary_R/G/B
Character_Color_Secondary_R/G/B
Character_Name           (string)
```

---

## Mesh Swap Pattern

```csharp
// 1. Find the SkinnedMeshRenderer on the current player
var smr = playerGO.GetComponentInChildren<SkinnedMeshRenderer>();

// 2. Load target species prefab
var speciesPrefab = Resources.Load<GameObject>("Synty/SidekickCharacters/HumanSpecies/01/...");

// 3. Swap mesh + materials
smr.sharedMesh = speciesPrefab.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
smr.sharedMaterials = speciesPrefab.GetComponentInChildren<SkinnedMeshRenderer>().sharedMaterials;

// 4. Update Animator avatar
var animator = playerGO.GetComponent<Animator>();
animator.avatar = speciesPrefab.GetComponent<Animator>().avatar;
```

---

## TDE Ability Preservation Checklist

When swapping from AstronautPlayer to SidekickPlayer, verify these abilities:
- [ ] `CharacterMovement` — walk/run speeds
- [ ] `CharacterRun` — run trigger
- [ ] `CharacterJump` — jump height/count
- [ ] `CharacterDash` — dash distance
- [ ] `CharacterButtonActivation` — interaction (E key)
- [ ] `CharacterInventory` — inventory reference
- [ ] `CharacterHandleWeapon` — tool hold slot

Reference prefab: `Assets/Prefabs/AstronautPlayer.prefab`

---

## Implementation Phases (Feature 007)

| Phase | Task | Script |
|-------|------|--------|
| A | Model swap: replace AstronautPlayer mesh | `CharacterCustomizer.cs` |
| B | Animation retarget to Sidekick rig | Animator Controller update |
| C | Character Creator UI scene | `CharacterCreatorUI.cs` |
| D | In-world wardrobe (Nano-Fabricator) | `BuildingInterior.cs` integration |

---

## Related Files

- `Assets/Scripts/Character/PlayerCharacterData.cs` — ScriptableObject for character data
- `Assets/Scripts/Character/CharacterCustomizer.cs` — Runtime color application
- `Assets/Scripts/UI/CharacterCreatorUI.cs` — Creator scene controller
- `CosmicWiki/guides/synty-sidekick-integration.md` — Full integration guide
- `CosmicWiki/pages/character/character-creator.md` — Wiki page for character system
