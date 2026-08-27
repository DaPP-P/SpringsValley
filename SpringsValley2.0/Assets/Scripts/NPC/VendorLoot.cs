using System;
using System.Collections.Generic;
using UnityEngine;

public class VendorLoot : MonoBehaviour
{
    // Retained so existing prefab references are not lost.
    public GameObject wheat;
    public GameObject corn;
    public GameObject healing_potion;
    public GameObject speed_potion;

    [Header("Starting stock for this vendor")]
    [SerializeField, Min(0)] private int startingWheat = 0;
    [SerializeField, Min(0)] private int startingCorn = 0;
    [SerializeField, Min(0)] private int startingHealingPotions = 1;
    [SerializeField, Min(0)] private int startingSpeedPotions = 0;

    private readonly Dictionary<string, int> items = new Dictionary<string, int>();
    private bool stockInitialised;

    public event Action StockChanged;

    private void Awake()
    {
        InitialiseStock();
    }

    public bool RemoveItem(string itemName, int amount)
    {
        InitialiseStock();
        if (string.IsNullOrWhiteSpace(itemName) || amount <= 0 || GetItemAmount(itemName) < amount)
        {
            return false;
        }

        items[itemName] -= amount;
        StockChanged?.Invoke();
        return true;
    }

    public bool IncreaseItem(string itemName, int amount)
    {
        InitialiseStock();
        if (string.IsNullOrWhiteSpace(itemName) || amount <= 0)
        {
            return false;
        }

        if (!items.ContainsKey(itemName))
        {
            items[itemName] = 0;
        }

        items[itemName] += amount;
        StockChanged?.Invoke();
        return true;
    }

    public int GetItemAmount(string itemName)
    {
        InitialiseStock();
        return !string.IsNullOrEmpty(itemName) && items.TryGetValue(itemName, out int amount) ? amount : 0;
    }

    public List<string> GetItemList()
    {
        InitialiseStock();
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

    public bool HasStock()
    {
        return GetItemList().Count > 0;
    }

    private void InitialiseStock()
    {
        if (stockInitialised)
        {
            return;
        }

        items["wheat"] = Mathf.Max(0, startingWheat);
        items["corn"] = Mathf.Max(0, startingCorn);
        items["healing_potion"] = Mathf.Max(0, startingHealingPotions);
        items["speed_potion"] = Mathf.Max(0, startingSpeedPotions);
        stockInitialised = true;
    }
}
