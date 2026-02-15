using UnityEngine;
using UnityEditor;
using System.IO;

namespace GalacticCrossing.Editor
{
    /// <summary>
    /// Helper script for creating and configuring quest-related components.
    /// Generates the PrologueManager and quest tracking systems.
    /// </summary>
    public static class QuestSetupHelper
    {
        #region Constants

        private const string SCRIPTS_PATH = "Assets/Scripts/Managers";
        private const string MENU_PATH = "Tools/Galactic Crossing/Advanced/";

        #endregion

        #region Menu Items

        [MenuItem(MENU_PATH + "Create Quest Manager", false, 300)]
        public static void CreateQuestManagerScript()
        {
            CreatePrologueManagerScript();
            CreateQuestDataScript();

            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("Quest Scripts Created",
                "Quest management scripts have been created:\n\n" +
                $"• {SCRIPTS_PATH}/PrologueManager.cs\n" +
                $"• {SCRIPTS_PATH}/QuestData.cs\n\n" +
                "These scripts need to be manually edited to implement your specific quest logic.",
                "OK");
        }

        [MenuItem(MENU_PATH + "Add Quest Manager to Scene", false, 301)]
        public static void AddQuestManagerToScene()
        {
            // Check if PrologueManager GameObject already exists
            GameObject existing = GameObject.Find("PrologueManager");
            if (existing != null)
            {
                EditorUtility.DisplayDialog("Already Exists",
                    "A PrologueManager GameObject already exists in the scene.",
                    "OK");
                Selection.activeGameObject = existing;
                return;
            }

            // Create PrologueManager GameObject
            GameObject managerObj = new GameObject("PrologueManager");
            managerObj.tag = "GameController";

            EditorUtility.DisplayDialog("Quest Manager Added",
                "PrologueManager GameObject created.\n\n" +
                "Next steps:\n" +
                "1. Add your PrologueManager script component\n" +
                "2. Configure quest stages and requirements\n" +
                "3. Link to G.A.I.A. NPC for quest dialogue",
                "OK");

            Selection.activeGameObject = managerObj;
            EditorGUIUtility.PingObject(managerObj);
        }

        #endregion

        #region Script Generation

