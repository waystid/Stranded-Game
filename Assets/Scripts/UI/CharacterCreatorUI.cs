using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Phase C — Character Creator scene controller.
/// Handles species cycling, color swatch selection, name entry, and game-start.
/// Attach to a root GameObject in the CharacterCreator scene.
/// </summary>
public class CharacterCreatorUI : MonoBehaviour
{
    // ── Preview ───────────────────────────────────────────────────────────────

    [Header("Preview")]
    [Tooltip("Empty GO in world space. Preview character is spawned here and rotated.")]
    public Transform previewRoot;
    public float rotationSpeed = 40f;

    // ── Species ───────────────────────────────────────────────────────────────

    [Header("Species (0-3 = HumanSpecies01-04, 4-7 = Starter01-04)")]
    public GameObject[] speciesPrefabs = new GameObject[8];
    public Text speciesLabel;

    private static readonly string[] SpeciesNames =
    {
        "Human I", "Human II", "Human III", "Human IV",
        "Starter I", "Starter II", "Starter III", "Starter IV"
    };

    // ── Name ──────────────────────────────────────────────────────────────────

    [Header("Name")]
    public InputField nameInput;

    // ── Color swatches ────────────────────────────────────────────────────────

    [Header("Color Swatch Buttons (8 per row)")]
    public Button[] skinButtons;
    public Button[] hairButtons;
    public Button[] primaryButtons;
    public Button[] secondaryButtons;

    // Preset palettes (8 colours each)
    private static readonly Color[] SkinPalette =
    {
        new Color(0.95f, 0.82f, 0.72f),   // light
        new Color(0.85f, 0.65f, 0.50f),   // medium
        new Color(0.72f, 0.50f, 0.35f),   // tan
        new Color(0.55f, 0.35f, 0.22f),   // brown
        new Color(0.35f, 0.22f, 0.12f),   // dark
        new Color(0.65f, 0.82f, 0.95f),   // pale blue
        new Color(0.58f, 0.88f, 0.62f),   // pale green
        new Color(0.82f, 0.65f, 0.95f),   // pale purple
    };

    private static readonly Color[] HairPalette =
    {
        new Color(0.95f, 0.95f, 0.92f),   // white
        new Color(0.88f, 0.78f, 0.45f),   // blonde
        new Color(0.72f, 0.45f, 0.22f),   // auburn
        new Color(0.20f, 0.12f, 0.06f),   // dark brown
        new Color(0.08f, 0.08f, 0.08f),   // black
        new Color(0.80f, 0.20f, 0.20f),   // red
        new Color(0.20f, 0.50f, 0.90f),   // blue
        new Color(0.60f, 0.22f, 0.80f),   // purple
    };

    private static readonly Color[] OutfitPalette =
    {
        new Color(0.20f, 0.40f, 0.78f),   // blue
        new Color(0.78f, 0.20f, 0.20f),   // red
        new Color(0.22f, 0.65f, 0.30f),   // green
        new Color(0.80f, 0.62f, 0.15f),   // gold
        new Color(0.50f, 0.20f, 0.72f),   // purple
        new Color(0.18f, 0.72f, 0.75f),   // teal
        new Color(0.92f, 0.92f, 0.92f),   // white
        new Color(0.12f, 0.12f, 0.12f),   // black
    };

    // ── Scene ─────────────────────────────────────────────────────────────────

    [Header("Scene")]
    public string nextScene = "SandboxShowcase";

    // ── Runtime state ─────────────────────────────────────────────────────────

    int _speciesIdx;
    Color _skin, _hair, _primary, _secondary;

