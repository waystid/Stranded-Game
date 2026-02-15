using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using System.Collections.Generic;
using System.Text;

namespace GalacticCrossing.Editor
{
    /// <summary>
    /// Validates the MVP setup and provides detailed reports.
    /// Checks for required components, assets, and configuration.
    /// </summary>
    public class SetupValidator : EditorWindow
    {
        #region Window

        private Vector2 scrollPosition;
        private ValidationReport lastReport;

        [MenuItem("Tools/Galactic Crossing/Validate Setup", false, 250)]
        public static void ShowWindow()
        {
            var window = GetWindow<SetupValidator>("Setup Validator");
            window.minSize = new Vector2(500, 600);
            window.Show();
        }

        #endregion

        #region GUI

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label("Galactic Crossing Setup Validator", EditorStyles.boldLabel);
            GUILayout.Label("Validates MVP setup and configuration", EditorStyles.miniLabel);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();

            if (GUILayout.Button("Run Validation", GUILayout.Height(30)))
            {
                lastReport = ValidateSetup();
            }

            EditorGUILayout.Space();

            if (lastReport != null)
            {
                DisplayReport(lastReport);
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "Click 'Run Validation' to check your MVP setup.\n\n" +
                    "This will verify:\n" +
                    "• Scene configuration\n" +
                    "• Required assets\n" +
                    "• Component setup\n" +
                    "• Prefab integrity",
                    MessageType.Info);
            }
        }

        private void DisplayReport(ValidationReport report)
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Overall Status
            GUILayout.Label("Overall Status", EditorStyles.boldLabel);
            if (report.IsValid)
            {
                EditorGUILayout.HelpBox("✓ Setup is complete and valid!", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox(
                    $"✗ Setup has {report.ErrorCount} error(s) and {report.WarningCount} warning(s)",
                    MessageType.Warning);
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();

            // Scene Validation
            DisplaySection("Scene Validation", report.SceneResults);

            // Asset Validation
            DisplaySection("Asset Validation", report.AssetResults);

            // Component Validation
            DisplaySection("Component Validation", report.ComponentResults);

            // Prefab Validation
            DisplaySection("Prefab Validation", report.PrefabResults);

            // Summary
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            EditorGUILayout.Space();

            GUILayout.Label("Summary", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Total Checks:", report.TotalChecks.ToString());
            EditorGUILayout.LabelField("Passed:", report.PassedChecks.ToString());
            EditorGUILayout.LabelField("Errors:", report.ErrorCount.ToString());
            EditorGUILayout.LabelField("Warnings:", report.WarningCount.ToString());

            EditorGUILayout.EndScrollView();
        }

        private void DisplaySection(string title, List<ValidationResult> results)
        {
            GUILayout.Label(title, EditorStyles.boldLabel);

            foreach (var result in results)
            {
                MessageType messageType = result.IsError ? MessageType.Error :
                                         result.IsWarning ? MessageType.Warning :
                                         MessageType.Info;

                string icon = result.Passed ? "✓" : "✗";
                EditorGUILayout.HelpBox($"{icon} {result.Message}", messageType);
            }

            EditorGUILayout.Space();
        }

        #endregion

        #region Validation

        /// <summary>
        /// Main validation entry point
        /// </summary>
        public static ValidationReport ValidateSetup()
        {
            Debug.Log("[SetupValidator] Running validation...");

            ValidationReport report = new ValidationReport();

            ValidateScene(report);
            ValidateAssets(report);
            ValidateComponents(report);
            ValidatePrefabs(report);

            Debug.Log($"[SetupValidator] Validation complete: {report.PassedChecks}/{report.TotalChecks} checks passed");

            return report;
        }

        /// <summary>
        /// Validates scene configuration
        /// </summary>
        private static void ValidateScene(ValidationReport report)
        {
            // Check if MVP scene exists
            string mvpScenePath = "Assets/Scenes/GalacticCrossingMVP.unity";
            if (System.IO.File.Exists(mvpScenePath))
            {
                report.AddSceneResult(true, "MVP scene exists");
            }
            else
            {
                report.AddSceneResult(false, "MVP scene not found", true);
            }

            // Check current scene
            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene.name == "GalacticCrossingMVP")
            {
                report.AddSceneResult(true, "Correct scene is open");

                // Validate scene objects
                ValidateSceneObjects(report);
            }
            else
            {
                report.AddSceneResult(false, "MVP scene is not currently open", false, true);
            }
        }

        /// <summary>
        /// Validates objects in the current scene
        /// </summary>
        private static void ValidateSceneObjects(ValidationReport report)
        {
            // Check for GridManager
            var gridManager = Object.FindFirstObjectByType<GridManager>();
            if (gridManager != null)
            {
                report.AddSceneResult(true, "GridManager exists in scene");

                if (gridManager.GridOrigin != null)
                {
                    report.AddSceneResult(true, "GridManager has GridOrigin configured");
                }
                else
                {
                    report.AddSceneResult(false, "GridManager missing GridOrigin", false, true);
                }
            }
            else
            {
                report.AddSceneResult(false, "GridManager not found in scene", true);
            }

            // Check for Player
            var characters = Object.FindObjectsByType<Character>(FindObjectsSortMode.None);
            Character player = null;
            foreach (var character in characters)
            {
                if (character.CharacterType == Character.CharacterTypes.Player)
                {
                    player = character;
                    break;
                }
            }

            if (player != null)
            {
                report.AddSceneResult(true, "Player character exists");

                // Check player components
                ValidatePlayerComponents(player, report);
            }
            else
            {
                report.AddSceneResult(false, "Player character not found", true);
            }

            // Check for AI enemies (should be removed)
            var aiBrains = Object.FindObjectsByType<AIBrain>(FindObjectsSortMode.None);
            if (aiBrains.Length == 0)
            {
                report.AddSceneResult(true, "No AI enemies found (correctly removed)");
            }
            else
            {
                report.AddSceneResult(false, $"{aiBrains.Length} AI enemies still in scene", false, true);
            }
        }

        /// <summary>
        /// Validates player character components
        /// </summary>
        private static void ValidatePlayerComponents(Character player, ValidationReport report)
        {
            // Check for CharacterHandleWeapon (should be removed)
            var handleWeapon = player.GetComponent<CharacterHandleWeapon>();
            if (handleWeapon == null)
            {
                report.AddComponentResult(true, "CharacterHandleWeapon removed from player");
            }
            else
            {
                report.AddComponentResult(false, "CharacterHandleWeapon still on player", false, true);
            }

            // Check for CharacterGridMovement
            var gridMovement = player.GetComponent<CharacterGridMovement>();
            if (gridMovement != null)
            {
                report.AddComponentResult(true, "CharacterGridMovement added to player");
            }
            else
            {
                report.AddComponentResult(false, "CharacterGridMovement not found on player", false, true);
            }

            // Check for Health component
            var health = player.GetComponent<Health>();
            if (health != null)
            {
                report.AddComponentResult(true, "Health component present on player");
            }
            else
            {
                report.AddComponentResult(false, "Health component missing from player", true);
            }
        }

        /// <summary>
        /// Validates created assets
        /// </summary>
        private static void ValidateAssets(ValidationReport report)
        {
            // Check for item assets
            ValidateItemAsset(report, "Assets/Resources/Items/ScrapMetal.asset", "ScrapMetal");
            ValidateItemAsset(report, "Assets/Resources/Items/EnergyCrystal.asset", "EnergyCrystal");
            ValidateItemAsset(report, "Assets/Resources/Items/AlienBerry.asset", "AlienBerry");

            // Check for directories
            if (AssetDatabase.IsValidFolder("Assets/Resources/Items"))
            {
                report.AddAssetResult(true, "Items directory exists");
            }
            else
            {
                report.AddAssetResult(false, "Items directory missing", true);
            }

            if (AssetDatabase.IsValidFolder("Assets/Prefabs"))
            {
                report.AddAssetResult(true, "Prefabs directory exists");
            }
            else
            {
                report.AddAssetResult(false, "Prefabs directory missing", true);
            }
        }

        /// <summary>
        /// Validates a single item asset
        /// </summary>
        private static void ValidateItemAsset(ValidationReport report, string path, string itemName)
        {
            var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            if (asset != null)
            {
                report.AddAssetResult(true, $"{itemName} asset exists");
            }
            else
            {
                report.AddAssetResult(false, $"{itemName} asset not found at {path}", true);
            }
        }

        /// <summary>
        /// Validates component configuration
        /// </summary>
        private static void ValidateComponents(ValidationReport report)
        {
            // Additional component checks can be added here
            report.AddComponentResult(true, "Component validation complete");
        }

        /// <summary>
        /// Validates created prefabs
        /// </summary>
        private static void ValidatePrefabs(ValidationReport report)
        {
            ValidatePrefab(report, "Assets/Prefabs/ScrapMetalPicker.prefab", "ScrapMetalPicker");
            ValidatePrefab(report, "Assets/Prefabs/EnergyCrystalPicker.prefab", "EnergyCrystalPicker");
            ValidatePrefab(report, "Assets/Prefabs/AlienBerryPicker.prefab", "AlienBerryPicker");
            ValidatePrefab(report, "Assets/Prefabs/AlienTree.prefab", "AlienTree");
            ValidatePrefab(report, "Assets/Prefabs/GAIA_NPC.prefab", "GAIA_NPC");
        }

        /// <summary>
        /// Validates a single prefab
        /// </summary>
        private static void ValidatePrefab(ValidationReport report, string path, string prefabName)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                report.AddPrefabResult(true, $"{prefabName} prefab exists");
            }
            else
            {
                report.AddPrefabResult(false, $"{prefabName} prefab not found", true);
            }
        }

        #endregion

        #region Validation Report

        /// <summary>
        /// Container for validation results
        /// </summary>
        public class ValidationReport
        {
            public List<ValidationResult> SceneResults = new List<ValidationResult>();
            public List<ValidationResult> AssetResults = new List<ValidationResult>();
            public List<ValidationResult> ComponentResults = new List<ValidationResult>();
            public List<ValidationResult> PrefabResults = new List<ValidationResult>();

            public int TotalChecks => SceneResults.Count + AssetResults.Count +
                                     ComponentResults.Count + PrefabResults.Count;

            public int PassedChecks
            {
                get
                {
                    int count = 0;
                    count += SceneResults.FindAll(r => r.Passed).Count;
                    count += AssetResults.FindAll(r => r.Passed).Count;
                    count += ComponentResults.FindAll(r => r.Passed).Count;
                    count += PrefabResults.FindAll(r => r.Passed).Count;
                    return count;
                }
            }

            public int ErrorCount
            {
                get
                {
                    int count = 0;
                    count += SceneResults.FindAll(r => r.IsError).Count;
                    count += AssetResults.FindAll(r => r.IsError).Count;
                    count += ComponentResults.FindAll(r => r.IsError).Count;
                    count += PrefabResults.FindAll(r => r.IsError).Count;
                    return count;
                }
            }

            public int WarningCount
            {
                get
                {
                    int count = 0;
                    count += SceneResults.FindAll(r => r.IsWarning).Count;
                    count += AssetResults.FindAll(r => r.IsWarning).Count;
                    count += ComponentResults.FindAll(r => r.IsWarning).Count;
                    count += PrefabResults.FindAll(r => r.IsWarning).Count;
                    return count;
                }
            }

            public bool IsValid => ErrorCount == 0;

            public void AddSceneResult(bool passed, string message, bool isError = false, bool isWarning = false)
            {
                SceneResults.Add(new ValidationResult
                {
                    Passed = passed,
                    Message = message,
                    IsError = isError,
                    IsWarning = isWarning
                });
            }

            public void AddAssetResult(bool passed, string message, bool isError = false, bool isWarning = false)
            {
                AssetResults.Add(new ValidationResult
                {
                    Passed = passed,
                    Message = message,
                    IsError = isError,
                    IsWarning = isWarning
                });
            }

            public void AddComponentResult(bool passed, string message, bool isError = false, bool isWarning = false)
            {
                ComponentResults.Add(new ValidationResult
                {
                    Passed = passed,
                    Message = message,
                    IsError = isError,
                    IsWarning = isWarning
                });
            }

            public void AddPrefabResult(bool passed, string message, bool isError = false, bool isWarning = false)
            {
                PrefabResults.Add(new ValidationResult
                {
                    Passed = passed,
                    Message = message,
                    IsError = isError,
                    IsWarning = isWarning
                });
            }
        }

        /// <summary>
        /// Individual validation result
        /// </summary>
        public class ValidationResult
        {
            public bool Passed;
            public string Message;
            public bool IsError;
            public bool IsWarning;
        }

        #endregion
    }
}
