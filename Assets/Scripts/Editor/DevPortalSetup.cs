using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using CosmicColony;

namespace CosmicColony.Editor
{
    public static class DevPortalSetup
    {
        private const string LOFT3D_SOURCE      = "Assets/TopDownEngine/Demos/Loft3D/Loft3D.unity";
        private const string LOFT3D_DEST_FOLDER = "Assets/Scenes/Loft3D";
        private const string LOFT3D_DEST        = "Assets/Scenes/Loft3D/Loft3D.unity";
        private const string STRANDED_DEV_PATH  = "Assets/Scenes/Loft3D/StrandedDev.unity";
        private const string STRANDED_DEV_NAME  = "StrandedDev";
        private const string CORE_SCENE         = "Assets/Scenes/CoreScene/CoreScene.unity";
        private const string LEVEL_SELECTION    = "Assets/TopDownEngine/Demos/LevelSelection/LevelSelection.unity";
        private const string LOFT_SPRITE        = "Assets/TopDownEngine/Demos/LevelSelection/Sprites/level-map-loft.png";

        private const string SYNTY_CC_SOURCE    = "Assets/Synty/SidekickCharacters/_Demos/_Scenes/Sidekick_Tool/Sidekick_RuntimePresetDemo.unity";
        private const string SYNTY_CC_FOLDER    = "Assets/Scenes/CharacterCreator";
        private const string SYNTY_CC_PATH      = "Assets/Scenes/CharacterCreator/SyntyCharCreator.unity";
        private const string SYNTY_CC_NAME      = "SyntyCharCreator";
        private const string MINIMAL3D_SPRITE   = "Assets/TopDownEngine/Demos/LevelSelection/Sprites/level-map-minimal3D.png";

        // ── Original setup (kept for reference) ──────────────────────────────
        [MenuItem("Tools/Galactic Crossing/Setup Dev Portal")]
        public static void SetupDevPortal()
        {
            // Copy Loft3D if not already there
            if (!AssetDatabase.AssetPathExists(LOFT3D_DEST))
            {
                if (!AssetDatabase.AssetPathExists(LOFT3D_DEST_FOLDER))
                    AssetDatabase.CreateFolder("Assets/Scenes", "Loft3D");
                if (!AssetDatabase.CopyAsset(LOFT3D_SOURCE, LOFT3D_DEST))
                { Debug.LogError("[DevPortalSetup] Failed to copy Loft3D."); return; }
                AssetDatabase.SaveAssets();
            }

            var buildScenes = EditorBuildSettings.scenes.ToList();
            AddSceneIfMissing(buildScenes, LOFT3D_DEST, 0);
            AddSceneIfMissing(buildScenes, CORE_SCENE, 0);
            EditorBuildSettings.scenes = buildScenes.ToArray();
            Debug.Log("[DevPortalSetup] Build Settings updated.");
        }

