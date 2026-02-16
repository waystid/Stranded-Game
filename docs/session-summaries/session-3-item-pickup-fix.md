# Session 3: Item Pickup System Fix

## Date
February 16, 2026

## Problem Statement
Items dropped from trees in the game were experiencing two critical issues:
1. Player walking over items did not trigger pickup
2. Items fell through the ground after being dropped

## Root Cause Analysis

### Issue 1: Pickup Detection Failure
- **Root Cause:** Items lacked Rigidbody components
- **Why it failed:** In Unity 3D, trigger collision events (OnTriggerEnter) require at least one colliding object to have a Rigidbody
- **The player has:** CharacterController (doesn't participate in trigger events the same way)
- **Items needed:** Rigidbody to enable trigger detection

### Issue 2: Items Falling Through Ground
- **Root Cause:** Colliders were set as triggers only (IsTrigger=true)
- **Why it failed:** Trigger colliders detect overlaps but don't create physical collision
- **The ground has:** MeshCollider (solid, non-trigger)
- **Items needed:** At least one solid (non-trigger) collider for ground collision

## Solution Architecture

Implemented a **three-component collision system** on each item prefab:

### Components Added to Root GameObject

1. **Rigidbody**
   - Purpose: Enable physics simulation and trigger event detection
   - Configuration:
     - Mass: 1
     - UseGravity: true (items fall when dropped from trees)
     - IsKinematic: false (allows physics interactions)
     - Drag: 0 (default)
     - Angular Drag: 0.05

2. **SphereCollider (Trigger)**
   - Purpose: Detect when player enters pickup range
   - Configuration:
     - IsTrigger: **true**
     - Radius: 0.5 units
     - Center: (0, 0, 0)
   - How it works: Fires OnTriggerEnter when player's CharacterController enters the sphere

3. **BoxCollider (Solid)**
   - Purpose: Provide physical collision with ground and environment
   - Configuration:
     - IsTrigger: **false**
     - Size: 0.3-0.4 units (varies by item)
     - Center: (0, 0, 0)
   - How it works: Creates physical collision preventing items from falling through surfaces

### Visual Child GameObject

The Visual child retains its original BoxCollider (IsTrigger=false, but very small due to 0.15 scale). This provides additional collision detail but the root-level BoxCollider is the primary ground collision provider.

## Modified Files

### Prefabs
1. `Assets/Prefabs/AlienBerryPicker.prefab`
   - Added: Rigidbody, SphereCollider (trigger, r=0.5), BoxCollider (solid, 0.3x0.3x0.3)

2. `Assets/Prefabs/EnergyCrystalPicker.prefab`
   - Added: Rigidbody, SphereCollider (trigger, r=0.5), BoxCollider (solid, 0.4x0.4x0.4)

3. `Assets/Prefabs/ScrapMetalPicker.prefab`
   - Added: Rigidbody, SphereCollider (trigger, r=0.5), BoxCollider (solid, 0.4x0.4x0.4)

## Key Unity Concepts Learned

### 1. Trigger vs. Solid Colliders
- **Trigger Colliders (IsTrigger=true):**
  - Detect overlaps without creating physical collision
  - Fire OnTriggerEnter/OnTriggerExit events
  - Don't prevent objects from passing through each other
  - Use cases: Pickup zones, damage areas, trigger volumes

- **Solid Colliders (IsTrigger=false):**
  - Create physical collision and prevent interpenetration
  - Fire OnCollisionEnter/OnCollisionExit events
  - Stop objects from passing through
  - Use cases: Ground collision, walls, physical objects

### 2. Multiple Colliders on Same GameObject
Unity allows multiple colliders on a single GameObject. This enables:
- One collider for trigger detection (large sphere for pickup range)
- Another collider for physical collision (smaller box for ground contact)
- Both colliders work simultaneously and independently

### 3. Rigidbody Requirements for Triggers
For OnTriggerEnter to fire in 3D:
- At least ONE of the two colliding objects must have a Rigidbody
- CharacterController is special and doesn't count as a Rigidbody for this purpose
- Items with trigger colliders NEED their own Rigidbody

### 4. The Pickup Flow
1. Tree is shaken (TreeShakeZone triggered)
2. Loot component spawns item prefab with random velocity
3. Item falls due to Rigidbody + gravity
4. Item's solid BoxCollider collides with ground MeshCollider
5. Item settles on ground
6. Player walks nearby
7. Player's CharacterController enters item's trigger SphereCollider
8. OnTriggerEnter fires on ItemPicker component
9. ItemPicker checks: RequirePlayerTag=true, player has "Player" tag ✓
10. Item added to inventory, GameObject disabled

## Testing Verification

### Before Fix
- ❌ Items fell through ground
- ❌ Walking over items did nothing
- ❌ Pressing Space/E had no effect (jumping instead)

### After Fix
- ✅ Items fall and land on ground properly
- ✅ Walking over items triggers automatic pickup
- ✅ Items disappear and are added to inventory
- ✅ No button press required (automatic pickup on trigger enter)

## Related Systems

### ItemPicker Component
- Location: `Assets/TopDownEngine/ThirdParty/MoreMountains/InventoryEngine/InventoryEngine/Scripts/Core/ItemPicker.cs`
- Key methods:
  - `OnTriggerEnter(Collider collider)` - Detects 3D trigger collision
  - `Pick(string targetInventoryName, string playerID)` - Adds item to inventory
  - `Pickable()` - Checks if inventory has space

### Player Setup
- GameObject: LoftSuspenders
- Key components:
  - CharacterController (movement)
  - Rigidbody (isKinematic=true)
  - CharacterInventory (manages items)
  - Tag: "Player" (required by ItemPicker.RequirePlayerTag)

### Loot Spawning
- Component: Loot (on AlienTree/LootSpawnPoint)
- Spawns items from loot table when tree is shaken
- Applies random force to spawned items

## Troubleshooting Guide

### Items still not picking up?
1. Check player has "Player" tag
2. Verify ItemPicker.RequirePlayerTag setting
3. Check item prefab has Rigidbody + trigger SphereCollider
4. Verify CharacterInventory component exists on player
5. Check inventory isn't full (or enable PickableIfInventoryIsFull)

### Items falling through ground?
1. Verify item has solid (non-trigger) collider
2. Check ground has collider (MeshCollider, BoxCollider, etc.)
3. Verify no layer collision matrix issues
4. Check collider size isn't too small (< 0.1 units can be unreliable)

### Items not spawning from trees?
1. Check Loot component on tree's LootSpawnPoint child
2. Verify loot table has item prefabs assigned
3. Check TreeShakeZone is triggering (button activation works)
4. Verify item prefabs exist in Assets/Prefabs/

## Future Considerations

### Potential Improvements
1. **Pickup Radius Tuning:** Currently 0.5 units - may want to adjust based on playtesting
2. **Pickup Feedback:** Add visual/audio feedback when item enters pickup range
3. **Item Magnet Effect:** Gradually pull items toward player when in range
4. **Pickup Button Option:** Add optional button press requirement (some players prefer this)
5. **Stack Limit Handling:** Better UI feedback when inventory is full

### Performance Notes
- Three colliders per item adds minimal overhead
- Rigidbody sleeps when item is stationary (optimized automatically)
- Consider object pooling if spawning many items frequently

## References
- Unity Collider Documentation: https://docs.unity3d.com/Manual/CollidersOverview.html
- Unity Physics Events: https://docs.unity3d.com/ScriptReference/Collider.OnTriggerEnter.html
- TopDown Engine Item Pickup: (Assets/TopDownEngine/Common/Scripts/Items/)
