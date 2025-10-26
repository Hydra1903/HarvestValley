using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalCellUI : MonoBehaviour
{
    [Header("UI References")]
    public Image icon;
    public Button sellButton;

    private GameObject linkedAnimal;
    private AnimalPen linkedPen;
    private AnimalData animalData;
    private int cellIndex = -1; 
    public TMP_Text indexText;
    private AnimalPenUIManager uiManager;

    private void Awake()
    {
        Clear();
    }

    public void Setup(GameObject animal, AnimalData data, int index, AnimalPen pen)
    {
        linkedAnimal = animal;
        linkedPen = pen;
        animalData = data;
        cellIndex = index;
        uiManager = pen.GetComponent<AnimalPenUIManager>();
        if (data != null)
        {
            icon.sprite = data.icon;
            icon.enabled = true;
        }

        if (sellButton)
        {
            sellButton.onClick.RemoveAllListeners();
            sellButton.onClick.AddListener(SellAnimal);
            sellButton.gameObject.SetActive(true);
            sellButton.interactable = true;
        }
    }

    private void SellAnimal()
    {
        if (linkedAnimal != null && uiManager != null)
        {
            string animalName = animalData != null ? animalData.animalName : "Animal";
            uiManager.ShowConfirmSell(cellIndex);
        }
    }

    public void Clear()
    {
        linkedAnimal = null;
        linkedPen = null;
        animalData = null;
        cellIndex = -1;
        if (indexText != null)
        {
            indexText.text = "";
        }
        icon.sprite = null;
        icon.enabled = false;

        if (sellButton)
        {
            sellButton.onClick.RemoveAllListeners();
            sellButton.gameObject.SetActive(false);
        }
    }
    public void SetIndexNumber(int number)
    {
        if (indexText != null)
            indexText.text = number.ToString();
    }
}
