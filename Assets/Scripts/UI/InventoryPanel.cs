using UnityEngine;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;

/// <summary>
/// Full inventory grid panel (4x8 = 32 slots).
/// Press I (configurable) to toggle open/close.
/// Uses TDE's InventoryDisplay component for the actual slot rendering —
/// this script only manages panel visibility.
///
/// Setup: assign PanelGroup (CanvasGroup on the panel root).
/// Add a TDE InventoryDisplay component on this GO (or a child) pointing to "PlayerMainInventory".
/// </summary>
public class InventoryPanel : MonoBehaviour, MMEventListener<MMInventoryEvent>
{
    [Header("Settings")]
    public string PlayerID = "Player1";
    public string MainInventoryName = "PlayerMainInventory";

    [Tooltip("Key to toggle inventory open/close")]
    public KeyCode ToggleKey = KeyCode.I;

    [Header("References")]
    [Tooltip("CanvasGroup on the inventory panel root — controls visibility + interactivity")]
    public CanvasGroup PanelGroup;

    private bool _isOpen;

    private void Start()
    {
        SetVisible(false);
    }

    private void OnEnable()  { this.MMEventStartListening<MMInventoryEvent>(); }
    private void OnDisable() { this.MMEventStopListening<MMInventoryEvent>(); }

    public void OnMMEvent(MMInventoryEvent inventoryEvent)
    {
        // InventoryDisplay handles its own slot refresh via MMEvents automatically.
        // This handler is a hook for future auto-open behaviour if needed.
    }

    private void Update()
    {
        if (Input.GetKeyDown(ToggleKey))
        {
            Toggle();
        }
    }

    public void Toggle()  { SetVisible(!_isOpen); }
    public void Open()    { SetVisible(true); }
    public void Close()   { SetVisible(false); }

    private void SetVisible(bool visible)
    {
        _isOpen = visible;
        if (PanelGroup == null) return;

        PanelGroup.alpha = visible ? 1f : 0f;
        PanelGroup.interactable = visible;
        PanelGroup.blocksRaycasts = visible;
    }
}
