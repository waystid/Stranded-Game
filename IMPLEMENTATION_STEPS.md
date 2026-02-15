# Galactic Crossing - Step-by-Step Implementation Guide

This guide provides a **sequential, ordered workflow** for implementing all assets in the Unity Editor. Follow these steps in order to minimize errors and ensure all dependencies are properly linked.

---

## Phase 1: Project Setup & Prerequisites

### Step 1.1: Verify TopDown Engine Installation
- [ ] Open Unity project
- [ ] Verify TopDown Engine is imported (check `Assets/TopDownEngine/`)
- [ ] Verify Inventory Engine is available
- [ ] Check that URP (Universal Render Pipeline) is configured

### Step 1.2: Create Folder Structure
Create all necessary folders before creating assets:

```bash
Assets/
├─ Data/
│  └─ Items/
├─ Prefabs/
│  ├─ Items/
│  ├─ Environment/
│  └─ NPCs/
├─ Scripts/
│  ├─ Items/
│  ├─ Environment/
│  └─ Managers/
├─ Art/
│  ├─ Icons/
│  ├─ Meshes/
│  └─ Materials/
└─ Audio/
   ├─ SFX/
   └─ Music/
```

**Actions:**
- [ ] Create `Assets/Data/Items/` folder
- [ ] Create `Assets/Prefabs/Items/` folder
- [ ] Create `Assets/Prefabs/Environment/` folder
- [ ] Create `Assets/Prefabs/NPCs/` folder
- [ ] Create `Assets/Scripts/Items/` folder
- [ ] Create `Assets/Scripts/Environment/` folder
- [ ] Create `Assets/Scripts/Managers/` folder
- [ ] Create `Assets/Art/Icons/` folder
- [ ] Create `Assets/Art/Meshes/` folder
- [ ] Create `Assets/Art/Materials/` folder
- [ ] Create `Assets/Audio/SFX/` folder
- [ ] Create `Assets/Audio/Music/` folder

### Step 1.3: Setup Layers
Create custom layers for the game:

**Actions:**
- [ ] Open Edit > Project Settings > Tags and Layers
- [ ] Add Layer: "Items"
- [ ] Add Layer: "Environment"
- [ ] Add Layer: "Interactable"
- [ ] Add Layer: "NPC"
- [ ] Verify "Player" layer exists (should be default in TDE)

### Step 1.4: Setup Tags
Create custom tags:

**Actions:**
- [ ] Open Edit > Project Settings > Tags and Layers
- [ ] Add Tag: "Player" (if not exists)
- [ ] Add Tag: "NPC"
- [ ] Add Tag: "GAIA"
- [ ] Add Tag: "Tree"

---

## Phase 2: Art Assets & Audio

### Step 2.1: Import/Create Icon Sprites
Create or import 2D icon sprites for inventory:

**Actions:**
- [ ] Create/import `ScrapMetal_Icon.png` (64x64 or 128x128)
  - Image: Twisted metal debris, metallic colors
  - Save to: `Assets/Art/Icons/`
- [ ] Create/import `EnergyCrystal_Icon.png` (64x64 or 128x128)
  - Image: Glowing cyan crystal, faceted
  - Save to: `Assets/Art/Icons/`
- [ ] Create/import `AlienBerry_Icon.png` (64x64 or 128x128)
  - Image: Purple berry cluster, vibrant
  - Save to: `Assets/Art/Icons/`

**Unity Configuration:**
- [ ] Select each icon in Project window
- [ ] Inspector > Texture Type: "Sprite (2D and UI)"
- [ ] Inspector > Sprite Mode: "Single"
- [ ] Apply changes

### Step 2.2: Import/Create 3D Meshes
Create or import 3D models:

**Actions:**
- [ ] Create/import `ScrapMetal_Mesh.fbx`
  - Poly count: 50-200 triangles
  - Style: Twisted hull plating
  - Save to: `Assets/Art/Meshes/`
- [ ] Create/import `EnergyCrystal_Mesh.fbx`
  - Poly count: 100-300 triangles
  - Style: Faceted crystalline structure
  - Save to: `Assets/Art/Meshes/`
- [ ] Create/import `AlienBerry_Mesh.fbx`
  - Poly count: 50-150 triangles
  - Style: Cluster of 3-5 round berries
  - Save to: `Assets/Art/Meshes/`
- [ ] Create/import `Tree_Trunk_Mesh.fbx`
  - Poly count: 300-500 triangles
  - Style: Cylindrical trunk
  - Save to: `Assets/Art/Meshes/`
- [ ] Create/import `Tree_Foliage_Mesh.fbx`
  - Poly count: 200-400 triangles
  - Style: Stylized sphere or alien leaf cluster
  - Save to: `Assets/Art/Meshes/`
- [ ] Create/import `HologramProjector_Mesh.fbx`
  - Poly count: 50-100 triangles
  - Style: Flat tech disc/plate
  - Save to: `Assets/Art/Meshes/`
- [ ] Create/import `HologramBody_Mesh.fbx`
  - Poly count: 500-1000 triangles
  - Style: Humanoid silhouette or abstract AI shape
  - Save to: `Assets/Art/Meshes/`

### Step 2.3: Import/Create Audio Assets
Create or import sound effects:

