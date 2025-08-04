using UnityEngine;

public class TestingHarvestAnimal : MonoBehaviour
{
    public ItemData blackWoolItem;
    public ItemData whiteWoolItem;
    public ItemData creamWoolItem;
    public ItemData goatMilkItem;

    public float harvestCooldown = 10f;
    private bool canHarvest = true;

    public enum AnimalType { Sheep_Black, Sheep_White, Sheep_Cream, Goat }
    public AnimalType animalType;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && canHarvest)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Inventory playerInventory = Inventory.Instance;
                InventoryUI inventoryUI = Object.FindAnyObjectByType<InventoryUI>();
                if (inventoryUI == null)
                    Debug.LogWarning("Cant found InventoryUI.");

                ItemData itemToGive = null;
                switch (animalType)
                {
                    case AnimalType.Sheep_Black:
                        itemToGive = blackWoolItem;
                        break;
                    case AnimalType.Sheep_White:
                        itemToGive = whiteWoolItem;
                        break;
                    case AnimalType.Sheep_Cream:
                        itemToGive = creamWoolItem;
                        break;
                    case AnimalType.Goat:
                        itemToGive = goatMilkItem;
                        break;
                }

                if (itemToGive != null && playerInventory.AddItem(itemToGive, 1))
                {
                    Debug.Log($"Added {itemToGive.itemName} To Inventory");
                    inventoryUI?.UpdateAllSlots();

                    canHarvest = false;
                    Invoke(nameof(ResetHarvest), harvestCooldown);
                }
                else
                {
                    Debug.Log("Cant add anymore");
                }
            }
        }
    }

    void ResetHarvest()
    {
        canHarvest = true;
    }
}
