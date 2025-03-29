using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using System.Data.Common;

public class Inventory : MonoBehaviour
{
    // Array of Inv Slots and Inv Images
    public GameObject[] invSlots;
    protected Image[] invImages;
    public TextMeshProUGUI[] invSlotCounts;

    public GameObject[] invCountBackground;

    public Sprite emptySprite;
    [SerializeField] private Sprite cornSprite;  
    [SerializeField] private Sprite wheatSprite; 

    [SerializeField] private Sprite healing_potionSprite; 
    public TextMeshProUGUI coinText;

    protected int selectedSlotIndex = -1; // Tracks the currently selected slot
    public GameObject contextMenuPrefab;
    protected GameObject activeContextMenu;

    protected PlayerHealth playerHealth;

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

        HandleRightClick();
    }

    protected void UpdateSlot(Image slotImage, TextMeshProUGUI slotCountText, GameObject slotCountBackground, List<string> itemOrder, int index)
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
            int itemCount = PlayerLoot.GetItemAmount(itemName);
            slotImage.sprite = GetSpriteForItem(itemName); // Get corresponding sprite
            slotCountText.text = itemCount.ToString();
        }

    }

    protected Sprite GetSpriteForItem(string itemName)
    {
        switch (itemName)
        {
            case "corn":
                return cornSprite;
            case "wheat":
                return wheatSprite;
            case "healing_potion":
                return healing_potionSprite;
            default:
                return null; // No sprite for unknown items
        }
    }

    protected virtual void OnSlotClicked(int index)
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
            // Reset all slots to white before highlighting the new one
            for (int i = 0; i < invSlots.Length; i++)
            {
                HighlightSlot(i, false, false);
            }

            // Highlight the selected slot
            selectedSlotIndex = index;
            HighlightSlot(index, true, false);
            Debug.Log($"Slot {index} selected");
        }
    }

    protected virtual void HighlightSlot(int index, bool isSelected, bool isVendor)
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
        } 
    }

    
protected virtual void HandleRightClick()
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
    }
}

    protected virtual void ShowContextMenu(int slotIndex, Vector3 position)
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
                    button.gameObject.SetActive(true);
                    button.onClick.AddListener(() => DropItem(slotIndex));
                    break;
                case "SellButton":
                    button.gameObject.SetActive(false);
                    break;
                case "BuyButton":
                    button.gameObject.SetActive(false);
                    break;
            }
        }
    }

    protected void UseItem(int slotIndex)
    {
        Debug.Log($"Using item in slot {slotIndex}");

        // Get the current item list
        List<string> itemOrder = PlayerLoot.GetItemList();

        string itemName = itemOrder[slotIndex]; // Get the item name in the slot

        // Ensure the slot index is valid
        if (slotIndex < itemOrder.Count)
        {
            // find the corrsponding item in item List
            if (ItemList.items.TryGetValue(itemName, out Item item))
            {
                if (item.UseAbility == "Heal") {
                    playerHealth.Heal(item.AbilityQuantity);
                    Debug.Log(PlayerHealth.health);
                }
            }
        }
        else
        {
            Debug.LogWarning("Invalid slot index or no item to drop!");
        }
        
        PlayerLoot.RemoveItem(itemName, 1);
        Destroy(activeContextMenu);
    }

    protected void DropItem(int slotIndex)
    {
        Debug.Log($"Dropping item in slot {slotIndex}");

            // Get the current item list
        List<string> itemOrder = PlayerLoot.GetItemList();

        // Ensure the slot index is valid
        if (slotIndex < itemOrder.Count)
        {
            string itemName = itemOrder[slotIndex]; // Get the item name in the slot
            PlayerLoot.DecreaseItem(itemName, 1);   // Decrease the item count by 1

            Debug.Log($"Dropped 1 {itemName}. Remaining: {PlayerLoot.GetItemAmount(itemName)}");
        }
        else
        {
            Debug.LogWarning("Invalid slot index or no item to drop!");
        }
        
        Destroy(activeContextMenu);
    }

    protected void SellItem(int slotIndex)
    {
        Debug.Log($"Selling item in slot {slotIndex}");
        
        // Removes the item
        List<string> itemOrder = PlayerLoot.GetItemList();
        string itemName = itemOrder[slotIndex]; // Get the item name in the slot
        PlayerLoot.RemoveItem(itemName, 1);   // Decrease the item count by 1
        
        // Gives some money
        PlayerLoot.coinAmount += 1;
        Destroy(activeContextMenu);
    }


}
