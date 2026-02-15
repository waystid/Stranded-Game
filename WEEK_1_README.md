# Week 1: Foundation - README

## Foundation Agent Deliverable

**Project**: Galactic Crossing MVP
**Week**: 1 - Grid Movement Foundation
**Status**: Ready for Manual Implementation
**Date**: 2026-02-15

---

## Quick Start

If you're ready to implement Week 1, follow these steps:

### For Quick Implementation (40 minutes)
1. Open `WEEK_1_QUICK_REFERENCE.md`
2. Follow the checklist step-by-step
3. Verify using the test criteria at the bottom

### For Detailed Understanding (2-3 hours)
1. Read `WEEK_1_TECHNICAL_ANALYSIS.md` to understand the architecture
2. Reference `WEEK_1_COMPONENT_DIAGRAM.md` for visual guidance
3. Implement following `WEEK_1_IMPLEMENTATION_GUIDE.md`
4. Use `WEEK_1_QUICK_REFERENCE.md` as a checklist

---

## Documentation Overview

This Week 1 package contains 5 comprehensive documents:

### 1. WEEK_1_README.md (This File)
**Purpose**: Navigation and overview
**Read time**: 5 minutes
**Use when**: Getting started, need orientation

### 2. WEEK_1_QUICK_REFERENCE.md
**Purpose**: Fast implementation checklist
**Read time**: 2 minutes
**File size**: 2.3 KB
**Use when**:
- You understand the concepts
- Need a quick reminder of settings
- Want to verify your work

**Contains**:
- GridManager setup (5 min)
- Character prefab modification (15 min)
- Combat cleanup checklist (20 min)
- Verification tests

### 3. WEEK_1_IMPLEMENTATION_GUIDE.md
**Purpose**: Comprehensive step-by-step guide
**Read time**: 30 minutes
**File size**: 17 KB
**Use when**:
- First time implementing
- Need detailed explanations
- Troubleshooting issues

**Contains**:
- Scene duplication instructions
- GridManager configuration
- Character prefab modifications
- Combat element removal
- Verification checklist
- Troubleshooting section
- File path reference

### 4. WEEK_1_TECHNICAL_ANALYSIS.md
**Purpose**: Deep technical understanding
**Read time**: 45 minutes
**File size**: 16 KB
**Use when**:
- Want to understand WHY, not just WHAT
- Planning modifications
- Learning TDE architecture

**Contains**:
- Code analysis of GridManager
- CharacterGridMovement internals
- CharacterOrientation3D logic
- Performance considerations
- Configuration rationale
- Risk assessment

### 5. WEEK_1_COMPONENT_DIAGRAM.md
**Purpose**: Visual architecture reference
**Read time**: 20 minutes (visual scanning)
**File size**: 28 KB
**Use when**:
- Visual learner
- Need to see component relationships
- Understanding data flow

**Contains**:
- Before/After component stacks
- Scene architecture diagrams
- GridManager system flow
- Movement behavior flowcharts
- Grid coordinate visualization
- Dependency diagrams

---

## Week 1 Objectives Checklist

### Primary Objectives

- [ ] **Scene Setup**
  - [ ] Create `/Assets/Scenes/` directory
  - [ ] Duplicate Loft3D scene to GalacticCrossingMVP
  - [ ] Scene opens without errors

- [ ] **GridManager Configuration**
  - [ ] GridManager GameObject exists in scene
  - [ ] Grid Unit Size = 1.0
  - [ ] Draw Debug Grid = TRUE
  - [ ] Debug grid visible in Play mode

- [ ] **Character Movement Modification**
  - [ ] CharacterHandleWeapon removed from LoftSuspenders
  - [ ] CharacterMovement removed from LoftSuspenders
  - [ ] CharacterGridMovement added with correct settings
    - [ ] Maximum Speed = 5.5
    - [ ] Acceleration = 15.0
    - [ ] Use Input Buffer = TRUE
    - [ ] Buffer Size = 2

- [ ] **Character Rotation Configuration**
  - [ ] CharacterOrientation3D configured
    - [ ] Rotation Mode = Movement Direction
    - [ ] Rotate To Face Movement Direction Speed = 8.0
    - [ ] Should Rotate To Face Weapon Direction = FALSE

- [ ] **Combat Cleanup**
  - [ ] All AI enemies removed (11 types)
  - [ ] All weapon pickups removed (10 types)
  - [ ] Explosive props removed
  - [ ] Furniture preserved
  - [ ] Lighting preserved
  - [ ] UI preserved

### Verification Tests

