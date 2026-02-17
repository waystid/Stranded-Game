using UnityEngine;

/// <summary>
/// Manages grid-snapped furniture placement with ghost preview.
///
/// States:
///   Idle    — normal gameplay, no placement active
///   Placing — ghost follows cursor; E to confirm, Escape to cancel
///
/// To use: call BeginPlacement(furnitureItem) from FurnitureItem.Use().
/// Add this component to a persistent Manager GameObject in the scene.
/// </summary>
public class PlacementController : MonoBehaviour
{
    public static PlacementController Instance { get; private set; }

    [Header("Ghost")]
    [Tooltip("Material used for the ghost preview (semi-transparent). Auto-created if null.")]
    public Material GhostMaterial;

    [Tooltip("Alpha for valid placement ghost.")]
    [Range(0f, 1f)]
    public float GhostAlphaValid = 0.4f;

    [Tooltip("Alpha for invalid placement ghost.")]
    [Range(0f, 1f)]
    public float GhostAlphaInvalid = 0.5f;

    [Tooltip("Tint color when placement is valid.")]
    public Color ValidColor = new Color(0.3f, 1f, 0.3f, 1f);

    [Tooltip("Tint color when placement is invalid (occupied / out of bounds).")]
    public Color InvalidColor = new Color(1f, 0.2f, 0.2f, 1f);

    // ── State ─────────────────────────────────────────────────────────────────

    public bool IsPlacing { get; private set; }

    private FurnitureItem _activeItem;
    private GameObject    _ghost;
    private bool          _placementValid;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        // Auto-create ghost material if not assigned
        if (GhostMaterial == null)
        {
            GhostMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            if (GhostMaterial != null)
            {
                GhostMaterial.SetFloat("_Surface", 1f);   // Transparent
                GhostMaterial.SetFloat("_Blend", 0f);     // Alpha blend
                GhostMaterial.SetFloat("_ZWrite", 0f);
                GhostMaterial.renderQueue = 3000;
            }
        }
    }

    void Update()
    {
        if (!IsPlacing) return;

        UpdateGhostPosition();

        if (Input.GetKeyDown(KeyCode.E) && _placementValid)
            ConfirmPlacement();
        else if (Input.GetKeyDown(KeyCode.Escape))
            CancelPlacement();
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Begin placing a furniture item. Spawns a ghost and enters Placing state.
    /// Call from FurnitureItem.Use().
    /// </summary>
    public void BeginPlacement(FurnitureItem item)
    {
        if (IsPlacing) CancelPlacement();

        if (item.PlacedPrefab == null)
        {
            Debug.LogWarning($"[PlacementController] FurnitureItem '{item.ItemName}' has no PlacedPrefab assigned.");
            return;
        }

        _activeItem = item;
        IsPlacing   = true;

        // Spawn ghost
        _ghost = Instantiate(item.PlacedPrefab);
        _ghost.name = "[Ghost] " + item.ItemName;
        ApplyGhostMaterial(_ghost, GhostAlphaValid, ValidColor);

        // Disable colliders on ghost so it doesn't interfere with raycast
        foreach (Collider c in _ghost.GetComponentsInChildren<Collider>())
            c.enabled = false;

        Debug.Log($"[PlacementController] Placing '{item.ItemName}'. E=confirm, Escape=cancel.");
    }

    // ── Private ───────────────────────────────────────────────────────────────

    private void UpdateGhostPosition()
    {
        if (_ghost == null || IslandGridManager.Instance == null) return;

        // Get hovered cell from GridCursor or default to (0,0)
        Vector2Int hoveredCell = GridCursor.Instance != null && GridCursor.Instance.IsHovering
            ? GridCursor.Instance.HoveredCell
            : Vector2Int.zero;

        Vector3 snapPos = IslandGridManager.Instance.CellToWorld(hoveredCell);
        _ghost.transform.position = snapPos;

        // Validate all footprint cells
        _placementValid = ValidateFootprint(hoveredCell);

        Color tint  = _placementValid ? ValidColor   : InvalidColor;
        float alpha = _placementValid ? GhostAlphaValid : GhostAlphaInvalid;
        ApplyGhostMaterial(_ghost, alpha, tint);
    }

    private bool ValidateFootprint(Vector2Int originCell)
    {
        if (_activeItem == null || IslandGridManager.Instance == null) return false;

        Vector2Int footprint = _activeItem.Footprint;
        for (int dx = 0; dx < footprint.x; dx++)
        {
            for (int dz = 0; dz < footprint.y; dz++)
            {
                Vector2Int cell = new Vector2Int(originCell.x + dx, originCell.y + dz);
                if (!IslandGridManager.Instance.IsInBounds(cell)) return false;
                GridCell data = IslandGridManager.Instance.GetCell(cell);
                if (data == null || !data.IsWalkable) return false;
            }
        }
        return true;
    }

    private void ConfirmPlacement()
    {
        if (_activeItem == null || _ghost == null) return;

        Vector2Int originCell = IslandGridManager.Instance.WorldToCell(_ghost.transform.position);
        Vector3    snapPos    = IslandGridManager.Instance.CellToWorld(originCell);

        // Instantiate real prefab
        GameObject placed = Instantiate(_activeItem.PlacedPrefab, snapPos, _ghost.transform.rotation);
        placed.name = _activeItem.ItemName;

        // Register footprint cells
        Vector2Int footprint = _activeItem.Footprint;
        for (int dx = 0; dx < footprint.x; dx++)
        {
            for (int dz = 0; dz < footprint.y; dz++)
            {
                Vector2Int cell = new Vector2Int(originCell.x + dx, originCell.y + dz);
                GridCell data   = IslandGridManager.Instance.GetCell(cell);
                data.occupant   = placed;
            }
        }

        Debug.Log($"[PlacementController] Placed '{_activeItem.ItemName}' at {originCell}.");

        CleanupGhost();
        IsPlacing   = false;
        _activeItem = null;
    }

    private void CancelPlacement()
    {
        Debug.Log($"[PlacementController] Placement cancelled.");
        CleanupGhost();
        IsPlacing   = false;
        _activeItem = null;
    }

    private void CleanupGhost()
    {
        if (_ghost != null)
        {
            Destroy(_ghost);
            _ghost = null;
        }
    }

    private void ApplyGhostMaterial(GameObject go, float alpha, Color tint)
    {
        if (GhostMaterial == null) return;

        Color c = tint;
        c.a = alpha;

        foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
        {
            r.sharedMaterial = GhostMaterial;
            var block = new MaterialPropertyBlock();
            r.GetPropertyBlock(block);
            block.SetColor("_BaseColor", c);
            r.SetPropertyBlock(block);
        }
    }
}
