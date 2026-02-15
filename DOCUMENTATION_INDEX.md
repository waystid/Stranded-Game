# Galactic Crossing - Documentation Index

## Quick Navigation Guide

This index helps you find the right documentation file for your specific need.

---

## "I Want To..." Quick Links

### "I want to create a specific asset"
→ **Go to:** `ASSET_CREATION_GUIDE.md`
- Section 1: ScriptableObject Documentation
- Section 2: ItemPicker Prefab Documentation
- Section 3: Tree Prefab Documentation
- Section 4: G.A.I.A. NPC Prefab Documentation

### "I need to know a specific value quickly"
→ **Go to:** `ASSET_QUICK_REFERENCE.md`
- Quick configuration blocks
- Critical values at a glance
- Component settings cheat sheet

### "I need to understand how systems connect"
→ **Go to:** `ASSET_RELATIONSHIPS_DIAGRAM.md`
- Asset relationship flowcharts
- Event system flows
- Dialogue → Quest → Item progression

### "I'm starting from scratch, what order should I do things?"
→ **Go to:** `IMPLEMENTATION_STEPS.md`
- Phase-by-phase sequential workflow
- Dependency management
- Complete checklist

### "I need deep technical context or theory"
→ **Go to:** `plan-rough-draft.md`
- Game Design Document
- Narrative walkthrough
- Technical architecture
- Shader implementation details

### "I want an overview of everything"
→ **Go to:** `ASSETS_README.md`
- Documentation suite overview
- Asset summary
- Workflow recommendations
- Support resources

### "I want to find specific information fast"
→ **You are here:** `DOCUMENTATION_INDEX.md`
- Use the sections below

---

## Documentation Files At-a-Glance

```
┌─────────────────────────────────────────────────────────────┐
│ START HERE: ASSETS_README.md                                │
│ Overview of all documentation and asset summary             │
└─────────────────────────────────────────────────────────────┘
                           │
                           │
         ┌─────────────────┼─────────────────┐
         │                 │                 │
         ▼                 ▼                 ▼
┌─────────────────┐ ┌─────────────┐ ┌──────────────────┐
│ IMPLEMENTATION_ │ │   ASSET_    │ │     ASSET_       │
│    STEPS.md     │ │ CREATION_   │ │ RELATIONSHIPS_   │
│                 │ │  GUIDE.md   │ │   DIAGRAM.md     │
│ Step-by-step    │ │             │ │                  │
│ workflow        │ │ Detailed    │ │ Visual system    │
│ (Start here if  │ │ specs       │ │ maps             │
│  implementing)  │ │ (Reference) │ │ (Understanding)  │
└─────────────────┘ └─────────────┘ └──────────────────┘
         │                 │                 │
         │                 │                 │
         └─────────────────┼─────────────────┘
                           │
                           ▼
                  ┌──────────────────┐
                  │   ASSET_QUICK_   │
                  │  REFERENCE.md    │
                  │                  │
                  │ Fast lookups     │
                  │ (Keep open)      │
                  └──────────────────┘

┌─────────────────────────────────────────────────────────────┐
│ DEEP DIVE: plan-rough-draft.md                              │
│ Full GDD with technical implementation details              │
└─────────────────────────────────────────────────────────────┘
```

---

## Search by Topic

### ScriptableObjects
- **Detailed Specs:** `ASSET_CREATION_GUIDE.md` → Section 1
- **Quick Values:** `ASSET_QUICK_REFERENCE.md` → ScriptableObjects Quick Reference
- **Creation Steps:** `IMPLEMENTATION_STEPS.md` → Phase 6
- **Relationships:** `ASSET_RELATIONSHIPS_DIAGRAM.md` → ScriptableObject → Prefab Relationships

### ItemPicker Prefabs
- **Detailed Specs:** `ASSET_CREATION_GUIDE.md` → Section 2
- **Quick Values:** `ASSET_QUICK_REFERENCE.md` → ItemPicker Prefabs Quick Reference
- **Creation Steps:** `IMPLEMENTATION_STEPS.md` → Phase 7
- **Relationships:** `ASSET_RELATIONSHIPS_DIAGRAM.md` → Item Pickup Event Flow

