using Synty.SidekickCharacters.API;
using Synty.SidekickCharacters.Database;
using Synty.SidekickCharacters.Database.DTO;
using Synty.SidekickCharacters.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Phase B — Character Creator scene controller.
/// Drives a live Synty character preview with Head / UpperBody / LowerBody preset pickers
/// and 4-channel color swatches. Saves all choices to PlayerPrefs via PlayerCharacterData.
///
/// Replaces CharacterCreatorUI.cs — attach to a root GameObject in the CharacterCreator scene.
/// </summary>
public class CharacterCreatorController : MonoBehaviour
{
    // ── Preview ───────────────────────────────────────────────────────────────

    [Header("Preview")]
    [Tooltip("Empty transform around which the preview character rotates.")]
    public Transform previewRoot;
    public float rotationSpeed = 40f;

    // ── Name ──────────────────────────────────────────────────────────────────

    [Header("Name")]
    public InputField nameInput;

    // ── Preset labels ─────────────────────────────────────────────────────────

    [Header("Part Preset Labels")]
    public Text headPresetLabel;
    public Text upperBodyPresetLabel;
    public Text lowerBodyPresetLabel;

    // ── Color swatches ────────────────────────────────────────────────────────

    [Header("Color Swatch Buttons (8 per row)")]
    public Button[] skinButtons;
    public Button[] hairButtons;
    public Button[] primaryButtons;
    public Button[] secondaryButtons;

    // Preset palettes
    private static readonly Color[] SkinPalette =
    {
        new Color(0.95f, 0.82f, 0.72f),
        new Color(0.85f, 0.65f, 0.50f),
        new Color(0.72f, 0.50f, 0.35f),
        new Color(0.55f, 0.35f, 0.22f),
        new Color(0.35f, 0.22f, 0.12f),
        new Color(0.65f, 0.82f, 0.95f),
        new Color(0.58f, 0.88f, 0.62f),
        new Color(0.82f, 0.65f, 0.95f),
    };

    private static readonly Color[] HairPalette =
    {
        new Color(0.95f, 0.95f, 0.92f),
        new Color(0.88f, 0.78f, 0.45f),
        new Color(0.72f, 0.45f, 0.22f),
        new Color(0.20f, 0.12f, 0.06f),
        new Color(0.08f, 0.08f, 0.08f),
        new Color(0.80f, 0.20f, 0.20f),
        new Color(0.20f, 0.50f, 0.90f),
        new Color(0.60f, 0.22f, 0.80f),
    };

    private static readonly Color[] OutfitPalette =
    {
        new Color(0.20f, 0.40f, 0.78f),
        new Color(0.78f, 0.20f, 0.20f),
        new Color(0.22f, 0.65f, 0.30f),
        new Color(0.80f, 0.62f, 0.15f),
        new Color(0.50f, 0.20f, 0.72f),
        new Color(0.18f, 0.72f, 0.75f),
        new Color(0.92f, 0.92f, 0.92f),
        new Color(0.12f, 0.12f, 0.12f),
    };

    // ── Scene ─────────────────────────────────────────────────────────────────

    [Header("Scene")]
    public string nextScene = "SandboxShowcase";

    // ── Shader IDs ────────────────────────────────────────────────────────────

    private static readonly int ID_Skin      = Shader.PropertyToID("_Color_Skin");
    private static readonly int ID_Hair      = Shader.PropertyToID("_Color_Hair");
    private static readonly int ID_Primary   = Shader.PropertyToID("_Color_Primary");
    private static readonly int ID_Secondary = Shader.PropertyToID("_Color_Secondary");

    // ── Runtime state ─────────────────────────────────────────────────────────

    private DatabaseManager _dbManager;
    private SidekickRuntime _sidekickRuntime;
    private Dictionary<CharacterPartType, Dictionary<string, SidekickPart>> _partLibrary;

    private List<SidekickPartPreset> _headPresets    = new List<SidekickPartPreset>();
    private List<SidekickPartPreset> _upperPresets   = new List<SidekickPartPreset>();
    private List<SidekickPartPreset> _lowerPresets   = new List<SidekickPartPreset>();

    private int _headIdx, _upperIdx, _lowerIdx;
    private Color _skin, _hair, _primary, _secondary;

