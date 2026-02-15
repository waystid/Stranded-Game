# Galactic Crossing - Assets Documentation Suite

## Overview

This folder contains comprehensive documentation for creating all ScriptableObjects and Prefabs required for the Galactic Crossing MVP. Since these assets must be created manually in the Unity Editor, this documentation suite serves as the complete implementation guide.

---

## Documentation Files

### 1. **ASSET_CREATION_GUIDE.md** (Main Reference)
**Purpose:** Complete specifications for all assets with detailed component configurations.

**Contents:**
- ScriptableObject documentation (ScrapMetal, EnergyCrystal, AlienBerry)
- ItemPicker prefab structures with component settings
- Tree prefab hierarchy and loot system configuration
- G.A.I.A. NPC prefab structure with dialogue content
- Full dialogue tree for all conversations

**Use When:** You need detailed specifications for creating a specific asset or configuring components.

**File Size:** Large (~15-20 pages equivalent)

---

### 2. **ASSET_QUICK_REFERENCE.md** (Cheat Sheet)
**Purpose:** Fast-lookup reference with condensed information.

**Contents:**
- Quick configuration blocks for all assets
- Critical values at a glance
- Common mistakes to avoid
- Fast testing checklist
- Audio/visual asset lists

**Use When:** You already know how to create the asset but need to quickly check a specific value or setting.

**File Size:** Medium (~8-10 pages equivalent)

---

### 3. **ASSET_RELATIONSHIPS_DIAGRAM.md** (Visual Guide)
**Purpose:** Visual representation of how all assets connect and interact.

**Contents:**
- Asset relationship flowcharts
- Component hierarchy diagrams
- Event system flows
- Dialogue → Quest → Item progression maps
- File structure visualization

**Use When:** You need to understand how systems connect, debug interaction flows, or plan new features.

**File Size:** Medium (~10-12 pages equivalent)

---

### 4. **IMPLEMENTATION_STEPS.md** (Sequential Workflow)
**Purpose:** Step-by-step ordered instructions for implementing everything.

**Contents:**
- 15 phases of implementation from setup to completion
- Prerequisite checks
- Dependency management (create X before Y)
- Testing procedures for each asset
- Troubleshooting common issues
- Completion checklist

**Use When:** You're starting from scratch or want to ensure you're following the correct implementation order.

**File Size:** Large (~25-30 pages equivalent)

---

### 5. **plan-rough-draft.md** (Game Design Document)
**Purpose:** Original GDD with narrative, technical architecture, and design philosophy.

**Contents:**
- Executive summary and project vision
- Narrative walkthrough (Day 0 and Day 1)
- TopDown Engine configuration (character controller, camera, input)
- Curved World Shader implementation
- Inventory and resource management systems
- AI behavior design
- Technical implementation details

**Use When:** You need deep context on why design decisions were made, or require technical implementation details for systems.

**File Size:** Very Large (~40+ pages equivalent)

---

## Quick Start Guide

### If you're starting asset creation for the first time:

1. **Start with:** `IMPLEMENTATION_STEPS.md`
   - Follow Phase 1 (Project Setup)
   - Create all folder structures
   - Set up layers and tags

2. **Reference:** `ASSET_CREATION_GUIDE.md`
   - Use for detailed component configurations
   - Check specifications for each asset

3. **Keep Open:** `ASSET_QUICK_REFERENCE.md`
   - Quick value lookups
   - Avoid common mistakes

4. **Understand Flow:** `ASSET_RELATIONSHIPS_DIAGRAM.md`
   - See how assets connect
   - Debug interaction issues

5. **Deep Dive:** `plan-rough-draft.md`
   - Technical implementation details
   - System architecture

---

## Asset Summary

### ScriptableObjects (Data Assets)
Located in: `Assets/Data/Items/`

1. **ScrapMetal.asset** - Resource item, stackable (30), used for crafting
2. **EnergyCrystal.asset** - Resource item, stackable (10), used for quests
3. **AlienBerry.asset** - Consumable item, stackable (10), restores 10 health

### Prefabs - Items
Located in: `Assets/Prefabs/Items/`

1. **ScrapMetalPicker.prefab** - Collectable metal debris
2. **EnergyCrystalPicker.prefab** - Collectable glowing crystal (with particles/light)
3. **AlienBerryPicker.prefab** - Collectable berry (respawns after 5 minutes)

### Prefabs - Environment
Located in: `Assets/Prefabs/Environment/`

1. **TreePrefab.prefab** - Shakeable tree that drops loot
   - 60% chance: 3-5 ScrapMetal
   - 30% chance: 1-2 AlienBerry
   - 10% chance: 1 EnergyCrystal
   - Loot respawns after 3 minutes