### Tree Prefab
- **Detailed Specs:** `ASSET_CREATION_GUIDE.md` → Section 3
- **Quick Values:** `ASSET_QUICK_REFERENCE.md` → Environment Prefabs Quick Reference
- **Creation Steps:** `IMPLEMENTATION_STEPS.md` → Phase 8
- **Relationships:** `ASSET_RELATIONSHIPS_DIAGRAM.md` → Tree Prefab Component Hierarchy
- **Loot System:** `ASSET_RELATIONSHIPS_DIAGRAM.md` → Tree Shake Interaction Flow

### G.A.I.A. NPC
- **Detailed Specs:** `ASSET_CREATION_GUIDE.md` → Section 4
- **Dialogue Content:** `ASSET_CREATION_GUIDE.md` → G.A.I.A. Dialogue Content
- **Quick Values:** `ASSET_QUICK_REFERENCE.md` → NPC Prefabs Quick Reference
- **Creation Steps:** `IMPLEMENTATION_STEPS.md` → Phase 9
- **Relationships:** `ASSET_RELATIONSHIPS_DIAGRAM.md` → G.A.I.A. NPC Component Hierarchy

### Curved World Shader
- **Technical Details:** `plan-rough-draft.md` → Section 5
- **Creation Steps:** `IMPLEMENTATION_STEPS.md` → Phase 3
- **Visual Example:** `ASSET_RELATIONSHIPS_DIAGRAM.md` → Curved World Shader Integration
- **Materials Using:** `IMPLEMENTATION_STEPS.md` → Phase 4

### Inventory System
- **Overview:** `plan-rough-draft.md` → Section 4
- **Data Flow:** `ASSET_RELATIONSHIPS_DIAGRAM.md` → Inventory System Data Flow
- **Item Configuration:** `ASSET_CREATION_GUIDE.md` → Section 1
- **Setup Steps:** `IMPLEMENTATION_STEPS.md` → Phase 13.4

### Loot System
- **Tree Loot:** `ASSET_CREATION_GUIDE.md` → Section 3 (TreePrefab)
- **Loot Flow:** `ASSET_RELATIONSHIPS_DIAGRAM.md` → Tree Shake Interaction Flow
- **Configuration:** `ASSET_QUICK_REFERENCE.md` → Component Configuration Quick Reference (Loot Component)

### Dialogue System
- **Dialogue Content:** `ASSET_CREATION_GUIDE.md` → G.A.I.A. Dialogue Content
- **Quick Lines:** `ASSET_QUICK_REFERENCE.md` → Dialogue Quick Reference
- **Setup Steps:** `IMPLEMENTATION_STEPS.md` → Phase 10
- **Quest Flow:** `ASSET_RELATIONSHIPS_DIAGRAM.md` → Dialogue → Quest → Item Flow

### Quest System
- **Day 0 Quests:** `plan-rough-draft.md` → Section 2.1
- **Day 1 Quests:** `plan-rough-draft.md` → Section 2.2
- **Progression Flow:** `ASSET_RELATIONSHIPS_DIAGRAM.md` → Dialogue → Quest → Item Flow
- **Integration Steps:** `IMPLEMENTATION_STEPS.md` → Phase 12
- **Quest Graph:** `ASSET_RELATIONSHIPS_DIAGRAM.md` → Quest Dependency Graph

### Character Controller
- **Configuration:** `plan-rough-draft.md` → Section 3.1
- **Physics Settings:** `plan-rough-draft.md` → Table 2
- **Setup Steps:** `IMPLEMENTATION_STEPS.md` → Phase 13.4

### Camera Setup
- **Configuration:** `plan-rough-draft.md` → Section 3.3
- **Setup Steps:** `IMPLEMENTATION_STEPS.md` → Phase 13.5

