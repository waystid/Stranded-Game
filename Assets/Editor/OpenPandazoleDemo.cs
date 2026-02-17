using UnityEditor;
using UnityEditor.SceneManagement;

public class OpenPandazoleDemo
{
    [MenuItem("Tools/Galactic Crossing/Open Pandazole Nature Demo")]
    public static void Open()
    {
        EditorSceneManager.OpenScene(
            "Assets/Pandazole_Ultimate_Pack/Pandazole Nature Environment Pack/Scenes/Demo.unity",
            OpenSceneMode.Single);
    }

    [MenuItem("Tools/Galactic Crossing/Open SandboxShowcase")]
    public static void OpenSandbox()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/SandboxShowcase.unity", OpenSceneMode.Single);
    }

    [MenuItem("Tools/Galactic Crossing/Import TWC Samples URP")]
public static void ImportTWCSamples() { AssetDatabase.ImportPackage("Assets/TileWorldCreator/Samples_URP.unitypackage", false); }

    [MenuItem("Tools/Galactic Crossing/Open TWC CliffIsland")]
    public static void OpenCliffIsland() { EditorSceneManager.OpenScene("Assets/TileWorldCreator/_Samples/CliffIsland URP/CliffIsland.unity", OpenSceneMode.Single); }

    [MenuItem("Tools/Galactic Crossing/Open TWC StylizedIsland")]
    public static void OpenStylizedIsland() { EditorSceneManager.OpenScene("Assets/TileWorldCreator/_Samples/StylizedIsland URP/ManualBuild.unity", OpenSceneMode.Single); }
}
