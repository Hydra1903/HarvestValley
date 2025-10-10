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
    private int cellIndex = -1; // <-- thêm
    public TMP_Text indexText;
    private void Awake()
    {
        Clear();
    }

    public void Setup(GameObject animal, AnimalData data, int index, AnimalPen pen)
    {
        linkedAnimal = animal;
        linkedPen = pen;
        animalData = data;
        cellIndex = index; // <-- gán index t? AnimalPen

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
        if (linkedAnimal != null && linkedPen != null)
        {
            // l?y index c?a cell trong m?ng cells
            int cellIndex = System.Array.IndexOf(linkedPen.cells, this);
            string animalName = animalData != null ? animalData.animalName : "Animal";

            linkedPen.ShowConfirmSell(cellIndex, animalName);
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
