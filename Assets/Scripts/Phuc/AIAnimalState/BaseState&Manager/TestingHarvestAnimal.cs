using UnityEngine;

public class TestingHarvestAnimal : MonoBehaviour
{
    public enum ResourceType { None, Wool, Milk }
    public ResourceType resourceType;

    private bool playerInRange = false;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Press E to collect");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            Harvest();
        }
    }

    void Harvest()
    {
        switch (resourceType)
        {
            case ResourceType.Wool:
                Debug.Log("Collecting Sheep Wool");
                break;
            case ResourceType.Milk:
                Debug.Log("Collecting Goat Milk");
                break;
        }

    }
}
