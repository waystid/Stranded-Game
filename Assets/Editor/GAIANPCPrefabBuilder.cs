using UnityEngine;
using UnityEditor;
using MoreMountains.TopDownEngine;
using System.IO;

/// <summary>
/// Editor script to programmatically build the G.A.I.A. NPC prefab for Galactic Crossing
/// G.A.I.A. (Global Artificial Intelligence Assistant) is the Tom Nook equivalent
/// </summary>
public class GAIANPCPrefabBuilder : MonoBehaviour
{
    private const string PREFAB_PATH = "Assets/Prefabs/";

    [MenuItem("Tools/Galactic Crossing/Build GAIA NPC Prefab")]
    public static void BuildGAIANPCPrefab()
    {
        Debug.Log("=== Starting G.A.I.A. NPC Prefab Build Process ===");

        // Ensure directories exist
        EnsureDirectoryExists(PREFAB_PATH);

        // Create root GameObject
        GameObject npcPrefab = new GameObject("GAIA_NPC");

        // 1. Create NPCModel (placeholder capsule)
        GameObject npcModel = CreateNPCModel(npcPrefab.transform);

        // 2. Add CapsuleCollider (non-trigger, for physical presence)
        CapsuleCollider bodyCollider = npcPrefab.AddComponent<CapsuleCollider>();
        bodyCollider.isTrigger = false;
        bodyCollider.radius = 0.5f;
        bodyCollider.height = 2.0f;
        bodyCollider.center = new Vector3(0f, 1.0f, 0f);
        bodyCollider.direction = 1; // Y-axis
        Debug.Log("Added CapsuleCollider to GAIA_NPC (non-trigger)");

        // 3. Create InteractionZone child object
        GameObject interactionZone = new GameObject("InteractionZone");
        interactionZone.transform.SetParent(npcPrefab.transform);
        interactionZone.transform.localPosition = Vector3.zero;

        // Add BoxCollider (trigger) to InteractionZone
        BoxCollider interactionCollider = interactionZone.AddComponent<BoxCollider>();
        interactionCollider.isTrigger = true;
        interactionCollider.size = new Vector3(4f, 4f, 4f); // 2m interaction radius
        interactionCollider.center = new Vector3(0f, 1.0f, 0f);
        Debug.Log("Added BoxCollider (trigger) to InteractionZone");

        // Add DialogueZone component to InteractionZone
        DialogueZone dialogueZone = interactionZone.AddComponent<DialogueZone>();

        // 4. Create FloatingIndicator (UI Canvas)
        GameObject floatingIndicator = CreateFloatingIndicator(npcPrefab.transform);

        // Configure DialogueZone component using SerializedObject
        SerializedObject serializedDialogue = new SerializedObject(dialogueZone);

        // Button Handled settings
        serializedDialogue.FindProperty("ButtonHandled").boolValue = true;
        serializedDialogue.FindProperty("CanMoveWhileTalking").boolValue = false;
        serializedDialogue.FindProperty("ActivableMoreThanOnce").boolValue = true;

        // Timing settings
        serializedDialogue.FindProperty("FadeDuration").floatValue = 0.2f;
        serializedDialogue.FindProperty("TransitionTime").floatValue = 0.2f;
        serializedDialogue.FindProperty("InactiveTime").floatValue = 2f;

        // Position settings
        SerializedProperty offsetProp = serializedDialogue.FindProperty("Offset");
        if (offsetProp != null)
        {
            offsetProp.vector3Value = new Vector3(0f, 3f, 0f); // Above NPC head
        }

        serializedDialogue.FindProperty("BoxesFollowZone").boolValue = true;

        // Text appearance
        SerializedProperty textColorProp = serializedDialogue.FindProperty("TextColor");
        if (textColorProp != null)
        {
            textColorProp.colorValue = Color.white;
        }

        SerializedProperty bgColorProp = serializedDialogue.FindProperty("TextBackgroundColor");
        if (bgColorProp != null)
        {
            bgColorProp.colorValue = new Color(0.1f, 0.1f, 0.15f, 0.95f); // Dark blue-black
        }

        serializedDialogue.FindProperty("TextSize").intValue = 40;

        SerializedProperty alignmentProp = serializedDialogue.FindProperty("Alignment");
        if (alignmentProp != null)
        {
            alignmentProp.enumValueIndex = 4; // TextAnchor.MiddleCenter = 4
        }

        // Prompt text
        SerializedProperty promptTextProp = serializedDialogue.FindProperty("PromptText");
        if (promptTextProp != null)
        {
            promptTextProp.stringValue = "Talk to G.A.I.A.";
        }

        // Configure dialogue lines (20+ lines from the plan document)
        SerializedProperty dialogueProp = serializedDialogue.FindProperty("Dialogue");
        if (dialogueProp != null)
        {
            // Clear existing array
            dialogueProp.ClearArray();

            // Add 24 dialogue lines based on the Day 0 crash landing sequence
            string[] dialogueLines = new string[]
            {
                "Welcome to the colonization initiative... er, emergency landing protocol. Please verify your bio-metrics.",
                "Systems check complete. Good news: You're alive! Bad news: We're very, very far from home.",
                "Analyzing planetary composition... breathable atmosphere detected. That's fortunate!",
                "I am G.A.I.A. - Global Artificial Intelligence Assistant. I'll be your guide through this... unexpected detour.",
                "First priority: Deploy the Pop-Up Hab Module. You'll find the Hab Kit in your inventory.",
                "Simply select 'Deploy' and choose a suitable location. Somewhere flat would be ideal.",
                "Excellent work! Next, we need to establish charging stations for Bit and Bot.",
                "Those are the maintenance drones. They'll be essential for rebuilding our mission infrastructure.",
                "Now, let's gather some basic resources. I'm detecting carbon debris scattered around the crash site.",
                "Collect 10 pieces of debris. We'll need them to calibrate the atmospheric scrubbers.",
                "I'm also detecting energy signatures from the local flora. Fascinating!",
                "Gather 6 of those crystalline formations. They might jumpstart our power systems.",
                "Wonderful progress! You're a natural survivor. Tom would be proud. Sorry, that's an old reference file.",
                "Let's initialize the system reboot ceremony. Think of it as... a fresh start party!",
                "This planet deserves a name. What shall we call our new home?",
                "A beautiful choice! I'm updating the star charts now. This place is officially on the map!",
                "You've earned some rest. The Stasis Bed is now active in your Hab Module.",
                "Interacting with it will save your progress and advance to the next day cycle.",
                "Tomorrow, I'll upload the Colony OS to your DataPad. It contains fabrication blueprints and bio-data tracking.",
                "I'm also receiving a transmission from Dr. Hoot, our Xeno-Biologist. Intriguing!",
                "She's requesting biological samples to assess planetary safety. Could you scan some local lifeforms?",
                "Bring me 5 unique specimens - the Float-Fish and Holo-Beetles should be abundant here.",
                "Once analyzed, we can establish a Bio-Lab for proper research. This is quite exciting!",
                "Remember, you can always return to chat. I'm here to help make this crash landing feel like home."
            };

            dialogueProp.arraySize = dialogueLines.Length;

            for (int i = 0; i < dialogueLines.Length; i++)
            {
                SerializedProperty element = dialogueProp.GetArrayElementAtIndex(i);
                SerializedProperty dialogueLineProp = element.FindPropertyRelative("DialogueLine");
                if (dialogueLineProp != null)
                {
                    dialogueLineProp.stringValue = dialogueLines[i];
                }
            }

            Debug.Log($"Added {dialogueLines.Length} dialogue lines to G.A.I.A.");
        }

        // ButtonActivated base class properties
        serializedDialogue.FindProperty("ButtonActivated").boolValue = true;
        serializedDialogue.FindProperty("AutoActivation").boolValue = false;

        serializedDialogue.ApplyModifiedProperties();

        Debug.Log("Configured DialogueZone component on InteractionZone");

        // Save as prefab
        string prefabPath = PREFAB_PATH + "GAIA_NPC.prefab";
        PrefabUtility.SaveAsPrefabAsset(npcPrefab, prefabPath);

        // Clean up scene object
        DestroyImmediate(npcPrefab);

        // Refresh asset database
        AssetDatabase.Refresh();

        Debug.Log("GAIA_NPC prefab created at: " + prefabPath);
        Debug.Log("=== G.A.I.A. NPC Prefab Build Complete ===");
    }

