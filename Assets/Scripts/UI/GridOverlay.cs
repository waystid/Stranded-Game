using UnityEngine;

/// <summary>
/// Draws a flat XZ grid over the play space using GL.Lines in OnRenderObject.
/// Enable/disable this component to show/hide the grid.
/// All parameters are public so DevConsole can drive them at runtime.
/// </summary>
[RequireComponent(typeof(Camera))]
public class GridOverlay : MonoBehaviour
{
    [Header("Grid Shape")]
    public float WorldSize  = 64f;   // total width/depth of the island
    public float CellSize   = 4f;    // size of each grid square in world units
    public float YOffset    = 0.05f; // height above ground plane

    [Header("Island Alignment")]
    [Tooltip("When set, the grid is drawn in this transform's local space (set to the Island root rotated 45°).")]
    public Transform IslandRoot;

    [Header("Appearance")]
    public Color GridColor     = new Color(1f, 1f, 1f, 0.25f);
    public Color AxisColor     = new Color(0.4f, 0.8f, 1f, 0.6f); // cyan for center axes
    public float AxisThickness = 1f; // visual note only (GL.LINES is always 1px)

    // ── Internal ──────────────────────────────────────────────────────────────
    Material _mat;

    void Awake()
    {
        // "Hidden/Internal-Colored" is always available in Unity; supports alpha blending
        _mat = new Material(Shader.Find("Hidden/Internal-Colored"));
        _mat.hideFlags = HideFlags.HideAndDontSave;
        _mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        _mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        _mat.SetInt("_Cull",     (int)UnityEngine.Rendering.CullMode.Off);
        _mat.SetInt("_ZWrite",   0);
        _mat.SetInt("_ZTest",    (int)UnityEngine.Rendering.CompareFunction.LessEqual);
    }

    void OnDestroy()
    {
        if (_mat != null)
            DestroyImmediate(_mat);
    }

    // Called after the camera finishes rendering; draws on top of scene geometry
    void OnRenderObject()
    {
        if (_mat == null) return;

        _mat.SetPass(0);
        GL.PushMatrix();

        // Draw in Island local space so the grid aligns with the rotated island
        if (IslandRoot != null)
            GL.MultMatrix(IslandRoot.localToWorldMatrix);

        float half  = WorldSize * 0.5f;
        int   cells = Mathf.Max(1, Mathf.RoundToInt(WorldSize / CellSize));

        GL.Begin(GL.LINES);

        for (int i = 0; i <= cells; i++)
        {
            float t = -half + i * CellSize;

            // Use axis color for the center lines (i == cells/2)
            bool isCenterX = Mathf.Approximately(t, 0f);
            bool isCenterZ = Mathf.Approximately(t, 0f);
            Color col = (isCenterX || isCenterZ) ? AxisColor : GridColor;

            GL.Color(col);

            // Line along Z at position X = t
            GL.Vertex3(t,    YOffset, -half);
            GL.Vertex3(t,    YOffset,  half);

            // Line along X at position Z = t
            GL.Vertex3(-half, YOffset, t);
            GL.Vertex3( half, YOffset, t);
        }

        GL.End();
        GL.PopMatrix();
    }
}
