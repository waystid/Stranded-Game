# Galactic Crossing - Asset Relationships Diagram

This document visualizes how all assets relate to each other in the Galactic Crossing MVP.

---

## Asset Relationship Overview

```
PLAYER CHARACTER
      │
      ├─────────────────────────────────────────┐
      │                                         │
      ▼                                         ▼
MAIN INVENTORY (20 Slots)              INTERACTION SYSTEM
      │                                         │
      │                                         │
      ├──────┬──────┬──────┐                   ├──────┬──────┐
      ▼      ▼      ▼      ▼                   ▼      ▼      ▼
   ScrapM  EnergC  AlienB  Other          TreeShake  GAIA  Other
   etal    rystal  erry    Items          Zone      Dlg    NPCs
      │      │      │                        │       │
      │      │      │                        │       │
      │      │      └─[USE]─────────────────►│       │
      │      │         Restores Health       │       │
      │      │                                │       │
      │      │      ┌────────────────────────┘       │
      │      │      │                                │
      │      │      │  LOOT TABLE                    │
      │      │      │                                │
      │      │      ├─► 60% ScrapMetalPicker         │
      │      │      ├─► 30% AlienBerryPicker         │
      │      │      └─► 10% EnergyCrystalPicker      │
      │      │                                        │
      └──────┴──────► [QUEST TRACKING] ◄─────────────┘
                            │
                            │
                     ┌──────┴──────┐
                     ▼             ▼
              Day 0 Quests   Day 1 Quests
```

---

## ScriptableObject → Prefab Relationships

### ScrapMetal Flow
```
ScrapMetal.asset (ScriptableObject)
      │
      │ [Referenced By]
      │
      ├─► ScrapMetalPicker.prefab
      │         │
      │         ├─ ItemPicker Component (Item: ScrapMetal.asset)
      │         ├─ BoxCollider (Trigger)
      │         └─ Visual (Mesh + Material)
      │
      │ [Spawned By]
      │
      ├─► TreePrefab.prefab (Loot Component)
      │         └─ Loot Entry: ScrapMetalPicker @ 60% weight
      │
      │ [Used In]
      │
      └─► Quest: "Gather Resources"
                └─ Requirement: 10x ScrapMetal
```

### EnergyCrystal Flow
```
EnergyCrystal.asset (ScriptableObject)
      │
      │ [Referenced By]
      │
      ├─► EnergyCrystalPicker.prefab
      │         │
      │         ├─ ItemPicker Component (Item: EnergyCrystal.asset)
      │         ├─ CapsuleCollider (Trigger)
      │         ├─ Visual (Emissive Mesh + Material)
      │         ├─ ParticleSystem (Cyan glow particles)
      │         └─ PointLight (Cyan glow)
      │
      │ [Spawned By]
      │
      ├─► TreePrefab.prefab (Loot Component)
      │         └─ Loot Entry: EnergyCrystalPicker @ 10% weight (RARE)
      │
      │ [Placed In World]
      │
      ├─► Crystal Spire Harvestables (Manual placement)
      │
      │ [Used In]
      │
      └─► Quest: "Gather Resources"
                └─ Requirement: 6x EnergyCrystal
```

### AlienBerry Flow
```
AlienBerry.asset (ScriptableObject)
      │
      │ [Uses Custom Script]
      │
      ├─► AlienBerryItem.cs
      │         └─ Use() method → ReceiveHealth(10)
      │
      │ [Referenced By]
      │
      ├─► AlienBerryPicker.prefab
      │         │
      │         ├─ ItemPicker Component (Item: AlienBerry.asset)
      │         │    └─ RespawnEnabled: TRUE (300s)
      │         ├─ SphereCollider (Trigger)
      │         └─ Visual (Purple berry mesh)
      │
      │ [Spawned By]
      │
      ├─► TreePrefab.prefab (Loot Component)
      │         └─ Loot Entry: AlienBerryPicker @ 30% weight
      │
      │ [Used By Player]
      │
      └─► Inventory → [Use] → Health Component
                              └─ CurrentHealth += 10
```

---

## Tree Prefab Component Hierarchy

