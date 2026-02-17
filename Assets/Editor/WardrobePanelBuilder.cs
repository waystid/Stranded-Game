using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Feature 007 Phase E — Editor utility that builds the WardrobePanel UI layout
/// and wires all Inspector references on WardrobeUI.
///
/// Run via: Tools → CosmicColony → Build Wardrobe Panel
///
/// Idempotent: clears existing children before rebuilding.
/// </summary>
public static class WardrobePanelBuilder
{
    // ── Layout constants ──────────────────────────────────────────────────────

    const float CardW         = 500f;
    const float CardH         = 760f;
    const float ContentTop    = 330f;   // y from card centre to first element top
    const float RowStep       = 70f;
    const float ColorRowStep  = 65f;
    const float SwatchSize    = 40f;
    const float BtnH          = 54f;

    // Swatch x-positions (6 per row, 48px apart)
    static readonly float[] SwatchX = { -120f, -72f, -24f, 24f, 72f, 120f };

    // ── Entry point ───────────────────────────────────────────────────────────

    [MenuItem("Tools/CosmicColony/Build Wardrobe Panel")]
    public static void BuildPanel()
    {
        var panelGO = GameObject.Find("WardrobePanel");
        if (panelGO == null) { Debug.LogError("[WardrobePanelBuilder] WardrobePanel not found."); return; }

        var wardrobeUI = panelGO.GetComponent<WardrobeUI>();
        if (wardrobeUI == null) { Debug.LogError("[WardrobePanelBuilder] WardrobeUI not found on WardrobePanel."); return; }

        // Clear any previous children
        for (int i = panelGO.transform.childCount - 1; i >= 0; i--)
            Object.DestroyImmediate(panelGO.transform.GetChild(i).gameObject);

        // ── WardrobePanel → full-screen dim overlay ───────────────────────────
        var panelRT = EnsureRectTransform(panelGO);
        panelRT.anchorMin = Vector2.zero;
        panelRT.anchorMax = Vector2.one;
        panelRT.offsetMin = Vector2.zero;
        panelRT.offsetMax = Vector2.zero;
        EnsureImage(panelGO).color = new Color(0f, 0f, 0f, 0.70f);

        // ── Card (centred) ────────────────────────────────────────────────────
        var card = MakePanel(panelGO.transform, "Card", new Color(0.08f, 0.09f, 0.12f, 0.97f));
        StretchRT(card, Vector2.zero, new Vector2(CardW, CardH));

        float y = ContentTop;

        // ── Title ─────────────────────────────────────────────────────────────
        var titleGO = MakeLabel(card.transform, "Title", "WARDROBE", 26, Color.white, FontStyles.Bold);
        Place(titleGO, 0, y, 460, 46);
        y -= 55f;

        // ── Preset rows ───────────────────────────────────────────────────────
        Divider(card.transform, y + 18f);
        y -= 12f;

        var (headPrev,  headLabel,  headNext)  = PresetRow(card.transform, "Head",  y); y -= RowStep;
        var (upperPrev, upperLabel, upperNext) = PresetRow(card.transform, "Upper", y); y -= RowStep;
        var (lowerPrev, lowerLabel, lowerNext) = PresetRow(card.transform, "Lower", y); y -= RowStep;

        // ── Color rows ────────────────────────────────────────────────────────
        Divider(card.transform, y + 24f);
        y -= 14f;

        var skinSwatches      = ColorRow(card.transform, "Skin",      y); y -= ColorRowStep;
        var hairSwatches      = ColorRow(card.transform, "Hair",      y); y -= ColorRowStep;
        var primarySwatches   = ColorRow(card.transform, "Primary",   y); y -= ColorRowStep;
        var secondarySwatches = ColorRow(card.transform, "Secondary", y); y -= ColorRowStep;

        // ── Action buttons ────────────────────────────────────────────────────
        Divider(card.transform, y + 22f);
        y -= 20f;

        var cancelBtn = MakeButton(card.transform, "CancelButton", "Cancel",
            new Color(0.22f, 0.22f, 0.28f), new Color(0.85f, 0.85f, 0.85f));
        Place(cancelBtn, -112f, y, 190f, BtnH);

        var applyBtn = MakeButton(card.transform, "ApplyButton", "Apply",
            new Color(0.18f, 0.48f, 0.22f), Color.white);
        Place(applyBtn, 112f, y, 190f, BtnH);

        // ── Wire all references ───────────────────────────────────────────────
        wardrobeUI.headLabel   = headLabel;
        wardrobeUI.upperLabel  = upperLabel;
        wardrobeUI.lowerLabel  = lowerLabel;

        wardrobeUI.headPrevButton  = headPrev;
        wardrobeUI.headNextButton  = headNext;
        wardrobeUI.upperPrevButton = upperPrev;
        wardrobeUI.upperNextButton = upperNext;
        wardrobeUI.lowerPrevButton = lowerPrev;
        wardrobeUI.lowerNextButton = lowerNext;

        wardrobeUI.skinSwatches      = skinSwatches;
        wardrobeUI.hairSwatches      = hairSwatches;
        wardrobeUI.primarySwatches   = primarySwatches;
        wardrobeUI.secondarySwatches = secondarySwatches;

        wardrobeUI.applyButton  = applyBtn.GetComponent<Button>();
        wardrobeUI.cancelButton = cancelBtn.GetComponent<Button>();

        EditorUtility.SetDirty(wardrobeUI);
        EditorSceneManager.MarkSceneDirty(panelGO.scene);

        Debug.Log("[WardrobePanelBuilder] Panel built and all references wired.");
    }

