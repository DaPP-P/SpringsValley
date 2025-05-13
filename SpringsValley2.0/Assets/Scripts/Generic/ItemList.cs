using UnityEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine.U2D;

public class Item
{
    public string Name { get; set; } // Item Name
    public string UseAbility { get; set; } // Use Ability Name
    public int AbilityQuantity { get; set; } // Item Quantity
    public int Price { get; set;} // Item Price
    public string Description { get; set; } // Item Description

    public Item(string name, string useAbility, int abilityquantity, int price, string description = null)
    {
        Name = name;
        UseAbility = useAbility;
        AbilityQuantity = abilityquantity;
        Price = price;
        Description = description;
    }
}

public static class ItemList
{
    public static Dictionary<string, Item> items = new Dictionary<string, Item>
    {
        { "wheat", new Item("Wheat", "Heal", 10, 1, "Wheat, can be consumed to heal 10 health points") },
        { "corn", new Item("Corn", "Heal", 10, 1, "Corn, can be consumed to heal 10 health points") },
        { "healing_potion", new Item("Healing Potion", "Heal", 25, 2, "Healing Potion, can be consumed to heal 25 health points") },
        {"speed_potion", new Item("Speed Potion", "Speed", 10, 2, "Speed Potion, can be consumed to increase speed by 10 for 2 minutes") },
    };
}
