Galactic Crossing: MVP Game Design Document and Technical Implementation Report
1. Executive Summary and Project Vision
1.1 Project Overview
This report serves as a comprehensive Game Design Document (GDD) and Technical Implementation Guide for Galactic Crossing, a 3D social simulation game designed to capture the "cozy" essence of Animal Crossing: New Horizons (ACNH) while transposing its mechanics into a soft science-fiction setting. The primary objective of this Minimum Viable Product (MVP) is to develop the "Crash Landing" prologue, a narrative-driven tutorial phase that mirrors the onboarding loop of ACNH’s "Day 0" and "Day 1."

Unlike traditional survival games predicated on scarcity and threat, Galactic Crossing emphasizes restoration, community building, and creative expression. The player does not struggle against a hostile environment but rather collaborates with it. The prompt requires a direct 1:1 feature mapping of the ACNH introduction, adapted for a planetary colonization theme, with a technical foundation built upon the More Mountains TopDown Engine (TDE) for Unity.

1.2 The "Cozy Sci-Fi" Aesthetic
The "cozy" genre relies on a specific set of ludonarrative pillars: safety, abundance, and player agency. In Animal Crossing, the island is a sanctuary; in Galactic Crossing, the crash site must feel equally welcoming—a "happy accident" rather than a disaster.

Visual Language: Soft, rounded geometry, pastel-infused lighting, and cel-shaded rendering akin to The Legend of Zelda: The Wind Waker.

Mechanical Tone: Non-punishing survival. "Hunger" is replaced by "Battery Level" (limiting high-energy actions, not life), and "Health" is a measure of stamina rather than mortality.

Social Core: The prompt emphasizes deeper social features. While the MVP focuses on the single-player prologue, the architecture is designed to support persistent multiplayer "planet hopping" in future iterations, leveraging TDE’s multiplayer scaffolding.

1.3 Scope of the Report
This document covers the complete design and technical execution for the prologue, defined as the sequence from the initial crash landing to the establishment of the Museum (Bio-Lab) plot. It provides:

Narrative & Mechanical Mapping: A beat-by-beat translation of ACNH tasks to Sci-Fi equivalents.

TopDown Engine Adaptation: Detailed configuration for transforming a combat engine into a weighted, non-combat exploration controller.

Visual Engineering: Shader Graph construction for the "Rolling Log" curved world effect and cel-shading.

Systems Architecture: Inventory management, resource gathering, and persistence using the Inventory Engine and MMSaveLoadManager.

2. Narrative Walkthrough and Mechanics Mapping (Day 0 – Day 1)
To ensure the "cozy" feeling is retained, Galactic Crossing rigidly adheres to the pacing structure of ACNH. This structure is scientifically designed to introduce mechanics (movement, inventory, crafting, placement) sequentially, preventing cognitive overload.

2.1 Day 0: Arrival and Orientation
The "Day 0" loop in ACNH is a scripted sequence that establishes the setting, the primary NPC relationship (Tom Nook), and the player's home base.

2.1.1 Sequence 1: The Crash (The Airport)
ACNH Context: The player arrives at the Dodo Airlines counter, chooses their map, appearance, and hemisphere.
Galactic Crossing Implementation:   

Scene: The player is inside a malfunctioning escape pod. A holographic interface (The Character Creator) boots up.

Mechanic – Map Selection: Instead of island layouts, the player chooses a "Landing Zone" scan. The MMSaveLoadManager must serialize this choice (Northern/Southern Hemisphere equivalent for seasonal star charts).   

Dialogue: The pod AI, G.A.I.A. (Global Artificial Intelligence Assistant), acts as the Tom Nook surrogate.

Dialogue Trigger: "Welcome to the colonization initiative... er, emergency landing protocol. Please verify your bio-metrics."

Technical Note: This scene takes place in a separate Unity Scene (MainMenu) which transitions to the GameScene via the TDE LevelManager.

2.1.2 Sequence 2: The Landing and Tent Placement
ACNH Context: The player lands, meets Timmy/Tommy, receives a tent, and must place it. They also assist two starting villagers.
Galactic Crossing Implementation:   

The Landing: The pod touches down in a meadow. The door blows open. The camera zooms out from a close-up to the isometric gameplay view using MMCameraEvent.   

NPCs: Two "Maintenance Drones" (Bit and Bot) roll out. These are the Timmy/Tommy equivalents.

