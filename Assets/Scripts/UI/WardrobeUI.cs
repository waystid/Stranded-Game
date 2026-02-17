using Synty.SidekickCharacters.API;
using Synty.SidekickCharacters.Database;
using Synty.SidekickCharacters.Database.DTO;
using Synty.SidekickCharacters.Enums;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Feature 007 Phase E — Wardrobe UI overlay.
///
/// Opened by WardrobeInteractable when the player activates the wardrobe zone.
/// Loads the current saved character state, lets the player navigate presets
/// and pick colors, then applies changes on confirm (no scene reload).
///
/// Wire all references in the Inspector.
/// Assign playerRoot to the HumanCustomPlayer GameObject in the scene.
/// </summary>
public class WardrobeUI : MonoBehaviour
{
    // ── Inspector References ──────────────────────────────────────────────────

    [Header("Panel")]
    [Tooltip("Root CanvasGroup — alpha 0 = hidden, alpha 1 = shown.")]
    public CanvasGroup wardrobePanel;

    [Header("Preset Labels")]
    public TextMeshProUGUI headLabel;
    public TextMeshProUGUI upperLabel;
    public TextMeshProUGUI lowerLabel;

    [Header("Preset Navigation Buttons")]
    public Button headPrevButton;
    public Button headNextButton;
    public Button upperPrevButton;
    public Button upperNextButton;
    public Button lowerPrevButton;
    public Button lowerNextButton;

    [Header("Color Swatches (assign Button arrays in Inspector)")]
    public Button[] skinSwatches;
    public Button[] hairSwatches;
    public Button[] primarySwatches;
    public Button[] secondarySwatches;

    [Header("Confirm/Cancel")]
    public Button applyButton;
    public Button cancelButton;

    [Header("Player")]
    [Tooltip("HumanCustomPlayer root — used to find SyntyCharacterLoader and CharacterCustomizer.")]
    public GameObject playerRoot;

    // ── Preset color palettes — override in Inspector or extend here ──────────
    // Each array entry is one swatch. Order matches skinSwatches / hairSwatches etc.

    [Header("Swatch Palettes")]
    public Color[] skinPalette    = new Color[] {
        new Color(0.95f, 0.82f, 0.72f),
        new Color(0.85f, 0.65f, 0.50f),
        new Color(0.70f, 0.48f, 0.35f),
        new Color(0.50f, 0.32f, 0.22f),
        new Color(0.35f, 0.20f, 0.13f),
        new Color(0.20f, 0.12f, 0.08f),
    };
    public Color[] hairPalette    = new Color[] {
        new Color(0.95f, 0.90f, 0.80f),
        new Color(0.70f, 0.55f, 0.35f),
        new Color(0.20f, 0.12f, 0.06f),
        new Color(0.10f, 0.08f, 0.08f),
        new Color(0.80f, 0.20f, 0.10f),
        new Color(0.20f, 0.30f, 0.80f),
    };
    public Color[] primaryPalette = new Color[] {
        new Color(0.20f, 0.40f, 0.75f),
        new Color(0.75f, 0.20f, 0.20f),
        new Color(0.20f, 0.65f, 0.30f),
        new Color(0.80f, 0.65f, 0.10f),
        new Color(0.55f, 0.20f, 0.75f),
        new Color(0.15f, 0.15f, 0.15f),
    };
    public Color[] secondaryPalette = new Color[] {
        new Color(0.90f, 0.90f, 0.90f),
        new Color(0.70f, 0.70f, 0.70f),
        new Color(0.40f, 0.40f, 0.40f),
        new Color(0.95f, 0.90f, 0.70f),
        new Color(0.95f, 0.55f, 0.20f),
        new Color(0.30f, 0.80f, 0.80f),
    };

    // ── Runtime state ─────────────────────────────────────────────────────────

    private PlayerCharacterData _workingData;
    private List<SidekickPartPreset> _headPresets  = new();
    private List<SidekickPartPreset> _upperPresets = new();
    private List<SidekickPartPreset> _lowerPresets = new();

    private SyntyCharacterLoader _loader;
    private CharacterCustomizer  _customizer;

    // ── Unity lifecycle ───────────────────────────────────────────────────────

