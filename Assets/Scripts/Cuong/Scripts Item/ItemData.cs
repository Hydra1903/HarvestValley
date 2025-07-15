using UnityEngine;

public enum ItemType { Seed, Crop, Tool}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int maxStack = 99;
    public ItemType itemType;
}