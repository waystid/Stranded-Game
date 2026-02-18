# Next Session: Plan 3-Team Agentic Sprint

**Date:** 2026-02-18 (or next session)
**Current Branch:** `core`
**Session Type:** Planning + Wiki/Devlog Setup

---

## What We Built This Session

The `core` branch is now a clean, organized dev portal for all game development work:

### CoreScene (Main Menu)
- Based on TDE StartScreen demo — copied to `Assets/Scenes/CoreScene/`
- One button: **DEMOS** → loads `LevelSelection`
- All assets self-contained in `Assets/Scenes/CoreScene/`

### LevelSelection (Dev Portal — 35 cards)
Carousel order:
```
[0]  Character Creator  → Assets/Scenes/CharacterCreator/SyntyCharCreator.unity
[1]  Stranded           → Assets/Scenes/Loft3D/StrandedDev.unity
[2]  TWC Stylized Island (Auto)
[3]  TWC Runtime Editor
[4]  TWC Cliff Island
[5]  TWC Deep Rock Crystals
[6]  TWC Mix Tilesets
[7]  TWC Pathfinding
[8]  TWC Pathfinding Follow
[9]  TWC Ramps
[10] TWC Stylized Manual
[11+] KoalaDungeon … (original TDE demos preserved)
```

### Branch Structure
```
core (base — stable dev portal)
├── 001-character-editor   ← Synty CC scene → TDE integration
├── 002-stranded           ← StrandedDev scene (Loft3D copy)
└── 003-island-biome       ← TWC island / biome work
```

### Key Files Added This Session
- `Assets/Scripts/UI/PortalButton.cs` — scene loading component
- `Assets/Scripts/Editor/DevPortalSetup.cs` — editor menu for all portal setup
- `Assets/Scenes/CoreScene/` — self-contained main menu
- `Assets/Scenes/CharacterCreator/SyntyCharCreator.unity` — Synty preset demo copy
- `Assets/Scenes/Loft3D/StrandedDev.unity` — dev sandbox (Loft3D copy)
- `Assets/Scenes/TileWorldCreator/TWC_*.unity` — 9 TWC demo copies

---

## Next Session Agenda: Plan All 3 Branches

### Session Goal
Define the concrete work plan for each branch so 3 agentic teams can execute independently, updating devlog + wiki at every step.

---

## Branch Plans (Draft — to be fleshed out next session)

### 001-character-editor
**Scene:** `Assets/Scenes/CharacterCreator/SyntyCharCreator.unity`
**Goal:** Get Synty character creator working, then export characters that animate correctly in TDE Loft3D

**Planning questions to answer:**
- Which Synty demo scene is best starting point? (RuntimePreset, RuntimeParts, RuntimeColor?)
- What does a "working" character export look like from Synty → TDE?
- What's the animation pipeline? (avatar, controller, clips)
- What does the integration test look like in StrandedDev?
- How do we document character specs in the wiki?

**Likely phases:**
1. Explore Synty CC scene — understand the tool
2. Export a character — test in isolation
3. Drop character into StrandedDev — verify TDE movement + animations
4. Document avatar/rig requirements in wiki
5. Wire up runtime color/parts customization

**Wiki pages to create/update:**
- `CosmicWiki/pages/characters/synty-character-pipeline.md`
- `CosmicWiki/agents/character-editor-agent.md`

---

### 002-stranded
**Scene:** `Assets/Scenes/Loft3D/StrandedDev.unity`
**Goal:** Evolve the Loft3D base into the Stranded game sandbox — player, movement, interactions, basic game loop

**Planning questions to answer:**
- What does the Stranded game feel like? (tone, pacing, core loop)
- What stays from Loft3D, what gets replaced?
- Player character — astronaut? Synty human? Custom?
- What systems are in scope: inventory, tools, NPCs, building?
- What's the first playable milestone?

**Likely phases:**
1. Clean the Loft3D scene — remove demo elements, set up fresh
2. Integrate player character (from 001 output or placeholder)
3. Island/world feel — environment, lighting, atmosphere
4. Core interaction loop — pick up, inspect, use
5. First playable: walk around, pick up item, basic HUD

