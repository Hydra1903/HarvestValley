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
    public GameObject inventoryPanel;
    public GameObject penInfoAnimal;
    public GameObject confirmSellPanel;

    [Header("Confirm Buttons")]
    public Button yesButton;
    public Button noButton;

    [Header("Animal Cells")]
    public AnimalCellUI[] cells;

    private int pendingSellIndex = -1;

    private void Start()
    {
        if (pen == null)
            pen = GetComponent<AnimalPen>();

        HideAllPanels();
        RefreshUI();
    }

    public void RefreshUI()
    {
        UpdateAnimalCount();
        UpdateAnimalCells();
        UpdateFeedStatus();
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
        bool allGood = pen.AreAllAnimalsFed();
        string text = allGood ? "Good" : "Bad";
        Color color = allGood ? Color.green : Color.red;

        if (penQualityText != null)
        {
            penQualityText.text = text;
            penQualityText.color = color;
        }
    }

    public void ShowPenInfo(bool show)
    {
        penInfoPanel?.SetActive(show);
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
        penInfoPanel?.SetActive(false);
        inventoryPanel?.SetActive(false);
        confirmSellPanel?.SetActive(false);
        penInfoAnimal?.SetActive(false);
    }
}
