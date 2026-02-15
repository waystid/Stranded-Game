using UnityEngine;
using System.Collections;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;

/// <summary>
/// TreeShakeZone - Extends ButtonActivated to create an interactable tree that shakes and drops loot
/// Used for resource gathering mechanics similar to Animal Crossing tree shaking
/// </summary>
public class TreeShakeZone : ButtonActivated
{
    [Header("Tree Shake Settings")]
    [Tooltip("The transform of the tree model that will be animated")]
    public Transform TreeModel;

    [Tooltip("Duration of the shake animation in seconds")]
    public float ShakeDuration = 0.5f;

    [Tooltip("Intensity of the shake (rotation angle in degrees)")]
    public float ShakeIntensity = 10f;

    [Tooltip("Cooldown time in seconds before the tree can be shaken again")]
    public float ShakeCooldown = 5f;

    [Header("Loot Settings")]
    [Tooltip("The Loot component that handles item drops")]
    public Loot LootComponent;

    // Internal state
    private bool _isShaking = false;
    private bool _onCooldown = false;
    private Quaternion _originalRotation;

    /// <summary>
    /// Initialization - store the original rotation of the tree
    /// </summary>
    protected override void Initialization()
    {
        base.Initialization();

        if (TreeModel != null)
        {
            _originalRotation = TreeModel.localRotation;
        }
        else
        {
            Debug.LogWarning("TreeShakeZone: TreeModel is not assigned!", this);
        }

        if (LootComponent == null)
        {
            Debug.LogWarning("TreeShakeZone: LootComponent is not assigned!", this);
        }
    }

    /// <summary>
    /// Override the activation method to implement shake behavior
    /// </summary>
    public override void ActivateZone()
    {
        // Prevent activation if already shaking or on cooldown
        if (_isShaking || _onCooldown)
        {
            return;
        }

        // Call base method for standard ButtonActivated behavior
        base.ActivateZone();

        // Start the shake sequence
        StartCoroutine(ShakeSequence());
    }

    /// <summary>
    /// Coroutine that handles the shake animation and loot spawning
    /// </summary>
    private IEnumerator ShakeSequence()
    {
        _isShaking = true;

        // Animation phase: Shake the tree back and forth
        float elapsed = 0f;

        while (elapsed < ShakeDuration)
        {
            elapsed += Time.deltaTime;

            // Calculate shake using a sine wave for smooth oscillation
            float progress = elapsed / ShakeDuration;
            float shakeAmount = Mathf.Sin(progress * Mathf.PI * 8) * ShakeIntensity * (1 - progress);

            // Apply rotation shake on the Z-axis
            if (TreeModel != null)
            {
                TreeModel.localRotation = _originalRotation * Quaternion.Euler(0f, 0f, shakeAmount);
            }

            yield return null;
        }

        // Reset to original rotation
        if (TreeModel != null)
        {
            TreeModel.localRotation = _originalRotation;
        }

        // Spawn loot after shake completes
        if (LootComponent != null)
        {
            LootComponent.SpawnLoot();
        }

        _isShaking = false;

        // Start cooldown
        StartCoroutine(CooldownTimer());
    }

    /// <summary>
    /// Cooldown timer to prevent rapid re-activation
    /// </summary>
    private IEnumerator CooldownTimer()
    {
        _onCooldown = true;
        yield return new WaitForSeconds(ShakeCooldown);
        _onCooldown = false;
    }

    /// <summary>
    /// Optional: Visual feedback for when the zone is on cooldown
    /// Override to customize prompt text
    /// </summary>
    public override string GetPromptText()
    {
        if (_onCooldown)
        {
            return "Tree is settling...";
        }
        return base.GetPromptText();
    }
}