        /// <summary>
        /// Creates the PrologueManager script template
        /// </summary>
        private static void CreatePrologueManagerScript()
        {
            string scriptPath = $"{SCRIPTS_PATH}/PrologueManager.cs";

            // Create directory if it doesn't exist
            if (!AssetDatabase.IsValidFolder(SCRIPTS_PATH))
            {
                CreateDirectory(SCRIPTS_PATH);
            }

            // Check if file already exists
            if (File.Exists(scriptPath))
            {
                Debug.Log($"[QuestSetup] PrologueManager.cs already exists at {scriptPath}");
                return;
            }

            // Generate script content
            string scriptContent = @"using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;

namespace GalacticCrossing
{
    /// <summary>
    /// Manages the prologue quest progression for Galactic Crossing.
    /// Tracks player progress through Day 0 and Day 1 objectives.
    /// </summary>
    public class PrologueManager : MMSingleton<PrologueManager>
    {
        #region Quest Stages

        public enum QuestStage
        {
            NotStarted,
            Gathering,      // Collecting debris and crystals
            PlacingHab,     // Placing the habitat module
            Campfire,       // The system reboot ceremony
            Day1Start,      // Day 1 begins
            MuseumQuest,    // Collecting specimens for the bio-lab
            Completed
        }

        #endregion

        #region Public Fields

        [Header(""Quest Progress"")]
        [Tooltip(""Current quest stage"")]
        public QuestStage CurrentStage = QuestStage.NotStarted;

        [Header(""Gathering Quest"")]
        [Tooltip(""Number of debris items required"")]
        public int DebrisRequired = 10;

        [Tooltip(""Number of crystals required"")]
        public int CrystalsRequired = 6;

        [Header(""Museum Quest"")]
        [Tooltip(""Number of unique specimens required for bio-lab"")]
        public int SpecimensRequired = 5;

        [Header(""References"")]
        [Tooltip(""Reference to the main player inventory"")]
        public Inventory PlayerInventory;

        #endregion

        #region Private Fields

        private int _currentDebris = 0;
        private int _currentCrystals = 0;
        private int _currentSpecimens = 0;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            InitializeReferences();
        }

        private void Start()
        {
            StartPrologue();
        }

        private void Update()
        {
            UpdateQuestProgress();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize references to required components
        /// </summary>
        private void InitializeReferences()
        {
            if (PlayerInventory == null)
            {
                // Try to find the main inventory
                PlayerInventory = FindObjectOfType<Inventory>();
                if (PlayerInventory == null)
                {
                    Debug.LogWarning(""[PrologueManager] Player inventory not found!"");
                }
            }
        }

        #endregion

        #region Quest Management

        /// <summary>
        /// Starts the prologue sequence
        /// </summary>
        public void StartPrologue()
        {
            Debug.Log(""[PrologueManager] Starting prologue..."");
            CurrentStage = QuestStage.Gathering;

            // Trigger initial dialogue with G.A.I.A.
            ShowGatheringObjective();
        }

        /// <summary>
        /// Updates quest progress based on current stage
        /// </summary>
        private void UpdateQuestProgress()
        {
            switch (CurrentStage)
            {
                case QuestStage.Gathering:
                    CheckGatheringProgress();
                    break;

                case QuestStage.MuseumQuest:
                    CheckMuseumProgress();
                    break;
            }
        }

        /// <summary>
        /// Checks if gathering objectives are complete
        /// </summary>
        private void CheckGatheringProgress()
        {
            if (PlayerInventory == null) return;

            // Check debris count
            _currentDebris = PlayerInventory.GetQuantity(""ScrapMetal"");

            // Check crystals count
            _currentCrystals = PlayerInventory.GetQuantity(""EnergyCrystal"");

            // Check if objectives are complete
            if (_currentDebris >= DebrisRequired && _currentCrystals >= CrystalsRequired)
            {
                CompleteGatheringQuest();
            }
        }

        /// <summary>
        /// Checks if museum quest objectives are complete
        /// </summary>
        private void CheckMuseumProgress()
        {
            // TODO: Implement specimen tracking
            // This would check for unique creature donations
        }

        #endregion

        #region Quest Completion

        /// <summary>
        /// Called when gathering quest is completed
        /// </summary>
        private void CompleteGatheringQuest()
        {
            if (CurrentStage != QuestStage.Gathering) return;

            Debug.Log(""[PrologueManager] Gathering quest completed!"");
            CurrentStage = QuestStage.PlacingHab;

            // Trigger dialogue or UI notification
            ShowPlacementObjective();
        }

        /// <summary>
        /// Called when hab placement is complete
        /// </summary>
        public void OnHabPlaced()
        {
            if (CurrentStage != QuestStage.PlacingHab) return;

            Debug.Log(""[PrologueManager] Hab placed!"");
            CurrentStage = QuestStage.Campfire;

            // Trigger campfire ceremony
            StartCampfireCeremony();
        }

        /// <summary>
        /// Called when campfire ceremony is complete
        /// </summary>
        public void OnCampfireComplete()
        {
            Debug.Log(""[PrologueManager] Campfire ceremony complete!"");
            CurrentStage = QuestStage.Day1Start;

            // Save progress
            SaveProgress();
        }

        /// <summary>
        /// Advances to Day 1 content
        /// </summary>
        public void StartDay1()
        {
            Debug.Log(""[PrologueManager] Starting Day 1..."");
            CurrentStage = QuestStage.MuseumQuest;

            // Trigger Day 1 dialogue
            ShowMuseumObjective();
        }

        #endregion

        #region UI/Dialogue Triggers

        /// <summary>
        /// Shows gathering objective UI/dialogue
        /// </summary>
        private void ShowGatheringObjective()
        {
            Debug.Log(""[PrologueManager] Objective: Gather resources"");
            // TODO: Trigger UI notification
            // TODO: Update G.A.I.A. dialogue
        }

        /// <summary>
        /// Shows placement objective UI/dialogue
        /// </summary>
        private void ShowPlacementObjective()
        {
            Debug.Log(""[PrologueManager] Objective: Place habitat module"");
            // TODO: Trigger UI notification
        }

        /// <summary>
        /// Starts the campfire ceremony event
        /// </summary>
        private void StartCampfireCeremony()
        {
            Debug.Log(""[PrologueManager] Starting campfire ceremony..."");
            // TODO: Trigger cutscene or dialogue sequence
            // TODO: Allow planet naming
        }

        /// <summary>
        /// Shows museum objective UI/dialogue
        /// </summary>
        private void ShowMuseumObjective()
        {
            Debug.Log(""[PrologueManager] Objective: Collect specimens"");
            // TODO: Trigger UI notification
        }

        #endregion

        #region Save/Load

        /// <summary>
        /// Saves quest progress
        /// </summary>
        private void SaveProgress()
        {
            MMSaveLoadManager.Save(CurrentStage, ""QuestProgress.data"", ""GalacticCrossingSave"");
            Debug.Log(""[PrologueManager] Progress saved"");
        }

        /// <summary>
        /// Loads quest progress
        /// </summary>
        public void LoadProgress()
        {
            var savedStage = (QuestStage)MMSaveLoadManager.Load(typeof(QuestStage),
                ""QuestProgress.data"", ""GalacticCrossingSave"");

            if (savedStage != null)
            {
                CurrentStage = savedStage;
                Debug.Log($""[PrologueManager] Progress loaded: {CurrentStage}"");
            }
        }

        #endregion

        #region Public API

        /// <summary>
        /// Gets current gathering progress as a percentage
        /// </summary>
        public float GetGatheringProgress()
        {
            float debrisPercent = (float)_currentDebris / DebrisRequired;
            float crystalPercent = (float)_currentCrystals / CrystalsRequired;
            return (debrisPercent + crystalPercent) / 2f;
        }

        /// <summary>
        /// Gets formatted quest status string
        /// </summary>
        public string GetQuestStatus()
        {
            switch (CurrentStage)
            {
                case QuestStage.Gathering:
                    return $""Debris: {_currentDebris}/{DebrisRequired}\\nCrystals: {_currentCrystals}/{CrystalsRequired}"";
                case QuestStage.PlacingHab:
                    return ""Place your habitat module"";
                case QuestStage.MuseumQuest:
                    return $""Specimens: {_currentSpecimens}/{SpecimensRequired}"";
                default:
                    return ""No active quest"";
            }
        }

        #endregion
    }
}
";

            // Write the file
            File.WriteAllText(scriptPath, scriptContent);
            Debug.Log($"[QuestSetup] Created PrologueManager.cs at {scriptPath}");
        }