**Wiki pages to create/update:**
- `CosmicWiki/pages/game-design/stranded-game-design.md`
- `CosmicWiki/pages/game-design/core-game-loop.md`
- `CosmicWiki/agents/stranded-dev-agent.md`

---

### 003-island-biome
**Scene:** `Assets/Scenes/TileWorldCreator/TWC_StylizedIsland.unity` (starting ref)
**Goal:** Build a procedural island world using TileWorldCreator — biomes, terrain variation, flora placement

**Planning questions to answer:**
- What biomes does Stranded have? (tropical, rocky, volcanic, arctic?)
- How does TWC procedural generation map to biome zones?
- What Pandazole/environment assets slot into each biome?
- How does the island connect to StrandedDev scene?
- What's the output format? (prefab, configuration asset, runtime gen?)

**Likely phases:**
1. Study TWC Stylized Island + Runtime Editor demos thoroughly
2. Map biome concept → TWC layer configuration
3. Build first biome prototype (lush tropical)
4. Add flora scatter (Pandazole pack)
5. Connect to StrandedDev scene grid system

**Wiki pages to create/update:**
- `CosmicWiki/pages/world/island-biome-system.md`
- `CosmicWiki/pages/world/twc-biome-configuration.md`
- `CosmicWiki/agents/island-biome-agent.md`

---

## How to Run Next Session

### 1. Start by planning each branch (in order)

```
"Let's plan 001-character-editor first. Read the branch,
SyntyCharCreator scene, and draft a phased implementation plan
with devlog entries and wiki pages defined."
```

Then:
```
"Now plan 002-stranded. Read StrandedDev scene, existing
game design notes, and draft the phased plan."
```

Then:
```
"Now plan 003-island-biome. Read the TWC Stylized Island scene,
existing island grid system, and plan the biome work."
```

### 2. For each plan, create:
- `CosmicWiki/devlog/entries/2026-02-18-{branch}-plan.md` — the session devlog
- `CosmicWiki/agents/{branch}-agent.md` — agent context file for that team
- A task checklist in the devlog tied to phases

### 3. Devlog + Wiki update workflow (per branch team)
Every agentic task should:
1. Read the branch agent file before starting
2. Do the work
3. Write a devlog entry for what was done
4. Update relevant wiki pages with findings
5. Commit devlog + wiki with the code changes

---

## Context for Planning

### Existing Wiki Structure
```
CosmicWiki/
├── agents/          ← per-team context files
├── data/            ← acnh_cosmic_mapping.json
├── devlog/
│   ├── entries/     ← session devlogs
│   └── DEVLOG_INDEX.md
├── guides/          ← integration guides
└── pages/
    ├── characters/
    ├── game-design/
    └── world/
```

### Key Asset References
- **Synty characters:** `Assets/Synty/SidekickCharacters/`
- **Pandazole pack:** `Assets/Pandazole/` (trees, rocks, flora)
- **TileWorldCreator:** `Assets/TileWorldCreator/`
- **TDE demos (reference):** `Assets/TopDownEngine/Demos/`
- **Existing scripts:** `Assets/Scripts/`

### Memory Notes
- MEMORY.md is the LLM memory file — update it with stable patterns found during planning
- Keep devlog entries focused: what was attempted, what worked, blockers, next steps
- Wiki pages should be reference material (facts, specs, APIs) not session notes

---

## Git Workflow for 3-Team Sprint

Each branch team:
```bash
git checkout 001-character-editor   # (or 002/003)
# ... do work ...
git add -p                          # stage selectively
git commit -m "feat(cc): ..."       # scoped commit message
git push origin 001-character-editor
# When phase complete → PR into core
```

PRs merge into `core`, not `main`.
`main` gets a merge from `core` only when a stable milestone is reached.

---

## Quick Reference

| Branch | Scene | First Task |
|--------|-------|-----------|
| 001-character-editor | SyntyCharCreator.unity | Explore Synty tool, define export pipeline |
| 002-stranded | StrandedDev.unity | Clean Loft3D, define game feel |
| 003-island-biome | TWC_StylizedIsland.unity | Study biome layers, map to TWC |

**Start next session with:**
```
"Read NEXT_SESSION_START.md and let's plan all 3 branches with devlogs and wiki pages."
```
