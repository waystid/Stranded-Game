using UnityEngine;

/// <summary>
/// Flora placement tool — press F to toggle placement mode.
/// Keys 1–9 select flora prefab. Left-click places on hovered cell (if walkable + empty).
/// Right-click removes flora from hovered cell. Escape exits.
///
/// Placed flora becomes a child of FloraRoot (the Island transform) so it
/// inherits the 45° Y rotation. Sets cell.occupant in IslandGridManager.
///
/// Requires GridCursor (auto-found) and IslandGridManager singleton.
/// </summary>
public class FloraPlacement : MonoBehaviour
{
    // ── Inspector ─────────────────────────────────────────────────────────────

    [Header("Mode")]
    [Tooltip("Key to toggle flora placement mode.")]
    public KeyCode toggleKey = KeyCode.F;

    [Header("Flora Prefabs")]
    [Tooltip("Pandazole prefabs to cycle through. Assign in Inspector.")]
    public GameObject[] floraPrefabs;

    [Tooltip("Parent transform for all placed flora (set to Island transform).")]
    public Transform floraRoot;

    [Header("Placement")]
    [Tooltip("Y position for placed flora (ground level on island).")]
    public float groundY = 0f;

    // ── State ─────────────────────────────────────────────────────────────────

    /// <summary>Whether the placer is currently active.</summary>
    public bool IsPlacing { get; private set; }

    /// <summary>Index into floraPrefabs for the currently selected flora.</summary>
    public int SelectedIndex { get; private set; } = 0;

    // ── Private ───────────────────────────────────────────────────────────────

    private GridCursor _cursor;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    void Awake()
    {
        _cursor = GetComponent<GridCursor>();
        if (_cursor == null)
            _cursor = FindObjectOfType<GridCursor>();

        // Default floraRoot to Island if not set
        if (floraRoot == null)
        {
            var island = GameObject.Find("Island");
            if (island != null)
                floraRoot = island.transform;
        }
    }

    void Update()
    {
        HandleModeToggle();
        if (!IsPlacing) return;

        HandlePrefabSelection();
        HandlePlacementInput();
    }

    // ── Private Helpers ───────────────────────────────────────────────────────

    void HandleModeToggle()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            IsPlacing = !IsPlacing;
            Debug.Log(IsPlacing
                ? $"[FloraPlacement] Place mode ON — selected: {CurrentPrefabName()}"
                : "[FloraPlacement] Place mode OFF");
        }
        if (Input.GetKeyDown(KeyCode.Escape) && IsPlacing)
        {
            IsPlacing = false;
            Debug.Log("[FloraPlacement] Place mode OFF");
        }
    }

    void HandlePrefabSelection()
    {
        if (floraPrefabs == null || floraPrefabs.Length == 0) return;

        // Keys 1–9 select prefab by index
        for (int i = 0; i < Mathf.Min(floraPrefabs.Length, 9); i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectedIndex = i;
                Debug.Log($"[FloraPlacement] Selected: {CurrentPrefabName()}");
            }
        }

        // [ / ] cycle through all prefabs
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            SelectedIndex = (SelectedIndex - 1 + floraPrefabs.Length) % floraPrefabs.Length;
            Debug.Log($"[FloraPlacement] Selected: {CurrentPrefabName()}");
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            SelectedIndex = (SelectedIndex + 1) % floraPrefabs.Length;
            Debug.Log($"[FloraPlacement] Selected: {CurrentPrefabName()}");
        }
    }

    void HandlePlacementInput()
    {
        if (_cursor == null || !_cursor.IsHovering) return;

        Vector2Int cell = _cursor.HoveredCell;

        if (Input.GetMouseButtonDown(0))
            TryPlaceFlora(cell);

        if (Input.GetMouseButtonDown(1))
            TryRemoveFlora(cell);
    }

    void TryPlaceFlora(Vector2Int cell)
    {
        if (IslandGridManager.Instance == null) return;
        if (floraPrefabs == null || floraPrefabs.Length == 0) return;

        GameObject prefab = floraPrefabs[SelectedIndex];
        if (prefab == null) return;

        GridCell data = IslandGridManager.Instance.GetCell(cell);

        if (!data.IsWalkable)
        {
            Debug.Log($"[FloraPlacement] Cell {cell} is not walkable — skipping.");
            return;
        }

        if (data.IsOccupied)
        {
            Debug.Log($"[FloraPlacement] Cell {cell} is already occupied — skipping.");
            return;
        }

        // Snap to grid center at ground level
        Vector3 worldPos = IslandGridManager.Instance.CellToWorld(cell);
        worldPos.y = groundY;

        GameObject placed = Object.Instantiate(prefab, worldPos, Quaternion.identity, floraRoot);
        placed.name = $"{prefab.name}_{cell.x}_{cell.y}";

        // Register occupant
        data.occupant = placed;

        Debug.Log($"[FloraPlacement] Placed {placed.name} at cell {cell}");
    }

    void TryRemoveFlora(Vector2Int cell)
    {
        if (IslandGridManager.Instance == null) return;

        GridCell data = IslandGridManager.Instance.GetCell(cell);

        if (!data.IsOccupied)
        {
            Debug.Log($"[FloraPlacement] Cell {cell} has no occupant.");
            return;
        }

        string removedName = data.occupant.name;
        Object.Destroy(data.occupant);
        data.ClearOccupant();

        Debug.Log($"[FloraPlacement] Removed {removedName} from cell {cell}");
    }

    string CurrentPrefabName()
    {
        if (floraPrefabs == null || floraPrefabs.Length == 0) return "none";
        var p = floraPrefabs[SelectedIndex];
        return p != null ? p.name : "null";
    }
}
