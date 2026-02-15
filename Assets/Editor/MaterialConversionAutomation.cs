using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace GalacticCrossing.Editor
{
    /// <summary>
    /// Material Conversion Automation Tool for Galactic Crossing
    ///
    /// This editor script automates the conversion of Loft3D materials to curved world shader variants.
    /// Based on the game design document, it:
    /// - Finds all materials in the Loft3D demo
    /// - Creates duplicates with "_CurvedWorld" suffix
    /// - Assigns the curved world shader (when available)
    /// - Provides batch assignment to scene objects
    /// - Supports undo operations
    ///
    /// Menu: Tools > Galactic Crossing > Convert Materials to Curved World
    /// </summary>
    public class MaterialConversionAutomation : EditorWindow
    {
        #region Constants

        private const string LOFT3D_MATERIALS_PATH = "Assets/TopDownEngine/Demos/Loft3D/Materials";
        private const string CURVED_WORLD_SUFFIX = "_CurvedWorld";
        private const string CURVED_WORLD_SHADER_NAME = "Shader Graphs/CurvedWorldLit";
        private const string FALLBACK_SHADER_NAME = "Universal Render Pipeline/Lit";

        #endregion

        #region UI State

        private Vector2 scrollPosition;
        private List<MaterialConversionItem> conversionItems = new List<MaterialConversionItem>();
        private bool includeSubdirectories = true;
        private bool autoAssignShader = true;
        private bool showExcludedMaterials = false;
        private int totalMaterials = 0;
        private int convertedMaterials = 0;

        #endregion

        #region Menu Items

        [MenuItem("Tools/Galactic Crossing/Convert Materials to Curved World")]
        public static void ShowWindow()
        {
            MaterialConversionAutomation window = GetWindow<MaterialConversionAutomation>("Material Conversion");
            window.minSize = new Vector2(600, 400);
            window.Show();
        }

        [MenuItem("Tools/Galactic Crossing/Batch Assign Materials to Scene")]
        public static void BatchAssignMaterialsToScene()
        {
            if (!EditorUtility.DisplayDialog("Batch Assign Materials",
                "This will scan all renderers in the scene and replace standard materials with their _CurvedWorld variants if available.\n\nThis operation can be undone.",
                "Proceed", "Cancel"))
            {
                return;
            }

            int assignmentCount = 0;
            Renderer[] allRenderers = ShaderHelperUtility.FindAllRenderers(true);

            Undo.RecordObjects(allRenderers, "Batch Assign Curved World Materials");

            foreach (Renderer renderer in allRenderers)
            {
                Material[] materials = renderer.sharedMaterials;
                bool modified = false;

                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] == null) continue;

                    // Check if a curved world variant exists
                    string originalPath = AssetDatabase.GetAssetPath(materials[i]);
                    string curvedWorldPath = originalPath.Replace(".mat", CURVED_WORLD_SUFFIX + ".mat");

                    Material curvedWorldMaterial = AssetDatabase.LoadAssetAtPath<Material>(curvedWorldPath);

                    if (curvedWorldMaterial != null)
                    {
                        materials[i] = curvedWorldMaterial;
                        modified = true;
                        assignmentCount++;
                    }
                }

                if (modified)
                {
                    renderer.sharedMaterials = materials;
                }
            }

            EditorUtility.DisplayDialog("Batch Assignment Complete",
                $"Assigned {assignmentCount} curved world materials to renderers in the scene.",
                "OK");

            Debug.Log($"[MaterialConversion] Batch assignment complete: {assignmentCount} materials assigned");
        }

        #endregion

        #region Window Lifecycle

        private void OnEnable()
        {
            RefreshMaterialList();
        }

        private void OnGUI()
        {
            DrawHeader();
            DrawToolbar();
            DrawMaterialList();
            DrawFooter();
        }

        #endregion

        #region UI Drawing

        private void DrawHeader()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Material Conversion Automation", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "This tool creates curved world variants of Loft3D materials. " +
                "Materials will be duplicated with the '_CurvedWorld' suffix and optionally assigned the curved world shader.",
                MessageType.Info);
            EditorGUILayout.Space(5);
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("Refresh List", EditorStyles.toolbarButton, GUILayout.Width(100)))
            {
                RefreshMaterialList();
            }

            GUILayout.FlexibleSpace();

            includeSubdirectories = GUILayout.Toggle(includeSubdirectories, "Include Subdirectories", EditorStyles.toolbarButton);
            autoAssignShader = GUILayout.Toggle(autoAssignShader, "Auto-Assign Shader", EditorStyles.toolbarButton);
            showExcludedMaterials = GUILayout.Toggle(showExcludedMaterials, "Show Excluded", EditorStyles.toolbarButton);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);
        }

        private void DrawMaterialList()
        {
            // Summary
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Total Materials: {totalMaterials}", GUILayout.Width(150));
            EditorGUILayout.LabelField($"Already Converted: {convertedMaterials}", GUILayout.Width(150));
            EditorGUILayout.LabelField($"To Convert: {totalMaterials - convertedMaterials}");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // Material list header
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField("Material", EditorStyles.boldLabel, GUILayout.Width(200));
            EditorGUILayout.LabelField("Priority", EditorStyles.boldLabel, GUILayout.Width(80));
            EditorGUILayout.LabelField("Status", EditorStyles.boldLabel, GUILayout.Width(100));
            EditorGUILayout.LabelField("Actions", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            // Material list (scrollable)
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            foreach (var item in conversionItems)
            {
                // Skip excluded materials if not showing them
                if (item.isExcluded && !showExcludedMaterials)
                {
                    continue;
                }

                DrawMaterialItem(item);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawMaterialItem(MaterialConversionItem item)
        {
            EditorGUILayout.BeginHorizontal();

            // Material name with color coding
            Color originalColor = GUI.color;
            if (item.isExcluded)
            {
                GUI.color = Color.gray;
            }
            else if (item.isConverted)
            {
                GUI.color = Color.green;
            }

            EditorGUILayout.ObjectField(item.originalMaterial, typeof(Material), false, GUILayout.Width(200));
            GUI.color = originalColor;

            // Priority
            string priorityLabel = item.isExcluded ? "EXCLUDED" : item.priority;
            EditorGUILayout.LabelField(priorityLabel, GUILayout.Width(80));

            // Status
            string status = item.isExcluded ? "Excluded" : (item.isConverted ? "Converted" : "Pending");
            EditorGUILayout.LabelField(status, GUILayout.Width(100));

            // Actions
            GUI.enabled = !item.isExcluded && !item.isConverted;
            if (GUILayout.Button("Convert", GUILayout.Width(70)))
            {
                ConvertSingleMaterial(item);
            }
            GUI.enabled = true;

            if (item.isConverted && item.convertedMaterial != null)
            {
                if (GUILayout.Button("Select", GUILayout.Width(70)))
                {
                    Selection.activeObject = item.convertedMaterial;
                    EditorGUIUtility.PingObject(item.convertedMaterial);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawFooter()
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();

            // Batch convert button
            GUI.enabled = conversionItems.Any(item => !item.isExcluded && !item.isConverted);
            if (GUILayout.Button("Convert All Pending Materials", GUILayout.Height(30)))
            {
                ConvertAllMaterials();
            }
            GUI.enabled = true;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // Shader status
            bool curvedWorldShaderExists = ShaderHelperUtility.ShaderExists(CURVED_WORLD_SHADER_NAME);
            string shaderMessage = curvedWorldShaderExists
                ? $"Curved World Shader Found: {CURVED_WORLD_SHADER_NAME}"
                : $"Curved World Shader Not Found. Using fallback: {FALLBACK_SHADER_NAME}";

            MessageType shaderMessageType = curvedWorldShaderExists ? MessageType.Info : MessageType.Warning;
            EditorGUILayout.HelpBox(shaderMessage, shaderMessageType);
        }

        #endregion

        #region Material Operations

        private void RefreshMaterialList()
        {
            conversionItems.Clear();
            totalMaterials = 0;
            convertedMaterials = 0;

            // Find all materials
            List<Material> materials = ShaderHelperUtility.FindMaterialsInDirectory(LOFT3D_MATERIALS_PATH, includeSubdirectories);

            foreach (Material mat in materials)
            {
                // Skip materials that are already curved world variants
                if (mat.name.Contains(CURVED_WORLD_SUFFIX))
                {
                    continue;
                }

                MaterialConversionItem item = new MaterialConversionItem
                {
                    originalMaterial = mat,
                    isExcluded = ShaderHelperUtility.ShouldExcludeMaterial(mat),
                    priority = ShaderHelperUtility.GetMaterialPriority(mat)
                };

                // Check if already converted
                string originalPath = AssetDatabase.GetAssetPath(mat);
                string curvedWorldPath = originalPath.Replace(".mat", CURVED_WORLD_SUFFIX + ".mat");
                Material existingCurvedWorld = AssetDatabase.LoadAssetAtPath<Material>(curvedWorldPath);

                if (existingCurvedWorld != null)
                {
                    item.isConverted = true;
                    item.convertedMaterial = existingCurvedWorld;
                    convertedMaterials++;
                }

                conversionItems.Add(item);
                totalMaterials++;
            }

            // Sort by priority (CRITICAL > HIGH > MEDIUM > LOW)
            conversionItems = conversionItems.OrderBy(item =>
            {
                if (item.isExcluded) return 4;
                switch (item.priority)
                {
                    case "CRITICAL": return 0;
                    case "HIGH": return 1;
                    case "MEDIUM": return 2;
                    default: return 3;
                }
            }).ToList();

            Debug.Log($"[MaterialConversion] Refreshed material list: {totalMaterials} materials found, {convertedMaterials} already converted");
        }

        private void ConvertSingleMaterial(MaterialConversionItem item)
        {
            if (item.isExcluded || item.isConverted)
            {
                return;
            }

            Undo.RecordObject(item.originalMaterial, "Convert Material to Curved World");

            // Duplicate the material
            Material duplicatedMaterial = ShaderHelperUtility.DuplicateMaterial(
                item.originalMaterial,
                CURVED_WORLD_SUFFIX
            );

            if (duplicatedMaterial == null)
            {
                Debug.LogError($"[MaterialConversion] Failed to duplicate material: {item.originalMaterial.name}");
                return;
            }

            // Assign shader if enabled
            if (autoAssignShader)
            {
                bool shaderAssigned = ShaderHelperUtility.AssignShaderToMaterial(
                    duplicatedMaterial,
                    CURVED_WORLD_SHADER_NAME,
                    FALLBACK_SHADER_NAME
                );

                if (!shaderAssigned)
                {
                    Debug.LogWarning($"[MaterialConversion] Could not assign shader to: {duplicatedMaterial.name}");
                }
            }

            // Update item status
            item.isConverted = true;
            item.convertedMaterial = duplicatedMaterial;
            convertedMaterials++;

            Debug.Log($"[MaterialConversion] Successfully converted: {item.originalMaterial.name} -> {duplicatedMaterial.name} ({item.priority} priority)");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void ConvertAllMaterials()
        {
            if (!EditorUtility.DisplayDialog("Convert All Materials",
                $"This will create curved world variants for {totalMaterials - convertedMaterials} materials.\n\nExcluded materials (particles, VFX, UI) will be skipped.",
                "Proceed", "Cancel"))
            {
                return;
            }

            int converted = 0;
            int failed = 0;

            foreach (var item in conversionItems)
            {
                if (item.isExcluded || item.isConverted)
                {
                    continue;
                }

                try
                {
                    ConvertSingleMaterial(item);
                    converted++;
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"[MaterialConversion] Error converting {item.originalMaterial.name}: {ex.Message}");
                    failed++;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Conversion Complete",
                $"Successfully converted {converted} materials.\n" +
                $"Failed: {failed}\n" +
                $"Excluded: {conversionItems.Count(item => item.isExcluded)}",
                "OK");

            Debug.Log($"[MaterialConversion] Batch conversion complete: {converted} converted, {failed} failed");
        }

        #endregion

        #region Helper Classes

        private class MaterialConversionItem
        {
            public Material originalMaterial;
            public Material convertedMaterial;
            public bool isExcluded;
            public bool isConverted;
            public string priority;
        }

        #endregion
    }

    #region Material Validation Tools

    /// <summary>
    /// Additional utility menu items for material validation and cleanup.
    /// </summary>
    public static class MaterialValidationTools
    {
        [MenuItem("Tools/Galactic Crossing/Validate Scene Materials")]
        public static void ValidateSceneMaterials()
        {
            Renderer[] allRenderers = ShaderHelperUtility.FindAllRenderers(true);
            int totalRenderers = allRenderers.Length;
            int curvedWorldCount = 0;
            int standardCount = 0;
            int missingCount = 0;

            foreach (Renderer renderer in allRenderers)
            {
                foreach (Material mat in renderer.sharedMaterials)
                {
                    if (mat == null)
                    {
                        missingCount++;
                    }
                    else if (mat.name.Contains("_CurvedWorld"))
                    {
                        curvedWorldCount++;
                    }
                    else
                    {
                        standardCount++;
                    }
                }
            }

            string report = $"Scene Material Validation Report:\n\n" +
                           $"Total Renderers: {totalRenderers}\n" +
                           $"Curved World Materials: {curvedWorldCount}\n" +
                           $"Standard Materials: {standardCount}\n" +
                           $"Missing Materials: {missingCount}\n\n" +
                           $"Recommendation: ";

            if (standardCount > 0)
            {
                report += $"Use 'Batch Assign Materials to Scene' to convert {standardCount} standard materials.";
            }
            else
            {
                report += "All materials are using curved world variants!";
            }

            EditorUtility.DisplayDialog("Material Validation", report, "OK");
            Debug.Log($"[MaterialValidation] {report}");
        }

        [MenuItem("Tools/Galactic Crossing/List All Material Conversions")]
        public static void ListAllMaterialConversions()
        {
            List<Material> allMaterials = ShaderHelperUtility.FindMaterialsInDirectory(
                "Assets/TopDownEngine/Demos/Loft3D/Materials", true);

            var curvedWorldMaterials = allMaterials.Where(m => m.name.Contains("_CurvedWorld")).ToList();
            var standardMaterials = allMaterials.Where(m => !m.name.Contains("_CurvedWorld")).ToList();

            Debug.Log($"[MaterialConversion] Material Conversion Summary:");
            Debug.Log($"  Total Materials: {allMaterials.Count}");
            Debug.Log($"  Curved World Variants: {curvedWorldMaterials.Count}");
            Debug.Log($"  Standard Materials: {standardMaterials.Count}");

            Debug.Log("\nCurved World Materials:");
            foreach (var mat in curvedWorldMaterials)
            {
                Debug.Log($"  - {mat.name} (Shader: {mat.shader.name})");
            }

            Debug.Log("\nStandard Materials:");
            foreach (var mat in standardMaterials)
            {
                bool excluded = ShaderHelperUtility.ShouldExcludeMaterial(mat);
                string priority = ShaderHelperUtility.GetMaterialPriority(mat);
                Debug.Log($"  - {mat.name} [{priority}] {(excluded ? "(EXCLUDED)" : "")}");
            }
        }
    }

    #endregion
}
