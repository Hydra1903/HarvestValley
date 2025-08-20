using UnityEngine;

public class TestingHarvestAnimal : MonoBehaviour
{
    [Header("Info")]
    public ItemData blackWoolItem;
    public ItemData whiteWoolItem;
    public ItemData creamWoolItem;
    public ItemData goatMilkItem;

    [Header("Setting")]
    public float interactDistance = 3f;


    public enum AnimalType { Sheep_Black, Sheep_White, Sheep_Cream, Goat }
    public AnimalType animalType;

    private AnimalFedding feeding;
    private Inventory playerInventory;

    private void Start()
    {
        feeding = GetComponent<AnimalFedding>();
        if (feeding != null && feeding.barn == null)
        {
            Barn foundBarn = FindAnyObjectByType<Barn>();
            if (foundBarn != null)
            {
                feeding.barn = foundBarn;
                Debug.Log($"[Auto] Assign Barn cho {gameObject.name}  trong scene.");
            }
            else
            {
                Debug.LogWarning($"Cant Found Barn for Assign {gameObject.name}!");
            }
        }
    }

    private void Update()
    {
        Inventory playerInventory = Inventory.Instance;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance <= interactDistance && Input.GetKeyDown(KeyCode.E))
        {
            if (feeding != null && feeding.CanHarvest())
            {
                if (playerInventory == null)
                {
                    Debug.LogError("Cant found Inventory");
                    return;
                }

                ItemData itemToGive = GetItemDataByType();
                if (itemToGive != null && playerInventory.AddItem(itemToGive, 1))
                {
                    Debug.Log($"Succes harvest {itemToGive.itemName} from{animalType}");
                    feeding.ResetHarvest();

                    InventoryUI ui = FindAnyObjectByType<InventoryUI>();
                    ui?.UpdateAllSlots();
                }
                else
                {
                    Debug.LogWarning("No longer adding that type of item.");
                }
            }
            else
            {
                Debug.Log("This Animal still not eaten or cant harvest");
            }
        }
    }

    private ItemData GetItemDataByType()
    {
        switch (animalType)
        {
            case AnimalType.Sheep_Black: return blackWoolItem;
            case AnimalType.Sheep_White: return whiteWoolItem;
            case AnimalType.Sheep_Cream: return creamWoolItem;
            case AnimalType.Goat: return goatMilkItem;
        }
        return null;
    }
}