- [ ] **Play Mode Tests**
  - [ ] Character moves in grid squares (not free movement)
  - [ ] Movement feels weighted (not instant)
  - [ ] Character rotates smoothly toward movement
  - [ ] No weapon UI appears
  - [ ] No enemies present

---

## What You'll Have After Week 1

### A Clean Foundation:
1. **Non-combat scene** ready for resource gathering
2. **Grid-based movement system** configured for "cozy" feel
3. **Modified character controller** optimized for exploration
4. **Clean environment** with preserved furniture and lighting

### Technical Setup:
- GridManager singleton managing grid state
- CharacterGridMovement controlling player movement
- CharacterOrientation3D providing smooth rotation
- Scene hierarchy organized for future expansion

### Ready For:
- **Week 2**: Resource gathering (debris, crystals)
- **Week 2**: G.A.I.A. NPC implementation
- **Week 2**: Hab placement mechanics
- **Week 3**: Curved world shader

---

## Implementation Path

### Recommended Approach

```
┌─────────────────────────────────────┐
│ 1. Read WEEK_1_QUICK_REFERENCE.md  │
│    (5 min - get overview)           │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│ 2. Skim WEEK_1_COMPONENT_DIAGRAM.md│
│    (10 min - visual understanding)  │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│ 3. Open Unity Editor                │
│    + WEEK_1_IMPLEMENTATION_GUIDE.md │
│    (side-by-side)                   │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│ 4. Follow implementation guide      │
│    step-by-step                     │
│    (90 min - actual work)           │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│ 5. Run verification tests           │
│    (from Quick Reference)           │
│    (15 min)                         │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│ 6. (Optional) Read Technical        │
│    Analysis for deep understanding  │
│    (45 min)                         │
└─────────────────────────────────────┘
```

### Alternative: Quick Implementation

```
┌─────────────────────────────────────┐
│ 1. Open WEEK_1_QUICK_REFERENCE.md  │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│ 2. Follow checklist in Unity       │
│    (40 min if experienced)          │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│ 3. Run verification tests           │
└─────────────────────────────────────┘
```

---

## Key Settings Summary

For quick reference during implementation:

### GridManager
```
Grid Unit Size: 1.0
Draw Debug Grid: TRUE
Debug Draw Mode: ThreeD
```

### CharacterGridMovement
```
Maximum Speed: 5.5
Acceleration: 15.0
Use Input Buffer: TRUE
Buffer Size: 2
```

### CharacterOrientation3D
```
Rotation Mode: Movement Direction
Movement Rotation Speed: Smooth
Rotate To Face Movement Direction Speed: 8.0
Should Rotate To Face Weapon Direction: FALSE
```

---

## Troubleshooting Quick Guide

### Character not moving in grid
- Check GridManager exists in scene
- Verify CharacterGridMovement is enabled
- Ensure CharacterMovement was removed

### Grid not visible
- DrawDebugGrid must be TRUE
- Must be in Play mode
- Gizmos must be enabled in Scene view

### Character rotation wrong
- Check Rotation Mode = Movement Direction
- Verify rotation speed is 8.0 (not 0)
- Ensure weapon rotation is disabled

### Prefab changes not appearing
- Save prefab after editing (Ctrl/Cmd+S)
- Exit prefab mode properly
- Reload scene if needed

**For detailed troubleshooting**, see Section 9 of `WEEK_1_IMPLEMENTATION_GUIDE.md`

---

## File Locations

### Documentation (All in project root)
```
/TopDown Engine/
├── WEEK_1_README.md                    (This file)
├── WEEK_1_QUICK_REFERENCE.md           (Checklist)
├── WEEK_1_IMPLEMENTATION_GUIDE.md      (Detailed guide)
├── WEEK_1_TECHNICAL_ANALYSIS.md        (Deep dive)
├── WEEK_1_COMPONENT_DIAGRAM.md         (Visual reference)
└── plan-rough-draft.md                 (Original GDD)
```

### Unity Assets (Key paths)
```
Assets/
├── Scenes/
│   └── GalacticCrossingMVP.unity       (Create this)
│
└── TopDownEngine/
    ├── Common/Scripts/
    │   ├── Managers/
    │   │   └── GridManager.cs          (Reference)
    │   └── Characters/CharacterAbilities/
    │       ├── CharacterGridMovement.cs     (Reference)
    │       └── CharacterOrientation3D.cs    (Reference)
    │
    └── Demos/Loft3D/
        ├── Loft3D.unity                (Source scene)
        └── Prefabs/
            ├── PlayableCharacters/
            │   └── LoftSuspenders.prefab    (Modify)
            ├── AI/                     (Remove from scene)
            └── ItemPickers/            (Remove weapons)
```

