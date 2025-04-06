using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
public class VendorInventory : Inventory
{
    // Array of Inv Slots and Inv Images
    public GameObject[] vendorInvSlots;
    protected Image[] VendorInvImages;
    public TextMeshProUGUI[] VendorInvSlotCounts;

    public GameObject[] VendorinvCountBackground;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        GameObject player = GameObject.FindWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        
        
        if(playerHealth == null) {
            Debug.Log("player health be null");
        } else {
            Debug.Log("Player health found");
        }


        // Initialize the image array to match the size of invSlots
        invImages = new Image[invSlots.Length];
        VendorInvImages = new Image[vendorInvSlots.Length];


        for (int i = 0; i < invSlots.Length; i++)
        {
            if (invSlots[i] != null)
            {
                invImages[i] = invSlots[i].GetComponent<Image>();

                // Add a Button component if not already present
                if (invSlots[i].GetComponent<Button>() == null)
                {
                    invSlots[i].AddComponent<Button>();
                }

                // Add an OnClick listener to each slot
                int index = i; // Capture index in a local variable
                invSlots[i].GetComponent<Button>().onClick.AddListener(() => OnSlotClicked(index));
            }
        }

        for (int i = 0; i < vendorInvSlots.Length; i++)
        {
            if (vendorInvSlots[i] != null)
            {
                VendorInvImages[i] = vendorInvSlots[i].GetComponent<Image>();

                // Add a Button component if not already present
                if (vendorInvSlots[i].GetComponent<Button>() == null)
                {
                    vendorInvSlots[i].AddComponent<Button>();
                }

                // Add an OnClick listener to each slot
                int index = i; // Capture index in a local variable
                vendorInvSlots[i].GetComponent<Button>().onClick.AddListener(() => VendorOnSlotClicked(index));
            }
        }
    }

    void Update()
    {
        // Update coin text
        coinText.text = "Coins: " + PlayerLoot.GetItemAmount("coin").ToString();

        // Update inventory slots
        List<string> itemOrder = PlayerLoot.GetItemList();

        for (int i = 0; i < invImages.Length; i++)
        {
            UpdateSlot(invImages[i], invSlotCounts[i], invCountBackground[i], itemOrder, i);
        }

        // Update inventory slots
        List<string> vendoritemOrder = VendorLoot.GetItemList();

        for (int i = 0; i < VendorInvImages.Length; i++)
        {
            VendorUpdateSlot(VendorInvImages[i], VendorInvSlotCounts[i], VendorinvCountBackground[i], vendoritemOrder, i);
        }

        HandleRightClick();
    }

    protected override void HandleRightClick()
    {
        if (Input.GetMouseButtonDown(1)) // Right-click
        {
            Vector3 mousePosition = Input.mousePosition;

            for (int i = 0; i < invSlots.Length; i++)
            {
                // Get the Image component of the slot
                Image slotImage = invSlots[i]?.GetComponent<Image>();

                if (slotImage != null && slotImage.sprite != emptySprite)   
                {
                    RectTransform slotRect = invSlots[i].GetComponent<RectTransform>();
                    if (RectTransformUtility.RectangleContainsScreenPoint(slotRect, mousePosition))
                    {
                        ShowContextMenu(i, mousePosition);
                        break;
                    }
                }
            }

            for (int i = 0; i < vendorInvSlots.Length; i++)
            {
                // Get the Image component of the slot
                Image slotImage = vendorInvSlots[i]?.GetComponent<Image>();

                if (slotImage != null && slotImage.sprite != emptySprite)   
                {
                    RectTransform slotRect = vendorInvSlots[i].GetComponent<RectTransform>();
                    if (RectTransformUtility.RectangleContainsScreenPoint(slotRect, mousePosition))
                    {
                        VendorShowContextMenu(i, mousePosition);
                        break;
                    }
                }
            }
        }
    }

    protected override void ShowContextMenu(int slotIndex, Vector3 position) {

        Debug.Log($"Showing context menu for slot {slotIndex} at position {position}");

        if (activeContextMenu != null)
        {
            Destroy(activeContextMenu);
        }

        Transform playerInventorySlots = GameObject.Find("Player Inventory Slots")?.transform;

        activeContextMenu = Instantiate(contextMenuPrefab, playerInventorySlots);

        activeContextMenu.transform.position = position;

        Button[] buttons = activeContextMenu.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            switch (button.name)
            {
                case "UseButton":
                    button.gameObject.SetActive(true);
                    button.onClick.AddListener(() => UseItem(slotIndex));
                    break;
                case "DropButton":
                    button.gameObject.SetActive(false);
                    button.onClick.AddListener(() => DropItem(slotIndex));
                    break;
                case "SellButton":
                    button.gameObject.SetActive(true);
                    button.onClick.AddListener(() => SellItem(slotIndex));
                    break;
                case "BuyButton":
                    button.gameObject.SetActive(false);
                    break;
            }
        }
    }

    protected void VendorShowContextMenu(int slotIndex, Vector3 position) {

        Debug.Log($"Showing context menu for slot {slotIndex} at position {position}");

        if (activeContextMenu != null)
        {
            Destroy(activeContextMenu);
        }

        Transform VendorInventorySlots = GameObject.Find("Vendor Inventory Slots")?.transform;

        activeContextMenu = Instantiate(contextMenuPrefab, VendorInventorySlots);

        activeContextMenu.transform.position = position;

        Button[] buttons = activeContextMenu.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            switch (button.name)
            {
                case "UseButton":
                    button.gameObject.SetActive(false);
                    break;
                 case "DropButton":
                    button.gameObject.SetActive(false);
                    button.onClick.AddListener(() => DropItem(slotIndex));
                    break;
                case "BuyButton":
                    button.gameObject.SetActive(true);
                    button.onClick.AddListener(() => BuyItem(slotIndex));
                    break;
                case "SellButton":
                    button.gameObject.SetActive(false);
                    button.onClick.AddListener(() => SellItem(slotIndex));
                    break;
            }
        }
    }

    protected void BuyItem(int slotIndex)
    {
        Debug.Log($"Buying item in slot {slotIndex}");
        
        List<string> itemOrder = VendorLoot.GetItemList();
        string itemName = itemOrder[slotIndex]; // Get the item name in the slot

        // Get the Item object from the dictionary
        if (ItemList.items.TryGetValue(itemName, out Item item))
        {
            if (PlayerLoot.coinAmount >= item.Price)
            {
                AudioManager.Instance.PlaySound(AudioManager.Instance.acceptSound, transform.position);
                VendorLoot.RemoveItem(itemName, 1);
                PlayerLoot.IncreaseItem(itemName, 1);
                PlayerLoot.coinAmount -= item.Price;
                Destroy(activeContextMenu);
            } 
            else
            {
                AudioManager.Instance.PlaySound(AudioManager.Instance.declineSound, transform.position);
                Debug.Log($"Cant afford {item}");
            }
        }
        else
        {
            Debug.LogWarning($"Item '{itemName}' not found in ItemList.");
        }
    }   

    protected override void OnSlotClicked(int index)
    {
    // If the clicked slot is already selected, unselect it
    if (selectedSlotIndex == index)
    {
        HighlightSlot(index, false, false);
        selectedSlotIndex = -1; // Reset selection
        Debug.Log($"Slot {index} deselected");
    }
    else
    {

        ResetAllSlots();

        // Highlight the selected slot
        selectedSlotIndex = index;
        HighlightSlot(index, true, false);
        Debug.Log($"Slot {index} selected");
        }
    }
    
    protected void VendorOnSlotClicked(int index)
    {
    // If the clicked slot is already selected, unselect it
    if (selectedSlotIndex == index)
    {
        HighlightSlot(index, false, true);
        selectedSlotIndex = -1; // Reset selection
        Debug.Log($"Slot {index} deselected");
    }
    else
    {
        ResetAllSlots();

        // Highlight the selected slot
        selectedSlotIndex = index;
        HighlightSlot(index, true, true);
        Debug.Log($"Slot {index} selected");
        }
    }

    protected override void HighlightSlot(int index, bool isSelected, bool isVendor)
    {

        if (!isVendor) {

            if (invSlots[index] != null)
            {
                Image slotImage = invSlots[index].GetComponent<Image>();
                if (slotImage != null)
                {
                    // Change the slot's color to indicate selection
                    slotImage.color = isSelected ? Color.yellow : Color.white;
                }
        }
    
        } else {

            if (vendorInvSlots[index] != null)
            {
            Image slotImage = vendorInvSlots[index].GetComponent<Image>();
            if (slotImage != null)
            {
                // Change the slot's color to indicate selection
                slotImage.color = isSelected ? Color.yellow : Color.white;
            }                
            }
        }
    }


    private void ResetAllSlots()
    {
        // Reset all player slots
        for (int i = 0; i < invSlots.Length; i++)
        {
            HighlightSlot(i, false, false);
        }

        // Reset all vendor slots
        for (int i = 0; i < vendorInvSlots.Length; i++)
        {
            HighlightSlot(i, false, true);
        }
    }

    protected void VendorUpdateSlot(Image slotImage, TextMeshProUGUI slotCountText, GameObject slotCountBackground, List<string> itemOrder, int index)
    {
        if (slotImage == null || index >= itemOrder.Count || slotCountText == null)
        {
            slotImage.sprite = emptySprite; // Clear slot if no item
            slotCountText.enabled = false;
            slotCountBackground.SetActive(false);
        }
        else
        {
            slotCountText.enabled = true;
            slotCountBackground.SetActive(true);
            string itemName = itemOrder[index];
            int itemCount = VendorLoot.GetItemAmount(itemName);
            slotImage.sprite = GetSpriteForItem(itemName); // Get corresponding sprite
            slotCountText.text = itemCount.ToString();
        }

    } 

    
}