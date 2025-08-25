using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalCellUI : MonoBehaviour
{
    [Header("UI References")]
    public Image icon;
    //public TMP_Text nameText;
    //public TMP_Text numberText;
    public Button sellButton;

    private GameObject linkedAnimal;  
    private AnimalPen linkedPen;      
    private AnimalData animalData;
    private void Awake()
    {
        Clear(); // ð?m b?o cell tr?ng lúc start
    }
    public void Setup(GameObject animal, AnimalData data, int index, AnimalPen pen)
    {
        linkedAnimal = animal;
        linkedPen = pen;
        animalData = data;

        if (data != null)
        {
            icon.sprite = data.icon;
            icon.enabled = true;
            //nameText.text = data.animalName;
        }
        //numberText.text = index.ToString();
        if (sellButton)
        {
            sellButton.onClick.RemoveAllListeners();
            sellButton.onClick.AddListener(SellAnimal);
            sellButton.gameObject.SetActive(true);
            sellButton.interactable = true;
        }
    }
    public bool IsEmpty()
    {
        return linkedAnimal == null;
    }
    private void SellAnimal()
    {
        Debug.Log($"[Cell] Sell clicked. Animal={(linkedAnimal ? linkedAnimal.name : "null")}  Pen={(linkedPen ? linkedPen.name : "null")}");
        if (linkedAnimal != null && linkedPen != null)
        {
            linkedPen.SellAnimal(linkedAnimal);  // g?i v? chu?ng ð? x? l?
            Clear();
        }
    }

    public void Clear()
    {
        linkedAnimal = null;
        linkedPen = null;
        animalData = null;

        icon.sprite = null;
        icon.enabled = false;
        //nameText.text = "";
        //numberText.text = "";
        if (sellButton)
        {
            sellButton.onClick.RemoveAllListeners();
            sellButton.gameObject.SetActive(false);
        }
    }
}