### Prefabs - NPCs
Located in: `Assets/Prefabs/NPCs/`

1. **GAIA_NPC.prefab** - Main quest-giver NPC with dialogue system
   - Day 0 quests: Deploy Habitat, Gather Resources, Name Planet
   - Day 1 quests: Scan Lifeforms, Place Museum Plot

---

## Key Systems

### Inventory System
- **Capacity:** 20 slots
- **Persistence:** Enabled (saves between sessions)
- **Target Inventory:** "MainInventory"

### Loot System
- Trees drop random loot based on weighted probability
- Loot respawns after timer expires
- Physics-based loot ejection

### Dialogue System
- G.A.I.A. NPC provides narrative and quests
- Dialogue triggers item rewards and quest progression
- Supports conditional dialogue based on quest state

### Quest System (Optional)
- Track objectives: item collection, placement tasks
- Monitor inventory for quest items
- Trigger dialogue based on completion

---

## Critical Technical Requirements

### Curved World Shader
**ESSENTIAL:** All visual materials MUST use the Curved World Shader to maintain the "Rolling Log" aesthetic.

**Implementation:**
- See `plan-rough-draft.md` Section 5 for full shader graph
- Apply to ALL materials: items, environment, NPCs, terrain
- Curve Amount: 0.005 (adjustable)

**Why:** Creates the signature Animal Crossing world curvature effect.

### Character Controller Settings
**ESSENTIAL:** Player movement must feel "weighted" and cozy, not twitchy.

**Key Values:**
- Walk Speed: 5.5
- Run Speed: 8.5
- Acceleration: 12-15 (NOT instant)
- Deceleration: 8-10 (creates drift/slide)
- Rotation Speed: 8.0 (smooth turns)

**Why:** Matches the "cozy" pacing of Animal Crossing.

### Layers Required
- **Items** - All ItemPicker objects
- **Environment** - Trees, rocks, terrain
- **Interactable** - DialogueZones, interaction triggers
- **NPC** - G.A.I.A. and future NPCs
- **Player** - Player character

---

## File Structure

```
/TopDown Engine/
├── ASSET_CREATION_GUIDE.md (this suite)
├── ASSET_QUICK_REFERENCE.md
├── ASSET_RELATIONSHIPS_DIAGRAM.md
├── IMPLEMENTATION_STEPS.md
├── ASSETS_README.md (you are here)
├── plan-rough-draft.md
│
└── Assets/
    ├── Data/
    │   └── Items/ (ScriptableObjects)
    ├── Prefabs/
    │   ├── Items/ (ItemPickers)
    │   ├── Environment/ (Trees, etc.)
    │   └── NPCs/ (G.A.I.A.)
    ├── Scripts/
    │   ├── Items/ (AlienBerryItem.cs)
    │   ├── Environment/
    │   └── Managers/
    ├── Art/
    │   ├── Icons/ (PNG sprites)
    │   ├── Meshes/ (FBX models)
    │   └── Materials/ (Shaders applied)
    └── Audio/
        ├── SFX/ (Sound effects)
        └── Music/ (Background tracks)
```

---

## Workflow Recommendations

### For Solo Developers:
1. Follow `IMPLEMENTATION_STEPS.md` linearly
2. Complete one phase before moving to the next
3. Test each asset immediately after creation
4. Keep `ASSET_QUICK_REFERENCE.md` open for value lookups

### For Teams:
1. **Art Team:** Create all meshes, materials, icons, audio (Phase 2)
2. **Programming Team:** Create scripts (Phase 5)
3. **Design Team:** Write dialogue content (Phase 10)
4. **Everyone:** Follow `IMPLEMENTATION_STEPS.md` for their assigned phases
5. **Integration Lead:** Assemble prefabs (Phases 6-9) and test (Phase 11)

---

## Testing Strategy

### Unit Testing (Per Asset)
After creating each asset:
1. Place in test scene
2. Verify basic functionality
3. Check component references
4. Test edge cases

### Integration Testing (Systems)
After completing a system:
1. Test item pickup → inventory
2. Test tree shake → loot drop → pickup
3. Test dialogue → quest → item reward
4. Verify save/load persistence

### Playthrough Testing (Full Experience)
After all assets complete:
1. Play Day 0 sequence start to finish
2. Play Day 1 sequence start to finish
3. Test non-linear paths (skip optional content)
4. Test failure states (full inventory, etc.)

---

## Common Issues & Solutions

### "Items don't pick up when I walk over them"
- **Check:** ItemPicker collider `IsTrigger` is TRUE
- **Check:** Player has "Player" tag
- **Check:** Item is on "Items" layer
- **Reference:** `IMPLEMENTATION_STEPS.md` Phase 14.3 (Troubleshooting)

