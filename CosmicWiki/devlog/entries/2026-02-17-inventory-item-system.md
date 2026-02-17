# Feature 002: Inventory & Item System - 2026-02-17

## Quick Reference

- **Date**: 2026-02-17
- **Type**: Feature Implementation
- **Status**: ✅ Complete (Phases A + B + D; Phase C verified via TDE built-in)
- **Branch**: `features/002-inventory`
- **Session**: 8
- **Related Wiki**: N/A (new system)
- **Next Session**: Feature 003 (NPC/Shop system) or Feature 005 (Building Placement) — depends on priority

---

## Objective

**Goal:** Build a flexible, future-safe item system on top of TDE's InventoryEngine that handles fundamentally different item behaviours (resources, consumables, tools, furniture, currency) in a single unified inventory.

**Why:**
1. Features 005 (buildings), 006 (tool actions), and future systems all need a stable type hierarchy from day 1
2. TDE's InventoryEngine already handles stacking, dropping, equipping — we just need typed subclasses
3. AlienBerryItem.cs was an orphan script with no architecture — needed migrating into a real system

**Success Criteria:**
- [x] Type hierarchy compiles clean (CosmicItem → Resource/Consumable/Tool/Furniture)
- [x] 6 item SO assets created and wired into ItemRegistry
- [x] HumanCustomPlayer has 32-slot main + 4-slot hotbar Inventory + ToolController
- [x] HotbarUI (keys 1–4) and InventoryPanel (I key) in SandboxShowcase
- [x] 0 compile errors throughout

---

## Implementation

### Phase A — Item Foundation

**New scripts created:**

| File | Purpose |
|------|---------|
| `Assets/Scripts/Items/CosmicItem.cs` | Base: `CosmicItemType` enum, `GridFootprint[]` |
| `Assets/Scripts/Items/ResourceItem.cs` | Stackable resource, no Use() override |
| `Assets/Scripts/Items/ConsumableItem.cs` | `Use()` → health/speed/stamina restore |
| `Assets/Scripts/Items/ToolItem.cs` | `Equip/UnEquip` → delegates to ToolController |
| `Assets/Scripts/Items/FurnitureItem.cs` | `Use()` stub for Feature 005 PlacementController |
| `Assets/Scripts/Items/ItemRegistry.cs` | `Get(id)`, `GetByType(type)`, auto-Init on first use |
| `Assets/Scripts/Character/ToolController.cs` | CharacterAbility, cylinder prop at ToolAttachPoint |

**AlienBerryItem.cs deleted** — logic fully migrated to ConsumableItem.HealthRestore.

**ScriptableObject assets** (`Assets/Data/Items/`):

| Asset | Class | ItemID | MaxStack | Notes |
|-------|-------|--------|----------|-------|
| EnergyCrystal.asset | ResourceItem | energy_crystal | 10 | Prefab → EnergyCrystalPicker |
| FerriteCore.asset | ResourceItem | ferrite_core | 30 | Prefab → FerritePicker (new) |
| AlienBerry.asset | ConsumableItem | alien_berry | 10 | HealthRestore=10, Prefab → AlienBerryPicker |
| Stardust.asset | ResourceItem | stardust | 999 | ItemType=Currency, Prefab → StardustPicker (new) |
| PlasmaCutter.asset | ToolItem | plasma_cutter | 1 | Equippable, TargetInventory=PlayerHotbarInventory |
| MineralExtractor.asset | ToolItem | mineral_extractor | 1 | Equippable, TargetInventory=PlayerHotbarInventory |

**ItemRegistry.asset** created at `Assets/Data/ItemRegistry.asset` — all 6 items registered.

**Picker prefabs:**
- EnergyCrystalPicker + AlienBerryPicker: updated Item refs (old SOs were missing/broken GUIDs)
- FerritePicker + StardustPicker: created by duplicating EnergyCrystalPicker, updated Item refs

**HumanCustomPlayer.prefab changes:**
- `CharacterInventory.MainInventoryName` → `PlayerMainInventory`
- `CharacterInventory.HotbarInventoryName` → `PlayerHotbarInventory`
- Added `Inventory` component (PlayerMainInventory, 32 slots, Persistent)
- Added `Inventory` component (PlayerHotbarInventory, 4 slots, Persistent)
- Added `ToolController` CharacterAbility

### Phase B — Inventory + Hotbar UI

**New scripts:**
- `Assets/Scripts/UI/HotbarUI.cs` — 4-slot strip, keys 1–4, MMInventoryEvent listener, auto use/equip
- `Assets/Scripts/UI/InventoryPanel.cs` — I key toggle, CanvasGroup show/hide

**SandboxShowcase scene additions:**
- `PlayerInventoryCanvas` (Screen Space-Overlay, sortOrder=5)
  - `HotbarPanel` (HotbarUI component, SlotIcons → 4× HotbarSlot_1..4 Image children)
  - `InventoryPanel` (CanvasGroup + InventoryPanel component, alpha=0 default)

### Phase D — Tool Equip Stub

`ToolController.EquipTool(ToolItem)`:
- Destroys previous prop
- Creates placeholder Cylinder at `ToolAttachPoint` (scale 0.05×0.2×0.05)
- Collider removed from prop so physics unaffected
- Logs equip/unequip to console

