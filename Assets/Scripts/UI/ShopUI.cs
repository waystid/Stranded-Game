using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.InventoryEngine;

/// <summary>
/// Canvas overlay UI for the NPC shop system (Feature 003).
///
/// Open/Close: called by ShopInteractable when the player activates the NPC zone.
/// Pauses time on open (timeScale=0), resumes on close.
///
/// Buy: checks player's Stardust inventory balance, deducts Stardust, adds item.
/// Sell: removes item from player inventory, grants Stardust.
///
/// Setup:
///   1. Attach to the same GO as a CanvasGroup (alpha=0 closed, 1 open).
///   2. Assign ShopInventoryData in Inspector.
///   3. Assign StardustItemID to match the Stardust CosmicItem.ItemID.
///   4. Connect Buy/Sell button onClick events per entry (auto-built at runtime).
///   5. ShopInteractable wires TargetShopUI = this component.
/// </summary>
[AddComponentMenu("CosmicColony/UI/ShopUI")]
public class ShopUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The CanvasGroup on the shop panel (controls visibility + blocking).")]
    public CanvasGroup ShopPanel;

    [Tooltip("The ShopInventory ScriptableObject with stock + prices.")]
    public ShopInventory ShopInventoryData;

    [Header("Stardust")]
    [Tooltip("ItemID of the Stardust currency item (must match CosmicItem.ItemID).")]
    public string StardustItemID = "stardust";

    [Tooltip("Inventory name that holds Stardust (usually the main inventory).")]
    public string MainInventoryName = "PlayerMainInventory";

    [Header("Item Registry")]
    [Tooltip("Assign ItemRegistry.asset for Stardust lookup during Sell transactions.")]
    public ItemRegistry Registry;

    [Header("Layout (optional)")]
    [Tooltip("Scroll content parent where shop row entries are spawned.")]
    public Transform EntryContainer;

    [Tooltip("Prefab for a single shop row (must have two Button children: Buy / Sell).")]
    public GameObject EntryRowPrefab;

    // ── Runtime ───────────────────────────────────────────────────────────────

    public bool IsOpen { get; private set; }

    private List<GameObject> _rows = new List<GameObject>();

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    void Start()
    {
        if (ShopPanel != null)
        {
            ShopPanel.alpha          = 0f;
            ShopPanel.interactable   = false;
            ShopPanel.blocksRaycasts = false;
        }
    }

    // ── Public API ────────────────────────────────────────────────────────────

    public void Open()
    {
        if (IsOpen) return;
        IsOpen = true;
        Time.timeScale = 0f;
        SetPanelVisible(true);
        PopulateRows();
        Debug.Log("[ShopUI] Shop opened.");
    }

    public void Close()
    {
        if (!IsOpen) return;
        IsOpen = false;
        Time.timeScale = 1f;
        SetPanelVisible(false);
        ClearRows();
        Debug.Log("[ShopUI] Shop closed.");
    }

    /// <summary>
    /// Buy qty units of the item from the shop.
    /// Deducts Stardust from player inventory and adds the item.
    /// </summary>
    public void Buy(CosmicItem item, int qty = 1)
    {
        if (ShopInventoryData == null) return;
        var entry = ShopInventoryData.GetEntry(item.ItemID);
        if (entry == null)
        {
            Debug.LogWarning($"[ShopUI] Item '{item.ItemID}' not in shop stock.");
            return;
        }

        int totalCost = entry.Value.StardustBuyPrice * qty;
        Inventory mainInv = GetMainInventory();
        if (mainInv == null) return;

        // Check Stardust balance
        int balance = CountItem(mainInv, StardustItemID);
        if (balance < totalCost)
        {
            Debug.Log($"[ShopUI] Not enough Stardust. Need {totalCost}, have {balance}.");
            return;
        }

        // Deduct Stardust
        RemoveItems(mainInv, StardustItemID, totalCost);

        // Add purchased item
        for (int i = 0; i < qty; i++)
            mainInv.AddItem(item, 1);

        Debug.Log($"[ShopUI] Bought {qty}x {item.ItemName} for {totalCost} Stardust.");
    }

    /// <summary>
    /// Sell qty units of the item from player inventory.
    /// Grants Stardust in return.
    /// </summary>
    public void Sell(CosmicItem item, int qty = 1)
    {
        if (ShopInventoryData == null) return;
        var entry = ShopInventoryData.GetEntry(item.ItemID);
        if (entry == null || entry.Value.StardustSellPrice <= 0)
        {
            Debug.Log($"[ShopUI] '{item.ItemID}' cannot be sold here.");
            return;
        }

        Inventory mainInv = GetMainInventory();
        if (mainInv == null) return;

        int owned = CountItem(mainInv, item.ItemID);
        if (owned < qty)
        {
            Debug.Log($"[ShopUI] Not enough '{item.ItemName}' to sell. Have {owned}, need {qty}.");
            return;
        }

        RemoveItems(mainInv, item.ItemID, qty);

        // Grant Stardust — look up via assigned Registry
        CosmicItem stardustItem = Registry != null ? Registry.Get(StardustItemID) : null;
        if (stardustItem != null)
        {
            int earned = entry.Value.StardustSellPrice * qty;
            mainInv.AddItem(stardustItem, earned);
            Debug.Log($"[ShopUI] Sold {qty}x {item.ItemName} for {earned} Stardust.");
        }
    }

    // ── Private Helpers ───────────────────────────────────────────────────────

    private void SetPanelVisible(bool visible)
    {
        if (ShopPanel == null) return;
        ShopPanel.alpha          = visible ? 1f : 0f;
        ShopPanel.interactable   = visible;
        ShopPanel.blocksRaycasts = visible;
    }

    private void PopulateRows()
    {
        if (ShopInventoryData == null || EntryContainer == null || EntryRowPrefab == null)
        {
            Debug.Log("[ShopUI] EntryContainer or EntryRowPrefab not assigned — skipping row build.");
            return;
        }

        ClearRows();
        foreach (var entry in ShopInventoryData.Stock)
        {
            if (entry.Item == null) continue;
            var row = Instantiate(EntryRowPrefab, EntryContainer);
            _rows.Add(row);

            // Find label
            var labels = row.GetComponentsInChildren<Text>();
            if (labels.Length > 0)
                labels[0].text = $"{entry.Item.ItemName}  —  {entry.StardustBuyPrice}⭐";

            // Wire Buy button (first Button child)
            var buttons = row.GetComponentsInChildren<Button>();
            CosmicItem capturedItem = entry.Item;
            if (buttons.Length > 0)
                buttons[0].onClick.AddListener(() => Buy(capturedItem, 1));
            if (buttons.Length > 1)
                buttons[1].onClick.AddListener(() => Sell(capturedItem, 1));
        }
    }

    private void ClearRows()
    {
        foreach (var row in _rows)
            if (row != null) Destroy(row);
        _rows.Clear();
    }

    private Inventory GetMainInventory()
    {
        var invs = Object.FindObjectsOfType<Inventory>();
        foreach (var inv in invs)
            if (inv.name == MainInventoryName)
                return inv;
        Debug.LogWarning($"[ShopUI] Inventory '{MainInventoryName}' not found.");
        return null;
    }

    private int CountItem(Inventory inv, string itemID)
    {
        int count = 0;
        if (inv?.Content == null) return 0;
        foreach (var slot in inv.Content)
            if (slot != null && slot.ItemID == itemID)
                count += slot.Quantity;
        return count;
    }

    private void RemoveItems(Inventory inv, string itemID, int qty)
    {
        int remaining = qty;
        if (inv?.Content == null) return;
        for (int i = 0; i < inv.Content.Length && remaining > 0; i++)
        {
            var slot = inv.Content[i];
            if (slot == null || slot.ItemID != itemID) continue;
            int take = Mathf.Min(slot.Quantity, remaining);
            inv.RemoveItem(i, take);
            remaining -= take;
        }
    }
}
