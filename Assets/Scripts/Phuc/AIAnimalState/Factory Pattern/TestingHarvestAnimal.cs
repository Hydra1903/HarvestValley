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

    private int previousDay = -1;
    private AnimalFedding feeding;
    private Inventory playerInventory;
    private bool canHarvest = false;
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
        if (GameTime.Instance != null)
            previousDay = GameTime.Instance.day;
    }
private void HandleNextDay()
{
        Debug.Log($"{gameObject.name} sang ngày m?i: {GameTime.Instance.day}");
        if (feeding != null && feeding.CanHarvest())
        {
            canHarvest = true;
        }
    }

    private void Update()
    {
        if (GameTime.Instance == null) return;

        if (GameTime.Instance.day != previousDay)
        {
            previousDay = GameTime.Instance.day;
            HandleNextDay(); 
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance <= interactDistance && Input.GetKeyDown(KeyCode.E))
        {
            TryHarvest(player);
        }
    }

    private void TryHarvest(GameObject player)
    {
        Inventory playerInventory = Inventory.Instance;
        if (playerInventory == null)
        {
            Debug.LogError("Cant found Inventory");
            return;
        }

        if (canHarvest && feeding != null && feeding.CanHarvest())
        {
            ItemData itemToGive = GetItemDataByType();

            if (itemToGive != null && playerInventory.AddItem(itemToGive, GetHarvestAmount()))
            {
                Notification.Instance.ShowNotification($"+1 {itemToGive.itemName} from {animalType}");
                feeding.ResetHarvest();
                canHarvest = false;

                InventoryUI ui = FindAnyObjectByType<InventoryUI>();
                ui?.UpdateAllSlots();
            }
            else
            {

                Notification.Instance.ShowNotification("Inventory full ho?c không th? thêm item này.");
            }
        }
        else
        {
            int fedDays = feeding != null ? feeding.GetDaysFed() : 0;

            Notification.Instance.ShowNotification($"Chýa ð? ði?u ki?n thu ho?ch. Ngày ð? ãn: {fedDays}");
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

    private int GetHarvestAmount()
    {
        switch (animalType)
        {
            case AnimalType.Goat: return 1;   // dê cho 1 s?a
            default: return 3;                // c?u cho 3 lông
        }
    }
}
