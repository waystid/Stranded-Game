using UnityEngine;
using UnityEditor;
using MoreMountains.InventoryEngine;
using System.IO;

/// <summary>
/// Editor script to programmatically build ItemPicker prefabs for Galactic Crossing
/// Creates ScrapMetalPicker, EnergyCrystalPicker, and AlienBerryPicker prefabs
/// </summary>
public class ItemPickerPrefabBuilder : MonoBehaviour
{
    private const string PREFAB_PATH = "Assets/Prefabs/ItemPickers/";

    [MenuItem("Tools/Galactic Crossing/Build ItemPicker Prefabs")]
    public static void BuildAllItemPickerPrefabs()
    {
        Debug.Log("=== Starting ItemPicker Prefab Build Process ===");

        // Ensure directories exist
        EnsureDirectoryExists(PREFAB_PATH);

        // Build each prefab
        BuildScrapMetalPicker();
        BuildEnergyCrystalPicker();
        BuildAlienBerryPicker();

        // Refresh the asset database to show new prefabs
        AssetDatabase.Refresh();

        Debug.Log("=== ItemPicker Prefab Build Complete ===");
    }

    /// <summary>
    /// Build ScrapMetal picker - collectable debris from the crash site
    /// </summary>
    private static void BuildScrapMetalPicker()
    {
        Debug.Log("Building ScrapMetalPicker...");

        // Create root GameObject
        GameObject pickerObject = new GameObject("ScrapMetalPicker");

        // Add BoxCollider (trigger)
        BoxCollider boxCollider = pickerObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector3(1f, 1f, 1f);

        // Add ItemPicker component
        ItemPicker itemPicker = pickerObject.AddComponent<ItemPicker>();

        // Configure ItemPicker properties using SerializedObject for proper field assignment
        SerializedObject serializedPicker = new SerializedObject(itemPicker);

        // Try to find the ScriptableObject item - if it doesn't exist, log a warning
        InventoryItem scrapMetalItem = AssetDatabase.LoadAssetAtPath<InventoryItem>("Assets/Resources/Items/ScrapMetal.asset");
        if (scrapMetalItem != null)
        {
            serializedPicker.FindProperty("Item").objectReferenceValue = scrapMetalItem;
        }
        else
        {
            Debug.LogWarning("ScrapMetal.asset not found. Please assign the Item ScriptableObject manually.");
        }

        serializedPicker.FindProperty("Quantity").intValue = 1;
        serializedPicker.FindProperty("RemainingQuantity").intValue = 1;
        serializedPicker.FindProperty("PickableIfInventoryIsFull").boolValue = false;
        serializedPicker.FindProperty("DisableObjectWhenDepleted").boolValue = true;
        serializedPicker.FindProperty("RequirePlayerTag").boolValue = true;

        serializedPicker.ApplyModifiedProperties();

        // Add placeholder visual (metal cube)
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.name = "Visual";
        visual.transform.SetParent(pickerObject.transform);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = new Vector3(0.5f, 0.3f, 0.8f);

        // Destroy the collider on the visual (parent has the trigger collider)
        DestroyImmediate(visual.GetComponent<Collider>());

        // Create a dark metallic material for the visual
        Material scrapMaterial = new Material(Shader.Find("Standard"));
        scrapMaterial.color = new Color(0.3f, 0.3f, 0.35f);
        scrapMaterial.SetFloat("_Metallic", 0.8f);
        scrapMaterial.SetFloat("_Glossiness", 0.3f);
        visual.GetComponent<Renderer>().material = scrapMaterial;

        // Save as prefab
        string prefabPath = PREFAB_PATH + "ScrapMetalPicker.prefab";
        PrefabUtility.SaveAsPrefabAsset(pickerObject, prefabPath);

        // Clean up the scene object
        DestroyImmediate(pickerObject);

        Debug.Log("ScrapMetalPicker created at: " + prefabPath);
    }

