using UnityEngine;
public enum ETriggerCanvas
{
    None,
    Barn,
    FarmStall,
    SeedShop,
    Builder,
    Merchant,
    SellAnimal,
    AnimalBarn1,
    AnimalBarn2,
    PenDoor,
    HouseDoor,
    BarnDoor,
    WaterWell
}
public class TriggerCanvas : MonoBehaviour
{
    public GameObject uiPrompt;
    public bool isPlayerNearby;
    public ETriggerCanvas state;
    private void Start()
    {
        uiPrompt.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            uiPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            uiPrompt.SetActive(false);
        }
    }
}