```
TreePrefab
      │
      ├─── Trunk (Visual Child)
      │      └─ MeshRenderer (Bark Material + Curved World Shader)
      │
      ├─── Foliage (Visual Child)
      │      └─ MeshRenderer (Leaf Material + Curved World Shader)
      │
      ├─── TreeCollider (Collision Child)
      │      └─ BoxCollider (IsTrigger: FALSE)
      │            └─ [Prevents player from walking through trunk]
      │
      └─── TreeShakeZone (Interaction Child)
             │
             ├─ SphereCollider (IsTrigger: TRUE, Radius: 1.5)
             │    └─ [Detection zone for player interaction]
             │
             ├─ ButtonActivated Component
             │    │
             │    ├─ CanActivate: TRUE
             │    ├─ ButtonA: TRUE (Interact button)
             │    ├─ ActivationCooldown: 1.0 second
             │    └─ ShowPrompt: "Press [A] to Shake Tree"
             │
             └─ Loot Component
                  │
                  ├─ GameObjectsToLoot[]:
                  │    ├─ [0] ScrapMetalPicker (Qty: 3-5, Weight: 60)
                  │    ├─ [1] AlienBerryPicker (Qty: 1-2, Weight: 30)
                  │    └─ [2] EnergyCrystalPicker (Qty: 1, Weight: 10)
                  │
                  ├─ SpawnMode: Random
                  ├─ NumberOfDrops: 1 (One type per shake)
                  ├─ SpawnOffset: (0, 2.5, 0) - Above foliage
                  ├─ ApplyForce: TRUE (Ejects loot)
                  │    └─ ForceMagnitude: 3.0
                  │
                  └─ LootRespawn: TRUE (180 seconds)
```

---

## G.A.I.A. NPC Component Hierarchy

```
GAIA_NPC
      │
      ├─── Visual (Child)
      │      │
      │      ├─── HologramProjector (Grandchild)
      │      │      └─ MeshRenderer (Tech disc, emissive material)
      │      │
      │      └─── HologramBody (Grandchild)
      │             ├─ MeshRenderer (Hologram shader + Curved World)
      │             └─ Animator (GAIA_Animator Controller)
      │                   └─ Idle State: Float animation (Y: ±0.3, 2s cycle)
      │
      ├─── DialogueZone (Child)
      │      │
      │      ├─ BoxCollider (IsTrigger: TRUE, Size: 2.5x2.0x2.5)
      │      │    └─ [Detection zone for player dialogue]
      │      │
      │      ├─ ButtonActivated Component
      │      │    │
      │      │    ├─ CanActivate: TRUE
      │      │    ├─ ButtonA: TRUE
      │      │    ├─ RequireColliderStay: TRUE
      │      │    └─ ShowPrompt: "Press [A] to Talk to G.A.I.A."
      │      │
      │      └─ DialogueTrigger Component
      │             │
      │             └─ Conversation: "GAIA_Prologue_Conversation"
      │                   │
      │                   └─ [Links to dialogue tree data]
      │
      └─── NameDisplay (Optional Child)
             └─ TextMeshPro (World Space)
                   └─ Text: "G.A.I.A." (Cyan, billboard)
```

---

## Dialogue → Quest → Item Flow

### Day 0: Crash Landing Sequence

```
┌─────────────────────────────────────────────────────────────────┐
│ GAIA_PROLOGUE_CONVERSATION (Dialogue Tree)                      │
└─────────────────────────────────────────────────────────────────┘
      │
      │ [Initial Meeting]
      ├─► Line 1-4: Introduction, lore
      │
      │ [Quest Trigger: "Deploy Habitat"]
      ├─► GiveItem(HabKit) → Player Inventory
      │        └─ [Player places HabKit in world]
      │               └─ Quest Complete
      │
      │ [Quest Trigger: "Gather Resources"]
      ├─► Quest Objectives:
      │      ├─ Collect ScrapMetal (0/10)
      │      │     └─ Pick up ScrapMetalPicker x10
      │      │
      │      └─ Collect EnergyCrystal (0/6)
      │            └─ Pick up EnergyCrystalPicker x6
      │                  OR Harvest from Crystal Spires
      │
      │ [Resource Turn-In Dialogue]
      ├─► CheckInventory(ScrapMetal, 10) AND
      │   CheckInventory(EnergyCrystal, 6)
      │        └─ If TRUE: RemoveItems() → Quest Complete
      │
      │ [Name Planet Dialogue]
      ├─► OpenUI(PlanetNamingInput)
      │        └─ SaveToFile(PlanetName.data)
      │
      │ [System Reboot Ceremony]
      ├─► GiveItem(DataPad) → Unlock Colony OS UI
      │        └─ AdvanceDay(Day 1)
      │
      └─► [End of Day 0]
```

### Day 1: Museum Setup Sequence