        // ── Main task: rename → add Stranded card → clean CoreScene ──────────
        [MenuItem("Tools/Galactic Crossing/Add Stranded to Level Selection")]
        public static void AddStrandedToLevelSelection()
        {
            // 1. Rename Loft3D.unity → StrandedDev.unity (GUID preserved)
            string renameSrc = AssetDatabase.AssetPathExists(LOFT3D_DEST) ? LOFT3D_DEST
                             : AssetDatabase.AssetPathExists(STRANDED_DEV_PATH) ? null
                             : null;

            if (renameSrc != null)
            {
                string err = AssetDatabase.RenameAsset(renameSrc, "StrandedDev.unity");
                if (!string.IsNullOrEmpty(err))
                { Debug.LogError($"[DevPortalSetup] Rename failed: {err}"); return; }
                AssetDatabase.SaveAssets();
                Debug.Log($"[DevPortalSetup] Renamed to {STRANDED_DEV_PATH}");
            }
            else if (!AssetDatabase.AssetPathExists(STRANDED_DEV_PATH))
            {
                // Neither exists — copy fresh and name it correctly
                if (!AssetDatabase.AssetPathExists(LOFT3D_DEST_FOLDER))
                    AssetDatabase.CreateFolder("Assets/Scenes", "Loft3D");
                if (!AssetDatabase.CopyAsset(LOFT3D_SOURCE, STRANDED_DEV_PATH))
                { Debug.LogError("[DevPortalSetup] Failed to copy Loft3D as StrandedDev."); return; }
                AssetDatabase.SaveAssets();
                Debug.Log($"[DevPortalSetup] Copied Loft3D as {STRANDED_DEV_PATH}");
            }
            else
            {
                Debug.Log("[DevPortalSetup] StrandedDev.unity already exists.");
            }

            // 2. Update Build Settings — replace old Loft3D entry with StrandedDev
            var buildScenes = EditorBuildSettings.scenes.ToList();
            var old = buildScenes.FirstOrDefault(s => s.path == LOFT3D_DEST);
            if (old != null) buildScenes.Remove(old);
            AddSceneIfMissing(buildScenes, STRANDED_DEV_PATH, 1); // index 1, after CoreScene
            AddSceneIfMissing(buildScenes, CORE_SCENE, 0);
            EditorBuildSettings.scenes = buildScenes.ToArray();
            Debug.Log("[DevPortalSetup] Build Settings updated with StrandedDev.");

            // 3. Open LevelSelection and add Stranded card
            var lsScene = EditorSceneManager.OpenScene(LEVEL_SELECTION, OpenSceneMode.Single);
            AddStrandedCard();
            EditorSceneManager.SaveScene(lsScene);
            Debug.Log("[DevPortalSetup] Stranded card added to LevelSelection.");

            // 4. Open CoreScene and remove ButtonLoft3D
            var coreScene = EditorSceneManager.OpenScene(CORE_SCENE, OpenSceneMode.Single);
            var buttonLoft = GameObject.Find("UICamera/Canvas/ButtonLoft3D");
            if (buttonLoft != null)
            {
                Object.DestroyImmediate(buttonLoft);
                Debug.Log("[DevPortalSetup] ButtonLoft3D removed from CoreScene.");
            }
            else
            {
                Debug.Log("[DevPortalSetup] ButtonLoft3D not found in CoreScene (already removed?).");
            }
            EditorSceneManager.SaveScene(coreScene);

            Debug.Log("[DevPortalSetup] All done!");
        }

        private static void AddStrandedCard()
        {
            // Find the carousel Content
            var content = GameObject.Find("UICamera/Canvas/Mask/MMCarousel/Content");
            if (content == null) { Debug.LogError("[DevPortalSetup] MMCarousel/Content not found."); return; }

            // Skip if already added
            if (content.transform.Find("Stranded") != null)
            { Debug.Log("[DevPortalSetup] Stranded card already exists."); return; }

            // Find KoalaDungeon as the template
            var koala = content.transform.Find("KoalaDungeon");
            if (koala == null) { Debug.LogError("[DevPortalSetup] KoalaDungeon card not found."); return; }

            // Duplicate
            var card = Object.Instantiate(koala.gameObject, content.transform);
            card.name = "Stranded";

            // Move to first position (index 0)
            card.transform.SetSiblingIndex(0);

            // Set title
            var title = card.transform.Find("ItemTitle")?.GetComponent<Text>();
            if (title != null) title.text = "Stranded";

            // Set description
            var desc = card.transform.Find("ItemText")?.GetComponent<Text>();
            if (desc != null) desc.text = "Dev Build";

            // Swap preview image to Loft sprite (closest match)
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(LOFT_SPRITE);
            if (sprite != null)
            {
                var img = card.GetComponent<Image>();
                if (img != null) img.sprite = sprite;
            }

            // Update LevelSelector — points to StrandedDev
            var levelSelector = card.GetComponentInChildren<LevelSelector>();
            if (levelSelector != null)
            {
                levelSelector.LevelName = STRANDED_DEV_NAME;
                levelSelector.DoNotUseLevelManager = true;
                EditorUtility.SetDirty(levelSelector);
            }
            else
            {
                Debug.LogWarning("[DevPortalSetup] LevelSelector not found on duplicated card.");
            }

            EditorUtility.SetDirty(content);
        }

