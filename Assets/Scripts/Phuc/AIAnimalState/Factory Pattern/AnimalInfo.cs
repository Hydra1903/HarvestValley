using UnityEngine;

public class AnimalInfo : MonoBehaviour
{
    public AnimalData animalData;

    public InfoPanelUI infoPanel;
    private bool isPlayerNearby = false;

    public void InjectPanel(InfoPanelUI panel)
    {
        infoPanel = panel;
        if (infoPanel != null) infoPanel.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetMouseButtonDown(0)) 
        {
            if (infoPanel == null) return;

            if (infoPanel.IsShowingOwner(this)) infoPanel.Hide();
            else infoPanel.Show(animalData, this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) isPlayerNearby = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            infoPanel?.Hide();
        }
    }
}
