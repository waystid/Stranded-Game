# Galactic Crossing - Asset Quick Reference

## ScriptableObjects Quick Reference

### ScrapMetal.asset
```
Type: InventoryItem
Location: Assets/Data/Items/ScrapMetal.asset

ItemID: "ScrapMetal"
ItemName: "Scrap Metal"
Stackable: TRUE
MaxStack: 30
Usable: FALSE
ItemClass: "Resource"
```

### EnergyCrystal.asset
```
Type: InventoryItem
Location: Assets/Data/Items/EnergyCrystal.asset

ItemID: "EnergyCrystal"
ItemName: "Energy Crystal"
Stackable: TRUE
MaxStack: 10
Usable: FALSE
ItemClass: "Resource"
```

### AlienBerry.asset
```
Type: AlienBerryItem (Custom Script)
Location: Assets/Data/Items/AlienBerry.asset

ItemID: "AlienBerry"
ItemName: "Alien Berry"
Stackable: TRUE
MaxStack: 10
Usable: TRUE
StaminaRestored: 10
ItemClass: "Consumable"
```

---

## ItemPicker Prefabs Quick Reference

### ScrapMetalPicker.prefab
```
Location: Assets/Prefabs/Items/ScrapMetalPicker.prefab

Components:
- ItemPicker (Item: ScrapMetal.asset, Quantity: 1)
- BoxCollider (IsTrigger: TRUE, Size: 0.8x0.8x0.8)
- Child: Visual (MeshFilter + MeshRenderer)

Layer: Items
Respawn: FALSE
```

### EnergyCrystalPicker.prefab
```
Location: Assets/Prefabs/Items/EnergyCrystalPicker.prefab

Components:
- ItemPicker (Item: EnergyCrystal.asset, Quantity: 1)
- CapsuleCollider (IsTrigger: TRUE, Radius: 0.4, Height: 1.2)
- Child: Visual (Emissive cyan material)
- Child: ParticleEffect (OPTIONAL - cyan glow)
- Child: PointLight (OPTIONAL - cyan, range 3.0)

Layer: Items
Respawn: FALSE
```

### AlienBerryPicker.prefab
```
Location: Assets/Prefabs/Items/AlienBerryPicker.prefab

Components:
- ItemPicker (Item: AlienBerry.asset, Quantity: 1)
- SphereCollider (IsTrigger: TRUE, Radius: 0.35)
- Child: Visual (Purple berry mesh)

Layer: Items
Respawn: TRUE (300 seconds = 5 minutes)
```

---

## Environment Prefabs Quick Reference

### TreePrefab.prefab
```
Location: Assets/Prefabs/Environment/TreePrefab.prefab

Hierarchy:
TreePrefab (Root)
├─ Trunk (Visual)
├─ Foliage (Visual)
├─ TreeCollider (BoxCollider - solid)
└─ TreeShakeZone
   ├─ SphereCollider (Trigger, Radius: 1.5)
   ├─ ButtonActivated (Cooldown: 1.0s)
   └─ Loot (Component)

Loot Table:
- ScrapMetalPicker: 3-5 qty, 60% weight
- AlienBerryPicker: 1-2 qty, 30% weight
- EnergyCrystalPicker: 1 qty, 10% weight

Settings:
- NumberOfDrops: 1 (one item type per shake)
- LootRespawnTime: 180s (3 minutes)
- SpawnOffset: (0, 2.5, 0)
- ApplyForce: TRUE, ForceMagnitude: 3.0
```

---

## NPC Prefabs Quick Reference

### GAIA_NPC.prefab
```
Location: Assets/Prefabs/NPCs/GAIA_NPC.prefab

Hierarchy:
GAIA_NPC (Root)
├─ Visual
│  ├─ HologramProjector (Disc mesh)
│  └─ HologramBody (Animated, hologram shader)
├─ DialogueZone
│  ├─ BoxCollider (Trigger, Size: 2.5x2.0x2.5)
│  ├─ ButtonActivated
│  └─ DialogueTrigger
└─ NameDisplay (OPTIONAL - TextMeshPro "G.A.I.A.")

Layer: NPC
Tag: "GAIA"
```

---

## Dialogue Quick Reference

