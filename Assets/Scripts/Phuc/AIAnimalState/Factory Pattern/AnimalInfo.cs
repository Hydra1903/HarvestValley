using UnityEngine;

public class AnimalInfo : MonoBehaviour
{
    public GameObject infoPanel;
    public string animalName;
    public string status;
    public string product;
    public bool canHarvest;

    private bool isPlayerNear = false;
    private bool isInfoVisible = false;

    private void Start()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false);
    }

    private void Update()
    {
        if (isPlayerNear && Input.GetMouseButtonDown(0))
        {
            isInfoVisible = !isInfoVisible;
            if (infoPanel != null)
                infoPanel.SetActive(isInfoVisible);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            isInfoVisible = false;
            if (infoPanel != null)
                infoPanel.SetActive(false);
        }
    }
}
