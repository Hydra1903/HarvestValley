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
    private bool canHarvest = false;

    private int lastFedTotalHours = -1;

    private void Start()
    {
        feeding = GetComponent<AnimalFedding>();
        if (feeding != null && feeding.barn == null)
        {
            Barn foundBarn = FindAnyObjectByType<Barn>();
            if (foundBarn != null)
            {
                feeding.barn = foundBarn;
                Debug.Log($"[Auto] Assign Barn cho {gameObject.name} trong scene.");
            }
            else
            {
                Debug.LogWarning($"Cant Found Barn for Assign {gameObject.name}!");
            }
        }

        if (GameTime.Instance != null)
        {
            lastFedTotalHours = GetTotalHoursFromGameTime();
        }
    }

    private void Update()
    {
        if (GameTime.Instance == null) return;

        int currentTotalHours = GetTotalHoursFromGameTime();
        if (currentTotalHours - lastFedTotalHours >= 24 && GameTime.Instance.hour >= 7)
        {
            HandleNextFeedingTime();
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance <= interactDistance && Input.GetKeyDown(KeyCode.E))
        {
            TryHarvest(player);
        }
    }

    private int GetTotalHoursFromGameTime()
    {
        var time = GameTime.Instance;
        return (time.year * 12 * 30 * 24) + (time.month * 30 * 24) + (time.day * 24) + time.hour;
    }

    private void HandleNextFeedingTime()
    {
        Debug.Log($"{gameObject.name} ð? ði?u ki?n ãn l?i lúc {GameTime.Instance.hour}h.");
        if (feeding != null)
        {
            feeding.ResetHarvest();
            canHarvest = feeding.CanHarvest();
            lastFedTotalHours = GetTotalHoursFromGameTime();
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
                Notification.Instance.ShowNotification($"+{GetHarvestAmount()} {itemToGive.itemName} t? {animalType}");
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
            case AnimalType.Goat: return 1;
            default: return 3;
        }
    }
}