Mission: G.A.I.A. tasks the player with deploying the Pop-Up Hab Module.

Mechanic – Placement:

The player receives the "Hab Kit" item in their MainInventory.

Using the Inventory GUI, they select "Deploy."

TDE Adaptation: Since TDE focuses on action, we script a placement mode where a "ghost" prefab follows the cursor/player. Upon confirmation (Action Button), the ghost is replaced by the actual Hab prefab using Instantiate.

Constraint: The player must also place "Charging Stations" for Bit and Bot (the neighbor tents).

2.1.3 Sequence 3: The Gathering (Tree Branches & Fruit)
ACNH Context: Tom Nook requests 10 Tree Branches for the campfire and 6 Native Fruit for juice.
Galactic Crossing Implementation:   

Context: G.A.I.A. needs "Carbon Scoring" (burnt debris) to calibrate the atmospheric scrubbers (Campfire) and "Energy Crystals" (Fruit) to jumpstart the drones.

Mechanic – Resource Gathering:

Debris: Scattered ItemPicker objects around the crash site. The player walks over them to collect.

Crystals: Interactable flora. The player approaches a Crystal Spire and presses 'Interact'. This triggers a custom Harvest script which spawns a pickup item.

Inventory Engine: The Inventory listens for ItemPickup events. The UI updates the quest tracker: "Debris: 3/10".

2.1.4 Sequence 4: The Island Warming Party (Naming the Planet)
ACNH Context: A campfire celebration where the player names the island and becomes Resident Representative.
Galactic Crossing Implementation:   

Event: The "System Reboot Ceremony." G.A.I.A. projects a hologram of a campfire.

Naming: A UI input field allows the player to name the planet. This string is stored in a GameManager singleton and saved via MMSaveLoadManager to planet_data.save.   

Conclusion: The player receives a "Stasis Bed" (Camping Cot). Entering the Hab and interacting with the bed saves the game and advances the "World State" to Day 1.

2.2 Day 1: The Loop Begins
"Day 1" transitions the game to real-time (synced with the system clock) and introduces the core progression loops: Crafting and the Museum.

2.2.1 The NookPhone (The DataPad)
ACNH Context: Tom Nook gives the player a NookPhone with apps for Nook Miles, DIY Recipes, and Map.
Galactic Crossing Implementation:   

Item: G.A.I.A. uploads the "Colony OS" to the player's wrist computer.

UI Architecture: Pressing a hotkey opens the DashboardGUI.

Bio-Data (Nook Miles): A list of achievements (e.g., "Steps Taken," "Crystals Harvested") powered by TDE's MMAchievementManager.

Fabricator (DIY): A list of crafting recipes.

