using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VendorInventory : Inventory
{
    public GameObject[] vendorInvSlots;
    protected Image[] VendorInvImages;
    public TextMeshProUGUI[] VendorInvSlotCounts;
    public GameObject[] VendorinvCountBackground;
    public GameObject textDescription;

    [SerializeField] private VendorLoot vendorLoot;

    private int selectedVendorSlotIndex = -1;
    private TextMeshProUGUI[] playerPriceTexts;
    private TextMeshProUGUI[] vendorPriceTexts;

    protected override void Start()
    {
        base.Start();
        ResolveVendorLoot();
        InitialiseVendorSlots();
        InitialisePriceLabels();
        SetDescription(null);
        SubscribeToVendorStock();
        RefreshInventory();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        ResolveVendorLoot();
        SubscribeToVendorStock();
        RefreshInventory();
    }

    protected override void OnDisable()
    {
        if (vendorLoot != null)
        {
            vendorLoot.StockChanged -= RefreshInventory;
        }

        base.OnDisable();
    }

    protected override void RefreshInventory()
    {
        base.RefreshInventory();

        List<string> playerItems = PlayerLoot.GetItemList();
        UpdatePriceLabels(playerPriceTexts, playerItems, false);

        if (VendorInvImages == null || vendorLoot == null)
        {
            UpdatePriceLabels(vendorPriceTexts, null, true);
            return;
        }

        List<string> vendorItems = vendorLoot.GetItemList();
        for (int index = 0; index < VendorInvImages.Length; index++)
        {
            VendorUpdateSlot(
                VendorInvImages[index],
                GetArrayValue(VendorInvSlotCounts, index),
                GetArrayValue(VendorinvCountBackground, index),
                vendorItems,
                index);
        }

        UpdatePriceLabels(vendorPriceTexts, vendorItems, true);

        if (selectedVendorSlotIndex >= vendorItems.Count)
        {
            selectedVendorSlotIndex = -1;
            SetDescription(null);
            ResetAllSlots();
        }
    }

    protected override void HandleRightClick()
    {
        if (!Input.GetMouseButtonDown(1))
        {
            return;
        }

        Vector3 mousePosition = Input.mousePosition;
        List<string> playerItems = PlayerLoot.GetItemList();
        if (TryOpenMenuAtPoint(invSlots, playerItems.Count, mousePosition, ShowContextMenu))
        {
            return;
        }

        int vendorItemCount = vendorLoot != null ? vendorLoot.GetItemList().Count : 0;
        TryOpenMenuAtPoint(vendorInvSlots, vendorItemCount, mousePosition, VendorShowContextMenu);
    }

    protected override void ShowContextMenu(int slotIndex, Vector3 position)
    {
        List<string> playerItems = PlayerLoot.GetItemList();
        if (slotIndex < 0 || slotIndex >= playerItems.Count || contextMenuPrefab == null ||
            !ItemList.items.TryGetValue(playerItems[slotIndex], out Item item))
        {
            return;
        }

        CloseContextMenu();
        activeContextMenu = Instantiate(contextMenuPrefab, transform);
        activeContextMenu.transform.position = position;

        foreach (Button button in activeContextMenu.GetComponentsInChildren<Button>(true))
        {
            switch (button.name)
            {
                case "UseButton":
                    button.gameObject.SetActive(true);
                    button.onClick.AddListener(() => UseItem(slotIndex));
                    break;
                case "DropButton":
                    button.gameObject.SetActive(false);
                    break;
                case "SellButton":
                    button.gameObject.SetActive(true);
                    SetButtonText(button, $"Sell (+{item.SellPrice} coins)");
                    button.onClick.AddListener(() => SellItem(slotIndex));
                    break;
                case "BuyButton":
                    button.gameObject.SetActive(false);
                    break;
                case "CloseButton":
                    button.onClick.AddListener(CloseContextMenu);
                    break;
            }
        }
    }

    protected override void OnSlotClicked(int index)
    {
        List<string> playerItems = PlayerLoot.GetItemList();
        if (index < 0 || index >= playerItems.Count)
        {
            selectedSlotIndex = -1;
            ResetAllSlots();
            return;
        }

        bool wasSelected = selectedSlotIndex == index && selectedVendorSlotIndex < 0;
        ResetAllSlots();
        selectedVendorSlotIndex = -1;
        selectedSlotIndex = wasSelected ? -1 : index;

        if (!wasSelected)
        {
            HighlightSlot(index, true, false);
            PlayAcceptSound();
        }

        SetDescription(null);
    }

    protected override void HighlightSlot(int index, bool isSelected, bool isVendor)
    {
        if (!isVendor)
        {
            base.HighlightSlot(index, isSelected, false);
            return;
        }

        if (vendorInvSlots == null || index < 0 || index >= vendorInvSlots.Length || vendorInvSlots[index] == null)
        {
            return;
        }

        Image slotImage = vendorInvSlots[index].GetComponent<Image>();
        if (slotImage != null)
        {
            slotImage.color = isSelected ? Color.yellow : Color.white;
        }
    }

    protected override void SellItem(int slotIndex)
    {
        List<string> playerItems = PlayerLoot.GetItemList();
        if (vendorLoot == null || slotIndex < 0 || slotIndex >= playerItems.Count)
        {
            PlayDeclineSound();
            CloseContextMenu();
            return;
        }

        string itemName = playerItems[slotIndex];
        if (!ItemList.items.TryGetValue(itemName, out Item item) || !PlayerLoot.RemoveItem(itemName, 1))
        {
            PlayDeclineSound();
            CloseContextMenu();
            return;
        }

        vendorLoot.IncreaseItem(itemName, 1);
        PlayerLoot.AddCoins(item.SellPrice);
        PlayAcceptSound();
        CloseContextMenu();
    }

    private void InitialiseVendorSlots()
    {
        int slotCount = vendorInvSlots != null ? vendorInvSlots.Length : 0;
        VendorInvImages = new Image[slotCount];

        for (int index = 0; index < slotCount; index++)
        {
            GameObject slot = vendorInvSlots[index];
            if (slot == null)
            {
                continue;
            }

            VendorInvImages[index] = slot.GetComponent<Image>();
            Button slotButton = slot.GetComponent<Button>();
            if (slotButton == null)
            {
                slotButton = slot.AddComponent<Button>();
            }

            int capturedIndex = index;
            slotButton.onClick.AddListener(() => VendorOnSlotClicked(capturedIndex));
        }
    }

    private void VendorOnSlotClicked(int index)
    {
        List<string> vendorItems = vendorLoot != null ? vendorLoot.GetItemList() : new List<string>();
        if (index < 0 || index >= vendorItems.Count)
        {
            selectedVendorSlotIndex = -1;
            ResetAllSlots();
            SetDescription(null);
            return;
        }

        bool wasSelected = selectedVendorSlotIndex == index && selectedSlotIndex < 0;
        ResetAllSlots();
        selectedSlotIndex = -1;
        selectedVendorSlotIndex = wasSelected ? -1 : index;

        if (wasSelected)
        {
            SetDescription(null);
            return;
        }

        HighlightSlot(index, true, true);
        string itemName = vendorItems[index];
        SetDescription(ItemList.items.TryGetValue(itemName, out Item item) ? item.Description : null);
        PlayAcceptSound();
    }

    private void VendorShowContextMenu(int slotIndex, Vector3 position)
    {
        List<string> vendorItems = vendorLoot != null ? vendorLoot.GetItemList() : new List<string>();
        if (slotIndex < 0 || slotIndex >= vendorItems.Count || contextMenuPrefab == null ||
            !ItemList.items.TryGetValue(vendorItems[slotIndex], out Item item))
        {
            return;
        }

        CloseContextMenu();
        activeContextMenu = Instantiate(contextMenuPrefab, transform);
        activeContextMenu.transform.position = position;

        foreach (Button button in activeContextMenu.GetComponentsInChildren<Button>(true))
        {
            switch (button.name)
            {
                case "UseButton":
                case "DropButton":
                case "SellButton":
                    button.gameObject.SetActive(false);
                    break;
                case "BuyButton":
                    button.gameObject.SetActive(true);
                    SetButtonText(button, $"Buy ({item.BuyPrice} coins)");
                    button.onClick.AddListener(() => BuyItem(slotIndex));
                    break;
                case "CloseButton":
                    button.onClick.AddListener(CloseContextMenu);
                    break;
            }
        }
    }

    private void BuyItem(int slotIndex)
    {
        List<string> vendorItems = vendorLoot != null ? vendorLoot.GetItemList() : new List<string>();
        if (slotIndex < 0 || slotIndex >= vendorItems.Count)
        {
            PlayDeclineSound();
            CloseContextMenu();
            return;
        }

        string itemName = vendorItems[slotIndex];
        if (!ItemList.items.TryGetValue(itemName, out Item item) ||
            vendorLoot.GetItemAmount(itemName) < 1 ||
            !PlayerLoot.TrySpendCoins(item.BuyPrice))
        {
            PlayDeclineSound();
            CloseContextMenu();
            return;
        }

        if (!vendorLoot.RemoveItem(itemName, 1))
        {
            PlayerLoot.AddCoins(item.BuyPrice);
            PlayDeclineSound();
            CloseContextMenu();
            return;
        }

        PlayerLoot.IncreaseItem(itemName, 1);
        PlayAcceptSound();
        CloseContextMenu();
    }

    private void VendorUpdateSlot(
        Image slotImage,
        TextMeshProUGUI slotCountText,
        GameObject slotCountBackground,
        List<string> itemOrder,
        int index)
    {
        if (slotImage == null)
        {
            return;
        }

        bool hasItem = itemOrder != null && index >= 0 && index < itemOrder.Count;
        slotImage.sprite = hasItem ? GetSpriteForItem(itemOrder[index]) : emptySprite;

        if (slotCountText != null)
        {
            slotCountText.enabled = hasItem;
            slotCountText.text = hasItem ? vendorLoot.GetItemAmount(itemOrder[index]).ToString() : string.Empty;
        }

        if (slotCountBackground != null)
        {
            slotCountBackground.SetActive(hasItem);
        }
    }

    private bool TryOpenMenuAtPoint(
        GameObject[] slots,
        int populatedSlotCount,
        Vector3 mousePosition,
        System.Action<int, Vector3> showMenu)
    {
        if (slots == null)
        {
            return false;
        }

        for (int index = 0; index < slots.Length && index < populatedSlotCount; index++)
        {
            RectTransform slotRect = slots[index] != null ? slots[index].GetComponent<RectTransform>() : null;
            if (slotRect != null && RectTransformUtility.RectangleContainsScreenPoint(slotRect, mousePosition))
            {
                showMenu(index, mousePosition);
                return true;
            }
        }

        return false;
    }

    private void ResetAllSlots()
    {
        ResetPlayerSlotHighlights();
        if (vendorInvSlots == null)
        {
            return;
        }

        for (int index = 0; index < vendorInvSlots.Length; index++)
        {
            HighlightSlot(index, false, true);
        }
    }

    private void InitialisePriceLabels()
    {
        playerPriceTexts = CreatePriceLabels(invSlots, invSlotCounts, "Sell Price");
        vendorPriceTexts = CreatePriceLabels(vendorInvSlots, VendorInvSlotCounts, "Buy Price");
    }

    private static TextMeshProUGUI[] CreatePriceLabels(
        GameObject[] slots,
        TextMeshProUGUI[] countTexts,
        string labelName)
    {
        int slotCount = slots != null ? slots.Length : 0;
        TextMeshProUGUI[] labels = new TextMeshProUGUI[slotCount];

        for (int index = 0; index < slotCount; index++)
        {
            GameObject slot = slots[index];
            if (slot == null)
            {
                continue;
            }

            Transform existingLabel = slot.transform.Find(labelName);
            TextMeshProUGUI priceText = existingLabel != null
                ? existingLabel.GetComponent<TextMeshProUGUI>()
                : null;

            if (priceText == null)
            {
                GameObject labelObject = new GameObject(labelName, typeof(RectTransform));
                labelObject.transform.SetParent(slot.transform, false);

                RectTransform labelRect = labelObject.GetComponent<RectTransform>();
                labelRect.anchorMin = new Vector2(0f, 1f);
                labelRect.anchorMax = new Vector2(1f, 1f);
                labelRect.pivot = new Vector2(0.5f, 1f);
                labelRect.anchoredPosition = new Vector2(0f, -2f);
                labelRect.sizeDelta = new Vector2(0f, 18f);

                priceText = labelObject.AddComponent<TextMeshProUGUI>();
            }

            TextMeshProUGUI countTemplate = GetArrayValue(countTexts, index);
            if (countTemplate != null)
            {
                priceText.font = countTemplate.font;
            }
            else if (TMP_Settings.defaultFontAsset != null)
            {
                priceText.font = TMP_Settings.defaultFontAsset;
            }

            priceText.text = string.Empty;
            priceText.fontSize = 16f;
            priceText.enableAutoSizing = true;
            priceText.fontSizeMin = 10f;
            priceText.fontSizeMax = 18f;
            priceText.fontStyle = FontStyles.Bold;
            priceText.alignment = TextAlignmentOptions.Top;
            priceText.color = Color.white;
            priceText.outlineColor = Color.black;
            priceText.outlineWidth = 0.2f;
            priceText.textWrappingMode = TextWrappingModes.NoWrap;
            priceText.overflowMode = TextOverflowModes.Overflow;
            priceText.raycastTarget = false;
            labels[index] = priceText;
        }

        return labels;
    }

    private static void UpdatePriceLabels(TextMeshProUGUI[] labels, List<string> itemOrder, bool isBuyPrice)
    {
        if (labels == null)
        {
            return;
        }

        int playerCoins = PlayerLoot.GetItemAmount("coin");
        for (int index = 0; index < labels.Length; index++)
        {
            TextMeshProUGUI priceText = labels[index];
            if (priceText == null)
            {
                continue;
            }

            Item item = null;
            bool hasItem = itemOrder != null && index < itemOrder.Count &&
                ItemList.items.TryGetValue(itemOrder[index], out item);
            if (!hasItem)
            {
                priceText.text = string.Empty;
                priceText.enabled = false;
                continue;
            }

            int price = isBuyPrice ? item.BuyPrice : item.SellPrice;
            priceText.text = isBuyPrice ? $"Buy {price}" : $"Sell {price}";
            priceText.color = isBuyPrice && playerCoins < price
                ? new Color(1f, 0.35f, 0.3f)
                : new Color(1f, 0.9f, 0.35f);
            priceText.enabled = true;
        }
    }

    private static void SetButtonText(Button button, string label)
    {
        TextMeshProUGUI buttonText = button != null
            ? button.GetComponentInChildren<TextMeshProUGUI>(true)
            : null;
        if (buttonText != null)
        {
            buttonText.text = label;
        }
    }

    private void SetDescription(string description)
    {
        if (textDescription == null)
        {
            return;
        }

        bool hasDescription = !string.IsNullOrEmpty(description);
        textDescription.SetActive(hasDescription);
        TextMeshProUGUI descriptionText = textDescription.GetComponent<TextMeshProUGUI>();
        if (descriptionText == null)
        {
            descriptionText = textDescription.GetComponentInChildren<TextMeshProUGUI>(true);
        }

        if (descriptionText != null)
        {
            descriptionText.text = description ?? string.Empty;
        }
    }

    private void ResolveVendorLoot()
    {
        if (vendorLoot != null)
        {
            return;
        }

        vendorLoot = GetComponent<VendorLoot>();
        if (vendorLoot == null)
        {
            vendorLoot = GetComponentInParent<VendorLoot>(true);
        }

        if (vendorLoot == null)
        {
            vendorLoot = GetComponentInChildren<VendorLoot>(true);
        }

        if (vendorLoot == null)
        {
            Debug.LogWarning("VendorInventory needs a VendorLoot component for this vendor's stock.", this);
        }
    }

    private void SubscribeToVendorStock()
    {
        if (vendorLoot == null)
        {
            return;
        }

        vendorLoot.StockChanged -= RefreshInventory;
        vendorLoot.StockChanged += RefreshInventory;
    }
}
