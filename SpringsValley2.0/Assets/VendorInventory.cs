using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
public class VendorInventory : Inventory
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
}
