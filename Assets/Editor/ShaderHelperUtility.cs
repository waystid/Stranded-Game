using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace GalacticCrossing.Editor
{
    /// <summary>
    /// Utility class providing helper methods for material and shader operations.
    /// Used by MaterialConversionAutomation to perform batch material operations.
    /// </summary>
    public static class ShaderHelperUtility
    {
        #region Material Discovery

        /// <summary>
        /// Finds all materials in the specified directory and optionally its subdirectories.
        /// </summary>
        /// <param name="directoryPath">The directory path relative to Assets (e.g., "TopDownEngine/Demos/Loft3D/Materials")</param>
        /// <param name="includeSubdirectories">Whether to search subdirectories recursively</param>
        /// <returns>List of Material assets found</returns>
        public static List<Material> FindMaterialsInDirectory(string directoryPath, bool includeSubdirectories = true)
        {
            List<Material> materials = new List<Material>();

            // Ensure path starts with Assets/
            if (!directoryPath.StartsWith("Assets/"))
            {
                directoryPath = "Assets/" + directoryPath;
            }

            // Check if directory exists
            if (!AssetDatabase.IsValidFolder(directoryPath))
            {
                Debug.LogError($"[ShaderHelper] Directory not found: {directoryPath}");
                return materials;
            }

            // Find all material GUIDs in the directory
            string searchPattern = includeSubdirectories ? "t:Material" : "t:Material";
            string[] materialGUIDs = AssetDatabase.FindAssets(searchPattern, new[] { directoryPath });

            Debug.Log($"[ShaderHelper] Found {materialGUIDs.Length} materials in {directoryPath}");

            foreach (string guid in materialGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                // If not including subdirectories, filter out paths with additional slashes
                if (!includeSubdirectories)
                {
                    string relativePath = assetPath.Substring(directoryPath.Length + 1);
                    if (relativePath.Contains("/"))
                    {
                        continue; // Skip files in subdirectories
                    }
                }

                Material mat = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
                if (mat != null)
                {
                    materials.Add(mat);
                }
            }

            return materials;
        }

        #endregion

        #region Material Operations

        /// <summary>
        /// Creates a duplicate of a material with a new name suffix.
        /// </summary>
        /// <param name="originalMaterial">The material to duplicate</param>
        /// <param name="suffix">Suffix to append to the material name (e.g., "_CurvedWorld")</param>
        /// <param name="targetDirectory">Optional: Directory to save the duplicate. If null, saves in same directory as original</param>
        /// <returns>The duplicated Material, or null if duplication failed</returns>
        public static Material DuplicateMaterial(Material originalMaterial, string suffix, string targetDirectory = null)
        {
            if (originalMaterial == null)
            {
                Debug.LogError("[ShaderHelper] Cannot duplicate null material");
                return null;
            }

            // Get the original material path
            string originalPath = AssetDatabase.GetAssetPath(originalMaterial);
            if (string.IsNullOrEmpty(originalPath))
            {
                Debug.LogError($"[ShaderHelper] Could not find asset path for material: {originalMaterial.name}");
                return null;
            }

            // Construct new path
            string directory = targetDirectory ?? Path.GetDirectoryName(originalPath);
            string fileName = Path.GetFileNameWithoutExtension(originalPath);
            string extension = Path.GetExtension(originalPath);
            string newPath = Path.Combine(directory, fileName + suffix + extension).Replace("\\", "/");

            // Check if material already exists
            if (File.Exists(newPath))
            {
                Debug.LogWarning($"[ShaderHelper] Material already exists, skipping: {newPath}");
                return AssetDatabase.LoadAssetAtPath<Material>(newPath);
            }

            // Use AssetDatabase to copy the asset
            bool copySuccess = AssetDatabase.CopyAsset(originalPath, newPath);

            if (!copySuccess)
            {
                Debug.LogError($"[ShaderHelper] Failed to copy material from {originalPath} to {newPath}");
                return null;
            }

            AssetDatabase.Refresh();

            // Load and return the new material
            Material duplicatedMaterial = AssetDatabase.LoadAssetAtPath<Material>(newPath);

            if (duplicatedMaterial != null)
            {
                Debug.Log($"[ShaderHelper] Successfully duplicated: {originalMaterial.name} -> {duplicatedMaterial.name}");
            }

            return duplicatedMaterial;
        }

        /// <summary>
        /// Assigns a material to a GameObject's Renderer component.
        /// Supports MeshRenderer, SkinnedMeshRenderer, and other Renderer types.
        /// </summary>
        /// <param name="gameObject">The GameObject to modify</param>
        /// <param name="material">The material to assign</param>
        /// <param name="materialIndex">The material index to replace (default 0 for single-material objects)</param>
        /// <returns>True if assignment was successful</returns>
        public static bool AssignMaterialToGameObject(GameObject gameObject, Material material, int materialIndex = 0)
        {
            if (gameObject == null)
            {
                Debug.LogError("[ShaderHelper] Cannot assign material to null GameObject");
                return false;
            }

            if (material == null)
            {
                Debug.LogError("[ShaderHelper] Cannot assign null material");
                return false;
            }

            Renderer renderer = gameObject.GetComponent<Renderer>();

            if (renderer == null)
            {
                Debug.LogWarning($"[ShaderHelper] GameObject '{gameObject.name}' has no Renderer component");
                return false;
            }

            // Check if the material index is valid
            if (materialIndex < 0 || materialIndex >= renderer.sharedMaterials.Length)
            {
                Debug.LogError($"[ShaderHelper] Material index {materialIndex} is out of range for GameObject '{gameObject.name}' (has {renderer.sharedMaterials.Length} materials)");
                return false;
            }

            // Create a copy of the materials array
            Material[] materials = renderer.sharedMaterials;
            materials[materialIndex] = material;
            renderer.sharedMaterials = materials;

            Debug.Log($"[ShaderHelper] Assigned material '{material.name}' to '{gameObject.name}' at index {materialIndex}");
            return true;
        }

        #endregion

        #region Scene Operations

        /// <summary>
        /// Finds all MeshRenderers in the current scene(s).
        /// </summary>
        /// <param name="includeInactive">Whether to include inactive GameObjects</param>
        /// <returns>Array of all MeshRenderer components found</returns>
        public static MeshRenderer[] FindAllMeshRenderers(bool includeInactive = true)
        {
            return GameObject.FindObjectsByType<MeshRenderer>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        }

        /// <summary>
        /// Finds all SkinnedMeshRenderers in the current scene(s).
        /// </summary>
        /// <param name="includeInactive">Whether to include inactive GameObjects</param>
        /// <returns>Array of all SkinnedMeshRenderer components found</returns>
        public static SkinnedMeshRenderer[] FindAllSkinnedMeshRenderers(bool includeInactive = true)
        {
            return GameObject.FindObjectsByType<SkinnedMeshRenderer>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        }

        /// <summary>
        /// Finds all Renderer components in the current scene(s).
        /// This includes MeshRenderer, SkinnedMeshRenderer, and other Renderer types.
        /// </summary>
        /// <param name="includeInactive">Whether to include inactive GameObjects</param>
        /// <returns>Array of all Renderer components found</returns>
        public static Renderer[] FindAllRenderers(bool includeInactive = true)
        {
            return GameObject.FindObjectsByType<Renderer>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validates that a material assignment won't break prefab connections.
        /// </summary>
        /// <param name="gameObject">The GameObject to check</param>
        /// <param name="warnIfPrefab">Whether to log a warning if the object is part of a prefab</param>
        /// <returns>True if safe to proceed, false if the operation might break prefab links</returns>
        public static bool ValidateMaterialAssignment(GameObject gameObject, bool warnIfPrefab = true)
        {
            if (gameObject == null)
            {
                return false;
            }

            // Check if this is a prefab instance
            PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(gameObject);
            PrefabInstanceStatus prefabStatus = PrefabUtility.GetPrefabInstanceStatus(gameObject);

            bool isPrefabInstance = prefabStatus == PrefabInstanceStatus.Connected;

            if (isPrefabInstance && warnIfPrefab)
            {
                Debug.LogWarning($"[ShaderHelper] GameObject '{gameObject.name}' is part of a prefab instance. " +
                                 "Material assignment will create a prefab override. Use 'Apply' to push changes to prefab if desired.");
            }

            // Always return true - we allow prefab overrides, just warn the user
            return true;
        }

        /// <summary>
        /// Checks if a shader exists in the project.
        /// </summary>
        /// <param name="shaderName">The name of the shader (e.g., "Shader Graphs/CurvedWorldLit")</param>
        /// <returns>True if the shader exists and can be found</returns>
        public static bool ShaderExists(string shaderName)
        {
            Shader shader = Shader.Find(shaderName);
            return shader != null;
        }

        /// <summary>
        /// Attempts to assign a shader to a material with fallback behavior.
        /// </summary>
        /// <param name="material">The material to modify</param>
        /// <param name="shaderName">The desired shader name</param>
        /// <param name="fallbackShaderName">Fallback shader if the primary is not found (optional)</param>
        /// <returns>True if shader was assigned (either primary or fallback)</returns>
        public static bool AssignShaderToMaterial(Material material, string shaderName, string fallbackShaderName = null)
        {
            if (material == null)
            {
                Debug.LogError("[ShaderHelper] Cannot assign shader to null material");
                return false;
            }

            // Try primary shader
            Shader shader = Shader.Find(shaderName);
            if (shader != null)
            {
                material.shader = shader;
                Debug.Log($"[ShaderHelper] Assigned shader '{shaderName}' to material '{material.name}'");
                return true;
            }

            Debug.LogWarning($"[ShaderHelper] Shader '{shaderName}' not found for material '{material.name}'");

            // Try fallback shader if provided
            if (!string.IsNullOrEmpty(fallbackShaderName))
            {
                Shader fallbackShader = Shader.Find(fallbackShaderName);
                if (fallbackShader != null)
                {
                    material.shader = fallbackShader;
                    Debug.LogWarning($"[ShaderHelper] Using fallback shader '{fallbackShaderName}' for material '{material.name}'");
                    return true;
                }
                else
                {
                    Debug.LogError($"[ShaderHelper] Fallback shader '{fallbackShaderName}' also not found!");
                }
            }

            return false;
        }

        #endregion

        #region Material Classification

        /// <summary>
        /// Determines if a material should be excluded from curved world conversion based on naming conventions.
        /// Excludes particle systems, VFX, and UI materials.
        /// </summary>
        /// <param name="material">The material to check</param>
        /// <returns>True if the material should be excluded from conversion</returns>
        public static bool ShouldExcludeMaterial(Material material)
        {
            if (material == null) return true;

            string materialName = material.name.ToLower();

            // Exclusion keywords based on the plan documentation
            string[] exclusionKeywords = new string[]
            {
                "particle",
                "explosion",
                "smoke",
                "reticle",
                "ui",
                "blood",
                "click",
                "walk",
                "remnant",
                "vfx",
                "effect"
            };

            foreach (string keyword in exclusionKeywords)
            {
                if (materialName.Contains(keyword))
                {
                    Debug.Log($"[ShaderHelper] Excluding material '{material.name}' (contains keyword: {keyword})");
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Categorizes materials by priority based on the game design document.
        /// </summary>
        /// <param name="material">The material to categorize</param>
        /// <returns>Priority level: CRITICAL, HIGH, MEDIUM, or LOW</returns>
        public static string GetMaterialPriority(Material material)
        {
            if (material == null) return "LOW";

            string materialName = material.name.ToLower();

            // CRITICAL - Ground materials
            if (materialName.Contains("ground"))
            {
                return "CRITICAL";
            }

            // HIGH - Wall materials
            if (materialName.Contains("wall"))
            {
                return "HIGH";
            }

            // MEDIUM - Furniture and character materials
            if (materialName.Contains("furniture") || materialName.Contains("couch") ||
                materialName.Contains("lamp") || materialName.Contains("metal") ||
                materialName.Contains("plant") || materialName.Contains("screen") ||
                materialName.Contains("wood"))
            {
                return "MEDIUM";
            }

            // Default to LOW priority
            return "LOW";
        }

        #endregion
    }
}
