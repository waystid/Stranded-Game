using UnityEngine;
using UnityEngine.UI;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;

/// <summary>
/// Always-visible 4-slot hotbar at the bottom of the screen.
/// Listens to MMInventoryEvents for live slot updates.
/// Keys 1-4 select slots: consumables are used, tools are equipped.
///
/// Setup: assign SlotIcons[4], SlotQuantities[4], SlotHighlights[4] in inspector.
/// </summary>
public class HotbarUI : MonoBehaviour, MMEventListener<MMInventoryEvent>
{
    [Header("Settings")]
    public string PlayerID = "Player1";
    public string HotbarInventoryName = "PlayerHotbarInventory";
    public int SlotCount = 4;

    [Header("Slot UI References")]
    [Tooltip("Image components for each slot icon (array length must equal SlotCount)")]
    public Image[] SlotIcons;

    [Tooltip("Text components for item quantity per slot")]
    public Text[] SlotQuantities;

    [Tooltip("Highlight GameObjects shown on the selected slot")]
    public GameObject[] SlotHighlights;

    private Inventory _hotbar;
    private int _selectedSlot = -1;

    private void Start()
    {
        _hotbar = Inventory.FindInventory(HotbarInventoryName, PlayerID);
        RefreshDisplay();
    }

    private void OnEnable()  { this.MMEventStartListening<MMInventoryEvent>(); }
    private void OnDisable() { this.MMEventStopListening<MMInventoryEvent>(); }

    public void OnMMEvent(MMInventoryEvent inventoryEvent)
    {
        if (inventoryEvent.TargetInventoryName == HotbarInventoryName)
        {
            RefreshDisplay();
        }
    }

    private void Update()
    {
        for (int i = 0; i < SlotCount; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectSlot(i);
            }
        }
    }

    private void SelectSlot(int index)
    {
        _selectedSlot = index;
        UpdateHighlights();

        if (_hotbar == null) return;
        if (index >= _hotbar.Content.Length) return;

        InventoryItem item = _hotbar.Content[index];
        if (InventoryItem.IsNull(item)) return;

        if (item.IsUsable)
        {
            _hotbar.UseItem(item, index);
        }
        else if (item.IsEquippable)
        {
            _hotbar.EquipItem(item, index);
        }
    }

    private void UpdateHighlights()
    {
        for (int i = 0; i < SlotCount && i < SlotHighlights.Length; i++)
        {
            if (SlotHighlights[i] != null)
                SlotHighlights[i].SetActive(i == _selectedSlot);
        }
    }

    private void RefreshDisplay()
    {
        if (_hotbar == null)
        {
            _hotbar = Inventory.FindInventory(HotbarInventoryName, PlayerID);
            if (_hotbar == null) return;
        }

        for (int i = 0; i < SlotCount; i++)
        {
            bool hasIcon = i < SlotIcons.Length && SlotIcons[i] != null;
            bool hasQty  = i < SlotQuantities.Length && SlotQuantities[i] != null;

            if (i >= _hotbar.Content.Length)
            {
                if (hasIcon) SlotIcons[i].sprite = null;
                if (hasQty)  SlotQuantities[i].text = "";
                continue;
            }

            InventoryItem item = _hotbar.Content[i];
            bool empty = InventoryItem.IsNull(item);

            if (hasIcon) SlotIcons[i].sprite = empty ? null : item.Icon;
            if (hasQty)  SlotQuantities[i].text = (!empty && item.MaximumStack > 1) ? item.Quantity.ToString() : "";
        }
    }
}
