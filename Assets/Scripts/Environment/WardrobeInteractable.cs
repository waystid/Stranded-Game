using MoreMountains.TopDownEngine;
using UnityEngine;

/// <summary>
/// Feature 007 Phase E — In-world wardrobe / mirror interactable.
///
/// Subclasses ButtonActivated. When the player presses the interact button
/// while inside the trigger zone, ActivateZone() opens the WardrobeUI overlay.
///
/// Setup:
///   1. Attach this component to the InteractionZone child of WardrobeMirror.
///   2. Add a SphereCollider (IsTrigger = true, radius ~1.5) on the same GO.
///   3. Assign WardrobeUI in the Inspector.
///   4. Set ButtonActivatedRequirement = Character, RequiresPlayerType = true,
///      UnlimitedActivations = true, ButtonPromptText = "Wardrobe".
/// </summary>
[AddComponentMenu("CosmicColony/Environment/Wardrobe Interactable")]
public class WardrobeInteractable : ButtonActivated
{
    [Header("Wardrobe")]
    [Tooltip("The WardrobeUI component to open when the player activates this zone.")]
    public WardrobeUI WardrobeUI;

    protected override void ActivateZone()
    {
        base.ActivateZone();

        if (WardrobeUI != null)
            WardrobeUI.Open();
        else
            Debug.LogWarning("[WardrobeInteractable] WardrobeUI reference not set.");
    }
}