2.2.2 The Museum Plot (Blathers' Tent)
ACNH Context: Donate 5 bugs/fish to Tom Nook to unlock Blathers' tent kit.
Galactic Crossing Implementation:   

Narrative: G.A.I.A. detects unknown biological signatures. She asks the player to scan 5 unique lifeforms to assess planetary safety.

Mechanic – Donations:

The player captures "Holo-Beetles" or "Float-Fish" (using a Net tool).

Talking to G.A.I.A. opens a custom DonationUI.

Upon 5 unique donations, G.A.I.A. receives a transmission from Dr. Hoot (Xeno-Biologist) who requests a landing site.

Reward: "Bio-Lab Marker Kit." The player places this to define where the museum will be built the next day.

3. Technical Implementation: Adapting the TopDown Engine
The TopDown Engine is a robust framework, but its default state is tuned for high-octane action games like Hotline Miami. To achieve the "cozy," weighted feel of Animal Crossing, we must fundamentally reconfigure the character controller and physics settings.

3.1 Character Controller Configuration: The "Weighted" Feel
In Animal Crossing, characters do not stop instantly; they have inertia. They lean into turns and accelerate gradually. This physical presence is crucial for the "cozy" aesthetic.

3.1.1 Physics Configuration in CharacterMovement
We will modify the CharacterMovement component parameters to simulate mass and friction.   

Parameter	Default TDE (Action)	Galactic Crossing (Cozy)	Reasoning
Walk Speed	8-10	5.5	Slower pacing encourages exploration and reduces anxiety.
Run Speed	12-15	8.5	Sprinting should be distinct but manageable.
Acceleration	50+ (Instant)	12 - 15	Introduces a 0.3s ramp-up time, making movement feel deliberate.
Deceleration	50+ (Instant)	8 - 10	Creates a "drifting stop" or slide when input is released.
Idle Threshold	0.1	0.05	Increases sensitivity to small stick movements for subtle positioning.
Interpolate	Off	On	Smoothes velocity changes for visual fluidity.
Code Insight: The TDE CharacterMovement script handles velocity in ProcessAbility(). By lowering deceleration, the Lerp function used to return velocity to zero takes longer, effectively simulating the "skidding" stop seen in ACNH.

3.1.2 Directional Movement and Rotation
ACNH uses 360-degree analog movement, but the character mesh rotates smoothly to face the direction of travel.

Component: CharacterOrientation3D

Settings:

Rotation Mode: MovementDirection.

Rotation Speed: 8.0 (Standard TDE is often 20+). A lower speed means the character mesh takes a few frames to turn around, adding to the sense of weight.

Model: Assign the rigged character mesh here.

3.1.3 Animation Blend Tree
To sell the weighted movement, the animation must match the physics. TDE sends a Speed float parameter to the Animator.   

Animator Setup:

Create a Blend Tree triggered by the Speed parameter.

Threshold 0.0: Idle Animation (Breathing, looking around).

Threshold 0.1 – 5.0: Walk Animation (Relaxed gait).

Threshold 5.1 – 8.5: Run Animation (Arms trailing, body leaning forward).

Damping: Set DampTime in the Animator transition to 0.15s to smooth the visual transition between Walk and Run, matching the physics acceleration.

3.2 Input System Configuration
We will use the Unity Input System (New) as TDE fully supports it. The input map must be simplified to remove combat contexts.   

Axis: Left Stick / WASD -> PrimaryMovement.

Button South (A/Space): Interaction. Used for talking to G.A.I.A., picking up items, and confirming UI.

Button East (B/Shift): Run. TDE’s CharacterRun ability should be bound here.

Button West (X/R): Inventory. Opens the Inventory Engine GUI.

Button North (Y/F): Tool Action. Uses the equipped item (Axe, Net).

Prohibited Inputs: All weapon firing, reloading, and weapon switching inputs should be unbound or removed from the CharacterHandleWeapon ability to strictly enforce non-combat gameplay.

3.3 Camera Architecture: The "Rolling Log" View
A critical component of the ACNH aesthetic is the camera perspective. It is not a standard top-down view but a specialized projection.

Implementation: CinemachineVirtualCamera (Standard in TDE).

Body: Transposer.

Follow Offset: (0, 10, -7). This creates a steep 50-60 degree angle.

Lens:

Field of View: 30-40 (Vertical). A lower FOV approximates an orthographic look while keeping perspective depth.

LookAt: The Player Character.

World Bending: The camera setup must work in tandem with the Curved World Shader (detailed in Section 5) to produce the rolling effect. The camera itself does not rotate; the world geometry deforms relative to the camera's Z-position.

4. Systems Design: Inventory and Resource Management
The Inventory Engine (IE) is bundled with TDE and is perfect for this application. We will configure it to mimic the "Pocket" system of ACNH.

4.1 Inventory Architecture
We utilize the CharacterInventory ability to link the player to the storage system.   

Main Inventory:

Name: MainInventory.

Capacity: 20 Slots.

Persistent: True. This ensures that when the player saves and quits, their items remain.

Equipment Inventory:

Name: EquipmentInventory.

Capacity: 1 Slot (Hand). This is where tools (Net, Axe) or held items (Fruit) are placed.

4.2 Resource Item Implementation
Items are created as ScriptableObjects. We need two primary types: Resources (Passive) and Consumables (Active).

4.2.1 Scrap Metal (Resource)
This item functions like the Tree Branch in ACNH. It is a crafting ingredient.

Class: InventoryItem.

Settings:

Item Name: "Scrap Metal".

Stackable: True (Max 30).

Icon: Sprite of twisted hull plating.

Prefab: A 3D mesh of debris with an ItemPicker component.

Picker Logic: The ItemPicker script handles the logic of adding the item to the inventory when the player walks over it or interacts with it. We set DisableObjectWhenDepleted to true so the debris disappears from the world.   

4.2.2 Alien Berry (Consumable/Healing)
In ACNH, eating fruit gives you energy to break rocks. In Galactic Crossing, eating berries restores "Stamina" (Health). To implement this, we need a custom Item Action.   

Implementation Strategy:

Create a new script HealthBonusItem.cs that inherits from InventoryItem.

Override the Use() method. This method is called when the player selects "Eat" from the inventory menu.

Inside the method, locate the Player character and access their Health component.

Call ReceiveHealth() to restore values.

Code Example: Health Consumable Script

C#
using UnityEngine;
using MoreMountains.InventoryEngine;
using MoreMountains.TopDownEngine;


public class AlienBerryItem : InventoryItem
{
   
    public int StaminaRestored = 10;

    /// <summary>
    /// Triggered when "Use" is selected in the Inventory GUI
    /// </summary>
    public override bool Use(string playerID)
    {
        // 1. Find the player character
        // TDE's LevelManager provides global access to the player
        Character targetCharacter = LevelManager.Instance.Players;

        if (targetCharacter!= null)
        {
            // 2. Get the Health component
            Health characterHealth = targetCharacter.GetComponent<Health>();
            
            if (characterHealth!= null)
            {
                // 3. Prevent over-healing (Optional but good UX)
                if (characterHealth.CurrentHealth >= characterHealth.MaximumHealth)
                {
                    return false; // Action failed, item not consumed
                }

                // 4. Apply Healing
                // ReceiveHealth(amount, instigator, position, feedback_name)
                characterHealth.ReceiveHealth(StaminaRestored, null, targetCharacter.transform.position, "Consumption");
                
                // 5. Trigger Feedbacks (Sound/Particles)
                // This would be handled by the Health component's Feedback setup, 
                // or we can trigger a specific MMFeedback here.
                
                return true; // Success: Item will be removed from inventory
            }
        }
        return false;
    }
}
Note: This script allows the item to function logically within the IE framework. The Use action in the Inventory Inspector must be mapped to this method.   

4.3 Quest Integration
The progression (e.g., "Collect 10 Scrap Metal") requires a Quest Manager. TDE does not have a built-in Quest system, but we can simulate one using a simple GameManager script or integrate Quest Machine. For the MVP, a lightweight singleton PrologueManager is sufficient.   

Logic:

In Update(), check Inventory.GetQuantity("ScrapMetal").

If Quantity >= 10 AND CurrentStage == QuestStage.Gathering, enable the "Turn In" dialogue option on the G.A.I.A. NPC.

5. Visual Engineering: The "Curved World" Shader
The signature look of Animal Crossing is the "Rolling Log" effect, where the world curves away from the camera. This is achieved not by modeling a cylinder, but by Vertex Displacement in the shader.

5.1 Mathematical Foundation
The effect uses a parabolic curve function applied to the Y-axis of the vertices based on their distance from the camera along the Z-axis.   

Formula:

Y 
offset
​
 =−(Z 
vertex
​
 −Z 
camera
​
 ) 
2
 ×CurvatureStrength
Z 
vertex
​
 −Z 
camera
​
 : The relative distance of the object from the camera.

Square ( 
2
 ): Creates the parabolic arc.

Negative Sign: Curves the world downward (creates the horizon).

CurvatureStrength: A small coefficient (e.g., 0.001) to control the steepness.

5.2 Shader Graph Implementation (URP)
We will build this in Unity's Shader Graph to ensure compatibility with the Universal Render Pipeline (URP).

Node Structure:

Inputs:

Position Node (Space: World).

Camera Position Node (Space: World).

CurveAmount Property (Float, default 0.005).

Calculation:

Subtract: Position.z - CameraPosition.z. This gives the distance D.

Multiply: D * D (Square the distance).

Multiply: Result * CurveAmount.

Multiply: Result * -1 (To curve downwards). This is the Y_Offset.

Application:

Add: Position.y (Original Y) + Y_Offset.

Construct a new Vector3: (Position.x, New_Y, Position.z).

Conversion:

Transform Node: Convert the new World Position back to Object Space (required by the Vertex Position master node output).   

Master Node: Connect to Vertex Position.

Crucial Insight: This shader must be applied to every object in the scene (Terrain, Trees, Buildings, Characters). If an object does not have this shader, it will appear to float or clip as the ground curves away beneath it.

5.3 Cel Shading (Wind Waker Style)
To complement the geometry, the lighting should be stylized.

Technique: N-Step Lighting Ramp (Quantized Lighting).

Shader Graph:

Compute the Dot Product of the Normal Vector and Main Light Direction.

Feed this value (-1 to 1) into the UVs of a Sample Texture 2D node containing a Ramp Texture (a horizontal gradient image with hard edges between colors).

This snaps the lighting to specific bands (light, mid, shadow) regardless of the actual light angle smoothness.   

Outlines: Use the Inverted Hull method. Add a second Pass to the shader that extrudes vertices along their normals and renders them black with Front Face Culling. This creates a crisp outline around the character.   

6. Advanced AI: Passive Wildlife System
A "cozy" world is alive but safe. We will implement passive wildlife (e.g., "Space Beetles") using TDE's Advanced AI system. This system uses a Brain to switch between States based on Decisions triggering Actions.   

6.1 Architecture: The "Fleeing" Behavior
We need an AI that wanders randomly but runs away if the player gets too close.

Component: AIBrain

State	Actions	Transitions (Decisions)	Logic
Wander	AIActionMoveRandomly3D	AIDecisionDetectTargetRadius3D	If player is detected within 5m, switch to Flee.
Flee	AIActionMoveAwayFromTarget3D	AIDecisionDistanceToTarget	If player is > 15m away, switch to Wander.
6.2 Configuration Details
Detection (The Scare):

Decision: AIDecisionDetectTargetRadius3D.

Radius: 5.0.

Target Layer: Player.

Result: Returns true when the player enters the radius, triggering the transition to the "Flee" state.

The Escape (The Action):

Action: AIActionMoveAwayFromTarget3D.

Behavior: The AI calculates the vector from the Player to itself and moves in that direction.

Speed: The AI's CharacterMovement component should have a WalkSpeed of 8.0 (faster than the player's 5.5) to ensure they can escape, creating a gameplay loop where the player must "sneak" (move the stick slightly) to catch them.   