    GameObject _previewGO;
    CharacterCustomizer _customizer;

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    void Start()
    {
        if (PlayerCharacterData.HasSavedData())
        {
            var d = ScriptableObject.CreateInstance<PlayerCharacterData>();
            d.Load();
            _speciesIdx = d.speciesIndex;
            _skin       = d.skinColor;
            _hair       = d.hairColor;
            _primary    = d.primaryColor;
            _secondary  = d.secondaryColor;
            if (nameInput != null) nameInput.text = d.characterName;
        }
        else
        {
            _speciesIdx = 0;
            _skin       = SkinPalette[1];
            _hair       = HairPalette[3];
            _primary    = OutfitPalette[0];
            _secondary  = OutfitPalette[6];
            if (nameInput != null) nameInput.text = "Colonist";
        }

        SetupSwatches(skinButtons,      SkinPalette,   c => _skin      = c);
        SetupSwatches(hairButtons,      HairPalette,   c => _hair      = c);
        SetupSwatches(primaryButtons,   OutfitPalette, c => _primary   = c);
        SetupSwatches(secondaryButtons, OutfitPalette, c => _secondary = c);

        SpawnPreview();
        RefreshSpeciesLabel();
    }

    void Update()
    {
        if (previewRoot != null)
            previewRoot.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    // ── Species ───────────────────────────────────────────────────────────────

    public void OnSpeciesPrev()
    {
        _speciesIdx = (_speciesIdx + 7) % 8;
        SpawnPreview();
        RefreshSpeciesLabel();
    }

    public void OnSpeciesNext()
    {
        _speciesIdx = (_speciesIdx + 1) % 8;
        SpawnPreview();
        RefreshSpeciesLabel();
    }

    void RefreshSpeciesLabel()
    {
        if (speciesLabel != null)
            speciesLabel.text = SpeciesNames[Mathf.Clamp(_speciesIdx, 0, SpeciesNames.Length - 1)];
    }

    // ── Preview ───────────────────────────────────────────────────────────────

    void SpawnPreview()
    {
        if (_previewGO != null)
            Destroy(_previewGO);

        if (speciesPrefabs == null || _speciesIdx >= speciesPrefabs.Length) return;

        var prefab = speciesPrefabs[_speciesIdx];
        if (prefab == null) return;

        var parent = previewRoot != null ? previewRoot : transform;
        _previewGO = Instantiate(prefab, parent);
        _previewGO.transform.localPosition = Vector3.zero;
        _previewGO.transform.localRotation = Quaternion.identity;

        // Disable physics/AI components that shouldn't run in the creator scene
        DisableRuntimeComponents(_previewGO);

        _customizer = _previewGO.GetComponentInChildren<CharacterCustomizer>();
        if (_customizer == null)
            _customizer = _previewGO.AddComponent<CharacterCustomizer>();

        RefreshPreview();
    }

    void DisableRuntimeComponents(GameObject go)
    {
        // Disable Rigidbody physics so the preview doesn't fall
        var rb = go.GetComponentInChildren<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // Disable CharacterController so it doesn't fight position
        var cc = go.GetComponentInChildren<UnityEngine.CharacterController>();
        if (cc != null) cc.enabled = false;
    }

    void RefreshPreview()
    {
        if (_customizer != null)
            _customizer.ApplyColors(_skin, _hair, _primary, _secondary);
    }

    // ── Swatches ──────────────────────────────────────────────────────────────

    void SetupSwatches(Button[] buttons, Color[] palette, System.Action<Color> setter)
    {
        if (buttons == null) return;
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;
            Color c = i < palette.Length ? palette[i] : Color.white;

            // Tint the swatch image
            var img = buttons[i].GetComponent<Image>();
            if (img != null) img.color = c;

            // Wire click (capture loop variable)
            buttons[i].onClick.RemoveAllListeners();
            Color captured = c;
            buttons[i].onClick.AddListener(() =>
            {
                setter(captured);
                RefreshPreview();
            });
        }
    }

    // ── Confirm ───────────────────────────────────────────────────────────────

    public void OnBeginAdventure()
    {
        var d = ScriptableObject.CreateInstance<PlayerCharacterData>();
        d.speciesIndex   = _speciesIdx;
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
}
