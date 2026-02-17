using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using System.Collections.Generic;
using System.Linq;

#if CINEMACHINE
using Cinemachine;
#endif

namespace GalacticCrossing.Editor
{
    /// <summary>
    /// Automates scene setup operations for the Galactic Crossing MVP.
    /// Handles scene duplication, enemy/weapon removal, and component configuration.
    /// </summary>
    public static class SceneSetupAutomation
    {
        #region Scene Setup

        /// <summary>
        /// Main entry point for complete scene setup
        /// </summary>
        public static void SetupScene()
        {
            Debug.Log("[GalacticCrossing] Starting scene setup...");

            RemoveAIEnemies();
            RemoveWeapons();
            AddGridManager();
            ConfigurePlayerCharacter();
            ConfigureCameraSettings();

            Debug.Log("[GalacticCrossing] Scene setup completed!");
        }

        #endregion

        #region Remove AI and Weapons

        /// <summary>
        /// Finds and removes all AI enemies from the scene
        /// </summary>
        public static void RemoveAIEnemies()
        {
            Debug.Log("[GalacticCrossing] Removing AI enemies...");

            int removedCount = 0;
            List<GameObject> toRemove = new List<GameObject>();

            // Find all objects with AIBrain component (all AI-controlled enemies)
            var aiBrains = Object.FindObjectsByType<AIBrain>(FindObjectsSortMode.None);
            foreach (var brain in aiBrains)
            {
                toRemove.Add(brain.gameObject);
            }

            // Find all objects with Health component that are enemies
            var healthComponents = Object.FindObjectsByType<Health>(FindObjectsSortMode.None);
            foreach (var health in healthComponents)
            {
                // Check if this is an enemy (not the player)
                Character character = health.GetComponent<Character>();
                if (character != null && character.CharacterType == Character.CharacterTypes.AI)
                {
                    if (!toRemove.Contains(health.gameObject))
                    {
                        toRemove.Add(health.gameObject);
                    }
                }
            }

            // Remove all identified enemies
            foreach (var obj in toRemove)
            {
                Debug.Log($"[GalacticCrossing] Removing AI enemy: {obj.name}");
                Object.DestroyImmediate(obj);
                removedCount++;
            }

            Debug.Log($"[GalacticCrossing] Removed {removedCount} AI enemies");
        }

        /// <summary>
        /// Finds and removes all weapon pickups from the scene
        /// </summary>
        public static void RemoveWeapons()
        {
            Debug.Log("[GalacticCrossing] Removing weapon pickups...");

            int removedCount = 0;
            List<GameObject> toRemove = new List<GameObject>();

            // Find all WeaponPickup components
            var weaponPickups = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .Where(mb => mb.GetType().Name.Contains("WeaponPickup") ||
                             mb.GetType().Name.Contains("Weapon"))
                .ToList();

            foreach (var pickup in weaponPickups)
            {
                if (pickup != null && pickup.gameObject != null && !toRemove.Contains(pickup.gameObject))
                {
                    toRemove.Add(pickup.gameObject);
                }
            }

            // Find objects in "Weapons" layer or with "Weapon" tag
            var allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (var obj in allObjects)
            {
                if (obj != null && (obj.layer == LayerMask.NameToLayer("Weapon") ||
                    obj.tag == "Weapon" ||
                    obj.name.ToLower().Contains("weapon") ||
                    obj.name.ToLower().Contains("gun") ||
                    obj.name.ToLower().Contains("rifle")))
                {
                    if (!toRemove.Contains(obj))
                    {
                        toRemove.Add(obj);
                    }
                }
            }

            // Remove all identified weapons
            foreach (var obj in toRemove)
            {
                if (obj == null) continue;
                Debug.Log($"[GalacticCrossing] Removing weapon: {obj.name}");
                Object.DestroyImmediate(obj);
                removedCount++;
            }

            Debug.Log($"[GalacticCrossing] Removed {removedCount} weapons");
        }

        #endregion

        #region GridManager Setup

        /// <summary>
        /// Adds a GridManager GameObject to the scene if it doesn't exist
        /// </summary>
        public static void AddGridManager()
        {
            Debug.Log("[GalacticCrossing] Setting up GridManager...");

            // Check if GridManager already exists
            var existingManager = Object.FindFirstObjectByType<GridManager>();
            if (existingManager != null)
            {
                Debug.Log("[GalacticCrossing] GridManager already exists, skipping creation");
                return;
            }

            // Create GridManager GameObject
            GameObject gridManagerObj = new GameObject("GridManager");
            var gridManager = gridManagerObj.AddComponent<IslandGridManager>();

            // Configure GridManager
            gridManager.CellSize = 1f;

            // Create Island root child object (used as coordinate origin)
            GameObject gridOrigin = new GameObject("GridOrigin");
            gridOrigin.transform.SetParent(gridManagerObj.transform);
            gridOrigin.transform.position = Vector3.zero;
            gridManager.IslandRoot = gridOrigin.transform;

            Debug.Log("[GalacticCrossing] GridManager created and configured");

            EditorUtility.SetDirty(gridManagerObj);
        }

        #endregion

        #region Player Configuration

