using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{

    // Array of Inv Slots and Inv Images
    public GameObject[] invSlots;
    private Image[] invImages;
    public TextMeshProUGUI[] invSlotCounts;

    public GameObject[] invCountBackground;

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
            UpdateSlot(invImages[i], invSlotCounts[i], invCountBackground[i], itemOrder, i);     
        }
    }

    private void UpdateSlot(Image slotImage, TextMeshProUGUI slotCountText, GameObject slotCountBackground, List<string> itemOrder, int index)
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
