using UnityEngine;
using UnityEditor;
using MoreMountains.TopDownEngine;
using System.IO;

/// <summary>
/// Editor script to programmatically build the Tree prefab for Galactic Crossing
/// Creates an interactable tree with shake animation and loot dropping
/// </summary>
public class TreePrefabBuilder : MonoBehaviour
{
    private const string PREFAB_PATH = "Assets/Prefabs/";

    [MenuItem("Tools/Galactic Crossing/Build Tree Prefab")]
    public static void BuildTreePrefab()
    {
        Debug.Log("=== Starting Tree Prefab Build Process ===");

        // Ensure directories exist
        EnsureDirectoryExists(PREFAB_PATH);

        // Create root GameObject
        GameObject treePrefab = new GameObject("TreePrefab");

        // 1. Create TreeModel (placeholder cylinder for trunk)
        GameObject treeModel = CreateTreeModel(treePrefab.transform);

        // 2. Create TreeCollider (CapsuleCollider, non-trigger)
        CapsuleCollider treeCollider = treePrefab.AddComponent<CapsuleCollider>();
        treeCollider.isTrigger = false;
        treeCollider.radius = 0.5f;
        treeCollider.height = 3.5f;
        treeCollider.center = new Vector3(0f, 1.75f, 0f);
        treeCollider.direction = 1; // Y-axis
        Debug.Log("Added CapsuleCollider to TreePrefab (non-trigger)");

        // 3. Create InteractionZone child object
        GameObject interactionZone = new GameObject("InteractionZone");
        interactionZone.transform.SetParent(treePrefab.transform);
        interactionZone.transform.localPosition = Vector3.zero;

        // Add BoxCollider (trigger) to InteractionZone
        BoxCollider interactionCollider = interactionZone.AddComponent<BoxCollider>();
        interactionCollider.isTrigger = true;
        interactionCollider.size = new Vector3(4f, 4f, 4f); // 2m radius = 4m box
        interactionCollider.center = new Vector3(0f, 1.5f, 0f);
        Debug.Log("Added BoxCollider (trigger) to InteractionZone");

        // Add TreeShakeZone component to InteractionZone
        TreeShakeZone shakeZone = interactionZone.AddComponent<TreeShakeZone>();

        // 4. Create LootSpawnPoint child object
        GameObject lootSpawnPoint = new GameObject("LootSpawnPoint");
        lootSpawnPoint.transform.SetParent(treePrefab.transform);
        lootSpawnPoint.transform.localPosition = new Vector3(0f, 2f, 0f);

        // Add Loot component to LootSpawnPoint
        Loot lootComponent = lootSpawnPoint.AddComponent<Loot>();

        // Configure Loot component using SerializedObject
        SerializedObject serializedLoot = new SerializedObject(lootComponent);

        serializedLoot.FindProperty("LootMode").enumValueIndex = (int)Loot.LootModes.LootTable;
        serializedLoot.FindProperty("SpawnLootOnDeath").boolValue = false;
        serializedLoot.FindProperty("SpawnLootOnDamage").boolValue = false;
        serializedLoot.FindProperty("CanSpawn").boolValue = true;
        serializedLoot.FindProperty("Delay").floatValue = 0f;

        // Set Quantity (Min 1, Max 3)
        SerializedProperty quantityProp = serializedLoot.FindProperty("Quantity");
        quantityProp.vector2Value = new Vector2(1, 3);

        // Set SpawnProperties for spawn radius
        SerializedProperty spawnProps = serializedLoot.FindProperty("SpawnProperties");
        if (spawnProps != null)
        {
            SerializedProperty mode = spawnProps.FindPropertyRelative("Mode");
            if (mode != null)
            {
                // MMSpawnAroundProperties.Mode.Circle = 1
                mode.enumValueIndex = 1;
            }

            SerializedProperty radius = spawnProps.FindPropertyRelative("Radius");
            if (radius != null)
            {
                radius.floatValue = 1.5f;
            }
        }

        serializedLoot.FindProperty("LimitedLootQuantity").boolValue = false;
        serializedLoot.FindProperty("AvoidObstacles").boolValue = true;
        serializedLoot.FindProperty("DimensionMode").enumValueIndex = (int)Loot.DimensionModes.ThreeD;

        serializedLoot.ApplyModifiedProperties();

        Debug.Log("Configured Loot component on LootSpawnPoint");

        // Configure TreeShakeZone component using SerializedObject
        SerializedObject serializedShakeZone = new SerializedObject(shakeZone);

        // Assign TreeModel reference
        serializedShakeZone.FindProperty("TreeModel").objectReferenceValue = treeModel.transform;

        // Configure shake parameters
        serializedShakeZone.FindProperty("ShakeDuration").floatValue = 0.5f;
        serializedShakeZone.FindProperty("ShakeIntensity").floatValue = 0.3f; // Adjusted to match spec (0.3 intensity)
        serializedShakeZone.FindProperty("ShakeCooldown").floatValue = 5.0f;

        // Assign Loot component reference
        serializedShakeZone.FindProperty("LootComponent").objectReferenceValue = lootComponent;

        // ButtonActivated base class properties
        serializedShakeZone.FindProperty("ButtonActivated").boolValue = true;
        serializedShakeZone.FindProperty("AutoActivation").boolValue = false;

        SerializedProperty promptTextProp = serializedShakeZone.FindProperty("PromptText");
        if (promptTextProp != null)
        {
            promptTextProp.stringValue = "Shake Tree";
        }

        serializedShakeZone.ApplyModifiedProperties();

        Debug.Log("Configured TreeShakeZone component on InteractionZone");

        // Save as prefab
        string prefabPath = PREFAB_PATH + "TreePrefab.prefab";
        PrefabUtility.SaveAsPrefabAsset(treePrefab, prefabPath);

        // Clean up scene object
        DestroyImmediate(treePrefab);

        // Refresh asset database
        AssetDatabase.Refresh();

        Debug.Log("TreePrefab created at: " + prefabPath);
        Debug.Log("=== Tree Prefab Build Complete ===");
    }

