using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimalInfo : MonoBehaviour
{
    //public AnimalData animalData;

    public InfoPanelUI infoPanel;
    private bool isPlayerNearby = false;
    public AnimalData data;

    public static AnimalInfo Instance;
    public TextMeshProUGUI nameAnimal;
    public TextMeshProUGUI remainingTime;
    public TextMeshProUGUI haveEaten;
    public GameObject[] icon;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
    public void SetInfo(string name, int remainingTime, bool haveEaten)
    {
        nameAnimal.text = name;
        if (remainingTime == 0)
        {
            this.remainingTime.text = "Có thể thu hoạch";
        }
        else
        {
            this.remainingTime.text = "Thời gian: Còn " + remainingTime.ToString() + " ngày";
        }
        if (haveEaten)
        {
            this.haveEaten.text = "Đã được ăn";
        }
        else
        {
            this.haveEaten.text = "Chưa được ăn";
        }
        for (int i = 0; i < icon.Length; i++)
        {
            icon[i].SetActive(false);
        }
        switch (name)
        {
            case "White Sheep":
                icon[0].SetActive(true);
                break;
            case "Black Sheep":
                icon[1].SetActive(true);
                break;
            case "Cream Sheep":
                icon[2].SetActive(true);
                break;
            case "White Goat":
                icon[3].SetActive(true);
                break;
            case "Black Goat":
                icon[4].SetActive(true);
                break;
        }
    }
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