**Actions:**
- [ ] Create/import `Metal_Drop_SFX.wav` → `Assets/Audio/SFX/`
- [ ] Create/import `Crystal_Chime_SFX.wav` → `Assets/Audio/SFX/`
- [ ] Create/import `Berry_Drop_SFX.wav` → `Assets/Audio/SFX/`
- [ ] Create/import `ScrapMetal_Pickup_SFX.wav` → `Assets/Audio/SFX/`
- [ ] Create/import `Crystal_Pickup_SFX.wav` → `Assets/Audio/SFX/`
- [ ] Create/import `Berry_Pickup_SFX.wav` → `Assets/Audio/SFX/`
- [ ] Create/import `Eat_Berry_SFX.wav` → `Assets/Audio/SFX/`
- [ ] Create/import `Tree_Shake_SFX.wav` → `Assets/Audio/SFX/`
- [ ] Create/import `Berry_Respawn_SFX.wav` → `Assets/Audio/SFX/`

**Unity Configuration:**
- [ ] Select each audio clip
- [ ] Inspector > Load Type: "Decompress On Load" (for short SFX)
- [ ] Apply changes

---

## Phase 3: Curved World Shader (CRITICAL)

**IMPORTANT:** This shader must be created before materials, as all materials will use it.

### Step 3.1: Create Curved World Shader Graph
Reference: See `plan-rough-draft.md` Section 5 for full details.

**Actions:**
- [ ] Right-click in `Assets/Shaders/` (create folder if needed)
- [ ] Create > Shader Graph > URP > Lit Shader Graph
- [ ] Name it: `CurvedWorldShader`
- [ ] Open Shader Graph editor

**Build the shader:**
- [ ] Add Property: "CurveAmount" (Float, default 0.005)
- [ ] Add Node: Position (World Space)
- [ ] Add Node: Camera Position (World Space)
- [ ] Add Node: Subtract (Position.z - Camera.z)
- [ ] Add Node: Multiply (Result × Result) - Square the distance
- [ ] Add Node: Multiply (Result × CurveAmount)
- [ ] Add Node: Multiply (Result × -1) - Negate for downward curve
- [ ] Add Node: Add (OriginalPosition.y + YOffset)
- [ ] Add Node: Combine (X, NewY, Z)
- [ ] Add Node: Transform (World to Object Space)
- [ ] Connect final result to Vertex Position output
- [ ] Save shader

**Test the shader:**
- [ ] Create test material with CurvedWorldShader
- [ ] Apply to a plane in a test scene
- [ ] Move camera back and forth (Z-axis)
- [ ] Verify plane curves downward as it gets farther from camera

### Step 3.2: Create Cel-Shading/Toon Shader (OPTIONAL)
For stylized "Wind Waker" look:

**Actions:**
- [ ] Create Shader Graph: `ToonShader`
- [ ] Implement N-step lighting ramp (see plan Section 5.3)
- [ ] Add outline pass (Inverted Hull method)
- [ ] Save shader

**Note:** This can be combined with CurvedWorldShader or applied separately.

---

## Phase 4: Materials

### Step 4.1: Create Item Materials

**ScrapMetal_Material:**
- [ ] Create Material in `Assets/Art/Materials/`
- [ ] Name: `ScrapMetal_Material`
- [ ] Shader: CurvedWorldShader (or Lit if shader not ready)
- [ ] Base Color: Metallic gray/brown
- [ ] Metallic: 0.8
- [ ] Smoothness: 0.3
- [ ] Assign texture if available
- [ ] **CRITICAL:** Set CurveAmount: 0.005

