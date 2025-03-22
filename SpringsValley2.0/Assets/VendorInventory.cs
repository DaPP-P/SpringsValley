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
            UpdateSlot(VendorInvImages[i], VendorInvSlotCounts[i], VendorinvCountBackground[i], vendoritemOrder, i);
        }

        HandleRightClick();
    }

    protected override void ShowContextMenu(int slotIndex, Vector3 position)

    {
        if (activeContextMenu != null)
        {
            Destroy(activeContextMenu);
        }

        activeContextMenu = Instantiate(contextMenuPrefab, transform);

        activeContextMenu.transform.position = position;

        Button[] buttons = activeContextMenu.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            switch (button.name)
            {
                case "UseButton":
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
            }
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

    
}