using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;
using CosmicColony;

namespace CosmicColony.Editor
{
    public static class DevPortalSetup
    {
        private const string LOFT3D_SOURCE = "Assets/TopDownEngine/Demos/Loft3D/Loft3D.unity";
        private const string LOFT3D_DEST_FOLDER = "Assets/Scenes/Loft3D";
        private const string LOFT3D_DEST = "Assets/Scenes/Loft3D/Loft3D.unity";
        private const string CORE_SCENE = "Assets/Scenes/CoreScene/CoreScene.unity";

        [MenuItem("Tools/Galactic Crossing/Setup Dev Portal")]
        public static void SetupDevPortal()
        {
            // 1. Copy Loft3D scene if not already there
            if (!AssetDatabase.AssetPathExists(LOFT3D_DEST))
            {
                if (!AssetDatabase.AssetPathExists(LOFT3D_DEST_FOLDER))
                    AssetDatabase.CreateFolder("Assets/Scenes", "Loft3D");

                bool copied = AssetDatabase.CopyAsset(LOFT3D_SOURCE, LOFT3D_DEST);
                if (!copied)
                {
                    Debug.LogError($"[DevPortalSetup] Failed to copy Loft3D scene to {LOFT3D_DEST}");
                    return;
                }
                AssetDatabase.SaveAssets();
                Debug.Log($"[DevPortalSetup] Copied Loft3D to {LOFT3D_DEST}");
            }
            else
            {
                Debug.Log("[DevPortalSetup] Loft3D scene already exists at destination.");
            }

            // 2. Add scenes to Build Settings (CoreScene first, then Loft3D)
            var buildScenes = EditorBuildSettings.scenes.ToList();
            AddSceneIfMissing(buildScenes, LOFT3D_DEST, 0);
            AddSceneIfMissing(buildScenes, CORE_SCENE, 0);
            EditorBuildSettings.scenes = buildScenes.ToArray();
            Debug.Log("[DevPortalSetup] Build Settings updated.");

            // 3. Find Canvas in current scene
            var canvasGO = GameObject.Find("UICamera/Canvas");
            if (canvasGO == null)
            {
                Debug.LogError("[DevPortalSetup] Could not find UICamera/Canvas in current scene. Make sure CoreScene is open.");
                return;
            }

            // 4. Skip if already set up
            if (canvasGO.transform.Find("ButtonLoft3D") != null)
            {
                Debug.Log("[DevPortalSetup] ButtonLoft3D already exists. Skipping button creation.");
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
                return;
            }

            // 5. Find existing ButtonAlt to clone from
            var buttonAltTF = canvasGO.transform.Find("ButtonAlt");
            if (buttonAltTF == null)
            {
                Debug.LogError("[DevPortalSetup] ButtonAlt not found in Canvas.");
                return;
            }

            // 6. Duplicate
            var newButton = Object.Instantiate(buttonAltTF.gameObject, canvasGO.transform);
            newButton.name = "ButtonLoft3D";

            // 7. Position: ButtonAlt is at y=107, new button above it (y=215 = 107 + 88 height + 20 gap)
            var rt = newButton.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(0f, 215f);

            // 8. Change label text
            var text = newButton.GetComponentInChildren<Text>();
            if (text != null)
                text.text = "LOFT 3D";

            // 9. Add PortalButton component
            var portal = newButton.AddComponent<PortalButton>();
            portal.SceneName = "Loft3D";

            // 10. Rewire MMTouchButton — remove old persistent listeners, add GoToScene
            var touchBtn = newButton.GetComponentInChildren<MMTouchButton>();
            if (touchBtn != null)
            {
                int listenerCount = touchBtn.ButtonPressedFirstTime.GetPersistentEventCount();
                for (int i = listenerCount - 1; i >= 0; i--)
                    UnityEventTools.RemovePersistentListener(touchBtn.ButtonPressedFirstTime, i);

                UnityEventTools.AddPersistentListener(touchBtn.ButtonPressedFirstTime, portal.GoToScene);
                EditorUtility.SetDirty(touchBtn);
            }
            else
            {
                Debug.LogWarning("[DevPortalSetup] MMTouchButton not found in duplicated button children.");
            }

            // 11. Mark scene dirty and save
            EditorUtility.SetDirty(canvasGO);
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            Debug.Log("[DevPortalSetup] Done! ButtonLoft3D added to Canvas, Loft3D scene copied, Build Settings updated.");
        }

        private static void AddSceneIfMissing(List<EditorBuildSettingsScene> scenes, string path, int insertAt)
        {
            if (scenes.Any(s => s.path == path)) return;
            scenes.Insert(insertAt, new EditorBuildSettingsScene(path, true));
        }
    }
}