---

## Search by Asset Type

### Icons (2D Sprites)
- **List:** `ASSET_QUICK_REFERENCE.md` → Visual Assets Needed → Icons
- **Import Steps:** `IMPLEMENTATION_STEPS.md` → Phase 2.1

### 3D Meshes
- **List:** `ASSET_QUICK_REFERENCE.md` → Visual Assets Needed → 3D Meshes
- **Import Steps:** `IMPLEMENTATION_STEPS.md` → Phase 2.2

### Materials
- **Creation Steps:** `IMPLEMENTATION_STEPS.md` → Phase 4
- **Curved World Integration:** `ASSET_RELATIONSHIPS_DIAGRAM.md` → Curved World Shader Integration

### Audio (SFX)
- **List:** `ASSET_QUICK_REFERENCE.md` → Audio Assets Needed
- **Import Steps:** `IMPLEMENTATION_STEPS.md` → Phase 2.3

### Scripts
- **AlienBerryItem:** `plan-rough-draft.md` → Lines 262-302
- **Creation Steps:** `IMPLEMENTATION_STEPS.md` → Phase 5

---

## Search by Implementation Phase

### Phase 1: Project Setup
→ `IMPLEMENTATION_STEPS.md` → Phase 1
- Verify engine installation
- Create folder structure
- Setup layers and tags

### Phase 2: Art Assets & Audio
→ `IMPLEMENTATION_STEPS.md` → Phase 2
- Import icons
- Import meshes
- Import audio

### Phase 3: Curved World Shader
→ `IMPLEMENTATION_STEPS.md` → Phase 3
→ `plan-rough-draft.md` → Section 5
- Create shader graph
- Test shader

### Phase 4: Materials
→ `IMPLEMENTATION_STEPS.md` → Phase 4
- Create item materials
- Create environment materials
- Create NPC materials

### Phase 5: Custom Scripts
→ `IMPLEMENTATION_STEPS.md` → Phase 5
- Create AlienBerryItem.cs

### Phase 6: ScriptableObjects
→ `IMPLEMENTATION_STEPS.md` → Phase 6
→ `ASSET_CREATION_GUIDE.md` → Section 1
- Create ScrapMetal.asset
- Create EnergyCrystal.asset
- Create AlienBerry.asset

### Phase 7: ItemPicker Prefabs
→ `IMPLEMENTATION_STEPS.md` → Phase 7
→ `ASSET_CREATION_GUIDE.md` → Section 2
- Create ScrapMetalPicker.prefab
- Create EnergyCrystalPicker.prefab
- Create AlienBerryPicker.prefab

### Phase 8: Environment Prefabs
→ `IMPLEMENTATION_STEPS.md` → Phase 8
→ `ASSET_CREATION_GUIDE.md` → Section 3
- Create TreePrefab.prefab

### Phase 9: NPC Prefabs
→ `IMPLEMENTATION_STEPS.md` → Phase 9
→ `ASSET_CREATION_GUIDE.md` → Section 4
- Create GAIA_NPC.prefab

### Phase 10: Dialogue System
→ `IMPLEMENTATION_STEPS.md` → Phase 10
→ `ASSET_CREATION_GUIDE.md` → G.A.I.A. Dialogue Content
- Setup dialogue system
- Write dialogue content

### Phase 11: Testing
→ `IMPLEMENTATION_STEPS.md` → Phase 11
- Test all assets in test scene

### Phase 12: Quest Integration
→ `IMPLEMENTATION_STEPS.md` → Phase 12
- Create quest manager
- Link quests to dialogue

### Phase 13: Main Game Scene
→ `IMPLEMENTATION_STEPS.md` → Phase 13
- Build terrain
- Place assets
- Setup player and camera

### Phase 14: Final Testing & Polish
→ `IMPLEMENTATION_STEPS.md` → Phase 14
- Full playthrough test
- Performance check
- Bug fixing

