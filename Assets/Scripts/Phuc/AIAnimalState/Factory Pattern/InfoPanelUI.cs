using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text stateText;
    [SerializeField] Text productText;
    [SerializeField] Text harvestText;
    [SerializeField] Image iconImage;

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
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        currentOwner = null;
        gameObject.SetActive(false);
    }

    public bool IsShowingOwner(AnimalInfo owner) => currentOwner == owner;
}
