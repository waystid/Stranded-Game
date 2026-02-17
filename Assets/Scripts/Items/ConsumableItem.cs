using UnityEngine;
using MoreMountains.TopDownEngine;

/// <summary>
/// Consumable item that applies an effect (health/speed) when used.
/// Migrated from AlienBerryItem — all berry-style items now use this class.
/// 
/// Returns true from Use() → TDE removes one unit from inventory (Consumable = true).
/// Returns false if effect cannot apply (e.g. player already at full health).
/// </summary>
[CreateAssetMenu(menuName = "CosmicColony/Items/Consumable")]
public class ConsumableItem : CosmicItem
{
    [Header("Consumable Effects")]
    [Tooltip("Health points restored on use (0 = no healing)")]
    public float HealthRestore;

    [Tooltip("Stamina points restored on use — reserved for future stamina system")]
    public float StaminaRestore;

    [Tooltip("Speed multiplier added on use (e.g. 1.5 = 50% faster). 0 = no boost.")]
    public float SpeedBoostAmount;

    [Tooltip("Duration of the speed boost in seconds")]
    public float SpeedBoostDuration;

    private void Reset()
    {
        ItemType = CosmicItemType.Consumable;
        Usable = true;
        Consumable = true;
        Droppable = true;
    }

    /// <summary>
    /// Applies health and/or speed effects to the first player.
    /// Returns true if the item was consumed, false if the effect could not apply.
    /// </summary>
    public override bool Use(string playerID)
    {
        Character targetCharacter = LevelManager.Instance?.Players?[0];
        if (targetCharacter == null)
        {
            Debug.LogWarning("[ConsumableItem] No player found in LevelManager.");
            return false;
        }

        bool applied = false;

        if (HealthRestore > 0f)
        {
            Health health = targetCharacter.GetComponent<Health>();
            if (health != null)
            {
                if (health.CurrentHealth >= health.MaximumHealth)
                {
                    // Already at full health — don't waste the item
                    return false;
                }
                health.ReceiveHealth(HealthRestore, targetCharacter.gameObject);
                applied = true;
            }
        }

        if (SpeedBoostAmount > 0f && SpeedBoostDuration > 0f)
        {
            // Speed boost stub — Feature 006 will implement full buff system
            Debug.Log($"[ConsumableItem] Speed boost x{SpeedBoostAmount} for {SpeedBoostDuration}s applied to {targetCharacter.name} (stub)");
            applied = true;
        }

        if (StaminaRestore > 0f)
        {
            // Stamina restore stub — reserved for future stamina system
            Debug.Log($"[ConsumableItem] Stamina restore {StaminaRestore} (stub)");
            applied = true;
        }

        return applied;
    }
}