Returning to Calm:

Decision: AIDecisionDistanceToTarget.

Comparison: Greater Than.

Distance: 15.0.

Result: When the AI is safe, it returns to the Wander state.

7. Persistence: Saving the World
Unlike level-based games, Galactic Crossing requires persistent world state (name, inventory, quest progress).

7.1 Using MMSaveLoadManager
TDE includes MMSaveLoadManager for binary, JSON, or encrypted serialization.   

7.1.1 Saving the Planet Name
We need to save the string entered during the "Island Warming Party."

C#
// Saving
string planetName = "Nebula-9";
MMSaveLoadManager.Save(planetName, "PlanetName.data", "GalacticCrossingSave");

// Loading
string loadedName = (string)MMSaveLoadManager.Load(typeof(string), "PlanetName.data", "GalacticCrossingSave");
7.1.2 Inventory Persistence
The Inventory Engine handles its own saving if the Persistent checkbox is ticked in the inspector. It saves the item IDs and quantities to a file named Inventory.data in the persistent data path. We must ensure MMSaveLoadManager.Load() is called on game start, typically handled automatically by the Inventory component's Awake() method.   

8. Future Roadmap: Online and Social Features
The prompt specifically requests "deeper online and social/community features." While the MVP is the single-player prologue, the architecture must support this future expansion.