```
┌─────────────────────────────────────────────────────────────────┐
│ GAIA_PROLOGUE_CONVERSATION (Continued)                          │
└─────────────────────────────────────────────────────────────────┘
      │
      │ [Morning Briefing]
      ├─► Line 14-16: Introduce bio-scanning
      │
      │ [Quest Trigger: "Scan Lifeforms"]
      ├─► Quest Objectives:
      │      └─ Scan 5 unique lifeforms (0/5)
      │            ├─ Use HoloScanner tool
      │            ├─ Target: Space Beetles (AI creatures)
      │            └─ Target: Float-Fish (AI creatures)
      │
      │ [Donation Dialogue]
      ├─► CheckDonations(UniqueSpecies, 5)
      │        └─ If TRUE: TriggerTransmission(DrHoot)
      │
      │ [Dr. Hoot Transmission]
      ├─► Line 17-20: Museum introduction
      │
      │ [Quest Trigger: "Place Museum Plot"]
      └─► GiveItem(BioLabMarkerKit) → Player Inventory
               └─ [Player places marker in world]
                      └─ Quest Complete
                             └─ [Museum construction starts next day]
```

---

## Inventory System Data Flow

```
┌──────────────────────────────────────────────────────────────┐
│ PLAYER CHARACTER                                             │
│   └─ CharacterInventory (Ability Component)                 │
│         └─ TargetInventory: "MainInventory"                  │
└──────────────────────────────────────────────────────────────┘
                           │
                           │ [References]
                           ▼
┌──────────────────────────────────────────────────────────────┐
│ MAIN INVENTORY (GameObject in Scene)                         │
│   └─ Inventory Component (InventoryEngine)                  │
│         ├─ InventoryName: "MainInventory"                    │
│         ├─ NumberOfSlots: 20                                 │
│         ├─ Persistent: TRUE                                  │
│         └─ Content[]: (Array of InventoryItems)              │
└──────────────────────────────────────────────────────────────┘
                           │
                           │ [Stores]
                           │
            ┌──────────────┼──────────────┐
            ▼              ▼              ▼
      ScrapMetal    EnergyCrystal    AlienBerry
      Qty: 0-30     Qty: 0-10        Qty: 0-10
            │              │              │
            │              │              └─► [Usable]
            │              │                    │
            │              │                    ▼
            │              │           Use() → ReceiveHealth(10)
            │              │                    │
            │              │                    └─► Health Component
            │              │                          └─ CurrentHealth += 10
            │              │
            └──────────────┴──────────────► [Quest Requirements]
                                                    │
                                                    └─► PrologueManager
                                                          └─ CheckQuestProgress()
```

---

## Item Pickup Event Flow

```
PLAYER MOVEMENT
      │
      │ [Enters Trigger]
      ▼
ItemPicker Collider (IsTrigger: TRUE)
      │
      │ [OnTriggerEnter]
      ▼
ItemPicker Component
      │
      ├─ CheckConditions()
      │    ├─ TriggerMode == Auto? → TRUE
      │    └─ Player tag? → TRUE
      │
      ├─ PlayFeedback(PickupSound)
      │
      ├─ AddItemToInventory(Item, Quantity)
      │         │
      │         └─► MainInventory.AddItem(Item)
      │                   │
      │                   ├─ Stack if stackable
      │                   ├─ Update UI
      │                   └─ Fire InventoryChangeEvent
      │
      └─ DisableObject(DisableDelay: 0.1s)
             │
             └─ If RespawnEnabled == TRUE:
                   └─ StartCoroutine(RespawnAfter(RespawnTime))
```

---

## Tree Shake Interaction Flow

```
PLAYER MOVEMENT
      │
      │ [Enters Trigger]
      ▼
TreeShakeZone (SphereCollider, IsTrigger: TRUE)
      │
      │ [OnTriggerStay]
      ▼
ButtonActivated Component
      │
      ├─ RequireColliderStay? → TRUE
      ├─ ShowPrompt("Press [A] to Shake Tree")
      │
      │ [Player Presses Button A]
      │
      ├─ CheckCooldown() → If ready:
      │
      ├─ StartCooldown(1.0 second)
      │
      ├─ PlayFeedback(MMFeedbacks)
      │    ├─ MMF_Scale → Shake foliage
      │    ├─ MMF_Rotation → Sway trunk
      │    ├─ MMF_ParticlesInstantiation → Falling leaves
      │    └─ MMF_Sound → Tree_Shake_SFX
      │
      └─ TriggerLootDrop()
             │
             ▼
        Loot Component
             │
             ├─ CheckLootAvailable() → If loot exists:
             │
             ├─ SelectRandomLoot(WeightedTable)
             │    │
             │    ├─ Roll random 0-100
             │    ├─ 0-60: ScrapMetalPicker (Qty: 3-5)
             │    ├─ 61-90: AlienBerryPicker (Qty: 1-2)
             │    └─ 91-100: EnergyCrystalPicker (Qty: 1)
             │
             ├─ InstantiateLoot(SelectedPrefab, Quantity)
             │    │
             │    └─► For each item:
             │          ├─ SpawnPosition = TreePos + SpawnOffset
             │          ├─ Instantiate(Prefab, SpawnPosition)
             │          └─ ApplyForce(ForceMagnitude: 3.0, Direction: Up+Random)
             │
             ├─ DepleteLoot()
             │
             └─ If LootRespawnEnabled == TRUE:
                   └─ StartCoroutine(RespawnLootAfter(180s))
```

