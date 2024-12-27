using System.Collections.Generic;
using UnityEngine;

public class PlayerLoot : MonoBehaviour
{

    public static Dictionary<string, int> items = new Dictionary<string, int>
    {
        { "coin", 0 },
        { "corn", 0 },
        { "wheat", 0}
    };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize if necessary
    }

    // Update is called once per frame
    void Update()
    {
        // Any updates related to loot management
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
    }

    // Decrease the amount of an item
    public static void DecreaseItem(string itemName, int amount)
    {
        if (items.ContainsKey(itemName))
        {
            items[itemName] = Mathf.Max(0, items[itemName] - amount); // Ensure no negative values
        }
    }

    // Get the current amount of an item
    public static int GetItemAmount(string itemName)
    {
        return items.ContainsKey(itemName) ? items[itemName] : 0;
    }

    // Get all items and their amounts as a list of strings
    public static List<string> GetAllItems()
    {
        List<string> itemList = new List<string>();

        foreach (KeyValuePair<string, int> item in items)
        {
            itemList.Add($"{item.Key}: {item.Value}");
        }

        return itemList;
    }
}