8.1 TopDown Engine Multiplayer Capabilities
TDE supports local multiplayer out of the box. For online play, it requires integration with a network solution like Photon (PUN2) or Mirror.

Strategy: The Character class in TDE allows for input to be driven by external scripts. In a networked environment, the local player's input drives their character, while remote players are driven by network position updates (NetworkTransform).

Social Spaces: The "Planet" concept acts as a server/room. When a player visits another planet (via the "Star Gate" - Airport equivalent), the game scene loads the host's terrain data and instantiates the visitor's character.

Community Features:

Blueprint Sharing: Players can design Hab layouts or furniture. These data structures (JSON) can be uploaded to a central server and downloaded by other players, similar to ACNH's Custom Design Kiosk.

Asynchronous Multiplayer: "Time Capsules" or "Holo-Messages" left on planets for visitors to find even when the host is offline.

9. Conclusion
This MVP GDD provides a concrete blueprint for transforming the action-oriented TopDown Engine into the serene Galactic Crossing. By aggressively retuning the CharacterMovement physics for weight , implementing a Curved World Shader for aesthetic whimsy , and leveraging the Inventory Engine and Advanced AI for non-combat loops , we create a solid foundation. The resulting prologue will not only teach the player the mechanics but also instill the "cozy" philosophy essential for the genre's success.   

Key Data Tables
Table 1: MVP Feature Mapping