    /// <summary>
    /// Build EnergyCrystal picker - glowing collectables similar to ACNH fruit
    /// </summary>
    private static void BuildEnergyCrystalPicker()
    {
        Debug.Log("Building EnergyCrystalPicker...");

        // Create root GameObject
        GameObject pickerObject = new GameObject("EnergyCrystalPicker");

        // Add BoxCollider (trigger)
        BoxCollider boxCollider = pickerObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector3(1f, 1.5f, 1f);

        // Add ItemPicker component
        ItemPicker itemPicker = pickerObject.AddComponent<ItemPicker>();

        // Configure ItemPicker properties
        SerializedObject serializedPicker = new SerializedObject(itemPicker);

        // Try to find the ScriptableObject item
        InventoryItem energyCrystalItem = AssetDatabase.LoadAssetAtPath<InventoryItem>("Assets/Resources/Items/EnergyCrystal.asset");
        if (energyCrystalItem != null)
        {
            serializedPicker.FindProperty("Item").objectReferenceValue = energyCrystalItem;
        }
        else
        {
            Debug.LogWarning("EnergyCrystal.asset not found. Please assign the Item ScriptableObject manually.");
        }

        serializedPicker.FindProperty("Quantity").intValue = 1;
        serializedPicker.FindProperty("RemainingQuantity").intValue = 1;
        serializedPicker.FindProperty("PickableIfInventoryIsFull").boolValue = false;
        serializedPicker.FindProperty("DisableObjectWhenDepleted").boolValue = true;
        serializedPicker.FindProperty("RequirePlayerTag").boolValue = true;

        serializedPicker.ApplyModifiedProperties();

        // Add placeholder visual (glowing crystal sphere)
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visual.name = "Visual";
        visual.transform.SetParent(pickerObject.transform);
        visual.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        visual.transform.localScale = new Vector3(0.6f, 0.8f, 0.6f);

        // Destroy the collider on the visual
        DestroyImmediate(visual.GetComponent<Collider>());

        // Create a bright cyan/blue emissive material
        Material crystalMaterial = new Material(Shader.Find("Standard"));
        crystalMaterial.color = new Color(0.2f, 0.8f, 1f);
        crystalMaterial.SetFloat("_Metallic", 0.2f);
        crystalMaterial.SetFloat("_Glossiness", 0.9f);
        crystalMaterial.EnableKeyword("_EMISSION");
        crystalMaterial.SetColor("_EmissionColor", new Color(0.3f, 1.2f, 1.5f));
        visual.GetComponent<Renderer>().material = crystalMaterial;

        // Save as prefab
        string prefabPath = PREFAB_PATH + "EnergyCrystalPicker.prefab";
        PrefabUtility.SaveAsPrefabAsset(pickerObject, prefabPath);

        // Clean up
        DestroyImmediate(pickerObject);

        Debug.Log("EnergyCrystalPicker created at: " + prefabPath);
    }

    /// <summary>
    /// Build AlienBerry picker - consumable healing item
    /// </summary>
    private static void BuildAlienBerryPicker()
    {
        Debug.Log("Building AlienBerryPicker...");

        // Create root GameObject
        GameObject pickerObject = new GameObject("AlienBerryPicker");

        // Add BoxCollider (trigger)
        BoxCollider boxCollider = pickerObject.AddComponent<BoxCollider>();
        boxCollider.isTrigger = true;
        boxCollider.size = new Vector3(1f, 1f, 1f);

        // Add ItemPicker component
        ItemPicker itemPicker = pickerObject.AddComponent<ItemPicker>();

        // Configure ItemPicker properties
        SerializedObject serializedPicker = new SerializedObject(itemPicker);

        // Try to find the ScriptableObject item
        InventoryItem alienBerryItem = AssetDatabase.LoadAssetAtPath<InventoryItem>("Assets/Resources/Items/AlienBerry.asset");
        if (alienBerryItem != null)
        {
            serializedPicker.FindProperty("Item").objectReferenceValue = alienBerryItem;
        }
        else
        {
            Debug.LogWarning("AlienBerry.asset not found. Please assign the Item ScriptableObject manually.");
        }

        serializedPicker.FindProperty("Quantity").intValue = 1;
        serializedPicker.FindProperty("RemainingQuantity").intValue = 1;
        serializedPicker.FindProperty("PickableIfInventoryIsFull").boolValue = false;
        serializedPicker.FindProperty("DisableObjectWhenDepleted").boolValue = true;
        serializedPicker.FindProperty("RequirePlayerTag").boolValue = true;

        serializedPicker.ApplyModifiedProperties();

        // Add placeholder visual (purple berry sphere)
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visual.name = "Visual";
        visual.transform.SetParent(pickerObject.transform);
        visual.transform.localPosition = new Vector3(0f, 0.3f, 0f);
        visual.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

        // Destroy the collider on the visual
        DestroyImmediate(visual.GetComponent<Collider>());

        // Create a purple/magenta organic material
        Material berryMaterial = new Material(Shader.Find("Standard"));
        berryMaterial.color = new Color(0.8f, 0.2f, 0.9f);
        berryMaterial.SetFloat("_Metallic", 0f);
        berryMaterial.SetFloat("_Glossiness", 0.6f);
        visual.GetComponent<Renderer>().material = berryMaterial;

        // Save as prefab
        string prefabPath = PREFAB_PATH + "AlienBerryPicker.prefab";
        PrefabUtility.SaveAsPrefabAsset(pickerObject, prefabPath);

        // Clean up
        DestroyImmediate(pickerObject);

        Debug.Log("AlienBerryPicker created at: " + prefabPath);
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
