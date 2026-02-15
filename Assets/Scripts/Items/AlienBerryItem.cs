using UnityEngine;
using MoreMountains.InventoryEngine;
using MoreMountains.TopDownEngine;

/// <summary>
/// AlienBerryItem - A consumable item that restores health/stamina when used
/// Extends InventoryItem to provide healing functionality similar to fruit in Animal Crossing
/// </summary>
public class AlienBerryItem : InventoryItem
{
    [Header("Healing Properties")]
    [Tooltip("Amount of stamina/health restored when consumed")]
    public int StaminaRestored = 10;

    /// <summary>
    /// Triggered when "Use" is selected in the Inventory GUI
    /// </summary>
    /// <param name="playerID">The ID of the player using the item</param>
    /// <returns>True if the item was successfully used, false otherwise</returns>
    public override bool Use(string playerID)
    {
        // 1. Find the player character
        // TDE's LevelManager provides global access to the player
        Character targetCharacter = LevelManager.Instance.Players[0];

        if (targetCharacter != null)
        {
            // 2. Get the Health component
            Health characterHealth = targetCharacter.GetComponent<Health>();

            if (characterHealth != null)
            {
                // 3. Prevent over-healing (Optional but good UX)
                if (characterHealth.CurrentHealth >= characterHealth.MaximumHealth)
                {
                    // Player is already at full health, don't consume the item
                    return false;
                }

                // 4. Apply Healing
                // ReceiveHealth(amount, instigator)
                characterHealth.ReceiveHealth(StaminaRestored, targetCharacter.gameObject);

                // 5. Trigger Feedbacks (Sound/Particles)
                // This would be handled by the Health component's Feedback setup
                // The "Consumption" feedback can be configured in the Health inspector

                return true; // Success: Item will be removed from inventory
            }
            else
            {
                Debug.LogWarning("AlienBerryItem: Target character does not have a Health component!");
            }
        }
        else
        {
            Debug.LogWarning("AlienBerryItem: Could not find player character!");
        }

        return false; // Failed to use item
    }
}
