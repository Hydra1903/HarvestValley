using UnityEngine;

public enum ItemType { Seed, Plant, Tool, AnimalProduct }

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string id;
    public string itemName;
    public Sprite icon;
    public int maxStack;
    public ItemType itemType;
    public string description;
    public string season;
}