    private GameObject _previewChar;
    private const string PREVIEW_NAME = "CC_PreviewChar";

    private bool _ready = false;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    void Start()
    {
        // Load saved state or defaults
        if (PlayerCharacterData.HasSavedData())
        {
            var d = ScriptableObject.CreateInstance<PlayerCharacterData>();
            d.Load();
            _headIdx  = d.headPresetIndex;
            _upperIdx = d.upperBodyPresetIndex;
            _lowerIdx = d.lowerBodyPresetIndex;
            _skin      = d.skinColor;
            _hair      = d.hairColor;
            _primary   = d.primaryColor;
            _secondary = d.secondaryColor;
            if (nameInput != null) nameInput.text = d.characterName;
        }
        else
        {
            _headIdx  = 0;
            _upperIdx = 0;
            _lowerIdx = 0;
            _skin      = SkinPalette[1];
            _hair      = HairPalette[3];
            _primary   = OutfitPalette[0];
            _secondary = OutfitPalette[6];
            if (nameInput != null) nameInput.text = "Colonist";
        }

        // Auto-find UI elements by name if not set via inspector
        AutoWire();

        // Wire color swatches (auto-find arrays if inspector arrays are null/empty)
        if (skinButtons == null      || skinButtons.Length      == 0) skinButtons      = FindSwatchButtons("SkinSwatches");
        if (hairButtons == null      || hairButtons.Length      == 0) hairButtons      = FindSwatchButtons("HairSwatches");
        if (primaryButtons == null   || primaryButtons.Length   == 0) primaryButtons   = FindSwatchButtons("PrimarySwatches");
        if (secondaryButtons == null || secondaryButtons.Length == 0) secondaryButtons = FindSwatchButtons("SecondarySwatches");

        SetupSwatches(skinButtons,      SkinPalette,   c => { _skin      = c; RefreshColors(); });
        SetupSwatches(hairButtons,      HairPalette,   c => { _hair      = c; RefreshColors(); });
        SetupSwatches(primaryButtons,   OutfitPalette, c => { _primary   = c; RefreshColors(); });
        SetupSwatches(secondaryButtons, OutfitPalette, c => { _secondary = c; RefreshColors(); });

        // Init Synty runtime
        InitSyntyRuntime();
    }

