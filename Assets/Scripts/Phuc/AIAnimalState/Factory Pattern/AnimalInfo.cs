using UnityEngine;

public class AnimalInfo : MonoBehaviour
{
    public InfoPanelUI infoPanel;
    private bool isPlayerNearby = false;
    public AnimalData data;
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
            else infoPanel.Show(data, this);
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
