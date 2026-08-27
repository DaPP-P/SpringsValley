using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public GameObject[] invSlots;
    protected Image[] invImages;
    public TextMeshProUGUI[] invSlotCounts;
    public GameObject[] invCountBackground;

    public Sprite emptySprite;
    [SerializeField] private Sprite cornSprite = null;
    [SerializeField] private Sprite wheatSprite = null;
    [SerializeField] private Sprite healing_potionSprite = null;
    [SerializeField] private Sprite speed_potionSprite = null;
    public TextMeshProUGUI coinText;

    protected int selectedSlotIndex = -1;
    public GameObject contextMenuPrefab;
    protected static GameObject activeContextMenu;
    protected PlayerHealth playerHealth;
    protected PlayerMovement playerMovement;

    protected virtual void Start()
    {
        ResolvePlayer();
        InitialisePlayerSlots();
        RefreshInventory();
    }

    protected virtual void OnEnable()
    {
        PlayerLoot.InventoryChanged += RefreshInventory;
        CloseContextMenu();
        RefreshInventory();
    }

    protected virtual void OnDisable()
    {
        PlayerLoot.InventoryChanged -= RefreshInventory;
        CloseContextMenu();
    }

    protected virtual void Update()
    {
        HandleRightClick();
    }

    protected void InitialisePlayerSlots()
    {
        int slotCount = invSlots != null ? invSlots.Length : 0;
        invImages = new Image[slotCount];

        for (int index = 0; index < slotCount; index++)
        {
            GameObject slot = invSlots[index];
            if (slot == null)
            {
                continue;
            }

            invImages[index] = slot.GetComponent<Image>();
            Button slotButton = slot.GetComponent<Button>();
            if (slotButton == null)
            {
                slotButton = slot.AddComponent<Button>();
            }

            int capturedIndex = index;
            slotButton.onClick.AddListener(() => OnSlotClicked(capturedIndex));
        }
    }

    protected void ResolvePlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("Inventory could not find an object tagged Player.", this);
            return;
        }

        playerHealth = player.GetComponent<PlayerHealth>();
        playerMovement = player.GetComponent<PlayerMovement>();
    }

    protected virtual void RefreshInventory()
    {
        if (coinText != null)
        {
            coinText.text = $"Coins: {PlayerLoot.GetItemAmount("coin")}";
        }

        if (invImages == null)
        {
            return;
        }

        List<string> itemOrder = PlayerLoot.GetItemList();
        for (int index = 0; index < invImages.Length; index++)
        {
            UpdateSlot(
                invImages[index],
                GetArrayValue(invSlotCounts, index),
                GetArrayValue(invCountBackground, index),
                itemOrder,
                index);
        }

        if (selectedSlotIndex >= itemOrder.Count)
        {
            selectedSlotIndex = -1;
            ResetPlayerSlotHighlights();
        }
    }

    protected virtual void UpdateSlot(
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
            slotCountText.text = hasItem ? PlayerLoot.GetItemAmount(itemOrder[index]).ToString() : string.Empty;
        }

        if (slotCountBackground != null)
        {
            slotCountBackground.SetActive(hasItem);
        }
    }

    protected Sprite GetSpriteForItem(string itemName)
    {
        switch (itemName)
        {
            case "corn":
                return cornSprite != null ? cornSprite : emptySprite;
            case "wheat":
                return wheatSprite != null ? wheatSprite : emptySprite;
            case "healing_potion":
                return healing_potionSprite != null ? healing_potionSprite : emptySprite;
            case "speed_potion":
                return speed_potionSprite != null ? speed_potionSprite : emptySprite;
            default:
                return emptySprite;
        }
    }

    protected virtual void OnSlotClicked(int index)
    {
        List<string> itemOrder = PlayerLoot.GetItemList();
        if (index < 0 || index >= itemOrder.Count)
        {
            selectedSlotIndex = -1;
            ResetPlayerSlotHighlights();
            return;
        }

        CloseContextMenu();
        bool wasSelected = selectedSlotIndex == index;
        ResetPlayerSlotHighlights();
        selectedSlotIndex = wasSelected ? -1 : index;

        if (!wasSelected)
        {
            HighlightSlot(index, true, false);
            PlayAcceptSound();
        }
    }

    protected virtual void HighlightSlot(int index, bool isSelected, bool isVendor)
    {
        if (isVendor || invSlots == null || index < 0 || index >= invSlots.Length || invSlots[index] == null)
        {
            return;
        }

        Image slotImage = invSlots[index].GetComponent<Image>();
        if (slotImage != null)
        {
            slotImage.color = isSelected ? Color.yellow : Color.white;
        }
    }

    protected virtual void HandleRightClick()
    {
        if (!Input.GetMouseButtonDown(1) || invSlots == null)
        {
            return;
        }

        Vector3 mousePosition = Input.mousePosition;
        List<string> itemOrder = PlayerLoot.GetItemList();
        for (int index = 0; index < invSlots.Length && index < itemOrder.Count; index++)
        {
            GameObject slot = invSlots[index];
            RectTransform slotRect = slot != null ? slot.GetComponent<RectTransform>() : null;
            if (slotRect != null && RectTransformUtility.RectangleContainsScreenPoint(slotRect, mousePosition))
            {
                ShowContextMenu(index, mousePosition);
                return;
            }
        }
    }

    protected virtual void ShowContextMenu(int slotIndex, Vector3 position)
    {
        if (!IsValidPlayerSlot(slotIndex) || contextMenuPrefab == null)
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
                    button.gameObject.SetActive(true);
                    button.onClick.AddListener(() => DropItem(slotIndex));
                    break;
                case "SellButton":
                case "BuyButton":
                    button.gameObject.SetActive(false);
                    break;
                case "CloseButton":
                    button.onClick.AddListener(CloseContextMenu);
                    break;
            }
        }
    }

    protected void CloseContextMenu()
    {
        if (activeContextMenu != null)
        {
            Destroy(activeContextMenu);
            activeContextMenu = null;
        }
    }

    protected void UseItem(int slotIndex)
    {
        if (!TryGetPlayerItem(slotIndex, out string itemName, out Item item))
        {
            CloseContextMenu();
            return;
        }

        bool canUse = false;
        if (item.UseAbility == "Heal" && playerHealth != null && playerHealth.isAlive && playerHealth.currentHealth < playerHealth.maxHealth)
        {
            canUse = true;
        }
        else if (item.UseAbility == "Speed" && playerMovement != null)
        {
            canUse = true;
        }

        if (canUse && PlayerLoot.RemoveItem(itemName, 1))
        {
            if (item.UseAbility == "Heal")
            {
                int healthBeforeUse = playerHealth.currentHealth;
                playerHealth.Heal(item.AbilityQuantity);
                int healthRestored = playerHealth.currentHealth - healthBeforeUse;
                ItemNotificationUI.ShowEffect($"Restored {healthRestored} health");
            }
            else
            {
                playerMovement.ApplySpeedBoost(item.AbilityQuantity, item.DurationSeconds);
                ItemNotificationUI.ShowEffect(
                    $"Speed +{item.AbilityQuantity} for {Mathf.RoundToInt(item.DurationSeconds)} seconds");
            }

            PlayAcceptSound();
        }
        else
        {
            PlayDeclineSound();
        }

        CloseContextMenu();
    }

    protected void DropItem(int slotIndex)
    {
        List<string> itemOrder = PlayerLoot.GetItemList();
        bool dropped = slotIndex >= 0 && slotIndex < itemOrder.Count && PlayerLoot.TryDropItem(itemOrder[slotIndex]);

        if (dropped)
        {
            PlayAcceptSound();
        }
        else
        {
            PlayDeclineSound();
        }

        CloseContextMenu();
    }

    protected virtual void SellItem(int slotIndex)
    {
        if (!TryGetPlayerItem(slotIndex, out string itemName, out Item item))
        {
            CloseContextMenu();
            return;
        }

        if (PlayerLoot.RemoveItem(itemName, 1))
        {
            PlayerLoot.AddCoins(item.SellPrice);
            PlayAcceptSound();
        }
        else
        {
            PlayDeclineSound();
        }

        CloseContextMenu();
    }

    protected void ResetPlayerSlotHighlights()
    {
        if (invSlots == null)
        {
            return;
        }

        for (int index = 0; index < invSlots.Length; index++)
        {
            HighlightSlot(index, false, false);
        }
    }

    protected void PlayAcceptSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.acceptSound, transform.position);
        }
    }

    protected void PlayDeclineSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(AudioManager.Instance.declineSound, transform.position);
        }
    }

    private bool TryGetPlayerItem(int slotIndex, out string itemName, out Item item)
    {
        itemName = null;
        item = null;
        List<string> itemOrder = PlayerLoot.GetItemList();
        if (slotIndex < 0 || slotIndex >= itemOrder.Count)
        {
            return false;
        }

        itemName = itemOrder[slotIndex];
        return PlayerLoot.GetItemAmount(itemName) > 0 && ItemList.items.TryGetValue(itemName, out item);
    }

    private bool IsValidPlayerSlot(int slotIndex)
    {
        return slotIndex >= 0 && slotIndex < PlayerLoot.GetItemList().Count;
    }

    protected static T GetArrayValue<T>(T[] values, int index) where T : class
    {
        return values != null && index >= 0 && index < values.Length ? values[index] : null;
    }
}
