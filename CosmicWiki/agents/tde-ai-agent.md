# TDE AI Agent

> **🤖 Specialized Agent for TopDown Engine AI System Integration**
>
> AIBrain/AIAction/AIDecision patterns, NPC state machine setup, and NavMesh integration for Feature 004.

---

## Overview

The **TDE AI Agent** provides ready-to-use patterns for building NPCs using the TopDown Engine's
AI system. Covers AIBrain setup, state transitions, patrol waypoints, detection radii, and
daily routine scheduling.

---

## Core Components

### AIBrain
**Path:** `Assets/TopDownEngine/ThirdParty/MoreMountains/MMTools/Foundation/MMAI/AIBrain.cs`

The central controller. Attach to NPC root. Holds list of AIStates, manages transitions.

```csharp
// Key fields to configure in Inspector:
// - States: List<AIState> (configure in inspector)
// - BrainActive: bool (set false to pause AI)
// - ActionsFrequency: float (how often actions tick, default 0)
// - DecisionsFrequency: float (how often decisions tick, default 0)
```

---

### AIAction Scripts
**Path:** `Assets/TopDownEngine/Common/Scripts/Characters/AI/Advanced/`

| Script | Purpose | Key Properties |
|--------|---------|----------------|
| `AIActionMovePatrol3D` | Patrol between waypoints | `PatrolPath` (MMPath), `WaypointPauseTime` |
| `AIActionMoveTowardsTarget3D` | Walk toward a detected target | `MinimumDistance` |
| `AIActionMoveAwayFromTarget3D` | Flee from target | `MinimumDistance` |
| `AIActionDoNothing` | Idle / pause state | `Duration` |
| `AIActionPlayMMFeedbacks` | Trigger effects/animations | `Feedbacks` |

---

### AIDecision Scripts
**Path:** `Assets/TopDownEngine/Common/Scripts/Characters/AI/Advanced/`

| Script | Purpose | Key Properties |
|--------|---------|----------------|
| `AIDecisionDetectTargetRadius3D` | Detect player within radius | `Radius`, `DetectionLayerMask` |
| `AIDecisionTargetIsAlive` | Check if target is alive | — |
| `AIDecisionTimeInState` | Transition after N seconds | `AfterTime` |
| `AIDecisionDistanceToTarget` | Transition at distance threshold | `Distance`, `ComparisonOperator` |
| `AIDecisionRandom` | Random branch | `Probability` |

---

## Villager State Machine Pattern

### States for Feature 004 Villagers

```
Idle ──[TimeInState > 5s]──→ Wander
  ↑                              │
  │[TimeInState > 15s]──────────┘
  │
  │[DetectTargetRadius < 5u]──→ Greet ──[TimeInState > 8s]──→ Idle
```

**Idle State:**
- Action: `AIActionDoNothing` (Duration = 3–6s randomized)
- Decision OUT: `AIDecisionTimeInState` (AfterTime = 5)

**Wander State:**
- Action: `AIActionMovePatrol3D` (use MMPath with waypoints)
- Decision OUT: `AIDecisionTimeInState` (AfterTime = 15) → Idle
- Decision OUT: `AIDecisionDetectTargetRadius3D` (Radius = 5) → Greet

**Greet State:**
- Action: `AIActionDoNothing` (face player)
- Action: `AIActionPlayMMFeedbacks` (dialogue trigger)
- Decision OUT: `AIDecisionTimeInState` (AfterTime = 8) → Idle

---

## Villager Prefab Setup (Inspector Steps)

1. Add `Character` component to root (disable weapons if not needed)
2. Add `AIBrain` component — check "BrainActive"
3. Add `CharacterOrientation3D` — face direction of movement
4. Add `NavMeshAgent` — speed 2, stoppingDistance 0.5
5. Add `AIActionMovePatrol3D` — assign MMPath child
6. Add `AIDecisionDetectTargetRadius3D` — Radius 5, LayerMask = Player
7. Configure states in AIBrain inspector

**Hierarchy:**
```
Villager_[Name]
  ├── Model/           ← SkinnedMeshRenderer + Animator
  ├── Waypoints/       ← MMPath component with patrol points
  ├── AIBrain          ← on root
  ├── NavMeshAgent     ← on root
  └── DialogueTrigger  ← SphereCollider (trigger, radius 3)
```

---

## MMPath Waypoint Setup

```csharp
// Add MMPath to a child GameObject named "Waypoints"
// MMPath.PathElements: List<MMPathMovementElement>
// Each element: Position (world), Delay, Curve

// For patrol:
// Add 4-8 points around NPC's home cell
// Set Delay = 2f at each waypoint
// Set MMPath.CycleOption = Loop
```

---

## Detection Layer Configuration

Add "Player" tag to `AstronautPlayer`/`SidekickPlayer`.
Set `AIDecisionDetectTargetRadius3D.TargetLayer` to include Player layer.

```
Layer setup:
  Player layer: 6 (or whatever Unity assigns)
  Detection mask in AIDecision: include layer 6
```

---

## Daily Routine System (Custom)

Feature 004 adds a custom `VillagerController.cs` on top of AIBrain:

```csharp
// Time periods (game time, not real time):
// Morning:   6:00 – 12:00 → home area patrol
// Afternoon: 12:00 – 18:00 → common areas + shop
// Evening:   18:00 – 22:00 → plaza or beach
// Night:     22:00 – 6:00  → home (AIBrain.BrainActive = false)

// DayNightCycle integration:
// VillagerController subscribes to DayNightCycle.OnTimeChanged
// → updates MMPath waypoints based on time period
```

---

## NavMesh Requirements

- NavMesh must be baked after terrain changes (Feature 001 dependency)
- Island ground surface must have `Navigation Static` checked
- Trees/Rocks must have `Navigation Static` + `Not Walkable` area set
- NavMesh bake: Window → AI → Navigation → Bake

---

## Related Files

- `Assets/Scripts/NPC/VillagerData.cs` — ScriptableObject: name, personality, home cell
- `Assets/Scripts/NPC/VillagerController.cs` — Daily routine scheduler
- `Assets/Scripts/NPC/DialoguePanel.cs` — Dialogue UI
- `Assets/Scripts/NPC/VillagerManager.cs` — Villager registry
- `CosmicWiki/guides/npc-system.md` — NPC system guide
- `CosmicWiki/pages/villagers/` — Per-species wiki pages