    // ── Factory helpers ───────────────────────────────────────────────────────

    /// Create a GO with Image (no button).
    static GameObject MakePanel(Transform parent, string name, Color col)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = col;
        return go;
    }

    /// Create a TMP label.
    static GameObject MakeLabel(Transform parent, string name, string text, float size,
        Color col, FontStyles style = FontStyles.Normal)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        var tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text       = text;
        tmp.fontSize   = size;
        tmp.color      = col;
        tmp.fontStyle  = style;
        tmp.alignment  = TextAlignmentOptions.Center;
        tmp.enableAutoSizing = false;
        return go;
    }

    /// Create a button with a TMP child label.
    static GameObject MakeButton(Transform parent, string name, string label, Color bg, Color textCol)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);
        go.GetComponent<Image>().color = bg;

        var btn = go.GetComponent<Button>();
        var colors = btn.colors;
        colors.highlightedColor = new Color(bg.r + 0.15f, bg.g + 0.15f, bg.b + 0.15f);
        colors.pressedColor     = new Color(bg.r - 0.10f, bg.g - 0.10f, bg.b - 0.10f);
        btn.colors = colors;

        if (!string.IsNullOrEmpty(label))
        {
            var txtGO = MakeLabel(go.transform, "Text", label, 18, textCol, FontStyles.Bold);
            var rt = txtGO.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
        return go;
    }

    /// Create a thin horizontal divider line.
    static void Divider(Transform parent, float y)
    {
        var go = MakePanel(parent, "Divider", new Color(1f, 1f, 1f, 0.12f));
        Place(go, 0f, y, 460f, 1f);
    }

    /// Three-element preset row: [←] [label] [→], plus a small category label.
    static (Button prev, TextMeshProUGUI label, Button next) PresetRow(
        Transform parent, string catName, float y)
    {
        var row = new GameObject(catName + "Row", typeof(RectTransform));
        row.transform.SetParent(parent, false);
        Place(row, 0f, y, 460f, 58f);

        // Category name (left)
        var catLbl = MakeLabel(row.transform, catName + "Cat", catName,
            13f, new Color(0.60f, 0.62f, 0.68f));
        Place(catLbl, -185f, 10f, 80f, 20f);

        // Prev button
        var prevGO = MakeButton(row.transform, catName + "Prev", "<",
            new Color(0.14f, 0.15f, 0.20f), Color.white);
        Place(prevGO, -98f, -8f, 44f, 44f);

        // Preset name label (centre)
        var nameLbl = MakeLabel(row.transform, catName + "Label", "—", 15f, Color.white);
        Place(nameLbl, 0f, -8f, 180f, 44f);

        // Next button
        var nextGO = MakeButton(row.transform, catName + "Next", ">",
            new Color(0.14f, 0.15f, 0.20f), Color.white);
        Place(nextGO, 98f, -8f, 44f, 44f);

        return (prevGO.GetComponent<Button>(), nameLbl.GetComponent<TextMeshProUGUI>(), nextGO.GetComponent<Button>());
    }

    /// Color swatch row: category label + 6 coloured swatch buttons.
    static Button[] ColorRow(Transform parent, string catName, float y)
    {
        var row = new GameObject(catName + "ColorRow", typeof(RectTransform));
        row.transform.SetParent(parent, false);
        Place(row, 0f, y, 460f, 50f);

        var catLbl = MakeLabel(row.transform, catName + "Cat", catName,
            13f, new Color(0.60f, 0.62f, 0.68f));
        Place(catLbl, -185f, 10f, 80f, 20f);

        var swatches = new Button[6];
        for (int i = 0; i < 6; i++)
        {
            var sw = MakeButton(row.transform, catName + "Swatch" + i, "",
                Color.white, Color.white);
            Place(sw, SwatchX[i], -4f, SwatchSize, SwatchSize);
            swatches[i] = sw.GetComponent<Button>();
        }
        return swatches;
    }

    // ── RectTransform utilities ───────────────────────────────────────────────

    /// Centre-pivot element at (x,y) with given size.
    static void Place(GameObject go, float x, float y, float w, float h)
    {
        var rt = EnsureRectTransform(go);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot     = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(x, y);
        rt.sizeDelta = new Vector2(w, h);
    }

    /// Centre-pivot, fixed size (for card).
    static void StretchRT(GameObject go, Vector2 pos, Vector2 size)
    {
        var rt = EnsureRectTransform(go);
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot     = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }

    static RectTransform EnsureRectTransform(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>();
        if (rt == null) rt = go.AddComponent<RectTransform>();
        return rt;
    }

    static Image EnsureImage(GameObject go)
    {
        var img = go.GetComponent<Image>();
        if (img == null) img = go.AddComponent<Image>();
        return img;
    }
}