### Phase 15: Documentation & Handoff
→ `IMPLEMENTATION_STEPS.md` → Phase 15
- Document settings
- Create asset inventory
- Backup project

---

## Troubleshooting Index

### Common Issues
→ `IMPLEMENTATION_STEPS.md` → Phase 14.3 (Troubleshooting)
→ `ASSETS_README.md` → Common Issues & Solutions

**Quick Links:**
- Items not picking up
- Berry not healing
- Tree not shaking
- Loot not spawning
- Dialogue not opening
- Curved world effect not working

---

## Component Configuration Index

### ItemPicker Component
- **Detailed:** `ASSET_CREATION_GUIDE.md` → Section 2 (each prefab)
- **Quick:** `ASSET_QUICK_REFERENCE.md` → Component Configuration Quick Reference

### ButtonActivated Component
- **Detailed:** `ASSET_CREATION_GUIDE.md` → Section 3 (TreePrefab) & Section 4 (G.A.I.A.)
- **Quick:** `ASSET_QUICK_REFERENCE.md` → Component Configuration Quick Reference

### Loot Component
- **Detailed:** `ASSET_CREATION_GUIDE.md` → Section 3 (TreePrefab)
- **Quick:** `ASSET_QUICK_REFERENCE.md` → Component Configuration Quick Reference

---

## Critical Values Quick Lookup

### Item Stack Sizes
→ `ASSET_QUICK_REFERENCE.md` → Critical Values Reference
- ScrapMetal: 30
- EnergyCrystal: 10
- AlienBerry: 10

### Quest Requirements
→ `ASSET_QUICK_REFERENCE.md` → Critical Values Reference
- Gather Resources: 10 ScrapMetal + 6 EnergyCrystal
- Scan Lifeforms: 5 unique specimens

### Respawn Times
→ `ASSET_QUICK_REFERENCE.md` → Critical Values Reference
- AlienBerry: 300 seconds (5 minutes)
- Tree Loot: 180 seconds (3 minutes)

### Interaction Ranges
→ `ASSET_QUICK_REFERENCE.md` → Critical Values Reference
- ItemPicker: 2.0 units
- Tree Shake: 1.5 radius
- G.A.I.A. Dialogue: 2.5x2.0x2.5 box

### Loot Probabilities
→ `ASSET_QUICK_REFERENCE.md` → Critical Values Reference
- ScrapMetal: 60%
- AlienBerry: 30%
- EnergyCrystal: 10%

---

## File Locations

### Documentation Files
```
/TopDown Engine/
├── ASSETS_README.md (Overview)
├── DOCUMENTATION_INDEX.md (This file)
├── ASSET_CREATION_GUIDE.md (Detailed specs)
├── ASSET_QUICK_REFERENCE.md (Cheat sheet)
├── ASSET_RELATIONSHIPS_DIAGRAM.md (Visual guide)
├── IMPLEMENTATION_STEPS.md (Sequential workflow)
└── plan-rough-draft.md (GDD)
```

### Asset Folders
```
/TopDown Engine/Assets/
├── Data/Items/ (ScriptableObjects)
├── Prefabs/
│   ├── Items/ (ItemPickers)
│   ├── Environment/ (Trees)
│   └── NPCs/ (G.A.I.A.)
├── Scripts/
│   ├── Items/ (AlienBerryItem.cs)
│   ├── Environment/
│   └── Managers/
├── Art/
│   ├── Icons/
│   ├── Meshes/
│   └── Materials/
└── Audio/
    ├── SFX/
    └── Music/
```

---

## Recommended Reading Order

### For First-Time Implementers:
1. **Start:** `ASSETS_README.md` (Understand the big picture)
2. **Then:** `IMPLEMENTATION_STEPS.md` (Follow step-by-step)
3. **Reference:** `ASSET_CREATION_GUIDE.md` (When creating each asset)
4. **Keep Open:** `ASSET_QUICK_REFERENCE.md` (For quick lookups)
5. **If Confused:** `ASSET_RELATIONSHIPS_DIAGRAM.md` (See how things connect)
6. **Deep Dive:** `plan-rough-draft.md` (When you need theory)

