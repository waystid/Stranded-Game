# Galactic Crossing - Asset Creation Guide
## Unity Editor Implementation Documentation

This guide provides complete specifications for creating all ScriptableObjects and Prefabs required for the Galactic Crossing MVP. Since ScriptableObjects and Prefabs cannot be created via scripts, this document serves as the authoritative reference for manual Unity Editor implementation.

---

## Table of Contents
1. [ScriptableObject Documentation](#scriptableobject-documentation)
2. [ItemPicker Prefab Documentation](#itempicker-prefab-documentation)
3. [Tree Prefab Documentation](#tree-prefab-documentation)
4. [G.A.I.A. NPC Prefab Documentation](#gaia-npc-prefab-documentation)
5. [Implementation Checklist](#implementation-checklist)

---

## ScriptableObject Documentation

### Location
All item ScriptableObjects should be created in:
```
Assets/Data/Items/
```

### 1. ScrapMetal.asset

**Type:** InventoryItem (Base Inventory Engine class)

**Configuration:**
```
Item Identity:
├─ ItemID: "ScrapMetal"
├─ ItemName: "Scrap Metal"
└─ ItemDescription: "Twisted hull plating from the crash site. Useful for basic fabrication."

Stacking:
├─ Stackable: TRUE
└─ MaximumStack: 30

Visual:
├─ Icon: [Sprite reference to ScrapMetal_Icon.png]
└─ Prefab: [Reference to ScrapMetalPicker.prefab]

Properties:
├─ Usable: FALSE (Cannot be consumed directly)
├─ Equippable: FALSE
├─ TargetInventoryName: "MainInventory"
└─ ItemClass: "Resource"

Audio:
├─ UseSound: (None)
├─ EquipSound: (None)
└─ DropSound: "Metal_Drop_SFX"
```

**Creation Steps:**
1. Right-click in `Assets/Data/Items/`
2. Create > TopDown Engine > Inventory Item
3. Name it `ScrapMetal`
4. Configure all fields as specified above
5. Assign the icon sprite from `Assets/Art/Icons/ScrapMetal_Icon.png`
6. Link to the ItemPicker prefab (created later)

---

### 2. EnergyCrystal.asset

**Type:** InventoryItem

**Configuration:**
```
Item Identity:
├─ ItemID: "EnergyCrystal"
├─ ItemName: "Energy Crystal"
└─ ItemDescription: "A glowing crystalline structure. Pulses with exotic energy."

Stacking:
├─ Stackable: TRUE
└─ MaximumStack: 10

Visual:
├─ Icon: [Sprite reference to EnergyCrystal_Icon.png]
└─ Prefab: [Reference to EnergyCrystalPicker.prefab]

Properties:
├─ Usable: FALSE
├─ Equippable: FALSE
├─ TargetInventoryName: "MainInventory"
└─ ItemClass: "Resource"

Audio:
├─ UseSound: (None)
├─ EquipSound: (None)
└─ DropSound: "Crystal_Chime_SFX"
```

**Creation Steps:**
1. Right-click in `Assets/Data/Items/`
2. Create > TopDown Engine > Inventory Item
3. Name it `EnergyCrystal`
4. Configure all fields as specified above
5. Assign the icon sprite
6. Link to the ItemPicker prefab

---

### 3. AlienBerry.asset

**Type:** AlienBerryItem (Custom script - extends InventoryItem)

**Important:** This requires the custom `AlienBerryItem.cs` script (see plan-rough-draft.md lines 262-302).

**Configuration:**
```
Item Identity:
├─ ItemID: "AlienBerry"
├─ ItemName: "Alien Berry"
└─ ItemDescription: "A vibrant, sweet-smelling fruit. Restores stamina when consumed."

Stacking:
├─ Stackable: TRUE
└─ MaximumStack: 10

Visual:
├─ Icon: [Sprite reference to AlienBerry_Icon.png]
└─ Prefab: [Reference to AlienBerryPicker.prefab]

Properties:
├─ Usable: TRUE
├─ Equippable: FALSE
├─ TargetInventoryName: "MainInventory"
└─ ItemClass: "Consumable"

Custom Properties (AlienBerryItem):
└─ StaminaRestored: 10

Audio:
├─ UseSound: "Eat_Berry_SFX"
├─ EquipSound: (None)
└─ DropSound: "Berry_Drop_SFX"
```

**Creation Steps:**
1. **First**, ensure `AlienBerryItem.cs` script exists in `Assets/Scripts/Items/`
2. Right-click in `Assets/Data/Items/`
3. Create > Galactic Crossing > Alien Berry Item (or manually create with custom script type)
4. Name it `AlienBerry`
5. Configure all fields as specified
6. Set StaminaRestored to 10
7. Link to the ItemPicker prefab

---

## ItemPicker Prefab Documentation

### Location
All ItemPicker prefabs should be created in:
```
Assets/Prefabs/Items/
```

ItemPickers are 3D objects that exist in the game world and can be collected by the player.

### 1. ScrapMetalPicker.prefab

**Hierarchy:**
```
ScrapMetalPicker (Root)
├─ Visual (Child GameObject)
│  └─ Component: MeshFilter (ScrapMetal_Mesh.fbx)
│  └─ Component: MeshRenderer (ScrapMetal_Material)
└─ (No additional children)
```

**Root GameObject Components:**

**Transform:**
```
Position: (0, 0, 0)
Rotation: (0, 0, 0)
Scale: (1, 1, 1)
```

**ItemPicker (TopDown Engine Component):**
```
Item:
└─ Item: [Reference to ScrapMetal.asset]

Picker Settings:
├─ Quantity: 1
├─ DisableObjectWhenDepleted: TRUE
├─ DisableDelay: 0.1

Trigger Settings:
├─ TriggerMode: "Auto" (Picks up on overlap)
├─ RequireButtonPress: FALSE
└─ PickupDistance: 2.0

Respawn:
├─ RespawnEnabled: FALSE
└─ RespawnTime: 0

Feedback:
├─ PickupFeedback: [Reference to MMFeedbacks component or none]
└─ PickupSound: "ScrapMetal_Pickup_SFX"
```

**BoxCollider (or SphereCollider):**
```
Is Trigger: TRUE
Center: (0, 0.5, 0)
Size: (0.8, 0.8, 0.8) for BoxCollider
  OR
Radius: 0.5 for SphereCollider
```

**Layer:**
```
Layer: "Items" (Create this layer if it doesn't exist)
```

**Visual Settings:**
- Mesh: Use a low-poly twisted metal piece (50-200 triangles)
- Material: Metallic PBR with rusted/scorched texture
- Should use the Curved World Shader (see plan Section 5)

**Creation Steps:**
1. Create Empty GameObject, name it `ScrapMetalPicker`
2. Add ItemPicker component
3. Configure all ItemPicker settings
4. Add BoxCollider, set as trigger
5. Create child GameObject named "Visual"
6. Add MeshFilter and MeshRenderer to Visual child
7. Assign mesh and material with Curved World Shader
8. Set layer to "Items"
9. Save as prefab in `Assets/Prefabs/Items/`
10. Link this prefab back to ScrapMetal.asset's Prefab field

---

### 2. EnergyCrystalPicker.prefab

**Hierarchy:**
```
EnergyCrystalPicker (Root)
├─ Visual (Child GameObject)
│  └─ Component: MeshFilter (EnergyCrystal_Mesh.fbx)
│  └─ Component: MeshRenderer (EnergyCrystal_Material)
├─ ParticleEffect (Child GameObject - OPTIONAL)
│  └─ Component: Particle System (Glowing particles)
└─ PointLight (Child GameObject - OPTIONAL)
   └─ Component: Light (Cyan/Blue glow)
```

**Root GameObject Components:**

**Transform:**
```
Position: (0, 0, 0)
Rotation: (0, 0, 0)
Scale: (0.8, 0.8, 0.8) - Slightly smaller than scrap
```

**ItemPicker:**
```
Item:
└─ Item: [Reference to EnergyCrystal.asset]

Picker Settings:
├─ Quantity: 1
├─ DisableObjectWhenDepleted: TRUE
├─ DisableDelay: 0.1

Trigger Settings:
├─ TriggerMode: "Auto"
├─ RequireButtonPress: FALSE
└─ PickupDistance: 2.0

Respawn:
├─ RespawnEnabled: FALSE

Feedback:
└─ PickupSound: "Crystal_Pickup_SFX"
```

**CapsuleCollider:**
```
Is Trigger: TRUE
Center: (0, 0.6, 0)
Radius: 0.4
Height: 1.2
Direction: Y-Axis
```

**Layer:**
```
Layer: "Items"
```

**Visual Settings:**
- Mesh: Crystalline geometry with faceted faces
- Material: Emissive shader with cyan/blue glow
  - Base Color: Light cyan
  - Emission: Cyan (intensity 2.0)
  - Must use Curved World Shader
- Optional: Add subtle rotation animation (rotate Y-axis 30 degrees/second)

**Optional Particle Effect:**
```
ParticleEffect GameObject:
├─ Position: (0, 0.5, 0)
└─ Particle System:
   ├─ Duration: Looping
   ├─ Start Lifetime: 1.5-2.5
   ├─ Start Speed: 0.5
   ├─ Start Size: 0.05-0.15
   ├─ Start Color: Cyan with alpha 0.6
   ├─ Gravity Modifier: -0.2 (floats up)
   ├─ Emission Rate: 5
   └─ Shape: Sphere, radius 0.3
```

**Optional Point Light:**
```
PointLight GameObject:
├─ Position: (0, 0.5, 0)
└─ Light Component:
   ├─ Type: Point
   ├─ Color: Cyan
   ├─ Intensity: 1.5
   ├─ Range: 3.0
   ├─ Shadows: None (performance)
   └─ Render Mode: Important
```

---

### 3. AlienBerryPicker.prefab

**Hierarchy:**
```
AlienBerryPicker (Root)
├─ Visual (Child GameObject)
│  └─ Component: MeshFilter (AlienBerry_Mesh.fbx)
│  └─ Component: MeshRenderer (AlienBerry_Material)
└─ (No additional children)
```

**Root GameObject Components:**

**Transform:**
```
Position: (0, 0, 0)
Rotation: (0, 0, 0)
Scale: (0.6, 0.6, 0.6) - Smallest of the three
```

**ItemPicker:**
```
Item:
└─ Item: [Reference to AlienBerry.asset]

Picker Settings:
├─ Quantity: 1
├─ DisableObjectWhenDepleted: TRUE
├─ DisableDelay: 0.1

Trigger Settings:
├─ TriggerMode: "Auto"
├─ RequireButtonPress: FALSE
└─ PickupDistance: 2.0

Respawn:
├─ RespawnEnabled: TRUE (Berries respawn!)
├─ RespawnTime: 300.0 (5 minutes)
└─ RespawnFeedback: "Berry_Respawn_SFX"

Feedback:
└─ PickupSound: "Berry_Pickup_SFX"
```

**SphereCollider:**
```
Is Trigger: TRUE
Center: (0, 0.3, 0)
Radius: 0.35
```

**Layer:**
```
Layer: "Items"
```

**Visual Settings:**
- Mesh: Round berry cluster (3-5 berries together)
- Material: Cel-shaded with bright purple/magenta color
  - Base Color: Vibrant purple
  - Smoothness: 0.6 (slight shine)
  - Must use Curved World Shader

---

## Tree Prefab Documentation

### Location
```
Assets/Prefabs/Environment/
```

The Tree prefab is a harvestable environmental object that drops loot when shaken.

### TreePrefab.prefab

**Hierarchy:**
```
TreePrefab (Root)
├─ Trunk (Child GameObject)
│  └─ Component: MeshFilter (Tree_Trunk_Mesh.fbx)
│  └─ Component: MeshRenderer (Tree_Trunk_Material)
├─ Foliage (Child GameObject)
│  └─ Component: MeshFilter (Tree_Foliage_Mesh.fbx)
│  └─ Component: MeshRenderer (Tree_Foliage_Material)
├─ TreeCollider (Child GameObject)
│  └─ Component: BoxCollider (Tree body collision)
└─ TreeShakeZone (Child GameObject)
   ├─ Component: SphereCollider (Trigger for interaction)
   ├─ Component: ButtonActivated (TopDown Engine)
   └─ Component: Loot (TopDown Engine)
```

**Root GameObject:**

**Transform:**
```
Position: (0, 0, 0)
Rotation: (0, 0, 0)
Scale: (1, 1, 1)
```

**Layer:**
```
Layer: "Environment"
```

---

**Trunk Child:**

**Transform:**
```
Local Position: (0, 0, 0)
Local Rotation: (0, 0, 0)
Local Scale: (1, 1, 1)
```

**Visual:**
- Mesh: Cylindrical trunk (300-500 triangles)
- Material: Bark texture with Curved World Shader
  - Color: Brown/gray
  - Normal map for bark detail

---

**Foliage Child:**

**Transform:**
```
Local Position: (0, 2.5, 0) - Offset upward
Local Rotation: (0, 0, 0)
Local Scale: (1, 1, 1)
```

**Visual:**
- Mesh: Low-poly sphere or stylized foliage (200-400 triangles)
- Material: Leaf texture with Curved World Shader
  - Color: Alien colors (purple, teal, orange - stylized)
  - Two-sided rendering: TRUE
  - Alpha clipping if using leaf cards

---

**TreeCollider Child:**

**Transform:**
```
Local Position: (0, 1.5, 0)
Local Rotation: (0, 0, 0)
Local Scale: (1, 1, 1)
```

**BoxCollider:**
```
Is Trigger: FALSE (Solid collision)
Center: (0, 0, 0)
Size: (0.8, 3.0, 0.8) - Matches trunk height
```

**Purpose:** Prevents player from walking through the tree trunk.

---

**TreeShakeZone Child:**

**Transform:**
```
Local Position: (0, 1.0, 0)
Local Rotation: (0, 0, 0)
Local Scale: (1, 1, 1)
```

**SphereCollider:**
```
Is Trigger: TRUE
Center: (0, 0, 0)
Radius: 1.5
```

**ButtonActivated Component (TopDown Engine):**
```
Activation Settings:
├─ CanActivate: TRUE
├─ ButtonA: TRUE (Interact button)
├─ ActivationMode: "OnButtonPress"
├─ RequireColliderStay: TRUE (Must be near tree)
└─ AutoActivation: FALSE

Visual Feedback:
├─ ShowPrompt: TRUE
├─ PromptText: "Press [A] to Shake Tree"
└─ PromptPrefab: [Reference to InteractionPrompt.prefab]

Activation Limitations:
├─ NumberOfActivations: -1 (Unlimited)
├─ ActivationCooldown: 1.0 (1 second between shakes)
└─ Activators: "Player" tag only
```

**Loot Component (TopDown Engine):**
```
Loot Table:
└─ GameObjectsToLoot: (Array of LootDefinitions)
   ├─ [0] Loot Definition:
   │  ├─ GameObject: [Reference to ScrapMetalPicker.prefab]
   │  ├─ Quantity: 3-5 (Random range)
   │  ├─ Weight: 60 (60% chance category)
   │  └─ Delay: 0.0
   ├─ [1] Loot Definition:
   │  ├─ GameObject: [Reference to AlienBerryPicker.prefab]
   │  ├─ Quantity: 1-2 (Random range)
   │  ├─ Weight: 30 (30% chance)
   │  └─ Delay: 0.0
   └─ [2] Loot Definition:
      ├─ GameObject: [Reference to EnergyCrystalPicker.prefab]
      ├─ Quantity: 1
      ├─ Weight: 10 (10% chance - rare!)
      └─ Delay: 0.0

Spawn Settings:
├─ SpawnMode: "Random" (Randomize what drops)
├─ NumberOfDrops: 1 (One item type per shake)
├─ SpawnOffset: (0, 2.5, 0) (Spawn above foliage)
├─ SpawnRadius: 1.0 (Items fall around tree)
└─ ApplyForce: TRUE (Items ejected with physics)
   ├─ ForceMode: "Impulse"
   ├─ ForceMagnitude: 3.0
   └─ ForceDirection: "Up and Random Horizontal"

Respawn:
├─ LootRespawnEnabled: TRUE
├─ LootRespawnTime: 180.0 (3 minutes - trees refill)
└─ LootRespawnFeedback: (None)

Feedback:
├─ LootFeedback: [Reference to MMFeedbacks]
│  └─ Contains: Shake animation, leaf particles, rustle sound
└─ NoLootFeedback: [Reference to MMFeedbacks]
   └─ Contains: "Tree is empty" sound/visual
```

**Animation (Optional but Recommended):**
- Use MMFeedbacks on TreeShakeZone to trigger:
  - MMF_Scale: Shake the Foliage object (scale wobble)
  - MMF_Rotation: Slight trunk sway
  - MMF_ParticlesInstantiation: Spawn falling leaves particle effect
  - MMF_Sound: Play "Tree_Shake_SFX"

**Layer:**
```
TreeShakeZone Layer: "Interactable"
```

---

## G.A.I.A. NPC Prefab Documentation

### Location
```
Assets/Prefabs/NPCs/
```

G.A.I.A. (Global Artificial Intelligence Assistant) is the primary NPC and quest-giver for the prologue.

### GAIA_NPC.prefab

**Hierarchy:**
```
GAIA_NPC (Root)
├─ Visual (Child GameObject)
│  ├─ HologramProjector (Grandchild - Static mesh)
│  │  └─ Component: MeshFilter, MeshRenderer
│  └─ HologramBody (Grandchild - Animated)
│     ├─ Component: MeshFilter, MeshRenderer (Hologram shader)
│     └─ Component: Animator (Idle float animation)
├─ DialogueZone (Child GameObject)
│  ├─ Component: BoxCollider (Trigger)
│  ├─ Component: ButtonActivated (TopDown Engine)
│  └─ Component: DialogueTrigger (Custom or third-party)
└─ NameDisplay (Child GameObject - OPTIONAL)
   └─ Component: TextMeshPro (World space "G.A.I.A.")
```

**Root GameObject:**

**Transform:**
```
Position: (0, 0, 0) - Place in world as needed
Rotation: (0, 180, 0) - Facing player spawn
Scale: (1, 1, 1)
```

**Layer:**
```
Layer: "NPC"
```

**Tag:**
```
Tag: "NPC" or "GAIA"
```

---

**Visual/HologramProjector:**

**Transform:**
```
Local Position: (0, 0, 0)
Local Rotation: (0, 0, 0)
Local Scale: (0.5, 0.1, 0.5)
```

**Visual:**
- Mesh: Flat disc or tech base (projector plate)
- Material: Metallic tech material
  - Emissive cyan trim
  - Must use Curved World Shader

---

**Visual/HologramBody:**

**Transform:**
```
Local Position: (0, 1.2, 0) - Floats above projector
Local Rotation: (0, 0, 0)
Local Scale: (1, 1, 1)
```

**Visual:**
- Mesh: Humanoid silhouette or abstract AI shape
  - Keep poly count low (500-1000 triangles)
- Material: **Hologram Shader** (Custom or asset)
  - Transparent, glowing cyan/blue
  - Scanline effect (horizontal scrolling lines)
  - Fresnel glow on edges
  - Flickering/noise animation
  - Must also apply Curved World Shader displacement

**Animator:**
```
Controller: GAIA_Animator
├─ Idle State:
│  └─ Animation: Gentle float up/down (0.3 units over 2 seconds)
├─ Talking State (OPTIONAL):
│  └─ Animation: Slight head tilt, gesture loops
└─ Parameters:
   └─ Bool: "IsTalking"
```

---

**DialogueZone Child:**

**Transform:**
```
Local Position: (0, 1.0, 0)
Local Rotation: (0, 0, 0)
Local Scale: (1, 1, 1)
```

**BoxCollider:**
```
Is Trigger: TRUE
Center: (0, 0, 0)
Size: (2.5, 2.0, 2.5) - Generous interaction zone
```

**ButtonActivated Component:**
```
Activation Settings:
├─ CanActivate: TRUE
├─ ButtonA: TRUE
├─ ActivationMode: "OnButtonPress"
├─ RequireColliderStay: TRUE
└─ AutoActivation: FALSE

Visual Feedback:
├─ ShowPrompt: TRUE
├─ PromptText: "Press [A] to Talk to G.A.I.A."
└─ PromptPrefab: [Reference to InteractionPrompt.prefab]

Activation Limitations:
├─ NumberOfActivations: -1 (Unlimited)
├─ ActivationCooldown: 0.5
└─ Activators: "Player" tag
```

**DialogueTrigger Component (If using Dialogue System):**

If using **Dialogue System for Unity** (Pixel Crushers):
```
Conversation: "GAIA_Prologue_Conversation"
Once: FALSE (Can talk multiple times)
Condition: (Based on quest state)
```

If using **custom dialogue system**:
- Create a `GAIADialogueManager.cs` script component
- Reference dialogue data ScriptableObject or JSON file

---

### G.A.I.A. Dialogue Content

**Conversation ID:** "GAIA_Prologue_Conversation"

**Dialogue Tree Structure:**

```
Initial Meeting (First conversation - Day 0):
├─ Line 1 (GAIA):
│  "Welcome to the colonization initiative... er, emergency landing protocol."
│  "Please verify your bio-metrics."
│  [Triggers character creator if not done]
├─ Line 2 (GAIA):
│  "Systems check complete. You're fortunate, [PlayerName]."
│  "The crash site is stable, and the planetary environment is... cozy."
├─ Line 3 (GAIA):
│  "I am G.A.I.A., your Global Artificial Intelligence Assistant."
│  "I'll guide you through establishing our temporary base."
└─ Line 4 (GAIA):
   "First priority: Deploy your Pop-Up Habitat Module."
   "You'll find the Hab Kit in your inventory. Select it and choose a location."
   [Gives player HabKit item]
   [Quest Trigger: "Deploy Habitat"]

Post-Habitat Deployment:
├─ Line 5 (GAIA):
│  "Excellent work! Your hab is operational."
│  "Next, we need to calibrate the atmospheric scrubbers."
├─ Line 6 (GAIA):
│  "I'm detecting Carbon Scoring debris from the crash."
│  "Collect 10 pieces of Scrap Metal scattered around the landing zone."
└─ Line 7 (GAIA):
   "I also need Energy Crystals to jumpstart the maintenance drones."
   "Harvest 6 crystals from the local flora."
   [Quest Trigger: "Gather Resources"]
   [Quest Objectives: ScrapMetal 0/10, EnergyCrystal 0/6]

Resource Turn-In (When player has items):
├─ Line 8 (GAIA):
│  "You've collected the materials! Impressive efficiency, [PlayerName]."
│  "Let me recalibrate the systems..."
│  [Takes items from inventory]
├─ Line 9 (GAIA):
│  "Scrubbers online. Drones charging. We're making progress."
│  "Tonight, we'll hold a System Reboot Ceremony."
└─ Line 10 (GAIA):
   "But first, we should name this planet. What shall we call our new home?"
   [Opens planet naming UI]
   [Saves planet name]

System Reboot Ceremony:
├─ Line 11 (GAIA):
│  "Welcome to [PlanetName], [PlayerName]."
│  "Your first day as Resident Coordinator begins now."
├─ Line 12 (GAIA):
│  "I've uploaded the Colony OS to your datapad."
│  "You can track your Bio-Data achievements and access the Fabricator."
│  [Gives DataPad item/unlocks UI]
└─ Line 13 (GAIA):
   "Rest for tonight. Tomorrow, we begin the real work."
   [Fades to black, advances to Day 1]

Day 1 - Morning Briefing:
├─ Line 14 (GAIA):
│  "Good morning, [PlayerName]. Systems are stable."
│  "I've detected unknown biological signatures in the area."
├─ Line 15 (GAIA):
│  "For planetary safety assessment, I need scans of 5 unique lifeforms."
│  "Use your Holo-Scanner to capture specimens."
│  [Quest Trigger: "Scan Lifeforms"]
└─ Line 16 (GAIA):
   "Bring the scan data back to me when you're ready."

Post-Scanning (5 lifeforms donated):
├─ Line 17 (GAIA):
│  "Fascinating data, [PlayerName]. These organisms are... docile."
│  "Wait—I'm receiving a transmission from orbit."
├─ Line 18 (GAIA):
│  "It's Dr. Hoot, a Xeno-Biologist from the Science Division!"
│  "He's requesting permission to establish a Bio-Lab on [PlanetName]."
├─ Line 19 (GAIA):
│  "This is excellent news! A museum of xenobiology will attract researchers."
│  "Here's the Bio-Lab Marker Kit. Choose a suitable location for construction."
│  [Gives BioLabMarkerKit item]
└─ Line 20 (GAIA):
   "Place the marker, and Dr. Hoot will begin construction tomorrow."
   [Quest Trigger: "Place Museum Plot"]

General Dialogue (Repeatable):
├─ "How can I assist you today, [PlayerName]?"
├─ "[PlanetName] is looking better every day."
├─ "Don't forget to check your Bio-Data achievements!"
└─ "The drones are running smoothly. Thank you for the Energy Crystals."
```

**Dialogue Presentation:**
- Text appears in dialogue box UI (bottom or top of screen)
- G.A.I.A.'s hologram plays talking animation
- Cyan text color for G.A.I.A.'s name
- Optional: Robotic/synthetic voice synthesis audio
- Player can advance dialogue with Interact button

---

**NameDisplay Child (OPTIONAL):**

**Transform:**
```
Local Position: (0, 2.5, 0) - Above hologram
Local Rotation: (0, 0, 0)
Local Scale: (1, 1, 1)
```

**TextMeshPro Component:**
```
Text: "G.A.I.A."
Font: Sci-fi style font
Font Size: 0.5 (World space)
Color: Cyan
Alignment: Center
Billboard: TRUE (Always faces camera)
```

---

## Implementation Checklist

Use this checklist to track asset creation progress:

### ScriptableObjects
- [ ] Create `Assets/Data/Items/` folder
- [ ] Create `ScrapMetal.asset` and configure all properties
- [ ] Create `EnergyCrystal.asset` and configure all properties
- [ ] Ensure `AlienBerryItem.cs` script exists
- [ ] Create `AlienBerry.asset` and configure all properties

### ItemPicker Prefabs
- [ ] Create `Assets/Prefabs/Items/` folder
- [ ] Create `ScrapMetalPicker.prefab` with all components
- [ ] Create `EnergyCrystalPicker.prefab` with particle effects
- [ ] Create `AlienBerryPicker.prefab` with respawn settings
- [ ] Link all picker prefabs to their ScriptableObject assets
- [ ] Test pickup functionality in a test scene

### Environment Prefabs
- [ ] Create `Assets/Prefabs/Environment/` folder
- [ ] Create `TreePrefab.prefab` with hierarchy
- [ ] Configure TreeShakeZone with ButtonActivated component
- [ ] Configure Loot component with loot table
- [ ] Create tree shake MMFeedbacks (animation/sound)
- [ ] Test tree shake and loot drop in test scene

### NPC Prefabs
- [ ] Create `Assets/Prefabs/NPCs/` folder
- [ ] Create `GAIA_NPC.prefab` with visual hierarchy
- [ ] Create hologram shader (or import from asset store)
- [ ] Create GAIA_Animator controller with idle float
- [ ] Configure DialogueZone with ButtonActivated
- [ ] Set up dialogue system integration
- [ ] Create dialogue conversation asset
- [ ] Write all dialogue lines (see dialogue content above)
- [ ] Test dialogue triggering in test scene

### Audio Assets
- [ ] Create/import "Metal_Drop_SFX"
- [ ] Create/import "Crystal_Chime_SFX"
- [ ] Create/import "Berry_Drop_SFX"
- [ ] Create/import "ScrapMetal_Pickup_SFX"
- [ ] Create/import "Crystal_Pickup_SFX"
- [ ] Create/import "Berry_Pickup_SFX"
- [ ] Create/import "Eat_Berry_SFX"
- [ ] Create/import "Tree_Shake_SFX"
- [ ] Create/import "Berry_Respawn_SFX"

### Visual Assets
- [ ] Create icon sprites (64x64 or 128x128):
  - [ ] ScrapMetal_Icon.png
  - [ ] EnergyCrystal_Icon.png
  - [ ] AlienBerry_Icon.png
- [ ] Create/import 3D meshes:
  - [ ] ScrapMetal_Mesh.fbx
  - [ ] EnergyCrystal_Mesh.fbx
  - [ ] AlienBerry_Mesh.fbx
  - [ ] Tree_Trunk_Mesh.fbx
  - [ ] Tree_Foliage_Mesh.fbx
  - [ ] HologramProjector_Mesh.fbx
  - [ ] HologramBody_Mesh.fbx
- [ ] Create materials with Curved World Shader:
  - [ ] ScrapMetal_Material
  - [ ] EnergyCrystal_Material (with emission)
  - [ ] AlienBerry_Material
  - [ ] Tree_Trunk_Material
  - [ ] Tree_Foliage_Material
  - [ ] Hologram_Material (custom shader)

### Particle Effects
- [ ] Create EnergyCrystal glow particles
- [ ] Create tree shake falling leaves particles
- [ ] Create hologram flicker particles (optional)

### UI Prefabs
- [ ] Create InteractionPrompt.prefab (shows button prompts)
- [ ] Create DialogueBox.prefab (for conversations)

### Testing
- [ ] Test all items can be picked up
- [ ] Test all items stack correctly in inventory
- [ ] Test AlienBerry consumption restores health
- [ ] Test tree shaking drops loot
- [ ] Test tree loot respawns after 3 minutes
- [ ] Test berry respawns after 5 minutes
- [ ] Test G.A.I.A. dialogue triggers correctly
- [ ] Test all dialogue branches and quest progression
- [ ] Verify all curved world shaders working
- [ ] Verify all audio plays correctly

---

## Additional Notes

### Layer Setup
Ensure these layers exist in your project:
1. **Items** - For all ItemPicker objects
2. **Environment** - For trees, rocks, etc.
3. **Interactable** - For DialogueZones, ButtonActivated triggers
4. **NPC** - For G.A.I.A. and future NPCs

### Curved World Shader Integration
All visual meshes MUST use the Curved World Shader (see plan-rough-draft.md Section 5) to maintain the "Rolling Log" aesthetic. This applies to:
- All ItemPicker visual children
- Tree trunk and foliage
- G.A.I.A. hologram (applied after hologram transparency shader)
- Terrain
- All environment objects

### Performance Considerations
- Keep particle effects minimal (5-10 particles max per object)
- Use simple colliders (sphere/box/capsule, avoid mesh colliders)
- Limit lights to essential objects only (EnergyCrystal, maybe G.A.I.A.)
- Keep mesh poly counts low for pickups (under 500 triangles)

### Inventory Engine Configuration
Make sure the Inventory Engine is properly initialized:
1. A `MainInventory` exists in the scene (attached to player or GameManager)
2. Capacity is set to 20 slots (as per plan)
3. Persistence is enabled
4. Inventory GUI is linked to the DataPad system

### Quest System Integration
If using a custom quest system:
1. Create quest data for:
   - "Deploy Habitat"
   - "Gather Resources" (ScrapMetal 0/10, EnergyCrystal 0/6)
   - "Scan Lifeforms" (0/5)
   - "Place Museum Plot"
2. Quest progress should update when:
   - Items are added to inventory
   - Dialogue choices are made
   - Objects are placed

---

## Conclusion

This document provides all necessary specifications for implementing the core assets of Galactic Crossing's MVP. Follow each section carefully, referring to the main plan-rough-draft.md for deeper technical context on systems like the Curved World Shader, Character Controller, and AI behaviors.

All assets must be created manually in the Unity Editor—there are no automated generation scripts for ScriptableObjects or Prefabs. Use this guide as your authoritative reference during implementation.

For questions about specific TopDown Engine components, refer to:
- TopDown Engine Documentation: https://topdown-engine-docs.moremountains.com/
- Inventory Engine Documentation: https://inventory-engine-docs.moremountains.com/

Good luck building Galactic Crossing!
