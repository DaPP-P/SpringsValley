using UnityEngine;
using System;
using System.Collections.Generic;
using System.Drawing;

public class Item
{
    public string Name { get; set; } // Item Name
    public string UseAbility { get; set; } // Use Ability Name
    public int AbilityQuantity { get; set; } // Item Quantity

    public Item(string name, string useAbility, int abilityquantity)
    {
        Name = name;
        UseAbility = useAbility;
        AbilityQuantity = abilityquantity;
    }
}

public static class ItemList
{
    public static Dictionary<string, Item> items = new Dictionary<string, Item>
    {
        { "wheat", new Item("Wheat", "Heal", 10) },
        { "corn", new Item("Corn", "Heal", 10) },
        { "healing_potion", new Item("Healing Potion", "Heal", 25) }
    };
}
