using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLoot : MonoBehaviour
{
    public static PlayerLoot Instance { get; private set; }
    public static int coinAmount;

    [Header("Dropped item prefabs")]
    public GameObject wheat;
    public GameObject corn;
    public GameObject healing_potion;
    public GameObject speed_potion;

    public static readonly Dictionary<string, int> items = new Dictionary<string, int>
    {
        { "wheat", 0 },
        { "corn", 0 },
        { "healing_potion", 0 },
        { "speed_potion", 0 }
    };

    public static event Action InventoryChanged;

    private void Awake()
    {
        if (Instance == null || Instance == this)
        {
            Instance = this;
            return;
        }

        Debug.LogWarning("A duplicate PlayerLoot component was removed.", this);
        Destroy(this);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public static bool IncreaseItem(string itemName, int amount)
    {
        if (string.IsNullOrWhiteSpace(itemName) || amount <= 0)
        {
            return false;
        }

        if (itemName != "coin" && !ItemList.items.ContainsKey(itemName))
        {
            Debug.LogWarning($"Cannot add unknown inventory item '{itemName}'.");
            return false;
        }

        if (itemName == "coin")
        {
            coinAmount += amount;
        }
        else
        {
            if (!items.ContainsKey(itemName))
            {
                items[itemName] = 0;
            }

            items[itemName] += amount;
        }

        InventoryChanged?.Invoke();
        ItemNotificationUI.ShowItemAdded(itemName, amount);
        return true;
    }

    // Kept for existing callers: decreasing an item means dropping it in the world.
    public static bool DecreaseItem(string itemName, int amount)
    {
        return TryDropItem(itemName, amount);
    }

    public static bool RemoveItem(string itemName, int amount)
    {
        if (string.IsNullOrWhiteSpace(itemName) || amount <= 0 || GetItemAmount(itemName) < amount)
        {
            return false;
        }

        if (itemName == "coin")
        {
            coinAmount -= amount;
        }
        else
        {
            items[itemName] -= amount;
        }

        InventoryChanged?.Invoke();
        return true;
    }

    public static bool TrySpendCoins(int amount)
    {
        return amount >= 0 && (amount == 0 || RemoveItem("coin", amount));
    }

    public static void AddCoins(int amount)
    {
        IncreaseItem("coin", amount);
    }

    public static bool TryDropItem(string itemName, int amount = 1)
    {
        if (amount <= 0 || GetItemAmount(itemName) < amount || Instance == null)
        {
            return false;
        }

        GameObject itemPrefab = Instance.GetDropPrefab(itemName);
        if (itemPrefab == null)
        {
            Debug.LogWarning($"No dropped-item prefab is assigned for '{itemName}'. The item was kept.", Instance);
            return false;
        }

        if (!RemoveItem(itemName, amount))
        {
            return false;
        }

        for (int index = 0; index < amount; index++)
        {
            Instance.SpawnItemInWorld(itemPrefab);
        }

        return true;
    }

    // Compatibility entry point for any Inspector events that already reference this method.
    public static void SpawnItemInWorld(string itemName)
    {
        if (Instance == null)
        {
            return;
        }

        GameObject itemPrefab = Instance.GetDropPrefab(itemName);
        if (itemPrefab != null)
        {
            Instance.SpawnItemInWorld(itemPrefab);
        }
    }

    public static int GetItemAmount(string itemName)
    {
        if (itemName == "coin")
        {
            return coinAmount;
        }

        return !string.IsNullOrEmpty(itemName) && items.TryGetValue(itemName, out int amount) ? amount : 0;
    }

    public static List<string> GetItemList()
    {
        List<string> itemOrder = new List<string>();

        foreach (string itemName in ItemList.items.Keys)
        {
            if (GetItemAmount(itemName) > 0)
            {
                itemOrder.Add(itemName);
            }
        }

        foreach (KeyValuePair<string, int> item in items)
        {
            if (item.Value > 0 && !itemOrder.Contains(item.Key))
            {
                itemOrder.Add(item.Key);
            }
        }

        return itemOrder;
    }

    public static void ResetInventory()
    {
        coinAmount = 0;

        List<string> keys = new List<string>(items.Keys);
        foreach (string key in keys)
        {
            items[key] = 0;
        }

        foreach (string key in ItemList.items.Keys)
        {
            if (!items.ContainsKey(key))
            {
                items[key] = 0;
            }
        }

        InventoryChanged?.Invoke();
    }

    // Dictionary order is no longer used by the UI, but this remains to avoid breaking old events.
    public static void MoveZeroCountItemsToEnd()
    {
    }

    private GameObject GetDropPrefab(string itemName)
    {
        GameObject itemPrefab;
        switch (itemName)
        {
            case "wheat":
                itemPrefab = wheat;
                break;
            case "corn":
                itemPrefab = corn;
                break;
            case "healing_potion":
                itemPrefab = healing_potion;
                break;
            case "speed_potion":
                itemPrefab = speed_potion;
                break;
            default:
                return null;
        }

        if (itemPrefab != null)
        {
            return itemPrefab;
        }

        // Existing scenes already assign crop/potion prefabs on each VendorLoot. Reuse
        // one of those references when the player's equivalent Inspector slot is empty.
        VendorLoot[] vendors = FindObjectsByType<VendorLoot>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (VendorLoot vendor in vendors)
        {
            switch (itemName)
            {
                case "wheat" when vendor.wheat != null:
                    return vendor.wheat;
                case "corn" when vendor.corn != null:
                    return vendor.corn;
                case "healing_potion" when vendor.healing_potion != null:
                    return vendor.healing_potion;
                case "speed_potion" when vendor.speed_potion != null:
                    return vendor.speed_potion;
            }
        }

        return null;
    }

    private void SpawnItemInWorld(GameObject itemPrefab)
    {
        GameObject spawnedItem = Instantiate(itemPrefab, transform.position, Quaternion.identity);
        Rigidbody2D spawnedBody = spawnedItem.GetComponent<Rigidbody2D>();
        if (spawnedBody == null)
        {
            return;
        }

        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
        spawnedBody.AddForce(randomDirection * 3f, ForceMode2D.Impulse);
        StartCoroutine(StopMovementAfterDelay(spawnedBody, 0.2f));
    }

    private IEnumerator StopMovementAfterDelay(Rigidbody2D spawnedBody, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (spawnedBody != null)
        {
            spawnedBody.linearVelocity = Vector2.zero;
        }
    }
}
