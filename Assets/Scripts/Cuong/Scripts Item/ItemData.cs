using UnityEngine;

public enum ItemType { Seed, Plant, Tool, AnimalProduct }

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public int maxStack = 99;
    public ItemType itemType;
}
// Dữ liệu gốc của item