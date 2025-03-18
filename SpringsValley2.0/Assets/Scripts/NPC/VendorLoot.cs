using System.Collections.Generic;
using UnityEngine;
using System.Collections;
public class VendorLoot : MonoBehaviour
{

     // Prefabs for collectable items
    public GameObject wheat;
    public GameObject corn;

    public GameObject healing_potion;
    
    public static VendorLoot Instance;

    public static Dictionary<string, int> items = new Dictionary<string, int>
    {
        { "wheat", 0 },
        { "corn", 0},
        {"healing_potion", 1}
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

        // Decrease the amount of an item and destorys the object
    public static void RemoveItem(string itemName, int amount)
    {
        if (items.ContainsKey(itemName))
        {
            items[itemName] = Mathf.Max(0, items[itemName] - amount); // Ensure no negative values
        }

        MoveZeroCountItemsToEnd();
    }

    // Increase the amount of an item
    public static void IncreaseItem(string itemName, int amount)
    {
        if (items.ContainsKey(itemName))
        {
            items[itemName] += amount;
        }
        else
        {
            items[itemName] = amount; // Add the item if it doesn't exist
        }

        MoveZeroCountItemsToEnd();
    }

    // Get the current amount of an item
    public static int GetItemAmount(string itemName)
    {
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
