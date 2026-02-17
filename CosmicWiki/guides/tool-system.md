# Tool System Guide (Feature 006)

**Branch:** `features/006-tools`

## Overview

Equipable player tools that interact with the world grid. Each tool has a visual prop,
an animator controller swap, and a world effect applied via Animation Events.

## Tool Catalog

| Tool | Cosmic Name | Action | Animation | Target |
|------|------------|--------|-----------|--------|
| Axe | Plasma Cutter | Chop Xeno-Flora | HumanM@CuttingTree01 | Flora cell |
| Shovel | Mineral Extractor | Terraform grid cell | HumanM@Gathering01 | Any flat cell |
| Net | Stasis Field Generator | Catch creatures | HumanM@Gathering02 | NPC in radius |
| Fishing Rod | Plasma Seiner | Fish at Nebula Pool | HumanM@Fishing01 | Water cell |
| Watering Can | Hydration Disperser | Grow crops | HumanM@Watering | Farm cell |

## Components

| Script | Role |
|--------|------|
| `ToolData.cs` | ScriptableObject: type, animator controller, use radius |
| `ToolController.cs` | Equip/unequip, animator controller swap |
| `ToolActions.cs` | ChopAction, DigAction, CatchAction, FishAction, WaterAction |

## Equip Flow

1. Player selects tool from hotbar (1–4 keys)
2. `ToolController.EquipTool(toolData)` called
3. Animator controller swapped: `animator.runtimeAnimatorController = toolData.controller`
4. Tool prop model shown (Pandazole mesh)
5. Player presses **E** → `ToolController.UseTool()`
6. Animation plays → Animation Event fires `OnToolActionFrame()`
7. `ToolActions.ApplyEffect(targetCell)` → world effect

## Animation Event Integration

See `CosmicWiki/agents/kevin-iglesias-agent.md` for clip durations and action frames.

```csharp
// In ToolController (called by animation event):
public void OnToolActionFrame()
{
    currentToolAction.Execute(gridCursor.HoveredCell);
}
```

## Tool Effects

| Action | World Effect |
|--------|-------------|
| Chop | Remove flora occupant from cell, spawn Ferrite Core item |
| Dig | Change TerrainType, rebake NavMesh |
| Catch | Stun nearby NPC, start mini-game |
| Fish | Start fishing mini-game at Nebula Pool cell |
| Water | Advance crop growth stage |

## Assets Used

- **Kevin Iglesias Human Animations** — `Assets/Kevin Iglesias/Human Animations/`
- **Pandazole Survival Crafting Pack** — tool prop models
- **SineVFX Crystals** — harvest VFX effect
- **TDE CharacterHandleWeapon** — tool-hold system reuse

## Status

- [ ] ToolData.cs ScriptableObject
- [ ] ToolController.cs equip + animator swap
- [ ] ToolActions.cs effect implementations
- [ ] Animator controllers per tool (5 controllers)
- [ ] Tool prop prefabs (Pandazole models)
- [ ] Animation events on Kevin Iglesias clips