### Day 0 Sequence
1. **Initial Meeting** → Character creator, introduction
2. **Deploy Habitat** → Give HabKit, quest starts
3. **Gather Resources** → Collect 10 ScrapMetal + 6 EnergyCrystal
4. **Resource Turn-In** → Complete gathering quest
5. **Name Planet** → UI input for planet name
6. **System Reboot Ceremony** → Give DataPad, advance to Day 1

### Day 1 Sequence
1. **Morning Briefing** → Introduce scanning mechanic
2. **Scan Lifeforms** → Capture 5 unique specimens
3. **Dr. Hoot Transmission** → Museum plot unlocked
4. **Place Museum Plot** → Give BioLabMarkerKit

### Key Dialogue Lines
- "Welcome to the colonization initiative... er, emergency landing protocol."
- "I am G.A.I.A., your Global Artificial Intelligence Assistant."
- "Collect 10 pieces of Scrap Metal scattered around the landing zone."
- "Harvest 6 crystals from the local flora."
- "What shall we call our new home?" [Planet naming]
- "I've detected unknown biological signatures in the area."
- "Dr. Hoot is requesting permission to establish a Bio-Lab!"

---

## Component Configuration Quick Reference

### ItemPicker Component
```
Essential Settings:
- Item: [Reference to ScriptableObject]
- Quantity: 1 (usually)
- DisableObjectWhenDepleted: TRUE
- TriggerMode: "Auto"
- RequireButtonPress: FALSE
```

### ButtonActivated Component
```
Essential Settings:
- CanActivate: TRUE
- ButtonA: TRUE
- ActivationMode: "OnButtonPress"
- RequireColliderStay: TRUE
- ShowPrompt: TRUE
- PromptText: "Press [A] to [Action]"
```

### Loot Component
```
Essential Settings:
- GameObjectsToLoot: [Array of LootDefinitions]
  - GameObject: [Prefab reference]
  - Quantity: Min-Max range
  - Weight: Percentage (0-100)
- SpawnMode: "Random"
- NumberOfDrops: 1
- SpawnOffset: (0, Y, 0)
- ApplyForce: TRUE
- LootRespawnEnabled: TRUE/FALSE
- LootRespawnTime: Seconds
```

---

## Audio Assets Needed

### Pickup Sounds
- ScrapMetal_Pickup_SFX (metallic clink)
- Crystal_Pickup_SFX (crystalline chime)
- Berry_Pickup_SFX (soft pop)

### Drop Sounds
- Metal_Drop_SFX (thud)
- Crystal_Chime_SFX (chime)
- Berry_Drop_SFX (soft bounce)

### Consumption Sounds
- Eat_Berry_SFX (bite/chew)

### Environment Sounds
- Tree_Shake_SFX (rustle, creaking)
- Berry_Respawn_SFX (sparkle/chime)

---

## Visual Assets Needed

### Icons (64x64 or 128x128)
- ScrapMetal_Icon.png
- EnergyCrystal_Icon.png
- AlienBerry_Icon.png

### 3D Meshes
- ScrapMetal_Mesh.fbx (twisted metal, 50-200 tris)
- EnergyCrystal_Mesh.fbx (faceted crystal, 100-300 tris)
- AlienBerry_Mesh.fbx (berry cluster, 50-150 tris)
- Tree_Trunk_Mesh.fbx (cylinder, 300-500 tris)
- Tree_Foliage_Mesh.fbx (sphere/stylized, 200-400 tris)
- HologramProjector_Mesh.fbx (disc, 50-100 tris)
- HologramBody_Mesh.fbx (humanoid silhouette, 500-1000 tris)

### Materials (All with Curved World Shader!)
- ScrapMetal_Material (metallic, rusted)
- EnergyCrystal_Material (emissive cyan)
- AlienBerry_Material (cel-shaded purple)
- Tree_Trunk_Material (bark texture)
- Tree_Foliage_Material (alien colored leaves)
- Hologram_Material (transparent cyan, scanlines)

---

## Layer Setup

Required Layers:
1. **Items** - All ItemPicker objects
2. **Environment** - Trees, rocks, terrain
3. **Interactable** - DialogueZones, interaction triggers
4. **NPC** - G.A.I.A. and future NPCs
5. **Player** - Player character

---

## Critical Values Reference

### Item Stack Sizes
- ScrapMetal: 30
- EnergyCrystal: 10
- AlienBerry: 10

