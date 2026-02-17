using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System;
using System.IO;

namespace GalacticCrossing.Editor
{
    /// <summary>
    /// Main Editor menu script for automating the Galactic Crossing MVP setup.
    /// Provides a centralized menu interface to run all setup operations.
    /// </summary>
    public class GalacticCrossingSetup : EditorWindow
    {
        #region Constants

        private const string MENU_PATH = "Tools/Galactic Crossing/";
        private const string LOFT_SCENE_PATH = "Assets/TopDownEngine/Demos/Loft3D/Loft3D.unity";
        private const string MVP_SCENE_PATH = "Assets/Scenes/GalacticCrossingMVP.unity";
        private const string SCENES_FOLDER = "Assets/Scenes";

        #endregion

        #region Menu Items

        /// <summary>
        /// Main menu item to run complete MVP setup
        /// </summary>
        [MenuItem(MENU_PATH + "Setup MVP", false, 1)]
        public static void SetupMVP()
        {
            if (!EditorUtility.DisplayDialog(
                "Galactic Crossing MVP Setup",
                "This will create a new scene and configure all MVP components.\n\n" +
                "The following operations will be performed:\n" +
                "1. Duplicate Loft3D scene\n" +
                "2. Remove AI enemies and weapons\n" +
                "3. Configure player for grid movement\n" +
                "4. Create resource items and prefabs\n" +
                "5. Setup GridManager\n\n" +
                "This process may take 2-3 minutes. Continue?",
                "Yes, Setup MVP",
                "Cancel"))
            {
                return;
            }

            try
            {
                RunCompleteSetup();
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Setup Failed",
                    $"An error occurred during setup:\n\n{e.Message}\n\nSee Console for details.",
                    "OK");
                Debug.LogError($"[GalacticCrossing] Setup failed: {e}");
            }
        }

        [MenuItem(MENU_PATH + "1. Scene Setup Only", false, 100)]
        public static void SetupSceneOnly()
        {
            try
            {
                EditorUtility.DisplayProgressBar("Scene Setup", "Setting up scene...", 0f);
                SceneSetupAutomation.SetupScene();
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Success", "Scene setup completed successfully!", "OK");
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogError($"[GalacticCrossing] Scene setup failed: {e}");
            }
        }

        [MenuItem(MENU_PATH + "2. Create Assets Only", false, 101)]
        public static void CreateAssetsOnly()
        {
            try
            {
                EditorUtility.DisplayProgressBar("Asset Creation", "Creating assets...", 0f);
                AssetCreationAutomation.CreateAllAssets();
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Success", "Assets created successfully!", "OK");
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogError($"[GalacticCrossing] Asset creation failed: {e}");
            }
        }

        [MenuItem(MENU_PATH + "3. Configure Scene Objects", false, 102)]
        public static void ConfigureSceneObjects()
        {
            try
            {
                EditorUtility.DisplayProgressBar("Configuration", "Configuring scene objects...", 0f);
                SceneSetupAutomation.ConfigureSceneObjects();
                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("Success", "Scene objects configured successfully!", "OK");
            }
            catch (Exception e)
            {
                EditorUtility.ClearProgressBar();
                Debug.LogError($"[GalacticCrossing] Configuration failed: {e}");
            }
        }

        [MenuItem(MENU_PATH + "Open Setup Window", false, 200)]
        public static void ValidateSetup()
        {
            ShowWindow();
        }

        #endregion

        #region Complete Setup

