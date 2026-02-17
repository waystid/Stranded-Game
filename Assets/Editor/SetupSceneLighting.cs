using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class SetupSceneLighting
{
    [MenuItem("Tools/Galactic Crossing/Setup Scene Lighting")]
    public static void Setup()
    {
        // Gradient ambient: sky=soft blue-green, equator=mid-grey, ground=dark green
        RenderSettings.ambientMode = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor      = new Color(0.35f, 0.55f, 0.65f);  // soft sky blue
        RenderSettings.ambientEquatorColor  = new Color(0.30f, 0.45f, 0.30f);  // mid green-grey
        RenderSettings.ambientGroundColor   = new Color(0.10f, 0.18f, 0.10f);  // dark green

        // Also lower ambient intensity slightly
        RenderSettings.ambientIntensity = 0.6f;

        EditorUtility.SetDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()[0]);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log("[SetupSceneLighting] Ambient lighting updated.");
    }
}
