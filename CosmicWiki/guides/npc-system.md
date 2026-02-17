# NPC System Guide (Feature 004)

**Branch:** `features/004-villagers`

## Overview

Living villager NPCs with patrol AI, daily routines, and dialogue interaction.
Built on TDE's AIBrain system with Quirky Series character models.

## AI Architecture

See `CosmicWiki/agents/tde-ai-agent.md` for full AIBrain/AIAction/AIDecision patterns.

**State flow:**
```
Idle → [5s timer] → Wander → [detected player < 5u] → Greet → [8s timer] → Idle
                 ↑_____________[15s patrol complete]___________↑
```

## Daily Routine

| Period | Time | Behavior |
|--------|------|---------|
| Morning | 06:00–12:00 | Patrol home area |
| Afternoon | 12:00–18:00 | Visit common areas, shops |
| Evening | 18:00–22:00 | Plaza or beach area |
| Night | 22:00–06:00 | Return home, sleep (AI disabled) |

Time driven by `DayNightCycle.cs` — villagers subscribe to time change events.

## Components

| Script | Role |
|--------|------|
| `VillagerData.cs` | ScriptableObject: name, personality, home cell, species |
| `VillagerController.cs` | Daily routine scheduler |
| `DialoguePanel.cs` | Portrait + text dialogue UI |
| `VillagerManager.cs` | Registry of all active villagers |

## Villager Prefab Structure

```
Villager_[Name]
  ├── Model/                 ← Quirky Series mesh + animator
  ├── Waypoints/             ← MMPath for patrol
  ├── AIBrain                ← on root
  ├── NavMeshAgent           ← on root
  ├── CharacterButtonActivation ← E-key dialogue trigger
  └── DialogueTriggerZone    ← SphereCollider trigger r=3
```

## Dialogue System

- Trigger: player walks within 5u OR presses E near villager
- Panel shows: portrait (128×128 sprite), 2-line text, [OK] button
- Personality types affect dialogue text: Peppy, Cranky, Lazy, Normal, Snooty, Jock, Smug, Uchi
- Text stored in `VillagerData.dialogueLines[]`

## Assets Used

- **Quirky Series Ultimate** — `Assets/Quirky Series - Animals Ultimate pack Free Samples/`
- **Kevin Iglesias** — walk/gather animations for routines
- **TDE AIBrain** — `Assets/TopDownEngine/ThirdParty/MoreMountains/MMTools/Foundation/MMAI/`

## Status

- [ ] VillagerData.cs ScriptableObject
- [ ] VillagerController.cs routine scheduler
- [ ] DialoguePanel.cs UI
- [ ] VillagerManager.cs registry
- [ ] Sample villager prefab with Quirky model
- [ ] NavMesh bake from Feature 001 terrain
