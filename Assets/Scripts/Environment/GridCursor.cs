using UnityEngine;

/// <summary>
/// Highlights the island grid cell under the player (or mouse/screen center).
/// Raycasts against the island ground plane, queries IslandGridManager.WorldToCell(),
/// then moves a highlight quad to the snapped cell center.
///
/// Attach to any GameObject in the scene — it will manage its own highlight quad child.
/// Requires: IslandGridManager singleton, a LayerMask set to the island ground layer.
/// </summary>
public class GridCursor : MonoBehaviour
{
    // ── Inspector ─────────────────────────────────────────────────────────────

    [Header("Raycast")]
    [Tooltip("Layer(s) the cursor ray hits. Must include the island ground layer.")]
    public LayerMask groundLayer = ~0;

    [Tooltip("Use mouse position for ray (true) or screen center (false).")]
    public bool useMousePosition = true;

    [Header("Highlight Quad")]
    [Tooltip("Size of the highlight quad in world units (should match CellSize).")]
    public float quadSize = 0.98f;

    [Tooltip("Y offset above the ground surface to prevent Z-fighting.")]
    public float yOffset = 0.02f;

    [Tooltip("Highlight color when cell is empty/walkable.")]
    public Color walkableColor = new Color(1f, 1f, 1f, 0.4f);

    [Tooltip("Highlight color when cell is occupied or unwalkable.")]
    public Color blockedColor  = new Color(1f, 0.2f, 0.2f, 0.4f);

    [Header("Visibility")]
    [Tooltip("Toggle cursor visibility from DevConsole or gameplay code.")]
    public bool isVisible = true;

    // ── Runtime State ─────────────────────────────────────────────────────────

    /// <summary>The grid cell currently under the cursor. (-999,-999) = no hit.</summary>
    public Vector2Int HoveredCell { get; private set; } = new Vector2Int(-999, -999);

    /// <summary>World-space center of the hovered cell.</summary>
    public Vector3 HoveredWorldPos { get; private set; }

    /// <summary>True when the cursor is hovering over a valid island cell.</summary>
    public bool IsHovering { get; private set; }

    // ── Private ───────────────────────────────────────────────────────────────

    private GameObject  _quad;
    private Renderer    _quadRenderer;
    private Camera      _cam;
    private static readonly int ColorPropID = Shader.PropertyToID("_Color");

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    void Awake()
    {
        _cam = Camera.main;
        CreateHighlightQuad();
    }

    void Update()
    {
        if (!isVisible)
        {
            SetQuadVisible(false);
            IsHovering = false;
            return;
        }

        if (IslandGridManager.Instance == null)
            return;

        Ray ray = BuildRay();
        if (Physics.Raycast(ray, out RaycastHit hit, 200f, groundLayer))
        {
            Vector2Int cell     = IslandGridManager.Instance.WorldToCell(hit.point);
            Vector3    worldPos = IslandGridManager.Instance.CellToWorld(cell);
            worldPos.y         += yOffset;

            HoveredCell     = cell;
            HoveredWorldPos = worldPos;
            IsHovering      = true;

            _quad.transform.position = worldPos;
            SetQuadVisible(true);
            UpdateQuadColor(cell);
        }
        else
        {
            HoveredCell = new Vector2Int(-999, -999);
            IsHovering  = false;
            SetQuadVisible(false);
        }
    }

    // ── Private Helpers ───────────────────────────────────────────────────────

    Ray BuildRay()
    {
        if (_cam == null)
            _cam = Camera.main;

        if (useMousePosition)
            return _cam.ScreenPointToRay(Input.mousePosition);

        // Screen center
        return _cam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
    }

    void UpdateQuadColor(Vector2Int cell)
    {
        GridCell data = IslandGridManager.Instance.GetCell(cell);
        Color c = (data != null && data.IsWalkable && !data.IsOccupied)
            ? walkableColor
            : blockedColor;

        if (_quadRenderer != null)
        {
            var block = new MaterialPropertyBlock();
            _quadRenderer.GetPropertyBlock(block);
            block.SetColor(ColorPropID, c);
            _quadRenderer.SetPropertyBlock(block);
        }
    }

    void SetQuadVisible(bool visible)
    {
        if (_quad != null && _quad.activeSelf != visible)
            _quad.SetActive(visible);
    }

    void CreateHighlightQuad()
    {
        _quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        _quad.name = "GridCursorHighlight";
        _quad.transform.SetParent(transform);
        _quad.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        _quad.transform.localScale    = new Vector3(quadSize, quadSize, 1f);

        // Remove collider — this is purely visual
        Destroy(_quad.GetComponent<Collider>());

        // Assign a semi-transparent material
        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetFloat("_Surface", 1f);          // Transparent surface type
        mat.SetFloat("_Blend", 0f);            // Alpha blend
        mat.SetFloat("_ZWrite", 0f);
        mat.renderQueue = 3000;
        mat.color = walkableColor;
        _quad.GetComponent<Renderer>().sharedMaterial = mat;

        _quadRenderer = _quad.GetComponent<Renderer>();
        SetQuadVisible(false);
    }
}
