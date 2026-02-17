# Character Creator

**Feature:** 007-character-creator | **Branch:** `features/007-character-creator`

## Overview

The Character Creator launches at game start and allows players to design their colonist.
The Synty SidekickCharacter system provides species variants with shader-driven color customization.

## Species Options

| Index | Species ID | Name | Base | Asset Path |
|-------|-----------|------|------|-----------|
| 0 | HumanSpecies01 | Human Type A | Slim | `Assets/Synty/SidekickCharacters/HumanSpecies/01/` |
| 1 | HumanSpecies02 | Human Type B | Medium | `Assets/Synty/SidekickCharacters/HumanSpecies/02/` |
| 2 | HumanSpecies03 | Human Type C | Athletic | `Assets/Synty/SidekickCharacters/HumanSpecies/03/` |
| 3 | HumanSpecies04 | Human Type D | Stocky | `Assets/Synty/SidekickCharacters/HumanSpecies/04/` |
| 4 | Starter01 | Alien Type A | Slim | `Assets/Synty/SidekickCharacters/Starter/01/` |
| 5 | Starter02 | Alien Type B | Medium | `Assets/Synty/SidekickCharacters/Starter/02/` |
| 6 | Starter03 | Alien Type C | Tall | `Assets/Synty/SidekickCharacters/Starter/03/` |
| 7 | Starter04 | Alien Type D | Compact | `Assets/Synty/SidekickCharacters/Starter/04/` |

## Customization Options

| Option | Type | Values | Shader Property |
|--------|------|--------|----------------|
| Skin Tone | Color Picker | Any RGB | `_Color_Skin` |
| Hair Color | Color Picker | Any RGB | `_Color_Hair` |
| Outfit Primary | Color Picker | Any RGB | `_Color_Primary` |
| Outfit Secondary | Color Picker | Any RGB | `_Color_Secondary` |

## Creator Scene Flow

```
Game Start
  → CharacterCreator.unity loads
  → Player chooses species (carousel)
  → Player adjusts colors (live preview)
  → Player enters name (max 12 chars)
  → [Begin Adventure] → saves to PlayerPrefs → loads SandboxShowcase.unity
```

## In-World Wardrobe

Available after building the Nano-Fabricator (Feature 005 dependency):
- Interact with Nano-Fabricator → CharacterCreator in "edit mode"
- Edit mode: color pickers only (species and name locked after creation)
- Changes apply immediately to active player character

## Scripts

| Script | Role |
|--------|------|
| `CharacterCreatorUI.cs` | Creator scene controller |
| `PlayerCharacterData.cs` | ScriptableObject / PlayerPrefs persistence |
| `CharacterCustomizer.cs` | Runtime color application |

## Related

- `CosmicWiki/agents/synty-sidekick-agent.md` — Asset paths, shader details, DB schema
- `CosmicWiki/guides/synty-sidekick-integration.md` — Full integration guide
- `Assets/Synty/SidekickCharacters/Database/Side_Kick_Data.db`