---

## Success Criteria

### You've successfully completed Week 1 when:

1. **Visual Confirmation**:
   - Scene opens showing Loft3D environment without enemies
   - Cyan grid visible on ground in Play mode
   - Character present and selectable

2. **Movement Test**:
   - Character moves in discrete grid squares
   - Movement has noticeable acceleration (not instant)
   - Character drifts slightly when releasing input
   - Turning animation is smooth (not snappy)

3. **Clean Environment**:
   - No enemies spawn or exist
   - No weapon pickups visible
   - Furniture and lighting intact
   - Scene feels peaceful

4. **Technical Verification**:
   - No errors in Console
   - GridManager exists in Hierarchy
   - LoftSuspenders prefab modified correctly
   - All verification checkboxes completed

---

## Next Steps Preview

After completing Week 1, you'll be ready for:

### Week 2: Resource Gathering & NPCs
- Create resource item prefabs (debris, crystals)
- Implement G.A.I.A. hologram NPC
- Build gathering interaction system
- Quest tracking UI
- Hab placement mechanics

### Week 3: Visual Polish
- Curved world shader ("Rolling Log" effect)
- Cel shading implementation
- Character outline rendering
- Camera configuration for "cozy" view

### Week 4: Inventory & Crafting
- Inventory UI customization
- Crafting system setup
- Resource recipes
- Save/load implementation

---

## Support Resources

### TopDown Engine Documentation
- Official Docs: https://topdown-engine-docs.moremountains.com/
- Grid Movement: https://topdown-engine-docs.moremountains.com/grid-movement.html
- Character Abilities: https://topdown-engine-docs.moremountains.com/character-abilities.html

### Unity Resources
- Prefab Workflow: https://docs.unity3d.com/Manual/Prefabs.html
- Scene Management: https://docs.unity3d.com/Manual/CreatingScenes.html
- Input System: https://docs.unity3d.com/Packages/com.unity.inputsystem@latest

### Project-Specific
- Original GDD: `plan-rough-draft.md`
- Technical Analysis: `WEEK_1_TECHNICAL_ANALYSIS.md`

---

## Version History

**v1.0 - 2026-02-15**
- Initial Foundation Agent deliverable
- Complete Week 1 documentation package
- Created by Claude Sonnet 4.5 Foundation Agent

---

## Document Statistics

**Total Documentation**: 5 files
**Total Size**: ~64 KB
**Total Read Time**: ~2 hours (full deep dive)
**Quick Implementation Time**: ~40 minutes (with Quick Reference)
**Full Implementation Time**: ~2-3 hours (with learning)

---

## Getting Help

### If You Get Stuck:

1. **Check the Quick Reference** (`WEEK_1_QUICK_REFERENCE.md`)
   - Verify you have the correct settings

2. **Consult Troubleshooting** (Section 9 of Implementation Guide)
   - Common issues and solutions

3. **Review Component Diagram** (`WEEK_1_COMPONENT_DIAGRAM.md`)
   - Understand component relationships visually

4. **Read Technical Analysis** (`WEEK_1_TECHNICAL_ANALYSIS.md`)
   - Deep dive into how the systems work

5. **Check Console for Errors**
   - Unity Console shows specific error messages
   - Google the error + "TopDown Engine"

6. **TopDown Engine Forums**
   - Active community at More Mountains
   - Search for grid movement issues

---

## Final Checklist Before Starting

- [ ] Unity Editor installed and project loaded
- [ ] All Week 1 documentation files downloaded/accessible
- [ ] Backup of original Loft3D scene (just in case)
- [ ] 2-3 hours of uninterrupted time available
- [ ] TopDown Engine documentation bookmarked
- [ ] Console cleared of existing errors

**Ready?** Start with `WEEK_1_QUICK_REFERENCE.md` or `WEEK_1_IMPLEMENTATION_GUIDE.md`

---

## Foundation Agent Sign-Off

This Week 1 package provides everything you need to transform the Loft3D combat demo into the foundation for Galactic Crossing's cozy, grid-based exploration game.

All objectives from the original request have been fulfilled:
1. Scene duplication strategy documented (cannot be automated)
2. GridManager configuration specified
3. Character movement setup detailed
4. Combat removal checklist provided

**Status**: Ready for manual implementation in Unity Editor

**Estimated Time**: 2-3 hours

**Good luck, and happy coding!**

---

**Foundation Agent**: Claude Sonnet 4.5
**Date**: 2026-02-15
**Project**: Galactic Crossing MVP - Week 1 Foundation
