using UnityEngine;
using UnityEditor;
using MoreMountains.InventoryEngine;
using MoreMountains.TopDownEngine;
using System.IO;

namespace GalacticCrossing.Editor
{
    /// <summary>
    /// Automates the creation of game assets for Galactic Crossing MVP.
    /// Creates ScriptableObjects, prefabs, and other required assets.
    /// </summary>
    public static class AssetCreationAutomation
    {
        #region Constants

        private const string RESOURCES_PATH = "Assets/Resources";
        private const string ITEMS_PATH = "Assets/Resources/Items";
        private const string PREFABS_PATH = "Assets/Prefabs";
        private const string GALACTIC_CROSSING_PATH = "Assets/GalacticCrossing";

        #endregion

        #region Main Entry Point

        /// <summary>
        /// Creates all required assets for the MVP
        /// </summary>
        public static void CreateAllAssets()
        {
            Debug.Log("[GalacticCrossing] Creating all assets...");

            // Ensure directories exist
            CreateDirectories();

            // Create items
            CreateScrapMetalItem();
            CreateEnergyCrystalItem();
            CreateAlienBerryItem();

            // Create prefabs
            CreateItemPickerPrefabs();
            CreateTreePrefab();
            CreateGAIAPrefab();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[GalacticCrossing] All assets created successfully!");
        }

        #endregion

        #region Directory Creation

        /// <summary>
        /// Creates all necessary directories
        /// </summary>
        private static void CreateDirectories()
        {
            CreateDirectory(RESOURCES_PATH);
            CreateDirectory(ITEMS_PATH);
            CreateDirectory(PREFABS_PATH);
            CreateDirectory(GALACTIC_CROSSING_PATH);

            Debug.Log("[GalacticCrossing] Directory structure created");
        }

        /// <summary>
        /// Creates a directory if it doesn't exist
        /// </summary>
        private static void CreateDirectory(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                string parentPath = Path.GetDirectoryName(path).Replace("\\", "/");
                string folderName = Path.GetFileName(path);
                AssetDatabase.CreateFolder(parentPath, folderName);
                Debug.Log($"[GalacticCrossing] Created directory: {path}");
            }
        }

        #endregion

        #region Item Creation

        /// <summary>
        /// Creates the Scrap Metal item ScriptableObject
        /// </summary>
        private static void CreateScrapMetalItem()
        {
            string assetPath = $"{ITEMS_PATH}/ScrapMetal.asset";

            if (AssetDatabase.LoadAssetAtPath<InventoryItem>(assetPath) != null)
            {
                Debug.Log("[GalacticCrossing] ScrapMetal item already exists, skipping");
                return;
            }

            var scrapMetal = ScriptableObject.CreateInstance<InventoryItem>();

            // Configure item properties
            scrapMetal.ItemID = "ScrapMetal";
            scrapMetal.ItemName = "Scrap Metal";
            scrapMetal.Description = "Twisted hull plating from the crash. Useful for crafting repairs.";
            scrapMetal.MaximumStack = 30;
            scrapMetal.Usable = false; // Can't be used directly, only for crafting

            // Create and save the asset
            AssetDatabase.CreateAsset(scrapMetal, assetPath);

            Debug.Log($"[GalacticCrossing] Created ScrapMetal item at {assetPath}");
        }

        /// <summary>
        /// Creates the Energy Crystal item ScriptableObject
        /// </summary>
        private static void CreateEnergyCrystalItem()
        {
            string assetPath = $"{ITEMS_PATH}/EnergyCrystal.asset";

            if (AssetDatabase.LoadAssetAtPath<InventoryItem>(assetPath) != null)
            {
                Debug.Log("[GalacticCrossing] EnergyCrystal item already exists, skipping");
                return;
            }

            var energyCrystal = ScriptableObject.CreateInstance<InventoryItem>();

            // Configure item properties
            energyCrystal.ItemID = "EnergyCrystal";
            energyCrystal.ItemName = "Energy Crystal";
            energyCrystal.Description = "A glowing crystalline formation. Contains stored energy from the planet's core.";
            energyCrystal.MaximumStack = 30;
            energyCrystal.Usable = false; // Resource item

            // Create and save the asset
            AssetDatabase.CreateAsset(energyCrystal, assetPath);

            Debug.Log($"[GalacticCrossing] Created EnergyCrystal item at {assetPath}");
        }