**EnergyCrystal_Material:**
- [ ] Create Material in `Assets/Art/Materials/`
- [ ] Name: `EnergyCrystal_Material`
- [ ] Shader: CurvedWorldShader
- [ ] Base Color: Light cyan (#00FFFF or similar)
- [ ] Metallic: 0.2
- [ ] Smoothness: 0.8 (shiny)
- [ ] Emission: Enabled
  - Emission Color: Cyan (#00FFFF)
  - Emission Intensity: 2.0
- [ ] **CRITICAL:** Set CurveAmount: 0.005

**AlienBerry_Material:**
- [ ] Create Material in `Assets/Art/Materials/`
- [ ] Name: `AlienBerry_Material`
- [ ] Shader: CurvedWorldShader (or ToonShader for cel-shading)
- [ ] Base Color: Vibrant purple (#8800FF or similar)
- [ ] Metallic: 0.1
- [ ] Smoothness: 0.6
- [ ] **CRITICAL:** Set CurveAmount: 0.005

### Step 4.2: Create Environment Materials

**Tree_Trunk_Material:**
- [ ] Create Material in `Assets/Art/Materials/`
- [ ] Name: `Tree_Trunk_Material`
- [ ] Shader: CurvedWorldShader
- [ ] Base Color: Brown/gray
- [ ] Apply bark texture if available
- [ ] Normal Map: Apply if available
- [ ] **CRITICAL:** Set CurveAmount: 0.005

**Tree_Foliage_Material:**
- [ ] Create Material in `Assets/Art/Materials/`
- [ ] Name: `Tree_Foliage_Material`
- [ ] Shader: CurvedWorldShader
- [ ] Base Color: Alien colors (purple, teal, orange - choose one)
- [ ] Two Sided: Enable
- [ ] **CRITICAL:** Set CurveAmount: 0.005

### Step 4.3: Create NPC Materials

**Hologram_Material:**
- [ ] Create Material in `Assets/Art/Materials/`
- [ ] Name: `Hologram_Material`
- [ ] Shader: Custom hologram shader (create if needed) OR use CurvedWorldShader with transparency
- [ ] Rendering Mode: Transparent
- [ ] Base Color: Cyan with alpha 0.5
- [ ] Emission: Cyan, intensity 1.5
- [ ] Add scanline effect if using custom shader
- [ ] **CRITICAL:** Also apply CurveAmount if possible

**HologramProjector_Material:**
- [ ] Create Material in `Assets/Art/Materials/`
- [ ] Name: `HologramProjector_Material`
- [ ] Shader: CurvedWorldShader
- [ ] Base Color: Dark metallic
- [ ] Metallic: 1.0
- [ ] Emission: Cyan trim (optional)
- [ ] **CRITICAL:** Set CurveAmount: 0.005

---

## Phase 5: Custom Scripts

### Step 5.1: Create AlienBerryItem Script

**Actions:**
- [ ] Create new C# script in `Assets/Scripts/Items/`
- [ ] Name: `AlienBerryItem.cs`
- [ ] Copy code from `plan-rough-draft.md` lines 262-302 OR write from scratch:

```csharp
using UnityEngine;
using MoreMountains.InventoryEngine;
using MoreMountains.TopDownEngine;

public class AlienBerryItem : InventoryItem
{
    [Header("Healing Properties")]
    public int StaminaRestored = 10;

    public override bool Use(string playerID)
    {
        // Find the player character
        Character targetCharacter = LevelManager.Instance.Players[0];

        if (targetCharacter != null)
        {
            // Get the Health component
            Health characterHealth = targetCharacter.GetComponent<Health>();

            if (characterHealth != null)
            {
                // Prevent over-healing
                if (characterHealth.CurrentHealth >= characterHealth.MaximumHealth)
                {
                    return false; // Action failed, item not consumed
                }

                // Apply Healing
                characterHealth.ReceiveHealth(StaminaRestored, null, targetCharacter.transform.position, "Consumption");

                return true; // Success: Item will be removed from inventory
            }
        }
        return false;
    }
}
```

- [ ] Save script
- [ ] Return to Unity, wait for compilation
- [ ] Verify no errors in Console

---

## Phase 6: ScriptableObjects (Items)

### Step 6.1: Create ScrapMetal.asset

**Actions:**
- [ ] Right-click in `Assets/Data/Items/`
- [ ] Create > TopDown Engine > Inventory Item
- [ ] Name: `ScrapMetal`

**Configure in Inspector:**
- [ ] ItemID: "ScrapMetal"
- [ ] ItemName: "Scrap Metal"
- [ ] ItemDescription: "Twisted hull plating from the crash site. Useful for basic fabrication."
- [ ] Icon: Drag `ScrapMetal_Icon` sprite here
- [ ] Prefab: (Leave empty for now - will link after prefab creation)
- [ ] Stackable: TRUE (check)
- [ ] MaximumStack: 30
- [ ] Usable: FALSE (uncheck)
- [ ] Equippable: FALSE (uncheck)
- [ ] TargetInventoryName: "MainInventory"
- [ ] ItemClass: "Resource"
- [ ] DropSound: Drag `Metal_Drop_SFX` here

### Step 6.2: Create EnergyCrystal.asset

**Actions:**
- [ ] Right-click in `Assets/Data/Items/`
- [ ] Create > TopDown Engine > Inventory Item
- [ ] Name: `EnergyCrystal`

**Configure in Inspector:**
- [ ] ItemID: "EnergyCrystal"
- [ ] ItemName: "Energy Crystal"
- [ ] ItemDescription: "A glowing crystalline structure. Pulses with exotic energy."
- [ ] Icon: Drag `EnergyCrystal_Icon` sprite here
- [ ] Prefab: (Leave empty for now)
- [ ] Stackable: TRUE
- [ ] MaximumStack: 10
- [ ] Usable: FALSE
- [ ] Equippable: FALSE
- [ ] TargetInventoryName: "MainInventory"
- [ ] ItemClass: "Resource"
- [ ] DropSound: Drag `Crystal_Chime_SFX` here

### Step 6.3: Create AlienBerry.asset

**Actions:**
- [ ] Right-click in `Assets/Data/Items/`
- [ ] Create > Create > AlienBerryItem (if available after script creation)
  - OR manually create and change script type in Inspector
- [ ] Name: `AlienBerry`

**Configure in Inspector:**
- [ ] ItemID: "AlienBerry"
- [ ] ItemName: "Alien Berry"
- [ ] ItemDescription: "A vibrant, sweet-smelling fruit. Restores stamina when consumed."
- [ ] Icon: Drag `AlienBerry_Icon` sprite here
- [ ] Prefab: (Leave empty for now)
- [ ] Stackable: TRUE
- [ ] MaximumStack: 10
- [ ] Usable: TRUE (CHECK THIS - important!)
- [ ] Equippable: FALSE
- [ ] TargetInventoryName: "MainInventory"
- [ ] ItemClass: "Consumable"
- [ ] UseSound: Drag `Eat_Berry_SFX` here
- [ ] DropSound: Drag `Berry_Drop_SFX` here

**AlienBerryItem-specific:**
- [ ] StaminaRestored: 10

**Verify Script Type:**
- [ ] Inspector > Script field shows "AlienBerryItem" (not just "InventoryItem")
- [ ] If wrong, manually change MonoScript reference

---

## Phase 7: Prefabs - ItemPickers

### Step 7.1: Create ScrapMetalPicker.prefab

**Actions:**
- [ ] Hierarchy > Right-click > Create Empty
- [ ] Name: `ScrapMetalPicker`
- [ ] Transform > Reset (Position 0,0,0)

**Add Components to Root:**
- [ ] Add Component > TopDown Engine > Items > ItemPicker
  - Item: Drag `ScrapMetal` asset here
  - Quantity: 1
  - DisableObjectWhenDepleted: TRUE
  - DisableDelay: 0.1
  - TriggerMode: Auto
  - RequireButtonPress: FALSE
  - PickupDistance: 2.0
  - RespawnEnabled: FALSE
  - PickupSound: `ScrapMetal_Pickup_SFX`

- [ ] Add Component > BoxCollider
  - IsTrigger: TRUE (CHECK THIS)
  - Center: (0, 0.5, 0)
  - Size: (0.8, 0.8, 0.8)

- [ ] Layer: Set to "Items"

**Create Visual Child:**
- [ ] Right-click ScrapMetalPicker > Create Empty
- [ ] Name: `Visual`
- [ ] Add Component > MeshFilter
  - Mesh: `ScrapMetal_Mesh`
- [ ] Add Component > MeshRenderer
  - Material: `ScrapMetal_Material`

**Save as Prefab:**
- [ ] Drag `ScrapMetalPicker` from Hierarchy to `Assets/Prefabs/Items/`
- [ ] Delete from Hierarchy
- [ ] Open `ScrapMetal.asset` in Inspector
- [ ] Prefab field: Drag `ScrapMetalPicker.prefab` here
- [ ] Apply changes

### Step 7.2: Create EnergyCrystalPicker.prefab

**Actions:**
- [ ] Hierarchy > Create Empty
- [ ] Name: `EnergyCrystalPicker`
- [ ] Transform > Reset
- [ ] Scale: (0.8, 0.8, 0.8)

**Add Components to Root:**
- [ ] Add Component > ItemPicker
  - Item: `EnergyCrystal` asset
  - Quantity: 1
  - DisableObjectWhenDepleted: TRUE
  - TriggerMode: Auto
  - RespawnEnabled: FALSE
  - PickupSound: `Crystal_Pickup_SFX`

- [ ] Add Component > CapsuleCollider
  - IsTrigger: TRUE
  - Center: (0, 0.6, 0)
  - Radius: 0.4
  - Height: 1.2
  - Direction: Y-Axis

- [ ] Layer: "Items"

**Create Visual Child:**
- [ ] Right-click > Create Empty > Name: `Visual`
- [ ] Position: (0, 1.2, 0) - floats above ground
- [ ] Add Component > MeshFilter: `EnergyCrystal_Mesh`
- [ ] Add Component > MeshRenderer: `EnergyCrystal_Material`

**OPTIONAL - Create Particle Effect Child:**
- [ ] Right-click > Create > Particle System
- [ ] Name: `ParticleEffect`
- [ ] Position: (0, 0.5, 0)
- [ ] Configure Particle System:
  - Looping: TRUE
  - Start Lifetime: 1.5-2.5
  - Start Speed: 0.5
  - Start Size: 0.05-0.15
  - Start Color: Cyan with alpha 0.6
  - Gravity Modifier: -0.2
  - Emission Rate: 5
  - Shape: Sphere, radius 0.3

**OPTIONAL - Create Point Light Child:**
- [ ] Right-click > Light > Point Light
- [ ] Name: `PointLight`
- [ ] Position: (0, 0.5, 0)
- [ ] Configure:
  - Color: Cyan
  - Intensity: 1.5
  - Range: 3.0
  - Shadows: None
  - Render Mode: Important

**Save as Prefab:**
- [ ] Drag to `Assets/Prefabs/Items/EnergyCrystalPicker.prefab`
- [ ] Delete from Hierarchy
- [ ] Link to `EnergyCrystal.asset` Prefab field

### Step 7.3: Create AlienBerryPicker.prefab

**Actions:**
- [ ] Hierarchy > Create Empty
- [ ] Name: `AlienBerryPicker`
- [ ] Transform > Reset
- [ ] Scale: (0.6, 0.6, 0.6)

**Add Components to Root:**
- [ ] Add Component > ItemPicker
  - Item: `AlienBerry` asset
  - Quantity: 1
  - DisableObjectWhenDepleted: TRUE
  - TriggerMode: Auto
  - RespawnEnabled: TRUE (CHECK THIS)
  - RespawnTime: 300.0 (5 minutes)
  - PickupSound: `Berry_Pickup_SFX`
  - RespawnFeedback: `Berry_Respawn_SFX`

- [ ] Add Component > SphereCollider
  - IsTrigger: TRUE
  - Center: (0, 0.3, 0)
  - Radius: 0.35

- [ ] Layer: "Items"

**Create Visual Child:**
- [ ] Right-click > Create Empty > Name: `Visual`
- [ ] Add Component > MeshFilter: `AlienBerry_Mesh`
- [ ] Add Component > MeshRenderer: `AlienBerry_Material`

**Save as Prefab:**
- [ ] Drag to `Assets/Prefabs/Items/AlienBerryPicker.prefab`
- [ ] Delete from Hierarchy
- [ ] Link to `AlienBerry.asset` Prefab field

---

## Phase 8: Prefabs - Environment (Tree)

### Step 8.1: Create TreePrefab.prefab

**Actions:**
- [ ] Hierarchy > Create Empty
- [ ] Name: `TreePrefab`
- [ ] Transform > Reset
- [ ] Layer: "Environment"
- [ ] Tag: "Tree"

**Create Trunk Child:**
- [ ] Right-click TreePrefab > Create Empty
- [ ] Name: `Trunk`
- [ ] Position: (0, 0, 0)
- [ ] Add Component > MeshFilter: `Tree_Trunk_Mesh`
- [ ] Add Component > MeshRenderer: `Tree_Trunk_Material`

**Create Foliage Child:**
- [ ] Right-click TreePrefab > Create Empty
- [ ] Name: `Foliage`
- [ ] Position: (0, 2.5, 0) - above trunk
- [ ] Add Component > MeshFilter: `Tree_Foliage_Mesh`
- [ ] Add Component > MeshRenderer: `Tree_Foliage_Material`

**Create TreeCollider Child:**
- [ ] Right-click TreePrefab > Create Empty
- [ ] Name: `TreeCollider`
- [ ] Position: (0, 1.5, 0)
- [ ] Add Component > BoxCollider
  - IsTrigger: FALSE (solid collision)
  - Center: (0, 0, 0)
  - Size: (0.8, 3.0, 0.8)

**Create TreeShakeZone Child:**
- [ ] Right-click TreePrefab > Create Empty
- [ ] Name: `TreeShakeZone`
- [ ] Position: (0, 1.0, 0)
- [ ] Layer: "Interactable"

**Add Components to TreeShakeZone:**

**SphereCollider:**
- [ ] Add Component > SphereCollider
  - IsTrigger: TRUE
  - Center: (0, 0, 0)
  - Radius: 1.5

**ButtonActivated:**
- [ ] Add Component > TopDown Engine > Interactable > ButtonActivated
  - CanActivate: TRUE
  - ButtonA: TRUE
  - ActivationMode: OnButtonPress
  - RequireColliderStay: TRUE
  - AutoActivation: FALSE
  - ShowPrompt: TRUE
  - PromptText: "Press [A] to Shake Tree"
  - PromptPrefab: (None for now - create InteractionPrompt.prefab separately if needed)
  - NumberOfActivations: -1 (unlimited)
  - ActivationCooldown: 1.0
  - Activators: "Player" tag

**Loot:**
- [ ] Add Component > TopDown Engine > Loot > Loot
  - GameObjectsToLoot: (Size: 3)
    - [0] Loot Definition:
      - GameObject: `ScrapMetalPicker.prefab`
      - Quantity: 3-5 (min 3, max 5)
      - Weight: 60
      - Delay: 0.0
    - [1] Loot Definition:
      - GameObject: `AlienBerryPicker.prefab`
      - Quantity: 1-2
      - Weight: 30
      - Delay: 0.0
    - [2] Loot Definition:
      - GameObject: `EnergyCrystalPicker.prefab`
      - Quantity: 1
      - Weight: 10
      - Delay: 0.0

  - SpawnMode: Random
  - NumberOfDrops: 1
  - SpawnOffset: (0, 2.5, 0)
  - SpawnRadius: 1.0
  - ApplyForce: TRUE
    - ForceMode: Impulse
    - ForceMagnitude: 3.0
    - ForceDirection: (0, 1, 0) with randomness

  - LootRespawnEnabled: TRUE
  - LootRespawnTime: 180.0 (3 minutes)

**OPTIONAL - Add MMFeedbacks for Tree Shake Animation:**
- [ ] Add Component > More Mountains > Feedbacks > MM Feedbacks
- [ ] Add Feedback: MMF_Scale (target: Foliage)
  - Animate shake effect
- [ ] Add Feedback: MMF_Rotation (target: Trunk)
  - Slight sway
- [ ] Add Feedback: MMF_Sound
  - Sound: `Tree_Shake_SFX`
- [ ] Add Feedback: MMF_ParticlesInstantiation
  - Spawn falling leaves

**Link MMFeedbacks to ButtonActivated:**
- [ ] ButtonActivated > ActivationFeedback: Assign MMFeedbacks component

**Save as Prefab:**
- [ ] Drag `TreePrefab` to `Assets/Prefabs/Environment/TreePrefab.prefab`
- [ ] Delete from Hierarchy

---

## Phase 9: Prefabs - NPCs (G.A.I.A.)

### Step 9.1: Create GAIA_NPC.prefab

**Actions:**
- [ ] Hierarchy > Create Empty
- [ ] Name: `GAIA_NPC`
- [ ] Transform > Reset
- [ ] Rotation: (0, 180, 0) - faces player spawn
- [ ] Layer: "NPC"
- [ ] Tag: "GAIA"

**Create Visual Parent Child:**
- [ ] Right-click GAIA_NPC > Create Empty
- [ ] Name: `Visual`

**Create HologramProjector Grandchild:**
- [ ] Right-click Visual > Create Empty
- [ ] Name: `HologramProjector`
- [ ] Position: (0, 0, 0)
- [ ] Scale: (0.5, 0.1, 0.5)
- [ ] Add Component > MeshFilter: `HologramProjector_Mesh`
- [ ] Add Component > MeshRenderer: `HologramProjector_Material`

**Create HologramBody Grandchild:**
- [ ] Right-click Visual > Create Empty
- [ ] Name: `HologramBody`
- [ ] Position: (0, 1.2, 0)
- [ ] Add Component > MeshFilter: `HologramBody_Mesh`
- [ ] Add Component > MeshRenderer: `Hologram_Material`
- [ ] Add Component > Animator
  - Controller: (Create `GAIA_Animator` controller - see below)

### Step 9.2: Create GAIA_Animator Controller

**Actions:**
- [ ] Right-click in `Assets/Animations/` (create folder if needed)
- [ ] Create > Animator Controller
- [ ] Name: `GAIA_Animator`
- [ ] Double-click to open Animator window

**Create Idle Animation:**
- [ ] Right-click in `Assets/Animations/` > Create > Animation
- [ ] Name: `GAIA_Idle_Float`
- [ ] Open Animation window
- [ ] Add property: Transform > Position.Y
- [ ] Keyframe 0s: Y = 1.2
- [ ] Keyframe 1s: Y = 1.5 (move up 0.3 units)
- [ ] Keyframe 2s: Y = 1.2 (move back down)
- [ ] Set to loop

**Configure Animator:**
- [ ] In Animator window, create state: "Idle"
- [ ] Assign `GAIA_Idle_Float` animation to Idle state
- [ ] Set Idle as default state (orange)
- [ ] OPTIONAL: Create "Talking" state with different animation
  - Add parameter: Bool "IsTalking"
  - Transition: Idle → Talking when IsTalking = true
  - Transition: Talking → Idle when IsTalking = false

**Link Animator:**
- [ ] Select HologramBody in Hierarchy
- [ ] Animator > Controller: `GAIA_Animator`

### Step 9.3: Create DialogueZone Child

**Actions:**
- [ ] Right-click GAIA_NPC > Create Empty
- [ ] Name: `DialogueZone`
- [ ] Position: (0, 1.0, 0)
- [ ] Layer: "Interactable"

**Add Components to DialogueZone:**

**BoxCollider:**
- [ ] Add Component > BoxCollider
  - IsTrigger: TRUE
  - Center: (0, 0, 0)
  - Size: (2.5, 2.0, 2.5)

**ButtonActivated:**
- [ ] Add Component > ButtonActivated
  - CanActivate: TRUE
  - ButtonA: TRUE
  - ActivationMode: OnButtonPress
  - RequireColliderStay: TRUE
  - ShowPrompt: TRUE
  - PromptText: "Press [A] to Talk to G.A.I.A."
  - NumberOfActivations: -1
  - ActivationCooldown: 0.5
  - Activators: "Player" tag

**DialogueTrigger (If using Dialogue System for Unity):**
- [ ] Add Component > Pixel Crushers > Dialogue System > Triggers > Dialogue System Trigger
  - Trigger: OnUse
  - Conversation: "GAIA_Prologue_Conversation"
  - Once: FALSE

**OR Custom Dialogue Component:**
- [ ] Create custom script `GAIADialogueManager.cs`
- [ ] Add to DialogueZone
- [ ] Reference dialogue data asset

### Step 9.4: Create NameDisplay Child (OPTIONAL)

**Actions:**
- [ ] Right-click GAIA_NPC > UI > Text - TextMeshPro
- [ ] Name: `NameDisplay`
- [ ] Position: (0, 2.5, 0)
- [ ] Configure RectTransform:
  - Width: 200
  - Height: 50
- [ ] Configure TextMeshPro:
  - Text: "G.A.I.A."
  - Font Size: 0.5
  - Color: Cyan
  - Alignment: Center
  - Auto Size: FALSE
- [ ] Add Component > Billboard (Custom script to face camera)
  - OR manually rotate to face camera

**Save as Prefab:**
- [ ] Drag `GAIA_NPC` to `Assets/Prefabs/NPCs/GAIA_NPC.prefab`
- [ ] Delete from Hierarchy

---

## Phase 10: Dialogue System Setup

### Step 10.1: Choose Dialogue System

**Option A: Use Dialogue System for Unity (Asset Store)**
- [ ] Import Dialogue System for Unity from Asset Store
- [ ] Create Dialogue Database
- [ ] Create conversation: "GAIA_Prologue_Conversation"
- [ ] Write all dialogue lines (see ASSET_CREATION_GUIDE.md dialogue section)

**Option B: Custom Dialogue System**
- [ ] Create dialogue data structure (JSON or ScriptableObject)
- [ ] Create dialogue UI prefabs
- [ ] Create dialogue manager script
- [ ] Write dialogue content

### Step 10.2: Write Dialogue Content

Reference: See `ASSET_CREATION_GUIDE.md` G.A.I.A. Dialogue Content section for all lines.

**Day 0 Conversations to create:**
- [ ] Initial Meeting (Lines 1-4)
- [ ] Post-Habitat Deployment (Lines 5-7)
- [ ] Resource Turn-In (Lines 8-10)
- [ ] System Reboot Ceremony (Lines 11-13)

**Day 1 Conversations:**
- [ ] Morning Briefing (Lines 14-16)
- [ ] Post-Scanning (Lines 17-20)
- [ ] General Dialogue (Repeatable lines)

### Step 10.3: Create Dialogue UI Prefabs

**Actions:**
- [ ] Create Canvas in scene
- [ ] Create DialogueBox UI panel
  - Background image
  - Speaker name text
  - Dialogue text
  - Continue button/indicator
- [ ] Create InteractionPrompt UI
  - "Press [A] to..." text
  - Icon/sprite
- [ ] Save as prefabs in `Assets/Prefabs/UI/`

---

## Phase 11: Testing in Test Scene

### Step 11.1: Create Test Scene

**Actions:**
- [ ] Create new scene: `TestScene`
- [ ] Add simple plane for ground (10x10 scale)
- [ ] Add lighting (Directional Light)
- [ ] Add TopDown Engine player prefab (from TDE demos)
- [ ] Add MainCamera with Cinemachine (if not on player)

### Step 11.2: Test ItemPickers

**Actions:**
- [ ] Drag `ScrapMetalPicker.prefab` into scene
- [ ] Enter Play mode
- [ ] Walk player over ScrapMetalPicker
- [ ] VERIFY: Item disappears, pickup sound plays, inventory updates
- [ ] Exit Play mode
- [ ] Repeat for EnergyCrystalPicker
- [ ] Repeat for AlienBerryPicker
- [ ] Wait 5 minutes (or adjust time)
- [ ] VERIFY: AlienBerryPicker respawns

### Step 11.3: Test AlienBerry Consumption

**Actions:**
- [ ] Pick up AlienBerry
- [ ] Open inventory (default key: I or Tab)
- [ ] Reduce player health manually (optional)
- [ ] Select AlienBerry
- [ ] Click "Use"
- [ ] VERIFY: Health increases by 10, berry consumed, sound plays

### Step 11.4: Test Tree Shaking

**Actions:**
- [ ] Drag `TreePrefab.prefab` into scene
- [ ] Enter Play mode
- [ ] Walk near tree
- [ ] VERIFY: Interaction prompt appears
- [ ] Press interact button (default: A or E)
- [ ] VERIFY: Tree shakes, loot spawns, sound plays
- [ ] VERIFY: Loot flies away from tree with physics
- [ ] Pick up loot
- [ ] Try shaking tree again immediately
- [ ] VERIFY: Cooldown prevents spam (1 second wait)
- [ ] Shake tree multiple times, observe loot variety
- [ ] VERIFY: ScrapMetal drops most often, EnergyCrystal is rare
- [ ] Wait 3 minutes (or adjust time)
- [ ] VERIFY: Tree can drop loot again

### Step 11.5: Test G.A.I.A. NPC

**Actions:**
- [ ] Drag `GAIA_NPC.prefab` into scene
- [ ] Enter Play mode
- [ ] VERIFY: Hologram floats up and down (animation)
- [ ] Walk near G.A.I.A.
- [ ] VERIFY: Dialogue prompt appears
- [ ] Press interact button
- [ ] VERIFY: Dialogue box opens
- [ ] Read through dialogue lines
- [ ] VERIFY: Can progress through conversation
- [ ] Exit dialogue
- [ ] Walk away, return
- [ ] VERIFY: Can talk to G.A.I.A. again

---

## Phase 12: Quest System Integration

### Step 12.1: Create Quest Manager (Optional)

If not using a third-party quest system:

**Actions:**
- [ ] Create script: `Assets/Scripts/Managers/PrologueManager.cs`
- [ ] Implement quest tracking logic:
  - Track current quest stage
  - Monitor inventory for quest items
  - Trigger dialogue based on progress
  - Award items on quest completion
- [ ] Create GameObject in scene: `PrologueManager`
- [ ] Attach script

### Step 12.2: Link Quests to Dialogue

**Actions:**
- [ ] In dialogue system, add quest triggers:
  - After "Deploy Habitat" dialogue → Give HabKit item
  - After "Gather Resources" dialogue → Start tracking
  - When 10 ScrapMetal + 6 EnergyCrystal collected → Enable turn-in dialogue
  - After turn-in → Remove items, trigger planet naming
  - After planet naming → Give DataPad, advance day
- [ ] Test full quest flow in test scene

---

## Phase 13: Main Game Scene Setup

### Step 13.1: Create GameScene

**Actions:**
- [ ] Create new scene: `GameScene`
- [ ] Save to: `Assets/Scenes/GameScene.unity`

### Step 13.2: Build Terrain

**Actions:**
- [ ] Create Terrain GameObject (or use modular ground planes)
- [ ] Apply CurvedWorldShader material to terrain/ground
- [ ] Design crash site area (meadow, landing zone)
- [ ] Add skybox (space/alien sky)

### Step 13.3: Place Initial Assets

**Actions:**
- [ ] Place player spawn point (0, 1, 0 or similar)
- [ ] Place G.A.I.A. NPC near spawn (5 units away, facing spawn)
- [ ] Scatter 15-20 ScrapMetalPicker around crash site
- [ ] Place 10-15 EnergyCrystalPicker on "Crystal Spires" (create simple rock meshes)
- [ ] Place 5-10 AlienBerryPicker on bushes/ground
- [ ] Place 8-12 TreePrefab around the area

### Step 13.4: Setup Player

**Actions:**
- [ ] Add TopDown Engine Player prefab (from demos or custom)
- [ ] Configure CharacterMovement component (see plan Table 2):
  - WalkSpeed: 5.5
  - RunSpeed: 8.5
  - Acceleration: 12-15
  - Deceleration: 8-10
- [ ] Add CharacterInventory ability
  - Target Inventory: "MainInventory"
- [ ] Create MainInventory GameObject in scene
  - Add Inventory component (Inventory Engine)
  - InventoryName: "MainInventory"
  - NumberOfSlots: 20
  - Persistent: TRUE

### Step 13.5: Setup Camera

**Actions:**
- [ ] Add Cinemachine Virtual Camera (or use TDE camera)
- [ ] Configure (see plan Section 3.3):
  - Body: Transposer
  - Follow Offset: (0, 10, -7)
  - FOV: 30-40
  - LookAt: Player
- [ ] Test camera follows player with curved world effect

---

## Phase 14: Final Testing & Polish

### Step 14.1: Full Playthrough Test

**Actions:**
- [ ] Start GameScene from beginning
- [ ] Test Day 0 sequence:
  - [ ] Talk to G.A.I.A. (initial meeting)
  - [ ] Receive HabKit (placeholder item for now)
  - [ ] "Place" habitat (simulate)
  - [ ] Collect 10 ScrapMetal
  - [ ] Collect 6 EnergyCrystal
  - [ ] Return to G.A.I.A., turn in items
  - [ ] Name planet
  - [ ] Receive DataPad
  - [ ] Advance to Day 1
- [ ] Test Day 1 sequence:
  - [ ] Morning briefing dialogue
  - [ ] Scan lifeforms (placeholder for now)
  - [ ] Dr. Hoot transmission
  - [ ] Receive BioLabMarkerKit
  - [ ] Place marker

### Step 14.2: Performance Check

**Actions:**
- [ ] Enter Play mode
- [ ] Open Profiler (Window > Analysis > Profiler)
- [ ] Check FPS (target: 60+ on target platform)
- [ ] Check draw calls (minimize as needed)
- [ ] Optimize particle effects if laggy
- [ ] Check memory usage

### Step 14.3: Bug Fixing

**Actions:**
- [ ] Create bug tracking list (spreadsheet or task manager)
- [ ] Test edge cases:
  - [ ] Picking up item with full inventory
  - [ ] Using berry at max health
  - [ ] Shaking empty tree
  - [ ] Talking to G.A.I.A. during dialogue
  - [ ] Rapid interaction spam
- [ ] Fix any issues found

### Step 14.4: Audio Polish

**Actions:**
- [ ] Verify all sounds play correctly
- [ ] Adjust audio volume levels for balance
- [ ] Add background music (if available)
- [ ] Test spatial audio for 3D sounds

### Step 14.5: Visual Polish

**Actions:**
- [ ] Verify Curved World Shader on all objects
- [ ] Check material consistency (colors, lighting)
- [ ] Add particle effects where appropriate
- [ ] Test lighting in different times of day (if applicable)

---

## Phase 15: Documentation & Handoff

### Step 15.1: Document Settings

**Actions:**
- [ ] Screenshot all key component settings
- [ ] Note any custom values used
- [ ] Document any deviations from the guide

### Step 15.2: Create Asset Inventory

**Actions:**
- [ ] List all created assets:
  - [ ] 3 ScriptableObjects (items)
  - [ ] 3 ItemPicker prefabs
  - [ ] 1 Tree prefab
  - [ ] 1 G.A.I.A. NPC prefab
  - [ ] All materials, meshes, icons, audio
- [ ] Verify all assets are in correct folders

### Step 15.3: Backup Project

**Actions:**
- [ ] Create project backup (zip entire Unity project)
- [ ] Commit to version control (Git) if using
- [ ] Tag commit as "MVP_Assets_Complete"

---

## Completion Checklist

### ScriptableObjects
- [ ] ScrapMetal.asset created and configured
- [ ] EnergyCrystal.asset created and configured
- [ ] AlienBerry.asset created and configured
- [ ] All linked to prefabs

### Prefabs - Items
- [ ] ScrapMetalPicker.prefab created and functional
- [ ] EnergyCrystalPicker.prefab created with particles/light
- [ ] AlienBerryPicker.prefab created with respawn

### Prefabs - Environment
- [ ] TreePrefab.prefab created with full hierarchy
- [ ] Tree shake interaction working
- [ ] Loot table configured correctly
- [ ] Tree loot respawn working

### Prefabs - NPCs
- [ ] GAIA_NPC.prefab created with visual hierarchy
- [ ] Hologram shader/material applied
- [ ] Float animation working
- [ ] Dialogue system integrated
- [ ] All dialogue written and functional

### Scripts
- [ ] AlienBerryItem.cs created and compiling
- [ ] Health restoration working
- [ ] No console errors

### Art Assets
- [ ] All icons imported
- [ ] All meshes imported
- [ ] All materials created with Curved World Shader
- [ ] Hologram shader created

### Audio Assets
- [ ] All SFX imported and referenced
- [ ] Sounds playing correctly

### Systems
- [ ] Inventory system functional
- [ ] Quest tracking working (if implemented)
- [ ] Dialogue system working
- [ ] Save/load tested (if implemented)

### Testing
- [ ] All items pickable
- [ ] All items stack correctly
- [ ] Berry consumption heals player
- [ ] Tree shake drops loot
- [ ] Loot respawns correctly
- [ ] Dialogue progresses correctly
- [ ] No game-breaking bugs

### Performance
- [ ] 60+ FPS on target platform
- [ ] No major lag spikes
- [ ] Curved World effect working smoothly

---

## Troubleshooting Common Issues

### Items Not Picking Up
- [ ] Check ItemPicker collider is set to IsTrigger = TRUE
- [ ] Verify player has "Player" tag
- [ ] Check TriggerMode is set to "Auto"
- [ ] Ensure item is on "Items" layer

### Berry Not Healing
- [ ] Verify AlienBerry.asset uses AlienBerryItem script (not base InventoryItem)
- [ ] Check "Usable" checkbox is enabled
- [ ] Verify StaminaRestored value is set (e.g., 10)
- [ ] Check player has Health component

### Tree Not Shaking
- [ ] Verify ButtonActivated CanActivate is TRUE
- [ ] Check player is within SphereCollider radius (1.5)
- [ ] Verify cooldown hasn't blocked interaction
- [ ] Check "Player" tag is in Activators list

### Loot Not Spawning from Tree
- [ ] Verify Loot component has GameObjectsToLoot array filled
- [ ] Check prefab references are valid
- [ ] Verify NumberOfDrops is at least 1
- [ ] Check LootRespawnEnabled and wait for respawn timer

### Dialogue Not Opening
- [ ] Verify DialogueZone collider is trigger
- [ ] Check ButtonActivated is configured
- [ ] Verify dialogue conversation asset exists
- [ ] Check player is within BoxCollider bounds

### Curved World Effect Not Working
- [ ] Verify all materials use CurvedWorldShader
- [ ] Check CurveAmount property is set (0.005)
- [ ] Ensure shader is connected to Vertex Position output
- [ ] Test with simple object first (cube/plane)

---

## Next Steps After Completion

Once all assets are created and tested:

1. **Integrate into Full Game Loop**
   - Connect to main menu scene
   - Implement save/load functionality
   - Add persistent world state

2. **Expand Content**
   - Create more item types
   - Add more NPC dialogues
   - Build out Day 2+ progression

3. **Polish**
   - Add more visual effects
   - Improve animations
   - Enhance audio design

4. **Playtest**
   - Get external feedback
   - Iterate on balance
   - Fix bugs

---

**Congratulations!** If you've completed all steps, you now have a fully functional asset suite for Galactic Crossing's MVP. All items, interactions, and systems should be operational and ready for integration into the larger game.

**Last Updated:** 2026-02-15
**Version:** 1.0 (MVP)
