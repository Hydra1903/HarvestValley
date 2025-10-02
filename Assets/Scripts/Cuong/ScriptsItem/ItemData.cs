using UnityEngine;

public enum ItemType { Seed, Plant, Tool, AnimalProduct }
public enum PlantType 
{ 
    Apple, 
    Apricot, 
    Asparagus, 
    Beetroot, 
    BellPepper, 
    BottleGourd, 
    Cabbage, 
    Carrot,
    Cauliflower,
    Cherry,
    Chilli,
    Corn,
    Cucumber,
    DelicataSquash,
    Eggplant,
    GreenBean,
    HayBale,
    Lemon,
    Onion,
    Orange,
    Peach,
    Pear,
    Plum,
    Potato,
    Pumpkin,
    StripedPumpkin,
    Tomato,
    Watermelon,
    WhitePumpkin,
    None
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string id;
    public PlantType plantType;
    public string itemName;
    public Sprite icon;
    public int maxStack;
    public ItemType itemType;
    public string description;
    public string season;
}