        /// <summary>
        /// Creates the Alien Berry item ScriptableObject
        /// Note: This creates a basic InventoryItem. The AlienBerryItem script should already exist
        /// in Assets/Scripts/Items/AlienBerryItem.cs and can be manually assigned.
        /// </summary>
        private static void CreateAlienBerryItem()
        {
            string assetPath = $"{ITEMS_PATH}/AlienBerry.asset";

            // Check if AlienBerryItem class exists and use it, otherwise use base InventoryItem
            var alienBerryType = System.Type.GetType("AlienBerryItem");
            InventoryItem alienBerry;

            if (alienBerryType != null && alienBerryType.IsSubclassOf(typeof(InventoryItem)))
            {
                alienBerry = (InventoryItem)ScriptableObject.CreateInstance(alienBerryType);
                Debug.Log("[GalacticCrossing] Using AlienBerryItem custom class");
            }
            else
            {
                alienBerry = ScriptableObject.CreateInstance<InventoryItem>();
                Debug.LogWarning("[GalacticCrossing] AlienBerryItem class not found, using base InventoryItem");
            }

            if (AssetDatabase.LoadAssetAtPath<InventoryItem>(assetPath) != null)
            {
                Debug.Log("[GalacticCrossing] AlienBerry item already exists, skipping");
                return;
            }

            // Configure item properties
            alienBerry.ItemID = "AlienBerry";
            alienBerry.ItemName = "Alien Berry";
            alienBerry.Description = "A nutritious alien fruit. Restores stamina when consumed.";
            alienBerry.MaximumStack = 10;
            alienBerry.Usable = true; // Can be consumed

            // If we successfully created an AlienBerryItem, set the stamina value
            if (alienBerryType != null)
            {
                var staminaField = alienBerryType.GetField("StaminaRestored");
                if (staminaField != null)
                {
                    staminaField.SetValue(alienBerry, 10);
                }
            }

            // Create and save the asset
            AssetDatabase.CreateAsset(alienBerry, assetPath);

            Debug.Log($"[GalacticCrossing] Created AlienBerry item at {assetPath}");
        }

        #endregion

        #region Prefab Creation

        /// <summary>
        /// Creates ItemPicker prefabs for each resource type
        /// </summary>
        private static void CreateItemPickerPrefabs()
        {
            Debug.Log("[GalacticCrossing] Creating ItemPicker prefabs...");

            CreateItemPickerPrefab("ScrapMetal", "ScrapMetalPicker", new Vector3(0.3f, 0.1f, 0.3f));
            CreateItemPickerPrefab("EnergyCrystal", "EnergyCrystalPicker", new Vector3(0.2f, 0.3f, 0.2f));
            CreateItemPickerPrefab("AlienBerry", "AlienBerryPicker", new Vector3(0.15f, 0.15f, 0.15f));

            Debug.Log("[GalacticCrossing] ItemPicker prefabs created");
        }

        /// <summary>
        /// Creates a single ItemPicker prefab
        /// </summary>
        private static void CreateItemPickerPrefab(string itemID, string prefabName, Vector3 scale)
        {
            string prefabPath = $"{PREFABS_PATH}/{prefabName}.prefab";

            // Check if prefab already exists
            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            {
                Debug.Log($"[GalacticCrossing] {prefabName} already exists, skipping");
                return;
            }

            // Create GameObject
            GameObject pickerObj = new GameObject(prefabName);

            // Add basic visual (cube for now - can be replaced with proper model)
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            visual.name = "Visual";
            visual.transform.SetParent(pickerObj.transform);
            visual.transform.localScale = scale;
            visual.transform.localPosition = Vector3.zero;

            // Add ItemPicker component
            var itemPicker = pickerObj.AddComponent<ItemPicker>();

            // Load the corresponding item
            InventoryItem item = AssetDatabase.LoadAssetAtPath<InventoryItem>($"{ITEMS_PATH}/{itemID}.asset");
            if (item != null)
            {
                itemPicker.Item = item;
                itemPicker.Quantity = 1;
            }
            else
            {
                Debug.LogWarning($"[GalacticCrossing] Could not find item {itemID} for picker");
            }

            // Configure collider as trigger
            var collider = pickerObj.GetComponent<BoxCollider>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }

            // Save as prefab
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(pickerObj, prefabPath);
            Object.DestroyImmediate(pickerObj);

