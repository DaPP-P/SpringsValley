using UnityEngine;
using System;
using System.Collections.Generic;
using System.Drawing;

public class Item
{
    public string Name { get; set; } // Item Name
    public string UseAbility { get; set; } // Use Ability Name
    public int AbilityQuantity { get; set; } // Item Quantity
    public int Price { get; set;} // Item Price

    public Item(string name, string useAbility, int abilityquantity, int price)
    {
        Name = name;
        UseAbility = useAbility;
        AbilityQuantity = abilityquantity;
        Price = price;
    }
}

public static class ItemList
{
    public static Dictionary<string, Item> items = new Dictionary<string, Item>
    {
        { "wheat", new Item("Wheat", "Heal", 10, 1) },
        { "corn", new Item("Corn", "Heal", 10, 1) },
        { "healing_potion", new Item("Healing Potion", "Heal", 25, 2) }
    };
}
