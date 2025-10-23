using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text stateText;
    [SerializeField] Text productText;
    [SerializeField] Text harvestText;
    [SerializeField] Image iconImage;
    [SerializeField] Text feedText;

    private AnimalInfo currentOwner;

    public void Show(AnimalData data, AnimalInfo owner)
    {
        currentOwner = owner;
        if (data != null)
        {
            nameText.text = data.animalName;
            productText.text = data.item ? $"San pham: {data.item.itemName}" : "San pham: -";
            iconImage.sprite = data.icon;
        }
        stateText.text = "Trang thai: Binh thuong";
        var harvestComp = owner.GetComponent<TestingHarvestAnimal>();
        var feeding = owner.GetComponent<AnimalFedding>();

        if (harvestComp != null && feeding != null)
        {
            if (feeding.CanHarvest())
                harvestText.text = "Co the thu hoach: OK";
            else
                harvestText.text = "Co the thu hoach: NO";
        }
        else
        {
            harvestText.text = "Co the thu hoach: -";
        }
        if (feeding != null)
        {
            switch (feeding.animalType)
            {
                case AnimalFedding.AnimalType.Sheep:
                    feedText.text = feeding.GetMealsToday() >= 1
                        ? "Feeding Quality: Have Eaten Today"
                        : "Feeding Quality: Haven't Eaten Today";
                    break;

                case AnimalFedding.AnimalType.Goat:
                    int meals = feeding.GetMealsToday();
                    feedText.text = $"Feeding Quality: {meals}/2";
                    break;

                default:
                    feedText.text = "Feeding Quality: Unknown";
                    feedText.color = Color.gray;
                    break;
            }
        }
        else
        {
            feedText.text = "T?nh tr?ng ãn: -";
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        currentOwner = null;
        gameObject.SetActive(false);
    }

    public bool IsShowingOwner(AnimalInfo owner) => currentOwner == owner;
       public void RefreshUI(AnimalInfo owner)
    {
        if (owner != null && owner.data != null)
        {
            Show(owner.data, owner);
        }
    }

}