`ToolItem.Equip/UnEquip` → `FindObjectOfType<ToolController>()` (replaces with direct ref in Feature 006)

---

## Technical Details

### CosmicItemType Enum

```csharp
public enum CosmicItemType {
    Resource,    // stackable raw materials
    Consumable,  // one-use effects
    Tool,        // equip → grid actions (Feature 006)
    Furniture,   // place on grid (Feature 005)
    Currency,    // Stardust
    Blueprint,   // reserved
    Seed,        // reserved
    Key          // reserved
}
```

### TDE InventoryEngine Corrections (Critical!)

```
// WRONG — does not exist:
MMEventListener<InventoryEvent>
this.MMEventStartListening<InventoryEvent>()

// CORRECT:
MMEventListener<MMInventoryEvent>
this.MMEventStartListening<MMInventoryEvent>()
// Fields: inventoryEvent.TargetInventoryName, inventoryEvent.InventoryEventType (MMInventoryEventType)
```

```
// WRONG — playerID param doesn't exist:
_hotbar.UseItem(item, index, "Player1")

// CORRECT:
_hotbar.UseItem(item, index)           // slot defaults to null
_hotbar.EquipItem(item, index)
```

### Inventory YAML (HumanCustomPlayer prefab)

```yaml
--- !u!114 &7000000000000000001
MonoBehaviour:
  m_Script: {fileID: 11500000, guid: 286d1c3722a3a0749afb21ea8728d132, type: 3}
  m_EditorClassIdentifier: MoreMountains.InventoryEngine::MoreMountains.InventoryEngine.Inventory
  PlayerID: Player1
  InventoryName: PlayerMainInventory
  InventoryType: 0
  Content: [32× {fileID: 0}]
```

**Inventory.cs GUID**: `286d1c3722a3a0749afb21ea8728d132`
**ToolController.cs GUID**: `588e5c85b134e451681c2a43b9903626`

### ConsumableItem.Use() Pattern

```csharp
public override bool Use(string playerID)
{
    Character target = LevelManager.Instance?.Players?[0];
    if (target == null) return false;

    if (HealthRestore > 0f)
    {
        Health h = target.GetComponent<Health>();
        if (h.CurrentHealth >= h.MaximumHealth) return false;  // don't waste item
        h.ReceiveHealth(HealthRestore, target.gameObject);
    }
    return true;  // true = consume from inventory
}
```

---

## Results

### ✅ Success Metrics

- ✅ All 9 new scripts compile with 0 errors
- ✅ 6 item SO assets created with correct properties
- ✅ ItemRegistry populated and saved
- ✅ HumanCustomPlayer has Inventory×2 + CharacterInventory names + ToolController
- ✅ PlayerInventoryCanvas in SandboxShowcase (sortOrder=5)
- ✅ AlienBerryItem.cs successfully deleted; ConsumableItem is the migration

### Testing Results

**Compile check:** ✅ 0 errors, 1 persistent menu-name warning (pre-existing, unrelated)
**Unity refresh:** ✅ All assets imported cleanly
**Git commit:** ✅ `8b051b70` — 44 files, +2350/-68 lines

### Console/Errors

**Errors:** 0
**Warnings:** 1 (pre-existing GalacticCrossing menu duplicate — not ours)
**Result:** ✅ Clean

---

## Lessons Learned

### What Worked Well

1. **Parallel script creation** — all 9 scripts written in one pass, only 1 compile fix needed (wrong event type)
2. **Direct YAML editing for prefab components** — adding Inventory+ToolController to HumanCustomPlayer via YAML was reliable for well-understood TDE component shapes
3. **Duplicate + update pattern for pickers** — duplicating EnergyCrystalPicker to make FerritePicker/StardustPicker, then patching Item GUID in YAML

### What Didn't Work

1. **`manage_scriptable_object` for object references** — `{"guid": "..."}` patches return "Cleared reference" instead of setting the ref. Must edit the .asset YAML directly using `{fileID: 1, guid: ..., type: 3}` format.
2. **`manage_scriptable_object` for array size** — `Items.Array.size` is an `ArraySize` type that MCP cannot set. Must pre-populate the array (create with N null entries, then patch by index, or edit YAML directly).
3. **Wrong InventoryEngine event type** — `InventoryEvent` doesn't exist; correct type is `MMInventoryEvent`.

### Critical Steps

1. **`InventoryItem.IsNull(item)` for null checks** — TDE slots use a custom null check, not `item == null`
2. **Content array must be pre-sized in prefab YAML** — an empty Content array means the Inventory can't hold items at runtime
3. **Prefab picker Item GUID update** — old picker prefabs had broken GUID refs (assets deleted); always verify with `find .meta | xargs grep GUID` before trusting existing refs

---

## Blockers Encountered

### 1. Wrong InventoryEvent Type Name
**Problem:** `InventoryEvent` not found — 4 compile errors in HotbarUI + InventoryPanel
**Investigation:** Grepped codebase for actual event struct — found `MMInventoryEvent` in InventoryEvents.cs
**Solution:** Replace all `InventoryEvent` → `MMInventoryEvent` in both scripts
**Resolution Time:** ~5 min