---

## Health Consumption Flow (AlienBerry)

```
PLAYER OPENS INVENTORY
      │
      │ [Selects AlienBerry]
      ▼
Inventory GUI
      │
      ├─ Display Item Details
      │    ├─ Name: "Alien Berry"
      │    ├─ Description: "Restores stamina when consumed."
      │    └─ Quantity: X
      │
      │ [Player Clicks "Use" Button]
      ▼
InventoryItem.Use(PlayerID)
      │
      │ [Overridden by AlienBerryItem.cs]
      ▼
AlienBerryItem.Use()
      │
      ├─ FindPlayer(LevelManager.Instance.Players[0])
      │
      ├─ GetComponent<Health>()
      │
      ├─ CheckIfHealing Needed:
      │    └─ If CurrentHealth >= MaxHealth:
      │          └─ return FALSE (Item not consumed)
      │
      ├─ ReceiveHealth(StaminaRestored: 10)
      │         │
      │         └─► Health Component
      │               ├─ CurrentHealth += 10
      │               ├─ Clamp(CurrentHealth, 0, MaximumHealth)
      │               ├─ Update Health UI Bar
      │               └─ PlayFeedback("Consumption")
      │
      ├─ PlaySound(Eat_Berry_SFX)
      │
      └─ return TRUE
             │
             └─► Inventory removes 1x AlienBerry from stack
```

---

## Curved World Shader Integration

All visual meshes must use the Curved World Shader to maintain aesthetic consistency.

```
┌────────────────────────────────────────────────────────────────┐
│ CURVED WORLD SHADER (Applied to ALL materials)                 │
│                                                                 │
│   Vertex Shader:                                                │
│   ├─ Read WorldPosition                                        │
│   ├─ Read CameraPosition                                       │
│   ├─ Calculate Distance: D = Position.z - Camera.z             │
│   ├─ Calculate YOffset: Y = -(D²) × CurveAmount                │
│   ├─ Apply: NewPosition.y = OriginalY + YOffset                │
│   └─ Output to Fragment Shader                                 │
│                                                                 │
│   CurveAmount: 0.005 (Adjustable float property)               │
└────────────────────────────────────────────────────────────────┘
                           │
                           │ [Applied To]
                           │
            ┌──────────────┼──────────────┬──────────────┐
            ▼              ▼              ▼              ▼
      ScrapMetal    EnergyCrystal    AlienBerry     Tree
      Material      Material         Material       Materials
            │              │              │              │
            ▼              ▼              ▼              ▼
      ItemPicker    ItemPicker       ItemPicker     TreePrefab
      Visual        Visual           Visual         Trunk/Foliage
                           │
                           └─────────► RESULT: Rolling Log Effect
                                      (World curves away from camera)
```

---

## File Reference Map