        /// <summary>
        /// Creates the QuestData script template
        /// </summary>
        private static void CreateQuestDataScript()
        {
            string scriptPath = $"{SCRIPTS_PATH}/QuestData.cs";

            if (File.Exists(scriptPath))
            {
                Debug.Log($"[QuestSetup] QuestData.cs already exists at {scriptPath}");
                return;
            }

            string scriptContent = @"using System;
using UnityEngine;

namespace GalacticCrossing
{
    /// <summary>
    /// Serializable data structure for quest information
    /// </summary>
    [Serializable]
    public class QuestData
    {
        [Header(""Quest Info"")]
        public string QuestID;
        public string QuestName;
        public string QuestDescription;

        [Header(""Objectives"")]
        public ObjectiveData[] Objectives;

        [Header(""Rewards"")]
        public string[] RewardItemIDs;
        public int[] RewardQuantities;

        [Header(""State"")]
        public bool IsActive = false;
        public bool IsCompleted = false;

        /// <summary>
        /// Checks if all objectives are complete
        /// </summary>
        public bool AreAllObjectivesComplete()
        {
            if (Objectives == null || Objectives.Length == 0)
                return false;

            foreach (var objective in Objectives)
            {
                if (!objective.IsCompleted)
                    return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Individual quest objective data
    /// </summary>
    [Serializable]
    public class ObjectiveData
    {
        public string ObjectiveDescription;
        public ObjectiveType Type;
        public string TargetID;
        public int RequiredAmount;
        public int CurrentAmount;
        public bool IsCompleted;

        public enum ObjectiveType
        {
            Collect,    // Collect X items
            Talk,       // Talk to NPC
            Place,      // Place object
            Reach,      // Reach location
            Defeat,     // Defeat enemies (future)
            Custom      // Custom condition
        }

        /// <summary>
        /// Updates progress and checks completion
        /// </summary>
        public void UpdateProgress(int amount)
        {
            CurrentAmount = Mathf.Min(CurrentAmount + amount, RequiredAmount);
            if (CurrentAmount >= RequiredAmount)
            {
                IsCompleted = true;
            }
        }
    }
}
";

            File.WriteAllText(scriptPath, scriptContent);
            Debug.Log($"[QuestSetup] Created QuestData.cs at {scriptPath}");
        }

        #endregion

        #region Helper Methods

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
                Debug.Log($"[QuestSetup] Created directory: {path}");
            }
        }

        #endregion
    }
}