        // ── Add Character Creator card ────────────────────────────────────────
        [MenuItem("Tools/Galactic Crossing/Add Character Creator to Level Selection")]
        public static void AddCharacterCreatorToLevelSelection()
        {
            // 1. Copy Synty preset demo scene to our folder
            if (!AssetDatabase.AssetPathExists(SYNTY_CC_PATH))
            {
                if (!AssetDatabase.AssetPathExists(SYNTY_CC_FOLDER))
                    AssetDatabase.CreateFolder("Assets/Scenes", "CharacterCreator");

                if (!AssetDatabase.CopyAsset(SYNTY_CC_SOURCE, SYNTY_CC_PATH))
                { Debug.LogError("[DevPortalSetup] Failed to copy Synty character creator scene."); return; }
                AssetDatabase.SaveAssets();
                Debug.Log($"[DevPortalSetup] Copied Synty CC scene to {SYNTY_CC_PATH}");
            }
            else
            {
                Debug.Log("[DevPortalSetup] SyntyCharCreator.unity already exists.");
            }

            // 2. Add to Build Settings at index 2 (after CoreScene=0, StrandedDev=1)
            var buildScenes = EditorBuildSettings.scenes.ToList();
            AddSceneIfMissing(buildScenes, SYNTY_CC_PATH, 2);
            EditorBuildSettings.scenes = buildScenes.ToArray();
            Debug.Log("[DevPortalSetup] Build Settings updated with SyntyCharCreator.");

            // 3. Open LevelSelection and add the card at position 1
            var lsScene = EditorSceneManager.OpenScene(LEVEL_SELECTION, OpenSceneMode.Single);
            AddCharacterCreatorCard();
            EditorSceneManager.SaveScene(lsScene);
            Debug.Log("[DevPortalSetup] Character Creator card added to LevelSelection.");

            // Leave LevelSelection open so it can be inspected
            Debug.Log("[DevPortalSetup] Done! SyntyCharCreator copied, card at slot 1 (Stranded→CharCreator→KoalaDungeon).");
        }

        private static void AddCharacterCreatorCard()
        {
            var content = GameObject.Find("UICamera/Canvas/Mask/MMCarousel/Content");
            if (content == null) { Debug.LogError("[DevPortalSetup] MMCarousel/Content not found."); return; }

            if (content.transform.Find("CharacterCreator") != null)
            { Debug.Log("[DevPortalSetup] CharacterCreator card already exists."); return; }

            // Use KoalaDungeon as template
            var koala = content.transform.Find("KoalaDungeon");
            if (koala == null) { Debug.LogError("[DevPortalSetup] KoalaDungeon not found."); return; }

            var card = Object.Instantiate(koala.gameObject, content.transform);
            card.name = "CharacterCreator";

            // Slot 1: after Stranded (0), before KoalaDungeon (1→2)
            card.transform.SetSiblingIndex(1);

            var title = card.transform.Find("ItemTitle")?.GetComponent<Text>();
            if (title != null) title.text = "Character Creator";

            var desc = card.transform.Find("ItemText")?.GetComponent<Text>();
            if (desc != null) desc.text = "Synty Dev";

            // Use a 3D sprite as stand-in preview
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(MINIMAL3D_SPRITE);
            if (sprite != null)
            {
                var img = card.GetComponent<Image>();
                if (img != null) img.sprite = sprite;
            }

            var levelSelector = card.GetComponentInChildren<LevelSelector>();
            if (levelSelector != null)
            {
                levelSelector.LevelName = SYNTY_CC_NAME;
                levelSelector.DoNotUseLevelManager = true;
                EditorUtility.SetDirty(levelSelector);
            }

            EditorUtility.SetDirty(content);
        }

        private static void AddSceneIfMissing(List<EditorBuildSettingsScene> scenes, string path, int insertAt)
        {
            if (scenes.Any(s => s.path == path)) return;
            scenes.Insert(insertAt, new EditorBuildSettingsScene(path, true));
        }
    }
}