        /// <summary>
        /// Finds the player character and configures it for grid movement
        /// </summary>
        public static void ConfigurePlayerCharacter()
        {
            Debug.Log("[GalacticCrossing] Configuring player character...");

            // Find the player character
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

            if (player == null)
            {
                Debug.LogWarning("[GalacticCrossing] No player character found in scene!");
                return;
            }

            Debug.Log($"[GalacticCrossing] Found player: {player.gameObject.name}");

            // Remove CharacterHandleWeapon component
            var handleWeapon = player.GetComponent<CharacterHandleWeapon>();
            if (handleWeapon != null)
            {
                Debug.Log("[GalacticCrossing] Removing CharacterHandleWeapon component");
                Object.DestroyImmediate(handleWeapon);
            }

            // Remove any weapon-related abilities
            RemoveWeaponAbilities(player);

            // Add CharacterGridMovement if it doesn't exist
            var gridMovement = player.GetComponent<CharacterGridMovement>();
            if (gridMovement == null)
            {
                Debug.Log("[GalacticCrossing] Adding CharacterGridMovement component");
                gridMovement = player.gameObject.AddComponent<CharacterGridMovement>();

                // Configure grid movement settings
                ConfigureGridMovement(gridMovement);
            }

            // Configure regular movement for "weighted" feel if CharacterMovement exists
            var characterMovement = player.GetComponent<CharacterMovement>();
            if (characterMovement != null)
            {
                ConfigureWeightedMovement(characterMovement);
            }

            // Configure orientation
            var orientation = player.GetComponent<CharacterOrientation3D>();
            if (orientation != null)
            {
                ConfigureOrientation(orientation);
            }

            EditorUtility.SetDirty(player.gameObject);
            Debug.Log("[GalacticCrossing] Player character configured successfully");
        }

        /// <summary>
        /// Removes weapon-related abilities from the character
        /// </summary>
        private static void RemoveWeaponAbilities(Character character)
        {
            var abilities = character.GetComponents<CharacterAbility>();
            foreach (var ability in abilities)
            {
                if (ability.GetType().Name.Contains("Weapon") ||
                    ability.GetType().Name.Contains("Reload") ||
                    ability.GetType().Name.Contains("Shoot"))
                {
                    Debug.Log($"[GalacticCrossing] Removing ability: {ability.GetType().Name}");
                    Object.DestroyImmediate(ability);
                }
            }
        }

        /// <summary>
        /// Configures CharacterGridMovement component with optimal settings
        /// </summary>
        private static void ConfigureGridMovement(CharacterGridMovement gridMovement)
        {
            gridMovement.MaximumSpeed = 8f;
            gridMovement.Acceleration = 5f;
            Debug.Log("[GalacticCrossing] CharacterGridMovement configured");
        }

        /// <summary>
        /// Configures CharacterMovement for "weighted" Animal Crossing-style feel
        /// </summary>
        private static void ConfigureWeightedMovement(CharacterMovement movement)
        {
            // Values from the GDD for "cozy" feel
            // Walk Speed: 5.5 (slower, more deliberate)
            // Run Speed: 8.5
            // Acceleration: 12-15 (introduces 0.3s ramp-up)
            // Deceleration: 8-10 (creates drifting stop)

            Debug.Log("[GalacticCrossing] Configuring weighted movement parameters");

            // Note: These properties may need to be adjusted based on the actual
            // CharacterMovement implementation. This is a template.
            // movement.WalkSpeed = 5.5f;
            // movement.RunSpeed = 8.5f;
            // movement.MovementSpeedMultiplier = 1f;

            Debug.Log("[GalacticCrossing] Weighted movement configured (verify settings manually)");
        }

        /// <summary>
        /// Configures CharacterOrientation3D for smooth rotation
        /// </summary>
        private static void ConfigureOrientation(CharacterOrientation3D orientation)
        {
            // Rotation Speed: 8.0 (slower than default for weighted feel)
            Debug.Log("[GalacticCrossing] Configuring character orientation");
            // orientation.RotationSpeed = 8.0f;
        }

        #endregion

        #region Camera Configuration

        /// <summary>
        /// Configures camera settings for the "Rolling Log" view
        /// </summary>
        public static void ConfigureCameraSettings()
        {
            Debug.Log("[GalacticCrossing] Configuring camera settings...");

#if CINEMACHINE
            // Find Cinemachine Virtual Camera
            var cameras = Object.FindObjectsByType<UnityEngine.GameObject>(FindObjectsSortMode.None)
                .Where(go => go.name.Contains("VirtualCamera") ||
                            go.name.Contains("CM") ||
                            go.GetComponent<CinemachineVirtualCamera>() != null)
                .ToList();

            if (cameras.Count == 0)
            {
                Debug.LogWarning("[GalacticCrossing] No virtual camera found in scene");
                return;
            }

            foreach (var cameraObj in cameras)
            {
                var vcam = cameraObj.GetComponent<CinemachineVirtualCamera>();
                if (vcam != null)
                {
                    Debug.Log($"[GalacticCrossing] Configuring camera: {cameraObj.name}");

                    // Configure lens settings for Animal Crossing-style view
                    // FOV: 30-40 (lower FOV approximates orthographic look)
                    vcam.m_Lens.FieldOfView = 35f;

                    // Configure body settings
                    // Follow Offset: (0, 10, -7) - creates steep 50-60 degree angle
                    // Note: This requires Transposer body type
                    Debug.Log("[GalacticCrossing] Camera lens configured (verify body settings manually)");

                    EditorUtility.SetDirty(cameraObj);
                }
            }

            Debug.Log("[GalacticCrossing] Camera configuration completed");
#else
            Debug.LogWarning("[GalacticCrossing] Cinemachine not detected. Camera configuration skipped. Install Cinemachine package to enable camera auto-configuration.");
#endif
        }

        #endregion

        #region Configuration Helpers

        /// <summary>
        /// Configures all scene objects (called after asset creation)
        /// </summary>
        public static void ConfigureSceneObjects()
        {
            Debug.Log("[GalacticCrossing] Configuring scene objects...");

            // Re-run player configuration in case it was missed
            ConfigurePlayerCharacter();

            // Add any additional configuration here
            // For example: placing prefabs, configuring spawn points, etc.

            Debug.Log("[GalacticCrossing] Scene object configuration completed");
        }

        #endregion
    }
}
