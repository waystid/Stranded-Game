using UnityEngine;
using UnityEditor;
using GiantGrey.TileWorldCreator;

/// <summary>
/// One-shot editor utility: Tools > Galactic Crossing > Setup TWC Island
///
/// Creates:
///   - Assets/Data/IslandConfiguration.asset  — TWC Configuration (40x40, CellularAutomata)
///   - TileWorldCreator GO in the active scene (position 0,0,0, Y=45°)
///     with TileWorldCreatorManager + TileWorldCreatorBridge wired to IslandGridManager
///
/// Run once after importing TileWorldCreator v4.
/// Safe to re-run — updates config and wires GO if already created.
/// </summary>
public static class TWCIslandSetup
{
    [MenuItem("Tools/Galactic Crossing/Setup TWC Island")]
    public static void SetupIsland()
    {
        // ── 1. Create or load Configuration asset ──────────────────────────────
        const string configPath = "Assets/Data/IslandConfiguration.asset";
        Configuration config = AssetDatabase.LoadAssetAtPath<Configuration>(configPath);
        bool isNew = config == null;

        if (isNew)
        {
            config = ScriptableObject.CreateInstance<Configuration>();
            AssetDatabase.CreateAsset(config, configPath);
        }

        // Grid dimensions
        config.width               = 40;
        config.height              = 40;
        config.cellSize            = 1f;
        config.useGlobalRandomSeed = true;
        config.globalRandomSeed    = 42;
        config.mergeTiles          = true;

        // ── 2. Blueprint Layer folder + IslandShape layer ──────────────────────
        if (config.blueprintLayerFolders == null)
            config.blueprintLayerFolders = new System.Collections.Generic.List<BlueprintLayerFolder>();

        BlueprintLayerFolder bpFolder;
        if (config.blueprintLayerFolders.Count > 0)
            bpFolder = config.blueprintLayerFolders[0];
        else
        {
            bpFolder = new BlueprintLayerFolder("Blueprint Layers");
            config.blueprintLayerFolders.Add(bpFolder);
        }

        // Find or create IslandShape layer
        BlueprintLayer islandLayer = null;
        foreach (var l in bpFolder.blueprintLayers)
            if (l != null && l.layerName == "IslandShape") { islandLayer = l; break; }

        if (islandLayer == null)
        {
            islandLayer            = ScriptableObject.CreateInstance<BlueprintLayer>();
            islandLayer.name       = "IslandShape";   // asset sub-object name
            islandLayer.layerName  = "IslandShape";
            islandLayer.isEnabled  = true;
            AssetDatabase.AddObjectToAsset(islandLayer, configPath);
            bpFolder.blueprintLayers.Add(islandLayer);
        }

        // ── 3. CellularAutomata modifier ───────────────────────────────────────
        bool hasCA = false;
        foreach (var m in islandLayer.tileMapModifiers)
            if (m is CellularAutomata) { hasCA = true; break; }

        if (!hasCA)
        {
            var ca               = ScriptableObject.CreateInstance<CellularAutomata>();
            ca.name              = "CellularAutomata";
            ca.fillProbability   = 0.45f;
            ca.smoothingSteps    = 4;
            ca.ensureConnected   = true;
            ca.useMapSize        = true;
            AssetDatabase.AddObjectToAsset(ca, configPath);
            islandLayer.tileMapModifiers.Add(ca);
        }

        // ── 4. Build Layer folder + Ground TilesBuildLayer ─────────────────────
        if (config.buildLayerFolders == null)
            config.buildLayerFolders = new System.Collections.Generic.List<BuildLayerFolder>();

        BuildLayerFolder buildFolder;
        if (config.buildLayerFolders.Count > 0)
            buildFolder = config.buildLayerFolders[0];
        else
        {
            buildFolder = new BuildLayerFolder("Build Layers");
            config.buildLayerFolders.Add(buildFolder);
        }

        bool hasBuildLayer = false;
        foreach (var bl in buildFolder.buildLayers)
            if (bl != null && bl.layerName == "Ground") { hasBuildLayer = true; break; }

        if (!hasBuildLayer)
        {
            var ground                         = ScriptableObject.CreateInstance<TilesBuildLayer>();
            ground.name                        = "Ground";
            ground.layerName                   = "Ground";
            ground.isEnabled                   = true;
            ground.useDualGrid                 = true;
            ground.assignedBlueprintLayerGuid  = islandLayer.guid;

            // Assign grass tile preset bundled with TWC
            const string presetPath = "Assets/TileWorldCreator/Tiles URP/GrassTiles/GrassTilesPreset.asset";
            var grassPreset = AssetDatabase.LoadAssetAtPath<TilePreset>(presetPath);
            if (grassPreset != null)
                ground.tilePresetsTop.Add(new TilesBuildLayer.TilePresetSelection { preset = grassPreset });
            else
                Debug.LogWarning("[TWCIslandSetup] Grass preset not found — assign a tile preset to 'Ground' build layer manually.");

            AssetDatabase.AddObjectToAsset(ground, configPath);
            buildFolder.buildLayers.Add(ground);
        }

        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[TWCIslandSetup] Configuration asset {(isNew ? "created" : "updated")}: {configPath}");

        // ── 5. Create or update TileWorldCreator GO in scene ──────────────────
        var existingTWC = Object.FindObjectOfType<TileWorldCreatorManager>();
        if (existingTWC != null)
        {
            existingTWC.configuration = config;
            EditorUtility.SetDirty(existingTWC);
            Debug.Log("[TWCIslandSetup] Existing TileWorldCreatorManager found — updated configuration reference.");
            return;
        }

        var go = new GameObject("TileWorldCreator");
        go.transform.position = Vector3.zero;
        go.transform.rotation = Quaternion.Euler(0f, 45f, 0f);

        var manager       = go.AddComponent<TileWorldCreatorManager>();
        manager.configuration = config;

        var bridge                = go.AddComponent<TileWorldCreatorBridge>();
        bridge.IslandLayerName    = "IslandShape";
        bridge.AutoGenerate       = true;

        // Point IslandGridManager.IslandRoot at the TWC GO (matches island orientation)
        var islandGridMgr = Object.FindObjectOfType<IslandGridManager>();
        if (islandGridMgr != null)
        {
            islandGridMgr.IslandRoot = go.transform;
            EditorUtility.SetDirty(islandGridMgr);
            Debug.Log("[TWCIslandSetup] IslandGridManager.IslandRoot → TileWorldCreator GO.");
        }

        Undo.RegisterCreatedObjectUndo(go, "Create TileWorldCreator");
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log("[TWCIslandSetup] Done. TileWorldCreator GO selected. Hit Play to generate island.");
        Selection.activeGameObject = go;
    }
}