```
PROJECT ROOT
│
├─── Assets/
│      │
│      ├─── Data/
│      │      └─── Items/
│      │             ├─── ScrapMetal.asset ───────┐
│      │             ├─── EnergyCrystal.asset ────┤
│      │             └─── AlienBerry.asset ───────┤
│      │                                          │
│      ├─── Prefabs/                             │
│      │      ├─── Items/                         │
│      │      │      ├─── ScrapMetalPicker.prefab ◄───┐
│      │      │      ├─── EnergyCrystalPicker.prefab ◄┤
│      │      │      └─── AlienBerryPicker.prefab ◄───┤
│      │      │                 │                     │
│      │      │                 └─► [References] ─────┘
│      │      │
│      │      ├─── Environment/
│      │      │      └─── TreePrefab.prefab
│      │      │             │
│      │      │             └─► [Loot Component References]
│      │      │                   ├─ ScrapMetalPicker.prefab
│      │      │                   ├─ EnergyCrystalPicker.prefab
│      │      │                   └─ AlienBerryPicker.prefab
│      │      │
│      │      └─── NPCs/
│      │             └─── GAIA_NPC.prefab
│      │                    │
│      │                    └─► [DialogueTrigger References]
│      │                          └─ GAIA_Prologue_Conversation.asset
│      │
│      ├─── Scripts/
│      │      ├─── Items/
│      │      │      └─── AlienBerryItem.cs
│      │      │             │
│      │      │             └─► [Used By AlienBerry.asset]
│      │      │
│      │      ├─── Environment/
│      │      │      └─── (Future scripts)
│      │      │
│      │      └─── Managers/
│      │             ├─── PrologueManager.cs
│      │             └─── QuestManager.cs
│      │
│      ├─── Art/
│      │      ├─── Icons/ (PNG sprites)
│      │      ├─── Meshes/ (FBX models)
│      │      └─── Materials/ (Materials with shaders)
│      │
│      └─── Audio/
│             ├─── SFX/ (Audio clips)
│             └─── Music/ (Background tracks)
│
└─── Documentation/
       ├─── plan-rough-draft.md (Main GDD)
       ├─── ASSET_CREATION_GUIDE.md (Detailed specs)
       ├─── ASSET_QUICK_REFERENCE.md (Cheat sheet)
       └─── ASSET_RELATIONSHIPS_DIAGRAM.md (This file)
```

---

## Event System Flow (High-Level)

```
                    ┌─────────────────────┐
                    │   GAME MANAGER      │
                    │  (Singleton)        │
                    └──────────┬──────────┘
                               │
                               │ [Manages]
                               │
            ┌──────────────────┼──────────────────┐
            ▼                  ▼                  ▼
      ┌──────────┐      ┌──────────┐      ┌──────────┐
      │ PROLOGUE │      │INVENTORY │      │  WORLD   │
      │ MANAGER  │      │ ENGINE   │      │  STATE   │
      └─────┬────┘      └────┬─────┘      └────┬─────┘
            │                │                  │
            │                │                  │
    ┌───────┴────────┐       │          ┌───────┴────────┐
    │                │       │          │                │
    ▼                ▼       │          ▼                ▼
┌────────┐      ┌────────┐   │    ┌─────────┐      ┌─────────┐
│ Quest  │      │Dialogue│   │    │  Time   │      │  Save   │
│ Tracker│      │ System │   │    │  System │      │  System │
└────────┘      └────────┘   │    └─────────┘      └─────────┘
                             │
                             │ [Listens To]
                             │
                    ┌────────┴────────┐
                    │                 │
                    ▼                 ▼
            InventoryChange     ItemUsed
            Event               Event
                    │                 │
                    └────────┬────────┘
                             │
                             │ [Fires To]
                             │
                    ┌────────┴────────┐
                    │                 │
                    ▼                 ▼
            Update Quest        Update UI
            Progress            (Health bar, etc.)
```

---

## Quest Dependency Graph

```
Day 0:
    Deploy Habitat
         │
         └──► [Required for] ─────┐
                                  │
    Gather Resources              │
    ├─ ScrapMetal (0/10)          │
    └─ EnergyCrystal (0/6)        ├──► Name Planet
         │                        │
         └──► [Required for] ─────┘
                                       │
                                       └──► System Reboot Ceremony
                                              │
                                              └──► [Unlocks Day 1]

Day 1:
    Scan Lifeforms (0/5)
         │
         └──► [Required for] ──► Dr. Hoot Transmission
                                       │
                                       └──► Place Museum Plot
                                              │
                                              └──► [Unlocks Day 2]
```

---

## Summary

This diagram shows:

1. **Data Flow**: How ScriptableObjects link to Prefabs
2. **Component Hierarchy**: How GameObjects are structured
3. **Interaction Flow**: How player actions trigger events
4. **Quest Progression**: How dialogue leads to quests leads to items
5. **System Integration**: How all systems connect (Inventory, Health, Loot, Dialogue)
6. **File Organization**: Where everything is stored in the project

Use this as a reference when:
- Creating new assets (understand what they connect to)
- Debugging interactions (trace the event flow)
- Planning new features (see where they fit in the architecture)
- Onboarding new team members (visual overview of the system)

---

**Last Updated:** 2026-02-15
**Version:** 1.0 (MVP)
