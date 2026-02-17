using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Creates and assigns a URP Render Pipeline Asset for the project.
/// Run via Tools > Galactic Crossing > Setup URP Pipeline
/// </summary>
public class SetupURPPipeline
{
    [MenuItem("Tools/Galactic Crossing/Setup URP Pipeline")]
    public static void Setup()
    {
        // Create Settings folder
        if (!AssetDatabase.IsValidFolder("Assets/Settings"))
            AssetDatabase.CreateFolder("Assets", "Settings");

        // Create the Renderer Data
        var rendererData = ScriptableObject.CreateInstance<UniversalRendererData>();
        rendererData.name = "UniversalRendererData";
        AssetDatabase.CreateAsset(rendererData, "Assets/Settings/UniversalRendererData.asset");

        // Create the Pipeline Asset using the renderer data
        var urpAsset = UniversalRenderPipelineAsset.Create(rendererData);
        urpAsset.name = "UniversalRenderPipelineAsset";
        AssetDatabase.CreateAsset(urpAsset, "Assets/Settings/UniversalRenderPipelineAsset.asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Assign as the active render pipeline
        GraphicsSettings.defaultRenderPipeline = urpAsset;
        EditorUtility.SetDirty(urpAsset);
        AssetDatabase.SaveAssets();

        Debug.Log("[SetupURPPipeline] URP pipeline asset created and assigned. Path: Assets/Settings/UniversalRenderPipelineAsset.asset");
        Selection.activeObject = urpAsset;
    }
}