### "AlienBerry doesn't restore health"
- **Check:** ScriptableObject uses `AlienBerryItem` script (not base `InventoryItem`)
- **Check:** `Usable` checkbox is enabled
- **Check:** `StaminaRestored` value is set (e.g., 10)
- **Reference:** `ASSET_CREATION_GUIDE.md` AlienBerry section

### "Tree doesn't shake or drop loot"
- **Check:** `ButtonActivated` component `CanActivate` is TRUE
- **Check:** Loot component has prefab references in `GameObjectsToLoot` array
- **Check:** `NumberOfDrops` is at least 1
- **Reference:** `ASSET_RELATIONSHIPS_DIAGRAM.md` Tree Shake Interaction Flow

### "Curved World effect not working"
- **Check:** Material is using `CurvedWorldShader`
- **Check:** `CurveAmount` property is set (0.005)
- **Check:** Shader graph connects to Vertex Position output
- **Reference:** `plan-rough-draft.md` Section 5.2

### "Dialogue box doesn't open"
- **Check:** DialogueZone collider is IsTrigger = TRUE
- **Check:** Player is within BoxCollider bounds
- **Check:** Dialogue conversation asset exists and is referenced
- **Reference:** `IMPLEMENTATION_STEPS.md` Phase 9.3

---

## Performance Guidelines

### Target Performance:
- **Frame Rate:** 60 FPS on target platform
- **Draw Calls:** < 100 per frame
- **Poly Count per Object:** < 1000 triangles
- **Texture Size:** 512x512 for most objects, 1024x1024 for terrain

### Optimization Tips:
1. Use simple colliders (sphere/box/capsule, NOT mesh)
2. Limit particle emission rates (5-10 particles max)
3. Use LOD (Level of Detail) for distant objects
4. Batch materials where possible
5. Disable shadows on small objects

---

## Support & Resources

### TopDown Engine Documentation:
- **Main Docs:** https://topdown-engine-docs.moremountains.com/
- **Character Abilities:** https://topdown-engine-docs.moremountains.com/character-abilities.html
- **Inventory:** https://topdown-engine-docs.moremountains.com/inventory.html
- **AI:** https://topdown-engine-docs.moremountains.com/ai.html

### Inventory Engine Documentation:
- **Main Docs:** https://inventory-engine-docs.moremountains.com/
- **Items:** https://inventory-engine-docs.moremountains.com/items.html
- **Save/Load:** https://inventory-engine-docs.moremountains.com/save-and-load.html

### Unity Resources:
- **Shader Graph:** https://learn.unity.com/tutorial/shader-graph
- **URP:** https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest
- **Cinemachine:** https://docs.unity3d.com/Packages/com.unity.cinemachine@latest

### Community:
- **Unity Forums:** https://forum.unity.com/
- **More Mountains Discord:** Check asset store page for invite link

---

## Version History

**Version 1.0** (2026-02-15)
- Initial documentation suite created
- All MVP assets documented
- Day 0 and Day 1 content specified
- Complete implementation workflow established

---

## Next Steps

After completing all assets in this documentation suite:

1. **Expand Content**
   - Create Day 2+ progression
   - Add more item types
   - Expand NPC cast

2. **Polish**
   - Enhance visual effects
   - Improve animations
   - Add background music

3. **Playtest**
   - Get external feedback
   - Balance quest difficulty
   - Fix bugs

4. **Optimize**
   - Profile performance
   - Reduce draw calls
   - Optimize shaders

5. **Prepare for Release**
   - Build for target platform
   - Create marketing materials
   - Write patch notes

---

## Credits

**Game Design Document:** Based on `plan-rough-draft.md`

**Engine:** Unity + TopDown Engine by More Mountains

**Target Inspiration:** Animal Crossing: New Horizons (Nintendo)

**Genre:** Cozy Sci-Fi Social Simulation

---

## License & Usage

This documentation is specific to the Galactic Crossing project.

**Internal Use Only:** Do not distribute outside the development team.

**Asset Store Assets:** Ensure compliance with TopDown Engine and Inventory Engine licenses.

---

## Contact

For questions about this documentation or asset implementation:

1. Review `IMPLEMENTATION_STEPS.md` troubleshooting section
2. Check `ASSET_RELATIONSHIPS_DIAGRAM.md` for system flow
3. Consult TopDown Engine official documentation
4. Reach out to lead developer/technical director

---

**Happy Creating!**

Build something cozy. Build something that makes people smile. Build Galactic Crossing.

---

**Last Updated:** 2026-02-15
**Documentation Version:** 1.0 (MVP)
**Project Phase:** Asset Creation
