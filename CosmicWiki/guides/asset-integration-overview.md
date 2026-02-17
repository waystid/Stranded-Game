# Asset Integration Overview

Quick reference for all asset packs in the project and which features use them.

## Asset Pack → Feature Map

| Asset Pack | Feature(s) | Primary Use |
|-----------|-----------|-------------|
| Pandazole Nature Environment | 001-world | Flora, terrain tiles |
| Pandazole Kitchen Food | 002-items | Alien Berry, food items |
| Pandazole Survival Crafting | 002-items, 006-tools | Crafting materials, tool props |
| Pandazole City Town | 005-buildings | Houses, shops, civic buildings |
| Pandazole Farm Ranch | 005-buildings | Farm buildings, fences |
| SineVFX Translucent Crystals | 001-world, 002-items | Crystal nodes, Energy Crystal |
| FarlandSkies | all (skybox) | Day/night sky variants |
| Quirky Series Animals | 004-villagers | NPC character models |
| Kevin Iglesias Human Animations | 006-tools, 007-character | Tool + character animations |
| Synty SidekickCharacters | 007-character-creator | Player character |
| Blink Animation Starter | 007-character-creator | Base locomotion clips |
| ithappy Cartoon City | 005-buildings | Additional building variety |

## Detailed Agent Files

Each major asset pack has a dedicated agent file in `CosmicWiki/agents/`:

- `pandazole-agent.md` — Full catalog of all 6 Pandazole packs
- `synty-sidekick-agent.md` — Sidekick character integration
- `kevin-iglesias-agent.md` — Animation clip catalog + retargeting
- `tde-ai-agent.md` — TopDown Engine AI patterns for NPCs

## Common Integration Patterns

### Adding a New Prop to the Scene
1. Glob search for the prefab: `Assets/Pandazole_Ultimate_Pack/**/*.prefab`
2. `manage_gameobject create` with `prefab_path`
3. Set position using island local coordinates (see IslandGridManager)
4. Add BoxCollider sized to mesh bounds

### Adding a New Character Model
1. Check Humanoid rig in FBX Import Settings
2. Verify avatar definition covers all major bones
3. Assign Kevin Iglesias animations via Animator Controller
4. Attach TDE `Character` component + abilities
5. Test in PlayMode: walk, run, interact

### Using Pandazole Materials Efficiently
- Enable GPU Instancing on all Pandazole materials
- Use Static Batching for non-moving props (`isStatic = true`)
- Do NOT create per-instance materials — use MaterialPropertyBlock for tinting
