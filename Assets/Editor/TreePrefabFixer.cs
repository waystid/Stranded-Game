using UnityEngine;
using UnityEditor;
using MoreMountains.TopDownEngine;

/// <summary>
/// Fixes the AlienTree prefab by:
/// 1. Adding/configuring TreeShakeZone component
/// 2. Adding/configuring Loot component
/// 3. Assigning TreeModel reference to Foliage child
/// </summary>
public class TreePrefabFixer : MonoBehaviour
{
    [MenuItem("Tools/Galactic Crossing/Fix AlienTree Prefab")]
    public static void FixAlienTreePrefab()
    {
        Debug.Log("=== Fixing AlienTree Prefab ===");

        // Load the prefab
        string prefabPath = "Assets/Prefabs/AlienTree.prefab";
        GameObject prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (prefabAsset == null)
        {
            Debug.LogError("Could not find AlienTree.prefab at: " + prefabPath);
            return;
        }

        // Load prefab into scene for editing
        GameObject prefabInstance = PrefabUtility.InstantiatePrefab(prefabAsset) as GameObject;

        // Find the Foliage child (the tree model to shake)
        Transform foliage = prefabInstance.transform.Find("Foliage");
        if (foliage == null)
        {
            Debug.LogError("Could not find 'Foliage' child in AlienTree prefab!");
            DestroyImmediate(prefabInstance);
            return;
        }

        // Check if InteractionZone exists, create if not
        Transform interactionZone = prefabInstance.transform.Find("InteractionZone");
        GameObject interactionZoneGO;

        if (interactionZone == null)
        {
            interactionZoneGO = new GameObject("InteractionZone");
            interactionZoneGO.transform.SetParent(prefabInstance.transform);
            interactionZoneGO.transform.localPosition = Vector3.zero;

            // Add BoxCollider (trigger)
            BoxCollider boxCollider = interactionZoneGO.AddComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = new Vector3(3f, 4f, 3f);
            boxCollider.center = new Vector3(0f, 2f, 0f);

            Debug.Log("Created InteractionZone with trigger collider");
        }
        else
        {
            interactionZoneGO = interactionZone.gameObject;
        }

        // Add or get TreeShakeZone component
        TreeShakeZone shakeZone = interactionZoneGO.GetComponent<TreeShakeZone>();
        if (shakeZone == null)
        {
            shakeZone = interactionZoneGO.AddComponent<TreeShakeZone>();
            Debug.Log("Added TreeShakeZone component");
        }

        // Check if LootSpawnPoint exists, create if not
        Transform lootSpawnPoint = prefabInstance.transform.Find("LootSpawnPoint");
        GameObject lootSpawnPointGO;

        if (lootSpawnPoint == null)
        {
            lootSpawnPointGO = new GameObject("LootSpawnPoint");
            lootSpawnPointGO.transform.SetParent(prefabInstance.transform);
            lootSpawnPointGO.transform.localPosition = new Vector3(0f, 2f, 0f);
            Debug.Log("Created LootSpawnPoint");
        }
        else
        {
            lootSpawnPointGO = lootSpawnPoint.gameObject;
        }

        // Add or get Loot component
        Loot lootComponent = lootSpawnPointGO.GetComponent<Loot>();
        if (lootComponent == null)
        {
            lootComponent = lootSpawnPointGO.AddComponent<Loot>();
            Debug.Log("Added Loot component");
        }

        // Configure TreeShakeZone using SerializedObject
        SerializedObject serializedShakeZone = new SerializedObject(shakeZone);

        serializedShakeZone.FindProperty("TreeModel").objectReferenceValue = foliage;
        serializedShakeZone.FindProperty("ShakeDuration").floatValue = 0.5f;
        serializedShakeZone.FindProperty("ShakeIntensity").floatValue = 10f;
        serializedShakeZone.FindProperty("ShakeCooldown").floatValue = 5f;
        serializedShakeZone.FindProperty("LootComponent").objectReferenceValue = lootComponent;

        // ButtonActivated properties
        serializedShakeZone.FindProperty("ButtonActivated").boolValue = true;
        serializedShakeZone.FindProperty("AutoActivation").boolValue = false;

        SerializedProperty promptTextProp = serializedShakeZone.FindProperty("PromptText");
        if (promptTextProp != null)
        {
            promptTextProp.stringValue = "Shake Tree";
        }

        serializedShakeZone.ApplyModifiedProperties();
        Debug.Log("Configured TreeShakeZone - TreeModel assigned to Foliage");

        // Configure Loot component using SerializedObject
        SerializedObject serializedLoot = new SerializedObject(lootComponent);

        serializedLoot.FindProperty("LootMode").enumValueIndex = (int)Loot.LootModes.LootTable;
        serializedLoot.FindProperty("SpawnLootOnDeath").boolValue = false;
        serializedLoot.FindProperty("SpawnLootOnDamage").boolValue = false;
        serializedLoot.FindProperty("CanSpawn").boolValue = true;
        serializedLoot.FindProperty("Delay").floatValue = 0f;

        SerializedProperty quantityProp = serializedLoot.FindProperty("Quantity");
        quantityProp.vector2Value = new Vector2(1, 3);

        serializedLoot.FindProperty("LimitedLootQuantity").boolValue = false;
        serializedLoot.FindProperty("AvoidObstacles").boolValue = true;
        serializedLoot.FindProperty("DimensionMode").enumValueIndex = (int)Loot.DimensionModes.ThreeD;

        // Configure spawn properties
        SerializedProperty spawnProps = serializedLoot.FindProperty("SpawnProperties");
        if (spawnProps != null)
        {
            SerializedProperty modeProp = spawnProps.FindPropertyRelative("Mode");
            if (modeProp != null)
            {
                modeProp.enumValueIndex = 1; // Circle mode
            }

            SerializedProperty radiusProp = spawnProps.FindPropertyRelative("Radius");
            if (radiusProp != null)
            {
                radiusProp.floatValue = 1.5f;
            }
        }

        serializedLoot.ApplyModifiedProperties();
        Debug.Log("Configured Loot component");

        // Apply changes back to prefab
        PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);

        // Clean up scene instance
        DestroyImmediate(prefabInstance);

        AssetDatabase.Refresh();

        Debug.Log("=== AlienTree Prefab Fixed Successfully! ===");
        Debug.Log("TreeModel: Assigned to Foliage child");
        Debug.Log("Loot: Configured (you still need to add ItemPicker prefabs to the loot table manually)");
    }
}
