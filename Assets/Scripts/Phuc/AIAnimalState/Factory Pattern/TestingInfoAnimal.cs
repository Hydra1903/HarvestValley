using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TestingInfoAnimal : MonoBehaviour
{
    public static TestingInfoAnimal Instance;
    public GameObject AnimalInfoPanel;
    public Text AnimalName;
    public Text AnimalItem;
    public Image iconAnimal;
    public RectTransform canvasRect;
    public Vector2 deviation;//deviation => do lech

    public Sprite BlackSheep;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
        }
    }
    public void ShowAnimalInfo(string name, string Item, Sprite iconAnimals, AnimalTypeed animaltype)
    {
        AnimalInfoPanel.SetActive(true);
        AnimalName.text = name; 
        AnimalItem.text=Item;
        iconAnimal.sprite=iconAnimals;
        switch (animaltype)
        {
            case AnimalTypeed.Sheep:
                iconAnimal.sprite = BlackSheep;
                break;
        }
    }
    public void Hide()
    {
        AnimalInfoPanel.SetActive(false);
    }
}
