using MoreMountains.TopDownEngine;
using UnityEngine;

/// <summary>
/// Feature 003 — NPC Shop interactable zone.
///
/// Subclasses ButtonActivated (same pattern as WardrobeInteractable).
/// When the player presses interact inside the trigger zone, opens the ShopUI overlay.
///
/// Setup:
///   1. Attach to the NPC InteractionZone child (or the NPC root).
///   2. Add a SphereCollider (IsTrigger = true, radius ~2) on the same GO.
///   3. Assign TargetShopUI in the Inspector.
///   4. Set ButtonActivatedRequirement = Character, RequiresPlayerType = true,
///      UnlimitedActivations = true, ButtonPromptText = "Shop".
/// </summary>
[AddComponentMenu("CosmicColony/Environment/Shop Interactable")]
public class ShopInteractable : ButtonActivated
{
    [Header("Shop")]
    [Tooltip("The ShopUI component to open when the player activates this zone.")]
    public ShopUI TargetShopUI;

    protected override void ActivateZone()
    {
        base.ActivateZone();

        if (TargetShopUI != null)
            TargetShopUI.Open();
        else
            Debug.LogWarning("[ShopInteractable] TargetShopUI reference not set.");
    }
}