    /// <summary>
    /// Creates the visual NPC model (holographic AI pod)
    /// </summary>
    private static GameObject CreateNPCModel(Transform parent)
    {
        GameObject npcModel = new GameObject("NPCModel");
        npcModel.transform.SetParent(parent);
        npcModel.transform.localPosition = Vector3.zero;

        // Create base capsule (AI pod housing)
        GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        capsule.name = "AI_Pod";
        capsule.transform.SetParent(npcModel.transform);
        capsule.transform.localPosition = new Vector3(0f, 1.0f, 0f);
        capsule.transform.localScale = new Vector3(0.8f, 1.0f, 0.8f);

        // Remove collider (parent has the main collider)
        DestroyImmediate(capsule.GetComponent<Collider>());

        // Create futuristic metallic material with cyan glow
        Material podMaterial = new Material(Shader.Find("Standard"));
        podMaterial.color = new Color(0.7f, 0.8f, 0.9f); // Light cyan-gray
        podMaterial.SetFloat("_Metallic", 0.9f);
        podMaterial.SetFloat("_Glossiness", 0.8f);
        podMaterial.EnableKeyword("_EMISSION");
        podMaterial.SetColor("_EmissionColor", new Color(0.3f, 0.6f, 1.0f) * 0.5f);
        capsule.GetComponent<Renderer>().material = podMaterial;

        // Create hologram core (glowing sphere)
        GameObject core = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        core.name = "Hologram_Core";
        core.transform.SetParent(npcModel.transform);
        core.transform.localPosition = new Vector3(0f, 1.5f, 0f);
        core.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        // Remove collider
        DestroyImmediate(core.GetComponent<Collider>());

        // Create bright glowing material
        Material coreMaterial = new Material(Shader.Find("Standard"));
        coreMaterial.color = new Color(0.4f, 0.8f, 1.0f);
        coreMaterial.SetFloat("_Metallic", 0f);
        coreMaterial.SetFloat("_Glossiness", 1.0f);
        coreMaterial.EnableKeyword("_EMISSION");
        coreMaterial.SetColor("_EmissionColor", new Color(0.5f, 1.0f, 1.5f) * 1.5f);
        core.GetComponent<Renderer>().material = coreMaterial;

        Debug.Log("Created NPCModel with AI Pod and Hologram Core");

        return npcModel;
    }

    /// <summary>
    /// Creates a floating indicator (UI Canvas above NPC)
    /// </summary>
    private static GameObject CreateFloatingIndicator(Transform parent)
    {
        GameObject indicator = new GameObject("FloatingIndicator");
        indicator.transform.SetParent(parent);
        indicator.transform.localPosition = new Vector3(0f, 2.5f, 0f);

        // Add Canvas component
        Canvas canvas = indicator.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;

        // Configure canvas transform
        RectTransform rectTransform = indicator.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200f, 100f);
        rectTransform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        // Add CanvasScaler (optional but recommended)
        UnityEngine.UI.CanvasScaler scaler = indicator.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10f;

        Debug.Log("Created FloatingIndicator Canvas");

        // Note: Actual UI elements (exclamation mark sprite, etc.) can be added manually
        // or in a future iteration of this builder

        return indicator;
    }

    /// <summary>
    /// Ensures the specified directory exists, creating it if necessary
    /// </summary>
    private static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Debug.Log("Created directory: " + path);
        }
    }
}
