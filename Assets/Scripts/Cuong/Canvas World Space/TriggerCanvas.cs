using UnityEngine;
public enum ETriggerCanvas
{
    None,
    Barn,
    FarmStall,
    SeedShop,
    Builder,
    Merchant,
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
