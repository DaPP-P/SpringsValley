using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{

    // Array of Inv Slots and Inv Images
    public GameObject[] invSlots;
    private Image[] invImages;


    public Sprite emptySprite;
    [SerializeField] private Sprite cornSprite;  
    [SerializeField] private Sprite wheatSprite; 
    public TextMeshProUGUI coinText;

    void Start()
    {
        // Initialize the image array to match the size of invSlots
        invImages = new Image[invSlots.Length];

        for (int i = 0; i < invSlots.Length; i++)
        {
            if (invSlots[i] != null)
            {
                invImages[i] = invSlots[i].GetComponent<Image>();
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
            UpdateSlot(invImages[i], itemOrder, i);
        }
    }

    private void UpdateSlot(Image slotImage, List<string> itemOrder, int index)
    {
        if (slotImage == null || index >= itemOrder.Count)
        {
            slotImage.sprite = emptySprite; // Clear slot if no item
        }
        else
        {
            string itemName = itemOrder[index];
            slotImage.sprite = GetSpriteForItem(itemName); // Get corresponding sprite
        }
    }

    private Sprite GetSpriteForItem(string itemName)
    {
        switch (itemName)
        {
            case "corn":
                return cornSprite;
            case "wheat":
                return wheatSprite;
            default:
                return null; // No sprite for unknown items
        }
    }
}
