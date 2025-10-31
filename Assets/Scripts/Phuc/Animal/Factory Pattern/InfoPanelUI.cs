using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;

public class InfoPanelUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI stateText;
    public Image productImage;
    public TextMeshProUGUI harvestText;
    public TextMeshProUGUI productLabelText;
    [System.Serializable]
    public class AnimalIcon
    {
        public AnimalTypeed type;   // Loại vật nuôi (giữ nguyên)
        public string variant;      // Màu hoặc biến thể
        public GameObject icon;     // Icon tương ứng
    }

    public List<AnimalIcon> animalIcons = new List<AnimalIcon>();

    public AnimalInfo CurrentOwner { get; private set; }

    public void Show(AnimalData data, AnimalInfo owner)
    {
        CurrentOwner = owner;

        string currentLang = LocalizationSettings.SelectedLocale != null
            ? LocalizationSettings.SelectedLocale.Identifier.Code
            : "vi";
        bool isEnglish = currentLang.StartsWith("en");

        if (data != null)
        {
            // Map sang AnimalType chính xác dựa trên variant rồi lấy tên hiển thị
            AnimalType mapped = AnimalHelpers.MapToAnimalType(data.animalType, data.variant);
            nameText.text = AnimalLocalization.GetLocalizedName(mapped);
            SetActiveIcon(data.animalType, data.variant);

        }

        var feeding = owner.GetComponent<AnimalFedding>();
        if (feeding != null)
        {
            bool eaten = feeding.GetMealsToday() > 0;
            stateText.text = isEnglish
                ? (eaten ? "Eaten" : "Not eaten")
                : (eaten ? "Đã được ăn" : "Chưa được ăn");
        }
        else stateText.text = "-";

        if (productLabelText != null)
        {
            productLabelText.text = isEnglish ? "Product" : "Sản phẩm";
        }

        var harvestComp = owner.GetComponent<TestingHarvestAnimal>();
        if (harvestComp != null)
        {
            int remainingDays = harvestComp.GetRemainingDaysToHarvest();
            if (remainingDays <= 0)
            {
                harvestText.text = isEnglish ? "Harvestable" : "Đã có thể thu hoạch";
            }
            else
            {
                harvestText.text = isEnglish
                    ? $"Time remaining: {remainingDays} days"
                    : $"Thời gian còn: {remainingDays} ngày";
            }
        }
        else
        {
            harvestText.text = "-";
        }
        if (productImage != null)
        {
            Sprite prodSprite = null;

            var harvestAnimal = owner.GetComponent<TestingHarvestAnimal>();
            if (harvestAnimal != null)
            {
                // Lấy ItemData theo loại động vật
                ItemData item = harvestAnimal.GetItemDataByType();
                if (item != null)
                    prodSprite = item.icon; // icon của sản phẩm
            }

            productImage.sprite = prodSprite;
            productImage.enabled = prodSprite != null;
        }
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        CurrentOwner = null;

        foreach (var icon in animalIcons)
        {
            if (icon.icon != null)
                icon.icon.SetActive(false);

        }
        if (productLabelText != null)
            productLabelText.text = "";
        gameObject.SetActive(false);
    }

    private void SetActiveIcon(AnimalTypeed type, string variant)
    {
        string normalizedVariant = variant.ToLower();

        foreach (var icon in animalIcons)
        {
            if (icon.icon != null && icon.type == type && icon.variant.ToLower() == normalizedVariant)
            {
                icon.icon.SetActive(true);
                break;
            }
        }
    }
}
