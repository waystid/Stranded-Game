using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Phase C — Reads the saved character name from PlayerPrefs and displays it
/// in a UI Text element. Attach anywhere in the SandboxShowcase scene.
/// </summary>
public class PlayerNameDisplay : MonoBehaviour
{
    [Tooltip("Text element to write the character name into.")]
    public Text nameText;

    [Tooltip("Prefix shown before the name (e.g. 'Colonist: ').")]
    public string prefix = "";

    void Start()
    {
        if (nameText == null)
            nameText = GetComponent<Text>();

        if (nameText == null) return;

        string savedName = PlayerPrefs.GetString(PlayerCharacterData.KEY_NAME, "Colonist");
        nameText.text = prefix + savedName;
    }
}