    void Update()
    {
        if (previewRoot != null)
            previewRoot.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    // ── Synty init ────────────────────────────────────────────────────────────

    private void InitSyntyRuntime()
    {
        _dbManager = new DatabaseManager();

        GameObject baseModel = Resources.Load<GameObject>("Meshes/SK_BaseModel");
        Material   baseMat   = Resources.Load<Material>("Materials/M_BaseMaterial");

        if (baseModel == null || baseMat == null)
        {
            Debug.LogError("[CharacterCreatorController] Could not load SK_BaseModel or M_BaseMaterial from Resources. Check Synty pack is imported.");
            return;
        }

        _sidekickRuntime = new SidekickRuntime(baseModel, baseMat, null, _dbManager);
        SidekickRuntime.PopulateToolData(_sidekickRuntime);
        _partLibrary = _sidekickRuntime.MappedPartDictionary;

        // Load presets per group
        _headPresets  = SidekickPartPreset.GetAllByGroup(_dbManager, PartGroup.Head);
        _upperPresets = SidekickPartPreset.GetAllByGroup(_dbManager, PartGroup.UpperBody);
        _lowerPresets = SidekickPartPreset.GetAllByGroup(_dbManager, PartGroup.LowerBody);

        if (_headPresets.Count == 0 || _upperPresets.Count == 0 || _lowerPresets.Count == 0)
        {
            Debug.LogWarning("[CharacterCreatorController] One or more preset groups is empty. " +
                             "Ensure a Synty content pack is installed.");
        }

        // Clamp saved indices to available range
        _headIdx  = Mathf.Clamp(_headIdx,  0, Mathf.Max(0, _headPresets.Count  - 1));
        _upperIdx = Mathf.Clamp(_upperIdx, 0, Mathf.Max(0, _upperPresets.Count - 1));
        _lowerIdx = Mathf.Clamp(_lowerIdx, 0, Mathf.Max(0, _lowerPresets.Count - 1));

        _ready = true;
        UpdateModel();
    }

    // ── Part picker ───────────────────────────────────────────────────────────

    public void OnHeadPrev()  { if (!_ready) return; _headIdx  = Mod(_headIdx  - 1, _headPresets.Count);  UpdateModel(); }
    public void OnHeadNext()  { if (!_ready) return; _headIdx  = Mod(_headIdx  + 1, _headPresets.Count);  UpdateModel(); }
    public void OnUpperPrev() { if (!_ready) return; _upperIdx = Mod(_upperIdx - 1, _upperPresets.Count); UpdateModel(); }
    public void OnUpperNext() { if (!_ready) return; _upperIdx = Mod(_upperIdx + 1, _upperPresets.Count); UpdateModel(); }
    public void OnLowerPrev() { if (!_ready) return; _lowerIdx = Mod(_lowerIdx - 1, _lowerPresets.Count); UpdateModel(); }
    public void OnLowerNext() { if (!_ready) return; _lowerIdx = Mod(_lowerIdx + 1, _lowerPresets.Count); UpdateModel(); }

    // ── Model update ──────────────────────────────────────────────────────────

    private void UpdateModel()
    {
        if (!_ready) return;

        RefreshPresetLabels();

        List<SkinnedMeshRenderer> parts = new List<SkinnedMeshRenderer>();

        // Collect parts from each preset group
        CollectPresetParts(_headPresets,  _headIdx,  parts);
        CollectPresetParts(_upperPresets, _upperIdx, parts);
        CollectPresetParts(_lowerPresets, _lowerIdx, parts);

        if (parts.Count == 0)
        {
            Debug.LogWarning("[CharacterCreatorController] No parts found — check preset data.");
            return;
        }

        // Destroy previous preview
        if (_previewChar != null)
            Destroy(_previewChar);

        // Create new character via Synty runtime
        _previewChar = _sidekickRuntime.CreateCharacter(PREVIEW_NAME, parts, false, true);

        if (_previewChar == null) return;

        // Parent to preview root so it rotates with it
        if (previewRoot != null)
        {
            _previewChar.transform.SetParent(previewRoot);
            _previewChar.transform.localPosition = Vector3.zero;
            _previewChar.transform.localRotation = Quaternion.identity;
        }

        RefreshColors();
    }

    private void CollectPresetParts(List<SidekickPartPreset> presets, int idx, List<SkinnedMeshRenderer> result)
    {
        if (presets.Count == 0) return;
        var preset = presets[idx];
        var rows   = SidekickPartPresetRow.GetAllByPreset(_dbManager, preset);

        foreach (var row in rows)
        {
            if (string.IsNullOrEmpty(row.PartName)) continue;

            try
            {
                string typeName = Synty.SidekickCharacters.Utils.CharacterPartTypeUtils.GetTypeNameFromShortcode(row.PartType);
                var type = Enum.Parse<CharacterPartType>(typeName);
                if (!_partLibrary.ContainsKey(type)) continue;
                var partDict = _partLibrary[type];
                if (!partDict.ContainsKey(row.PartName)) continue;
                var partGO = partDict[row.PartName].GetPartModel();
                if (partGO == null) continue;
                var smr = partGO.GetComponentInChildren<SkinnedMeshRenderer>();
                if (smr != null) result.Add(smr);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[CharacterCreatorController] Skipping part '{row.PartName}': {e.Message}");
            }
        }
    }

    // ── Color refresh ─────────────────────────────────────────────────────────

    private void RefreshColors()
    {
        if (_previewChar == null) return;
        foreach (var smr in _previewChar.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            var block = new MaterialPropertyBlock();
            smr.GetPropertyBlock(block);
            block.SetColor(ID_Skin,      _skin);
            block.SetColor(ID_Hair,      _hair);
            block.SetColor(ID_Primary,   _primary);
            block.SetColor(ID_Secondary, _secondary);
            smr.SetPropertyBlock(block);
        }
    }

    // ── Label refresh ─────────────────────────────────────────────────────────

    private void RefreshPresetLabels()
    {
        if (headPresetLabel != null && _headPresets.Count > 0)
            headPresetLabel.text = $"{_headPresets[_headIdx].Name} ({_headIdx + 1}/{_headPresets.Count})";

        if (upperBodyPresetLabel != null && _upperPresets.Count > 0)
            upperBodyPresetLabel.text = $"{_upperPresets[_upperIdx].Name} ({_upperIdx + 1}/{_upperPresets.Count})";

        if (lowerBodyPresetLabel != null && _lowerPresets.Count > 0)
            lowerBodyPresetLabel.text = $"{_lowerPresets[_lowerIdx].Name} ({_lowerIdx + 1}/{_lowerPresets.Count})";
    }

    // ── Color swatches ────────────────────────────────────────────────────────

    private void SetupSwatches(Button[] buttons, Color[] palette, System.Action<Color> setter)
    {
        if (buttons == null) return;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;
            Color c = i < palette.Length ? palette[i] : Color.white;
            var img = buttons[i].GetComponent<Image>();
            if (img != null) img.color = c;
            buttons[i].onClick.RemoveAllListeners();
            Color captured = c;
            buttons[i].onClick.AddListener(() => setter(captured));
        }
    }

