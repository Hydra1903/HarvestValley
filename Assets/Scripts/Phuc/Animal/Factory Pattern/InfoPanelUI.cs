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
    [SerializeField] Text feedText;
    [SerializeField] Text feedDaysText;

    private AnimalInfo currentOwner;

    public void Show(AnimalData data, AnimalInfo owner)
    {
        currentOwner = owner;

        if (data != null)
        {
            nameText.text = data.animalName;
            productText.text = data.item ? $"Sản Phẩm: {data.item.itemName}" : "Sản Phẩm: -";
            iconImage.sprite = data.icon;
        }

        stateText.text = "Trạng Thái: Bình Thường";

        var harvestComp = owner.GetComponent<TestingHarvestAnimal>();
        var feeding = owner.GetComponent<AnimalFedding>();

        // --- Kiểm tra thu hoạch ---
        if (harvestComp != null && feeding != null)
        {
            if (feeding.CanHarvest())
                harvestText.text = "Có thể thu hoạch: Được";
            else
                harvestText.text = "Có thể thu hoạch: Không";
        }
        // --- Tình trạng ăn và số ngày ăn ---
        if (feeding != null)
        {
            switch (feeding.animalTypes)
            {
                case AnimalFedding.FeedingAnimalType.Sheep:
                    feedText.text = feeding.GetMealsToday() >= 1
                        ? "Tình trạng ăn: Đã được ăn hôm nay"
                        : "Tình trạng ăn: Chưa được ăn hôm nay";
                    feedDaysText.text = $"Số ngày đã ăn: {feeding.daysFed}/3"; 
                    break;

                case AnimalFedding.FeedingAnimalType.Goat:
                    int meals = feeding.GetMealsToday();
                    feedText.text = $"Số lượng ăn: {meals}/2";
                    feedDaysText.text = $"Số ngày đã ăn: {feeding.daysFed}/5"; 
                    break;

                default:
                    feedText.text = "Tình trạng ăn: Không rõ";
                    feedText.color = Color.gray;
                    feedDaysText.text = "Số ngày đã ăn: -";
                    break;
            }
        }
        else
        {
            feedText.text = "Tình trạng ăn: -";
            feedDaysText.text = "Số ngày đã ăn: -";
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
