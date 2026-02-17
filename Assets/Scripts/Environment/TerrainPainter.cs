using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Terrain paint tool — press T to toggle paint mode.
/// Keys 1–5 select terrain type. Left-click paints the hovered cell.
/// Right-click erases (resets cell to Flat). Escape exits paint mode.
///
/// Visual feedback: persistent colored quad overlay on each painted cell,
/// using the same MaterialPropertyBlock pattern as GridCursor.
///
/// Requires GridCursor (auto-found) and IslandGridManager singleton.
/// </summary>
public class TerrainPainter : MonoBehaviour
{
    // ── Inspector ─────────────────────────────────────────────────────────────

    [Header("Mode")]
    [Tooltip("Key to toggle terrain paint mode.")]
    public KeyCode toggleKey = KeyCode.T;

    [Header("Terrain Colors")]
    [Tooltip("Overlay colors for each TerrainType (Flat, Cliff, Water, Beach, Radiation).")]
    public Color flatColor       = new Color(0f,    0f,    0f,    0f);    // transparent = remove overlay
    public Color cliffColor      = new Color(0.35f, 0.30f, 0.25f, 0.55f);
    public Color waterColor      = new Color(0.15f, 0.45f, 0.90f, 0.55f);
    public Color beachColor      = new Color(0.90f, 0.80f, 0.45f, 0.55f);
    public Color radiationColor  = new Color(0.20f, 0.85f, 0.20f, 0.65f);

    [Header("Overlay")]
    [Tooltip("Y offset above ground to prevent Z-fighting (must be above GridCursor quad).")]
    public float overlayYOffset = 0.04f;

    [Tooltip("Overlay quad world-unit size (should match GridCursor.quadSize).")]
    public float overlaySize = 0.96f;

    // ── State ─────────────────────────────────────────────────────────────────

    /// <summary>Whether the painter is currently active.</summary>
    public bool IsPainting { get; private set; }

    /// <summary>The terrain type that will be applied when painting.</summary>
    public TerrainType SelectedTerrain { get; private set; } = TerrainType.Flat;

    // ── Private ───────────────────────────────────────────────────────────────

    private GridCursor   _cursor;
    private Transform    _overlayRoot;
    private Material     _overlayMat;
    private static readonly int ColorPropID = Shader.PropertyToID("_Color");

    // Cell → overlay quad
    private readonly Dictionary<Vector2Int, GameObject> _overlays =
        new Dictionary<Vector2Int, GameObject>();

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    void Awake()
    {
        _cursor = GetComponent<GridCursor>();
        if (_cursor == null)
            _cursor = FindObjectOfType<GridCursor>();

        // Shared overlay material
        _overlayMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        _overlayMat.SetFloat("_Surface", 1f);
        _overlayMat.SetFloat("_Blend",   0f);
        _overlayMat.SetFloat("_ZWrite",  0f);
        _overlayMat.renderQueue = 3001;

        // Container for all overlay quads
        var rootGO = new GameObject("TerrainOverlays");
        rootGO.transform.SetParent(transform);
        _overlayRoot = rootGO.transform;
    }

    void Update()
    {
        HandleModeToggle();
        if (!IsPainting) return;

        HandleTerrainSelection();
        HandlePaintInput();
    }

    // ── Private Helpers ───────────────────────────────────────────────────────

    void HandleModeToggle()
    {
        if (Input.GetKeyDown(toggleKey) || Input.GetKeyDown(KeyCode.Escape))
        {
            IsPainting = !IsPainting;
            if (Input.GetKeyDown(KeyCode.Escape)) IsPainting = false;
            Debug.Log(IsPainting
                ? $"[TerrainPainter] Paint mode ON — selected: {SelectedTerrain}"
                : "[TerrainPainter] Paint mode OFF");
        }
    }

    void HandleTerrainSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetTerrain(TerrainType.Flat);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetTerrain(TerrainType.Cliff);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetTerrain(TerrainType.Water);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetTerrain(TerrainType.Beach);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SetTerrain(TerrainType.Radiation);
    }

    void SetTerrain(TerrainType t)
    {
        SelectedTerrain = t;
        Debug.Log($"[TerrainPainter] Selected terrain: {t}");
    }

    void HandlePaintInput()
    {
        if (_cursor == null || !_cursor.IsHovering) return;

        Vector2Int cell = _cursor.HoveredCell;

        if (Input.GetMouseButton(0))
            PaintCell(cell, SelectedTerrain);

        if (Input.GetMouseButton(1))
            PaintCell(cell, TerrainType.Flat);
    }

    void PaintCell(Vector2Int cell, TerrainType type)
    {
        if (IslandGridManager.Instance == null) return;

        // Write to grid data
        IslandGridManager.Instance.SetTerrainType(cell, type);

        // Update visual overlay
        if (type == TerrainType.Flat)
        {
            RemoveOverlay(cell);
        }
        else
        {
            SetOrCreateOverlay(cell, type);
        }
    }

    void SetOrCreateOverlay(Vector2Int cell, TerrainType type)
    {
        if (!_overlays.TryGetValue(cell, out GameObject quad) || quad == null)
        {
            quad = CreateOverlayQuad(cell);
            _overlays[cell] = quad;
        }

        // Reposition to current cell center
        Vector3 worldPos = IslandGridManager.Instance.CellToWorld(cell);
        worldPos.y += overlayYOffset;
        quad.transform.position = worldPos;
        quad.SetActive(true);

        // Tint by terrain type
        Color c = TerrainColor(type);
        var block = new MaterialPropertyBlock();
        var r = quad.GetComponent<Renderer>();
        r.GetPropertyBlock(block);
        block.SetColor(ColorPropID, c);
        r.SetPropertyBlock(block);
    }

    void RemoveOverlay(Vector2Int cell)
    {
        if (_overlays.TryGetValue(cell, out GameObject quad) && quad != null)
            quad.SetActive(false);
    }

    GameObject CreateOverlayQuad(Vector2Int cell)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.name = $"Overlay_{cell.x}_{cell.y}";
        go.transform.SetParent(_overlayRoot);
        go.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        go.transform.localScale    = new Vector3(overlaySize, overlaySize, 1f);
        Destroy(go.GetComponent<Collider>());
        go.GetComponent<Renderer>().sharedMaterial = _overlayMat;
        return go;
    }

    Color TerrainColor(TerrainType type)
    {
        switch (type)
        {
            case TerrainType.Flat:      return flatColor;
            case TerrainType.Cliff:     return cliffColor;
            case TerrainType.Water:     return waterColor;
            case TerrainType.Beach:     return beachColor;
            case TerrainType.Radiation: return radiationColor;
            default:                    return Color.white;
        }
    }
}
