using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace GalacticCrossing.Editor
{
    /// <summary>
    /// Utility methods for safely adding, removing, and configuring components.
    /// Provides helper functions for common Editor operations.
    /// </summary>
    public static class ComponentConfigurationHelper
    {
        #region Component Addition/Removal

        /// <summary>
        /// Safely adds a component to a GameObject if it doesn't already exist
        /// </summary>
        /// <typeparam name="T">Component type to add</typeparam>
        /// <param name="gameObject">Target GameObject</param>
        /// <returns>The component (existing or newly added)</returns>
        public static T AddComponentSafe<T>(GameObject gameObject) where T : Component
        {
            if (gameObject == null)
            {
                Debug.LogError("[ComponentHelper] Cannot add component to null GameObject");
                return null;
            }

            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
                Debug.Log($"[ComponentHelper] Added {typeof(T).Name} to {gameObject.name}");
                EditorUtility.SetDirty(gameObject);
            }
            else
            {
                Debug.Log($"[ComponentHelper] {typeof(T).Name} already exists on {gameObject.name}");
            }

            return component;
        }

        /// <summary>
        /// Safely removes a component from a GameObject if it exists
        /// </summary>
        /// <typeparam name="T">Component type to remove</typeparam>
        /// <param name="gameObject">Target GameObject</param>
        /// <returns>True if component was removed, false if it didn't exist</returns>
        public static bool RemoveComponentSafe<T>(GameObject gameObject) where T : Component
        {
            if (gameObject == null)
            {
                Debug.LogError("[ComponentHelper] Cannot remove component from null GameObject");
                return false;
            }

            T component = gameObject.GetComponent<T>();
            if (component != null)
            {
                UnityEngine.Object.DestroyImmediate(component);
                Debug.Log($"[ComponentHelper] Removed {typeof(T).Name} from {gameObject.name}");
                EditorUtility.SetDirty(gameObject);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes all components of a specific type from a GameObject
        /// </summary>
        /// <typeparam name="T">Component type to remove</typeparam>
        /// <param name="gameObject">Target GameObject</param>
        /// <returns>Number of components removed</returns>
        public static int RemoveAllComponents<T>(GameObject gameObject) where T : Component
        {
            if (gameObject == null)
            {
                Debug.LogError("[ComponentHelper] Cannot remove components from null GameObject");
                return 0;
            }

            T[] components = gameObject.GetComponents<T>();
            int count = 0;

            foreach (var component in components)
            {
                UnityEngine.Object.DestroyImmediate(component);
                count++;
            }

            if (count > 0)
            {
                Debug.Log($"[ComponentHelper] Removed {count} {typeof(T).Name} components from {gameObject.name}");
                EditorUtility.SetDirty(gameObject);
            }

            return count;
        }

        #endregion

        #region Component Configuration

        /// <summary>
        /// Sets a field value on a component using reflection
        /// </summary>
        /// <param name="component">Target component</param>
        /// <param name="fieldName">Name of the field to set</param>
        /// <param name="value">Value to set</param>
        /// <returns>True if successful, false otherwise</returns>
        public static bool SetComponentField(Component component, string fieldName, object value)
        {
            if (component == null)
            {
                Debug.LogError("[ComponentHelper] Cannot set field on null component");
                return false;
            }

            Type componentType = component.GetType();
            FieldInfo field = componentType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (field == null)
            {
                Debug.LogWarning($"[ComponentHelper] Field '{fieldName}' not found on {componentType.Name}");
                return false;
            }

            try
            {
                field.SetValue(component, value);
                EditorUtility.SetDirty(component);
                Debug.Log($"[ComponentHelper] Set {componentType.Name}.{fieldName} = {value}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[ComponentHelper] Failed to set field: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets a field value from a component using reflection
        /// </summary>
        /// <param name="component">Target component</param>
        /// <param name="fieldName">Name of the field to get</param>
        /// <returns>The field value, or null if not found</returns>
        public static object GetComponentField(Component component, string fieldName)
        {
            if (component == null)
            {
                Debug.LogError("[ComponentHelper] Cannot get field from null component");
                return null;
            }

            Type componentType = component.GetType();
            FieldInfo field = componentType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (field == null)
            {
                Debug.LogWarning($"[ComponentHelper] Field '{fieldName}' not found on {componentType.Name}");
                return null;
            }

            return field.GetValue(component);
        }

        #endregion

        #region Collider Configuration

        /// <summary>
        /// Configures a collider as a trigger with specified parameters
        /// </summary>
        /// <param name="gameObject">GameObject to configure collider on</param>
        /// <param name="isTrigger">Whether the collider should be a trigger</param>
        /// <param name="size">Size of the collider (for BoxCollider)</param>
        /// <param name="center">Center offset of the collider</param>
        /// <returns>The configured collider</returns>
        public static Collider ConfigureBoxCollider(GameObject gameObject, bool isTrigger = true, Vector3? size = null, Vector3? center = null)
        {
            if (gameObject == null)
            {
                Debug.LogError("[ComponentHelper] Cannot configure collider on null GameObject");
                return null;
            }

            var boxCollider = gameObject.GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                boxCollider = gameObject.AddComponent<BoxCollider>();
            }

            boxCollider.isTrigger = isTrigger;

            if (size.HasValue)
            {
                boxCollider.size = size.Value;
            }

            if (center.HasValue)
            {
                boxCollider.center = center.Value;
            }

            EditorUtility.SetDirty(gameObject);
            Debug.Log($"[ComponentHelper] Configured BoxCollider on {gameObject.name}");

            return boxCollider;
        }

        /// <summary>
        /// Configures a sphere collider with specified parameters
        /// </summary>
        /// <param name="gameObject">GameObject to configure collider on</param>
        /// <param name="isTrigger">Whether the collider should be a trigger</param>
        /// <param name="radius">Radius of the sphere collider</param>
        /// <param name="center">Center offset of the collider</param>
        /// <returns>The configured collider</returns>
        public static SphereCollider ConfigureSphereCollider(GameObject gameObject, bool isTrigger = true, float radius = 1f, Vector3? center = null)
        {
            if (gameObject == null)
            {
                Debug.LogError("[ComponentHelper] Cannot configure collider on null GameObject");
                return null;
            }

            var sphereCollider = gameObject.GetComponent<SphereCollider>();
            if (sphereCollider == null)
            {
                sphereCollider = gameObject.AddComponent<SphereCollider>();
            }

            sphereCollider.isTrigger = isTrigger;
            sphereCollider.radius = radius;

            if (center.HasValue)
            {
                sphereCollider.center = center.Value;
            }

            EditorUtility.SetDirty(gameObject);
            Debug.Log($"[ComponentHelper] Configured SphereCollider on {gameObject.name}");

            return sphereCollider;
        }

        #endregion

        #region Reference Assignment

        /// <summary>
        /// Assigns a reference from one object to another using reflection
        /// </summary>
        /// <param name="sourceComponent">Component that needs the reference</param>
        /// <param name="fieldName">Name of the field to assign to</param>
        /// <param name="targetObject">Object to assign as reference</param>
        /// <returns>True if successful</returns>
        public static bool AssignReference(Component sourceComponent, string fieldName, UnityEngine.Object targetObject)
        {
            return SetComponentField(sourceComponent, fieldName, targetObject);
        }

        /// <summary>
        /// Finds and assigns a component reference by type
        /// </summary>
        /// <typeparam name="T">Type of component to find and assign</typeparam>
        /// <param name="sourceComponent">Component that needs the reference</param>
        /// <param name="fieldName">Name of the field to assign to</param>
        /// <param name="searchInChildren">Whether to search in children</param>
        /// <returns>True if successful</returns>
        public static bool FindAndAssignReference<T>(Component sourceComponent, string fieldName, bool searchInChildren = false) where T : Component
        {
            if (sourceComponent == null)
            {
                Debug.LogError("[ComponentHelper] Cannot assign reference on null component");
                return false;
            }

            T target;
            if (searchInChildren)
            {
                target = sourceComponent.GetComponentInChildren<T>();
            }
            else
            {
                target = sourceComponent.GetComponent<T>();
            }

            if (target == null)
            {
                Debug.LogWarning($"[ComponentHelper] Could not find {typeof(T).Name} to assign");
                return false;
            }

            return SetComponentField(sourceComponent, fieldName, target);
        }

        #endregion

        #region Layer and Tag Configuration

        /// <summary>
        /// Sets the layer of a GameObject and optionally all its children
        /// </summary>
        /// <param name="gameObject">Target GameObject</param>
        /// <param name="layerName">Name of the layer</param>
        /// <param name="includeChildren">Whether to set children's layers too</param>
        public static void SetLayer(GameObject gameObject, string layerName, bool includeChildren = false)
        {
            if (gameObject == null)
            {
                Debug.LogError("[ComponentHelper] Cannot set layer on null GameObject");
                return;
            }

            int layer = LayerMask.NameToLayer(layerName);
            if (layer == -1)
            {
                Debug.LogWarning($"[ComponentHelper] Layer '{layerName}' does not exist");
                return;
            }

            gameObject.layer = layer;

            if (includeChildren)
            {
                foreach (Transform child in gameObject.transform)
                {
                    SetLayer(child.gameObject, layerName, true);
                }
            }

            EditorUtility.SetDirty(gameObject);
            Debug.Log($"[ComponentHelper] Set layer '{layerName}' on {gameObject.name}" +
                     (includeChildren ? " and children" : ""));
        }

        /// <summary>
        /// Sets the tag of a GameObject
        /// </summary>
        /// <param name="gameObject">Target GameObject</param>
        /// <param name="tag">Tag to set</param>
        public static void SetTag(GameObject gameObject, string tag)
        {
            if (gameObject == null)
            {
                Debug.LogError("[ComponentHelper] Cannot set tag on null GameObject");
                return;
            }

            try
            {
                gameObject.tag = tag;
                EditorUtility.SetDirty(gameObject);
                Debug.Log($"[ComponentHelper] Set tag '{tag}' on {gameObject.name}");
            }
            catch (UnityException e)
            {
                Debug.LogWarning($"[ComponentHelper] Tag '{tag}' does not exist: {e.Message}");
            }
        }

        #endregion

        #region Prefab Utilities

        /// <summary>
        /// Creates a prefab variant from a GameObject
        /// </summary>
        /// <param name="gameObject">Source GameObject</param>
        /// <param name="path">Path to save the prefab</param>
        /// <returns>The created prefab</returns>
        public static GameObject CreatePrefabVariant(GameObject gameObject, string path)
        {
            if (gameObject == null)
            {
                Debug.LogError("[ComponentHelper] Cannot create prefab from null GameObject");
                return null;
            }

            // Ensure path ends with .prefab
            if (!path.EndsWith(".prefab"))
            {
                path += ".prefab";
            }

            // Check if prefab already exists
            if (AssetDatabase.LoadAssetAtPath<GameObject>(path) != null)
            {
                if (!EditorUtility.DisplayDialog("Prefab Exists",
                    $"A prefab already exists at:\n{path}\n\nOverwrite it?",
                    "Yes", "No"))
                {
                    return null;
                }
            }

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(gameObject, path);
            Debug.Log($"[ComponentHelper] Created prefab at {path}");

            return prefab;
        }

        /// <summary>
        /// Instantiates a prefab in the scene
        /// </summary>
        /// <param name="prefabPath">Path to the prefab asset</param>
        /// <param name="position">Position to instantiate at</param>
        /// <param name="parent">Parent transform (optional)</param>
        /// <returns>The instantiated GameObject</returns>
        public static GameObject InstantiatePrefab(string prefabPath, Vector3 position, Transform parent = null)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.LogError($"[ComponentHelper] Prefab not found at {prefabPath}");
                return null;
            }

            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (instance != null)
            {
                instance.transform.position = position;
                if (parent != null)
                {
                    instance.transform.SetParent(parent);
                }
                Debug.Log($"[ComponentHelper] Instantiated prefab {prefab.name} at {position}");
            }

            return instance;
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validates that all required components exist on a GameObject
        /// </summary>
        /// <param name="gameObject">GameObject to validate</param>
        /// <param name="requiredComponents">Array of required component types</param>
        /// <returns>True if all components exist</returns>
        public static bool ValidateComponents(GameObject gameObject, params Type[] requiredComponents)
        {
            if (gameObject == null)
            {
                Debug.LogError("[ComponentHelper] Cannot validate null GameObject");
                return false;
            }

            bool allValid = true;

            foreach (Type componentType in requiredComponents)
            {
                if (gameObject.GetComponent(componentType) == null)
                {
                    Debug.LogWarning($"[ComponentHelper] Missing required component {componentType.Name} on {gameObject.name}");
                    allValid = false;
                }
            }

            return allValid;
        }

        #endregion

        #region Utility

        /// <summary>
        /// Logs the component structure of a GameObject
        /// </summary>
        /// <param name="gameObject">GameObject to log</param>
        public static void LogComponentStructure(GameObject gameObject)
        {
            if (gameObject == null)
            {
                Debug.LogError("[ComponentHelper] Cannot log structure of null GameObject");
                return;
            }

            Debug.Log($"=== Component Structure for {gameObject.name} ===");

            Component[] components = gameObject.GetComponents<Component>();
            foreach (var component in components)
            {
                if (component != null)
                {
                    Debug.Log($"  - {component.GetType().Name}");
                }
            }

            Debug.Log("=== End Component Structure ===");
        }

        #endregion
    }
}
