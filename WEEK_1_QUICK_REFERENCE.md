# Week 1 Quick Reference Card

## GridManager Setup (5 min)
```
1. Hierarchy → Right-click → Create Empty → Name: "GridManager"
2. Position: (0, 0, 0)
3. Add Component → "Grid Manager"
4. Settings:
   ✓ Grid Origin: Self-reference
   ✓ Grid Unit Size: 1.0
   ✓ Draw Debug Grid: TRUE
   ✓ Debug Draw Mode: ThreeD
```

## Character Prefab Modification (15 min)
**Path**: `Assets/TopDownEngine/Demos/Loft3D/Prefabs/PlayableCharacters/LoftSuspenders.prefab`

### Step 1: Remove Components
```
- CharacterHandleWeapon → Remove
- CharacterMovement → Remove
```

### Step 2: Add CharacterGridMovement
```
Add Component → "Character Grid Movement"
Settings:
  ✓ Maximum Speed: 5.5
  ✓ Acceleration: 15.0
  ✓ Use Input Buffer: TRUE
  ✓ Buffer Size: 2
```

### Step 3: Configure CharacterOrientation3D
```
Find existing component or Add Component → "Character Orientation 3D"
Settings:
  ✓ Rotation Mode: Movement Direction
  ✓ Movement Rotation Speed: Smooth
  ✓ Rotate To Face Movement Direction Speed: 8.0
  ✓ Should Rotate To Face Weapon Direction: FALSE
```

## Combat Cleanup (20 min)

### Delete All AI Enemies:
- LoftPatrolAndShootAI
- LoftSuitRagdollAI
- PatrolSeekAndSwordAI
- LoftSuitAI
- LoftTieMouseDrivenPathfinderAI
- Loft3DBlueCube
- LoftPatrolAndPathfinderAI
- LoftPatrolSeekAndDestroyAI
- LoftPatrolSeekAndDestroyReloadingAI
- LoftPatrolConeSeekAndDestroyAI
- LoftTieAI

### Delete All Weapon Pickups:
- Search "Weapon" → Delete all
- Search "Ammo" → Delete all
- Search "Crate" → Delete remaining combat items

### Keep:
- All furniture (LoftBed, LoftTable, etc.)
- All lighting
- UI Canvas
- Camera system

## Verification Tests

### In Editor:
- [ ] Grid Unit Size = 1.0
- [ ] Draw Debug Grid = TRUE
- [ ] No CharacterHandleWeapon on prefab
- [ ] CharacterGridMovement present
- [ ] Rotation Speed = 8.0

### In Play Mode:
- [ ] Cyan grid visible on ground
- [ ] Character moves in grid squares
- [ ] Movement feels weighted (not instant)
- [ ] Character rotates smoothly
- [ ] No enemies present
- [ ] No weapon UI

## File Paths
- Scene: `/Assets/Scenes/GalacticCrossingMVP.unity`
- Prefab: `/Assets/TopDownEngine/Demos/Loft3D/Prefabs/PlayableCharacters/LoftSuspenders.prefab`
- AI Path: `/Assets/TopDownEngine/Demos/Loft3D/Prefabs/AI/`

## Total Time: ~40 minutes