    /// <summary>
    /// Creates the visual tree model with trunk and canopy
    /// </summary>
    private static GameObject CreateTreeModel(Transform parent)
    {
        GameObject treeModel = new GameObject("TreeModel");
        treeModel.transform.SetParent(parent);
        treeModel.transform.localPosition = Vector3.zero;

        // Create trunk (cylinder)
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name = "Trunk";
        trunk.transform.SetParent(treeModel.transform);
        trunk.transform.localPosition = new Vector3(0f, 1.5f, 0f);
        trunk.transform.localScale = new Vector3(0.5f, 1.5f, 0.5f);

        // Remove collider (parent has the main collider)
        DestroyImmediate(trunk.GetComponent<Collider>());

        // Create bark material
        Material barkMaterial = new Material(Shader.Find("Standard"));
        barkMaterial.color = new Color(0.4f, 0.25f, 0.15f); // Brown
        barkMaterial.SetFloat("_Metallic", 0f);
        barkMaterial.SetFloat("_Glossiness", 0.2f);
        trunk.GetComponent<Renderer>().material = barkMaterial;

        // Create canopy (sphere)
        GameObject canopy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        canopy.name = "Canopy";
        canopy.transform.SetParent(treeModel.transform);
        canopy.transform.localPosition = new Vector3(0f, 3.5f, 0f);
        canopy.transform.localScale = new Vector3(2.5f, 2.0f, 2.5f);

        // Remove collider
        DestroyImmediate(canopy.GetComponent<Collider>());

        // Create foliage material (vibrant green)
        Material foliageMaterial = new Material(Shader.Find("Standard"));
        foliageMaterial.color = new Color(0.2f, 0.7f, 0.3f); // Green
        foliageMaterial.SetFloat("_Metallic", 0f);
        foliageMaterial.SetFloat("_Glossiness", 0.3f);
        canopy.GetComponent<Renderer>().material = foliageMaterial;

        Debug.Log("Created TreeModel with Trunk and Canopy");

        return treeModel;
    }

    /// <summary>
    /// Ensures the specified directory exists, creating it if necessary
    /// </summary>
    private static void EnsureDirectoryExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Debug.Log("Created directory: " + path);
        }
    }
}