            Debug.Log($"[GalacticCrossing] Created ItemPicker prefab: {prefabPath}");
        }

        /// <summary>
        /// Creates the Tree prefab with shake zone
        /// </summary>
        private static void CreateTreePrefab()
        {
            string prefabPath = $"{PREFABS_PATH}/AlienTree.prefab";

            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            {
                Debug.Log("[GalacticCrossing] AlienTree prefab already exists, skipping");
                return;
            }

            // Create tree GameObject
            GameObject treeObj = new GameObject("AlienTree");

            // Add visual (cylinder for trunk, sphere for foliage)
            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = "Trunk";
            trunk.transform.SetParent(treeObj.transform);
            trunk.transform.localPosition = new Vector3(0, 1, 0);
            trunk.transform.localScale = new Vector3(0.5f, 2f, 0.5f);

            GameObject foliage = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            foliage.name = "Foliage";
            foliage.transform.SetParent(treeObj.transform);
            foliage.transform.localPosition = new Vector3(0, 3.5f, 0);
            foliage.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);

            // Add TreeShakeZone component if it exists
            var treeShakeType = System.Type.GetType("TreeShakeZone");
            if (treeShakeType != null)
            {
                var shakeZone = treeObj.AddComponent(treeShakeType);
                Debug.Log("[GalacticCrossing] Added TreeShakeZone component");

                // Try to configure it
                var itemField = treeShakeType.GetField("ItemToDrop");
                if (itemField != null)
                {
                    var berryItem = AssetDatabase.LoadAssetAtPath<InventoryItem>($"{ITEMS_PATH}/AlienBerry.asset");
                    if (berryItem != null)
                    {
                        itemField.SetValue(shakeZone, berryItem);
                    }
                }
            }
            else
            {
                Debug.LogWarning("[GalacticCrossing] TreeShakeZone class not found");
            }

            // Add collider
            var capsuleCollider = treeObj.AddComponent<CapsuleCollider>();
            capsuleCollider.radius = 0.5f;
            capsuleCollider.height = 4f;
            capsuleCollider.center = new Vector3(0, 2, 0);

            // Save as prefab
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(treeObj, prefabPath);
            Object.DestroyImmediate(treeObj);

            Debug.Log($"[GalacticCrossing] Created AlienTree prefab: {prefabPath}");
        }

        /// <summary>
        /// Creates the G.A.I.A. NPC prefab
        /// </summary>
        private static void CreateGAIAPrefab()
        {
            string prefabPath = $"{PREFABS_PATH}/GAIA_NPC.prefab";

            if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
            {
                Debug.Log("[GalacticCrossing] GAIA_NPC prefab already exists, skipping");
                return;
            }

            // Create NPC GameObject
            GameObject gaiaObj = new GameObject("GAIA_NPC");

            // Add visual representation (sphere for hologram)
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visual.name = "HologramVisual";
            visual.transform.SetParent(gaiaObj.transform);
            visual.transform.localPosition = new Vector3(0, 1, 0);
            visual.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

            // Make it look holographic (semi-transparent material)
            var renderer = visual.GetComponent<Renderer>();
            if (renderer != null)
            {
                // Create a simple semi-transparent material
                Material hologramMat = new Material(Shader.Find("Standard"));
                hologramMat.SetFloat("_Mode", 3); // Transparent mode
                hologramMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                hologramMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                hologramMat.SetInt("_ZWrite", 0);
                hologramMat.DisableKeyword("_ALPHATEST_ON");
                hologramMat.EnableKeyword("_ALPHABLEND_ON");
                hologramMat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                hologramMat.renderQueue = 3000;
                hologramMat.color = new Color(0.3f, 0.8f, 1.0f, 0.6f); // Cyan hologram

                renderer.material = hologramMat;
            }

            // Add DialogueZone component
            var dialogueZone = gaiaObj.AddComponent<DialogueZone>();

            // Configure dialogue
            dialogueZone.Dialogue = new DialogueElement[]
            {
                new DialogueElement { DialogueLine = "Welcome to the colonization initiative... er, emergency landing protocol." },
                new DialogueElement { DialogueLine = "I am G.A.I.A., your Global Artificial Intelligence Assistant." },
                new DialogueElement { DialogueLine = "Let's get you settled on this lovely planet!" }
            };

            dialogueZone.ButtonPromptText = "Talk to G.A.I.A.";

            // Add trigger collider for interaction zone
            var sphereCollider = gaiaObj.GetComponent<SphereCollider>();
            if (sphereCollider == null)
            {
                sphereCollider = gaiaObj.AddComponent<SphereCollider>();
            }
            sphereCollider.isTrigger = true;
            sphereCollider.radius = 2f;
            sphereCollider.center = new Vector3(0, 1, 0);

            // Save as prefab
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(gaiaObj, prefabPath);
            Object.DestroyImmediate(gaiaObj);

            Debug.Log($"[GalacticCrossing] Created GAIA_NPC prefab: {prefabPath}");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a basic material with specified color
        /// </summary>
        private static Material CreateBasicMaterial(Color color, string name = "NewMaterial")
        {
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = color;
            mat.name = name;
            return mat;
        }

        #endregion
    }
}
