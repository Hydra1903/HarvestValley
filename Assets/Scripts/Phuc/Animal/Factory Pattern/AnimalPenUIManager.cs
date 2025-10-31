using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnimalPenUIManager : MonoBehaviour
{
    private BarnUI barnUI;

    [Header("References")]
    public AnimalPen pen;
    public TMP_Text animalCountText;
    public TMP_Text penInfoText;
    public TMP_Text penQualityText;

    [Header("Panels")]
    public GameObject penInfoPanel;  
    public InfoPanelUI penInfoPanel2;   // <-- đã đổi từ GameObject sang InfoPanelUI
    public GameObject inventoryPanel;
    public GameObject penInfoAnimal;
    public GameObject confirmSellPanel;
    public GameObject panelLevel1;
    public GameObject panelLevel2;

    [Header("Confirm Buttons")]
    public Button yesButton;
    public Button noButton;

    [Header("Animal Cells")]
    public AnimalCellUI[] cells;

    public GameObject[] SpritePen1;
    public GameObject[] SpritePen2;
    public GameObject[] receiveHayBalePen1;
    public GameObject[] receiveHayBalePen2;
    public TextMeshProUGUI textLevelPen1;
    public TextMeshProUGUI textLevelPen2;
    private int lastLevelPen1 = -1;
    private int lastLevelPen2 = -1;
    private int pendingSellIndex = -1;

    private void Start()
    {
        if (pen == null)
            pen = GetComponent<AnimalPen>();

        HideAllPanels();
        RefreshUI();
        UpdateUIPen();
        lastLevelPen1 = Builder.Instance.currentlevelPen1;
        lastLevelPen2 = Builder.Instance.currentlevelPen2;
    }

    private void Update()
    {
        RefreshUI();
        CheckPenLevelChange();
    }

    public void RefreshUI()
    {
        UpdateAnimalCount();
        UpdateAnimalCells();
        UpdateFeedStatus();

        // --- cập nhật panel nếu đang hiển thị con vật ---
        if (penInfoPanel2 != null && penInfoPanel2.CurrentOwner != null)
        {
            penInfoPanel2.gameObject.SetActive(true);
            penInfoPanel2.Show(penInfoPanel2.CurrentOwner.data, penInfoPanel2.CurrentOwner);
        }
    }

    public void UpdateAnimalCount()
    {
        string count = $"{pen.SpawnedAnimalCount} / {pen.MaxAnimals}";

        if (animalCountText != null) animalCountText.text = count;
        if (penInfoText != null) penInfoText.text = count;
    }

    public void UpdateAnimalCells()
    {
        var animals = pen.GetSpawnedAnimals();

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Clear();
            cells[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < animals.Count && i < cells.Length; i++)
        {
            var (animal, data) = animals[i];
            cells[i].gameObject.SetActive(true);
            cells[i].Setup(animal, data, i, pen);
            cells[i].SetIndexNumber(i + 1);
        }
    }

    public void UpdateFeedStatus()
    {
        bool allGood = pen.IsAnyAnimalFed();

        string text = allGood ? "Tốt" : "Xấu";
        Color color = allGood ? Color.green : Color.red;

        var currentLocale = UnityEngine.Localization.Settings.LocalizationSettings.SelectedLocale;
        if (currentLocale != null && currentLocale.Identifier.Code.StartsWith("en"))
        {
            text = allGood ? "Good" : "Bad";
        }

        if (penQualityText != null)
        {
            penQualityText.text = text;
            penQualityText.color = color;
        }
    }


    public void ShowPenInfo(bool show)
    {
        penInfoPanel?.gameObject.SetActive(show);
        inventoryPanel?.SetActive(show);
        penInfoAnimal?.SetActive(show);

        if (show)
        {
            RefreshUI();
        }
    }

    public void ShowInventory(bool show)
    {
        if (inventoryPanel == null) return;

        inventoryPanel.SetActive(show);
        penInfoAnimal.SetActive(show);

        if (show)
        {
            UpdateAnimalCells();
        }
    }

    public void ShowConfirmSell(int cellIndex)
    {
        if (confirmSellPanel == null) return;

        pendingSellIndex = cellIndex;
        confirmSellPanel.SetActive(true);

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(() =>
        {
            pen.SellAnimal(pendingSellIndex);
            confirmSellPanel.SetActive(false);
            pendingSellIndex = -1;
        });

        noButton.onClick.AddListener(() =>
        {
            confirmSellPanel.SetActive(false);
            pendingSellIndex = -1;
        });
    }

    private void HideAllPanels()
    {
        penInfoPanel?.gameObject.SetActive(false);
        inventoryPanel?.SetActive(false);
        confirmSellPanel?.SetActive(false);
        penInfoAnimal?.SetActive(false);
    }

    public void UpdateUIPen()
    {
        if (Builder.Instance.currentlevelPen1 == 1)
        {
            SpritePen1[0].SetActive(true);
            receiveHayBalePen1[0].SetActive(true);
            textLevelPen1.text = "Cấp 1";
        }
        else if (Builder.Instance.currentlevelPen1 == 2)
        {
            SpritePen1[0].SetActive(false);
            receiveHayBalePen1[0].SetActive(false);
            SpritePen1[1].SetActive(true);
            receiveHayBalePen1[1].SetActive(true);
            textLevelPen1.text = "Cấp 2";
        }

        if (Builder.Instance.currentlevelPen2 == 1)
        {
            SpritePen2[0].SetActive(true);
            receiveHayBalePen2[0].SetActive(true);
            textLevelPen2.text = "Cấp 1";
        }
        else if (Builder.Instance.currentlevelPen2 == 2)
        {
            SpritePen2[0].SetActive(false);
            receiveHayBalePen2[0].SetActive(false);
            SpritePen2[1].SetActive(true);
            receiveHayBalePen2[1].SetActive(true);
            textLevelPen2.text = "Cấp 2";
        }
    }
    private void CheckPenLevelChange()
    {
        if (Builder.Instance == null) return;

        bool changed = false;

        if (Builder.Instance.currentlevelPen1 != lastLevelPen1)
        {
            lastLevelPen1 = Builder.Instance.currentlevelPen1;
            changed = true;
        }

        if (Builder.Instance.currentlevelPen2 != lastLevelPen2)
        {
            lastLevelPen2 = Builder.Instance.currentlevelPen2;
            changed = true;
        }

        if (changed)
        {
            pen.UpdateMaxAnimals();
            pen.UpdateActiveHayManager();
            UpdateUIPen();
        }
    }
}
