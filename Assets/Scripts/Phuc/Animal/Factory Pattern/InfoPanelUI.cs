using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InfoPanelUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI stateText;
    public Image productImage;
    public TextMeshProUGUI harvestText;

    [System.Serializable]
    public class AnimalIcon
    {
        public AnimalTypeed type;   // Loại vật nuôi
        public string variant;      // Màu hoặc biến thể
        public GameObject icon;     // Icon tương ứng
    }

    public List<AnimalIcon> animalIcons = new List<AnimalIcon>();

    public AnimalInfo CurrentOwner { get; private set; }

    public void Show(AnimalData data, AnimalInfo owner)
    {
        CurrentOwner = owner;

        if (data != null)
        {
            nameText.text = data.animalName;
            productImage.sprite = data.item?.icon ?? null;

            // Bật icon dựa vào type + variant
            SetActiveIcon(data.animalType, data.variant);
        }

        // Trạng thái ăn
        var feeding = owner.GetComponent<AnimalFedding>();
        stateText.text = feeding != null
            ? (feeding.GetMealsToday() > 0 ? "Đã được ăn" : "Chưa được ăn")
            : "-";

        // Trạng thái thu hoạch
        var harvestComp = owner.GetComponent<TestingHarvestAnimal>();
        if (harvestComp != null)
        {
            int remainingDays = harvestComp.GetRemainingDaysToHarvest();
            harvestText.text = remainingDays <= 0 ? "Đã có thể thu hoạch " : $"Thời gian còn: {remainingDays} ngày";
        }
        else
        {
            harvestText.text = "-";
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        CurrentOwner = null;

        // Tắt tất cả icon
        foreach (var icon in animalIcons)
        {
            if (icon.icon != null)
                icon.icon.SetActive(false);
        }

        gameObject.SetActive(false);
    }

    private void SetActiveIcon(AnimalTypeed type, string variant)
    {
        // Tắt hết trước
        foreach (var icon in animalIcons)
            if (icon.icon != null)
                icon.icon.SetActive(false);

        // Bật icon đúng type + variant
        foreach (var icon in animalIcons)
        {
            if (icon.icon != null && icon.type == type && icon.variant == variant)
            {
                icon.icon.SetActive(true);
                break; // chỉ bật 1 icon
            }
        }
    }
}
