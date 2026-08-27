using System.Collections.Generic;

public sealed class Item
{
    public string Name { get; }
    public string UseAbility { get; }
    public int AbilityQuantity { get; }
    public int BuyPrice { get; }
    public int SellPrice { get; }
    public int Price => BuyPrice;
    public string Description { get; }
    public float DurationSeconds { get; }

    public Item(
        string name,
        string useAbility,
        int abilityQuantity,
        int buyPrice,
        int sellPrice,
        string description,
        float durationSeconds = 0f)
    {
        Name = name;
        UseAbility = useAbility;
        AbilityQuantity = abilityQuantity;
        BuyPrice = buyPrice;
        SellPrice = sellPrice;
        Description = description;
        DurationSeconds = durationSeconds;
    }
}

public static class ItemList
{
    public static readonly Dictionary<string, Item> items = new Dictionary<string, Item>
    {
        { "wheat", new Item("Wheat", "Heal", 10, 1, 1, "Consume to restore 10 health.") },
        { "corn", new Item("Corn", "Heal", 10, 1, 1, "Consume to restore 10 health.") },
        { "healing_potion", new Item("Healing Potion", "Heal", 25, 2, 1, "Consume to restore 25 health.") },
        { "speed_potion", new Item("Speed Potion", "Speed", 10, 2, 1, "Consume to increase movement speed by 10 for 2 minutes.", 120f) }
    };
}