Feature	ACNH (Source)	Galactic Crossing (Implementation)	TDE Component
Guide	Tom Nook	G.A.I.A. (AI)	DialogueZone
Home	Tent	Pop-Up Hab	ItemPicker (Placement Logic)
Currency	Nook Miles	Bio-Data	MMAchievementManager
Health	Stamina (Fruit)	Battery (Berries)	Health, HealthBonusItem
Threat	Bees/Spiders	Static Shock/Gas	DamageOnTouch (Environmental)
Table 2: Character Physics Settings

Setting	Value	Effect
Walk Speed	5.5	Relaxed pacing.
Acceleration	15.0	Heavy start-up; feels grounded.
Deceleration	10.0	Drifting stop; prevents "twitchy" feel.
Rotation Speed	8.0	Smooth turning arcs.
End of Report


animalcrossing.fandom.com
Guide:Tutorial (New Horizons) | Animal Crossing Wiki - Fandom
Opens in a new window

ign.com
Day 0 - Getting Started - Animal Crossing: New Horizons Guide - IGN
Opens in a new window

topdown-engine-docs.moremountains.com
Save and Load | TopDown Engine Documentation
Opens in a new window

ign.com
Beginner's Guide: What to Do During Your First Days - Animal Crossing - IGN
Opens in a new window

reddit.com
Days 1, 2, and 3 - Progression Guide / Walkthrough! I hope this is helpful for anybody wondering what to do next :) : r/AnimalCrossing - Reddit
Opens in a new window

en.wikibooks.org
Animal Crossing: New Horizons/Arrival - Wikibooks, open books for an open world
Opens in a new window

topdown-engine-docs.moremountains.com
Character Abilities | TopDown Engine Documentation
Opens in a new window

topdown-engine-docs.moremountains.com
MoreMountains.TopDownEngine.CharacterMovement Class Reference
Opens in a new window

topdown-engine-docs.moremountains.com
Animations | TopDown Engine Documentation
Opens in a new window

topdown-engine.moremountains.com
TopDown Engine - the best 2D and 3D top down solution for Unity, by More Mountains
Opens in a new window

topdown-engine-docs.moremountains.com
Inventory | TopDown Engine Documentation - More Mountains
Opens in a new window

topdown-engine-docs.moremountains.com
Recipes | TopDown Engine Documentation - More Mountains
Opens in a new window

topdown-engine-docs.moremountains.com
Loot - TopDown Engine Documentation
Opens in a new window

topdown-engine-docs.moremountains.com
MoreMountains.InventoryEngine.Inventory Class Reference - TopDown Engine Documentation
Opens in a new window

youtube.com
Healing with Health Items - 2D Platformer Unity #18 - YouTube
Opens in a new window

topdown-engine-docs.moremountains.com
Health & Damage | TopDown Engine Documentation
Opens in a new window

topdown-engine-docs.moremountains.com
MoreMountains.InventoryEngine.InventoryItem Class Reference - TopDown Engine Documentation
Opens in a new window

youtube.com
Quest Machine - Tutorial: TopDown Engine Integration (Part 2) - YouTube
Opens in a new window

github.com
skylarbeaty/curved-world: Recreation of the world curvature ... - GitHub
Opens in a new window

github.com
Curved or rolling horizon shader (aka "Animal Crossing" rolling log world effect) · bevyengine bevy · Discussion #10062 - GitHub
Opens in a new window

notslot.com
World Bending Effect | Unity Tutorial - NotSlot
Opens in a new window

learn.unity.com
Shader Graph: Vertex Displacement - Unity Learn
Opens in a new window

danielilett.com
Stylised Water in Shader Graph and URP - Daniel Ilett
Opens in a new window

youtube.com
How to make Zelda: Wind Waker's Style Lighting - Unity - YouTube
Opens in a new window

medium.com
Toon Shaders in Unity — From Shader Graph to Custom HLSL | by madcritter - Medium
Opens in a new window

studio.uclaacm.com
Outline Shaders in Unity's URP - ACM Studio
Opens in a new window

youtube.com
Hull Outline Shader in Unity URP Using Renderer Features and Culling! ✔️ 2020.3 | Game Dev Tutorial - YouTube
Opens in a new window

topdown-engine-docs.moremountains.com
Advanced AI | TopDown Engine Documentation
Opens in a new window

inventory-engine-docs.moremountains.com
Save and load | Inventory Engine Documentation - More Mountains