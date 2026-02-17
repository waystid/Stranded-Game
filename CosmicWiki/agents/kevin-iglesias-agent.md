# Kevin Iglesias Animation Agent

> **🤖 Specialized Agent for Kevin Iglesias Animation Integration**
>
> Full clip catalog, retargeting steps, animator controller paths, and tool animation wiring for Features 006 and 007.

---

## Overview

The **Kevin Iglesias Agent** catalogs all Human Animations from the Kevin Iglesias pack and
provides patterns for retargeting clips to new character rigs (Synty Sidekick in Feature 007)
and wiring tool-specific animations (Feature 006).

---

## Asset Paths

```
Assets/Kevin Iglesias/Human Animations/
  Unity Demo Scenes/
    Animations/
      HumanM@*.anim      ← Male animation clips
      HumanF@*.anim      ← Female animation clips (if present)
    AnimatorControllers/
      HumanM_*.controller
  Models/
    HumanM.fbx           ← Source model (reference rig)
  Readme.txt
```

---

## Animation Clip Catalog

### Gathering / Tool Animations (Feature 006)

| Controller Name | Action | Tool Mapping |
|----------------|--------|-------------|
| `HumanM@CuttingTree01` | 2-hand axe swing loop | Plasma Cutter (chop) |
| `HumanM@CuttingTree02` | Axe finishing blow | Plasma Cutter (finish) |
| `HumanM@Gathering01` | Bend + pick up | Mineral Extractor (dig) |
| `HumanM@Gathering02` | Reach forward grab | Stasis Field Generator (catch) |
| `HumanM@Gathering03` | Crouch pick up | Generic gather |
| `HumanM@Gathering04` | Low angle dig | Mineral Extractor (alt) |
| `HumanM@Fishing01` | Cast rod + wait loop | Plasma Seiner (fish cast) |
| `HumanM@Fishing02` | Reel in catch | Plasma Seiner (reel) |
| `HumanM@Watering` | Water can pour loop | Hydration Disperser |

### Locomotion (Feature 007 / base character)

| Clip | Description |
|------|-------------|
| `HumanM@Idle` | Standard idle |
| `HumanM@Walk` | Walk cycle |
| `HumanM@Run` | Run cycle |
| `HumanM@Jump` | Jump (separate clips: takeoff, airborne, land) |
| `HumanM@Sprint` | Sprint cycle |

### Combat / Other

| Clip | Description |
|------|-------------|
| `HumanM@Attack01` | Single punch |
| `HumanM@Attack02` | Combo hit |
| `HumanM@Death` | Death animation |
| `HumanM@GetHit` | Hit reaction |

---

## Retargeting to Synty Sidekick Rig

### Prerequisites
1. Synty SidekickCharacter FBX must be set to **Humanoid** rig in Import Settings
2. Kevin Iglesias HumanM.fbx must also be **Humanoid** rig
3. Both must have valid Avatar definitions

### Steps
1. Select the target animation clip (e.g., `HumanM@CuttingTree01.anim`)
2. In Project window → right-click → Create → Animation → Copy Animation Clip
3. Rename to `SidekickM@CuttingTree01.anim`
4. Select the copy → Inspector → Model → Avatar → assign Sidekick avatar
   - OR: Set source avatar to Sidekick's avatar in FBX import settings

**Automated approach (Feature 007):**
```csharp
// At runtime, Mecanim handles retargeting automatically IF:
// - Both source FBX and target FBX use Humanoid rig type
// - Animator.avatar = sidekickAvatar (set on SidekickPlayer)
// - AnimatorController clips reference Generic@clip (not baked)
// No clip copying needed — Unity retargets at play time
```

---

## Animator Controller Pattern (Feature 006)

Each tool gets its own AnimatorController that is swapped at runtime:

```csharp
// ToolController.cs pattern:
// Store references to tool animator controllers
[SerializeField] private RuntimeAnimatorController defaultController;
[SerializeField] private RuntimeAnimatorController plasmaCutterController;
[SerializeField] private RuntimeAnimatorController mineralExtractorController;

// On tool equip:
private void EquipTool(ToolType tool)
{
    var animator = playerGO.GetComponent<Animator>();
    switch (tool)
    {
        case ToolType.PlasmaCutter:
            animator.runtimeAnimatorController = plasmaCutterController;
            break;
        // etc.
    }
}
```

**Controller paths to create:**
```
Assets/Animations/Tools/
  PlasmaCutter.controller      ← Idle, Swing, Finish states
  MineralExtractor.controller  ← Idle, Dig states
  StasisField.controller       ← Idle, Catch states
  PlasmaSeine.controller       ← Idle, Cast, Reel states
  HydrationDisperser.controller ← Idle, Water states
```

---

## Animation Event Integration

Tool effects are triggered via Animation Events (not time-based):

```csharp
// Add to animation clip at action frame:
// Function: "OnToolActionFrame"
// In ToolController.cs:
public void OnToolActionFrame()
{
    currentTool.ApplyEffect(targetCell);
}

// OR use ToolActions.cs callback:
public void OnCuttingComplete()
{
    ChopAction.Execute(targetCell, onComplete: SpawnFerriteCoreAtCell);
}
```

---

## Clip Duration Reference

| Clip | Duration | Loop | Action Frame |
|------|----------|------|-------------|
| CuttingTree01 | ~1.8s | Yes | 0.9s (mid-swing) |
| Gathering01 | ~1.2s | No | 1.0s (pickup) |
| Fishing01 | ~3.0s | Yes | — (wait loop) |
| Watering | ~2.0s | Yes | — (pour loop) |

*Exact frames vary — measure in Animation window*

---

## Related Files

- `Assets/Scripts/Tools/ToolController.cs` — Tool equip/unequip + animator swap
- `Assets/Scripts/Tools/ToolActions.cs` — Effect application per tool
- `CosmicWiki/pages/tools/` — Per-tool wiki pages
- `CosmicWiki/guides/tool-system.md` — Tool system guide
- `CosmicWiki/guides/synty-sidekick-integration.md` — Phase B retargeting
