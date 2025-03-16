using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class PlayerLoot : MonoBehaviour
{

    public static PlayerLoot Instance;
    public static int coinAmount;

    // Prefabs for collectable items
    public GameObject wheat;
    public GameObject corn;
    public static Dictionary<string, int> items = new Dictionary<string, int>
    {
        { "coin", 0 },
        { "wheat", 0 },
        { "corn", 0}
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Increase the amount of an item
    public static void IncreaseItem(string itemName, int amount)
    {
        if (itemName == "coin")
        {
            coinAmount += amount;
        }

        else if (items.ContainsKey(itemName))
        {
            items[itemName] += amount;
        }
        else
        {
            items[itemName] = amount; // Add the item if it doesn't exist
        }

        MoveZeroCountItemsToEnd();
    }

    // Decrease the amount of an item
    public static void DecreaseItem(string itemName, int amount)
    {
        if (itemName == "coin")
        {
            coinAmount = Mathf.Max(0, coinAmount - amount); // Ensure no negative values
        }
        else if (items.ContainsKey(itemName))
        {
            items[itemName] = Mathf.Max(0, items[itemName] - amount); // Ensure no negative values
            SpawnItemInWorld(itemName);
        }

        MoveZeroCountItemsToEnd();
    }

    // Decrease the amount of an item and destorys the object
    public static void RemoveItem(string itemName, int amount)
    {
        if (itemName == "coin")
        {
            coinAmount = Mathf.Max(0, coinAmount - amount); // Ensure no negative values
        }
        else if (items.ContainsKey(itemName))
        {
            items[itemName] = Mathf.Max(0, items[itemName] - amount); // Ensure no negative values
        }

        MoveZeroCountItemsToEnd();
    }

    public static void SpawnItemInWorld(string itemName)
    {
        GameObject itemPrefab = null;

        switch (itemName)
        {
            case "wheat":
                itemPrefab = Instance.wheat;
                break;
            case "corn":
                itemPrefab = Instance.corn;
                break;
        }

        if (itemPrefab != null)
        {
            Vector3 spawnPosition = Instance.transform.position;
            GameObject spawnedItem = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
            Rigidbody2D rb = spawnedItem.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 randomDirection = Random.insideUnitCircle.normalized; // Random direction
                float forceMagnitude = 3f; // Adjust this value for desired speed
                rb.AddForce(randomDirection * forceMagnitude, ForceMode2D.Impulse);
                Instance.StartCoroutine(Instance.StopMovementAfterDelay(rb, 0.2f)); 
            }
        }
    }

    private IEnumerator StopMovementAfterDelay(Rigidbody2D rb, float delay)
    {
    yield return new WaitForSeconds(delay);
    StopMovement(rb);
    }

    private void StopMovement(Rigidbody2D rb)
    {   
        rb.linearVelocity = Vector2.zero;
    }

    // Get the current amount of an item
    public static int GetItemAmount(string itemName)
    {
        if (itemName == "coin")
        {
            return coinAmount;
        }

        return items.ContainsKey(itemName) ? items[itemName] : 0;
    }

    // Get all items and their amounts as a list of strings
    public static List<string> GetItemList()
    {
        List<string> itemOrder = new List<string>();

        foreach (KeyValuePair<string, int> item in items)
        {
            if (item.Value > 0) // Only include items with a quantity > 0
            {
                itemOrder.Add(item.Key);
            }
        }

        return itemOrder;
    }

    // Moves items with count 0 to the end of the dictionary
    public static void MoveZeroCountItemsToEnd()
    {
        var nonZeroItems = new Dictionary<string, int>();
        var zeroItems = new Dictionary<string, int>();

        foreach (var item in items)
        {
            if (item.Value > 0)
            {
                nonZeroItems.Add(item.Key, item.Value);
            }
            else
            {
                zeroItems.Add(item.Key, item.Value);
            }
        }

        // Combine non-zero and zero count items
        items.Clear();
        foreach (var item in nonZeroItems)
        {
            items.Add(item.Key, item.Value);
        }
        foreach (var item in zeroItems)
        {
            items.Add(item.Key, item.Value);
        }
    }
}
