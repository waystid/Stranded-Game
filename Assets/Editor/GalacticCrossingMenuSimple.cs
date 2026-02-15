using UnityEngine;
using UnityEditor;

/// <summary>
/// Simplified menu without namespace for better Unity compatibility
/// </summary>
public class GalacticCrossingMenuSimple
{
    [MenuItem("Tools/Galactic Crossing (Simple)/Test Connection")]
    public static void TestConnection()
    {
        Debug.Log("✅ Galactic Crossing Editor Scripts are working!");
        EditorUtility.DisplayDialog(
            "Success!",
            "Galactic Crossing Editor Scripts are loaded correctly!\n\nYou should see the full menu at:\nTools > Galactic Crossing",
            "OK");
    }

    [MenuItem("Tools/Galactic Crossing (Simple)/Open Loft3D Scene")]
    public static void OpenLoft3D()
    {
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene(
            "Assets/TopDownEngine/Demos/Loft3D/Loft3D.unity");
        Debug.Log("Opened Loft3D scene");
    }

    [MenuItem("Tools/Galactic Crossing (Simple)/Show Scripts Location")]
    public static void ShowScriptsLocation()
    {
        EditorUtility.RevealInFinder(Application.dataPath + "/Editor");
        Debug.Log("Editor scripts location: " + Application.dataPath + "/Editor");
    }
}