    void Awake()
    {
        // Ensure panel starts hidden
        SetPanelVisible(false);

        // Wire buttons
        headPrevButton ?.onClick.AddListener(() => StepPreset(ref _workingData.headPresetIndex,      _headPresets,  -1, headLabel));
        headNextButton ?.onClick.AddListener(() => StepPreset(ref _workingData.headPresetIndex,      _headPresets,  +1, headLabel));
        upperPrevButton?.onClick.AddListener(() => StepPreset(ref _workingData.upperBodyPresetIndex, _upperPresets, -1, upperLabel));
        upperNextButton?.onClick.AddListener(() => StepPreset(ref _workingData.upperBodyPresetIndex, _upperPresets, +1, upperLabel));
        lowerPrevButton?.onClick.AddListener(() => StepPreset(ref _workingData.lowerBodyPresetIndex, _lowerPresets, -1, lowerLabel));
        lowerNextButton?.onClick.AddListener(() => StepPreset(ref _workingData.lowerBodyPresetIndex, _lowerPresets, +1, lowerLabel));

        applyButton ?.onClick.AddListener(OnApply);
        cancelButton?.onClick.AddListener(OnCancel);

        WireSwatches(skinSwatches,      skinPalette,      c => _workingData.skinColor      = c);
        WireSwatches(hairSwatches,      hairPalette,      c => _workingData.hairColor      = c);
        WireSwatches(primarySwatches,   primaryPalette,   c => _workingData.primaryColor   = c);
        WireSwatches(secondarySwatches, secondaryPalette, c => _workingData.secondaryColor = c);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Open the wardrobe UI. Loads the current saved state and pauses gameplay.
    /// </summary>
    public void Open()
    {
        // Find player components
        if (playerRoot == null)
            playerRoot = GameObject.FindWithTag("Player");

        if (playerRoot != null)
        {
            _loader     = playerRoot.GetComponent<SyntyCharacterLoader>();
            _customizer = playerRoot.GetComponent<CharacterCustomizer>();
        }

        // Load working copy of saved data
        _workingData = ScriptableObject.CreateInstance<PlayerCharacterData>();
        if (PlayerCharacterData.HasSavedData())
            _workingData.Load();

        // Init Synty preset lists (lightweight — DB only, no mesh spawn)
        var db = new DatabaseManager();
        _headPresets  = SidekickPartPreset.GetAllByGroup(db, PartGroup.Head);
        _upperPresets = SidekickPartPreset.GetAllByGroup(db, PartGroup.UpperBody);
        _lowerPresets = SidekickPartPreset.GetAllByGroup(db, PartGroup.LowerBody);

        // Populate labels
        UpdateLabel(headLabel,  _headPresets,  _workingData.headPresetIndex);
        UpdateLabel(upperLabel, _upperPresets, _workingData.upperBodyPresetIndex);
        UpdateLabel(lowerLabel, _lowerPresets, _workingData.lowerBodyPresetIndex);

        // Show panel and pause
        SetPanelVisible(true);
        Time.timeScale = 0f;

        Debug.Log("[WardrobeUI] Opened.");
    }

    /// <summary>
    /// Close the wardrobe UI without applying changes. Resumes gameplay.
    /// </summary>
    public void Close()
    {
        SetPanelVisible(false);
        Time.timeScale = 1f;

        // Free working data
        if (_workingData != null)
        {
            Destroy(_workingData);
            _workingData = null;
        }

        Debug.Log("[WardrobeUI] Closed.");
    }

    // ── Button handlers ───────────────────────────────────────────────────────

    private void OnApply()
    {
        if (_workingData == null) return;

        // Save to PlayerPrefs
        _workingData.Save();

        // Hot-swap mesh
        _loader?.SwapMesh(_workingData);

        // Re-apply colors (new SMRs exist after SwapMesh)
        _customizer?.ApplyColors(_workingData);

        Close();
        Debug.Log("[WardrobeUI] Applied changes.");
    }

    private void OnCancel()
    {
        Close();
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private void SetPanelVisible(bool visible)
    {
        if (wardrobePanel == null) return;
        wardrobePanel.alpha          = visible ? 1f : 0f;
        wardrobePanel.interactable   = visible;
        wardrobePanel.blocksRaycasts = visible;
    }

    private void StepPreset(ref int index, List<SidekickPartPreset> presets, int delta, TextMeshProUGUI label)
    {
        if (presets == null || presets.Count == 0) return;
        index = (index + delta + presets.Count) % presets.Count;
        UpdateLabel(label, presets, index);
    }

    private static void UpdateLabel(TextMeshProUGUI label, List<SidekickPartPreset> presets, int index)
    {
        if (label == null || presets == null || presets.Count == 0) return;
        index = Mathf.Clamp(index, 0, presets.Count - 1);
        label.text = presets[index].Name;
    }

    private void WireSwatches(Button[] swatches, Color[] palette, System.Action<Color> setter)
    {
        if (swatches == null) return;
        for (int i = 0; i < swatches.Length; i++)
        {
            if (swatches[i] == null) continue;
            int idx = i; // capture for closure
            Color col = (palette != null && idx < palette.Length) ? palette[idx] : Color.white;

            // Tint the button image to show the color
            var img = swatches[i].GetComponent<Image>();
            if (img != null) img.color = col;

            swatches[i].onClick.AddListener(() => setter(col));
        }
    }
}
