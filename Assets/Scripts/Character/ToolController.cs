using UnityEngine;
using MoreMountains.TopDownEngine;

/// <summary>
/// CharacterAbility that manages the currently equipped tool.
/// Called by ToolItem.Equip()/UnEquip() and HotbarUI when a tool slot is selected.
/// Feature 006 (Tool Actions) will extend HandleInput() to map tool use to grid actions.
/// </summary>
public class ToolController : CharacterAbility
{
    [Header("Tool Attach")]
    [Tooltip("The hand bone transform where the tool prop model is parented (prop_r)")]
    public Transform ToolAttachPoint;

    /// <summary>The tool currently held by the player. Null if none.</summary>
    public ToolItem CurrentTool { get; private set; }

    private GameObject _toolProp;

    /// <summary>
    /// Equip a tool: destroy any previous prop, spawn a placeholder cylinder at the attach point.
    /// Feature 006 will spawn the real tool model from ToolItem.AnimatorControllerOverridePath.
    /// </summary>
    public void EquipTool(ToolItem tool)
    {
        UnequipTool();
        CurrentTool = tool;

        if (ToolAttachPoint != null)
        {
            _toolProp = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            _toolProp.name = $"[ToolProp] {tool.ItemName}";
            _toolProp.transform.SetParent(ToolAttachPoint, false);
            _toolProp.transform.localPosition = new Vector3(0f, 0.2f, 0f);
            _toolProp.transform.localScale = new Vector3(0.05f, 0.2f, 0.05f);

            // Remove collider so the prop doesn't block physics
            Destroy(_toolProp.GetComponent<Collider>());
        }

        Debug.Log($"[ToolController] Equipped: {tool.ItemName}");
    }

    /// <summary>
    /// Unequip the current tool and destroy its prop.
    /// </summary>
    public void UnequipTool()
    {
        if (_toolProp != null)
        {
            Destroy(_toolProp);
            _toolProp = null;
        }
        if (CurrentTool != null)
        {
            Debug.Log($"[ToolController] Unequipped: {CurrentTool.ItemName}");
            CurrentTool = null;
        }
    }
}
