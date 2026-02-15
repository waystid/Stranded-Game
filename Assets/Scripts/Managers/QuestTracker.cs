using UnityEngine;
using MoreMountains.InventoryEngine;
using MoreMountains.TopDownEngine;

/// <summary>
/// QuestTracker - Manages the progression of the prologue quest
/// Tracks collection of ScrapMetal and EnergyCrystal items for the Day 0 gathering quest
/// </summary>
public class QuestTracker : MonoBehaviour
{
    [Header("Quest Configuration")]
    [Tooltip("Reference to the player's main inventory")]
    public Inventory PlayerInventory;

    [Tooltip("Number of Scrap Metal pieces required")]
    public int ScrapMetalRequired = 10;

    [Tooltip("Number of Energy Crystals required")]
    public int EnergyCrystalRequired = 6;

    [Tooltip("Item ID for Scrap Metal (must match the ItemID in the ScriptableObject)")]
    public string ScrapMetalItemID = "ScrapMetal";

    [Tooltip("Item ID for Energy Crystal (must match the ItemID in the ScriptableObject)")]
    public string EnergyCrystalItemID = "EnergyCrystal";

    [Header("Quest State")]
    [Tooltip("Current stage of the quest")]
    public QuestStage CurrentStage = QuestStage.Gathering;

    // Quest stage enumeration
    public enum QuestStage
    {
        Gathering,      // Player is collecting resources
        ReadyToTurnIn,  // Player has collected enough resources
        Completed       // Quest has been turned in
    }

    // Internal tracking
    private int _currentScrapMetal = 0;
    private int _currentEnergyCrystal = 0;
    private bool _questCompleted = false;

    /// <summary>
    /// Initialization - find the player inventory if not assigned
    /// </summary>
    private void Start()
    {
        if (PlayerInventory == null)
        {
            // Attempt to find the player's main inventory
            PlayerInventory = FindFirstObjectByType<Inventory>();

            if (PlayerInventory == null)
            {
                Debug.LogError("QuestTracker: PlayerInventory not assigned and could not be found!");
            }
        }
    }

    /// <summary>
    /// Update is called once per frame
    /// Checks inventory quantities and updates quest progress
    /// </summary>
    private void Update()
    {
        // Only track during the gathering stage
        if (CurrentStage != QuestStage.Gathering || _questCompleted)
        {
            return;
        }

        // Check inventory for required items
        if (PlayerInventory != null)
        {
            // Get current quantities from inventory
            _currentScrapMetal = PlayerInventory.GetQuantity(ScrapMetalItemID);
            _currentEnergyCrystal = PlayerInventory.GetQuantity(EnergyCrystalItemID);

            // Check if collection requirements are met
            if (_currentScrapMetal >= ScrapMetalRequired &&
                _currentEnergyCrystal >= EnergyCrystalRequired)
            {
                // Transition to ReadyToTurnIn stage
                CurrentStage = QuestStage.ReadyToTurnIn;
                OnQuestReadyToTurnIn();
            }
        }
    }

    /// <summary>
    /// Called when the player has collected enough resources
    /// Override this method or use events to trigger dialogue/UI updates
    /// </summary>
    protected virtual void OnQuestReadyToTurnIn()
    {
        Debug.Log("QuestTracker: All resources collected! Ready to turn in to G.A.I.A.");

        // Here you would trigger:
        // - Enable the "Turn In" dialogue option on the G.A.I.A. NPC
        // - Update UI notification
        // - Play sound effect
        // - Trigger MMGameEvent for other systems to react
    }

    /// <summary>
    /// Call this method when the player turns in the quest to G.A.I.A.
    /// This should be called from the dialogue system or NPC interaction script
    /// </summary>
    public void CompleteQuest()
    {
        if (CurrentStage == QuestStage.ReadyToTurnIn)
        {
            CurrentStage = QuestStage.Completed;
            _questCompleted = true;

            // Remove the items from inventory
            if (PlayerInventory != null)
            {
                PlayerInventory.RemoveItemByID(ScrapMetalItemID, ScrapMetalRequired);
                PlayerInventory.RemoveItemByID(EnergyCrystalItemID, EnergyCrystalRequired);
            }

            OnQuestCompleted();
        }
        else
        {
            Debug.LogWarning("QuestTracker: Cannot complete quest - requirements not met!");
        }
    }

    /// <summary>
    /// Called when the quest is successfully completed
    /// Override this method to trigger rewards, story progression, etc.
    /// </summary>
    protected virtual void OnQuestCompleted()
    {
        Debug.Log("QuestTracker: Quest completed! The Island Warming Party can begin.");

        // Here you would trigger:
        // - Story progression to the "Island Warming Party" sequence
        // - Unlock rewards
        // - Save game state
        // - Trigger cutscene or dialogue
    }

    /// <summary>
    /// Get current progress as a formatted string for UI display
    /// </summary>
    public string GetProgressText()
    {
        return $"Scrap Metal: {_currentScrapMetal}/{ScrapMetalRequired}\n" +
               $"Energy Crystals: {_currentEnergyCrystal}/{EnergyCrystalRequired}";
    }

    /// <summary>
    /// Get progress percentage (0-100) for progress bars
    /// </summary>
    public float GetProgressPercentage()
    {
        float scrapProgress = (float)_currentScrapMetal / ScrapMetalRequired;
        float crystalProgress = (float)_currentEnergyCrystal / EnergyCrystalRequired;

        // Average of both requirements
        return ((scrapProgress + crystalProgress) / 2f) * 100f;
    }
}