    // ── Save and start ────────────────────────────────────────────────────────

    public void OnBeginAdventure()
    {
        var d = ScriptableObject.CreateInstance<PlayerCharacterData>();

        d.headPresetIndex      = _headIdx;
        d.upperBodyPresetIndex = _upperIdx;
        d.lowerBodyPresetIndex = _lowerIdx;

        d.skinColor      = _skin;
        d.hairColor      = _hair;
        d.primaryColor   = _primary;
        d.secondaryColor = _secondary;

        string n = nameInput != null ? nameInput.text.Trim() : "";
        if (string.IsNullOrEmpty(n)) n = "Colonist";
        if (n.Length > 12) n = n.Substring(0, 12);
        d.characterName = n;

        d.Save();
        SceneManager.LoadScene(nextScene);
    }

    // ── Auto-wiring (find by child name, no inspector required) ───────────────

    /// <summary>
    /// Finds buttons and labels by child GameObject name and wires them up.
    /// Runs before InitSyntyRuntime so labels are ready when presets load.
    /// </summary>
    private void AutoWire()
    {
        BindButton("HeadPrevBtn",           OnHeadPrev);
        BindButton("HeadNextBtn",           OnHeadNext);
        BindButton("UpperPrevBtn",          OnUpperPrev);
        BindButton("UpperNextBtn",          OnUpperNext);
        BindButton("LowerPrevBtn",          OnLowerPrev);
        BindButton("LowerNextBtn",          OnLowerNext);
        BindButton("BeginAdventureBtn",     OnBeginAdventure);

        if (headPresetLabel == null)       headPresetLabel       = FindDeepChild<Text>("HeadLabel");
        if (upperBodyPresetLabel == null)  upperBodyPresetLabel  = FindDeepChild<Text>("UpperLabel");
        if (lowerBodyPresetLabel == null)  lowerBodyPresetLabel  = FindDeepChild<Text>("LowerLabel");
        if (nameInput == null)             nameInput             = FindDeepChild<InputField>("NameInput");

        if (previewRoot == null)
        {
            var pr = GameObject.Find("PreviewRoot");
            if (pr != null) previewRoot = pr.transform;
        }
    }

    private void BindButton(string goName, UnityEngine.Events.UnityAction action)
    {
        var btn = FindDeepChild<Button>(goName);
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(action);
        }
    }

    private Button[] FindSwatchButtons(string parentName)
    {
        // Find the container GO by name scene-wide
        var parentGO = GameObject.Find(parentName);
        if (parentGO == null) return new Button[0];
        var btns = new System.Collections.Generic.List<Button>();
        foreach (Transform child in parentGO.transform)
        {
            var b = child.GetComponent<Button>();
            if (b != null) btns.Add(b);
        }
        return btns.ToArray();
    }

    private T FindDeepChild<T>(string childName) where T : Component
    {
        // Search own children first
        foreach (var t in GetComponentsInChildren<Transform>(true))
        {
            if (t.name == childName)
            {
                var c = t.GetComponent<T>();
                if (c != null) return c;
            }
        }
        // Fallback: search the whole scene (Controller may not be in the Canvas hierarchy)
        foreach (var c in FindObjectsOfType<T>(true))
        {
            if (c.gameObject.name == childName) return c;
        }
        return null;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static int Mod(int a, int b) => b == 0 ? 0 : ((a % b) + b) % b;
}