### Quest Requirements
- Deploy Habitat: 1 HabKit placement
- Gather Resources: 10 ScrapMetal + 6 EnergyCrystal
- Scan Lifeforms: 5 unique specimens
- Place Museum Plot: 1 BioLabMarkerKit placement

### Respawn Times
- AlienBerry: 300 seconds (5 minutes)
- Tree Loot: 180 seconds (3 minutes)

### Interaction Ranges
- ItemPicker: 2.0 units (auto-pickup)
- Tree Shake: 1.5 radius (sphere collider)
- G.A.I.A. Dialogue: 2.5x2.0x2.5 box (approach from any side)

### Tree Loot Probabilities
- ScrapMetal: 60% chance
- AlienBerry: 30% chance
- EnergyCrystal: 10% chance

### Health Restoration
- AlienBerry: +10 stamina per berry

---

## File Structure

```
Assets/
├─ Data/
│  └─ Items/
│     ├─ ScrapMetal.asset
│     ├─ EnergyCrystal.asset
│     └─ AlienBerry.asset
├─ Prefabs/
│  ├─ Items/
│  │  ├─ ScrapMetalPicker.prefab
│  │  ├─ EnergyCrystalPicker.prefab
│  │  └─ AlienBerryPicker.prefab
│  ├─ Environment/
│  │  └─ TreePrefab.prefab
│  └─ NPCs/
│     └─ GAIA_NPC.prefab
├─ Scripts/
│  ├─ Items/
│  │  └─ AlienBerryItem.cs
│  ├─ Environment/
│  │  └─ (Future tree shake scripts if needed)
│  └─ Managers/
│     └─ (Quest/dialogue management)
├─ Art/
│  ├─ Icons/
│  │  ├─ ScrapMetal_Icon.png
│  │  ├─ EnergyCrystal_Icon.png
│  │  └─ AlienBerry_Icon.png
│  ├─ Meshes/
│  │  └─ (All .fbx files)
│  └─ Materials/
│     └─ (All materials)
└─ Audio/
   ├─ SFX/
   │  └─ (All sound effects)
   └─ Music/
      └─ (Background music)
```

---

## Common Mistakes to Avoid

1. **Forgetting to set IsTrigger on colliders** → Items won't be picked up
2. **Not linking ScriptableObject to Prefab** → ItemPicker won't know what to give
3. **Not linking Prefab to ScriptableObject** → Item won't spawn in world when dropped
4. **Missing Curved World Shader** → Objects will appear to float
5. **Wrong layer assignments** → Interaction won't work
6. **Forgetting to mark ButtonActivated as "CanActivate"** → Can't interact
7. **Not setting RespawnEnabled** → Berries won't come back
8. **Missing Audio clip references** → Silent pickups
9. **Loot weights not totaling sensibly** → Unpredictable drop rates
10. **Dialogue not linked to quest triggers** → Progression breaks

---

## Testing Checklist (Fast)

Quick tests to run after creating each asset:

**ScriptableObjects:**
- [ ] Open in inspector, all fields filled
- [ ] Icon displays correctly
- [ ] Prefab reference is valid

**ItemPickers:**
- [ ] Place in scene, collider visible in scene view (green wireframe)
- [ ] Play mode: walk through it → should pick up
- [ ] Check inventory: item appears with correct quantity
- [ ] Berry: wait 5 min → should respawn

**Tree:**
- [ ] Place in scene
- [ ] Play mode: walk near → interaction prompt appears
- [ ] Press interact button → loot spawns
- [ ] Loot flies away from tree
- [ ] Shake again immediately → cooldown prevents spam
- [ ] Wait 3 min → loot refills

**G.A.I.A.:**
- [ ] Place in scene
- [ ] Play mode: walk near → dialogue prompt appears
- [ ] Press interact → dialogue box opens
- [ ] Can read through all lines
- [ ] Quest triggers activate
- [ ] Items are given to player

---

## Support Resources

- **TopDown Engine Docs**: https://topdown-engine-docs.moremountains.com/
- **Inventory Engine Docs**: https://inventory-engine-docs.moremountains.com/
- **Main GDD**: plan-rough-draft.md
- **Full Asset Guide**: ASSET_CREATION_GUIDE.md
- **Unity Forums**: https://forum.unity.com/
- **More Mountains Discord**: (Check asset store page for invite)

---

**Last Updated:** 2026-02-15
**Version:** 1.0 (MVP)