### 2. UseItem/EquipItem Signature Mismatch
**Problem:** Called `UseItem(item, index, playerID)` — TDE actual signature is `UseItem(item, index, slot=null)`
**Solution:** Removed playerID param from both calls
**Resolution Time:** ~2 min

### 3. ItemRegistry Array Population via MCP
**Problem:** `manage_scriptable_object` can't set `Array.size` (unsupported SerializedPropertyType)
**Solution:** Created SO with 6 null entries via MCP, then edited .asset YAML directly with correct GUIDs
**Resolution Time:** ~10 min

---

## Next Steps

### Immediate (Next Session)

**Option A — Feature 003: NPC/Shop System**
- Objective: Add NPC shopkeeper with dialogue + trade UI
- Approach: ButtonActivated interactable + TDE NPC character + custom ShopUI
- Prerequisite: Feature 002 (items) ✅

**Option B — Feature 005: Building Placement**
- Objective: Implement PlacementController for FurnitureItem.Use()
- Approach: Grid snapping via IslandGridManager, ghost preview, confirm/cancel
- Prerequisite: Feature 002 (items) ✅, Feature 001 (grid) ✅

**Option C — Feature 006: Tool Actions**
- Objective: Wire ToolController.HandleInput() → grid cell actions (chop, mine)
- Approach: IslandGridManager cell query + ToolItem type detection
- Prerequisite: Feature 002 (ToolController stub) ✅

### Future Sessions

**Short Term:**
1. **Feature 005** — Building placement (FurnitureItem.Use() → PlacementController)
2. **Feature 006** — Tool grid actions (chop flora, mine rocks)
3. **Feature 003** — NPC shopkeeper + trade UI (uses ItemRegistry.GetByType)

**Medium Term:**
- Crafting system (ResourceItem combinations → FurnitureItem output)
- Item icons (assign Sprite to each SO asset's Icon field)
- ToolController.ToolAttachPoint wired to `prop_r` bone on HumanCustomPlayer

---

## Related Entries

**Previous:** [Wardrobe Interactable (Feature 007 Phase E)](./2026-02-17-wardrobe-interactable.md)
**Next:** TBD

---

## Files Modified

### New Files
- `Assets/Scripts/Items/CosmicItem.cs` — base item class + CosmicItemType enum
- `Assets/Scripts/Items/ResourceItem.cs` — stackable resource
- `Assets/Scripts/Items/ConsumableItem.cs` — health/stamina/speed consumable
- `Assets/Scripts/Items/ToolItem.cs` — equippable tool stub
- `Assets/Scripts/Items/FurnitureItem.cs` — furniture placement stub
- `Assets/Scripts/Items/ItemRegistry.cs` — global item registry SO
- `Assets/Scripts/UI/HotbarUI.cs` — 4-slot hotbar UI
- `Assets/Scripts/UI/InventoryPanel.cs` — I-key inventory toggle
- `Assets/Scripts/Character/ToolController.cs` — tool ability stub
- `Assets/Data/Items/EnergyCrystal.asset` — ResourceItem SO
- `Assets/Data/Items/FerriteCore.asset` — ResourceItem SO
- `Assets/Data/Items/AlienBerry.asset` — ConsumableItem SO (+10 HP)
- `Assets/Data/Items/Stardust.asset` — Currency ResourceItem SO
- `Assets/Data/Items/PlasmaCutter.asset` — ToolItem SO
- `Assets/Data/Items/MineralExtractor.asset` — ToolItem SO
- `Assets/Data/ItemRegistry.asset` — populated with all 6 items
- `Assets/Prefabs/FerritePicker.prefab` — world pickup for FerriteCore
- `Assets/Prefabs/StardustPicker.prefab` — world pickup for Stardust

### Modified Files
- `Assets/Prefabs/HumanCustomPlayer.prefab` — Inventory×2 + ToolController + CharacterInventory names
- `Assets/Prefabs/EnergyCrystalPicker.prefab` — Item ref updated to new EnergyCrystal.asset
- `Assets/Prefabs/AlienBerryPicker.prefab` — Item ref updated to new AlienBerry.asset
- `Assets/Scenes/SandboxShowcase.unity` — PlayerInventoryCanvas added

### Deleted Files
- `Assets/Scripts/Items/AlienBerryItem.cs` — migrated to ConsumableItem

### Git Status
- Branch: `features/002-inventory`
- Commit: `8b051b70`
- Files changed: 44 (+2350/-68)

---

## Metrics

**Scripts created:** 9 new, 1 deleted
**Assets created:** 6 item SOs + 1 registry + 2 picker prefabs
**Prefabs modified:** 3 (HumanCustomPlayer, EnergyCrystalPicker, AlienBerryPicker)
**Scenes modified:** 1 (SandboxShowcase)
**Compile errors during session:** 4 (all fixed in one pass)
**Final error count:** 0

---

**Entry Created:** 2026-02-17
**Status:** ✅ Complete
**Ready for Next Session:** Yes — pick Feature 003, 005, or 006
