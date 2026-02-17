using UnityEngine;

/// <summary>
/// Applies Synty SidekickCharacter color customization to the player mesh at runtime.
/// Reads from PlayerCharacterData (PlayerPrefs) and pushes colors via MaterialPropertyBlock
/// to every SkinnedMeshRenderer on the character — zero material instances created.
///
/// Attach to the SidekickPlayer root. Call ApplyColors() after loading character data.
///
/// Shader property names (Synty color mask shader):
///   _Color_Skin       — skin tone channel
///   _Color_Hair       — hair color channel
///   _Color_Primary    — outfit primary color
///   _Color_Secondary  — outfit secondary color
/// </summary>
public class CharacterCustomizer : MonoBehaviour
{
    // ── Inspector ─────────────────────────────────────────────────────────────

    [Tooltip("If set, colors are applied from this ScriptableObject on Awake. " +
             "Otherwise colors are loaded from PlayerPrefs.")]
    public PlayerCharacterData characterData;

    [Header("Default Colors (used if no saved data)")]
    public Color defaultSkin      = new Color(0.85f, 0.65f, 0.50f, 1f);
    public Color defaultHair      = new Color(0.20f, 0.12f, 0.06f, 1f);
    public Color defaultPrimary   = new Color(0.20f, 0.40f, 0.75f, 1f);
    public Color defaultSecondary = new Color(0.90f, 0.90f, 0.90f, 1f);

    // ── Shader property IDs ───────────────────────────────────────────────────

    private static readonly int ID_Skin      = Shader.PropertyToID("_Color_Skin");
    private static readonly int ID_Hair      = Shader.PropertyToID("_Color_Hair");
    private static readonly int ID_Primary   = Shader.PropertyToID("_Color_Primary");
    private static readonly int ID_Secondary = Shader.PropertyToID("_Color_Secondary");

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    void Start()
    {
        if (characterData != null)
        {
            if (PlayerCharacterData.HasSavedData())
                characterData.Load();

            ApplyColors(characterData.skinColor, characterData.hairColor,
                        characterData.primaryColor, characterData.secondaryColor);
        }
        else if (PlayerCharacterData.HasSavedData())
        {
            var data = ScriptableObject.CreateInstance<PlayerCharacterData>();
            data.Load();
            ApplyColors(data.skinColor, data.hairColor, data.primaryColor, data.secondaryColor);
        }
        else
        {
            ApplyColors(defaultSkin, defaultHair, defaultPrimary, defaultSecondary);
        }
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Apply colors from a PlayerCharacterData object. Safe to call at runtime.
    /// </summary>
    public void ApplyColors(PlayerCharacterData data)
    {
        ApplyColors(data.skinColor, data.hairColor, data.primaryColor, data.secondaryColor);
    }

    /// <summary>
    /// Apply individual color values to all SkinnedMeshRenderers on the character.
    /// Uses MaterialPropertyBlock — no extra material instances.
    /// </summary>
    public void ApplyColors(Color skin, Color hair, Color primary, Color secondary)
    {
        foreach (var r in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            var block = new MaterialPropertyBlock();
            r.GetPropertyBlock(block);
            block.SetColor(ID_Skin,      skin);
            block.SetColor(ID_Hair,      hair);
            block.SetColor(ID_Primary,   primary);
            block.SetColor(ID_Secondary, secondary);
            r.SetPropertyBlock(block);
        }
    }

    /// <summary>
    /// Reset all colors to the default inspector values.
    /// </summary>
    public void ResetColors()
    {
        ApplyColors(defaultSkin, defaultHair, defaultPrimary, defaultSecondary);
    }
}
