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
            }
        }
    }

    private void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector3.Distance(player.transform.position, transform.position);

        // Kiểm tra khoảng cách
        if (distance <= interactDistance)
        {
            Debug.Log($"{gameObject.name}: Đã đến khoảng cách thu hoạch ({distance:F2}m)");

            // Kiểm tra xem player có đang nhìn con vật này không
            PlayerLookAtAnimal lookScript = player.GetComponent<PlayerLookAtAnimal>();
            bool isLooking = lookScript != null && lookScript.CurrentAnimal == this.GetComponent<AnimalInfo>();

            canHarvest = feeding != null && feeding.CanHarvest();

            if (isLooking && Input.GetKeyDown(KeyCode.E))
            {
                TryHarvest(player);
            }
        }
    }



    private void TryHarvest(GameObject player)
    {
        if (feeding == null) return;

        Inventory playerInventory = Inventory.Instance;
        if (playerInventory == null) return;

        if (canHarvest)
        {
            ItemData itemToGive = GetItemDataByType();
            if (itemToGive != null && playerInventory.AddItem(itemToGive, GetHarvestAmount()))
            {
                Notification.Instance.ShowNotification($"+{GetHarvestAmount()} {itemToGive.itemName}");
                feeding.ResetHarvest();
                canHarvest = false;
            }
        }
        else
        {
            if (animalType == AnimalType.Goat)
            {
                Notification.Instance.ShowNotification("Chưa đủ điều kiện để thu hoạch");
            }
            else
            {
                Notification.Instance.ShowNotification("Chưa đủ điều kiện để thu hoạch");
            }
        }
    }

    private ItemData GetItemDataByType()
    {
        return animalType switch
        {
            AnimalType.Sheep_Black => blackWoolItem,
            AnimalType.Sheep_White => whiteWoolItem,
            AnimalType.Sheep_Cream => creamWoolItem,
            AnimalType.Goat => goatMilkItem,
            _ => null
        };
    }
    public int GetRemainingDaysToHarvest()
    {
        // Ví dụ: nếu cần ăn 3 ngày để thu hoạch
        int requiredDays = 3;
        var feed = GetComponent<AnimalFedding>();
        if (feed == null) return -1;

        int remaining = requiredDays - feed.daysFed;
        return Mathf.Max(0, remaining);
    }
    private int GetHarvestAmount()
    {
        return animalType == AnimalType.Goat ? 1 : 3;
    }
}
