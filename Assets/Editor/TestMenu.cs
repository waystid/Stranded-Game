
using UnityEngine;
using UnityEditor;

public class TestMenu
{
    [MenuItem("Tools/Test Menu")]
    public static void Test()
    {
        Debug.Log("Test Menu Works!");
    }

    [MenuItem("Tools/CosmicColony/Set Test Character Prefs")]
    public static void SetTestCharacterPrefs()
    {
        PlayerPrefs.SetInt(PlayerCharacterData.KEY_SPECIES, 0);
        PlayerPrefs.SetString(PlayerCharacterData.KEY_NAME, "TestColonist");
        PlayerPrefs.SetInt(PlayerCharacterData.KEY_HEAD_PRESET, 0);
        PlayerPrefs.SetInt(PlayerCharacterData.KEY_UPPER_PRESET, 0);
        PlayerPrefs.SetInt(PlayerCharacterData.KEY_LOWER_PRESET, 0);
        PlayerPrefs.SetFloat(PlayerCharacterData.KEY_SKIN_R, 0.85f);
        PlayerPrefs.SetFloat(PlayerCharacterData.KEY_SKIN_G, 0.65f);
        PlayerPrefs.SetFloat(PlayerCharacterData.KEY_SKIN_B, 0.50f);
        PlayerPrefs.SetFloat(PlayerCharacterData.KEY_HAIR_R, 0.20f);
        PlayerPrefs.SetFloat(PlayerCharacterData.KEY_HAIR_G, 0.12f);
        PlayerPrefs.SetFloat(PlayerCharacterData.KEY_HAIR_B, 0.06f);
        PlayerPrefs.SetFloat(PlayerCharacterData.KEY_PRIMARY_R, 0.20f);
        PlayerPrefs.SetFloat(PlayerCharacterData.KEY_PRIMARY_G, 0.40f);
        PlayerPrefs.SetFloat(PlayerCharacterData.KEY_PRIMARY_B, 0.75f);
        PlayerPrefs.SetFloat(PlayerCharacterData.KEY_SECONDARY_R, 0.90f);
        PlayerPrefs.SetFloat(PlayerCharacterData.KEY_SECONDARY_G, 0.90f);
        PlayerPrefs.SetFloat(PlayerCharacterData.KEY_SECONDARY_B, 0.90f);
        PlayerPrefs.Save();
        Debug.Log("[TestMenu] Character PlayerPrefs set — preset indices 0/0/0, default colors.");
    }

    [MenuItem("Tools/CosmicColony/Clear Character Prefs")]
    public static void ClearCharacterPrefs()
    {
        PlayerPrefs.DeleteKey(PlayerCharacterData.KEY_SPECIES);
        PlayerPrefs.DeleteKey(PlayerCharacterData.KEY_NAME);
        PlayerPrefs.DeleteKey(PlayerCharacterData.KEY_HEAD_PRESET);
        PlayerPrefs.DeleteKey(PlayerCharacterData.KEY_UPPER_PRESET);
        PlayerPrefs.DeleteKey(PlayerCharacterData.KEY_LOWER_PRESET);
        PlayerPrefs.Save();
        Debug.Log("[TestMenu] Character PlayerPrefs cleared.");
    }
}