### For Experienced Unity Developers:
1. **Skim:** `ASSETS_README.md` (Get overview)
2. **Use:** `ASSET_QUICK_REFERENCE.md` (Primary reference)
3. **Check:** `ASSET_RELATIONSHIPS_DIAGRAM.md` (System architecture)
4. **Validate:** `ASSET_CREATION_GUIDE.md` (Detailed specs when needed)

### For Artists/Audio Designers:
1. **Read:** `ASSETS_README.md` → Asset Summary
2. **Follow:** `IMPLEMENTATION_STEPS.md` → Phase 2 (Art Assets & Audio)
3. **Reference:** `ASSET_QUICK_REFERENCE.md` → Visual Assets Needed / Audio Assets Needed

### For Writers/Designers:
1. **Read:** `plan-rough-draft.md` → Section 2 (Narrative Walkthrough)
2. **Write:** `ASSET_CREATION_GUIDE.md` → G.A.I.A. Dialogue Content (template)
3. **Reference:** `ASSET_QUICK_REFERENCE.md` → Dialogue Quick Reference

---

## Documentation Statistics

| File | Pages (Est.) | Primary Audience | Use Case |
|------|--------------|------------------|----------|
| ASSETS_README.md | 8 | Everyone | Overview |
| DOCUMENTATION_INDEX.md | 5 | Everyone | Navigation |
| ASSET_CREATION_GUIDE.md | 18 | Implementers | Detailed specs |
| ASSET_QUICK_REFERENCE.md | 10 | Implementers | Fast lookup |
| ASSET_RELATIONSHIPS_DIAGRAM.md | 12 | Tech/Design | Understanding |
| IMPLEMENTATION_STEPS.md | 28 | Implementers | Sequential workflow |
| plan-rough-draft.md | 42 | Tech/Design | Deep dive |

**Total Documentation:** ~123 pages equivalent

---

## External Resources

### TopDown Engine
- **Docs:** https://topdown-engine-docs.moremountains.com/
- **Character:** https://topdown-engine-docs.moremountains.com/character-abilities.html
- **Inventory:** https://topdown-engine-docs.moremountains.com/inventory.html
- **AI:** https://topdown-engine-docs.moremountains.com/ai.html
- **Loot:** https://topdown-engine-docs.moremountains.com/loot.html

### Inventory Engine
- **Docs:** https://inventory-engine-docs.moremountains.com/
- **Items:** https://inventory-engine-docs.moremountains.com/items.html
- **Save/Load:** https://inventory-engine-docs.moremountains.com/save-and-load.html

### Unity
- **Shader Graph:** https://learn.unity.com/tutorial/shader-graph
- **URP:** https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest
- **Cinemachine:** https://docs.unity3d.com/Packages/com.unity.cinemachine@latest

---

## Version Information

**Documentation Version:** 1.0 (MVP)
**Last Updated:** 2026-02-15
**Project Phase:** Asset Creation
**Target Unity Version:** 2022.3 LTS or later
**TopDown Engine Version:** Latest stable
**Inventory Engine Version:** Latest stable

---

## How to Use This Index

1. **Know what you want to do?** → Use "I Want To..." section at top
2. **Looking for a specific topic?** → Use "Search by Topic" section
3. **Following implementation order?** → Use "Search by Implementation Phase"
4. **Need a specific value?** → Use "Critical Values Quick Lookup"
5. **Experiencing an issue?** → Use "Troubleshooting Index"
6. **First time here?** → Use "Recommended Reading Order"

---

**Navigation Tip:** Use Ctrl+F (Cmd+F on Mac) to search this document for keywords like "tree", "berry", "dialogue", "shader", etc.

---

Happy documenting! This index should help you navigate the comprehensive asset documentation suite efficiently.

**Last Updated:** 2026-02-15