        /// <summary>
        /// Runs the complete MVP setup process
        /// </summary>
        private static void RunCompleteSetup()
        {
            int totalSteps = 5;
            int currentStep = 0;

            // Step 1: Create Scenes folder and duplicate scene
            currentStep++;
            EditorUtility.DisplayProgressBar("MVP Setup",
                $"Step {currentStep}/{totalSteps}: Creating scene...",
                (float)currentStep / totalSteps);
            CreateScenesFolder();
            DuplicateScene();

            // Step 2: Setup scene (remove enemies, weapons, etc.)
            currentStep++;
            EditorUtility.DisplayProgressBar("MVP Setup",
                $"Step {currentStep}/{totalSteps}: Cleaning up scene...",
                (float)currentStep / totalSteps);
            SceneSetupAutomation.RemoveAIEnemies();
            SceneSetupAutomation.RemoveWeapons();

            // Step 3: Create assets
            currentStep++;
            EditorUtility.DisplayProgressBar("MVP Setup",
                $"Step {currentStep}/{totalSteps}: Creating game assets...",
                (float)currentStep / totalSteps);
            AssetCreationAutomation.CreateAllAssets();

            // Step 4: Configure scene objects
            currentStep++;
            EditorUtility.DisplayProgressBar("MVP Setup",
                $"Step {currentStep}/{totalSteps}: Configuring scene objects...",
                (float)currentStep / totalSteps);
            SceneSetupAutomation.ConfigureSceneObjects();

            // Step 5: Final validation and save
            currentStep++;
            EditorUtility.DisplayProgressBar("MVP Setup",
                $"Step {currentStep}/{totalSteps}: Finalizing setup...",
                (float)currentStep / totalSteps);
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.ClearProgressBar();

            // Show results
            EditorUtility.DisplayDialog("Setup Complete!",
                "Galactic Crossing MVP has been set up successfully!\n\n" +
                "Scene: " + MVP_SCENE_PATH + "\n\n" +
                "Next steps:\n" +
                "1. Press Play to test the scene\n" +
                "2. Configure player controls in Project Settings > Input\n" +
                "3. Adjust GridManager settings if needed\n\n" +
                "Check the Console for detailed logs.",
                "OK");

            Debug.Log("[GalacticCrossing] MVP setup completed successfully!");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates the Scenes folder if it doesn't exist
        /// </summary>
        private static void CreateScenesFolder()
        {
            if (!AssetDatabase.IsValidFolder(SCENES_FOLDER))
            {
                string parentFolder = Path.GetDirectoryName(SCENES_FOLDER).Replace("\\", "/");
                string folderName = Path.GetFileName(SCENES_FOLDER);
                AssetDatabase.CreateFolder(parentFolder, folderName);
                Debug.Log($"[GalacticCrossing] Created folder: {SCENES_FOLDER}");
            }
        }

        /// <summary>
        /// Duplicates the Loft3D scene to create the MVP scene
        /// </summary>
        private static void DuplicateScene()
        {
            if (!File.Exists(LOFT_SCENE_PATH))
            {
                throw new FileNotFoundException($"Source scene not found: {LOFT_SCENE_PATH}");
            }

            // Check if MVP scene already exists
            if (File.Exists(MVP_SCENE_PATH))
            {
                if (!EditorUtility.DisplayDialog("Scene Exists",
                    $"The MVP scene already exists at:\n{MVP_SCENE_PATH}\n\nOverwrite it?",
                    "Yes, Overwrite",
                    "Cancel"))
                {
                    throw new OperationCanceledException("User cancelled scene duplication.");
                }
            }

            // Copy the scene
            AssetDatabase.CopyAsset(LOFT_SCENE_PATH, MVP_SCENE_PATH);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Open the new scene
            EditorSceneManager.OpenScene(MVP_SCENE_PATH);

            Debug.Log($"[GalacticCrossing] Scene duplicated: {LOFT_SCENE_PATH} -> {MVP_SCENE_PATH}");
        }

        #endregion

        #region Validation Window

        private Vector2 scrollPosition;

        [MenuItem(MENU_PATH + "About", false, 300)]
        public static void ShowWindow()
        {
            var window = GetWindow<GalacticCrossingSetup>("GC Setup");
            window.minSize = new Vector2(400, 500);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            GUILayout.Label("Galactic Crossing MVP Setup", EditorStyles.boldLabel);
            GUILayout.Label("Automation Tools for Unity Editor", EditorStyles.miniLabel);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Setup Section
            GUILayout.Label("Quick Setup", EditorStyles.boldLabel);
            GUILayout.Label("Run the complete setup process:", EditorStyles.wordWrappedLabel);

            if (GUILayout.Button("Setup Complete MVP", GUILayout.Height(30)))
            {
                SetupMVP();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();

            // Individual Steps
            GUILayout.Label("Individual Setup Steps", EditorStyles.boldLabel);
            GUILayout.Label("Run individual setup operations:", EditorStyles.wordWrappedLabel);

            if (GUILayout.Button("1. Scene Setup"))
            {
                SetupSceneOnly();
            }

            if (GUILayout.Button("2. Create Assets"))
            {
                CreateAssetsOnly();
            }

            if (GUILayout.Button("3. Configure Scene Objects"))
            {
                ConfigureSceneObjects();
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();

            // Validation Section
            GUILayout.Label("Validation", EditorStyles.boldLabel);

            bool sceneExists = File.Exists(MVP_SCENE_PATH);
            EditorGUILayout.LabelField("MVP Scene:", sceneExists ? "✓ Created" : "✗ Not Found");

            bool assetsExist = AssetDatabase.IsValidFolder("Assets/Resources/Items");
            EditorGUILayout.LabelField("Item Assets:", assetsExist ? "✓ Created" : "✗ Not Found");

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();

            // Info Section
            GUILayout.Label("Information", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "This tool automates the setup of the Galactic Crossing MVP.\n\n" +
                "It will:\n" +
                "• Duplicate the Loft3D scene\n" +
                "• Remove combat elements\n" +
                "• Add grid movement\n" +
                "• Create resource items\n" +
                "• Setup NPCs and environment",
                MessageType.Info);

            EditorGUILayout.EndScrollView();
        }

        #endregion
    }
}
