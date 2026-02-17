using UnityEngine;
using MoreMountains.TopDownEngine;

/// <summary>
/// CharacterAbility that manages the currently equipped tool and executes grid actions.
/// Called by ToolItem.Equip()/UnEquip() and HotbarUI when a tool slot is selected.
/// Press E (mapped to "Use" input) with a tool equipped to act on the hovered grid cell.
/// </summary>
public class ToolController : CharacterAbility
{
    [Header("Tool Attach")]
    [Tooltip("The hand bone transform where the tool prop model is parented (prop_r)")]
    public Transform ToolAttachPoint;

    [Header("Tool Use")]
    [Tooltip("Tags checked when using a Chop tool action.")]
    public string FloraTag = "Flora";

    [Tooltip("Tags checked when using a Mine tool action.")]
    public string RockTag = "Rock";

    /// <summary>The tool currently held by the player. Null if none.</summary>
    public ToolItem CurrentTool { get; private set; }

    private GameObject _toolProp;
    private bool _useButtonDown = false;

    /// <summary>
    /// Equip a tool: destroy any previous prop, spawn a placeholder cylinder at the attach point.
    /// Feature 006 will spawn the real tool model from ToolItem.AnimatorControllerOverridePath.
    /// </summary>
    public void EquipTool(ToolItem tool)
    {
        UnequipTool();
        CurrentTool = tool;

        if (ToolAttachPoint != null)
        {
            _toolProp = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            _toolProp.name = $"[ToolProp] {tool.ItemName}";
            _toolProp.transform.SetParent(ToolAttachPoint, false);
            _toolProp.transform.localPosition = new Vector3(0f, 0.2f, 0f);
            _toolProp.transform.localScale = new Vector3(0.05f, 0.2f, 0.05f);

            // Remove collider so the prop doesn't block physics
            Destroy(_toolProp.GetComponent<Collider>());
        }

        Debug.Log($"[ToolController] Equipped: {tool.ItemName}");
    }

    /// <summary>
    /// Unequip the current tool and destroy its prop.
    /// </summary>
    public void UnequipTool()
    {
        if (_toolProp != null)
        {
            Destroy(_toolProp);
            _toolProp = null;
        }
        if (CurrentTool != null)
        {
            Debug.Log($"[ToolController] Unequipped: {CurrentTool.ItemName}");
            CurrentTool = null;
        }
    }

    // ── CharacterAbility override ─────────────────────────────────────────────

    protected override void HandleInput()
    {
        // Listen for the "E" key (Unity Input: "Interact" button, or fall back to KeyCode.E)
        bool usePressed = Input.GetKeyDown(KeyCode.E);

        if (usePressed && CurrentTool != null && CurrentTool.ActionType != ToolActionType.None)
        {
            UseActiveTool();
        }
    }

    // ── Tool Use Logic ───────────────────────────────────────────────────────

    private void UseActiveTool()
    {
        if (IslandGridManager.Instance == null)
        {
            Debug.LogWarning("[ToolController] IslandGridManager not found.");
            return;
        }

        // Determine target cell: prefer GridCursor hovered cell, else cell in front of player
        Vector2Int targetCell;
        if (GridCursor.Instance != null && GridCursor.Instance.IsHovering)
        {
            targetCell = GridCursor.Instance.HoveredCell;
        }
        else
        {
            // Fallback: cell directly in front of player
            Vector3 forward = _character.transform.forward;
            Vector3 checkPos = _character.transform.position + forward * 1.5f;
            targetCell = IslandGridManager.Instance.WorldToCell(checkPos);
        }

        GridCell cell = IslandGridManager.Instance.GetCell(targetCell);
        if (cell == null) return;

        switch (CurrentTool.ActionType)
        {
            case ToolActionType.Chop:
                TryChop(targetCell, cell);
                break;

            case ToolActionType.Mine:
                TryMine(targetCell, cell);
                break;

            case ToolActionType.Dig:
                TryDig(targetCell, cell);
                break;

            default:
                Debug.Log($"[ToolController] Action '{CurrentTool.ActionType}' not yet implemented.");
                break;
        }
    }

    private void TryChop(Vector2Int cell, GridCell data)
    {
        if (data.occupant != null && data.occupant.CompareTag(FloraTag))
        {
            Vector3 dropPos = IslandGridManager.Instance.CellToWorld(cell);
            Destroy(data.occupant);
            data.ClearOccupant();

            if (CurrentTool.DropPrefab != null)
                Instantiate(CurrentTool.DropPrefab, dropPos, Quaternion.identity);

            Debug.Log($"[ToolController] Chopped flora at {cell}.");
        }
        else
        {
            Debug.Log($"[ToolController] Nothing to chop at {cell}.");
        }
    }

    private void TryMine(Vector2Int cell, GridCell data)
    {
        if (data.occupant != null && data.occupant.CompareTag(RockTag))
        {
            Vector3 dropPos = IslandGridManager.Instance.CellToWorld(cell);
            Destroy(data.occupant);
            data.ClearOccupant();

            if (CurrentTool.DropPrefab != null)
                Instantiate(CurrentTool.DropPrefab, dropPos, Quaternion.identity);

            Debug.Log($"[ToolController] Mined rock at {cell}.");
        }
        else
        {
            Debug.Log($"[ToolController] Nothing to mine at {cell}.");
        }
    }

    private void TryDig(Vector2Int cell, GridCell data)
    {
        if (data.terrainType == TerrainType.Flat && !data.IsOccupied)
        {
            IslandGridManager.Instance.SetTerrainType(cell, TerrainType.Water);
            Debug.Log($"[ToolController] Dug water hole at {cell}.");
        }
        else
        {
            Debug.Log($"[ToolController] Cannot dig at {cell} (occupied or wrong terrain).");
        }
    }
}
