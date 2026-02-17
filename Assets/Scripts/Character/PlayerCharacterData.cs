using UnityEngine;

/// <summary>
/// ScriptableObject that stores a player's character customization choices.
/// Saved to PlayerPrefs at game-start (Character Creator scene).
/// Loaded by CharacterCustomizer at runtime.
///
/// PlayerPrefs key names mirror the constants below.
/// </summary>
[CreateAssetMenu(fileName = "PlayerCharacterData", menuName = "CosmicColony/PlayerCharacterData")]
public class PlayerCharacterData : ScriptableObject
{
    // ── Identity ──────────────────────────────────────────────────────────────

    [Tooltip("Player display name (max 12 chars).")]
    public string characterName = "Colonist";

    [Tooltip("Species index: 0-3 = HumanSpecies01-04, 4-7 = Starter01-04.")]
    [Range(0, 7)]
    public int speciesIndex = 0;

    // ── Colors ────────────────────────────────────────────────────────────────

    public Color skinColor     = new Color(0.85f, 0.65f, 0.50f, 1f);
    public Color hairColor     = new Color(0.20f, 0.12f, 0.06f, 1f);
    public Color primaryColor  = new Color(0.20f, 0.40f, 0.75f, 1f);
    public Color secondaryColor = new Color(0.90f, 0.90f, 0.90f, 1f);

    // ── PlayerPrefs Keys ─────────────────────────────────────────────────────

    public const string KEY_SPECIES   = "Character_Species";
    public const string KEY_NAME      = "Character_Name";
    public const string KEY_SKIN_R    = "Character_Color_Skin_R";
    public const string KEY_SKIN_G    = "Character_Color_Skin_G";
    public const string KEY_SKIN_B    = "Character_Color_Skin_B";
    public const string KEY_HAIR_R    = "Character_Color_Hair_R";
    public const string KEY_HAIR_G    = "Character_Color_Hair_G";
    public const string KEY_HAIR_B    = "Character_Color_Hair_B";
    public const string KEY_PRIMARY_R = "Character_Color_Primary_R";
    public const string KEY_PRIMARY_G = "Character_Color_Primary_G";
    public const string KEY_PRIMARY_B = "Character_Color_Primary_B";

    // ── Persistence ───────────────────────────────────────────────────────────

    /// <summary>Write this data to PlayerPrefs.</summary>
    public void Save()
    {
        PlayerPrefs.SetInt(KEY_SPECIES, speciesIndex);
        PlayerPrefs.SetString(KEY_NAME, characterName);
        PlayerPrefs.SetFloat(KEY_SKIN_R, skinColor.r);
        PlayerPrefs.SetFloat(KEY_SKIN_G, skinColor.g);
        PlayerPrefs.SetFloat(KEY_SKIN_B, skinColor.b);
        PlayerPrefs.SetFloat(KEY_HAIR_R, hairColor.r);
        PlayerPrefs.SetFloat(KEY_HAIR_G, hairColor.g);
        PlayerPrefs.SetFloat(KEY_HAIR_B, hairColor.b);
        PlayerPrefs.SetFloat(KEY_PRIMARY_R, primaryColor.r);
        PlayerPrefs.SetFloat(KEY_PRIMARY_G, primaryColor.g);
        PlayerPrefs.SetFloat(KEY_PRIMARY_B, primaryColor.b);
        PlayerPrefs.Save();
    }

    /// <summary>Load from PlayerPrefs into this data object.</summary>
    public void Load()
    {
        speciesIndex   = PlayerPrefs.GetInt(KEY_SPECIES, 0);
        characterName  = PlayerPrefs.GetString(KEY_NAME, "Colonist");
        skinColor = new Color(
            PlayerPrefs.GetFloat(KEY_SKIN_R, 0.85f),
            PlayerPrefs.GetFloat(KEY_SKIN_G, 0.65f),
            PlayerPrefs.GetFloat(KEY_SKIN_B, 0.50f));
        hairColor = new Color(
            PlayerPrefs.GetFloat(KEY_HAIR_R, 0.20f),
            PlayerPrefs.GetFloat(KEY_HAIR_G, 0.12f),
            PlayerPrefs.GetFloat(KEY_HAIR_B, 0.06f));
        primaryColor = new Color(
            PlayerPrefs.GetFloat(KEY_PRIMARY_R, 0.20f),
            PlayerPrefs.GetFloat(KEY_PRIMARY_G, 0.40f),
            PlayerPrefs.GetFloat(KEY_PRIMARY_B, 0.75f));
    }

    /// <summary>True if the player has ever saved character data.</summary>
    public static bool HasSavedData() => PlayerPrefs.HasKey(KEY_SPECIES);
}
